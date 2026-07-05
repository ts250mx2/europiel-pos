using System.Data;
using EuropielPos.Data.Services;
using EuropielPos.Domain.Diagnostico;
using EuropielPos.Domain.Sincronizacion;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Port de la clase <c>UploadDocumentos</c> de modInterfaz.vb: sube los
/// documentos escaneados pendientes a S3, registra el documento en el
/// servidor central y limpia los archivos locales.
/// La ruta de escaneo era fija (<c>C:\EuroScan\uploads</c>); aquí es configurable.
/// </summary>
public interface IUploadDocumentosService
{
    Task IniciaEnvioDocumentosAsync(string claveBloque, CancellationToken ct = default);

    void BorrarDocumentosAntiguos();
}

public class UploadDocumentosService : IUploadDocumentosService
{
    private readonly IClienteApiPos _api;
    private readonly IDocumentoService _documento;
    private readonly IS3Service _s3;
    private readonly IInterfazEnvioService _envio;
    private readonly EscritorLog _log;
    private readonly string _rutaUploads;

    public UploadDocumentosService(IClienteApiPos api, IDocumentoService documento, IS3Service s3,
        IInterfazEnvioService envio, string rutaUploads = @"C:\EuroScan\uploads")
    {
        _api = api;
        _documento = documento;
        _s3 = s3;
        _envio = envio;
        _rutaUploads = rutaUploads;
        _log = new EscritorLog();
    }

    public async Task IniciaEnvioDocumentosAsync(string claveBloque, CancellationToken ct = default)
    {
        var mensajes = new System.Text.StringBuilder();
        void Mensaje(string m) => mensajes.AppendLine($"[{DateTime.Now}] \t{m}");

        try
        {
            Mensaje($"INICIANDO PROCESO {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");

            if (!await _api.HayConexionAsync(ct))
            {
                Mensaje($"Sin conexion a internet {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
            }
            else
            {
                DataTable documentos = await _documento.RecuperaDocumentosPorEnviarAsync(ct);
                Mensaje("Documentos por enviar: " + documentos.Rows.Count);

                foreach (DataRow dr in documentos.Rows)
                {
                    try
                    {
                        Mensaje($"Inicio Envio: {dr["nombre_archivo"]} {DateTime.Now:HH:mm:ss.fff}");

                        await EnviaDocumentoV2Async(
                            Convert.ToInt32(dr["id"]),
                            Convert.ToInt32(dr["id_tipo_documento"]),
                            Convert.ToInt32(dr["id_relacion"]),
                            Convert.ToInt32(dr["id_relacion_local"]),
                            Convert.ToInt32(dr["id_usuario_alta"]),
                            Convert.ToString(dr["nombre_archivo"])!,
                            Convert.ToString(dr["referencia"]) ?? string.Empty,
                            Convert.ToInt32(dr["id_sucursal_interfaz"]),
                            Convert.ToInt32(dr["no_caja_interfaz"]),
                            Mensaje, ct);

                        Mensaje($"Fin Envio: {dr["nombre_archivo"]} {DateTime.Now:HH:mm:ss.fff}");
                    }
                    catch (Exception ex)
                    {
                        Mensaje($"Error for each: {ex.Message}{DateTime.Now:HH:mm:ss.fff}");
                    }
                }
            }

            Mensaje($"TERMINANDO PROCESO {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
        }
        catch (Exception ex)
        {
            Mensaje(ex.Message);
        }

        mensajes.AppendLine("=================================================================================");
        _log.EscribirEnLog(mensajes.ToString(), "enviodocumentos");
    }

    /// <summary>Sube el archivo a S3 (carpeta archivos-clientes/AAMM), registra
    /// el documento con la URL de AWS y borra el archivo local, como la V2 del
    /// original (la V1 multipart era código muerto y no se portó).</summary>
    private async Task EnviaDocumentoV2Async(int id, int idTipoDocumento, int idRelacion, int idRelacionLocal,
        int idUsuarioAlta, string nombreArchivo, string referencia, int idSucursalInterfaz, int noCajaInterfaz,
        Action<string> mensaje, CancellationToken ct)
    {
        mensaje("EnviaDocumento: inicio");

        string carpetaMes = "archivos-clientes/" + DateTime.Now.ToString("yyMM");
        var archivoAws = await _s3.SendFileToS3Async(Path.Combine(_rutaUploads, nombreArchivo), carpetaMes, ct);

        if (archivoAws is not null)
        {
            string resultado = await _envio.SaveDocumentoAwsAsync(idTipoDocumento, idRelacion, idUsuarioAlta,
                archivoAws.Id, referencia, idSucursalInterfaz, noCajaInterfaz, idRelacionLocal,
                archivoAws.Id, archivoAws.Url, ct);

            if (resultado == "OK")
            {
                mensaje("EnviaDocumento: OK");
                await _documento.ActualizaFechaInterfazDocumentoLocalAsync(id, DateTime.Now, ct);

                // Los documentos ya enviados se borran para no acumular archivos locales.
                try
                {
                    File.Delete(Path.Combine(_rutaUploads, nombreArchivo));
                }
                catch (Exception ex)
                {
                    mensaje("Error al eliminar documento " + ex.Message);
                }
            }
        }

        mensaje("EnviaDocumento: fin");
    }

    /// <summary>Borra archivos con más de 7 días (revisa máximo 50 por corrida,
    /// como el original).</summary>
    public void BorrarDocumentosAntiguos()
    {
        try
        {
            DateTime fechaMinima = DateTime.Now.AddDays(-7.0);
            int revisados = 0;

            foreach (string archivo in Directory.EnumerateFiles(_rutaUploads, "*", SearchOption.AllDirectories))
            {
                if (revisados == 50)
                    return;

                if (File.GetCreationTime(archivo) < fechaMinima)
                    File.Delete(archivo);

                revisados++;
            }
        }
        catch (Exception ex)
        {
            _log.EscribirEnLog("BorrarDocumentosAntiguos Error: " + ex.Message, "enviodocumentos");
        }
    }
}
