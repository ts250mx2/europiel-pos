using System.Text.Json;
using EuropielPos.Domain.Sincronizacion;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas del contrato y mapeo de GetSucursales (sin red: valida
/// deserialización del payload del API).
/// </summary>
public class InterfazSucursalesTests
{
    private static readonly JsonSerializerOptions Opciones = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public void SucursalApi_DeserializaPayloadCompleto()
    {
        const string json = """
            {
              "Message": "ok",
              "Value": {
                "Sucursales": [
                  {
                    "IdSucursal": 42, "IdPais": 1, "Descripcion": "GDL Andares",
                    "Prefijo": "GDL", "TimeZone": "America/Mexico_City",
                    "MaxPagosVendedor": 4, "AnticipoMinimo": 1500.50,
                    "Idioma": "es-MX", "Moneda": "MXN",
                    "GioInicioTurno1": "2026-07-01T08:00:00",
                    "PermitirTokenizacionConekta": true,
                    "PosProcesadorCobrosDefault": "NETPAY",
                    "ValidaToku": 1, "EsEuroskin": 0
                  }
                ]
              }
            }
            """;

        var respuesta = JsonSerializer.Deserialize<RespuestaApi<ValorSucursales>>(json, Opciones);

        var s = Assert.Single(respuesta!.Value!.Sucursales!);
        Assert.Equal(42, s.IdSucursal);
        Assert.Equal("GDL Andares", s.Descripcion);
        Assert.Equal(1500.50m, s.AnticipoMinimo);
        Assert.Equal(new DateTime(2026, 7, 1, 8, 0, 0), s.GioInicioTurno1);
        Assert.True(s.PermitirTokenizacionConekta);
        Assert.Equal("NETPAY", s.PosProcesadorCobrosDefault);
        Assert.Equal(1, s.ValidaToku);
    }

    [Fact]
    public void ContextoPos_ValoresPorDefectoDeCajaNueva()
    {
        var contexto = new ContextoPos();

        Assert.Null(contexto.NoCaja);        // caja sin configurar manda cuerpo vacío
        Assert.Equal("1", contexto.ClaveBloque); // default del original
    }
}
