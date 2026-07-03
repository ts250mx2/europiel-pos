using EuropielPos.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EuropielPos.Data.Tests;

/// <summary>
/// Pruebas de caracterización de <see cref="PacienteService"/> contra la BD local.
/// Solo se prueban métodos de lectura; los que mutan datos
/// (LimpiarPacientePaso, ProcesaPacientePaso, ActualizaDatosPaciente)
/// se validarán cuando se porte el flujo de sincronización completo.
/// </summary>
public class PacienteServiceTests
{
    [HechoConBdLocal]
    public async Task BuscaPaciente_DevuelveListaSinExcepcion()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PacienteService(db);

        var resultado = await servicio.BuscaPacienteAsync("a", idSucursal: 0, idSucursalAgendaExterna: 0);

        Assert.NotNull(resultado);
        // Si hay resultados, el mapeo debe traer los campos base sin nulls inesperados.
        foreach (var p in resultado.Take(5))
        {
            Assert.True(p.IdLocal > 0);
            Assert.NotNull(p.Nombre);
        }
    }

    [HechoConBdLocal]
    public async Task RecuperaPacientePorId_GeneraFragmentoJsonConFormatoOriginal()
    {
        await using var db = BaseDatosLocal.CrearContexto();

        var idExistente = await db.Paciente.Select(p => p.IdLocal).FirstOrDefaultAsync();
        Assert.True(idExistente > 0, "La tabla paciente está vacía.");

        var servicio = new PacienteService(db);
        var json = await servicio.RecuperaPacientePorIdAsync(idExistente, numeroPaciente: 1);

        Assert.StartsWith(",\"paciente1\":{", json);
        Assert.Contains($"\"id_local\": \"{idExistente}\"", json);
        Assert.Contains("\"id_sucursal_2\":", json);
    }

    [HechoConBdLocal]
    public async Task ValidaNombrePaciente_DevuelveDataTable()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PacienteService(db);

        var tabla = await servicio.ValidaNombrePacienteAsync("ZZZZ", "ZZZZ", "ZZZZ");

        Assert.NotNull(tabla);
    }

    [HechoConBdLocal]
    public async Task RecuperaPacienteAEnviar_DevuelveListaSinExcepcion()
    {
        await using var db = BaseDatosLocal.CrearContexto();
        var servicio = new PacienteService(db);

        var pendientes = await servicio.RecuperaPacienteAEnviarAsync();

        Assert.NotNull(pendientes);
    }
}
