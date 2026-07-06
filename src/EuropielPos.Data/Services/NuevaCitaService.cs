using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>Área pendiente de agendar (renglón del grid de frmCitaDetalle).</summary>
public class AreaAgendable
{
    public int NoPaquete { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public int IdServicio { get; set; }

    public int? IdPaciente { get; set; }

    public int IdPacienteLocal { get; set; }

    public int? IdPaquete { get; set; }

    public int IdPaqueteLocal { get; set; }

    public decimal Duracion { get; set; }

    public string Estatus { get; set; } = string.Empty;

    public decimal DuracionServicio { get; set; }

    public bool Seleccionada { get; set; }
}

/// <summary>Configuración de tiempos de la sucursal + estado del paciente.</summary>
public class ConfigAgendado
{
    public bool PacienteBloqueado { get; set; }

    public bool ClienteTiene1erCita { get; set; } = true;

    public int TiempoAdicionalPrimeraCita { get; set; }

    public int TiempoMinimoPrimeraCita { get; set; }

    public bool SesionExtendida { get; set; }

    public int AumentoSesionExtendida { get; set; }

    public decimal AumentoPorcSesionExtendida { get; set; }

    public int TiempoMinimoSesionExtendida { get; set; }
}

/// <summary>
/// Port del flujo de alta de citas de <c>frmCitaDetalle.vb</c>: áreas
/// pendientes del paciente, cálculo de duración y guardado con las
/// validaciones de los SPs del original.
/// </summary>
public interface INuevaCitaService
{
    Task<(ConfigAgendado Config, List<AreaAgendable> Areas)> AreasDelPacienteAsync(int idPacienteLocal, int idSucursalAgendaExterna, CancellationToken ct = default);

    /// <summary>Duración total según las reglas del original (CalculaDuracion).</summary>
    int CalculaDuracion(IEnumerable<AreaAgendable> seleccionadas, ConfigAgendado config, DateTime fechaCita);

    /// <returns>(idCitaNueva, envioOnline) si se guardó; lanza excepción con el
    /// mensaje de validación si no.</returns>
    Task<(int IdCita, bool EnvioOnline)> GuardarCitaAsync(int idPacienteLocal, DateTime fechaInicio, int duracion,
        IReadOnlyList<AreaAgendable> seleccionadas, ConfigAgendado config, int idUsuario,
        int idSucursalAgendaExterna, CancellationToken ct = default);
}

public class NuevaCitaService : INuevaCitaService
{
    private readonly PosDbContext _db;
    private readonly ContextoPos _contexto;
    private readonly Sincronizacion.IInterfazCatalogosDescargaService _descargas;

    public NuevaCitaService(PosDbContext db, ContextoPos contexto, Sincronizacion.IInterfazCatalogosDescargaService descargas)
    {
        _db = db;
        _contexto = contexto;
        _descargas = descargas;
    }

    public async Task<(ConfigAgendado Config, List<AreaAgendable> Areas)> AreasDelPacienteAsync(int idPacienteLocal, int idSucursalAgendaExterna, CancellationToken ct = default)
    {
        // La agenda externa manda si el usuario agenda para otra sucursal.
        int idSucursal = idSucursalAgendaExterna > 0 ? idSucursalAgendaExterna : _contexto.IdSucursal;

        var sucursal = await _db.Sucursal.AsNoTracking().FirstOrDefaultAsync(x => x.IdSucursal == idSucursal, ct)
                       ?? throw new Exception($"No existe la sucursal {idSucursal} en el catálogo local.");

        var paciente = await _db.Paciente.AsNoTracking().FirstOrDefaultAsync(x => x.IdLocal == idPacienteLocal, ct);

        var config = new ConfigAgendado
        {
            PacienteBloqueado = paciente?.Estatus == "B",
            ClienteTiene1erCita = paciente?.ClienteTiene1ercita ?? true,
            TiempoAdicionalPrimeraCita = sucursal.TiempoAdicionalPrimeraCita ?? 0,
            TiempoMinimoPrimeraCita = sucursal.TiempoMinimoPrimeraCita ?? 0,
            SesionExtendida = sucursal.SesionExtendida ?? false,
            AumentoSesionExtendida = sucursal.AumentoSesionExtendida ?? 0,
            AumentoPorcSesionExtendida = sucursal.AumentoPorcSesionExtendida ?? 0,
            TiempoMinimoSesionExtendida = sucursal.TiempoMinimoSesionExtendida ?? 0,
        };

        string tipoTiempos = sucursal.TipoTiempos ?? string.Empty;
        bool robusto = paciente?.ClienteRobustoDs == 1 && paciente?.ClienteRobustoS == 1;

        // Áreas pendientes (SP del original); devuelve renglones de paquete_servicio.
        var renglones = await ProcedimientoAlmacenado.ListaAsync(_db, "recupera_areas_por_paciente",
            new Dictionary<string, object?> { ["@id_paciente_local"] = idPacienteLocal },
            r => new
            {
                IdLocalPaquete = r.EnteroONull("id_local_paquete") ?? 0,
                IdPaquete = r.EnteroONull("id_paquete"),
                IdServicio = r.EnteroONull("id_servicio") ?? 0,
                IdPaciente = r.EnteroONull("id_paciente"),
                IdPacienteLocal = r.EnteroONull("id_paciente_local") ?? 0,
                Estatus = r.CadenaODefecto("estatus"),
            }, ct);

        var idsServicio = renglones.Select(x => x.IdServicio).Distinct().ToList();
        var servicios = await _db.Servicio.AsNoTracking()
            .Where(s => idsServicio.Contains(s.IdServicio))
            .ToDictionaryAsync(s => s.IdServicio, ct);

        var areas = new List<AreaAgendable>();
        int idPaqueteAnterior = renglones.Count > 0 ? renglones[0].IdLocalPaquete : 0;
        int noPaquete = 1;

        foreach (var a in renglones)
        {
            if (a.IdLocalPaquete != idPaqueteAnterior)
            {
                noPaquete++;
                idPaqueteAnterior = a.IdLocalPaquete;
            }

            if (!servicios.TryGetValue(a.IdServicio, out var s))
                continue;

            // Las áreas "plus" (tipo 11/12) no se agendan salvo en el país 5.
            if (_contexto.IdPais != 5 && (s.IdTipoServicio == 11 || s.IdTipoServicio == 12))
                continue;

            // Las que ya están agendadas (estatus A) no se muestran.
            if (a.Estatus == "A")
                continue;

            decimal duracion = s.Duracion ?? 0;
            decimal duracionServicio = 0;

            if (tipoTiempos == "R")
            {
                duracion = s.TrDuracionLaser ?? 0;
                duracionServicio = s.TrDuracionServicio ?? 0;
            }

            if (robusto)
                duracion *= 1.2m;

            string descripcion = (s.Descripcion ?? string.Empty) + a.Estatus switch
            {
                "E" => " (Extra)",
                "R1" => " (Retoque 1)",
                "R2" => " (Retoque 2)",
                _ => string.Empty,
            };

            areas.Add(new AreaAgendable
            {
                NoPaquete = noPaquete,
                Descripcion = descripcion,
                IdServicio = a.IdServicio,
                IdPaciente = a.IdPaciente,
                IdPacienteLocal = a.IdPacienteLocal,
                IdPaquete = a.IdPaquete,
                IdPaqueteLocal = a.IdLocalPaquete,
                Duracion = duracion,
                Estatus = a.Estatus,
                DuracionServicio = duracionServicio,
            });
        }

        return (config, areas);
    }

    public int CalculaDuracion(IEnumerable<AreaAgendable> seleccionadas, ConfigAgendado config, DateTime fechaCita)
    {
        var lista = seleccionadas.ToList();
        if (lista.Count == 0)
            return 0;

        // Suma de duraciones + el mayor tiempo de servicio (regla LB20170901:
        // el tiempo de servicio solo cuenta una vez, en la primera cita).
        decimal duracion = lista.Sum(a => a.Duracion) + lista.Max(a => a.DuracionServicio);

        bool entreSemana = fechaCita.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;

        if (config.SesionExtendida && entreSemana)
        {
            duracion = Math.Ceiling((duracion + config.AumentoSesionExtendida) * config.AumentoPorcSesionExtendida);

            if (duracion < config.TiempoMinimoSesionExtendida)
                duracion = config.TiempoMinimoSesionExtendida;
        }

        int adicional = config.ClienteTiene1erCita ? 0 : config.TiempoAdicionalPrimeraCita;
        int total = (int)duracion + adicional;

        if (adicional > 0 && total < config.TiempoMinimoPrimeraCita)
            total = config.TiempoMinimoPrimeraCita;

        return total;
    }

    public async Task<(int IdCita, bool EnvioOnline)> GuardarCitaAsync(int idPacienteLocal, DateTime fechaInicio, int duracion,
        IReadOnlyList<AreaAgendable> seleccionadas, ConfigAgendado config, int idUsuario,
        int idSucursalAgendaExterna, CancellationToken ct = default)
    {
        // ---- Validaciones de ValidaCita ----
        int idSucursalHorario = idSucursalAgendaExterna > 0 ? idSucursalAgendaExterna : _contexto.IdSucursal;

        if (config.PacienteBloqueado)
            throw new Exception("El cliente está bloqueado para agendar citas.");

        if (fechaInicio.Date < DateTime.Today)
            throw new Exception("Solo se permiten citas a futuro.");

        if ((fechaInicio.Date - DateTime.Today).TotalDays > 60)
            throw new Exception("No se pueden agregar citas a mas de 60 dias de hoy");

        if (duracion == 0 || seleccionadas.Count == 0)
            throw new Exception("Seleccione al menos una area o paquete");

        var horario = await ProcedimientoAlmacenado.TablaAsync(_db, "pos_valida_horario_cita",
            new Dictionary<string, object?>
            {
                ["@fecha_inicio"] = fechaInicio.ToString("yyyyMMdd HH:mm"),
                ["@duracion"] = duracion,
                ["@id_sucursal"] = idSucursalHorario,
            }, ct);

        string respuestaHorario = horario.Rows.Count > 0 ? Convert.ToString(horario.Rows[0]["id"]) ?? "" : "";
        if (respuestaHorario != "1")
            throw new Exception(respuestaHorario);

        // ---- Validación por paquete (pos_valida_cita), con la misma
        //      distribución de horarios/duraciones del original ----
        var grupos = seleccionadas
            .GroupBy(a => a.IdPaqueteLocal)
            .Select(g => new
            {
                IdPaqueteLocal = g.Key,
                TieneIdPaquete = g.All(a => a.IdPaquete is not null),
                Servicios = string.Join("|", g.Select(a => $"{a.IdServicio},{a.Estatus}")),
                Duracion = g.Sum(a => a.Duracion),
                MaxServicio = g.Max(a => a.DuracionServicio),
            })
            .ToList();

        bool envioOnline = grupos.All(g => g.TieneIdPaquete);

        DateTime fechaServidor = await _descargas.GetServerDateAsync(int.Parse(_contexto.ClaveBloque), ct);
        int desbloqueo = _contexto.FechaDesbloqueo > fechaServidor ? 1 : 0;

        var errores = new System.Text.StringBuilder();
        decimal minutosAcumulados = 0;
        int duracionGlobalRestante = duracion;
        int posicion = 1;

        foreach (var g in grupos)
        {
            // Reparte la duración global entre paquetes como el original:
            // el primero absorbe el tiempo de servicio, el último el remanente.
            decimal duracionIndividual = g.Duracion;
            if (posicion == 1 && g.MaxServicio > 0)
                duracionIndividual += g.MaxServicio;
            if (posicion == grupos.Count)
                duracionIndividual = duracionGlobalRestante;

            DateTime fechaGrupo = fechaInicio.AddMinutes((double)minutosAcumulados);

            var validacion = await ProcedimientoAlmacenado.TablaAsync(_db, "pos_valida_cita",
                new Dictionary<string, object?>
                {
                    ["@id_paciente"] = idPacienteLocal,
                    ["@id_paquete"] = g.IdPaqueteLocal,
                    ["@fecha_cita"] = fechaGrupo.ToString("yyyyMMdd HH:mm"),
                    ["@desbloqueo"] = desbloqueo,
                    ["@servicios"] = g.Servicios,
                }, ct);

            string resultado = validacion.Rows.Count > 0 ? Convert.ToString(validacion.Rows[0]["resultado"]) ?? "" : "";
            if (resultado != "OK")
                errores.AppendLine(resultado);

            minutosAcumulados += duracionIndividual;
            duracionGlobalRestante -= (int)duracionIndividual;
            posicion++;
        }

        if (errores.Length > 0)
            throw new Exception(errores.ToString());

        // ---- Guardado (guarda_cita_x_areas) ----
        string ids = string.Join("|", seleccionadas.Select(a => $"{a.IdServicio},{a.IdPaqueteLocal},{a.Estatus}"));

        var guardado = await ProcedimientoAlmacenado.TablaAsync(_db, "guarda_cita_x_areas",
            new Dictionary<string, object?>
            {
                ["@fecha_inicio"] = fechaInicio.ToString("yyyyMMdd HH:mm"),
                ["@duracion"] = duracion,
                ["@id_paciente_local"] = idPacienteLocal,
                ["@id_usuario_alta"] = idUsuario,
                ["@id_sucursal"] = _contexto.IdSucursal, // el original guarda con la sucursal propia
                ["@ids"] = ids,
                ["@envio_online"] = envioOnline,
            }, ct);

        int idCita = guardado.Rows.Count > 0 ? Convert.ToInt32(guardado.Rows[0]["id"]) : 0;

        return (idCita, envioOnline);
    }
}
