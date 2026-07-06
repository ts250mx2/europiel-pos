using EuropielPos.Data.Entities;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>Paquete con saldo para abonar en caja.</summary>
public class PaqueteCobrable
{
    public int IdLocal { get; set; }

    public int? IdPaquete { get; set; }

    public DateTime? FechaCompra { get; set; }

    public decimal SaldoTotal { get; set; }

    public decimal SaldoVencido { get; set; }
}

/// <summary>Solicitud de cobro en caja.</summary>
public class CobroCaja
{
    public int IdPacienteLocal { get; set; }

    public int IdPaciente { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public decimal Efectivo { get; set; }

    public decimal TarjetaCredito { get; set; }

    public decimal TarjetaDebito { get; set; }

    public decimal Transferencia { get; set; }

    public int IdBanco { get; set; }

    /// <summary>18 = abono a paquete (convención del original); si es un
    /// servicio suelto, el id del servicio.</summary>
    public int IdProducto { get; set; } = 18;

    /// <summary>Id remoto del paquete al que se abona (0 si no aplica).</summary>
    public int IdPaquete { get; set; }

    public bool EsAnticipo { get; set; }

    public decimal Total => Efectivo + TarjetaCredito + TarjetaDebito + Transferencia;
}

/// <summary>Resultado del cobro para mostrar en pantalla.</summary>
public class ResultadoCobro
{
    public int IdPago { get; set; }

    public int Folio { get; set; }

    public decimal Total { get; set; }
}

/// <summary>
/// Cobros de caja (port del flujo de <c>frmCobranza.vb</c>): folio,
/// guarda_pago con las convenciones del original y catálogos de apoyo.
/// El envío al servidor lo hace después el ciclo de sincronización.
/// </summary>
public interface ICajaService
{
    Task<List<Bancos>> BancosAsync(CancellationToken ct = default);

    Task<List<TipoServicio>> TiposServicioAsync(CancellationToken ct = default);

    Task<List<Servicio>> ServiciosPorTipoAsync(int idTipoServicio, CancellationToken ct = default);

    Task<List<PaqueteCobrable>> PaquetesConSaldoAsync(int idPacienteLocal, CancellationToken ct = default);

    Task<ResultadoCobro> CobrarAsync(CobroCaja cobro, int idUsuario, CancellationToken ct = default);
}

public class CajaService : ICajaService
{
    private readonly PosDbContext _db;
    private readonly IPagoCajaService _pagoCaja;
    private readonly ContextoPos _contexto;

    public CajaService(PosDbContext db, IPagoCajaService pagoCaja, ContextoPos contexto)
    {
        _db = db;
        _pagoCaja = pagoCaja;
        _contexto = contexto;
    }

    public Task<List<Bancos>> BancosAsync(CancellationToken ct = default) =>
        _db.Bancos.AsNoTracking()
            .Where(b => b.IdPais == null || b.IdPais == _contexto.IdPais)
            .OrderBy(b => b.Descripcion)
            .ToListAsync(ct);

    public Task<List<TipoServicio>> TiposServicioAsync(CancellationToken ct = default) =>
        _db.TipoServicio.AsNoTracking()
            .OrderBy(t => t.IdTipoServicio)
            .ToListAsync(ct);

    public Task<List<Servicio>> ServiciosPorTipoAsync(int idTipoServicio, CancellationToken ct = default) =>
        _db.Servicio.AsNoTracking()
            .Where(s => s.IdTipoServicio == idTipoServicio && s.PermitirVenta == true)
            .OrderBy(s => s.Orden ?? int.MaxValue).ThenBy(s => s.Descripcion)
            .ToListAsync(ct);

    public Task<List<PaqueteCobrable>> PaquetesConSaldoAsync(int idPacienteLocal, CancellationToken ct = default) =>
        _db.Paquete.AsNoTracking()
            .Where(p => (p.IdPacienteLocal == idPacienteLocal
                         || p.IdPacienteLocal2 == idPacienteLocal
                         || p.IdPacienteLocal3 == idPacienteLocal
                         || p.IdPacienteLocal4 == idPacienteLocal
                         || p.IdPacienteLocal5 == idPacienteLocal)
                        && (p.SaldoTotal ?? 0) > 0)
            .OrderByDescending(p => p.FechaCompra)
            .Select(p => new PaqueteCobrable
            {
                IdLocal = p.IdLocal,
                IdPaquete = p.IdPaquete,
                FechaCompra = p.FechaCompra,
                SaldoTotal = p.SaldoTotal ?? 0,
                SaldoVencido = p.SaldoVencido ?? 0,
            })
            .ToListAsync(ct);

    public async Task<ResultadoCobro> CobrarAsync(CobroCaja cobro, int idUsuario, CancellationToken ct = default)
    {
        if (cobro.Total <= 0)
            throw new Exception("Captura el monto a cobrar.");

        if ((cobro.TarjetaCredito > 0 || cobro.TarjetaDebito > 0) && cobro.IdBanco == 0)
            throw new Exception("Selecciona el banco de la terminal para cobros con tarjeta.");

        // Serie de folios del original: los cobros 100% por transferencia
        // usan la serie 999000 con rfc TRANSFERENCIA.
        bool soloTransferencia = cobro.Transferencia > 0
            && cobro.Efectivo == 0 && cobro.TarjetaCredito == 0 && cobro.TarjetaDebito == 0;

        int folio = await _pagoCaja.RecuperaSigteFolioAsync(_contexto.EsEuroskin,
            soloTransferencia ? "T" : string.Empty, ct);

        string rfc = soloTransferencia ? "TRANSFERENCIA" : string.Empty;

        // Convenciones de frmCobranza: tipo recibo R, subtotal=total, iva 0,
        // guarda_paciente=1.
        int idPago = await _pagoCaja.GuardaPagoCajaAsync(
            idUsuario: idUsuario,
            tipoRecibo: "R",
            nombre: cobro.NombreCliente,
            domicilio: string.Empty,
            rfc: rfc,
            subtotal: cobro.Total,
            iva: 0,
            total: cobro.Total,
            pago: cobro.Total,
            folioRecibo: folio,
            idPacienteLocal: cobro.IdPacienteLocal,
            idPaciente: cobro.IdPaciente,
            guardaPaciente: 1,
            idBanco: cobro.IdBanco,
            idSucursal: _contexto.IdSucursal,
            esAnticipo: cobro.EsAnticipo ? 1 : 0,
            pagoEfectivo: cobro.Efectivo,
            pagoTc: cobro.TarjetaCredito,
            pagoTd: cobro.TarjetaDebito,
            pagoTransferencia: cobro.Transferencia,
            idProducto: cobro.IdProducto,
            idPaquete: cobro.IdPaquete,
            esEuroskin: _contexto.EsEuroskin,
            ct: ct);

        if (idPago <= 0)
            throw new Exception("El pago no se pudo registrar (guarda_pago devolvió " + idPago + ").");

        return new ResultadoCobro { IdPago = idPago, Folio = folio, Total = cobro.Total };
    }
}
