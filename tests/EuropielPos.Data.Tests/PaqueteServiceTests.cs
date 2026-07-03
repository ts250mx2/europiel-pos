using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización de <see cref="PaqueteService"/> (bloque 1) contra
/// la BD local. Solo métodos de lectura.
/// </summary>
public class PaqueteServiceTests
{
    [HechoConBdLocal]
    public async Task RecuperaClientesParaReimpresion_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var tabla = await servicio.RecuperaClientesParaReimpresionAsync(string.Empty);

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaPaqueteServicio_GeneraFragmentoConFormatoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idPaquete = await db.PaqueteServicio
            .Where(s => s.IdLocalPaquete != null)
            .Select(s => s.IdLocalPaquete!.Value)
            .FirstOrDefaultAsync();
        if (idPaquete == 0)
            return; // BD sin paquetes con servicios

        var servicio = new PaqueteService(db);
        var fragmento = await servicio.RecuperaPaqueteServicioAsync(idPaquete);

        // Objetos separados por coma, sin corchetes ni coma final.
        Assert.StartsWith("{\"id_local\": \"", fragmento);
        Assert.EndsWith("\"}", fragmento);
        Assert.Contains("\"es_gratis\": \"", fragmento);
    }

    [HechoConBdLocal]
    public async Task RecuperaPaqueteFinanciamiento_ConservaElTypoHistoricoDelContrato()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idPaquete = await db.PaqueteFinanciamiento
            .Where(f => f.IdPaqueteLocal != null)
            .Select(f => f.IdPaqueteLocal!.Value)
            .FirstOrDefaultAsync();
        if (idPaquete == 0)
            return; // BD sin financiamientos

        var servicio = new PaqueteService(db);
        var fragmento = await servicio.RecuperaPaqueteFinanciamientoAsync(idPaquete);

        // El servidor espera la clave con el typo "id_financiemiento" (sic).
        Assert.Contains("\"id_financiemiento\": \"", fragmento);
        Assert.False(fragmento.EndsWith(","), "No debe quedar coma final.");
    }

    [HechoConBdLocal]
    public async Task RecuperaPaqueteFinanciamiento_PaqueteInexistente_DevuelveVacio()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var fragmento = await servicio.RecuperaPaqueteFinanciamientoAsync(-99999);

        Assert.Equal(string.Empty, fragmento);
    }
}
