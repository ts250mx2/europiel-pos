using EuropielPos.Data.Services;
using EuropielPos.Domain.Diagnostico;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de los últimos BL portados: EscritorLog (con directorio temporal)
/// y lecturas de Documento/ReconocimientoFacial contra la BD local.
/// </summary>
public class ServiciosRestantesTests
{
    [Fact]
    public void EscritorLog_CreaArchivoYAgregaLineas()
    {
        var dir = Path.Combine(Path.GetTempPath(), "europiel-log-test-" + Guid.NewGuid());
        try
        {
            var log = new EscritorLog(dir);

            log.EscribirEnLog("línea 1", "prueba");
            log.EscribirEnLog("línea 2", "prueba");

            var lineas = File.ReadAllLines(Path.Combine(dir, "prueba.log"));
            Assert.Equal(["línea 1", "línea 2"], lineas);
        }
        finally
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void EscritorLog_NuncaLanzaExcepcion()
    {
        // Ruta inválida: el original tragaba el error y seguía.
        var log = new EscritorLog("Z:\\ruta\\que\\no\\existe\\" + Guid.NewGuid());

        var ex = Record.Exception(() => log.EscribirEnLog("mensaje", "prueba"));

        Assert.Null(ex);
    }

    [HechoConBdLocal]
    public async Task RecuperaDocumentosPorEnviar_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new DocumentoService(db);

        var tabla = await servicio.RecuperaDocumentosPorEnviarAsync();

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaLogSucursalEstatus_SinRegistroDelDia_LoCreaConEstatusTrue()
    {
        // Comportamiento real del SP (caracterizado): si no hay registro del
        // día para ese gio_inicio_turno, lo INSERTA con estatus=1 y devuelve true.
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new ReconocimientoFacialService(db);

        // gio_inicio_turno único para no chocar con datos reales.
        var turnoUnico = new DateTime(1900, 1, 1).AddSeconds(Random.Shared.Next(1, 86400));
        try
        {
            var estatus = await servicio.RecuperaLogSucursalEstatusAsync(-99999, "TEST", turnoUnico, -1);

            Assert.True(estatus);
        }
        finally
        {
            await db.Database.ExecuteSqlAsync(
                $"DELETE FROM log_sucursal_intentos_checkin WHERE id_sucursal = {-99999}");
        }
    }

    [HechoConBdLocal]
    public async Task RecuperaReconocimientoFacialPendiente_DevuelveCadena()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new ReconocimientoFacialService(db);

        var idFeedBack = await servicio.RecuperaReconocimientoFacialPendienteAsync("X");

        Assert.NotNull(idFeedBack);
    }
}
