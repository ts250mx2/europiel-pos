using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace EuropielPos.Data.Services;

/// <summary>
/// Configuración SMTP (sección <c>Correo</c> de appsettings). La contraseña
/// real vive en <c>appsettings.Local.json</c>, nunca en el repositorio —
/// el BL original la tenía hardcodeada en el código.
/// </summary>
public class CorreoSettings
{
    public string Host { get; set; } = "smtp.sendgrid.net";

    public int Puerto { get; set; } = 587;

    public string Usuario { get; set; } = "apikey";

    public string Password { get; set; } = string.Empty;

    public string Remitente { get; set; } = "sistema@europiel.com.mx";

    public string NombreRemitente { get; set; } = "Notificacion Europiel";
}

/// <summary>
/// Port de <c>SendEmailBL.vb</c>. Conserva el contrato original: devuelve
/// "Email enviado..." si tuvo éxito, o el mensaje de error como texto
/// (los llamadores muestran ese texto al usuario).
/// </summary>
public interface ICorreoService
{
    Task<string> SendEmailAsync(string para, string bcc, string asunto, string cuerpo, string cc, CancellationToken ct = default);
}

public class CorreoService : ICorreoService
{
    private readonly CorreoSettings _settings;

    public CorreoService(IOptions<CorreoSettings> settings) => _settings = settings.Value;

    public async Task<string> SendEmailAsync(string para, string bcc, string asunto, string cuerpo, string cc, CancellationToken ct = default)
    {
        try
        {
            using var mensaje = new MailMessage();
            mensaje.From = new MailAddress(_settings.Remitente, _settings.NombreRemitente);

            // Las listas van separadas por ';' — si una dirección es inválida se
            // devuelve el error indicando cuál, igual que el original.
            foreach (var dir in Separar(para))
            {
                try { mensaje.To.Add(new MailAddress(dir)); }
                catch (Exception ex) { return $"{dir}: {ex.Message}"; }
            }

            foreach (var dir in Separar(bcc))
            {
                try { mensaje.Bcc.Add(new MailAddress(dir)); }
                catch (Exception ex) { return $"{dir}: {ex.Message}"; }
            }

            foreach (var dir in Separar(cc))
            {
                try { mensaje.CC.Add(new MailAddress(dir)); }
                catch (Exception ex) { return $"{dir}: {ex.Message}"; }
            }

            mensaje.Subject = asunto;
            mensaje.Body = cuerpo;
            mensaje.IsBodyHtml = true;
            mensaje.Priority = MailPriority.High;

            using var smtp = new SmtpClient(_settings.Host, _settings.Puerto)
            {
                Credentials = new NetworkCredential(_settings.Usuario, _settings.Password),
                EnableSsl = true,
            };

            await smtp.SendMailAsync(mensaje, ct);
        }
        catch (Exception e)
        {
            return e.Message;
        }

        return "Email enviado...";
    }

    private static IEnumerable<string> Separar(string direcciones) =>
        (direcciones ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
