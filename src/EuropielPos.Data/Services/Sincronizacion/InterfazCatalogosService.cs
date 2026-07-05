using EuropielPos.Domain.Sincronizacion;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Primer bloque del port de <c>modInterfaz.vb</c>: catálogos de
/// bloques/sucursales que alimentan la pantalla de configuración inicial.
/// </summary>
public interface IInterfazCatalogosService
{
    Task<List<SucursalBloqueApi>> GetSucursalYBloqueAsync(CancellationToken ct = default);

    Task<List<BloqueApi>> GetBloquesAsync(CancellationToken ct = default);
}

public class InterfazCatalogosService : IInterfazCatalogosService
{
    private readonly IClienteApiPos _api;

    public InterfazCatalogosService(IClienteApiPos api) => _api = api;

    public async Task<List<SucursalBloqueApi>> GetSucursalYBloqueAsync(CancellationToken ct = default)
    {
        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorSucursalBloque>>(
                "/api/europielpos/GetSucursalYBloque", ct: ct);

            return respuesta?.Value?.SucursalBloque ?? [];
        }
        catch (Exception ex)
        {
            // El original envolvía el error con este prefijo (y decía
            // "GetBloques" por un copy-paste; aquí con el nombre correcto).
            throw new Exception("Error HttpPost_GetSucursalYBloque: " + ex.Message, ex);
        }
    }

    public async Task<List<BloqueApi>> GetBloquesAsync(CancellationToken ct = default)
    {
        try
        {
            var respuesta = await _api.PostAsync<RespuestaApi<ValorBloque>>(
                "/api/europielpos/GetBloques", ct: ct);

            return respuesta?.Value?.Bloques ?? [];
        }
        catch (Exception ex)
        {
            throw new Exception("Error HttpPost_GetBloques: " + ex.Message, ex);
        }
    }
}
