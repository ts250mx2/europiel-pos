using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>DocumentoBL.vb</c> — documentos escaneados/generados localmente
/// pendientes de subir al servidor (contratos, identificaciones, etc.).
/// </summary>
public interface IDocumentoService
{
    Task GuardaDocumentoLocalAsync(int idTipoDocumento, int idRelacion, int idRelacionLocal, int idUsuarioAlta,
        string nombreArchivo, string referencia, CancellationToken ct = default);

    Task<DataTable> RecuperaDocumentosPorEnviarAsync(CancellationToken ct = default);

    Task ActualizaFechaInterfazDocumentoLocalAsync(int id, DateTime fecha, CancellationToken ct = default);
}

public class DocumentoService : IDocumentoService
{
    private readonly PosDbContext _db;

    public DocumentoService(PosDbContext db) => _db = db;

    public Task GuardaDocumentoLocalAsync(int idTipoDocumento, int idRelacion, int idRelacionLocal, int idUsuarioAlta,
        string nombreArchivo, string referencia, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC guarda_documento_local @id_tipo_documento = {idTipoDocumento}, @id_relacion = {idRelacion}, @id_relacion_local = {idRelacionLocal}, @id_usuario_alta = {idUsuarioAlta}, @nombre_archivo = {nombreArchivo}, @referencia = {referencia}", ct);

    public Task<DataTable> RecuperaDocumentosPorEnviarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "recupera_documento_local_por_enviar",
            new Dictionary<string, object?>(), ct);

    /// <remarks>En el BL original este método se llamaba (mal)
    /// <c>ActualizaFechaInterfazPaquete</c> por un copy-paste. Este SP usa
    /// milisegundos en la fecha, a diferencia del resto.</remarks>
    public Task ActualizaFechaInterfazDocumentoLocalAsync(int id, DateTime fecha, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_fecha_interfaz_documento_local @id = {id}, @fecha = {fecha.ToString("yyyy-MM-dd HH:mm:ss.fff")}", ct);
}
