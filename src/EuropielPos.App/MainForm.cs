using Microsoft.AspNetCore.Components.WebView.WindowsForms;

namespace EuropielPos.App;

/// <summary>
/// Ventana anfitriona: WinForms por fuera (acceso a impresoras, pinpad y
/// hardware del punto de venta), Blazor + MudBlazor por dentro.
/// </summary>
public partial class MainForm : Form
{
    public MainForm(IServiceProvider servicios)
    {
        InitializeComponent();

        var blazor = new BlazorWebView
        {
            Dock = DockStyle.Fill,
            HostPage = "wwwroot/index.html",
            Services = servicios,
        };
        blazor.RootComponents.Add<Ui.Main>("#app");

        Controls.Add(blazor);
    }
}
