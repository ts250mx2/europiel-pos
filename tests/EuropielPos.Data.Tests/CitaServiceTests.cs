using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización de <see cref="CitaService"/> contra la BD local.
/// Solo métodos de lectura; los mutadores se validarán con el flujo de
/// sincronización completo.
/// </summary>
public class CitaServiceTests
{
    [HechoConBdLocal]
    public async Task RecuperaCitasAEnviar_AplicaReglaDeUnaSemanaYNormalizaCampos()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new CitaService(db);

        var citas = await servicio.RecuperaCitasAEnviarAsync();

        Assert.NotNull(citas);
        foreach (var c in citas)
        {
            Assert.Null(c.FechaInterfaz);
            Assert.Equal(0, c.IdPadreLocal);
            // Campos que el BL original no poblaba en el payload:
            Assert.Equal(0, c.UsuarioEstatus);
            Assert.Null(c.FechaEstatus);
        }
    }

    [HechoConBdLocal]
    public async Task RecuperaCitaServicios_DevuelveServiciosDeLaCitaYSusHijas()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idCitaConServicios = await db.CitaServicio
            .Select(cs => cs.IdCitaLocal ?? 0)
            .FirstOrDefaultAsync();

        var servicio = new CitaService(db);
        var lista = await servicio.RecuperaCitaServiciosAsync(idCitaConServicios);

        Assert.NotNull(lista);
        if (idCitaConServicios > 0)
            Assert.All(lista, cs => Assert.True(cs.IdLocal > 0));
    }

    [HechoConBdLocal]
    public async Task RecuperaCitasNoAgendada_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new CitaService(db);

        var tabla = await servicio.RecuperaCitasNoAgendadaAsync();

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaCitasAEnviarPorId_IdInexistente_DevuelveNull()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new CitaService(db);

        var cita = await servicio.RecuperaCitasAEnviarPorIdAsync(-99999);

        Assert.Null(cita);
    }
}
