using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>LogInterfazBL.vb</c> — bitácora de las corridas de sincronización.
/// </summary>
public interface ILogInterfazService
{
    Task<int> GuardaInicioLogInterfazAsync(string tipo, DateTime fechaInicio, CancellationToken ct = default);

    Task GuardaFinLogInterfazAsync(DateTime fechaFin, string mensaje, DateTime? fechaServidor, int id, CancellationToken ct = default);

    Task<DateTime?> RecuperaFechaLogInterfazAsync(string tipo1, string tipo2, CancellationToken ct = default);
}

public class LogInterfazService : ILogInterfazService
{
    private readonly PosDbContext _db;

    public LogInterfazService(PosDbContext db) => _db = db;

    /// <returns>El id del registro insertado.</returns>
    public async Task<int> GuardaInicioLogInterfazAsync(string tipo, DateTime fechaInicio, CancellationToken ct = default)
    {
        // El original mandaba la fecha como texto yyyy-MM-dd HH:mm:ss,
        // así que se conserva la precisión de segundos.
        var registro = new LogInterfaz
        {
            Tipo = tipo,
            FechaInicio = SinMilisegundos(fechaInicio),
        };

        _db.LogInterfaz.Add(registro);
        await _db.SaveChangesAsync(ct);

        return registro.Id;
    }

    public Task GuardaFinLogInterfazAsync(DateTime fechaFin, string mensaje, DateTime? fechaServidor, int id, CancellationToken ct = default)
    {
        DateTime fin = SinMilisegundos(fechaFin);
        DateTime? servidor = fechaServidor is null ? null : SinMilisegundos(fechaServidor.Value);

        return _db.LogInterfaz
            .Where(l => l.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.FechaFin, fin)
                .SetProperty(l => l.Mensaje, mensaje)
                .SetProperty(l => l.FechaServidor, servidor), ct);
    }

    /// <remarks>Stored procedure <c>recupera_sigte_fecha_interfaz</c>. El original
    /// tronaba si el SP devolvía null; aquí se devuelve <c>null</c>.</remarks>
    public async Task<DateTime?> RecuperaFechaLogInterfazAsync(string tipo1, string tipo2, CancellationToken ct = default)
    {
        var resultado = await ProcedimientoAlmacenado.EscalarAsync(_db, "recupera_sigte_fecha_interfaz",
            new Dictionary<string, object?>
            {
                ["@tipo1"] = tipo1,
                ["@tipo2"] = tipo2,
            }, ct);

        return resultado is null or DBNull ? null : Convert.ToDateTime(resultado);
    }

    private static DateTime SinMilisegundos(DateTime fecha) =>
        new(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, fecha.Second);
}
