using System.Data;
using System.Data.Common;
using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>ParametroBL.vb</c>. La tabla <c>parametro</c> tiene un solo
/// registro con la configuración local de la sucursal/caja.
/// </summary>
public interface IParametroService
{
    Task<Parametro?> RecuperaParametroAsync(CancellationToken ct = default);

    Task ActualizaParametroDifVentasSemanaPasadaAsync(decimal difVentasSemanaPasada, CancellationToken ct = default);

    Task<decimal> RecuperaDifVentasSemanaPasadaAsync(CancellationToken ct = default);
}

public class ParametroService : IParametroService
{
    private readonly PosDbContext _db;

    public ParametroService(PosDbContext db) => _db = db;

    /// <remarks>Equivale a <c>SELECT TOP 1 * FROM parametro</c> del original.</remarks>
    public Task<Parametro?> RecuperaParametroAsync(CancellationToken ct = default) =>
        _db.Parametro.AsNoTracking().FirstOrDefaultAsync(ct);

    /// <remarks>Stored procedure <c>actualiza_parametro_dif_ventas_semana_pasada</c>.</remarks>
    public Task ActualizaParametroDifVentasSemanaPasadaAsync(decimal difVentasSemanaPasada, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_parametro_dif_ventas_semana_pasada @dif_ventas_semana_pasada = {difVentasSemanaPasada}", ct);

    /// <remarks>Stored procedure <c>recupera_dif_ventas_semana_pasada</c>.</remarks>
    public async Task<decimal> RecuperaDifVentasSemanaPasadaAsync(CancellationToken ct = default)
    {
        // EXEC no se puede componer con SqlQuery<T>, por eso se usa ADO.NET directo.
        DbConnection conn = _db.Database.GetDbConnection();
        bool cerrarConexion = conn.State == ConnectionState.Closed;
        if (cerrarConexion)
            await conn.OpenAsync(ct);

        try
        {
            await using DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "recupera_dif_ventas_semana_pasada";
            cmd.CommandType = CommandType.StoredProcedure;

            object? resultado = await cmd.ExecuteScalarAsync(ct);
            return resultado is DBNull or null ? 0m : Convert.ToDecimal(resultado);
        }
        finally
        {
            if (cerrarConexion)
                await conn.CloseAsync();
        }
    }
}
