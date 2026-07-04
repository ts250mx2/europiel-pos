using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>ReconocimientoFacialBL.vb</c> — bitácora de envíos al sistema
/// de reconocimiento facial y control de intentos de check-in por sucursal.
/// </summary>
public interface IReconocimientoFacialService
{
    Task<int> GuardaEnvioReconocimientoFacialAsync(int branchId, string tipo, string idFeedBack, int idUsuarioRegistro, CancellationToken ct = default);

    Task GuardaLogSucursalIntentosCheckinAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default);

    Task ActualizaLogSucursalPosponerAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default);

    Task ActualizaLogSucursalEstatusAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, bool estatus, CancellationToken ct = default);

    Task<bool> RecuperaLogSucursalEstatusAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default);

    Task ActualizaLogSucursalUsuarioRespondioNingunoAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default);

    Task<string> RecuperaReconocimientoFacialPendienteAsync(string tipo, CancellationToken ct = default);

    Task ActualizaGioReconocimientoFacialAsync(string idFeedBack, int idGio, CancellationToken ct = default);
}

public class ReconocimientoFacialService : IReconocimientoFacialService
{
    private readonly PosDbContext _db;

    public ReconocimientoFacialService(PosDbContext db) => _db = db;

    /// <returns>El id del registro insertado.</returns>
    public async Task<int> GuardaEnvioReconocimientoFacialAsync(int branchId, string tipo, string idFeedBack, int idUsuarioRegistro, CancellationToken ct = default)
    {
        var registro = new LogEnvioReconocimientoFacial
        {
            BranchId = branchId,
            Tipo = tipo,
            IdFeedBack = idFeedBack,
            FechaRegistro = DateTime.Now,
            IdUsuarioRegistro = idUsuarioRegistro,
        };

        _db.LogEnvioReconocimientoFacial.Add(registro);
        await _db.SaveChangesAsync(ct);

        return registro.Id;
    }

    public Task GuardaLogSucursalIntentosCheckinAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC guarda_log_sucursal_intentos_checkin @id_sucursal = {idSucursal}, @tipo = {tipo}, @gio_inicio_turno = {gioInicioTurno}, @id_usuario_registro = {idUsuarioRegistro}", ct);

    public Task ActualizaLogSucursalPosponerAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_log_sucursal_posponer @id_sucursal = {idSucursal}, @tipo = {tipo}, @gio_inicio_turno = {gioInicioTurno}, @id_usuario_registro = {idUsuarioRegistro}", ct);

    public Task ActualizaLogSucursalEstatusAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, bool estatus, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_log_sucursal_estatus @id_sucursal = {idSucursal}, @tipo = {tipo}, @gio_inicio_turno = {gioInicioTurno}, @id_usuario_registro = {idUsuarioRegistro}, @estatus = {estatus}", ct);

    public async Task<bool> RecuperaLogSucursalEstatusAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_log_sucursal_estatus",
            new Dictionary<string, object?>
            {
                ["@id_sucursal"] = idSucursal,
                ["@tipo"] = tipo,
                ["@gio_inicio_turno"] = gioInicioTurno,
                ["@id_usuario_registro"] = idUsuarioRegistro,
            }, ct);

        bool estatus = false;
        foreach (System.Data.DataRow fila in filas.Rows)
            estatus = fila["estatus"] is not DBNull && Convert.ToBoolean(fila["estatus"]);

        return estatus;
    }

    public Task ActualizaLogSucursalUsuarioRespondioNingunoAsync(int idSucursal, string tipo, DateTime gioInicioTurno, int idUsuarioRegistro, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_log_sucursal_usuario_respondio_ninguno @id_sucursal = {idSucursal}, @tipo = {tipo}, @gio_inicio_turno = {gioInicioTurno}, @id_usuario_registro = {idUsuarioRegistro}", ct);

    /// <returns>El último id_feed_back pendiente, o cadena vacía.</returns>
    public async Task<string> RecuperaReconocimientoFacialPendienteAsync(string tipo, CancellationToken ct = default)
    {
        var filas = await ProcedimientoAlmacenado.TablaAsync(_db, "recupera_reconocimiento_facial_pendiente",
            new Dictionary<string, object?> { ["@tipo"] = tipo }, ct);

        string idFeedBack = string.Empty;
        foreach (System.Data.DataRow fila in filas.Rows)
            idFeedBack = Convert.ToString(fila["id_feed_back"]) ?? string.Empty;

        return idFeedBack;
    }

    public Task ActualizaGioReconocimientoFacialAsync(string idFeedBack, int idGio, CancellationToken ct = default) =>
        _db.LogEnvioReconocimientoFacial
            .Where(l => l.IdFeedBack == idFeedBack)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.IdGio, idGio), ct);
}
