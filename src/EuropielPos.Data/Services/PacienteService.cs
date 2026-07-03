using System.Data;
using System.Text;
using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Port de <c>PacienteBL.vb</c>. La mayoría de los métodos delegan en los
/// stored procedures originales para conservar el comportamiento exacto.
/// </summary>
public interface IPacienteService
{
    Task LimpiarPacientePasoAsync(CancellationToken ct = default);

    Task ProcesaPacientePasoAsync(CancellationToken ct = default);

    Task<string> RecuperaPacientePorIdAsync(int idPacienteLocal, int numeroPaciente, CancellationToken ct = default);

    Task<List<Paciente>> BuscaPacienteAsync(string texto, int idSucursal, int idSucursalAgendaExterna, CancellationToken ct = default);

    Task<List<Paquete>> BuscaPacientePaqueteAsync(string texto, int idSucursal, int idSucursalAgendaExterna, int idPaquete, CancellationToken ct = default);

    Task ActualizaDatosPacienteAsync(int idLocal, string telefono, string celular, string correo, CancellationToken ct = default);

    Task<DataTable> ValidaNombrePacienteAsync(string nombre, string apPaterno, string apMaterno, CancellationToken ct = default);

    Task<List<Paciente>> RecuperaPacienteAEnviarAsync(CancellationToken ct = default);

    Task<DataTable> BuscaAreasPorPacienteAsync(int idPaciente, string idAreasNuevas, CancellationToken ct = default);
}

public class PacienteService : IPacienteService
{
    private readonly PosDbContext _db;

    public PacienteService(PosDbContext db) => _db = db;

    public Task LimpiarPacientePasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_paciente_paso", ct);

    public Task ProcesaPacientePasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_paciente_paso", ct);

    /// <summary>
    /// Arma el fragmento JSON del paciente para el payload de sincronización,
    /// con el mismo formato que generaba el BL original.
    /// </summary>
    public async Task<string> RecuperaPacientePorIdAsync(int idPacienteLocal, int numeroPaciente, CancellationToken ct = default)
    {
        var detalle = new StringBuilder();

        var pacientes = await _db.Paciente.AsNoTracking()
            .Where(p => p.IdLocal == idPacienteLocal)
            .ToListAsync(ct);

        foreach (var p in pacientes)
        {
            // Nota: el original enviaba id_sucursal también en id_sucursal_2;
            // se conserva ese comportamiento.
            detalle.Append(
                $",\"paciente{numeroPaciente}\":{{\"id_local\": \"{p.IdLocal}\", " +
                $"\"fecha_modificacion_local\": \"{ConfiguraFecha(p.FechaModificacionLocal)}\", " +
                $"\"fecha_interfaz\": \"{ConfiguraFecha(p.FechaInterfaz)}\", " +
                $"\"id_paciente\": \"{p.IdPaciente}\", " +
                $"\"nombre\": \"{p.Nombre}\", " +
                $"\"ap_paterno\": \"{p.ApPaterno}\", " +
                $"\"ap_materno\": \"{p.ApMaterno}\", " +
                $"\"telefono_1\": \"{p.Telefono1}\", " +
                $"\"telefono_2\": \"{p.Telefono2}\", " +
                $"\"email\": \"{p.Email}\", " +
                $"\"id_sucursal\": \"{p.IdSucursal}\", " +
                $"\"id_sucursal_2\": \"{p.IdSucursal}\", " +
                $"\"fecha_alta\": \"{ConfiguraFecha(p.FechaAlta)}\", " +
                $"\"id_usuario_alta\": \"{p.IdUsuarioAlta}\" }}");
        }

        return detalle.ToString();
    }

    /// <remarks>Stored procedure <c>busca_cliente</c>.</remarks>
    public Task<List<Paciente>> BuscaPacienteAsync(string texto, int idSucursal, int idSucursalAgendaExterna, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "busca_cliente",
            new Dictionary<string, object?>
            {
                ["@texto"] = texto,
                ["@id_sucursal"] = idSucursal,
                ["@id_sucursal_agenda_externa"] = idSucursalAgendaExterna,
            },
            r => new Paciente
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                IdPaciente = r.EnteroONull("id_paciente"),
                Nombre = r.CadenaODefecto("nombre"),
                ApPaterno = r.CadenaODefecto("ap_paterno"),
                ApMaterno = r.CadenaODefecto("ap_materno"),
                Telefono1 = r.CadenaODefecto("telefono_1"),
                Telefono2 = r.CadenaODefecto("telefono_2"),
                Email = r.CadenaODefecto("email"),
                TipoIdentificacionCliente = r.CadenaODefecto("tipo_identificacion_cliente"),
                Identidad = r.CadenaODefecto("identidad"),
                IdTipoIdentificacion2 = r.EnteroONull("id_tipo_identificacion2"),
                Identidad2 = r.CadenaODefecto("identidad2"),
                Domicilio = r.CadenaODefecto("domicilio"),
                Sexo = r.CadenaODefecto("sexo"),
                FechaNacimiento = r.FechaONull("fecha_nacimiento"),
            }, ct);

    /// <remarks>Stored procedure <c>busca_paciente_paquete</c>.</remarks>
    public Task<List<Paquete>> BuscaPacientePaqueteAsync(string texto, int idSucursal, int idSucursalAgendaExterna, int idPaquete, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "busca_paciente_paquete",
            new Dictionary<string, object?>
            {
                ["@texto"] = texto,
                ["@id_sucursal"] = idSucursal,
                ["@id_sucursal_agenda_externa"] = idSucursalAgendaExterna,
                ["@id_paquete"] = idPaquete,
            },
            r => new Paquete
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                IdPaquete = Convert.ToInt32(r["id_paquete"]),
                IdPaciente = r.EnteroONull("id_paciente"),
                FechaCompra = Convert.ToDateTime(r["fecha_compra"]),
                NombrePaciente1 = r.CadenaODefecto("nombre"),
            }, ct);

    /// <remarks>Stored procedure <c>actualiza_datos_cliente</c>.</remarks>
    public Task ActualizaDatosPacienteAsync(int idLocal, string telefono, string celular, string correo, CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync(
            $"EXEC actualiza_datos_cliente @id_local = {idLocal}, @telefono = {telefono}, @celular = {celular}, @correo = {correo}", ct);

    /// <remarks>Stored procedure <c>valida_paciente</c>. Devuelve DataTable como el original;
    /// se tipará cuando se porte la pantalla que lo consume.</remarks>
    public Task<DataTable> ValidaNombrePacienteAsync(string nombre, string apPaterno, string apMaterno, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "valida_paciente",
            new Dictionary<string, object?>
            {
                ["@nombre"] = nombre,
                ["@ap_paterno"] = apPaterno,
                ["@ap_materno"] = apMaterno,
            }, ct);

    /// <remarks>Stored procedure <c>recupera_pacientes_enviar</c> (pacientes pendientes de sincronizar).</remarks>
    public Task<List<Paciente>> RecuperaPacienteAEnviarAsync(CancellationToken ct = default) =>
        ProcedimientoAlmacenado.ListaAsync(_db, "recupera_pacientes_enviar",
            new Dictionary<string, object?> { ["@fecha_hoy"] = DateTime.Today },
            r => new Paciente
            {
                IdLocal = Convert.ToInt32(r["id_local"]),
                FechaModificacionLocal = r.FechaONull("fecha_modificacion_local"),
                FechaInterfaz = r.FechaONull("fecha_interfaz"),
                IdPaciente = r.EnteroONull("id_paciente"),
                Nombre = r.CadenaODefecto("nombre"),
                ApPaterno = r.CadenaODefecto("ap_paterno"),
                ApMaterno = r.CadenaODefecto("ap_materno"),
                Telefono1 = r.CadenaODefecto("telefono_1"),
                Telefono2 = r.CadenaODefecto("telefono_2"),
                Email = r.CadenaODefecto("email"),
                IdSucursal = r.EnteroONull("id_sucursal"),
                IdSucursal2 = r.EnteroONull("id_sucursal_2"),
                FechaAlta = r.FechaONull("fecha_alta"),
                IdUsuarioAlta = r.EnteroONull("id_usuario_alta"),
                ClienteRobustoDs = r.EnteroONull("cliente_robusto_ds"),
                ClienteRobustoS = r.EnteroONull("cliente_robusto_s"),
                Estatus = r.CadenaODefecto("estatus"),
                Identidad = r.CadenaODefecto("identidad"),
            }, ct);

    /// <remarks>Stored procedure <c>busca_areas_por_paciente</c>.</remarks>
    public Task<DataTable> BuscaAreasPorPacienteAsync(int idPaciente, string idAreasNuevas, CancellationToken ct = default) =>
        ProcedimientoAlmacenado.TablaAsync(_db, "busca_areas_por_paciente",
            new Dictionary<string, object?>
            {
                ["@id_paciente"] = idPaciente,
                ["@id_areas_nuevas"] = idAreasNuevas,
            }, ct);

    /// <summary>Port de <c>ConfiguraFecha</c> de <c>modInterfaz.vb</c>.</summary>
    private static string? ConfiguraFecha(DateTime? fecha) =>
        fecha?.ToString("yyyy-MM-dd HH:mm:ss");
}
