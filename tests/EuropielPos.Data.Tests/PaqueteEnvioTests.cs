using System.Text.Json;
using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización del bloque 2 de <see cref="PaqueteService"/>
/// (payloads de envío) contra la BD local.
/// </summary>
public class PaqueteEnvioTests
{
    [HechoConBdLocal]
    public async Task RecuperaPaquetesAEnviar_AplicaReglaDeServiciosPendientes()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var paquetes = await servicio.RecuperaPaquetesAEnviarAsync();

        Assert.NotNull(paquetes);
        Assert.All(paquetes, p => Assert.Null(p.FechaInterfaz));
    }

    [HechoConBdLocal]
    public async Task RecuperaJsonPaquete_GeneraPayloadCompletoConContratoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var paquete = await db.Paquete.AsNoTracking().FirstOrDefaultAsync();
        if (paquete is null)
            return; // BD sin paquetes

        var servicio = new PaqueteService(db);
        var json = await servicio.RecuperaJsonPaqueteAEnviarAsync(paquete, bloque: "MX1", noCaja: 1, idSucursal: 10);

        using var doc = JsonDocument.Parse(json);
        var raiz = doc.RootElement;

        Assert.Equal("MX1", raiz.GetProperty("bloque").GetString());
        Assert.Equal(paquete.IdLocal, raiz.GetProperty("id_local").GetInt32());
        // El contrato arranca con los campos euroskin/conekta (orden de declaración VB).
        Assert.True(raiz.TryGetProperty("es_euroskin", out _));
        Assert.True(raiz.TryGetProperty("paquete_financiamiento", out _));
        Assert.True(raiz.TryGetProperty("paciente1", out _));
        // El financiamiento anidado conserva el typo histórico.
        var fin = raiz.GetProperty("paquete_financiamiento");
        if (fin.ValueKind == JsonValueKind.Array && fin.GetArrayLength() > 0)
            Assert.True(fin[0].TryGetProperty("id_financiemiento", out _));
    }

    [HechoConBdLocal]
    public async Task RecuperaPacienteEnvio_ReplicaBanderaEdadYSucursal2()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var paciente = await db.Paciente.AsNoTracking().FirstOrDefaultAsync();
        if (paciente is null)
            return;

        var servicio = new PaqueteService(db);
        var envio = await servicio.RecuperaPacienteEnvioAsync(paciente.IdLocal);

        Assert.Equal(paciente.IdLocal, envio.id_local);
        Assert.Equal(envio.id_sucursal, envio.id_sucursal_2); // repite id_sucursal
        Assert.True(envio.edad is 0 or 1);                    // bandera 1/0, no la edad real
    }

    [HechoConBdLocal]
    public async Task RecuperaPacienteEnvio_IdInexistente_DevuelveDtoVacio()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PaqueteService(db);

        var envio = await servicio.RecuperaPacienteEnvioAsync(-99999);

        Assert.Equal(0, envio.id_local);
    }
}
