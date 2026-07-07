using System.Text;
using EuropielPos.Data.Services.Sincronizacion;
using EuropielPos.Domain.Reportes;
using EuropielPos.Domain.Sincronizacion;

namespace EuropielPos.Data.Services.Reportes;

/// <summary>
/// Reporte diario y consulta de caja. Port de frmReporteDiario /
/// frmReporteConsultaCaja: el API central arma los datos
/// (GetReporteDiario / GetReporteConsultaCaja) y el cliente los vuelca a un
/// HTML que se abre en el navegador con <c>window.print()</c>.
/// La consulta de caja original imprimía con Crystal (repConsultaCaja.rpt);
/// aquí se usa su variante HTML (GeneraReporteConsultaCajaExcel del original),
/// que muestra la misma información.
/// </summary>
public interface IReporteCentralService
{
    /// <summary>Pide el reporte diario al central, genera el HTML y lo escribe
    /// en la carpeta temp. Devuelve la ruta del archivo.</summary>
    Task<string> GeneraArchivoReporteDiarioAsync(DateTime fechaInicio, DateTime fechaFin, int idUsuario, CancellationToken ct = default);

    /// <summary>Pide la consulta de caja al central, genera el HTML y lo
    /// escribe en la carpeta temp. Devuelve la ruta del archivo.</summary>
    Task<string> GeneraArchivoConsultaCajaAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default);
}

public class ReporteCentralService : IReporteCentralService
{
    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;

    public ReporteCentralService(IClienteApiPos api, ContextoPos contexto)
    {
        _api = api;
        _contexto = contexto;
    }

    public async Task<string> GeneraArchivoReporteDiarioAsync(DateTime fechaInicio, DateTime fechaFin, int idUsuario, CancellationToken ct = default)
    {
        // El formulario original truncaba ambas fechas a medianoche.
        fechaInicio = fechaInicio.Date;
        fechaFin = fechaFin.Date;

        string cuerpo = $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"FechaInicio\": \"{fechaInicio:yyyy-MM-dd HH:mm:ss}\", \"FechaFin\": \"{fechaFin:yyyy-MM-dd HH:mm:ss}\", \"IdUsuario\": \"{idUsuario}\" }}";

        var respuesta = await _api.PostAsync<RespuestaReporteDiario>(
            "/api/europielpos/GetReporteDiario", cuerpo,
            new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque },
            timeoutSegundos: 300, ct: ct);

        var reporte = respuesta?.Value?.ReporteDiario
                      ?? throw new Exception("El servidor no devolvió el reporte diario.");

        string html = GeneraHtmlReporteDiario(reporte, fechaInicio, fechaFin, DateTime.Now);
        return EscribeTemp("ReporteDiario", html);
    }

    public async Task<string> GeneraArchivoConsultaCajaAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default)
    {
        fechaInicio = fechaInicio.Date;
        fechaFin = fechaFin.Date;

        string cuerpo = $"{{ \"IdSucursal\": \"{_contexto.IdSucursal}\", \"FechaInicio\": \"{fechaInicio:yyyy-MM-dd HH:mm:ss}\", \"FechaFin\": \"{fechaFin:yyyy-MM-dd HH:mm:ss}\" }}";

        var respuesta = await _api.PostAsync<RespuestaReporteConsultaCaja>(
            "/api/europielpos/GetReporteConsultaCaja", cuerpo,
            new Dictionary<string, string> { ["ClaveBloque"] = _contexto.ClaveBloque },
            timeoutSegundos: 150, ct: ct);

        var reporte = respuesta?.Value?.ReporteConsultaCaja
                      ?? throw new Exception("El servidor no devolvió la consulta de caja.");

        string html = GeneraHtmlConsultaCaja(reporte, fechaInicio, fechaFin);
        return EscribeTemp("ReporteConsultaCaja", html);
    }

    /// <summary>Escribe el HTML en {app}/temp con el mismo patrón de nombre del
    /// original (que usaba "hhmmss" en 12 horas (sic)).</summary>
    private static string EscribeTemp(string prefijo, string html)
    {
        string carpeta = Path.Combine(AppContext.BaseDirectory, "temp");
        Directory.CreateDirectory(carpeta);

        string ruta = Path.Combine(carpeta, $"{prefijo}_{DateTime.Now:yyyyMMdd_hhmmss}.html");
        File.WriteAllText(ruta, html);
        return ruta;
    }

    // -----------------------------------------------------------------------
    // Generadores HTML (puros, probados por unidad). Ports fieles de
    // reportes.GeneraReporteDiario / GeneraReporteConsultaCajaExcel: mismas
    // tablas, encabezados, formatos y peculiaridades (colspan desfasados,
    // valores sin codificar a HTML, window.print() al final).
    // -----------------------------------------------------------------------

    public static string GeneraHtmlReporteDiario(ReporteDiarioApi rd, DateTime fechaInicio, DateTime fechaFin, DateTime fechaHoy)
    {
        var s = new StringBuilder();
        s.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">\n<html>\n  <head>\n  <meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\">\n  <meta name=\"generator\" content=\"content\">\n  <title></title>\n  </head>\n  <body>\n");
        s.Append("<style type='text/css'>");
        s.Append("body");
        s.Append("{");
        s.Append("font: 10px normal Verdana, Arial, Helvetica, sans-serif;");
        s.Append("}");
        s.Append(EstilosComunes());
        s.Append("table, span {");
        s.Append("font-size: 11px;");
        s.Append("}");
        s.Append("</style>");
        s.Append("<span>Fecha: " + fechaHoy.ToString("dd/MM/yyyy HH:mm:ss") + "</span>");

        foreach (var suc in rd.Sucursales)
        {
            string sucursal = suc.descripcion ?? string.Empty;

            var pagosHoy = rd.PagosHoy.Where(r => r.sucursal == sucursal).ToList();
            var pagosAyer = rd.PagosAyer.Where(r => r.sucursal == sucursal).ToList();
            var pagosManiana = rd.PagosManiana.Where(r => r.sucursal == sucursal).ToList();
            var atrasadaNoPaso = rd.CobranzaAtrasadaNoPaso.Where(r => r.sucursal == sucursal).ToList();
            var citasHoy = rd.CitasHoy.Where(r => r.sucursal == sucursal).ToList();
            var comisiones = rd.ComisionesNoSePagaran.Where(r => r.sucursal == sucursal).ToList();
            var valoracion = rd.ClientesParaValoracion.Where(r => r.sucursal == sucursal).ToList();

            if (pagosHoy.Count == 0 && pagosAyer.Count == 0 && pagosManiana.Count == 0 && atrasadaNoPaso.Count == 0
                && citasHoy.Count == 0 && comisiones.Count == 0 && valoracion.Count == 0)
                continue;

            s.AppendLine("<hr /><h1 align='left'>" + sucursal + "</h1><br />");

            if (pagosHoy.Count > 0)
                SeccionPagos(s, pagosHoy, "Clientes con pagos pendientes, que tienen cita hoy " + fechaHoy.ToString("dd/MM/yyyy"), conClaveAcceso: true);

            if (pagosAyer.Count > 0)
                SeccionPagos(s, pagosAyer, "Clientes con pagos pendientes, que tenian cita ayer y no asistieron.", conClaveAcceso: false);

            if (pagosManiana.Count > 0)
                SeccionPagos(s, pagosManiana, "Clientes con pagos pendientes, que tienen cita mañana.", conClaveAcceso: true);

            if (atrasadaNoPaso.Count > 0)
            {
                s.AppendLine("<h3 align='left'>Cobranza atrasada - AUTOMATICA QUE NO PASO.</h3><br />");
                s.AppendLine("<table width='900px' border='1' cellpadding='1' cellspacing='0'><tr bgcolor='#E5E5E5'>");
                s.AppendLine("<td><b>Ref PROSA</b></td>");
                s.AppendLine("<td><b>Cliente</b></td>");
                s.AppendLine("<td><b>Pago</b></td>");
                s.AppendLine("<td><b>Vencido</b></td>");
                s.AppendLine("<td><b>Tipo Cob</b></td>");
                s.AppendLine("<td><b>Fecha</b></td>");
                s.AppendLine("<td><b>Forma Pago</b></td>");
                s.AppendLine("<td><b>Tarjeta</b></td>");
                s.AppendLine("<td><b>Banco</b></td>");
                s.AppendLine("<td><b>Sucursal</b></td>");
                s.AppendLine("<td><b>No Paso?</b></td>");
                s.AppendLine("<td><b>Comentarios</b></td>");
                s.AppendLine("<td><b>Proxima Cita</b></td>");
                s.AppendLine("</tr>");

                int i = 0;
                foreach (var item in atrasadaNoPaso)
                {
                    i += 1;
                    s.AppendLine("<tr" + Zebra(i) + ">");
                    s.AppendLine("<td></td>");
                    s.AppendLine("<td>" + item.nombre_paciente_1 + "</td>");
                    s.AppendLine("<td>" + item.num_pago + "</td>");
                    s.AppendLine("<td>" + item.monto_pendiente + "</td>");
                    s.AppendLine("<td>" + item.tipo_cobranza + "</td>");
                    s.AppendLine("<td>" + item.fecha_pago.ToString("dd/MM/yyyy") + "</td>");
                    // El original mostraba tarjeta_cvv en la columna "Banco" (sic).
                    s.AppendLine("<td>" + item.forma_pago + "</td>");
                    s.AppendLine("<td>" + item.tarjeta_numero + "</td>");
                    s.AppendLine("<td>" + item.tarjeta_cvv + "</td>");
                    s.AppendLine("<td>" + item.sucursal + "</td>");
                    s.AppendLine("<td>" + (item.no_paso_fecha is null ? "" : item.no_paso_fecha.Value.ToString("dd/MM/yyyy")) + "</td>");
                    s.AppendLine("<td>" + item.comentarios + "</td>");
                    s.AppendLine("<td>" + (item.proxima_cita is null ? "" : item.proxima_cita.Value.ToString("dd/MM/yyyy")) + "</td>");
                    s.AppendLine("</tr>");
                }

                s.AppendLine("</table><br /><br />");
            }

            if (citasHoy.Count > 0)
            {
                s.AppendLine("<h3 align='left'>Citas de hoy.</h3><br />");
                s.AppendLine("<table width='900px' border='1' cellpadding='1' cellspacing='0'><tr bgcolor='#E5E5E5'>");
                s.AppendLine("<td><b>Cliente</b></td>");
                s.AppendLine("<td><b>Hora</b></td>");
                s.AppendLine("<td><b>Telefonos</b></td>");
                s.AppendLine("<td><b>Clave de Acceso</b></td>");
                s.AppendLine("</tr>");

                int i = 0;
                foreach (var item in citasHoy)
                {
                    i += 1;
                    s.AppendLine("<tr" + Zebra(i) + ">");
                    s.AppendLine("<td>" + item.nombre_paciente + "</td>");
                    s.AppendLine("<td>" + item.fecha_inicio.ToString("HH:mm") + "</td>");
                    s.AppendLine("<td>" + item.telefono + "</td>");
                    s.AppendLine("<td>" + item.clave_acceso + "</td>");
                    s.AppendLine("</tr>");
                }

                s.AppendLine("</table><br /><br />");
            }

            if (comisiones.Count > 0)
            {
                s.AppendLine("<h3 align='left'>Comisiones que no se pagaran .</h3><br />");
                s.AppendLine("<table width='900px' border='1' cellpadding='1' cellspacing='0'><tr bgcolor='#E5E5E5'>");
                s.AppendLine("<td><b>Cliente</b></td>");
                s.AppendLine("<td><b>Fecha de Compra</b></td>");
                s.AppendLine("<td><b>Vendedor</b></td>");
                s.AppendLine("<td><b>Fecha</b></td>");
                s.AppendLine("<td><b>Mensaje</b></td>");
                s.AppendLine("</tr>");

                int i = 0;
                foreach (var item in comisiones)
                {
                    i += 1;
                    s.AppendLine("<tr" + Zebra(i) + ">");
                    s.AppendLine("<td>" + item.nombre_paciente + "</td>");
                    s.AppendLine("<td>" + item.fecha_compra.ToString("dd/MM/yyyy") + "</td>");
                    s.AppendLine("<td>" + item.vendedor + "</td>");
                    s.AppendLine("<td>" + item.fecha.ToString("dd/MM/yyyy") + "</td>");
                    s.AppendLine("<td>" + item.mensaje + "</td>");
                    s.AppendLine("</tr>");
                }

                s.AppendLine("</table><br /><br />");
            }

            if (valoracion.Count > 0)
            {
                s.AppendLine("<h3 align='left'>Clientes para valoración .</h3><br />");
                s.AppendLine("<table width='900px' border='1' cellpadding='1' cellspacing='0'><tr bgcolor='#E5E5E5'>");
                s.AppendLine("<td><b>Cliente</b></td>");
                s.AppendLine("<td><b>Hora</b></td>");
                s.AppendLine("<td><b>Telefonos</b></td>");
                s.AppendLine("<td><b>Clave de Acceso</b></td>");
                s.AppendLine("<td><b>Citas Asistidas</b></td>");
                s.AppendLine("</tr>");

                int i = 0;
                foreach (var item in valoracion)
                {
                    i += 1;
                    s.AppendLine("<tr" + Zebra(i) + ">");
                    s.AppendLine("<td>" + item.nombre_paciente + "</td>");
                    s.AppendLine("<td>" + item.fecha_inicio.ToString("HH:mm") + "</td>");
                    s.AppendLine("<td>" + item.telefono + "</td>");
                    s.AppendLine("<td>" + item.clave_acceso + "</td>");
                    s.AppendLine("<td>" + item.citas_asistidas + "</td>");
                    s.AppendLine("</tr>");
                }

                s.AppendLine("</table><br /><br />");
            }
        }

        // -------- Cobranza global (todas las sucursales) --------
        s.Append("<br />");
        s.Append("<hr />");
        s.Append("<hr />");
        s.Append("<table width='1000' border='0'>");
        s.Append("<tr>");
        s.Append("<br />");
        s.Append("<th colspan='4' height='15' valign='middle'  scope='col'><h3 align='left'>Cobranza</h3></th>");
        s.Append("</tr>");

        if (rd.CobranzaNormal.Count > 0)
        {
            s.Append("<tr>");
            s.Append("<td align='left'>");
            s.Append("<table cellspacing='0' rules='all' border='1' style='border-collapse:collapse;'>");
            s.Append("<tbody>");
            s.Append("<tr bgcolor='#E5E5E5'>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Cliente</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Pago</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Vencido</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Tipo Cob</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Fecha</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Forma Pago</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Tarjeta</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Banco</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Sucursal</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>No Paso?</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Telefonos</th>");
            s.Append("</tr>");

            int i = 0;
            foreach (var dr in rd.CobranzaNormal)
            {
                i += 1;
                string bgColor = Zebra(i);

                if (dr.es_negrita == 1)
                    bgColor += " style='font-weight: bold;'";

                s.AppendLine("<tr" + bgColor + ">");
                s.Append("<td class='bodytext_table' >" + dr.nombre_paciente_1 + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.num_pago + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.monto_pendiente.ToString("###,##0.00") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.tipo_cobranza + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.fecha_pago.ToString("dd/MM/yyyy") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.forma_pago + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.tarjeta_numero + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.tarjeta_cvv + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.sucursal + "</td>");
                s.Append("<td class='bodytext_table' >" + (dr.no_paso_fecha is null ? "" : dr.no_paso_fecha.Value.ToString("dd/MM/yyyy")) + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.telefonos + " " + dr.comentarios + "</td>");
                s.Append("</tr>");
            }

            s.Append("</tbody>");
            s.Append("</table>");
            s.Append("</tr>");
            s.Append("</table>");
        }
        else
        {
            s.Append("<strong>No se encontraron registros para la búsqueda especificada</strong>");
        }

        s.Append("<br />");

        if (rd.TotalCobranzaNormal.Count > 0)
        {
            s.Append("<h4>");
            s.Append("<table align='left'>");
            s.Append("<tr>");
            s.Append("<td><b>Total:</b></td>");
            s.Append("<td><b>" + rd.TotalCobranzaNormal[0].total_monto_pendiente.ToString("$ ###,###,##0.00") + "</b></td>");
            s.Append("<td width='150px'>&nbsp;</td>");
            s.Append("<td><b>Total NEGRITAS:</b></td>");
            s.Append("<td><b>" + rd.TotalCobranzaNormal[0].total_negritas.ToString("###,###,##0") + "</b></td>");
            s.Append("</tr>");
            s.Append("</table>");
            s.Append("</h4>");
            s.Append("<br />");
        }

        s.Append("</td>");
        s.Append("</tr>");
        s.Append("</table>");
        s.Append("<script type='text/javascript'>");
        s.Append("window.print();");
        s.Append("</script>");
        s.Append("  </body>\n</html>\n");

        return s.ToString();
    }

    /// <summary>Secciones "pagos hoy/ayer/mañana": misma tabla, sólo cambia el
    /// título y si lleva la columna de clave de acceso.</summary>
    private static void SeccionPagos(StringBuilder s, List<RdPago> filas, string titulo, bool conClaveAcceso)
    {
        s.AppendLine("<h3 align='left'>" + titulo + "</h3><br />");
        s.AppendLine("<table width='900px' border='1' cellpadding='1' cellspacing='0'><tr bgcolor='#E5E5E5'>");
        s.AppendLine("<td><b>R</b></td>");
        s.AppendLine("<td><b>Cliente</b></td>");
        s.AppendLine("<td><b>Hora</b></td>");
        s.AppendLine("<td><b>Vencido</b></td>");
        s.AppendLine("<td><b>Fecha de Pago</b></td>");
        s.AppendLine("<td><b>Telefonos</b></td>");
        s.AppendLine("<td><b>Costo Paq.</b></td>");
        s.AppendLine("<td><b>Pagado</b></td>");
        s.AppendLine("<td><b>Comentarios</b></td>");
        if (conClaveAcceso)
            s.AppendLine("<td><b>Clave acceso</b></td>");
        s.AppendLine("</tr>");

        int i = 0;
        foreach (var item in filas)
        {
            i += 1;
            s.AppendLine("<tr" + Zebra(i) + ">");
            if (item.reagenda > 0)
                s.AppendLine("<td>R-" + item.reagenda + "</td>");
            else
                s.AppendLine("<td></td>");
            s.AppendLine("<td>" + item.nombre_paciente + "</td>");
            s.AppendLine("<td>" + item.fecha_inicio.ToString("HH:mm") + "</td>");
            s.AppendLine("<td>" + item.monto_pendiente + "</td>");
            s.AppendLine("<td>" + item.fecha_pago.ToString("dd/MM/yyyy") + "</td>");
            s.AppendLine("<td>" + item.telefonos + "</td>");
            s.AppendLine("<td>" + item.costo_total + "</td>");
            s.AppendLine("<td>" + item.monto_pagado + "</td>");
            s.AppendLine("<td>" + item.comentarios + "</td>");
            if (conClaveAcceso)
                s.AppendLine("<td>" + item.clave_acceso + "</td>");
            s.AppendLine("</tr>");
        }

        s.AppendLine("</table><br /><br />");
    }

    public static string GeneraHtmlConsultaCaja(ReporteConsultaCajaApi rd, DateTime fechaInicio, DateTime fechaFin)
    {
        var s = new StringBuilder();
        s.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">\n<html>\n  <head>\n  <meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\">\n  <meta name=\"generator\" content=\"content\">\n  <title></title>\n  </head>\n  <body>\n");
        s.Append("<style type='text/css'>");
        s.Append(EstilosComunes());
        s.Append("table {");
        s.Append("font-size: 11px;");
        s.Append("}");
        s.Append("</style>");
        s.Append("<table width='1000' border='0'>");
        s.Append("<tr>");
        s.Append("<th colspan='4' height='40' valign='middle'  scope='col'><h1 align='left'>Consulta de Caja</h1></th>");
        s.Append("</tr>");

        if (rd.Consulta.Count > 0)
        {
            s.Append("<tr>");
            s.Append("<tr>");
            s.Append("<td>");
            s.Append("<table width='60%'>");
            s.Append("<tr>");
            s.Append("<td nowrap><h4>Fecha Inicio:</h4></td>");
            s.Append("<td nowrap><h3>" + fechaInicio.ToString("dd/MM/yyyy") + "</h3>");
            s.Append("</td>");
            s.Append("<td nowrap><h4>&nbsp;&nbsp;&nbsp;&nbsp;Fecha Fin:</h4></td>");
            s.Append("<td nowrap><h3>" + fechaFin.ToString("dd/MM/yyyy") + "</h3>");
            s.Append("</td>");
            s.Append("</tr>");
            s.Append("</table>");
            s.Append("</td>");
            s.Append("</tr>");
            s.Append("<br />");
            s.Append("<table width='1000' border='0'>");
            s.Append("<tr>");
            s.Append("<th height='15' valign='middle'scope='col'><h3 align='left'>Resultados</h3></th>");
            s.Append("</tr>");
            s.Append("<tr>");
            s.Append("<td align='left'>");
            s.Append("<br />");
            s.Append("<table cellspacing='0' rules='all' border='1'>");
            s.Append("<tbody>");
            s.Append("<tr>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Cliente</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Fecha</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Tipo Comprobante</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Cobro</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Sub total</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>IVA</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Total</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Saldo</th>");
            s.Append("<th class='title_table' scope='col' style='color:Black;'>Concepto</th>");
            s.Append("</tr>");

            int i = 0;
            foreach (var dr in rd.Consulta)
            {
                i += 1;
                s.AppendLine("<tr" + Zebra(i) + ">");
                s.Append("<td class='bodytext_table' >" + dr.nombre + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.fecha.ToString("dd-MM-yyyy HH:mm") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.tipo_recibo + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.usuario + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.subtotal.ToString("###,##0.00") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.iva.ToString("###,##0.00") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.total.ToString("###,##0.00") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.saldo.ToString("###,##0.00") + "</td>");
                s.Append("<td class='bodytext_table' >" + dr.comentario + "</td>");
                s.Append("</tr>");
            }

            s.Append("</tbody>");
            s.Append("</table>");
            s.Append("<table width='900'>");
            s.Append("<tr>");
            s.Append("<td>");

            if (rd.TotalDepositar.Count > 0)
            {
                s.Append("<table align='right' id='tblTotalDepositar'>");
                s.Append("<tr>");
                s.Append("<td colspan='2'><b>Total a Depositar</b></td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>T. Credito + T. Debito&nbsp;&nbsp;</td>");
                s.Append("<td align='right'>" + rd.TotalDepositar[0].pago_tctd + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>Netpay</td>");
                s.Append("<td align='right'>" + rd.TotalDepositar[0].pago_netpay + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>Prosa</td>");
                s.Append("<td align='right'>" + rd.TotalDepositar[0].pago_prosa + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>Total</td>");
                s.Append("<td align='right'>" + rd.TotalDepositar[0].total + "</td>");
                s.Append("</tr>");
                s.Append("</table>");
                s.Append("</td>");
            }

            s.Append("<td>");

            if (rd.FormaPago.Count > 0)
            {
                s.Append("<table align='right'>");
                s.Append("<tr>");
                s.Append("<td colspan='2'><b>Ingresos por forma de pago</b></td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>Efectivo</td>");
                s.Append("<td align='right'>" + rd.FormaPago[0].pago_efectivo + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>T. Credito</td>");
                s.Append("<td align='right'>" + rd.FormaPago[0].pago_tc + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>T. Debito</td>");
                s.Append("<td align='right'>" + rd.FormaPago[0].pago_td + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>Sin Identificar</td>");
                s.Append("<td align='right'>" + rd.FormaPago[0].sin_identificar + "</td>");
                s.Append("</tr>");
                s.Append("<tr>");
                s.Append("<td>Total Pagado</td>");
                s.Append("<td align='right'>" + rd.FormaPago[0].total_pagado + "</td>");
                s.Append("</tr>");
                s.Append("</table>");
            }

            s.Append("</td>");
            s.Append("<td>");
            s.Append("</td>");
            s.Append("</tr>");
            s.Append("</table>");
        }
        else
        {
            s.Append("<strong>No se encontraron registros para la búsqueda especificada</strong>");
        }

        s.Append("<br />");
        s.Append("</td>");
        s.Append("</tr>");
        s.Append("</table>");
        s.Append("<script type='text/javascript'>");
        s.Append("window.print();");
        s.Append("</script>");
        s.Append("  </body>\n</html>\n");

        return s.ToString();
    }

    private static string Zebra(int i) => i % 2 == 0 ? " bgcolor='#E5E5E5'" : "";

    private static string EstilosComunes()
    {
        var s = new StringBuilder();
        s.Append("h1, h2, h3, h4, h5, h6 {");
        s.Append("font-family: Arial, Helvetica, sans-serif;");
        s.Append("}");
        s.Append("h1 {");
        s.Append("font-size: 26px;");
        s.Append("letter-spacing: -1px;");
        s.Append("}");
        s.Append("h3 {");
        s.Append("font-size: 16px;");
        s.Append("}");
        s.Append("h4 {");
        s.Append("font-size: 11px;");
        s.Append("}");
        s.Append(".title_table {");
        s.Append("font-size: 11px;");
        s.Append("color: #FFFFFF;");
        s.Append("padding: 2px 5px 2px 5px;");
        s.Append("}");
        s.Append(".bodytext_table {");
        s.Append("font-size: 11px;");
        s.Append("line-height: 18px;");
        s.Append("padding: 2px 2px 2px 2px;");
        s.Append("}");
        return s.ToString();
    }
}
