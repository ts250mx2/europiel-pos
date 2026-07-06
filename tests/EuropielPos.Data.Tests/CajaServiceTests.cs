using EuropielPos.Data.Services;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de <see cref="CajaService"/> contra la BD local (solo lecturas y
/// validaciones; el cobro real muta pago_caja y se probará en el piloto).
/// </summary>
public class CajaServiceTests
{
    private static CajaService Crear(EuropielPos.Data.PosDbContext db) =>
        new(db, new PagoCajaService(db), new ContextoPos());

    [HechoConBdLocal]
    public async Task Catalogos_TiposServicioYBancos_Cargan()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = Crear(db);

        var tipos = await servicio.TiposServicioAsync();
        var bancos = await servicio.BancosAsync();

        Assert.NotNull(tipos);
        Assert.NotNull(bancos);
    }

    [HechoConBdLocal]
    public async Task ServiciosPorTipo_SoloVentaPermitida()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var tipoConServicios = await db.Servicio.AsNoTracking()
            .Where(s => s.PermitirVenta == true && s.IdTipoServicio != null)
            .Select(s => s.IdTipoServicio!.Value)
            .FirstOrDefaultAsync();

        if (tipoConServicios == 0)
            return;

        var servicio = Crear(db);
        var servicios = await servicio.ServiciosPorTipoAsync(tipoConServicios);

        Assert.NotEmpty(servicios);
        Assert.All(servicios, s => Assert.True(s.PermitirVenta));
    }

    [HechoConBdLocal]
    public async Task PaquetesConSaldo_SoloConSaldoPositivo()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idPaciente = await db.Paquete.AsNoTracking()
            .Where(p => (p.SaldoTotal ?? 0) > 0 && p.IdPacienteLocal != null)
            .Select(p => p.IdPacienteLocal!.Value)
            .FirstOrDefaultAsync();

        if (idPaciente == 0)
            return;

        var servicio = Crear(db);
        var paquetes = await servicio.PaquetesConSaldoAsync(idPaciente);

        Assert.NotEmpty(paquetes);
        Assert.All(paquetes, p => Assert.True(p.SaldoTotal > 0));
    }

    [HechoConBdLocal]
    public async Task Cobrar_SinMonto_Falla()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = Crear(db);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            servicio.CobrarAsync(new CobroCaja(), idUsuario: 1));

        Assert.Contains("monto", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [HechoConBdLocal]
    public async Task Cobrar_TarjetaSinBanco_Falla()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = Crear(db);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            servicio.CobrarAsync(new CobroCaja { TarjetaCredito = 100 }, idUsuario: 1));

        Assert.Contains("banco", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
