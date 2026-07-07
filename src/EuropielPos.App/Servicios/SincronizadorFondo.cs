using EuropielPos.Data.Services.Sincronizacion;
using EuropielPos.Domain.Sesion;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Threading.Timer;

namespace EuropielPos.App.Servicios;

/// <summary>
/// Port de los timers de sincronización del MDIForm: <c>TimerInterfaz</c>
/// (cada 4 minutos) y <c>TimerInterfazCitas</c> (cada 2 minutos), que corrían
/// los procesos en hilos y pintaban el avance en la barra de estado.
/// Solo trabaja con sesión iniciada (el MDI existía únicamente tras el login)
/// y, a diferencia del original, evita corridas encimadas.
/// </summary>
public sealed class SincronizadorFondo : IDisposable
{
    /// <summary>TimerInterfaz.Interval = 240000.</summary>
    public static readonly TimeSpan IntervaloInterfaz = TimeSpan.FromMinutes(4);

    /// <summary>TimerInterfazCitas.Interval = 120000.</summary>
    public static readonly TimeSpan IntervaloCitas = TimeSpan.FromMinutes(2);

    private readonly IServiceScopeFactory _scopes;
    private readonly SesionPos _sesion;
    private Timer? _timer;
    private DateTime? _ultimaInterfaz;
    private DateTime? _ultimasCitas;
    private int _corriendoInterfaz;
    private int _corriendoCitas;

    public SincronizadorFondo(IServiceScopeFactory scopes, SesionPos sesion)
    {
        _scopes = scopes;
        _sesion = sesion;
    }

    /// <summary>El texto del lblStatusBar del original.</summary>
    public string UltimoMensaje { get; private set; } = "Estatus: En espera de inicio de sesión.";

    public bool Corriendo => _corriendoInterfaz == 1 || _corriendoCitas == 1;

    /// <summary>Se dispara al cambiar el mensaje o terminar una corrida.</summary>
    public event Action? Cambio;

    /// <summary>Arranca la vigilancia. El primer ciclo corre en cuanto hay
    /// sesión (como IniciaProcesoInterfaz al cargar el MDI).</summary>
    public void Iniciar() =>
        _timer ??= new Timer(_ => _ = TickAsync(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

    private async Task TickAsync()
    {
        try
        {
            if (!_sesion.Autenticado)
                return;

            if (_ultimaInterfaz is null || DateTime.Now - _ultimaInterfaz >= IntervaloInterfaz)
                await CorrerInterfazAsync();

            if (_ultimasCitas is null || DateTime.Now - _ultimasCitas >= IntervaloCitas)
                await CorrerCitasAsync();
        }
        catch
        {
            // el ciclo de fondo nunca debe tumbar la aplicación
        }
    }

    /// <summary>Corre ProcesoInterfaz una vez (también sirve como disparo manual).</summary>
    public async Task CorrerInterfazAsync()
    {
        if (Interlocked.Exchange(ref _corriendoInterfaz, 1) == 1)
            return;

        try
        {
            _ultimaInterfaz = DateTime.Now;
            Reporta("Estatus: Enviado datos al servidor...");

            using var scope = _scopes.CreateScope();
            var orquestador = scope.ServiceProvider.GetRequiredService<ISincronizacionOrquestador>();
            orquestador.MensajeTerminoEnvio += Reporta;
            await orquestador.ProcesoInterfazAsync();
        }
        catch (Exception ex)
        {
            Reporta("Estatus: Error: " + ex.Message);
        }
        finally
        {
            Interlocked.Exchange(ref _corriendoInterfaz, 0);
            Cambio?.Invoke();
        }
    }

    /// <summary>Corre ProcesoInterfazCitas una vez. La reimpresión de contratos
    /// queda fuera hasta que exista el módulo de impresión de contratos.</summary>
    public async Task CorrerCitasAsync()
    {
        if (Interlocked.Exchange(ref _corriendoCitas, 1) == 1)
            return;

        try
        {
            _ultimasCitas = DateTime.Now;
            Reporta("Estatus: Enviado datos al servidor...");

            using var scope = _scopes.CreateScope();
            var orquestador = scope.ServiceProvider.GetRequiredService<ISincronizacionOrquestador>();
            orquestador.MensajeTerminoEnvio += Reporta;
            await orquestador.ProcesoInterfazCitasAsync(imprimirContrato: null);
        }
        catch (Exception ex)
        {
            Reporta("Estatus: Error: " + ex.Message);
        }
        finally
        {
            Interlocked.Exchange(ref _corriendoCitas, 0);
            Cambio?.Invoke();
        }
    }

    private void Reporta(string mensaje)
    {
        UltimoMensaje = mensaje;
        Cambio?.Invoke();
    }

    public void Dispose() => _timer?.Dispose();
}
