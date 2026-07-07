using EuropielPos.Data.Services.Sincronizacion;
using Xunit;

namespace EuropielPos.Data.Tests;

public class SincronizacionEstadoTests
{
    [HechoConBdLocal]
    public async Task CuentaPendientes_DevuelveConteosNoNegativosYTotalConsistente()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new SincronizacionEstadoService(db);

        var p = await servicio.CuentaPendientesAsync();

        Assert.True(p.Pacientes >= 0);
        Assert.True(p.Pagos >= 0);
        Assert.Equal(
            p.Pacientes + p.Paquetes + p.Pagos + p.Citas + p.Requerimientos + p.Gios + p.Asistencias + p.RespuestasNetpay,
            p.Total);
    }

    [HechoConBdLocal]
    public async Task UltimasCorridas_RespetaElLimiteYVienenOrdenadas()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new SincronizacionEstadoService(db);

        var corridas = await servicio.UltimasCorridasAsync(5);

        Assert.True(corridas.Count <= 5);

        for (int i = 1; i < corridas.Count; i++)
            Assert.True(corridas[i - 1].Id > corridas[i].Id, "Deben venir de la más reciente a la más vieja.");
    }
}
