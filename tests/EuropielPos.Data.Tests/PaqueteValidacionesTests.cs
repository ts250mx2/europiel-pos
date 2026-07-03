using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización del bloque 3 de <see cref="PaqueteService"/>
/// (validaciones de venta) contra la BD local. Solo métodos de lectura;
/// no se toca RecuperarYBorrarAreasPaqueteServicio porque borra datos.
/// </summary>
public class PaqueteValidacionesTests
{
    [HechoConBdLocal]
    public async Task RecuperaPaquetePorId_PaqueteExistente_MaterializaEntidad()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idExistente = await db.Paquete.Select(p => p.IdLocal).FirstOrDefaultAsync();
        if (idExistente == 0)
            return; // BD sin paquetes

        var servicio = new PaqueteService(db);
        var paquete = await servicio.RecuperaPaquetePorIdAsync(idExistente);

        Assert.NotNull(paquete);
        Assert.Equal(idExistente, paquete!.IdLocal);
    }

    [HechoConBdLocal]
    public async Task RecuperaPaquetePorId_IdInexistente_DevuelveNull()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var paquete = await servicio.RecuperaPaquetePorIdAsync(-99999);

        Assert.Null(paquete);
    }

    [HechoConBdLocal]
    public async Task ValidaTarjetaNueva_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var tabla = await servicio.ValidaTarjetaNuevaAsync("0000000000000000");

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaSaldoTotalPaquetes_PacientesInexistentes_DevuelveCero()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var saldo = await servicio.RecuperaSaldoTotalPaquetesAsync(-1, -2, -3, -4, -5);

        Assert.Equal(0m, saldo);
    }

    [HechoConBdLocal]
    public async Task RecuperaReimpresionContrato_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var tabla = await servicio.RecuperaReimpresionContratoAsync();

        Assert.NotNull(tabla);
    }
}
