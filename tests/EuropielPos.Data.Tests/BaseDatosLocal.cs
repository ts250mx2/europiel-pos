using EuropielPos.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Acceso a la BD local para pruebas de caracterización. La cadena de conexión
/// vive en <c>testsettings.local.json</c> (git-ignorado) o en la variable de
/// entorno <c>EUROPIEL_POS_TEST_CS</c>. Sin ella, las pruebas se omiten.
/// </summary>
public static class BaseDatosLocal
{
    public static string? CadenaConexion { get; } = Cargar();

    private static string? Cargar()
    {
        var porVariable = Environment.GetEnvironmentVariable("EUROPIEL_POS_TEST_CS");
        if (!string.IsNullOrWhiteSpace(porVariable))
            return porVariable;

        var ruta = Path.Combine(AppContext.BaseDirectory, "testsettings.local.json");
        if (!File.Exists(ruta))
            return null;

        var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(ruta));
        return json.RootElement.GetProperty("ConnectionString").GetString();
    }

    public static PosDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<PosDbContext>()
            .UseSqlServer(CadenaConexion!)
            .Options;

        return new PosDbContext(options);
    }
}

/// <summary>
/// [Fact] que se omite automáticamente cuando no hay BD local configurada.
/// </summary>
public sealed class HechoConBdLocalAttribute : FactAttribute
{
    public HechoConBdLocalAttribute()
    {
        if (BaseDatosLocal.CadenaConexion is null)
            Skip = "Sin BD local: falta testsettings.local.json o la variable EUROPIEL_POS_TEST_CS.";
    }
}
