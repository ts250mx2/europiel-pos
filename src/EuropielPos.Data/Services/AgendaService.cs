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

/// <summary>
/// Consultas de la agenda (vista de día). Lee de la BD local, que el motor
/// de sincronización mantiene al día contra el servidor central.
/// </summary>
public interface IAgendaService
{
    Task<List<CitaAgenda>> CitasDelDiaAsync(DateTime fecha, int idSucursal, CancellationToken ct = default);
}

public class AgendaService : IAgendaService
{
    private readonly PosDbContext _db;

    public AgendaService(PosDbContext db) => _db = db;

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
}
