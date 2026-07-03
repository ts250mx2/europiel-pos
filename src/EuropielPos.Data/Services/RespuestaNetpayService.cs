using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>RespuestaNetpayBL.vb</c> — respuestas de la terminal bancaria
/// (Netpay y otros procesadores) pendientes de sincronizar.
/// </summary>
public interface IRespuestaNetpayService
{
    Task ActualizaFechaInterfazRespuestaNetpayAsync(int id, DateTime fecha, CancellationToken ct = default);

    Task<List<RespuestaNetpay>> RecuperaRespuestaNetpayAEnviarAsync(CancellationToken ct = default);
}

public class RespuestaNetpayService : IRespuestaNetpayService
{
    private readonly PosDbContext _db;

    public RespuestaNetpayService(PosDbContext db) => _db = db;

    public Task ActualizaFechaInterfazRespuestaNetpayAsync(int id, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_respuesta_netpay @id = {id}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    public Task<List<RespuestaNetpay>> RecuperaRespuestaNetpayAEnviarAsync(CancellationToken ct = default) =>
        _db.RespuestaNetpay.AsNoTracking()
            .Where(r => r.FechaInterfaz == null)
            .ToListAsync(ct);
}
