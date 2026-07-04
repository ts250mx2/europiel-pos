using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace EuropielPos.Data.Services;

/// <summary>
/// Configuración de AWS S3 (sección <c>AwsS3</c> de appsettings). Las llaves
/// reales viven en <c>appsettings.Local.json</c> — el BL original las tenía
/// hardcodeadas en el código.
/// </summary>
public class AwsS3Settings
{
    public string AccessKeyId { get; set; } = string.Empty;

    public string SecretAccessKey { get; set; } = string.Empty;

    public string BucketName { get; set; } = "europiel-system-files";

    public string Region { get; set; } = "us-east-1";
}

public class ArchivoSubidoS3
{
    public string Id { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Port de <c>EuropielAmazonS3.vb</c> — subida de documentos escaneados al
/// bucket de la empresa.
/// </summary>
public interface IS3Service
{
    Task<ArchivoSubidoS3?> SendFileToS3Async(string rutaArchivo, string carpeta, CancellationToken ct = default);

    Task<string> DeleteFileAsync(string carpeta, string nombreArchivo, CancellationToken ct = default);
}

public class S3Service : IS3Service
{
    private readonly AwsS3Settings _settings;

    public S3Service(IOptions<AwsS3Settings> settings) => _settings = settings.Value;

    /// <returns>Id (GUID + extensión) y URL pública del archivo, o <c>null</c>
    /// si falló la subida (el original tragaba el error y devolvía Nothing).</returns>
    public async Task<ArchivoSubidoS3?> SendFileToS3Async(string rutaArchivo, string carpeta, CancellationToken ct = default)
    {
        try
        {
            using var cliente = CrearCliente();

            var archivo = new ArchivoSubidoS3
            {
                Id = Guid.NewGuid() + Path.GetExtension(rutaArchivo),
            };

            string key = carpeta.Length > 0 ? $"{carpeta}/{archivo.Id}" : archivo.Id;
            archivo.Url = $"https://{_settings.BucketName}.s3.amazonaws.com/{key}";

            var peticion = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                FilePath = rutaArchivo,
                CannedACL = S3CannedACL.PublicRead, // igual que el original: el bucket sirve las URLs públicas
            };

            await cliente.PutObjectAsync(peticion, ct);

            return archivo;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al subir archivo a S3: {0}", e.Message);
            return null;
        }
    }

    /// <returns>"ok" si se borró; el mensaje de error en caso contrario
    /// (el original devolvía "False" por un bug de Option Strict Off).</returns>
    public async Task<string> DeleteFileAsync(string carpeta, string nombreArchivo, CancellationToken ct = default)
    {
        try
        {
            using var cliente = CrearCliente();

            var peticion = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = $"{carpeta}/{nombreArchivo}",
            };

            await cliente.DeleteObjectAsync(peticion, ct);

            return "ok";
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al borrar archivo de S3: {0}", e.Message);
            return e.Message;
        }
    }

    private AmazonS3Client CrearCliente() =>
        new(new BasicAWSCredentials(_settings.AccessKeyId, _settings.SecretAccessKey),
            RegionEndpoint.GetBySystemName(_settings.Region));
}
