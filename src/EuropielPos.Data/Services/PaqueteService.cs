using System.Data;
using System.Text;
using EuropielPos.Domain.Paquetes;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>PaqueteBL.vb</c> (ventas de paquetes/tratamientos).
/// El original tiene ~2,500 líneas y 43 métodos; se está portando por bloques:
///   Bloque 1 (este): sincronización básica, reventa y consultas.
///   Bloque 2 (pendiente): payloads de envío (RecuperaPaquetesAEnviar, JSONs).
///   Bloque 3 (pendiente): validaciones de venta y operaciones.
/// </summary>
public interface IPaqueteService
{
    Task LimpiarPaquetePasoAsync(CancellationToken ct = default);

    Task KillPaqueteAsync(CancellationToken ct = default);

    Task ProcesaPaquetePasoAsync(CancellationToken ct = default);

    Task LimpiarPaqueteIbanErroneoPasoAsync(CancellationToken ct = default);

    Task ProcesaPaqueteIbanErroneoPasoAsync(CancellationToken ct = default);

    Task ActualizaFechaInterfazPaqueteAsync(int idPaqueteLocal, DateTime fecha, CancellationToken ct = default);

    Task ActualizaFechaInterfazPacienteAsync(int idPacienteLocal, DateTime fecha, CancellationToken ct = default);

    Task ActualizaReventaPaqueteAsync(ReventaPaquete reventa, CancellationToken ct = default);

    Task<DataTable> RecuperaClientesParaReimpresionAsync(string cliente, CancellationToken ct = default);

    Task<DataTable> RecuperaClientesParaEscaneoAsync(string cliente, DateTime fechaInicio, DateTime fechaFin, int idUsuario, CancellationToken ct = default);

    Task<string> RecuperaPaqueteFinanciamientoAsync(int idPaqueteLocal, CancellationToken ct = default);

    Task<string> RecuperaPaqueteServicioAsync(int idPaqueteLocal, CancellationToken ct = default);
}

public partial class PaqueteService : IPaqueteService
{
    private readonly PosDbContext _db;

    public PaqueteService(PosDbContext db) => _db = db;

    public Task LimpiarPaquetePasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_paquete_paso", ct);

    /// <remarks>Mata procesos bloqueados sobre las tablas de paquete
    /// (SP <c>terminar_procesos_paquete</c>).</remarks>
    public Task KillPaqueteAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC terminar_procesos_paquete", ct);

    public Task ProcesaPaquetePasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_paquete_paso", ct);

    /// <remarks>Los SPs de IBAN erróneo solo existen en las BDs de los tenants
    /// europeos (España); en el resto la llamada falla igual que en el original.</remarks>
    public Task LimpiarPaqueteIbanErroneoPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_paquete_iban_erroneo_paso", ct);

    /// <remarks>Ver <see cref="LimpiarPaqueteIbanErroneoPasoAsync"/>.</remarks>
    public Task ProcesaPaqueteIbanErroneoPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_paquete_iban_paso", ct);

    public Task ActualizaFechaInterfazPaqueteAsync(int idPaqueteLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_paquete @id_paquete_local = {idPaqueteLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    public Task ActualizaFechaInterfazPacienteAsync(int idPacienteLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_paciente @id_paciente_local = {idPacienteLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    /// <remarks>Stored procedure <c>actualiza_reventa_paquete</c> (41 parámetros);
    /// las fechas viajan como texto formateado, igual que en el original.</remarks>
    public Task ActualizaReventaPaqueteAsync(ReventaPaquete r, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.EscalarAsync(_db, "actualiza_reventa_paquete",
            new Dictionary<string, object?>
            {
                ["@id_paquete_local"] = r.IdPaqueteLocal,
                ["@anticipo"] = r.Anticipo,
                ["@costo_total"] = r.CostoTotal,
                ["@token_t1"] = r.TokenT1,
                ["@pagos_x_cubrir"] = r.PagosXCubrir,
                ["@pago_unitario"] = r.PagoUnitario,
                ["@fecha_pago_1"] = Fecha(r.FechaPago1),
                ["@fecha_pago_2"] = Fecha(r.FechaPago2),
                ["@fecha_pago_3"] = Fecha(r.FechaPago3),
                ["@fecha_pago_4"] = Fecha(r.FechaPago4),
                ["@fecha_pago_5"] = Fecha(r.FechaPago5),
                ["@fecha_pago_6"] = Fecha(r.FechaPago6),
                ["@fecha_pago_7"] = Fecha(r.FechaPago7),
                ["@fecha_pago_8"] = Fecha(r.FechaPago8),
                ["@fecha_pago_9"] = Fecha(r.FechaPago9),
                ["@fecha_pago_10"] = Fecha(r.FechaPago10),
                ["@monto_pago_1"] = r.MontoPago1,
                ["@monto_pago_2"] = r.MontoPago2,
                ["@monto_pago_3"] = r.MontoPago3,
                ["@monto_pago_4"] = r.MontoPago4,
                ["@monto_pago_5"] = r.MontoPago5,
                ["@monto_pago_6"] = r.MontoPago6,
                ["@monto_pago_7"] = r.MontoPago7,
                ["@monto_pago_8"] = r.MontoPago8,
                ["@monto_pago_9"] = r.MontoPago9,
                ["@monto_pago_10"] = r.MontoPago10,
                ["@id_paciente"] = r.IdPaciente,
                ["@id_paciente_local"] = r.IdPacienteLocal,
                ["@nombre_paciente_1"] = r.NombrePaciente1,
                ["@id_paciente_2"] = r.IdPaciente2,
                ["@id_paciente_local_2"] = r.IdPacienteLocal2,
                ["@nombre_paciente_2"] = r.NombrePaciente2,
                ["@id_paciente_3"] = r.IdPaciente3,
                ["@id_paciente_local_3"] = r.IdPacienteLocal3,
                ["@nombre_paciente_3"] = r.NombrePaciente3,
                ["@id_paciente_4"] = r.IdPaciente4,
                ["@id_paciente_local_4"] = r.IdPacienteLocal4,
                ["@nombre_paciente_4"] = r.NombrePaciente4,
                ["@id_paciente_5"] = r.IdPaciente5,
                ["@id_paciente_local_5"] = r.IdPacienteLocal5,
                ["@nombre_paciente_5"] = r.NombrePaciente5,
            }, ct);

    public Task<DataTable> RecuperaClientesParaReimpresionAsync(string cliente, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_clientes_para_reimpresion",
            new Dictionary<string, object?> { ["@cliente"] = cliente }, ct);

    public Task<DataTable> RecuperaClientesParaEscaneoAsync(string cliente, DateTime fechaInicio, DateTime fechaFin, int idUsuario, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_clientes_para_escaneo_con_pagos",
            new Dictionary<string, object?>
            {
                ["@cliente"] = cliente,
                ["@fecha_inicio"] = Fecha(fechaInicio),
                ["@fecha_fin"] = Fecha(fechaFin),
                ["@id_usuario"] = idUsuario,
            }, ct);

    /// <summary>
    /// Fragmento JSON con los financiamientos del paquete (objetos separados
    /// por coma, sin corchetes), con el mismo formato del original —
    /// incluida la clave con el typo histórico <c>id_financiemiento</c>.
    /// </summary>
    public async Task<string> RecuperaPaqueteFinanciamientoAsync(int idPaqueteLocal, CancellationToken ct = default)
    {
        var filas = await _db.PaqueteFinanciamiento.AsNoTracking()
            .Where(f => f.IdPaqueteLocal == idPaqueteLocal)
            .ToListAsync(ct);

        var detalle = new StringBuilder();

        foreach (var f in filas)
        {
            detalle.Append(
                $"{{\"id_local\": \"{f.IdLocal}\", " +
                $"\"fecha_modificacion_local\": \"{Fecha(f.FechaModificacionLocal)}\", " +
                $"\"fecha_interfaz\": \"{Fecha(f.FechaInterfaz)}\", " +
                $"\"id_financiemiento\": \"{f.IdFinanciemiento}\", " +
                $"\"id_paquete_local\": \"{f.IdPaqueteLocal}\", " +
                $"\"id_paquete\": \"{f.IdPaquete}\", " +
                $"\"num_pago\": \"{f.NumPago}\", " +
                $"\"monto\": \"{f.Monto}\", " +
                $"\"fecha\": \"{Fecha(f.Fecha)}\", " +
                $"\"tipo\": \"{f.Tipo}\", " +
                $"\"bandera_manual\": \"{f.BanderaManual}\"}},");
        }

        return QuitarComaFinal(detalle);
    }

    /// <summary>
    /// Fragmento JSON con los servicios del paquete, formato original.
    /// </summary>
    public async Task<string> RecuperaPaqueteServicioAsync(int idPaqueteLocal, CancellationToken ct = default)
    {
        var filas = await _db.PaqueteServicio.AsNoTracking()
            .Where(s => s.IdLocalPaquete == idPaqueteLocal)
            .ToListAsync(ct);

        var detalle = new StringBuilder();

        foreach (var s in filas)
        {
            detalle.Append(
                $"{{\"id_local\": \"{s.IdLocal}\", " +
                $"\"fecha_modificacion_local\": \"{Fecha(s.FechaModificacionLocal)}\", " +
                $"\"fecha_interfaz\": \"{Fecha(s.FechaInterfaz)}\", " +
                $"\"id_paquete_servicio\": \"{s.IdPaqueteServicio}\", " +
                $"\"id_local_paquete\": \"{s.IdLocalPaquete}\", " +
                $"\"id_paquete\": \"{s.IdPaquete}\", " +
                $"\"id_servicio\": \"{s.IdServicio}\", " +
                $"\"cantidad\": \"{s.Cantidad}\", " +
                $"\"id_paciente_local\": \"{s.IdPacienteLocal}\", " +
                $"\"id_paciente\": \"{s.IdPaciente}\", " +
                $"\"es_gratis\": \"{s.EsGratis}\"}},");
        }

        return QuitarComaFinal(detalle);
    }

    private static string QuitarComaFinal(StringBuilder detalle)
    {
        if (detalle.Length > 0)
            detalle.Length--;
        return detalle.ToString();
    }

    private static string? Fecha(DateTime? fecha) =>
        fecha?.ToString("yyyy-MM-dd HH:mm:ss");
}
