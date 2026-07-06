using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Impresion;

/// <summary>Datos ya resueltos para armar el ticket del recibo.</summary>
public class ReciboDatos
{
    public string Empresa { get; set; } = string.Empty;

    public string RazonSocial { get; set; } = string.Empty;

    public string Rfc { get; set; } = string.Empty;

    public string Sucursal { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Ciudad { get; set; } = string.Empty;

    public int Folio { get; set; }

    public DateTime Fecha { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public decimal Efectivo { get; set; }

    public decimal TarjetaCredito { get; set; }

    public decimal TarjetaDebito { get; set; }

    public decimal Transferencia { get; set; }

    public decimal Total { get; set; }

    public string Moneda { get; set; } = string.Empty;

    public string Cajero { get; set; } = string.Empty;

    public string Leyenda1 { get; set; } = string.Empty;

    public string Leyenda2 { get; set; } = string.Empty;
}

/// <summary>
/// Recibo de caja en formato ticket (40 columnas). Sustituye al
/// repReciboCajaTicket.rpt de Crystal: arma el texto y lo manda crudo a la
/// impresora de vouchers configurada en <c>parametro.impresora_voucher</c>.
/// </summary>
public interface IReciboTicketService
{
    /// <summary>Arma el texto del ticket de un pago registrado.</summary>
    Task<string> GeneraReciboAsync(int idPagoLocal, string cajero, CancellationToken ct = default);

    /// <summary>Imprime el ticket en la impresora de vouchers. Devuelve null
    /// si se imprimió; el motivo si no fue posible.</summary>
    Task<string?> ImprimeReciboAsync(int idPagoLocal, string cajero, CancellationToken ct = default);
}

public class ReciboTicketService : IReciboTicketService
{
    public const int Columnas = 40;

    private readonly PosDbContext _db;

    public ReciboTicketService(PosDbContext db) => _db = db;

    public async Task<string> GeneraReciboAsync(int idPagoLocal, string cajero, CancellationToken ct = default)
    {
        var datos = await CargaDatosAsync(idPagoLocal, cajero, ct);
        return GeneraTexto(datos);
    }

    public async Task<string?> ImprimeReciboAsync(int idPagoLocal, string cajero, CancellationToken ct = default)
    {
        var parametro = await _db.Parametro.AsNoTracking().FirstOrDefaultAsync(ct);
        string? impresora = parametro?.ImpresoraVoucher;

        if (string.IsNullOrWhiteSpace(impresora))
            return "No hay impresora de vouchers configurada (parametro.impresora_voucher).";

        var datos = await CargaDatosAsync(idPagoLocal, cajero, ct);
        string texto = GeneraTexto(datos);

        // ESC @ inicializa; GS V 66 corta el papel dejando margen.
        string trabajo = "\x1B\x40" + texto + "\n\n\n\n" + "\x1D\x56\x42\x00";

        return ImpresoraRaw.EnviaTexto(impresora, trabajo)
            ? null
            : $"No se pudo imprimir en '{impresora}'. Revisa que esté encendida y en línea.";
    }

    private async Task<ReciboDatos> CargaDatosAsync(int idPagoLocal, string cajero, CancellationToken ct)
    {
        var pago = await _db.PagoCaja.AsNoTracking().FirstOrDefaultAsync(p => p.IdLocal == idPagoLocal, ct)
                   ?? throw new Exception($"No existe el pago local {idPagoLocal}.");

        var forma = await _db.PagoCajaForma.AsNoTracking().FirstOrDefaultAsync(f => f.IdPagoLocal == idPagoLocal, ct);
        var parametro = await _db.Parametro.AsNoTracking().FirstOrDefaultAsync(ct);
        var sucursal = await _db.Sucursal.AsNoTracking().FirstOrDefaultAsync(s => s.IdSucursal == pago.IdSucursal, ct);

        return new ReciboDatos
        {
            Empresa = parametro?.NombreEmpresa ?? "Europiel",
            RazonSocial = parametro?.RazonSocial ?? string.Empty,
            Rfc = parametro?.Rfc ?? string.Empty,
            Sucursal = sucursal?.Descripcion ?? string.Empty,
            Direccion = sucursal?.Direccion ?? string.Empty,
            Ciudad = sucursal?.CiudadSucursal ?? string.Empty,
            Folio = pago.FolioRecibo ?? 0,
            Fecha = pago.Fecha ?? pago.FechaAlta ?? DateTime.Now,
            Cliente = pago.Nombre ?? string.Empty,
            Efectivo = forma?.PagoEfectivo ?? 0,
            TarjetaCredito = forma?.PagoTc ?? 0,
            TarjetaDebito = forma?.PagoTd ?? 0,
            Transferencia = forma?.PagoTransferencia ?? 0,
            Total = pago.Total ?? 0,
            Moneda = parametro?.Moneda ?? string.Empty,
            Cajero = cajero,
            Leyenda1 = parametro?.ReciboLeyenda1 ?? string.Empty,
            Leyenda2 = parametro?.ReciboLeyenda2 ?? string.Empty,
        };
    }

    /// <summary>Formatea el ticket a 40 columnas (puro, probado por unidad).</summary>
    public static string GeneraTexto(ReciboDatos d)
    {
        var t = new StringBuilder();

        void Linea(string s = "") => t.AppendLine(Recorta(s));
        void Centrada(string s) => t.AppendLine(Centra(s));
        void Separador() => t.AppendLine(new string('-', Columnas));
        void ParImporte(string etiqueta, decimal monto)
        {
            if (monto > 0)
                t.AppendLine(AlineaPar(etiqueta, monto.ToString("C2")));
        }

        Centrada(d.Empresa.ToUpperInvariant());
        if (d.RazonSocial.Length > 0) Centrada(d.RazonSocial);
        if (d.Rfc.Length > 0) Centrada("RFC: " + d.Rfc);
        Centrada(d.Sucursal);
        foreach (var linea in Envuelve(d.Direccion))
            Centrada(linea);
        if (d.Ciudad.Length > 0) Centrada(d.Ciudad);

        Separador();
        Centrada("RECIBO DE PAGO");
        Linea(AlineaPar("FOLIO:", d.Folio.ToString()));
        Linea(AlineaPar("FECHA:", d.Fecha.ToString("dd/MM/yyyy HH:mm")));
        Separador();

        Linea("CLIENTE:");
        foreach (var linea in Envuelve(d.Cliente))
            Linea("  " + linea);

        Separador();
        ParImporte("EFECTIVO", d.Efectivo);
        ParImporte("T. CREDITO", d.TarjetaCredito);
        ParImporte("T. DEBITO", d.TarjetaDebito);
        ParImporte("TRANSFERENCIA", d.Transferencia);
        Separador();
        t.AppendLine(AlineaPar("TOTAL " + d.Moneda, d.Total.ToString("C2")));
        Separador();

        Linea(AlineaPar("Le atendió:", d.Cajero));
        Linea();

        foreach (var linea in Envuelve(d.Leyenda1))
            Centrada(linea);
        foreach (var linea in Envuelve(d.Leyenda2))
            Centrada(linea);

        Linea();
        Centrada("¡GRACIAS POR SU PREFERENCIA!");

        return t.ToString();
    }

    private static string Recorta(string s) =>
        s.Length <= Columnas ? s : s[..Columnas];

    private static string Centra(string s)
    {
        s = Recorta(s.Trim());
        int margen = (Columnas - s.Length) / 2;
        return new string(' ', Math.Max(0, margen)) + s;
    }

    private static string AlineaPar(string izquierda, string derecha)
    {
        int espacios = Columnas - izquierda.Length - derecha.Length;
        return espacios > 0
            ? izquierda + new string(' ', espacios) + derecha
            : Recorta(izquierda + " " + derecha);
    }

    private static IEnumerable<string> Envuelve(string texto)
    {
        texto = texto.Trim();
        while (texto.Length > Columnas)
        {
            int corte = texto.LastIndexOf(' ', Columnas);
            if (corte <= 0)
                corte = Columnas;

            yield return texto[..corte].Trim();
            texto = texto[corte..].Trim();
        }

        if (texto.Length > 0)
            yield return texto;
    }
}
