using System.Data;
using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>CitaBL.vb</c> (agenda y sincronización de citas).
/// </summary>
public interface ICitaService
{
    Task LimpiarCitaPasoAsync(CancellationToken ct = default);

    Task ProcesaCitaPasoAsync(CancellationToken ct = default);

    Task BorrarCitasAFuturoAsync(CancellationToken ct = default);

    Task ActualizaFechaInterfazCitaAsync(int idCitaLocal, DateTime fecha, CancellationToken ct = default);

    Task ActualizaRespuestaEnvioCitaAsync(int idCitaLocal, string estatusInterfaz, string msgEstatusInterfaz, int idCita, CancellationToken ct = default);

    Task ProcesaCitaBorradaAsync(int procesada, CancellationToken ct = default);

    Task<List<CitaServicio>> RecuperaCitaServiciosAsync(int idCita, CancellationToken ct = default);

    Task<DateTime?> RecuperaFechaFinParaEnvioCitaAsync(int idCitaLocal, CancellationToken ct = default);

    Task GuardaMostroAlertaAsync(int idCitaLocal, CancellationToken ct = default);

    Task<DataTable> RecuperaCitasNoAgendadaAsync(CancellationToken ct = default);

    Task<List<Cita>> RecuperaCitasAEnviarAsync(CancellationToken ct = default);

    Task<Cita?> RecuperaCitasAEnviarPorIdAsync(int idCita, CancellationToken ct = default);

    Task ActualizaCitaEnvioOnlineAsync(int idCitaLocal, CancellationToken ct = default);

    Task ProcesaHorariosCierreJuntasAsync(CancellationToken ct = default);

    Task ActualizaReimpresionContratoAsync(int idCita, CancellationToken ct = default);
}

public class CitaService : ICitaService
{
    private readonly PosDbContext _db;

    public CitaService(PosDbContext db) => _db = db;

    public Task LimpiarCitaPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_cita_paso", ct);

    public Task ProcesaCitaPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_cita_paso", ct);

    public Task BorrarCitasAFuturoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC borrar_citas_a_futuro", ct);

    /// <remarks>El original enviaba la fecha ya formateada como texto
    /// (<c>yyyy-MM-dd HH:mm:ss</c>); se conserva ese contrato con el SP.</remarks>
    public Task ActualizaFechaInterfazCitaAsync(int idCitaLocal, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_cita @id_cita_local = {idCitaLocal}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss")}", ct);

    public Task ActualizaRespuestaEnvioCitaAsync(int idCitaLocal, string estatusInterfaz, string msgEstatusInterfaz, int idCita, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_respuesta_envio_cita @id_cita_local = {idCitaLocal}, @estatus_interfaz = {estatusInterfaz}, @msg_estatus_interfaz = {msgEstatusInterfaz}, @id_cita = {idCita}", ct);

    public Task ProcesaCitaBorradaAsync(int procesada, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_cita_borrada @procesada = {procesada}", ct);

    /// <remarks>Servicios de la cita o de sus citas hijas (empalmadas).</remarks>
    public Task<List<CitaServicio>> RecuperaCitaServiciosAsync(int idCita, CancellationToken ct = default) =>
        (from cs in _db.CitaServicio.AsNoTracking()
         join c in _db.Cita on cs.IdCitaLocal equals c.IdLocal
         where c.IdLocal == idCita || c.IdPadreLocal == idCita
         select cs).ToListAsync(ct);

    /// <remarks>Stored procedure <c>recupera_fecha_fin_para_envio_cita</c>. El original
    /// tronaba si el SP no devolvía filas; aquí se devuelve <c>null</c>.</remarks>
    public async Task<DateTime?> RecuperaFechaFinParaEnvioCitaAsync(int idCitaLocal, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_fecha_fin_para_envio_cita",
            new Dictionary<string, object?> { ["@id_cita_local"] = idCitaLocal }, ct);

        if (filas.Rows.Count == 0 || filas.Rows[0][0] is DBNull)
            return null;

        return Convert.ToDateTime(filas.Rows[0][0]);
    }

    /// <remarks><c>GETDATE()</c> se ejecuta en el servidor, igual que el original.</remarks>
    public Task GuardaMostroAlertaAsync(int idCitaLocal, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"UPDATE cita SET fecha_mostro_alerta = GETDATE() WHERE id_local = {idCitaLocal}", ct);

    public Task<DataTable> RecuperaCitasNoAgendadaAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_citas_no_agendadas",
            new Dictionary<string, object?>(), ct);

    /// <remarks>
    /// Citas pendientes de sincronizar: sin envío online, sin fecha de interfaz,
    /// solo citas padre y con máximo 1 semana desde su alta (regla LB20200107).
    /// </remarks>
    public async Task<List<Cita>> RecuperaCitasAEnviarAsync(CancellationToken ct = default)
    {
        var hoy = DateTime.Today;

        var citas = await _db.Cita.AsNoTracking()
            .Where(c => (c.EnvioOnline ?? 0) == 0
                        && c.FechaInterfaz == null
                        && c.IdPadreLocal == 0
                        && c.FechaAlta!.Value.Date.AddDays(7) >= hoy)
            .ToListAsync(ct);

        foreach (var c in citas)
            NormalizaCamposNoEnviados(c);

        return citas;
    }

    /// <remarks>El original tronaba si la cita no existía o ya no era enviable;
    /// aquí se devuelve <c>null</c>. También corrige la inyección SQL del original.</remarks>
    public async Task<Cita?> RecuperaCitasAEnviarPorIdAsync(int idCita, CancellationToken ct = default)
    {
        var hoy = DateTime.Today;

        var cita = await _db.Cita.AsNoTracking()
            .Where(c => c.IdLocal == idCita
                        && c.FechaInterfaz == null
                        && c.IdPadreLocal == 0
                        && c.FechaAlta!.Value.Date.AddDays(7) >= hoy)
            .FirstOrDefaultAsync(ct);

        if (cita is not null)
            NormalizaCamposNoEnviados(cita);

        return cita;
    }

    public Task ActualizaCitaEnvioOnlineAsync(int idCitaLocal, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC actualiza_cita_envio_online @id_cita_local = {idCitaLocal}", ct);

    public Task ProcesaHorariosCierreJuntasAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_mobile_horarios_cierre_juntas_paso", ct);

    public Task ActualizaReimpresionContratoAsync(int idCita, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC actualiza_cita_reimpresion_contrato @id_cita = {idCita}", ct);

    /// <summary>
    /// El BL original NO poblaba estos campos al armar la cita para envío
    /// (usuario_estatus quedaba en 0 y el resto comentado). Se replica ese
    /// comportamiento para que el payload de sincronización sea idéntico.
    /// </summary>
    private static void NormalizaCamposNoEnviados(Cita c)
    {
        c.UsuarioEstatus = 0;
        c.FechaEstatus = null;
        c.IdUsuarioServicio = null;
        c.FechaUsuarioCaptura = null;
    }
}
