using System.Text.Json;
using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización de <see cref="PagoCajaService"/> contra la BD local.
/// Solo métodos de lectura — es el flujo de dinero, los mutadores
/// (GuardaPagoCaja, ActualizaAnticipoSuelto...) se validarán al portar la
/// pantalla de caja.
/// </summary>
public class PagoCajaServiceTests
{
    [HechoConBdLocal]
    public async Task RecuperaPagoCajaAEnviar_SoloPagosSinSincronizarConPaciente()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PagoCajaService(db);

        var pagos = await servicio.RecuperaPagoCajaAEnviarAsync();

        Assert.NotNull(pagos);
        Assert.All(pagos, p =>
        {
            Assert.Null(p.FechaInterfaz);
            Assert.NotNull(p.IdPacienteLocal);
        });
    }

    [HechoConBdLocal]
    public async Task RecuperaSigteFolio_DevuelveFolioMayorAlMaximo()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PagoCajaService(db);

        var maxNormal = await db.PagoCaja
            .Where(p => p.Rfc != null && p.Rfc != "TRANSFERENCIA" && p.EsEuroskin == 0)
            .MaxAsync(p => (int?)p.FolioRecibo) ?? 0;

        var folio = await servicio.RecuperaSigteFolioAsync(esEuroskin: 0);

        var esperado = (maxNormal == 0 ? 1000 : maxNormal) + 1;
        Assert.Equal(esperado, folio);
    }

    [HechoConBdLocal]
    public async Task RecuperaSigteFolio_Transferencia_UsaSerie999000()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PagoCajaService(db);

        var folio = await servicio.RecuperaSigteFolioAsync(esEuroskin: 0, formaPago: "T");

        Assert.True(folio >= 999001, $"El folio de transferencia debe partir de 999001; se obtuvo {folio}.");
    }

    [HechoConBdLocal]
    public async Task RecuperaJsonPagoCaja_GeneraFragmentoConContratoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var pago = await db.PagoCaja.AsNoTracking().FirstOrDefaultAsync();
        if (pago is null)
            return; // BD sin pagos: nada que caracterizar

        var servicio = new PagoCajaService(db);
        var json = await servicio.RecuperaJsonPagoCajaAEnviarAsync(pago, bloque: "MX1", noCaja: 1, idSucursal: 10);

        // Igual que el original: abre {"pago_caja": ... pero NO cierra la llave exterior.
        Assert.StartsWith("{\"pago_caja\": {", json);
        Assert.Contains(",\"pago_caja_forma\": {", json);
        Assert.Contains(",\"pago_caja_detalle\": [", json);
        Assert.False(json.EndsWith("}}"), "El fragmento no debe cerrar la llave exterior.");

        // Cerrándolo manualmente debe ser JSON válido con el contrato esperado.
        using var doc = JsonDocument.Parse(json + "}");
        var pc = doc.RootElement.GetProperty("pago_caja");
        Assert.Equal("MX1", pc.GetProperty("bloque").GetString());
        Assert.True(pc.TryGetProperty("FOLIO_RECIBO", out _)); // mayúsculas históricas
    }

    [HechoConBdLocal]
    public async Task RecuperaPagoCajaPorId_ReplicaComportamientoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idExistente = await db.PagoCaja.Select(p => p.IdLocal).FirstOrDefaultAsync();
        var servicio = new PagoCajaService(db);

        if (idExistente > 0)
        {
            var pago = await servicio.RecuperaPagoCajaPorIdAsync(idExistente);
            Assert.NotNull(pago);
            Assert.Null(pago!.IdPago);        // el BL original nunca lo poblaba
            Assert.NotNull(pago.EsEuroskin);  // default 0 cuando venía null
        }

        Assert.Null(await servicio.RecuperaPagoCajaPorIdAsync(-99999));
    }

    [HechoConBdLocal]
    public async Task RecuperaTotalPagosPorCliente_ClienteInexistente_DevuelveCero()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PagoCajaService(db);

        var total = await servicio.RecuperaTotalPagosPorClienteAsync(-99999);

        Assert.Equal(0m, total);
    }
}
