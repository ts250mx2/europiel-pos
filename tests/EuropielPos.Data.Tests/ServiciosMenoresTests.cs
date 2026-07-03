using EuropielPos.Data.Services;
using EuropielPos.Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de los BL menores: Crypto (puro, sin BD) y lecturas de
/// LogInterfaz/RespuestaNetpay contra la BD local.
/// </summary>
public class ServiciosMenoresTests
{
    // --- Crypto: valores verificables de forma determinista ---

    [Fact]
    public void GenerateSha256String_CoincideConElValorConocido()
    {
        // SHA-256("abc") es un vector de prueba estándar.
        var hash = Crypto.GenerateSha256String("abc");

        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
    }

    [Fact]
    public void ConvertHexStringToByteArray_ConvierteCorrectamente()
    {
        var bytes = Crypto.ConvertHexStringToByteArray("0A1B2C");

        Assert.Equal(new byte[] { 0x0A, 0x1B, 0x2C }, bytes);
    }

    [Fact]
    public void StartEncryption_EsDeterministaYLlevaPrefijoMerchant()
    {
        const string key = "000102030405060708090A0B0C0D0E0F000102030405060708090A0B0C0D0E0F";
        const string iv = "000102030405060708090A0B0C0D0E0F";

        var r1 = Crypto.StartEncryption("cuerpo de prueba", key, iv, "MERCHANT1");
        var r2 = Crypto.StartEncryption("cuerpo de prueba", key, iv, "MERCHANT1");

        Assert.Equal(r1, r2); // AES-CBC con IV fijo es determinista
        Assert.StartsWith("MERCHANT1_", r1);
        // 64 chars ASCII del SHA-256 → 5 bloques AES de 16 bytes → 80 bytes → 160 hex
        Assert.Equal("MERCHANT1_".Length + 160, r1.Length);
    }

    // --- LogInterfaz / RespuestaNetpay contra BD local ---

    [HechoConBdLocal]
    public async Task LogInterfaz_InsertaYActualiza()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new LogInterfazService(db);

        var id = await servicio.GuardaInicioLogInterfazAsync("TEST_MIGRACION", DateTime.Now);
        Assert.True(id > 0);

        await servicio.GuardaFinLogInterfazAsync(DateTime.Now, "prueba de caracterización", null, id);

        // ExecuteUpdate no pasa por el change tracker: releer sin caché.
        var registro = await db.LogInterfaz.AsNoTracking().FirstAsync(l => l.Id == id);
        Assert.Equal("prueba de caracterización", registro.Mensaje);
        Assert.NotNull(registro.FechaFin);

        // Limpieza del registro de prueba.
        await db.LogInterfaz.Where(l => l.Id == id).ExecuteDeleteAsync();
    }

    [HechoConBdLocal]
    public async Task RecuperaRespuestaNetpayAEnviar_SoloPendientes()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new RespuestaNetpayService(db);

        var pendientes = await servicio.RecuperaRespuestaNetpayAEnviarAsync();

        Assert.NotNull(pendientes);
        Assert.All(pendientes, r => Assert.Null(r.FechaInterfaz));
    }
}
