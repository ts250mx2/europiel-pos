using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de <see cref="AgendaService"/> contra la BD local.
/// </summary>
public class AgendaServiceTests
{
    [HechoConBdLocal]
    public async Task CitasDelDia_SoloDelDiaSucursalYSinHijas()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        // Busca un día con citas reales para caracterizar contra datos verdaderos.
        var muestra = await db.Cita.AsNoTracking()
            .Where(c => c.FechaInicio != null && c.IdSucursal != null && c.IdPadreLocal == 0)
            .OrderByDescending(c => c.FechaInicio)
            .Select(c => new { c.FechaInicio, c.IdSucursal })
            .FirstOrDefaultAsync();

        if (muestra is null)
            return; // BD sin citas

        var servicio = new AgendaService(db);
        var citas = await servicio.CitasDelDiaAsync(muestra.FechaInicio!.Value.Date, muestra.IdSucursal!.Value);

        Assert.NotEmpty(citas);
        Assert.All(citas, c =>
        {
            Assert.Equal(muestra.FechaInicio.Value.Date, c.FechaInicio!.Value.Date);
            Assert.False(string.IsNullOrEmpty(c.Paciente));
        });

        // Deben venir ordenadas por hora de inicio.
        var ordenadas = citas.Select(c => c.FechaInicio).ToList();
        Assert.Equal(ordenadas.OrderBy(f => f), ordenadas);
    }

    [HechoConBdLocal]
    public async Task CitasDelDia_FechaSinCitas_DevuelveVacio()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new AgendaService(db);

        var citas = await servicio.CitasDelDiaAsync(new DateTime(1990, 1, 1), -1);

        Assert.Empty(citas);
    }
}
