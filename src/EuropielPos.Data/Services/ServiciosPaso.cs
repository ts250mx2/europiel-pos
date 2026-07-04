using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data.Services;

// Ports de los BL pequeños de sincronización de tablas *_paso:
// BancosPOSBL.vb, PaqueteServicioBL.vb, ParametroAnticipoBL.vb y EquiposBL.vb.
// Cada uno limpia su tabla de staging y procesa lo recibido del servidor.

public interface IBancosPosService
{
    Task LimpiarBancosPosPasoAsync(CancellationToken ct = default);

    Task ProcesaBancosPosPasoAsync(CancellationToken ct = default);
}

public class BancosPosService : IBancosPosService
{
    private readonly PosDbContext _db;

    public BancosPosService(PosDbContext db) => _db = db;

    public Task LimpiarBancosPosPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_bancos_pos_paso", ct);

    public Task ProcesaBancosPosPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_bancos_pos_paso", ct);
}

public interface IPaqueteServicioPasoService
{
    Task LimpiarPaqueteServicioPasoAsync(CancellationToken ct = default);

    Task ProcesaPaqueteServicioPasoAsync(CancellationToken ct = default);
}

public class PaqueteServicioPasoService : IPaqueteServicioPasoService
{
    private readonly PosDbContext _db;

    public PaqueteServicioPasoService(PosDbContext db) => _db = db;

    public Task LimpiarPaqueteServicioPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_paquete_servicio_paso", ct);

    public Task ProcesaPaqueteServicioPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_paquete_servicio_paso", ct);
}

public interface IParametroAnticipoService
{
    Task LimpiarAnticipoMinimoPasoAsync(CancellationToken ct = default);

    Task ProcesaAnticipoMinimoPasoAsync(CancellationToken ct = default);
}

public class ParametroAnticipoService : IParametroAnticipoService
{
    private readonly PosDbContext _db;

    public ParametroAnticipoService(PosDbContext db) => _db = db;

    public Task LimpiarAnticipoMinimoPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpiar_parametros_anticipo_paso", ct);

    public Task ProcesaAnticipoMinimoPasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_anticipo_minimo_paso", ct);
}

public interface IEquiposService
{
    Task LimpiarMaquinasLaserDetallePasoAsync(CancellationToken ct = default);

    Task ProcesaMaquinasLaserDetallePasoAsync(CancellationToken ct = default);
}

public class EquiposService : IEquiposService
{
    private readonly PosDbContext _db;

    public EquiposService(PosDbContext db) => _db = db;

    public Task LimpiarMaquinasLaserDetallePasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC limpia_maquinas_laser_detalle_paso", ct);

    public Task ProcesaMaquinasLaserDetallePasoAsync(CancellationToken ct = default) =>
        _db.Database.ExecuteSqlAsync($"EXEC procesa_maquinas_laser_detalle_paso", ct);
}
