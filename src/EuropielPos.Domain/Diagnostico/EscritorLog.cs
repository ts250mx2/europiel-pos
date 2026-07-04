namespace EuropielPos.Domain.Diagnostico;

/// <summary>
/// Port de <c>WriteLog.vb</c> — bitácora en archivo con rotación al superar
/// 2 MB. La ruta base es configurable (el original la tenía fija en
/// <c>C:\EuropielPOS\Logs</c>); en el futuro debería migrarse a ILogger.
/// Igual que el original, nunca lanza excepción si falla la escritura.
/// </summary>
public class EscritorLog
{
    private const int TamanoMaximoLog = 2_000_000;

    private readonly string _rutaBase;

    public EscritorLog(string rutaBase = @"C:\EuropielPOS\Logs") => _rutaBase = rutaBase;

    public void EscribirEnLog(string mensaje, string nombreLog)
    {
        try
        {
            Directory.CreateDirectory(_rutaBase);

            string ruta = Path.Combine(_rutaBase, nombreLog + ".log");

            // Rotación: al superar el tamaño máximo se renombra con marca de hora.
            if (ObtenerTamano(ruta) > TamanoMaximoLog && File.Exists(ruta))
            {
                string respaldo = Path.Combine(_rutaBase, $"{nombreLog}_{DateTime.Now:yyyyMMddHH}.old.log");
                File.Copy(ruta, respaldo);
                File.Delete(ruta);
            }

            File.AppendAllLines(ruta, [mensaje]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static long ObtenerTamano(string ruta) =>
        File.Exists(ruta) ? new FileInfo(ruta).Length : 1;
}
