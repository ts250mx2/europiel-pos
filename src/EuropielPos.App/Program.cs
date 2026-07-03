using EuropielPos.Data;
using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        // Formularios: se registran para poder recibir dependencias por constructor
        builder.Services.AddTransient<MainForm>();

        using var host = builder.Build();

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}
