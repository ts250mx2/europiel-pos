using System.Runtime.InteropServices;

namespace EuropielPos.Data.Services.Impresion;

/// <summary>
/// Port de <c>RawPrinterHelper.vb</c>: manda bytes crudos (ESC/POS) a una
/// impresora de Windows por nombre — el camino correcto para impresoras de
/// tickets, sin drivers de página ni PDF.
/// </summary>
public static partial class ImpresoraRaw
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string? pDocName;
        [MarshalAs(UnmanagedType.LPStr)] public string? pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)] public string? pDataType;
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

    [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    /// <summary>Envía bytes crudos a la impresora indicada.</summary>
    public static bool EnviaBytes(string impresora, byte[] bytes)
    {
        var di = new DOCINFOA
        {
            pDocName = "EuropielPOS",
            pOutputFile = null,
            pDataType = "RAW",
        };

        bool exito = false;
        IntPtr pBytes = Marshal.AllocCoTaskMem(bytes.Length);

        try
        {
            Marshal.Copy(bytes, 0, pBytes, bytes.Length);

            if (OpenPrinter(impresora.Normalize(), out IntPtr hPrinter, IntPtr.Zero))
            {
                try
                {
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            exito = WritePrinter(hPrinter, pBytes, bytes.Length, out int escritos)
                                    && escritos == bytes.Length;
                            EndPagePrinter(hPrinter);
                        }

                        EndDocPrinter(hPrinter);
                    }
                }
                finally
                {
                    ClosePrinter(hPrinter);
                }
            }
        }
        finally
        {
            Marshal.FreeCoTaskMem(pBytes);
        }

        return exito;
    }

    /// <summary>Envía texto (codificado como ANSI/CP-850 apto para tickets).</summary>
    public static bool EnviaTexto(string impresora, string texto)
    {
        // CP-850 conserva acentos y ñ en la mayoría de impresoras térmicas.
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        byte[] bytes = System.Text.Encoding.GetEncoding(850).GetBytes(texto);

        return EnviaBytes(impresora, bytes);
    }
}
