using System.Text.Json;
using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización de <see cref="RequerimientoService"/> contra la BD local.
/// Solo métodos de lectura.
/// </summary>
public class RequerimientoServiceTests
{
    [HechoConBdLocal]
    public async Task RecuperaPrioridad_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new RequerimientoService(db);

        var tabla = await servicio.RecuperaPrioridadAsync(string.Empty);

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaRequerimientoAEnviar_DevuelveListaSinExcepcion()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new RequerimientoService(db);

        var pendientes = await servicio.RecuperaRequerimientoAEnviarAsync();

        Assert.NotNull(pendientes);
    }

    [HechoConBdLocal]
    public async Task RecuperaJsonRequerimiento_GeneraPayloadConContratoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idExistente = await db.Requerimiento.Select(x => x.IdLocal).FirstOrDefaultAsync();
        var servicio = new RequerimientoService(db);

        var json = await servicio.RecuperaJsonRequerimientoAEnviarAsync(idExistente, bloque: "MX1", noCaja: 1, idSucursal: 10);

        using var doc = JsonDocument.Parse(json);
        var raiz = doc.RootElement;

        // El contrato del servidor exige snake_case y la inconsistencia histórica fecha_Vencimiento.
        Assert.Equal("MX1", raiz.GetProperty("bloque").GetString());
        Assert.Equal(1, raiz.GetProperty("no_caja").GetInt32());
        Assert.Equal(10, raiz.GetProperty("id_sucursal").GetInt32());
        Assert.True(raiz.TryGetProperty("fecha_Vencimiento", out _));
        Assert.True(raiz.TryGetProperty("id_tipo_falla", out _));
    }

    [HechoConBdLocal]
    public async Task RecuperaRequerimientoEnvio_IdInexistente_DevuelveDtoVacio()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new RequerimientoService(db);

        var dto = await servicio.RecuperaRequerimientoEnvioAsync(-99999);

        Assert.NotNull(dto);
        Assert.Equal(0, dto.id_local);
    }
}
