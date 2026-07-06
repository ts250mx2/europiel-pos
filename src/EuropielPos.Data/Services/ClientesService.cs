using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>Paquete del cliente resumido para el expediente.</summary>
public class PaqueteCliente
{
    public int IdLocal { get; set; }

    public int? IdPaquete { get; set; }

    public DateTime? FechaCompra { get; set; }

    public decimal CostoTotal { get; set; }

    public decimal Anticipo { get; set; }

    public decimal SaldoTotal { get; set; }

    public decimal SaldoVencido { get; set; }

    public bool EsReventa { get; set; }

    public int AreasPendientes { get; set; }

    public bool PendienteSincronizar { get; set; }
}

/// <summary>Cita del historial del cliente.</summary>
public class CitaCliente
{
    public DateTime? FechaInicio { get; set; }

    public string? ListaServicios { get; set; }

    public bool Asistida { get; set; }

    public string? Estatus { get; set; }
}

/// <summary>Expediente del cliente para la pantalla de Clientes.</summary>
public class ExpedienteCliente
{
    public decimal TotalPagado { get; set; }

    public decimal SaldoTotal { get; set; }

    public List<PaqueteCliente> Paquetes { get; set; } = [];

    public List<CitaCliente> UltimasCitas { get; set; } = [];
}

/// <summary>
/// Expediente del cliente: paquetes, historial de citas y totales.
/// La actualización de contacto reutiliza el SP del original
/// (<c>actualiza_datos_cliente</c> vía PacienteService).
/// </summary>
public interface IClientesService
{
    Task<ExpedienteCliente> ExpedienteAsync(int idPacienteLocal, CancellationToken ct = default);
}

public class ClientesService : IClientesService
{
    private readonly PosDbContext _db;
    private readonly IPagoCajaService _pagoCaja;

    public ClientesService(PosDbContext db, IPagoCajaService pagoCaja)
    {
        _db = db;
        _pagoCaja = pagoCaja;
    }

    public async Task<ExpedienteCliente> ExpedienteAsync(int idPacienteLocal, CancellationToken ct = default)
    {
        var expediente = new ExpedienteCliente();

        // Paquetes donde participa el cliente (puede ser cualquiera de los 5 titulares).
        expediente.Paquetes = await _db.Paquete.AsNoTracking()
            .Where(p => p.IdPacienteLocal == idPacienteLocal
                        || p.IdPacienteLocal2 == idPacienteLocal
                        || p.IdPacienteLocal3 == idPacienteLocal
                        || p.IdPacienteLocal4 == idPacienteLocal
                        || p.IdPacienteLocal5 == idPacienteLocal)
            .OrderByDescending(p => p.FechaCompra)
            .Select(p => new PaqueteCliente
            {
                IdLocal = p.IdLocal,
                IdPaquete = p.IdPaquete,
                FechaCompra = p.FechaCompra,
                CostoTotal = p.CostoTotal ?? 0,
                Anticipo = p.Anticipo ?? 0,
                SaldoTotal = p.SaldoTotal ?? 0,
                SaldoVencido = p.SaldoVencido ?? 0,
                EsReventa = p.EsReventa ?? false,
                PendienteSincronizar = p.FechaInterfaz == null,
                AreasPendientes = _db.PaqueteServicio.Count(s =>
                    s.IdLocalPaquete == p.IdLocal
                    && s.IdPacienteLocal == idPacienteLocal
                    && s.Estatus != "A"),
            })
            .ToListAsync(ct);

        expediente.SaldoTotal = expediente.Paquetes.Sum(p => p.SaldoTotal);

        // Últimas citas del cliente (historial local).
        expediente.UltimasCitas = await _db.Cita.AsNoTracking()
            .Where(c => c.IdPacienteLocal == idPacienteLocal && c.IdPadreLocal == 0)
            .OrderByDescending(c => c.FechaInicio)
            .Take(10)
            .Select(c => new CitaCliente
            {
                FechaInicio = c.FechaInicio,
                ListaServicios = c.ListaServicios,
                Asistida = c.Asistida == 1,
                Estatus = c.Estatus,
            })
            .ToListAsync(ct);

        // Total pagado por el cliente (SP del original).
        try
        {
            expediente.TotalPagado = await _pagoCaja.RecuperaTotalPagosPorClienteAsync(idPacienteLocal, ct);
        }
        catch
        {
            expediente.TotalPagado = 0; // el expediente abre aunque el SP falle
        }

        return expediente;
    }
}
