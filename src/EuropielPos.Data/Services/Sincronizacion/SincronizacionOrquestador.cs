using System.Text;
using EuropielPos.Data.Entities;
using EuropielPos.Data.Services;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Port de los orquestadores de <c>modInterfaz.vb</c>: ProcesoInterfaz (el
/// ciclo maestro), ProcesoInterfazCitas, ProcesoInterfazPaciente,
/// ProcesoCargaInicial, SincronizarAgenda y EnvioCitaOnLine.
/// Cada etapa registra su corrida en <c>log_interfaz</c> y los errores en
/// <c>log_errores</c>, sin abortar el resto del ciclo (igual que el original).
/// </summary>
public interface ISincronizacionOrquestador
{
    /// <summary>Progreso para la UI (el RaiseEvent MensajeTerminoEnvio del original).</summary>
    event Action<string>? MensajeTerminoEnvio;

    /// <summary>Progreso de la carga inicial (mensaje, terminó).</summary>
    event Action<string, bool>? MensajeAvance;

    Task ProcesoInterfazAsync(CancellationToken ct = default);

    Task ProcesoInterfazCitasAsync(Func<Paquete, Task>? imprimirContrato = null, CancellationToken ct = default);

    Task ProcesoInterfazPacienteAsync(CancellationToken ct = default);

    Task ProcesoCargaInicialAsync(CancellationToken ct = default);

    Task<string> SincronizarAgendaAsync(CancellationToken ct = default);

    Task EnvioCitaOnLineAsync(int idCita, CancellationToken ct = default);
}

public class SincronizacionOrquestador : ISincronizacionOrquestador
{
    private readonly PosDbContext _db;
    private readonly IClienteApiPos _api;
    private readonly ContextoPos _contexto;
    private readonly ILogInterfazService _log;
    private readonly InterfazTransaccionesService _transacciones;
    private readonly IInterfazEnvioService _envio;
    private readonly IPaqueteService _paquete;
    private readonly IPaqueteValidacionesService _paqueteValidaciones;
    private readonly IPagoCajaService _pagoCaja;
    private readonly IRespuestaNetpayService _netpay;
    private readonly IPacienteService _paciente;
    private readonly IRequerimientoService _requerimiento;
    private readonly IGioService _gio;
    private readonly ICitaService _cita;
    private readonly IPaqueteEnvioService _paqueteEnvio;

    public event Action<string>? MensajeTerminoEnvio;

    public event Action<string, bool>? MensajeAvance;

    public SincronizacionOrquestador(PosDbContext db, IClienteApiPos api, ContextoPos contexto,
        ILogInterfazService log, InterfazTransaccionesService transacciones, IInterfazEnvioService envio,
        IPaqueteService paquete, IPaqueteValidacionesService paqueteValidaciones, IPagoCajaService pagoCaja,
        IRespuestaNetpayService netpay, IPacienteService paciente, IRequerimientoService requerimiento,
        IGioService gio, ICitaService cita, IPaqueteEnvioService paqueteEnvio)
    {
        _db = db;
        _api = api;
        _contexto = contexto;
        _log = log;
        _transacciones = transacciones;
        _envio = envio;
        _paquete = paquete;
        _paqueteValidaciones = paqueteValidaciones;
        _pagoCaja = pagoCaja;
        _netpay = netpay;
        _paciente = paciente;
        _requerimiento = requerimiento;
        _gio = gio;
        _cita = cita;
        _paqueteEnvio = paqueteEnvio;
    }

    public async Task ProcesoInterfazAsync(CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
            {
                string sinConexion = $"Estatus: No hay conexión con el servidor, Ultimo intento:  {DateTime.Now:dd/MM/yyyy HH:mm}";
                await GuardaLogErrorAsync("CheckForInternetConnection", sinConexion, ct);
                MensajeTerminoEnvio?.Invoke(sinConexion);
                return;
            }

            await GuardaLogErrorAsync("Inicio Interface", $"Inicia Interface: {DateTime.Now:dd/MM/yyyy HH:mm}", ct);

            // ---- Descargas (cada una con su bitácora, sin abortar el ciclo) ----
            await EtapaRecuperaAsync("recupera_paciente", () => _transacciones.GetPacientesAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_paquete", () => _transacciones.GetPaquetesAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_paquete_servicio", () => _transacciones.GetPaqueteServicioAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_requerimiento", () => _transacciones.GetRequerimientosAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_gio", () => _transacciones.GetGiosAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_gio_asistencia", () => _transacciones.GetGiosAsistenciaAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_maquinas_laser_detalle", () => _transacciones.GetMaquinasLaserDetalleAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_parametros_general", () => _transacciones.GetParametrosGeneralAsync(ct), ct);
            await EtapaRecuperaAsync("recupera_anticipos_minimos",
                () => _transacciones.GetConfigAnticiposMinimosAsync(int.Parse(_contexto.ClaveBloque), _contexto.IdSucursal, ct), ct);

            // ---- Envíos pendientes ----
            var paquetes = await _paqueteEnvio.RecuperaPaquetesAEnviarAsync(ct);
            var pagos = await _pagoCaja.RecuperaPagoCajaAEnviarAsync(ct);
            var respuestasNetpay = await _netpay.RecuperaRespuestaNetpayAEnviarAsync(ct);

            int porEnviar = paquetes.Count + pagos.Count + respuestasNetpay.Count;
            ReportaConectado(porEnviar);

            var msgError = new StringBuilder();

            porEnviar = await EtapaEnviaAsync("envio_paquete", "Envio Paquetes: ", msgError, paquetes, porEnviar,
                p => _envio.SavePaqueteAsync(p, ct),
                p => _paquete.ActualizaFechaInterfazPaqueteAsync(p.IdLocal, DateTime.Now, ct), ct);

            porEnviar = await EtapaEnviaAsync("envio_pago", "Envio Pagos: ", msgError, pagos, porEnviar,
                p => _envio.SavePagoAsync(p, ct),
                p => _pagoCaja.ActualizaFechaInterfazPagoCajaAsync(p.IdLocal, DateTime.Now, ct), ct);

            porEnviar = await EtapaEnviaAsync("envio_respuesta_netpay", "Envio Respuesta Netpay: ", msgError, respuestasNetpay, porEnviar,
                r => _envio.SaveRespuestaNetpayAsync(r, ct),
                r => _netpay.ActualizaFechaInterfazRespuestaNetpayAsync(r.Id, DateTime.Now, ct), ct);

            var pacientes = await _paciente.RecuperaPacienteAEnviarAsync(ct);
            porEnviar = await EtapaEnviaAsync("envio_paciente", "Envio Pacientes: ", msgError, pacientes, porEnviar,
                p => _envio.SavePacienteAsync(p.IdLocal, ct),
                p => _paquete.ActualizaFechaInterfazPacienteAsync(p.IdLocal, DateTime.Now, ct), ct);

            ReportaEstadoFinal(msgError, await _db.Paciente.CountAsync(x => x.FechaInterfaz == null, ct));

            var requerimientos = await _requerimiento.RecuperaRequerimientoAEnviarAsync(ct);
            porEnviar = await EtapaEnviaAsync("envio_requerimiento", "Envio Requerimientos: ", msgError, requerimientos, porEnviar,
                r => _envio.SaveRequerimientoAsync(r.IdLocal, ct),
                r => _requerimiento.ActualizaFechaInterfazRequerimientoAsync(r.IdLocal, DateTime.Now, ct), ct);

            ReportaEstadoFinal(msgError, await _db.Requerimiento.CountAsync(x => x.FechaInterfaz == null, ct));

            var gios = await _gio.RecuperaGioAEnviarAsync(ct);
            porEnviar = await EtapaEnviaAsync("envio_gio", "Envio GIO: ", msgError, gios, porEnviar,
                g => _envio.SaveGioAsync(g.IdLocal, ct),
                g => _gio.ActualizaFechaInterfazGioAsync(g.IdLocal, DateTime.Now, ct), ct);

            ReportaEstadoFinal(msgError, await _db.Gio.CountAsync(x => x.FechaInterfaz == null, ct));

            var asistencias = await _gio.RecuperaGioAsistenciaAEnviarAsync(ct);
            porEnviar = await EtapaEnviaAsync("envio_gio_asistencia", "Envio GIO Asistencia: ", msgError, asistencias, porEnviar,
                a => _envio.SaveGioAsistenciaAsync(a.IdLocal, ct),
                a => _gio.ActualizaFechaInterfazGioAsistenciaAsync(a.IdLocal, DateTime.Now, ct), ct);

            ReportaEstadoFinal(msgError, await _db.GioAsistencia.CountAsync(x => x.FechaInterfaz == null, ct));
        }
        catch (Exception ex)
        {
            MensajeTerminoEnvio?.Invoke("Estatus: Error: " + ex.Message);
        }
    }

    public async Task ProcesoInterfazCitasAsync(Func<Paquete, Task>? imprimirContrato = null, CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
            {
                MensajeTerminoEnvio?.Invoke($"Estatus: No hay conexión con el servidor, Ultimo intento:  {DateTime.Now:dd/MM/yyyy HH:mm}");
                return;
            }

            var msgError = new StringBuilder();
            var msgErrorCitas = new StringBuilder();

            await EtapaRecuperaAsync("recupera_cita", () => _transacciones.GetCitasAsync(ct), ct);

            // ---- Envío de citas (con la semántica OK|estado|id_cita del servidor) ----
            var citas = await _cita.RecuperaCitasAEnviarAsync(ct);
            int porEnviar = citas.Count;
            ReportaConectado(porEnviar);

            int idLog = 0;
            try
            {
                idLog = await _log.GuardaInicioLogInterfazAsync("envio_cita", DateTime.Now, ct);

                foreach (var c in citas)
                {
                    string resultado = await _envio.SaveCitaAsync(c, ct);
                    string[] mensajes = resultado.Split('|');

                    if (mensajes[0] == "OK")
                    {
                        await _cita.ActualizaFechaInterfazCitaAsync(c.IdLocal, DateTime.Now, ct);

                        if (mensajes.Length > 1 && mensajes[1] == "OK")
                        {
                            await _cita.ActualizaRespuestaEnvioCitaAsync(c.IdLocal, "C", string.Empty, Convert.ToInt32(mensajes[2]), ct);
                        }
                        else if (mensajes.Length > 1 && mensajes[1] != "OK"
                                 && mensajes[1] != "Paquete pendiente de procesar"
                                 && mensajes[1] != "Paciente pendiente de procesar")
                        {
                            await _cita.ActualizaRespuestaEnvioCitaAsync(c.IdLocal, "I", mensajes[1], 0, ct);
                        }
                    }
                    else
                    {
                        msgErrorCitas.Append(resultado).Append(", ");
                    }

                    ReportaConectado(--porEnviar);
                }

                if (msgErrorCitas.Length > 0)
                    msgError.Append("Envio Citas: ").Append(msgErrorCitas);

                await _log.GuardaFinLogInterfazAsync(DateTime.Now, ResultadoDe(msgErrorCitas), null, idLog, ct);
            }
            catch (Exception ex)
            {
                await GuardaLogErrorAsync("envio_cita", ex.Message, ct);
                throw;
            }

            await EtapaRecuperaAsync("recupera_cita_borrada", () => _transacciones.GetCitasBorradasAsync(ct), ct);

            // ---- Reimpresión de contratos: requiere impresión (Crystal/PDF).
            // Hasta que exista el módulo de impresión, solo se procesa si el
            // llamador provee el callback (así no se notifica al servidor sin imprimir).
            if (imprimirContrato is not null)
            {
                var reimpresiones = await _paqueteValidaciones.RecuperaReimpresionContratoAsync(ct);

                foreach (System.Data.DataRow fila in reimpresiones.Rows)
                {
                    int idCita = Convert.ToInt32(fila["id_cita"]);
                    int idPaquete = Convert.ToInt32(fila["reimprimir_contrato"]);

                    var paq = await _db.Paquete.AsNoTracking().FirstOrDefaultAsync(x => x.IdPaquete == idPaquete, ct);
                    if (paq is null)
                        continue;

                    await imprimirContrato(paq);

                    string resultado = await _envio.SavePaqueteReimpresionContratoAsync(idPaquete, idCita, ct);

                    if (resultado == "OK")
                        await _cita.ActualizaReimpresionContratoAsync(idCita, ct);
                    else
                        msgErrorCitas.Append(resultado).Append(", ");
                }
            }

            ReportaEstadoFinal(msgError,
                await _db.Cita.CountAsync(x => x.FechaInterfaz == null && x.IdPadreLocal == 0, ct));
        }
        catch (Exception ex)
        {
            MensajeTerminoEnvio?.Invoke("Estatus: Error: " + ex.Message);
        }
    }

    /// <remarks>En el original el envío estaba comentado: solo registra la
    /// corrida con "OK". Se conserva tal cual.</remarks>
    public async Task ProcesoInterfazPacienteAsync(CancellationToken ct = default)
    {
        try
        {
            if (!await _api.HayConexionAsync(ct))
            {
                MensajeTerminoEnvio?.Invoke($"Estatus: No hay conexión con el servidor, Ultimo intento:  {DateTime.Now:dd/MM/yyyy HH:mm}");
                return;
            }

            int idLog = await _log.GuardaInicioLogInterfazAsync("envio_paciente", DateTime.Now, ct);
            await _log.GuardaFinLogInterfazAsync(DateTime.Now, "OK", null, idLog, ct);
        }
        catch
        {
            // el original tragaba cualquier error aquí
        }
    }

    public async Task ProcesoCargaInicialAsync(CancellationToken ct = default)
    {
        if (!await _api.HayConexionAsync(ct))
            throw new Exception("No hay conexión con el servidor.");

        MensajeAvance?.Invoke("- Descargando datos...", false);

        await CargaInicialAsync("carga_inicial_paquete", () => _transacciones.GetPaquetesCargaInicialAsync(ct), ct);
        await CargaInicialAsync("carga_inicial_paciente", () => _transacciones.GetPacientesCargaInicialAsync(ct), ct);
        await CargaInicialAsync("carga_inicial_paquete_servicio", () => _transacciones.GetPaqueteServicioCargaInicialAsync(ct), ct);
        await CargaInicialAsync("carga_inicial_cita", () => _transacciones.GetCitasCargaInicialAsync(ct), ct);

        MensajeAvance?.Invoke("- Guardando datos: 100%", false);
        MensajeAvance?.Invoke("Procesando datos...", false);

        try
        {
            await _db.Database.ExecuteSqlAsync($"EXEC procesa_carga_inicial", ct);
            MensajeAvance?.Invoke("Proceso finalizado", true);
        }
        catch (Exception ex)
        {
            await GuardaLogErrorAsync("procesa_carga_inicial", ex.Message, ct);
            throw;
        }
    }

    public async Task<string> SincronizarAgendaAsync(CancellationToken ct = default)
    {
        int idLog = 0;
        try
        {
            idLog = await _log.GuardaInicioLogInterfazAsync("recupera_cita_a_futuro", DateTime.Now, ct);

            string resultado = await _transacciones.GetCitasAFuturoAsync(borrarCitasAFuturo: true, ct);
            (string estado, DateTime? fechaServidor) = SeparaResultado(resultado);

            await _log.GuardaFinLogInterfazAsync(DateTime.Now, estado, fechaServidor, idLog, ct);
            return "OK";
        }
        catch (Exception ex)
        {
            await GuardaLogErrorAsync("recupera_cita_a_futuro", ex.Message, ct);
            throw new Exception(ex.Message, ex);
        }
    }

    /// <remarks>Agenda en línea: si el envío falla, la cita se marca con
    /// envio_online para que la recoja el ciclo normal después.</remarks>
    public async Task EnvioCitaOnLineAsync(int idCita, CancellationToken ct = default)
    {
        var c = await _cita.RecuperaCitasAEnviarPorIdAsync(idCita, ct)
                ?? throw new Exception("La cita no está disponible para envío.");

        string resultado = await _envio.SaveCitaAsync(c, ct);
        string[] mensajes = resultado.Split('|');

        if (mensajes[0] == "OK")
        {
            await _cita.ActualizaFechaInterfazCitaAsync(c.IdLocal, DateTime.Now, ct);

            if (mensajes.Length > 1 && mensajes[1] == "OK")
            {
                await _cita.ActualizaRespuestaEnvioCitaAsync(c.IdLocal, "C", string.Empty, Convert.ToInt32(mensajes[2]), ct);
            }
            else if (mensajes.Length > 1 && mensajes[1] != "OK"
                     && mensajes[1] != "Paquete pendiente de procesar"
                     && mensajes[1] != "Paciente pendiente de procesar")
            {
                await _cita.ActualizaRespuestaEnvioCitaAsync(c.IdLocal, "I", mensajes[1], 0, ct);
                throw new Exception("Error al agendar: " + mensajes[1]);
            }
        }
        else
        {
            await _cita.ActualizaCitaEnvioOnlineAsync(c.IdLocal, ct);
        }
    }

    // ----- Etapas comunes -----

    /// <summary>Etapa de descarga: bitácora + llamada + parseo OK|fecha.
    /// Los errores se registran en log_errores sin abortar el ciclo.</summary>
    private async Task EtapaRecuperaAsync(string tipo, Func<Task<string>> descarga, CancellationToken ct)
    {
        try
        {
            int idLog = await _log.GuardaInicioLogInterfazAsync(tipo, DateTime.Now, ct);

            string resultado = await descarga();
            (string estado, DateTime? fechaServidor) = SeparaResultado(resultado);

            await _log.GuardaFinLogInterfazAsync(DateTime.Now, estado, fechaServidor, idLog, ct);
        }
        catch (Exception ex)
        {
            await GuardaLogErrorAsync(tipo, ex.Message, ct);
        }
    }

    /// <summary>Etapa de envío: recorre pendientes, marca fecha_interfaz en
    /// los exitosos y acumula errores; la corrida queda en log_interfaz.</summary>
    private async Task<int> EtapaEnviaAsync<T>(string tipo, string prefijoError, StringBuilder msgError,
        IReadOnlyList<T> pendientes, int porEnviar,
        Func<T, Task<string>> enviar, Func<T, Task> marcarEnviado, CancellationToken ct)
    {
        var errores = new StringBuilder();

        try
        {
            int idLog = await _log.GuardaInicioLogInterfazAsync(tipo, DateTime.Now, ct);

            foreach (var item in pendientes)
            {
                string resultado = await enviar(item);

                if (resultado == "OK")
                    await marcarEnviado(item);
                else
                    errores.Append(resultado).Append(", ");

                ReportaConectado(--porEnviar);
            }

            if (errores.Length > 0)
                msgError.Append(prefijoError).Append(errores);

            await _log.GuardaFinLogInterfazAsync(DateTime.Now, ResultadoDe(errores), null, idLog, ct);
        }
        catch (Exception ex)
        {
            await GuardaLogErrorAsync(tipo, ex.Message, ct);
        }

        return porEnviar;
    }

    private async Task CargaInicialAsync(string tipo, Func<Task<string>> descarga, CancellationToken ct)
    {
        try
        {
            var log = new LogInterfaz { Tipo = tipo, FechaInicio = DateTime.Now };
            _db.LogInterfaz.Add(log);
            await _db.SaveChangesAsync(ct);

            string resultado = await descarga();

            log.FechaFin = DateTime.Now;
            log.Mensaje = resultado;
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            await GuardaLogErrorAsync(tipo, ex.Message, ct);
            throw;
        }
    }

    /// <summary>Port de <c>manejo_errores.GuardaLogError</c>: registra en
    /// log_errores y nunca lanza.</summary>
    private async Task GuardaLogErrorAsync(string tipo, string mensaje, CancellationToken ct)
    {
        try
        {
            await _db.Database.ExecuteSqlAsync(
                $"INSERT INTO log_errores (tipo, mensaje, fecha) VALUES ({tipo}, {mensaje}, getdate())", ct);
        }
        catch
        {
            // igual que el original: el log de errores nunca truena el proceso
        }
    }

    private static (string Estado, DateTime? FechaServidor) SeparaResultado(string resultado)
    {
        if (!resultado.StartsWith("OK"))
            return (resultado, null);

        string[] valores = resultado.Split('|');
        DateTime? fecha = valores.Length > 1 && DateTime.TryParse(valores[1], out var f) ? f : null;

        return (valores[0], fecha);
    }

    private static string ResultadoDe(StringBuilder errores) =>
        errores.Length == 0 ? "OK" : errores.ToString();

    private void ReportaConectado(int porEnviar) =>
        MensajeTerminoEnvio?.Invoke($"Estatus: Conectado, Registros por enviar: {porEnviar}, Última conexión:  {DateTime.Now:dd/MM/yyyy HH:mm}");

    private void ReportaEstadoFinal(StringBuilder msgError, int pendientes)
    {
        if (msgError.Length > 0)
        {
            string texto = msgError.ToString();
            MensajeTerminoEnvio?.Invoke($"Estatus: Error envio, Última conexión: {DateTime.Now:dd/MM/yyyy HH:mm}, Error: {texto[..^1]}");
        }
        else
        {
            ReportaConectado(pendientes);
        }
    }
}
