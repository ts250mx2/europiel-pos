using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>Cita del día lista para pintar en la agenda.</summary>
public class CitaAgenda
{
    public int IdLocal { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string Paciente { get; set; } = string.Empty;

    public string? ListaServicios { get; set; }

    public string? Estatus { get; set; }

    public string? TipoCita { get; set; }

    public bool EsMorada { get; set; }

    public bool Asistida { get; set; }

    public bool Bloqueada { get; set; }

    public bool PendienteSincronizar { get; set; }
}

/// <summary>Detalle completo de una cita para el dialog de la agenda.</summary>
public class CitaDetalle
{
    public int IdLocal { get; set; }

    public int IdCita { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string Paciente { get; set; } = string.Empty;

    public string? Telefono1 { get; set; }

    public string? Telefono2 { get; set; }

    public string? Email { get; set; }

    public string? TipoCita { get; set; }

    public string? Estatus { get; set; }

    public string? EstatusInterfaz { get; set; }

    public string? MsgEstatusInterfaz { get; set; }

    public int? IdPaquete { get; set; }

    public DateTime? FechaAlta { get; set; }

    public bool PendienteSincronizar { get; set; }

    public List<string> Servicios { get; set; } = [];
}

/// <summary>
/// Consultas de la agenda (vista de día). Lee de la BD local, que el motor
/// de sincronización mantiene al día contra el servidor central.
/// </summary>
public interface IAgendaService
{
    Task<List<CitaAgenda>> CitasDelDiaAsync(DateTime fecha, int idSucursal, CancellationToken ct = default);

    Task<CitaDetalle?> DetalleCitaAsync(int idLocal, CancellationToken ct = default);

    /// <returns>null si se canceló; el mensaje de error si no.</returns>
    Task<string?> CancelarCitaAsync(int idLocal, int idUsuario, CancellationToken ct = default);
}

public class AgendaService : IAgendaService
{
    private readonly PosDbContext _db;
    private readonly Sincronizacion.IInterfazEnvioService _envio;

    public AgendaService(PosDbContext db, Sincronizacion.IInterfazEnvioService envio)
    {
        _db = db;
        _envio = envio;
    }

    public async Task<List<CitaAgenda>> CitasDelDiaAsync(DateTime fecha, int idSucursal, CancellationToken ct = default)
    {
        DateTime inicio = fecha.Date;
        DateTime fin = inicio.AddDays(1);

        var citas = await (
            from c in _db.Cita.AsNoTracking()
            join p in _db.Paciente.AsNoTracking() on c.IdPacienteLocal equals p.IdLocal into pacientes
            from p in pacientes.DefaultIfEmpty()
            join t in _db.TipoCita.AsNoTracking() on c.IdTipoCita equals t.IdTipoCita into tipos
            from t in tipos.DefaultIfEmpty()
            where c.FechaInicio >= inicio && c.FechaInicio < fin
                  && c.IdSucursal == idSucursal
                  && c.IdPadreLocal == 0 // las citas hijas son empalmes de la padre
            orderby c.FechaInicio
            select new CitaAgenda
            {
                IdLocal = c.IdLocal,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                Paciente = p == null
                    ? "(sin paciente)"
                    : ((p.Nombre ?? "") + " " + (p.ApPaterno ?? "")).Trim(),
                ListaServicios = c.ListaServicios,
                Estatus = c.Estatus,
                TipoCita = t == null ? null : t.Descripcion,
                // Reglas de color del original (cita_estatus_color):
                EsMorada = c.EsMorada == 1,
                Asistida = c.Asistida == 1,
                Bloqueada = c.Estatus == "B" || c.Estatus == "BLOQUEADA",
                PendienteSincronizar = c.FechaInterfaz == null,
            }).ToListAsync(ct);

        return citas;
    }

    public async Task<CitaDetalle?> DetalleCitaAsync(int idLocal, CancellationToken ct = default)
    {
        var detalle = await (
            from c in _db.Cita.AsNoTracking()
            join p in _db.Paciente.AsNoTracking() on c.IdPacienteLocal equals p.IdLocal into pacientes
            from p in pacientes.DefaultIfEmpty()
            join t in _db.TipoCita.AsNoTracking() on c.IdTipoCita equals t.IdTipoCita into tipos
            from t in tipos.DefaultIfEmpty()
            where c.IdLocal == idLocal
            select new CitaDetalle
            {
                IdLocal = c.IdLocal,
                IdCita = c.IdCita,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                Paciente = p == null
                    ? "(sin paciente)"
                    : ((p.Nombre ?? "") + " " + (p.ApPaterno ?? "") + " " + (p.ApMaterno ?? "")).Trim(),
                Telefono1 = p == null ? null : p.Telefono1,
                Telefono2 = p == null ? null : p.Telefono2,
                Email = p == null ? null : p.Email,
                TipoCita = t == null ? null : t.Descripcion,
                Estatus = c.Estatus,
                EstatusInterfaz = c.EstatusInterfaz,
                MsgEstatusInterfaz = c.MsgEstatusInterfaz,
                IdPaquete = c.IdPaquete,
                FechaAlta = c.FechaAlta,
                PendienteSincronizar = c.FechaInterfaz == null,
            }).FirstOrDefaultAsync(ct);

        if (detalle is null)
            return null;

        // Servicios de la cita y de sus hijas (empalmes), con descripción.
        detalle.Servicios = await (
            from cs in _db.CitaServicio.AsNoTracking()
            join c in _db.Cita.AsNoTracking() on cs.IdCitaLocal equals c.IdLocal
            join s in _db.Servicio.AsNoTracking() on cs.IdServicio equals s.IdServicio into servicios
            from s in servicios.DefaultIfEmpty()
            where c.IdLocal == idLocal || c.IdPadreLocal == idLocal
            select s == null ? "Servicio " + cs.IdServicio : s.Descripcion ?? string.Empty
        ).ToListAsync(ct);

        return detalle;
    }

    /// <summary>
    /// Cancelación con las reglas del original (frmCalendario): no se puede
    /// cancelar si falta menos de 1 hora; primero avisa al servidor y solo
    /// si responde OK borra la cita local.
    /// </summary>
    public async Task<string?> CancelarCitaAsync(int idLocal, int idUsuario, CancellationToken ct = default)
    {
        var cita = await _db.Cita.FirstOrDefaultAsync(x => x.IdLocal == idLocal, ct);

        if (cita is null)
            return "La cita ya no existe.";

        if (cita.FechaInicio is not null
            && cita.FechaInicio.Value > DateTime.Now
            && (cita.FechaInicio.Value - DateTime.Now).TotalMinutes < 60)
        {
            return "No es posible cancelar citas 1 hora antes.";
        }

        string respuesta = await _envio.CancelCitaAsync(idLocal, cita.IdCita, idUsuario, ct);

        if (!respuesta.StartsWith("OK"))
            return "Error al cancelar: " + respuesta;

        _db.Cita.Remove(cita);
        await _db.SaveChangesAsync(ct);

        return null;
    }
}
