using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>GeneralBL.vb</c> — procesos de inicio de sesión y QR de usuario.
/// </summary>
public interface IGeneralService
{
    Task ProcesosInicioSesionAsync(CancellationToken ct = default);

    Task<DataTable> GeneraQrUsuarioAsync(int idUsuario, CancellationToken ct = default);
}

public class GeneralService : IGeneralService
{
    private readonly PosDbContext _db;

    public GeneralService(PosDbContext db) => _db = db;

    public Task ProcesosInicioSesionAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesos_inicio_sesion_2", ct);

    public Task<DataTable> GeneraQrUsuarioAsync(int idUsuario, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "genera_qr_usuario",
            new Dictionary<string, object?> { ["@id_usuario"] = idUsuario }, ct);
}
