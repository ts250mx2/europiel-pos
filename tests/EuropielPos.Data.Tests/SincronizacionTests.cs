using System.Text.Json;
using EuropielPos.Domain.Sincronizacion;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas del contrato JSON del API central (RespuestaApi y catálogos).
/// Sin llamadas de red: valida que la deserialización coincide con lo que
/// devolvía JavaScriptSerializer en el VB.
/// </summary>
public class SincronizacionTests
{
    private static readonly JsonSerializerOptions Opciones = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public void RespuestaApi_DeserializaSucursalYBloque()
    {
        const string json = """
            {
              "Message": "ok",
              "Value": {
                "SucursalBloque": [
                  { "IdSucursal": 10, "Sucursal": "MTY Centro", "IdBloque": 2, "ClaveBloque": 1, "NombreBloque": "México" }
                ]
              }
            }
            """;

        var respuesta = JsonSerializer.Deserialize<RespuestaApi<ValorSucursalBloque>>(json, Opciones);

        Assert.NotNull(respuesta);
        var item = Assert.Single(respuesta!.Value!.SucursalBloque!);
        Assert.Equal(10, item.IdSucursal);
        Assert.Equal("MTY Centro", item.Sucursal);
        Assert.Equal(1, item.ClaveBloque);
    }

    [Fact]
    public void RespuestaApi_DeserializaBloques()
    {
        const string json = """
            {
              "Message": null,
              "Value": { "Bloques": [ { "IdBloque": 5, "ClaveBloque": 3, "Nombre": "España" } ] }
            }
            """;

        var respuesta = JsonSerializer.Deserialize<RespuestaApi<ValorBloque>>(json, Opciones);

        var bloque = Assert.Single(respuesta!.Value!.Bloques!);
        Assert.Equal(5, bloque.IdBloque);
        // DetalleBloque replica el formato clave|id de los combos del original.
        Assert.Equal("3|5", bloque.DetalleBloque);
    }

    [Fact]
    public void RespuestaApi_ValueNull_NoTruena()
    {
        var respuesta = JsonSerializer.Deserialize<RespuestaApi<ValorBloque>>(
            """{ "Message": "sin datos", "Value": null }""", Opciones);

        Assert.NotNull(respuesta);
        Assert.Null(respuesta!.Value);
        Assert.Equal("sin datos", respuesta.Message);
    }
}
