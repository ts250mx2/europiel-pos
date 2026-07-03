using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>SucursalBL.vb</c>.
/// </summary>
public interface ISucursalService
{
    Task EliminaSucursalesAsync(CancellationToken ct = default);

    Task<Sucursal?> RecuperaSucursalAsync(int idSucursal, CancellationToken ct = default);
}

public class SucursalService : ISucursalService
{
    private readonly PosDbContext _db;

    public SucursalService(PosDbContext db) => _db = db;

    /// <remarks>Equivale a <c>DELETE FROM sucursal</c> (se usa antes de re-sincronizar el catálogo).</remarks>
    public Task EliminaSucursalesAsync(CancellationToken ct = default) =>
        _db.Sucursal.ExecuteDeleteAsync(ct);

    /// <remarks>
    /// El original construía el SQL concatenando el id (inyección SQL);
    /// aquí EF Core parametriza la consulta.
    /// </remarks>
    public async Task<Sucursal?> RecuperaSucursalAsync(int idSucursal, CancellationToken ct = default)
    {
        Sucursal? sucursal = await _db.Sucursal.AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdSucursal == idSucursal, ct);

        if (sucursal is null)
            return null;

        // Defaults que el BL original aplicaba cuando la columna venía en NULL.
        sucursal.ValidarAreasMaxVentasPorCliente ??= false;
        sucursal.PermitirSolicitarHasta6pagosXToken ??= false;
        sucursal.ValidarBinTarjetaAnticipoMinimo ??= false;
        sucursal.ValidarBinTarjetaAnticipoMinimoMonto ??= 0m;
        sucursal.ValidaToku ??= 0;
        sucursal.TerminalIdMercadoPago ??= string.Empty;

        return sucursal;
    }
}
