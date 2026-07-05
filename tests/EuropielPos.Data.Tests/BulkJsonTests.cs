using System.Text.Json;
using EuropielPos.Data.Services.Sincronizacion;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas del BulkInsert JSON→SqlBulkCopy contra la tabla de paso real
/// (requerimiento_paso), con limpieza de los registros de prueba.
/// </summary>
public class BulkJsonTests
{
    [HechoConBdLocal]
    public async Task BulkInsert_InsertaFilasDesdeJsonConFechasIsoYLegadas()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        const int idLocalPrueba = -777001;

        // Mezcla fecha ISO (Json.NET) y formato legado /Date(ms)/ (JavaScriptSerializer).
        using var doc = JsonDocument.Parse($$"""
            [
              {
                "id_local": {{idLocalPrueba}},
                "concepto": "prueba bulk migración",
                "monto": 123.45,
                "fecha_alta": "2026-07-01T10:30:00",
                "fecha_vencimiento": "/Date(1783087815000)/",
                "es_borrado": false,
                "tipo": "R"
              }
            ]
            """);

        try
        {
            await BulkJson.BulkInsertAsync(db, "requerimiento_paso", doc.RootElement);

            var fila = await db.RequerimientoPaso.AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdLocal == idLocalPrueba);

            Assert.NotNull(fila);
            Assert.Equal("prueba bulk migración", fila!.Concepto);
            Assert.Equal(123.45m, fila.Monto);
            Assert.Equal(new DateTime(2026, 7, 1, 10, 30, 0), fila.FechaAlta);
            Assert.NotNull(fila.FechaVencimiento); // fecha legada convertida
            Assert.False(fila.EsBorrado);
        }
        finally
        {
            await db.Database.ExecuteSqlAsync($"DELETE FROM requerimiento_paso WHERE id_local = {idLocalPrueba}");
        }
    }

    [HechoConBdLocal]
    public async Task BulkInsert_SobrescribeColumnas()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        const int idLocalPrueba = -777002;
        var fechaForzada = new DateTime(2026, 1, 2, 3, 4, 5);

        using var doc = JsonDocument.Parse($$"""
            [ { "id_local": {{idLocalPrueba}}, "concepto": "con override" } ]
            """);

        try
        {
            await BulkJson.BulkInsertAsync(db, "requerimiento_paso", doc.RootElement,
                new Dictionary<string, object?> { ["fecha_interfaz"] = fechaForzada });

            var fila = await db.RequerimientoPaso.AsNoTracking()
                .FirstAsync(r => r.IdLocal == idLocalPrueba);

            Assert.Equal(fechaForzada, fila.FechaInterfaz);
        }
        finally
        {
            await db.Database.ExecuteSqlAsync($"DELETE FROM requerimiento_paso WHERE id_local = {idLocalPrueba}");
        }
    }

    [Fact]
    public async Task BulkInsert_ArregloVacio_NoHaceNada()
    {
        using var doc = JsonDocument.Parse("[]");

        // Sin BD: no debe intentar conectarse siquiera.
        await BulkJson.BulkInsertAsync(null!, "tabla_inexistente", doc.RootElement);
    }
}
