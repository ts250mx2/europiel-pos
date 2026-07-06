using EuropielPos.Data.Services.Impresion;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas del formato del ticket de recibo (lógica pura, sin impresora).
/// </summary>
public class ReciboTicketTests
{
    private static ReciboDatos Datos() => new()
    {
        Empresa = "Europiel",
        RazonSocial = "Europiel Láser Center SA de CV",
        Rfc = "ELC010101ABC",
        Sucursal = "MTY Centro",
        Direccion = "Av. Constitución 1500, Col. Centro, Monterrey NL",
        Ciudad = "Monterrey",
        Folio = 4521,
        Fecha = new DateTime(2026, 7, 6, 14, 35, 0),
        Cliente = "María Guadalupe Rodríguez Hernández",
        Efectivo = 500,
        TarjetaCredito = 1000,
        Total = 1500,
        Moneda = "MXN",
        Cajero = "cajera1",
        Leyenda1 = "Este recibo es su comprobante de pago",
        Leyenda2 = "Conserve este documento",
    };

    [Fact]
    public void GeneraTexto_NingunaLineaExcedeLasColumnas()
    {
        string texto = ReciboTicketService.GeneraTexto(Datos());

        foreach (string linea in texto.Split('\n'))
            Assert.True(linea.TrimEnd('\r').Length <= ReciboTicketService.Columnas,
                $"Línea de {linea.TrimEnd('\r').Length} columnas: '{linea}'");
    }

    [Fact]
    public void GeneraTexto_ContieneDatosClave()
    {
        string texto = ReciboTicketService.GeneraTexto(Datos());

        Assert.Contains("EUROPIEL", texto);
        Assert.Contains("4521", texto);
        Assert.Contains("06/07/2026 14:35", texto);
        Assert.Contains("María Guadalupe", texto);
        Assert.Contains("EFECTIVO", texto);
        Assert.Contains("T. CREDITO", texto);
        Assert.Contains("$1,500.00", texto);
        Assert.Contains("cajera1", texto);
        Assert.Contains("GRACIAS POR SU PREFERENCIA", texto);
    }

    [Fact]
    public void GeneraTexto_FormasEnCeroNoSeImprimen()
    {
        var datos = Datos();
        datos.TarjetaDebito = 0;
        datos.Transferencia = 0;

        string texto = ReciboTicketService.GeneraTexto(datos);

        Assert.DoesNotContain("T. DEBITO", texto);
        Assert.DoesNotContain("TRANSFERENCIA", texto);
    }

    [Fact]
    public void GeneraTexto_DireccionLargaSeEnvuelve()
    {
        var datos = Datos();
        datos.Direccion = new string('X', 100); // sin espacios: corte duro

        string texto = ReciboTicketService.GeneraTexto(datos);

        foreach (string linea in texto.Split('\n'))
            Assert.True(linea.TrimEnd('\r').Length <= ReciboTicketService.Columnas);
    }
}
