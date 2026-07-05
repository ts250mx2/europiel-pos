using System.Data;
using System.Globalization;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services.Sincronizacion;

/// <summary>
/// Port del <c>BulkInsert</c> de modInterfaz.vb. El original convertía
/// entidades a DataTable por reflexión (propiedad = columna); aquí se toma
/// el arreglo JSON del API tal cual — sus claves son los nombres de columna
/// de la BD — y se inserta con SqlBulkCopy.
/// </summary>
public static class BulkJson
{
    /// <param name="sobrescribir">Valores a forzar en todas las filas (p. ej.
    /// <c>fecha_interfaz = ahora</c> en las cargas iniciales, como el original).</param>
    public static async Task BulkInsertAsync(PosDbContext db, string tabla, JsonElement arreglo,
        IReadOnlyDictionary<string, object?>? sobrescribir = null, CancellationToken ct = default)
    {
        if (arreglo.ValueKind != JsonValueKind.Array || arreglo.GetArrayLength() == 0)
            return;

        // Columnas: las claves del primer elemento (el API manda filas homogéneas).
        var columnas = arreglo[0].EnumerateObject().Select(p => p.Name).ToList();

        if (sobrescribir is not null)
        {
            foreach (var clave in sobrescribir.Keys)
            {
                if (!columnas.Contains(clave))
                    columnas.Add(clave);
            }
        }

        var tablaDatos = new DataTable();
        foreach (var col in columnas)
            tablaDatos.Columns.Add(col, typeof(object));

        foreach (var fila in arreglo.EnumerateArray())
        {
            var valores = new object?[columnas.Count];
            for (int i = 0; i < columnas.Count; i++)
            {
                if (sobrescribir is not null && sobrescribir.TryGetValue(columnas[i], out var forzado))
                    valores[i] = forzado ?? DBNull.Value;
                else
                    valores[i] = fila.TryGetProperty(columnas[i], out var v) ? ConvertirValor(v) : DBNull.Value;
            }

            tablaDatos.Rows.Add(valores);
        }

        var conexion = (SqlConnection)db.Database.GetDbConnection();
        bool cerrar = conexion.State == ConnectionState.Closed;
        if (cerrar)
            await conexion.OpenAsync(ct);

        try
        {
            using var bulk = new SqlBulkCopy(conexion)
            {
                DestinationTableName = tabla,
                BulkCopyTimeout = 300,
            };

            foreach (var col in columnas)
                bulk.ColumnMappings.Add(col, col);

            await bulk.WriteToServerAsync(tablaDatos, ct);
        }
        finally
        {
            if (cerrar)
                await conexion.CloseAsync();
        }
    }

    /// <summary>Convierte un JsonElement al CLR más cercano; SqlBulkCopy hace
    /// la conversión final al tipo de la columna destino.</summary>
    private static object ConvertirValor(JsonElement v) => v.ValueKind switch
    {
        JsonValueKind.Null or JsonValueKind.Undefined => DBNull.Value,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => v.TryGetInt64(out long l) ? l : v.GetDecimal(),
        JsonValueKind.String => ConvertirCadena(v.GetString()!),
        _ => v.GetRawText(),
    };

    /// <summary>Las fechas pueden venir en ISO 8601 (Json.NET) o en el formato
    /// legado <c>/Date(ms)/</c> de JavaScriptSerializer.</summary>
    private static object ConvertirCadena(string s)
    {
        if (s.StartsWith("/Date(", StringComparison.Ordinal))
        {
            var ms = s.AsSpan(6, s.IndexOf(')') - 6);
            // Puede traer offset de zona (+0600); solo se toman los milisegundos.
            int corte = ms.IndexOfAny('+', '-');
            if (corte > 0)
                ms = ms[..corte];

            return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(ms, CultureInfo.InvariantCulture)).LocalDateTime;
        }

        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha)
            && s.Length >= 10 && (s[4] == '-' || s[2] == '/'))
        {
            return fecha;
        }

        return s;
    }
}
