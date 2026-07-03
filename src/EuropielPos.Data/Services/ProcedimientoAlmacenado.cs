using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

/// <summary>
/// Ejecución de stored procedures que devuelven filas. EF Core no puede
/// componer <c>EXEC</c> dentro de <c>SqlQuery&lt;T&gt;</c>, así que se usa
/// ADO.NET directo sobre la conexión del DbContext.
/// </summary>
internal static class ProcedimientoAlmacenado
{
    public static async Task<List<T>> ListaAsync<T>(
        PosDbContext db,
        string nombre,
        IReadOnlyDictionary<string, object?> parametros,
        Func<DbDataReader, T> mapear,
        CancellationToken ct = default)
    {
        var lista = new List<T>();

        await EjecutarConReaderAsync(db, nombre, parametros, async reader =>
        {
            while (await reader.ReadAsync(ct))
                lista.Add(mapear(reader));
        }, ct);

        return lista;
    }

    public static async Task<DataTable> TablaAsync(
        PosDbContext db,
        string nombre,
        IReadOnlyDictionary<string, object?> parametros,
        CancellationToken ct = default)
    {
        var tabla = new DataTable();

        await EjecutarConReaderAsync(db, nombre, parametros, reader =>
        {
            tabla.Load(reader);
            return Task.CompletedTask;
        }, ct);

        return tabla;
    }

    private static async Task EjecutarConReaderAsync(
        PosDbContext db,
        string nombre,
        IReadOnlyDictionary<string, object?> parametros,
        Func<DbDataReader, Task> procesar,
        CancellationToken ct)
    {
        DbConnection conn = db.Database.GetDbConnection();
        bool cerrarConexion = conn.State == ConnectionState.Closed;
        if (cerrarConexion)
            await conn.OpenAsync(ct);

        try
        {
            await using DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = nombre;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 300; // mismo timeout que el SqlHelper original

            foreach (var (clave, valor) in parametros)
            {
                DbParameter p = cmd.CreateParameter();
                p.ParameterName = clave;
                p.Value = valor ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }

            await using DbDataReader reader = await cmd.ExecuteReaderAsync(ct);
            await procesar(reader);
        }
        finally
        {
            if (cerrarConexion)
                await conn.CloseAsync();
        }
    }

    // ----- Lecturas null-safe equivalentes a los "If IsDBNull(...)" del VB -----

    public static string CadenaODefecto(this DbDataReader r, string columna) =>
        r.IsDBNull(r.GetOrdinal(columna)) ? string.Empty : Convert.ToString(r[columna])!;

    public static int? EnteroONull(this DbDataReader r, string columna) =>
        r.IsDBNull(r.GetOrdinal(columna)) ? null : Convert.ToInt32(r[columna]);

    public static DateTime? FechaONull(this DbDataReader r, string columna) =>
        r.IsDBNull(r.GetOrdinal(columna)) ? null : Convert.ToDateTime(r[columna]);
}
