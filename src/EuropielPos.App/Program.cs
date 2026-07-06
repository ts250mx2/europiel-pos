using EuropielPos.Data;
using EuropielPos.Data.Services;
using EuropielPos.Data.Services.Sincronizacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace EuropielPos.App;

static class Program
{
    /// <summary>
    ///  Punto de entrada de la aplicación. Configura el host genérico
    ///  (configuración, logging e inyección de dependencias) y arranca
    ///  el formulario principal.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var builder = Host.CreateApplicationBuilder();

        // Configuración local con credenciales reales; el archivo está git-ignorado
        // y nunca debe subirse al repositorio.
        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

        // Base de datos
        builder.Services.AddDbContext<PosDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("PosDatabase")));

        // Servicios de negocio (ports de las clases BL del proyecto VB original)
        builder.Services.AddScoped<IParametroService, ParametroService>();
        builder.Services.AddScoped<ISucursalService, SucursalService>();
        builder.Services.AddScoped<IPacienteService, PacienteService>();
        builder.Services.AddScoped<ICitaService, CitaService>();
        builder.Services.AddScoped<IRequerimientoService, RequerimientoService>();
        builder.Services.AddScoped<IGioService, GioService>();
        builder.Services.AddScoped<IPagoCajaService, PagoCajaService>();
        builder.Services.AddScoped<IPaqueteService, PaqueteService>();
        builder.Services.AddScoped<IPaqueteEnvioService, PaqueteService>();
        builder.Services.AddScoped<IPaqueteValidacionesService, PaqueteService>();
        builder.Services.AddScoped<ILogInterfazService, LogInterfazService>();
        builder.Services.AddScoped<IRespuestaNetpayService, RespuestaNetpayService>();
        builder.Services.AddScoped<ICorreoService, CorreoService>();
        builder.Services.AddScoped<IBancosPosService, BancosPosService>();
        builder.Services.AddScoped<IPaqueteServicioPasoService, PaqueteServicioPasoService>();
        builder.Services.AddScoped<IParametroAnticipoService, ParametroAnticipoService>();
        builder.Services.AddScoped<IEquiposService, EquiposService>();
        builder.Services.AddScoped<IMensajeService, MensajeService>();
        builder.Services.AddScoped<IGeneralService, GeneralService>();
        builder.Services.AddScoped<IDocumentoService, DocumentoService>();
        builder.Services.AddScoped<IReconocimientoFacialService, ReconocimientoFacialService>();
        builder.Services.AddScoped<IS3Service, S3Service>();

        // UI Blazor Hybrid (MudBlazor dentro de la ventana WinForms)
        builder.Services.AddWindowsFormsBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddSingleton<EuropielPos.Domain.Sesion.SesionPos>();
        builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
        builder.Services.AddScoped<IAgendaService, AgendaService>();
        builder.Services.AddScoped<INuevaCitaService, NuevaCitaService>();

        // Motor de sincronización con el servidor central
        builder.Services.AddSingleton<EuropielPos.Domain.Sincronizacion.ContextoPos>();
        builder.Services.AddHttpClient<IClienteApiPos, ClienteApiPos>();
        builder.Services.AddScoped<IInterfazCatalogosService, InterfazCatalogosService>();
        builder.Services.AddScoped<IInterfazSucursalesService, InterfazSucursalesService>();
        builder.Services.AddScoped<IInterfazCatalogosDescargaService, InterfazCatalogosDescargaService>();
        builder.Services.AddScoped<IInterfazEnvioService, InterfazEnvioService>();
        builder.Services.AddScoped<InterfazTransaccionesService>();
        builder.Services.AddScoped<IInterfazTransaccionesService>(s => s.GetRequiredService<InterfazTransaccionesService>());
        builder.Services.AddScoped<IInterfazTransaccionesRestantes>(s => s.GetRequiredService<InterfazTransaccionesService>());
        builder.Services.AddScoped<IInterfazExtras>(s => s.GetRequiredService<InterfazTransaccionesService>());
        builder.Services.AddScoped<ISincronizacionOrquestador, SincronizacionOrquestador>();
        builder.Services.AddScoped<ICheckInGiosService, CheckInGiosService>();
        builder.Services.AddScoped<IUploadDocumentosService, UploadDocumentosService>();
        builder.Services.AddHttpClient<IFeeniciaService, FeeniciaService>();

        // Configuración tipada
        builder.Services.Configure<CorreoSettings>(builder.Configuration.GetSection("Correo"));
        builder.Services.Configure<AwsS3Settings>(builder.Configuration.GetSection("AwsS3"));
        builder.Services.Configure<SincronizacionSettings>(builder.Configuration.GetSection("Sincronizacion"));

        // Formularios: se registran para poder recibir dependencias por constructor
        builder.Services.AddTransient<MainForm>();

        using var host = builder.Build();

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}
