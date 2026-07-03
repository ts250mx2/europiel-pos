using System.Text.Json;
using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización de <see cref="GioService"/> contra la BD local.
/// Solo métodos de lectura.
/// </summary>
public class GioServiceTests
{
    [HechoConBdLocal]
    public async Task RecuperaTurno_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new GioService(db);

        var tabla = await servicio.RecuperaTurnoAsync(string.Empty);

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaGioAEnviar_DevuelveListaSinExcepcion()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new GioService(db);

        var pendientes = await servicio.RecuperaGioAEnviarAsync();

        Assert.NotNull(pendientes);
    }

    [HechoConBdLocal]
    public async Task RecuperaGioAsistenciaAEnviar_DevuelveListaSinExcepcion()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new GioService(db);

        var pendientes = await servicio.RecuperaGioAsistenciaAEnviarAsync();

        Assert.NotNull(pendientes);
    }

    [HechoConBdLocal]
    public async Task RecuperaJsonGio_GeneraPayloadConContratoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idExistente = await db.Gio.Select(g => g.IdLocal).FirstOrDefaultAsync();
        var servicio = new GioService(db);

        var json = await servicio.RecuperaJsonGioAEnviarAsync(idExistente, bloque: "MX1", noCaja: 1, idSucursal: 10);

        using var doc = JsonDocument.Parse(json);
        var raiz = doc.RootElement;

        Assert.Equal("MX1", raiz.GetProperty("bloque").GetString());
        Assert.True(raiz.TryGetProperty("clave_interbancaria", out _));
        Assert.True(raiz.TryGetProperty("es_cambio_sucursal", out _));
    }

    [HechoConBdLocal]
    public async Task RecuperaNombreGio_IdInexistente_DevuelveVacio()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new GioService(db);

        var nombre = await servicio.RecuperaNombreGioAsync(-99999);

        Assert.Equal(string.Empty, nombre);
    }
}
