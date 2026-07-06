using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de <see cref="ClientesService"/> contra la BD local.
/// </summary>
public class ClientesServiceTests
{
    [HechoConBdLocal]
    public async Task Expediente_ClienteConPaquete_TraePaquetesYSaldos()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idPaciente = await db.Paquete.AsNoTracking()
            .Where(p => p.IdPacienteLocal != null)
            .Select(p => p.IdPacienteLocal!.Value)
            .FirstOrDefaultAsync();

        if (idPaciente == 0)
            return; // BD sin paquetes

        var servicio = new ClientesService(db, new PagoCajaService(db));
        var expediente = await servicio.ExpedienteAsync(idPaciente);

        Assert.NotEmpty(expediente.Paquetes);
        Assert.Equal(expediente.Paquetes.Sum(p => p.SaldoTotal), expediente.SaldoTotal);
        // Ordenados del más reciente al más antiguo.
        var fechas = expediente.Paquetes.Select(p => p.FechaCompra).ToList();
        Assert.Equal(fechas.OrderByDescending(f => f), fechas);
    }

    [HechoConBdLocal]
    public async Task Expediente_ClienteInexistente_DevuelveVacio()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new ClientesService(db, new PagoCajaService(db));

        var expediente = await servicio.ExpedienteAsync(-99999);

        Assert.Empty(expediente.Paquetes);
        Assert.Empty(expediente.UltimasCitas);
        Assert.Equal(0m, expediente.SaldoTotal);
    }
}
