using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización contra la BD local EUROPIEL_POS.
/// Validan que los servicios portados devuelven lo mismo que los BL de VB.NET.
/// </summary>
public class ServiciosCatalogoTests
{
    [HechoConBdLocal]
    public async Task RecuperaParametro_DevuelveElRegistroDeConfiguracion()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new ParametroService(db);

        var parametro = await servicio.RecuperaParametroAsync();

        Assert.NotNull(parametro);
        Assert.False(string.IsNullOrEmpty(parametro!.Idioma));
    }

    [HechoConBdLocal]
    public async Task RecuperaDifVentasSemanaPasada_EjecutaElStoredProcedure()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new ParametroService(db);

        // No debe lanzar excepción; el valor puede ser 0 o cualquier decimal.
        var dif = await servicio.RecuperaDifVentasSemanaPasadaAsync();

        Assert.IsType<decimal>(dif);
    }

    [HechoConBdLocal]
    public async Task RecuperaSucursal_AplicaDefaultsDelBLOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        // Toma una sucursal real de la BD para consultarla vía el servicio.
        var idExistente = await db.Sucursal.Select(s => s.IdSucursal).FirstOrDefaultAsync();
        Assert.True(idExistente > 0, "La tabla sucursal está vacía; no se puede probar RecuperaSucursal.");

        var servicio = new SucursalService(db);
        var sucursal = await servicio.RecuperaSucursalAsync(idExistente);

        Assert.NotNull(sucursal);
        // Defaults que el BL de VB aplicaba cuando la columna venía NULL:
        Assert.NotNull(sucursal!.ValidarAreasMaxVentasPorCliente);
        Assert.NotNull(sucursal.PermitirSolicitarHasta6pagosXToken);
        Assert.NotNull(sucursal.ValidarBinTarjetaAnticipoMinimo);
        Assert.NotNull(sucursal.ValidarBinTarjetaAnticipoMinimoMonto);
        Assert.NotNull(sucursal.ValidaToku);
        Assert.NotNull(sucursal.TerminalIdMercadoPago);
    }

    [HechoConBdLocal]
    public async Task RecuperaSucursal_IdInexistente_DevuelveNull()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new SucursalService(db);

        var sucursal = await servicio.RecuperaSucursalAsync(-99999);

        Assert.Null(sucursal);
    }
}
