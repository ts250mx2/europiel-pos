using System.Globalization;
using System.Text.Json;
using EuropielPos.Data.Services.Reportes;
using EuropielPos.Domain.Reportes;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de los generadores HTML del reporte diario y la consulta de caja
/// (lógica pura, sin API ni impresora). Los formatos numéricos dependen de la
/// cultura, así que se fija es-MX como en las cajas reales.
/// </summary>
public class ReporteCentralTests
{
    public ReporteCentralTests()
    {
        CultureInfo.CurrentCulture = new CultureInfo("es-MX");
    }

    private static ReporteDiarioApi ReporteDiario() => new()
    {
        Sucursales = [new RdSucursal { id_sucursal = 5, descripcion = "MTY Centro" }],
        PagosHoy =
        [
            new RdPago
            {
                id_paquete = 1, nombre_paciente = "Laura Treviño", sucursal = "MTY Centro",
                fecha_inicio = new DateTime(2026, 7, 6, 9, 30, 0), monto_pendiente = 850.50m,
                fecha_pago = new DateTime(2026, 7, 1), telefonos = "8112345678",
                costo_total = 12000, monto_pagado = 4000, reagenda = 2, clave_acceso = "AB12",
            },
        ],
        CitasHoy =
        [
            new RdCitaHoy
            {
                nombre_paciente = "Karla Soto", sucursal = "MTY Centro",
                fecha_inicio = new DateTime(2026, 7, 6, 11, 0, 0), telefono = "8187654321", clave_acceso = "XY99",
            },
        ],
        CobranzaNormal =
        [
            new RdCobranzaNormal
            {
                nombre_paciente_1 = "Ana Pérez", num_pago = 3, monto_pendiente = 1500m,
                tipo_cobranza = "AUTO", fecha_pago = new DateTime(2026, 7, 6),
                forma_pago = "TC", tarjeta_numero = "****1234", sucursal = "MTY Centro",
                telefonos = "8100000000", comentarios = "pendiente", es_negrita = 1,
            },
        ],
        TotalCobranzaNormal = [new RdTotalCobranzaNormal { total_monto_pendiente = 1500m, total_negritas = 1 }],
    };

    [Fact]
    public void ReporteDiario_ContieneSeccionesYDatos()
    {
        string html = ReporteCentralService.GeneraHtmlReporteDiario(
            ReporteDiario(), new DateTime(2026, 7, 6), new DateTime(2026, 7, 6), new DateTime(2026, 7, 6, 8, 0, 0));

        Assert.Contains("<h1 align='left'>MTY Centro</h1>", html);
        Assert.Contains("Clientes con pagos pendientes, que tienen cita hoy 06/07/2026", html);
        Assert.Contains("Laura Treviño", html);
        Assert.Contains("<td>R-2</td>", html); // columna de reagenda
        Assert.Contains("<td>09:30</td>", html);
        Assert.Contains("Citas de hoy.", html);
        Assert.Contains("Karla Soto", html);
        Assert.Contains(">Cobranza</h3>", html);
        Assert.Contains("Ana Pérez", html);
        Assert.Contains("1,500.00", html);
        Assert.Contains("style='font-weight: bold;'", html); // es_negrita = 1
        Assert.Contains("$ 1,500.00", html); // total cobranza
        Assert.Contains("window.print();", html);
    }

    [Fact]
    public void ReporteDiario_SucursalSinDatosNoImprimeEncabezado()
    {
        var rd = ReporteDiario();
        rd.Sucursales.Add(new RdSucursal { id_sucursal = 9, descripcion = "GDL Sur" });

        string html = ReporteCentralService.GeneraHtmlReporteDiario(
            rd, new DateTime(2026, 7, 6), new DateTime(2026, 7, 6), new DateTime(2026, 7, 6, 8, 0, 0));

        Assert.DoesNotContain("GDL Sur", html);
    }

    [Fact]
    public void ReporteDiario_SinCobranzaMuestraMensajeVacio()
    {
        var rd = new ReporteDiarioApi();

        string html = ReporteCentralService.GeneraHtmlReporteDiario(
            rd, new DateTime(2026, 7, 6), new DateTime(2026, 7, 6), new DateTime(2026, 7, 6, 8, 0, 0));

        Assert.Contains("No se encontraron registros para la búsqueda especificada", html);
    }

    private static ReporteConsultaCajaApi ConsultaCaja() => new()
    {
        Consulta =
        [
            new RcConsulta
            {
                id = 1, id_pago = 100, fecha = new DateTime(2026, 7, 6, 13, 45, 0),
                tipo_recibo = "Recibo", nombre = "María López", subtotal = 1293.10m, iva = 206.90m,
                total = 1500m, saldo = 8500m, usuario = "cajera1", comentario = "abono a paquete",
            },
        ],
        FormaPago = [new RcFormaPago { pago_efectivo = 500m, pago_tc = 1000m, pago_td = 0m, sin_identificar = 0m, total_pagado = 1500m }],
        TotalDepositar = [new RcTotalDepositar { pago_tctd = 1000m, pago_netpay = 0m, pago_prosa = 0m, total = 1000m }],
    };

    [Fact]
    public void ConsultaCaja_ContieneResultadosYTotales()
    {
        string html = ReporteCentralService.GeneraHtmlConsultaCaja(
            ConsultaCaja(), new DateTime(2026, 7, 1), new DateTime(2026, 7, 6));

        Assert.Contains("Consulta de Caja", html);
        Assert.Contains("01/07/2026", html);
        Assert.Contains("06/07/2026", html);
        Assert.Contains("María López", html);
        Assert.Contains("06-07-2026 13:45", html);
        Assert.Contains("1,293.10", html);
        Assert.Contains("Total a Depositar", html);
        Assert.Contains("Ingresos por forma de pago", html);
        Assert.Contains("window.print();", html);
    }

    [Fact]
    public void ConsultaCaja_SinRegistrosMuestraMensajeVacio()
    {
        string html = ReporteCentralService.GeneraHtmlConsultaCaja(
            new ReporteConsultaCajaApi(), new DateTime(2026, 7, 1), new DateTime(2026, 7, 6));

        Assert.Contains("No se encontraron registros para la búsqueda especificada", html);
        Assert.DoesNotContain("Total a Depositar", html);
    }

    [Fact]
    public void Dtos_DeserializanFechasIsoYLegado()
    {
        // El API central puede mandar fechas ISO o /Date(ms)/ (JavaScriptSerializer).
        string json = """
        {
          "Message": "OK",
          "Value": {
            "ReporteConsultaCaja": {
              "Consulta": [
                { "id": 1, "id_pago": 2, "fecha": "/Date(1783100700000)/", "nombre": "X",
                  "subtotal": 100.0, "iva": 16.0, "total": 116.0, "saldo": 0.0 }
              ],
              "Totales": [], "FormaPago": [], "TotalDepositar": [], "Terminal": [], "TerminalResumen": []
            },
            "fecha_servidor": "2026-07-06T08:00:00"
          }
        }
        """;

        var r = JsonSerializer.Deserialize<RespuestaReporteConsultaCaja>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(r?.Value?.ReporteConsultaCaja);
        Assert.Equal(new DateTime(2026, 7, 6, 8, 0, 0), r!.Value!.fecha_servidor);
        Assert.Equal(2026, r.Value.ReporteConsultaCaja!.Consulta[0].fecha.Year);
    }
}
