using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>Registros locales pendientes de subir al servidor central
/// (fecha_interfaz vacía, los mismos criterios que usa el orquestador).</summary>
public record PendientesSincronizacion(
    int Pacientes,
    int Paquetes,
    int Pagos,
    int Citas,
    int Requerimientos,
    int Gios,
    int Asistencias,
    int RespuestasNetpay)
{
    public int Total => Pacientes + Paquetes + Pagos + Citas + Requerimientos + Gios + Asistencias + RespuestasNetpay;
}

/// <summary>
/// Estado de la sincronización para la UI: cuántos registros faltan por enviar
/// y las últimas corridas de la bitácora <c>log_interfaz</c>.
/// </summary>
public interface ISincronizacionEstadoService
{
    Task<PendientesSincronizacion> CuentaPendientesAsync(CancellationToken ct = default);

    Task<List<LogInterfaz>> UltimasCorridasAsync(int cuantas, CancellationToken ct = default);
}

public class SincronizacionEstadoService : ISincronizacionEstadoService
{
    private readonly PosDbContext _db;

    public SincronizacionEstadoService(PosDbContext db) => _db = db;

    public async Task<PendientesSincronizacion> CuentaPendientesAsync(CancellationToken ct = default) =>
        new(
            Pacientes: await _db.Paciente.CountAsync(x => x.FechaInterfaz == null, ct),
            Paquetes: await _db.Paquete.CountAsync(x => x.FechaInterfaz == null, ct),
            Pagos: await _db.PagoCaja.CountAsync(x => x.FechaInterfaz == null, ct),
            // Las citas hijas (id_padre_local > 0) viajan con su padre.
            Citas: await _db.Cita.CountAsync(x => x.FechaInterfaz == null && x.IdPadreLocal == 0, ct),
            Requerimientos: await _db.Requerimiento.CountAsync(x => x.FechaInterfaz == null, ct),
            Gios: await _db.Gio.CountAsync(x => x.FechaInterfaz == null, ct),
            Asistencias: await _db.GioAsistencia.CountAsync(x => x.FechaInterfaz == null, ct),
            RespuestasNetpay: await _db.RespuestaNetpay.CountAsync(x => x.FechaInterfaz == null, ct));

    public Task<List<LogInterfaz>> UltimasCorridasAsync(int cuantas, CancellationToken ct = default) =>
        _db.LogInterfaz.AsNoTracking()
            .OrderByDescending(l => l.Id)
            .Take(cuantas)
            .ToListAsync(ct);
}
