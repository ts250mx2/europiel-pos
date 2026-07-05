using System.Text.Json;
using EuropielPos.Domain.Sincronizacion;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Contratos JSON de los catálogos del bloque 2b (sin red).
/// </summary>
public class CatalogosApiTests
{
    private static readonly JsonSerializerOptions Opciones = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public void HorarioCierreJuntas_DeserializaSnakeCase()
    {
        // Este payload viaja en snake_case porque el VB deserializaba
        // directo a la entidad EF.
        const string json = """
            { "Value": { "HorariosCierreJuntas": [
              { "id": 3, "id_sucursal": 7, "fecha": "2026-07-04T13:00:00", "fecha_fin": "2026-07-04T14:00:00" }
            ] } }
            """;

        var r = JsonSerializer.Deserialize<RespuestaApi<ValorHorariosCierreJuntas>>(json, Opciones);

        var h = Assert.Single(r!.Value!.HorariosCierreJuntas!);
        Assert.Equal(7, h.id_sucursal);
        Assert.Equal(new DateTime(2026, 7, 4, 14, 0, 0), h.fecha_fin);
    }

    [Fact]
    public void TipoIdentificacion_DeserializaMinusculas()
    {
        const string json = """
            { "Value": { "TipoIdentificacion": [
              { "id_tipo_identificacion": 2, "clave": "INE", "descripcion": "Credencial de elector" }
            ] } }
            """;

        var r = JsonSerializer.Deserialize<RespuestaApi<ValorTipoIdentificacion>>(json, Opciones);

        var t = Assert.Single(r!.Value!.TipoIdentificacion!);
        Assert.Equal(2, t.id_tipo_identificacion);
        Assert.Equal("INE", t.clave);
    }

    [Fact]
    public void FechaServidor_DeserializaConEpoch()
    {
        const string json = """
            { "Message": "ok", "fecha_servidor": "2026-07-05T09:30:15", "epoch": 1783087815 }
            """;

        var r = JsonSerializer.Deserialize<FechaServidorDetalleRespuesta>(json, Opciones);

        Assert.Equal(new DateTime(2026, 7, 5, 9, 30, 15), r!.fecha_servidor);
        Assert.Equal(1783087815L, r.epoch);
    }

    [Fact]
    public void ServicioYUsuario_DeserializanPascalCase()
    {
        const string jsonServicios = """
            { "Value": { "Servicios": [ { "IdServicio": 9, "Descripcion": "Axilas", "Precio": 350.75, "PermitirVenta": true } ] } }
            """;
        const string jsonUsuarios = """
            { "Value": { "Usuarios": [ { "IdUsuario": 4, "Login": "cajera1", "EsActivo": true, "TipoUsuario": "CAJA" } ] } }
            """;

        var s = Assert.Single(JsonSerializer.Deserialize<RespuestaApi<ValorServicios>>(jsonServicios, Opciones)!.Value!.Servicios!);
        var u = Assert.Single(JsonSerializer.Deserialize<RespuestaApi<ValorUsuarios>>(jsonUsuarios, Opciones)!.Value!.Usuarios!);

        Assert.Equal(350.75m, s.Precio);
        Assert.True(s.PermitirVenta);
        Assert.Equal("cajera1", u.Login);
        Assert.Equal("CAJA", u.TipoUsuario);
    }
}
