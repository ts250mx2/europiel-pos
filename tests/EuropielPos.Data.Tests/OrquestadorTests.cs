using EuropielPos.Data.Services.Sincronizacion;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de las piezas puras del orquestador y extras del motor.
/// </summary>
public class OrquestadorTests
{
    [Fact]
    public void ObtenerMontoRandom_SiempreEnRango_YNuncaElTope()
    {
        // El original usaba Random.Next(0, count-1): el último valor (10)
        // jamás se elegía. Se replica ese comportamiento.
        for (int i = 0; i < 500; i++)
        {
            var monto = InterfazTransaccionesService.ObtenerMontoRandom();

            Assert.InRange(monto, 1m, 9.5m);
            Assert.Equal(0m, monto * 2 % 1); // múltiplos de 0.5
        }
    }
}
