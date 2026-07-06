using EuropielPos.Domain.Sesion;
using EuropielPos.Domain.Sincronizacion;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port del flujo de login de <c>frmLogin.vb</c>: valida contra la tabla
/// local <c>usuario</c> y llena la sesión y el contexto de la caja desde
/// <c>parametro</c> y <c>sucursal</c> (lo que el original metía en modGeneral).
/// </summary>
public interface IAutenticacionService
{
    /// <returns>null si las credenciales son correctas; el mensaje de error si no.</returns>
    Task<string?> IniciarSesionAsync(string login, string password, CancellationToken ct = default);
}

public class AutenticacionService : IAutenticacionService
{
    private readonly PosDbContext _db;
    private readonly SesionPos _sesion;
    private readonly ContextoPos _contexto;
    private readonly IGeneralService _general;

    public AutenticacionService(PosDbContext db, SesionPos sesion, ContextoPos contexto, IGeneralService general)
    {
        _db = db;
        _sesion = sesion;
        _contexto = contexto;
        _general = general;
    }

    public async Task<string?> IniciarSesionAsync(string login, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            return "Ingrese usuario y contraseña!";

        // Igual que el original: comparación directa contra la tabla usuario
        // (las contraseñas llegan del servidor central en la descarga de usuarios).
        var usuario = await _db.Usuario.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EsActivo == true && x.Login == login && x.Password == password, ct);

        if (usuario is null)
            return "Usuario o contraseña incorrectos.";

        var parametro = await _db.Parametro.AsNoTracking().FirstOrDefaultAsync(ct);
        if (parametro is null)
            return "La caja no está configurada (sin registro en parametro).";

        var sucursal = await _db.Sucursal.AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdSucursal == parametro.IdSucursal, ct);

        // Contexto de la caja (el modGeneral del original).
        _contexto.IdSucursal = parametro.IdSucursal ?? 0;
        _contexto.NoCaja = parametro.NoCaja;
        _contexto.Bloque = parametro.Bloque ?? string.Empty;
        _contexto.ClaveBloque = parametro.ClaveBloque?.ToString() ?? "1";
        _contexto.IdBloque = sucursal?.IdBloque ?? 0;
        _contexto.VersionSistema = int.TryParse(parametro.VersionSistema, out int version) ? version : 0;
        _contexto.EsEuroskin = parametro.EsEuroskin ?? 0;
        _contexto.IdPais = sucursal?.IdPais ?? 0;

        // Sesión del usuario.
        _sesion.Autenticado = true;
        _sesion.IdUsuario = usuario.IdUsuario;
        _sesion.UsuarioNombre = usuario.Nombre ?? string.Empty;
        _sesion.UsuarioLogin = usuario.Login ?? string.Empty;
        _sesion.TipoUsuario = usuario.TipoUsuario ?? string.Empty;
        _sesion.IdSucursalAgendaExterna = usuario.IdSucursalAgendaExterna;
        _sesion.NombreSucursal = sucursal?.Descripcion ?? string.Empty;
        _sesion.CiudadSucursal = sucursal?.CiudadSucursal ?? string.Empty;

        // Procesos de inicio de sesión del original (SP procesos_inicio_sesion_2);
        // si fallan no bloquean el login.
        try
        {
            await _general.ProcesosInicioSesionAsync(ct);
        }
        catch
        {
            // el POS debe poder abrir aun sin este proceso
        }

        return null;
    }
}
