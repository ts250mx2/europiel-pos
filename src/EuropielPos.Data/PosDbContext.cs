using System;
using System.Collections.Generic;
using EuropielPos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuropielPos.Data;

public partial class PosDbContext : DbContext
{
    public PosDbContext(DbContextOptions<PosDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bancos> Bancos { get; set; }

    public virtual DbSet<BancosPos> BancosPos { get; set; }

    public virtual DbSet<BancosPosPaso> BancosPosPaso { get; set; }

    public virtual DbSet<BinsTarjetaInvalida> BinsTarjetaInvalida { get; set; }

    public virtual DbSet<BonosPorSucursal> BonosPorSucursal { get; set; }

    public virtual DbSet<Cita> Cita { get; set; }

    public virtual DbSet<CitaBorrada> CitaBorrada { get; set; }

    public virtual DbSet<CitaPaso> CitaPaso { get; set; }

    public virtual DbSet<CitaServicio> CitaServicio { get; set; }

    public virtual DbSet<ConfigPagosPaquete> ConfigPagosPaquete { get; set; }

    public virtual DbSet<ConfigPrecioServicio> ConfigPrecioServicio { get; set; }

    public virtual DbSet<ControlScripts> ControlScripts { get; set; }

    public virtual DbSet<Documento> Documento { get; set; }

    public virtual DbSet<DocumentoLocal> DocumentoLocal { get; set; }

    public virtual DbSet<Estados> Estados { get; set; }

    public virtual DbSet<ExcepcionHorarioCita> ExcepcionHorarioCita { get; set; }

    public virtual DbSet<FeeniciaMpos> FeeniciaMpos { get; set; }

    public virtual DbSet<FeeniciaSubscripciones> FeeniciaSubscripciones { get; set; }

    public virtual DbSet<Gio> Gio { get; set; }

    public virtual DbSet<GioAsistencia> GioAsistencia { get; set; }

    public virtual DbSet<GioAsistenciaPaso> GioAsistenciaPaso { get; set; }

    public virtual DbSet<GioPaso> GioPaso { get; set; }

    public virtual DbSet<GioTurno> GioTurno { get; set; }

    public virtual DbSet<Interfaz> Interfaz { get; set; }

    public virtual DbSet<KushkiSmartLink> KushkiSmartLink { get; set; }

    public virtual DbSet<LogCancelacionMasiva> LogCancelacionMasiva { get; set; }

    public virtual DbSet<LogEnvioReconocimientoFacial> LogEnvioReconocimientoFacial { get; set; }

    public virtual DbSet<LogErrores> LogErrores { get; set; }

    public virtual DbSet<LogInterfaz> LogInterfaz { get; set; }

    public virtual DbSet<LogSucursalIntentosCheckin> LogSucursalIntentosCheckin { get; set; }

    public virtual DbSet<MaquinasLaserDetalle> MaquinasLaserDetalle { get; set; }

    public virtual DbSet<MaquinasLaserDetallePaso> MaquinasLaserDetallePaso { get; set; }

    public virtual DbSet<MobileAgendaBloques> MobileAgendaBloques { get; set; }

    public virtual DbSet<MobileHorariosCierreJuntas> MobileHorariosCierreJuntas { get; set; }

    public virtual DbSet<MobileHorariosCierreJuntasPaso> MobileHorariosCierreJuntasPaso { get; set; }

    public virtual DbSet<Municipios> Municipios { get; set; }

    public virtual DbSet<NetpayUrl> NetpayUrl { get; set; }

    public virtual DbSet<Paciente> Paciente { get; set; }

    public virtual DbSet<PacientePaso> PacientePaso { get; set; }

    public virtual DbSet<PacientesEliminados> PacientesEliminados { get; set; }

    public virtual DbSet<PagoCaja> PagoCaja { get; set; }

    public virtual DbSet<PagoCajaDetalle> PagoCajaDetalle { get; set; }

    public virtual DbSet<PagoCajaForma> PagoCajaForma { get; set; }

    public virtual DbSet<Paquete> Paquete { get; set; }

    public virtual DbSet<PaqueteFinanciamiento> PaqueteFinanciamiento { get; set; }

    public virtual DbSet<PaqueteLog> PaqueteLog { get; set; }

    public virtual DbSet<PaquetePaso> PaquetePaso { get; set; }

    public virtual DbSet<PaqueteServicio> PaqueteServicio { get; set; }

    public virtual DbSet<PaqueteServicioPaso> PaqueteServicioPaso { get; set; }

    public virtual DbSet<PaquetesEliminados> PaquetesEliminados { get; set; }

    public virtual DbSet<Parametro> Parametro { get; set; }

    public virtual DbSet<ParametrosAnticipoMinimo> ParametrosAnticipoMinimo { get; set; }

    public virtual DbSet<ParametrosAnticipoMinimoDetalle> ParametrosAnticipoMinimoDetalle { get; set; }

    public virtual DbSet<ParametrosAnticipoPaso> ParametrosAnticipoPaso { get; set; }

    public virtual DbSet<ParametrosGeneral> ParametrosGeneral { get; set; }

    public virtual DbSet<Requerimiento> Requerimiento { get; set; }

    public virtual DbSet<RequerimientoPaso> RequerimientoPaso { get; set; }

    public virtual DbSet<RespuestaNetpay> RespuestaNetpay { get; set; }

    public virtual DbSet<Servicio> Servicio { get; set; }

    public virtual DbSet<ServicioCombo> ServicioCombo { get; set; }

    public virtual DbSet<ServicioSucursal> ServicioSucursal { get; set; }

    public virtual DbSet<Sucursal> Sucursal { get; set; }

    public virtual DbSet<SucursalAgendaExterna> SucursalAgendaExterna { get; set; }

    public virtual DbSet<SucursalHorario> SucursalHorario { get; set; }

    public virtual DbSet<SucursalesAfines> SucursalesAfines { get; set; }

    public virtual DbSet<TipoCita> TipoCita { get; set; }

    public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }

    public virtual DbSet<TipoFalla> TipoFalla { get; set; }

    public virtual DbSet<TipoIdentificacion> TipoIdentificacion { get; set; }

    public virtual DbSet<TipoParametroAnticipoMinimo> TipoParametroAnticipoMinimo { get; set; }

    public virtual DbSet<TipoServicio> TipoServicio { get; set; }

    public virtual DbSet<TokenAutorizacion> TokenAutorizacion { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<VersionesInstaladas> VersionesInstaladas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Modern_Spanish_CI_AS");

        modelBuilder.Entity<Bancos>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bancos");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdBanco)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_banco");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
        });

        modelBuilder.Entity<BancosPos>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bancos_pos");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<BancosPosPaso>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bancos_pos_paso");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<BinsTarjetaInvalida>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("bins_tarjeta_invalida");

            entity.Property(e => e.Banco)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("banco");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Tarjeta)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta");
        });

        modelBuilder.Entity<BonosPorSucursal>(entity =>
        {
            entity.HasKey(e => e.IdBono);

            entity.ToTable("bonos_por_sucursal");

            entity.Property(e => e.IdBono).HasColumnName("id_bono");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Premio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("premio");
            entity.Property(e => e.Tipo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.TipoBono)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo_bono");
            entity.Property(e => e.Venta)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("venta");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_CITA");

            entity.ToTable("cita");

            entity.HasIndex(e => new { e.FechaInicio, e.IdPadreLocal }, "IDX_20170518_01");

            entity.HasIndex(e => e.IdPadreLocal, "IDX_20170525_01");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.AltaCita)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("alta_cita");
            entity.Property(e => e.Asistida)
                .HasDefaultValue(0)
                .HasColumnName("asistida");
            entity.Property(e => e.CitasRealmenteAsistidas).HasColumnName("citas_realmente_asistidas");
            entity.Property(e => e.ClienteAsistio).HasColumnName("cliente_asistio");
            entity.Property(e => e.EnvioOnline)
                .HasDefaultValue(0)
                .HasColumnName("envio_online");
            entity.Property(e => e.EsConsultaMedica).HasColumnName("es_consulta_medica");
            entity.Property(e => e.EsMorada).HasColumnName("es_morada");
            entity.Property(e => e.Estatus)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.EstatusInterfaz)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estatus_interfaz");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaEstatus)
                .HasColumnType("datetime")
                .HasColumnName("fecha_estatus");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaMostroAlerta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_mostro_alerta");
            entity.Property(e => e.FechaUsuarioCaptura)
                .HasColumnType("datetime")
                .HasColumnName("fecha_usuario_captura");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPadre).HasColumnName("id_padre");
            entity.Property(e => e.IdPadreLocal).HasColumnName("id_padre_local");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTipoCita).HasColumnName("id_tipo_cita");
            entity.Property(e => e.IdUsuarioServicio).HasColumnName("id_usuario_servicio");
            entity.Property(e => e.ListaServicios)
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasColumnName("lista_servicios");
            entity.Property(e => e.MsgEstatusInterfaz)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("msg_estatus_interfaz");
            entity.Property(e => e.NoSePago)
                .HasDefaultValue(false)
                .HasColumnName("no_se_pago");
            entity.Property(e => e.NumCita).HasColumnName("num_cita");
            entity.Property(e => e.ReimprimirContrato).HasColumnName("reimprimir_contrato");
            entity.Property(e => e.UsuarioAlta).HasColumnName("usuario_alta");
            entity.Property(e => e.UsuarioEstatus).HasColumnName("usuario_estatus");
        });

        modelBuilder.Entity<CitaBorrada>(entity =>
        {
            entity.ToTable("cita_borrada");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.Procesada).HasColumnName("procesada");
        });

        modelBuilder.Entity<CitaPaso>(entity =>
        {
            entity.ToTable("cita_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AltaCita)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("alta_cita");
            entity.Property(e => e.Asistida).HasColumnName("asistida");
            entity.Property(e => e.CitasRealmenteAsistidas).HasColumnName("citas_realmente_asistidas");
            entity.Property(e => e.ClienteAsistio).HasColumnName("cliente_asistio");
            entity.Property(e => e.EsConsultaMedica).HasColumnName("es_consulta_medica");
            entity.Property(e => e.EsMorada).HasColumnName("es_morada");
            entity.Property(e => e.Estatus)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.EstatusInterfaz)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estatus_interfaz");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaConsultaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_consulta_interfaz");
            entity.Property(e => e.FechaEstatus)
                .HasColumnType("datetime")
                .HasColumnName("fecha_estatus");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaUsuarioCaptura)
                .HasColumnType("datetime")
                .HasColumnName("fecha_usuario_captura");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPadre).HasColumnName("id_padre");
            entity.Property(e => e.IdPadreLocal).HasColumnName("id_padre_local");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdTipoCita).HasColumnName("id_tipo_cita");
            entity.Property(e => e.IdUsuarioServicio).HasColumnName("id_usuario_servicio");
            entity.Property(e => e.ListaServicios)
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasColumnName("lista_servicios");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.NoSePago).HasColumnName("no_se_pago");
            entity.Property(e => e.NumCita).HasColumnName("num_cita");
            entity.Property(e => e.ReimprimirContrato).HasColumnName("reimprimir_contrato");
            entity.Property(e => e.UsuarioAlta).HasColumnName("usuario_alta");
            entity.Property(e => e.UsuarioEstatus).HasColumnName("usuario_estatus");
        });

        modelBuilder.Entity<CitaServicio>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("cita_servicio");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdCita).HasColumnName("id_cita");
            entity.Property(e => e.IdCitaLocal).HasColumnName("id_cita_local");
            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.Tipo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<ConfigPagosPaquete>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK_config_pagos_paquete_id");

            entity.ToTable("config_pagos_paquete");

            entity.Property(e => e.IdDetalle)
                .ValueGeneratedNever()
                .HasColumnName("id_detalle");
            entity.Property(e => e.DiasMaximoDifEntrePagos).HasColumnName("dias_maximo_dif_entre_pagos");
            entity.Property(e => e.DiasMaximoPagos).HasColumnName("dias_maximo_pagos");
            entity.Property(e => e.MaxPagos).HasColumnName("max_pagos");
            entity.Property(e => e.NoServiciosFin).HasColumnName("no_servicios_fin");
            entity.Property(e => e.NoServiciosIni).HasColumnName("no_servicios_ini");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.VentaMinima)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("venta_minima");
        });

        modelBuilder.Entity<ConfigPrecioServicio>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK_config_precio_servicio_id");

            entity.ToTable("config_precio_servicio");

            entity.Property(e => e.IdDetalle)
                .ValueGeneratedNever()
                .HasColumnName("id_detalle");
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("descuento");
            entity.Property(e => e.NoServiciosFin).HasColumnName("no_servicios_fin");
            entity.Property(e => e.NoServiciosIni).HasColumnName("no_servicios_ini");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Tipo)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.TipoDescuento)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo_descuento");
        });

        modelBuilder.Entity<ControlScripts>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK_excepcion_horario_cita");

            entity.ToTable("control_scripts");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("nombre_archivo");
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("documento");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdDocumento).HasColumnName("id_documento");
            entity.Property(e => e.IdRelacion).HasColumnName("id_relacion");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTipoDocumento).HasColumnName("id_tipo_documento");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Referencia)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("referencia");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Documento)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("FK_documento_sucursal");

            entity.HasOne(d => d.IdTipoDocumentoNavigation).WithMany(p => p.Documento)
                .HasForeignKey(d => d.IdTipoDocumento)
                .HasConstraintName("FK_documento_tipo_documento");
        });

        modelBuilder.Entity<DocumentoLocal>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("documento_local");

            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.IdRelacion).HasColumnName("id_relacion");
            entity.Property(e => e.IdRelacionLocal).HasColumnName("id_relacion_local");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdTipoDocumento).HasColumnName("id_tipo_documento");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("nombre_archivo");
            entity.Property(e => e.Referencia)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("referencia");
        });

        modelBuilder.Entity<Estados>(entity =>
        {
            entity.HasKey(e => e.IdEstado);

            entity.ToTable("estados");

            entity.Property(e => e.IdEstado)
                .ValueGeneratedNever()
                .HasColumnName("id_estado");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.Estado)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Sigla)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("sigla");
        });

        modelBuilder.Entity<ExcepcionHorarioCita>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK_excepcion_horario_cita_2");

            entity.ToTable("excepcion_horario_cita");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("mensaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<FeeniciaMpos>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("feenicia_mpos");

            entity.Property(e => e.AcquirerBank)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("acquirerBank");
            entity.Property(e => e.Affiliation).HasColumnName("affiliation");
            entity.Property(e => e.Aid)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("aid");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Arqc)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("arqc");
            entity.Property(e => e.Caja)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("caja");
            entity.Property(e => e.CardholderName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("cardholderName");
            entity.Property(e => e.CodigoRespuesta)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo_respuesta");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FeecontrollerUrl)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("feecontroller_url");
            entity.Property(e => e.Id)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.IdBloque).HasColumnName("id_bloque");
            entity.Property(e => e.IdLocal)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_local");
            entity.Property(e => e.IdLog).HasColumnName("idLog");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Importe)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("importe");
            entity.Property(e => e.IssuerBank)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("issuerBank");
            entity.Property(e => e.Last4digits)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("last4digits");
            entity.Property(e => e.Marca)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("marca");
            entity.Property(e => e.MensajeEnvio)
                .IsUnicode(false)
                .HasColumnName("mensaje_envio");
            entity.Property(e => e.MensajeRespuesta)
                .IsUnicode(false)
                .HasColumnName("mensaje_respuesta");
            entity.Property(e => e.Merchant)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("merchant");
            entity.Property(e => e.Msi)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("msi");
            entity.Property(e => e.NumAuth)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("numAuth");
            entity.Property(e => e.OrderId)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("orderId");
            entity.Property(e => e.Propina)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("propina");
            entity.Property(e => e.ReceiptId)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("receiptId");
            entity.Property(e => e.SignatureType)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("signatureType");
            entity.Property(e => e.Terminal)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("terminal");
            entity.Property(e => e.TerminalId).HasColumnName("terminal_id");
            entity.Property(e => e.Ticket).HasColumnName("ticket");
            entity.Property(e => e.Tienda)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("tienda");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("transactionId");
            entity.Property(e => e.TypeSale)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("typeSale");
        });

        modelBuilder.Entity<FeeniciaSubscripciones>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_feenicia_subscripciones_id_local");

            entity.ToTable("feenicia_subscripciones");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CodigoRespuesta)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("codigo_respuesta");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdBloque).HasColumnName("id_bloque");
            entity.Property(e => e.IdLocalPaquete).HasColumnName("id_local_paquete");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPlan)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("id_plan");
            entity.Property(e => e.IdSubscripcion).HasColumnName("id_subscripcion");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.MensajeRespuesta)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("mensaje_respuesta");
            entity.Property(e => e.Payload)
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasColumnName("payload");
            entity.Property(e => e.PlanUnit).HasColumnName("plan_unit");

            entity.HasOne(d => d.IdLocalPaqueteNavigation).WithMany(p => p.FeeniciaSubscripciones)
                .HasForeignKey(d => d.IdLocalPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_feenicia_subscripciones_id_local_paquete");
        });

        modelBuilder.Entity<Gio>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("gio");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.Banco)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("banco");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.ClaveInterbancaria)
                .HasMaxLength(18)
                .IsUnicode(false)
                .HasColumnName("clave_interbancaria");
            entity.Property(e => e.EsActivo).HasColumnName("es_activo");
            entity.Property(e => e.EsCambioSucursal).HasColumnName("es_cambio_sucursal");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Horario)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("horario");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdGio).HasColumnName("id_gio");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.Nombre)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreCuenta)
                .HasMaxLength(130)
                .IsUnicode(false)
                .HasColumnName("nombre_cuenta");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<GioAsistencia>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("gio_asistencia");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.HoraFin)
                .HasColumnType("datetime")
                .HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("hora_inicio");
            entity.Property(e => e.Horario)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("horario");
            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.IdGio).HasColumnName("id_gio");
            entity.Property(e => e.IdGioLocal).HasColumnName("id_gio_local");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.IdUsuarioRegistro).HasColumnName("id_usuario_registro");
        });

        modelBuilder.Entity<GioAsistenciaPaso>(entity =>
        {
            entity.ToTable("gio_asistencia_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.HoraFin)
                .HasColumnType("datetime")
                .HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("hora_inicio");
            entity.Property(e => e.Horario)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("horario");
            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.IdGio).HasColumnName("id_gio");
            entity.Property(e => e.IdGioLocal).HasColumnName("id_gio_local");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.IdUsuarioRegistro).HasColumnName("id_usuario_registro");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
        });

        modelBuilder.Entity<GioPaso>(entity =>
        {
            entity.ToTable("gio_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Banco)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("banco");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.ClaveInterbancaria)
                .HasMaxLength(18)
                .IsUnicode(false)
                .HasColumnName("clave_interbancaria");
            entity.Property(e => e.EsActivo).HasColumnName("es_activo");
            entity.Property(e => e.EsCambioSucursal).HasColumnName("es_cambio_sucursal");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.Horario)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("horario");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdGio).HasColumnName("id_gio");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.Nombre)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreCuenta)
                .HasMaxLength(130)
                .IsUnicode(false)
                .HasColumnName("nombre_cuenta");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<GioTurno>(entity =>
        {
            entity.HasKey(e => e.IdTurno);

            entity.ToTable("gio_turno");

            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.HoraFin)
                .HasColumnType("datetime")
                .HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("hora_inicio");
            entity.Property(e => e.NombreTurno)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre_turno");
        });

        modelBuilder.Entity<Interfaz>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("interfaz");

            entity.Property(e => e.CantidadRegistros).HasColumnName("cantidad_registros");
            entity.Property(e => e.FechaEjecucion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ejecucion");
            entity.Property(e => e.FechaEjecucionFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ejecucion_fin");
            entity.Property(e => e.IdInterfaz).HasColumnName("id_interfaz");
            entity.Property(e => e.Tipo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<KushkiSmartLink>(entity =>
        {
            entity.ToTable("kushki_smart_link");

            entity.Property(e => e.KushkiSmartLinkId).HasColumnName("kushkiSmartLinkId");
            entity.Property(e => e.BrandLogo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("brandLogo");
            entity.Property(e => e.BuyButtonText)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("buyButtonText");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("createDate");
            entity.Property(e => e.Currency)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("currency");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ExecutionLimit).HasColumnName("executionLimit");
            entity.Property(e => e.IdLocalPaciente).HasColumnName("id_local_paciente");
            entity.Property(e => e.IdLocalPaquete).HasColumnName("id_local_paquete");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("iva");
            entity.Property(e => e.Label)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("label");
            entity.Property(e => e.Link)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("link");
            entity.Property(e => e.MerchantName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("merchantName");
            entity.Property(e => e.NumberOfFees).HasColumnName("numberOfFees");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("paymentMethod");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("paymentType");
            entity.Property(e => e.Periodicity)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("periodicity");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.PlanName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("planName");
            entity.Property(e => e.ProductImage)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("productImage");
            entity.Property(e => e.ProductName)
                .IsUnicode(false)
                .HasColumnName("productName");
            entity.Property(e => e.PromotionalText)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("promotionalText");
            entity.Property(e => e.PublicMerchantId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("publicMerchantId");
            entity.Property(e => e.SmartLinkId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("smartLinkId");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Structure)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("structure");
            entity.Property(e => e.SubscriptionDay).HasColumnName("subscriptionDay");
            entity.Property(e => e.SubtotalIva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotalIva");
            entity.Property(e => e.SubtotalIva0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotalIva0");
            entity.Property(e => e.TermAndConditions)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("termAndConditions");
            entity.Property(e => e.Terms)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("terms");
            entity.Property(e => e.Value)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("value");
        });

        modelBuilder.Entity<LogCancelacionMasiva>(entity =>
        {
            entity.HasKey(e => e.IdLog);

            entity.ToTable("log_cancelacion_masiva");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.DiaCancelacion)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("dia_cancelacion");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaVisualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_visualizacion");
            entity.Property(e => e.HoraFin)
                .HasColumnType("datetime")
                .HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("hora_inicio");
            entity.Property(e => e.IdCitas)
                .HasColumnType("text")
                .HasColumnName("id_citas");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
        });

        modelBuilder.Entity<LogEnvioReconocimientoFacial>(entity =>
        {
            entity.ToTable("log_envio_reconocimiento_facial");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdFeedBack)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("id_feed_back");
            entity.Property(e => e.IdGio).HasColumnName("id_gio");
            entity.Property(e => e.IdUsuarioRegistro).HasColumnName("id_usuario_registro");
            entity.Property(e => e.Tipo)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<LogErrores>(entity =>
        {
            entity.HasKey(e => e.IdLog);

            entity.ToTable("log_errores");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Mensaje)
                .IsUnicode(false)
                .HasColumnName("mensaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<LogInterfaz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__log_inte__3213E83F870CBA39");

            entity.ToTable("log_interfaz");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaServidor)
                .HasColumnType("datetime")
                .HasColumnName("fecha_servidor");
            entity.Property(e => e.Mensaje)
                .IsUnicode(false)
                .HasColumnName("mensaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<LogSucursalIntentosCheckin>(entity =>
        {
            entity.ToTable("log_sucursal_intentos_checkin");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CantidadPosponer).HasColumnName("cantidad_posponer");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.GioInicioTurno)
                .HasColumnType("datetime")
                .HasColumnName("gio_inicio_turno");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuarioRegistro).HasColumnName("id_usuario_registro");
            entity.Property(e => e.IdUsuarioRespondioNinguno).HasColumnName("id_usuario_respondio_ninguno");
            entity.Property(e => e.Tipo)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<MaquinasLaserDetalle>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_maquinas_laser_detalle_id");

            entity.ToTable("maquinas_laser_detalle");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdMaquina).HasColumnName("id_maquina");
            entity.Property(e => e.IdMaquinaDet).HasColumnName("id_maquina_det");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ip_address");
            entity.Property(e => e.MacAddress)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("mac_address");
            entity.Property(e => e.NoCabina).HasColumnName("no_cabina");
            entity.Property(e => e.NumSerieDispositivo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("num_serie_dispositivo");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serie");
        });

        modelBuilder.Entity<MaquinasLaserDetallePaso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_maquinas_laser_detalle_paso_id");

            entity.ToTable("maquinas_laser_detalle_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdMaquina).HasColumnName("id_maquina");
            entity.Property(e => e.IdMaquinaDet).HasColumnName("id_maquina_det");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ip_address");
            entity.Property(e => e.MacAddress)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("mac_address");
            entity.Property(e => e.NoCabina).HasColumnName("no_cabina");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.NumSerieDispositivo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("num_serie_dispositivo");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("serie");
        });

        modelBuilder.Entity<MobileAgendaBloques>(entity =>
        {
            entity.ToTable("mobile_agenda_bloques");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.Tipo)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<MobileHorariosCierreJuntas>(entity =>
        {
            entity.ToTable("mobile_horarios_cierre_juntas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
        });

        modelBuilder.Entity<MobileHorariosCierreJuntasPaso>(entity =>
        {
            entity.ToTable("mobile_horarios_cierre_juntas_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
        });

        modelBuilder.Entity<Municipios>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio);

            entity.ToTable("municipios");

            entity.Property(e => e.IdMunicipio)
                .ValueGeneratedNever()
                .HasColumnName("id_municipio");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Municipio)
                .HasMaxLength(164)
                .IsUnicode(false)
                .HasColumnName("municipio");
        });

        modelBuilder.Entity<NetpayUrl>(entity =>
        {
            entity.ToTable("netpay_url");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EsBorrada).HasColumnName("es_borrada");
            entity.Property(e => e.EsDefault).HasColumnName("es_default");
            entity.Property(e => e.Url)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("url");
            entity.Property(e => e.UsarTls12).HasColumnName("usar_tls12");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_paciente_1");

            entity.ToTable("paciente");

            entity.HasIndex(e => new { e.IdPaciente, e.IdSucursal2 }, "IDX_20170518_02");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.ApMaterno)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("ap_materno");
            entity.Property(e => e.ApPaterno)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("ap_paterno");
            entity.Property(e => e.CargosAutoUltimos6meses).HasColumnName("cargos_auto_ultimos_6meses");
            entity.Property(e => e.ClienteRobustoDs)
                .HasDefaultValue(0)
                .HasColumnName("cliente_robusto_ds");
            entity.Property(e => e.ClienteRobustoS)
                .HasDefaultValue(0)
                .HasColumnName("cliente_robusto_s");
            entity.Property(e => e.ClienteTiene1ercita).HasColumnName("cliente_tiene_1ercita");
            entity.Property(e => e.ClienteTieneCitaFuturo).HasColumnName("cliente_tiene_cita_futuro");
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo_postal");
            entity.Property(e => e.Colonia)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("colonia");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("domicilio");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Estatus)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaNacimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursal2).HasColumnName("id_sucursal_2");
            entity.Property(e => e.IdTipoIdentificacion).HasColumnName("id_tipo_identificacion");
            entity.Property(e => e.IdTipoIdentificacion2).HasColumnName("id_tipo_identificacion2");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.Identidad)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("identidad");
            entity.Property(e => e.Identidad2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("identidad2");
            entity.Property(e => e.Municipio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("municipio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Num)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("num");
            entity.Property(e => e.SaldoTotalPaciente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_total_paciente");
            entity.Property(e => e.SaldoVencidoPaciente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido_paciente");
            entity.Property(e => e.SaldoVencidoPaciente5Dias)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido_paciente_5_dias");
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sexo");
            entity.Property(e => e.Telefono1)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("telefono_1");
            entity.Property(e => e.Telefono2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("telefono_2");
            entity.Property(e => e.TipoIdentificacionCliente)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo_identificacion_cliente");
            entity.Property(e => e.TotalPagado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_pagado");
        });

        modelBuilder.Entity<PacientePaso>(entity =>
        {
            entity.ToTable("paciente_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApMaterno)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("ap_materno");
            entity.Property(e => e.ApPaterno)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("ap_paterno");
            entity.Property(e => e.CargosAutoUltimos6meses).HasColumnName("cargos_auto_ultimos_6meses");
            entity.Property(e => e.ClienteRobustoDs).HasColumnName("cliente_robusto_ds");
            entity.Property(e => e.ClienteRobustoS).HasColumnName("cliente_robusto_s");
            entity.Property(e => e.ClienteTiene1ercita).HasColumnName("cliente_tiene_1ercita");
            entity.Property(e => e.ClienteTieneCitaFuturo).HasColumnName("cliente_tiene_cita_futuro");
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo_postal");
            entity.Property(e => e.Colonia)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("colonia");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("domicilio");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Estatus)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaNacimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursal2).HasColumnName("id_sucursal_2");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdTipoIdentificacion).HasColumnName("id_tipo_identificacion");
            entity.Property(e => e.IdTipoIdentificacion2).HasColumnName("id_tipo_identificacion2");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.Identidad)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("identidad");
            entity.Property(e => e.Identidad2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("identidad2");
            entity.Property(e => e.Municipio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("municipio");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.Nombre)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Num)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("num");
            entity.Property(e => e.SaldoTotalPaciente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_total_paciente");
            entity.Property(e => e.SaldoVencidoPaciente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido_paciente");
            entity.Property(e => e.SaldoVencidoPaciente5Dias)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido_paciente_5_dias");
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sexo");
            entity.Property(e => e.Telefono1)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("telefono_1");
            entity.Property(e => e.Telefono2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("telefono_2");
            entity.Property(e => e.TipoIdentificacionCliente)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo_identificacion_cliente");
            entity.Property(e => e.TotalPagado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_pagado");
        });

        modelBuilder.Entity<PacientesEliminados>(entity =>
        {
            entity.ToTable("pacientes_eliminados");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
        });

        modelBuilder.Entity<PagoCaja>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_pago_caja_1");

            entity.ToTable("pago_caja");

            entity.HasIndex(e => new { e.FechaInterfaz, e.Fecha }, "IDX_20200320_01");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("domicilio");
            entity.Property(e => e.EsAnticipo).HasColumnName("es_anticipo");
            entity.Property(e => e.EsEuroskin).HasColumnName("es_euroskin");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FolioFacturacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("folio_facturacion");
            entity.Property(e => e.FolioRecibo).HasColumnName("FOLIO_RECIBO");
            entity.Property(e => e.IdBanco)
                .HasDefaultValue(0)
                .HasColumnName("id_banco");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("iva");
            entity.Property(e => e.Nombre)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Pago)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago");
            entity.Property(e => e.Rfc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("rfc");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.TipoRecibo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("tipo_recibo");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total");
        });

        modelBuilder.Entity<PagoCajaDetalle>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_PAGOS_CAJA");

            entity.ToTable("pago_caja_detalle");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdLocalPaciente).HasColumnName("id_local_paciente");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.IdPagoDetalle).HasColumnName("id_pago_detalle");
            entity.Property(e => e.IdPagoLocal).HasColumnName("id_pago_local");
            entity.Property(e => e.IdPaquete)
                .HasDefaultValue(0)
                .HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdRecuperador).HasColumnName("id_recuperador");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.PagoRecuperacion).HasColumnName("pago_recuperacion");

            entity.HasOne(d => d.IdPagoLocalNavigation).WithMany(p => p.PagoCajaDetalle)
                .HasForeignKey(d => d.IdPagoLocal)
                .HasConstraintName("FK_pago_caja_detalle_pago_caja");
        });

        modelBuilder.Entity<PagoCajaForma>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("pago_caja_forma");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.IdPagoLocal).HasColumnName("id_pago_local");
            entity.Property(e => e.Pago)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago");
            entity.Property(e => e.PagoCa)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_ca");
            entity.Property(e => e.PagoEfectivo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_efectivo");
            entity.Property(e => e.PagoSinCategoria)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_sin_categoria");
            entity.Property(e => e.PagoTc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_tc");
            entity.Property(e => e.PagoTd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_td");
            entity.Property(e => e.PagoTransferencia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_transferencia");

            entity.HasOne(d => d.IdPagoLocalNavigation).WithMany(p => p.PagoCajaForma)
                .HasForeignKey(d => d.IdPagoLocal)
                .HasConstraintName("FK_pago_caja_forma_pago_caja");
        });

        modelBuilder.Entity<Paquete>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_PAQUETE");

            entity.ToTable("paquete");

            entity.HasIndex(e => e.TipoCobranza, "IDX_20170523");

            entity.HasIndex(e => e.IdPaquete, "IDX_20190117_1");

            entity.HasIndex(e => new { e.EsReventa, e.IdLocal, e.FormaPago, e.FechaPago1, e.SaldoTotal }, "IDX_20200320_02");

            entity.HasIndex(e => new { e.IdPaquete, e.SaldoTotal }, "IDX_20200320_03");

            entity.HasIndex(e => e.FechaInterfaz, "IDX_20200602_01");

            entity.HasIndex(e => e.FechaInterfaz, "idx_20190107_01");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.Anticipo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo");
            entity.Property(e => e.BorradoEnMigracion).HasColumnName("borrado_en_migracion");
            entity.Property(e => e.CantPagosSaldoVencido)
                .HasDefaultValue(0)
                .HasColumnName("cant_pagos_saldo_vencido");
            entity.Property(e => e.ClienteReferido).HasColumnName("cliente_referido");
            entity.Property(e => e.CobrableConekta)
                .HasDefaultValue(false)
                .HasColumnName("cobrable_conekta");
            entity.Property(e => e.CompartidoEnfermera).HasColumnName("compartido_enfermera");
            entity.Property(e => e.ConektaPromocionMsi).HasColumnName("conekta_promocion_msi");
            entity.Property(e => e.Contrato)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("contrato");
            entity.Property(e => e.CostoTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_total");
            entity.Property(e => e.CostoTotalCalculado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_total_calculado");
            entity.Property(e => e.Dni)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("dni");
            entity.Property(e => e.EsEuroskin).HasColumnName("es_euroskin");
            entity.Property(e => e.EsNegrita)
                .HasDefaultValue(false)
                .HasColumnName("es_negrita");
            entity.Property(e => e.EsReventa).HasColumnName("es_reventa");
            entity.Property(e => e.FechaActualizacionSaldos)
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion_saldos");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaCapturaIban)
                .HasColumnType("datetime")
                .HasColumnName("fecha_captura_iban");
            entity.Property(e => e.FechaCitaUltimatum)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cita_ultimatum");
            entity.Property(e => e.FechaCobranzaAutomatica)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cobranza_automatica");
            entity.Property(e => e.FechaCompra)
                .HasColumnType("datetime")
                .HasColumnName("fecha_compra");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaNegrita)
                .HasColumnType("datetime")
                .HasColumnName("fecha_negrita");
            entity.Property(e => e.FechaPago1)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_1");
            entity.Property(e => e.FechaPago10)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_10");
            entity.Property(e => e.FechaPago2)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_2");
            entity.Property(e => e.FechaPago3)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_3");
            entity.Property(e => e.FechaPago4)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_4");
            entity.Property(e => e.FechaPago5)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_5");
            entity.Property(e => e.FechaPago6)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_6");
            entity.Property(e => e.FechaPago7)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_7");
            entity.Property(e => e.FechaPago8)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_8");
            entity.Property(e => e.FechaPago9)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_9");
            entity.Property(e => e.FechaRecuperacionCobranza)
                .HasColumnType("datetime")
                .HasColumnName("fecha_recuperacion_cobranza");
            entity.Property(e => e.FechaSigtePago)
                .HasColumnType("datetime")
                .HasColumnName("fecha_sigte_pago");
            entity.Property(e => e.FechaVtaComEnf)
                .HasColumnType("datetime")
                .HasColumnName("fecha_vta_com_enf");
            entity.Property(e => e.FirmaContrato)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("firma_contrato");
            entity.Property(e => e.FormaPago)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("forma_pago");
            entity.Property(e => e.Iban)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("iban");
            entity.Property(e => e.IbanErroneo).HasColumnName("iban_erroneo");
            entity.Property(e => e.IdBanco).HasColumnName("id_banco");
            entity.Property(e => e.IdBanco2).HasColumnName("id_banco2");
            entity.Property(e => e.IdClienteReferido).HasColumnName("id_cliente_referido");
            entity.Property(e => e.IdClienteRefirio).HasColumnName("id_cliente_refirio");
            entity.Property(e => e.IdEnfermera).HasColumnName("id_enfermera");
            entity.Property(e => e.IdMedio).HasColumnName("id_medio");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPaciente2).HasColumnName("id_paciente_2");
            entity.Property(e => e.IdPaciente3).HasColumnName("id_paciente_3");
            entity.Property(e => e.IdPaciente4).HasColumnName("id_paciente_4");
            entity.Property(e => e.IdPaciente5).HasColumnName("id_paciente_5");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPacienteLocal2).HasColumnName("id_paciente_local_2");
            entity.Property(e => e.IdPacienteLocal3).HasColumnName("id_paciente_local_3");
            entity.Property(e => e.IdPacienteLocal4).HasColumnName("id_paciente_local_4");
            entity.Property(e => e.IdPacienteLocal5).HasColumnName("id_paciente_local_5");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalOrigen).HasColumnName("id_sucursal_origen");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdUsuarioModifica).HasColumnName("id_usuario_modifica");
            entity.Property(e => e.IdUsuarioVtaComEnf).HasColumnName("id_usuario_vta_com_enf");
            entity.Property(e => e.MontoPago1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_1");
            entity.Property(e => e.MontoPago10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_10");
            entity.Property(e => e.MontoPago2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_2");
            entity.Property(e => e.MontoPago3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_3");
            entity.Property(e => e.MontoPago4)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_4");
            entity.Property(e => e.MontoPago5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_5");
            entity.Property(e => e.MontoPago6)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_6");
            entity.Property(e => e.MontoPago7)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_7");
            entity.Property(e => e.MontoPago8)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_8");
            entity.Property(e => e.MontoPago9)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_9");
            entity.Property(e => e.MostrarEnSucursales).HasColumnName("mostrar_en_sucursales");
            entity.Property(e => e.NoCuenta)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("no_cuenta");
            entity.Property(e => e.NoDisponiblePorMigracion).HasColumnName("no_disponible_por_migracion");
            entity.Property(e => e.NombrePaciente1)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_1");
            entity.Property(e => e.NombrePaciente2)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_2");
            entity.Property(e => e.NombrePaciente3)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_3");
            entity.Property(e => e.NombrePaciente4)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_4");
            entity.Property(e => e.NombrePaciente5)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_5");
            entity.Property(e => e.NumPagoSaldoVencido).HasColumnName("num_pago_saldo_vencido");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.PagadoMitad)
                .HasDefaultValue(false)
                .HasColumnName("pagado_mitad");
            entity.Property(e => e.PagadoMitadFecha)
                .HasColumnType("datetime")
                .HasColumnName("pagado_mitad_fecha");
            entity.Property(e => e.PagoUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_unitario");
            entity.Property(e => e.PagosXCubrir).HasColumnName("pagos_x_cubrir");
            entity.Property(e => e.PaqueteCompleto).HasColumnName("paquete_completo");
            entity.Property(e => e.PerdidaGarantia).HasColumnName("perdida_garantia");
            entity.Property(e => e.ProvieneDeMigracion).HasColumnName("proviene_de_migracion");
            entity.Property(e => e.SaldoTotal)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_total");
            entity.Property(e => e.SaldoVencido)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido");
            entity.Property(e => e.SaldoVencido5Dias)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido_5_dias");
            entity.Property(e => e.SiEnviarPaqueteServicio).HasColumnName("si_enviar_paquete_servicio");
            entity.Property(e => e.SugerirTc1)
                .HasDefaultValue(true)
                .HasColumnName("sugerir_tc_1");
            entity.Property(e => e.TarjetaCs)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cs");
            entity.Property(e => e.TarjetaCsT2)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cs_t2");
            entity.Property(e => e.TarjetaCvv)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cvv");
            entity.Property(e => e.TarjetaCvvT2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cvv_t2");
            entity.Property(e => e.TarjetaFechaVenc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_fecha_venc");
            entity.Property(e => e.TarjetaFechaVencT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_fecha_venc_t2");
            entity.Property(e => e.TarjetaNoPudoValidarCauto)
                .HasDefaultValue(false)
                .HasColumnName("tarjeta_no_pudo_validar_cauto");
            entity.Property(e => e.TarjetaNumero)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_numero");
            entity.Property(e => e.TarjetaNumeroT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_numero_t2");
            entity.Property(e => e.TarjetaPrimaria).HasColumnName("tarjeta_primaria");
            entity.Property(e => e.TarjetaTipo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_tipo");
            entity.Property(e => e.TarjetaTipoT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_tipo_t2");
            entity.Property(e => e.TipoCobranza)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza");
            entity.Property(e => e.TipoCobranza1)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_1");
            entity.Property(e => e.TipoCobranza10)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_10");
            entity.Property(e => e.TipoCobranza2)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_2");
            entity.Property(e => e.TipoCobranza3)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_3");
            entity.Property(e => e.TipoCobranza4)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_4");
            entity.Property(e => e.TipoCobranza5)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_5");
            entity.Property(e => e.TipoCobranza6)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_6");
            entity.Property(e => e.TipoCobranza7)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_7");
            entity.Property(e => e.TipoCobranza8)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_8");
            entity.Property(e => e.TipoCobranza9)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_9");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo_cuenta");
            entity.Property(e => e.TokenT1)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("token_t1");
            entity.Property(e => e.TokenT2)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("token_t2");
            entity.Property(e => e.TratCorporal).HasColumnName("trat_corporal");
            entity.Property(e => e.TratCorporal2).HasColumnName("trat_corporal_2");
            entity.Property(e => e.TratCorporal3).HasColumnName("trat_corporal_3");
            entity.Property(e => e.TratCorporal4).HasColumnName("trat_corporal_4");
            entity.Property(e => e.TratCorporal5).HasColumnName("trat_corporal_5");
            entity.Property(e => e.TratFacial).HasColumnName("trat_facial");
            entity.Property(e => e.TratFacial2).HasColumnName("trat_facial_2");
            entity.Property(e => e.TratFacial3).HasColumnName("trat_facial_3");
            entity.Property(e => e.TratFacial4).HasColumnName("trat_facial_4");
            entity.Property(e => e.TratFacial5).HasColumnName("trat_facial_5");
            entity.Property(e => e.TratLaser).HasColumnName("trat_laser");
            entity.Property(e => e.TratLaser2).HasColumnName("trat_laser_2");
            entity.Property(e => e.TratLaser3).HasColumnName("trat_laser_3");
            entity.Property(e => e.TratLaser4).HasColumnName("trat_laser_4");
            entity.Property(e => e.TratLaser5).HasColumnName("trat_laser_5");
            entity.Property(e => e.TratamientoGratisReferido).HasColumnName("tratamiento_gratis_referido");
            entity.Property(e => e.VentaCompletaEnfermera).HasColumnName("venta_completa_enfermera");
        });

        modelBuilder.Entity<PaqueteFinanciamiento>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("paquete_financiamiento");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.BanderaManual).HasColumnName("bandera_manual");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdFinanciemiento).HasColumnName("id_financiemiento");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.NumPago).HasColumnName("num_pago");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<PaqueteLog>(entity =>
        {
            entity.HasKey(e => e.IdLog);

            entity.ToTable("paquete_log");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.Anticipo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo");
            entity.Property(e => e.BorradoEnMigracion).HasColumnName("borrado_en_migracion");
            entity.Property(e => e.CompartidoEnfermera).HasColumnName("compartido_enfermera");
            entity.Property(e => e.CostoTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_total");
            entity.Property(e => e.CostoTotalCalculado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_total_calculado");
            entity.Property(e => e.EsReventa).HasColumnName("es_reventa");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaCitaUltimatum)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cita_ultimatum");
            entity.Property(e => e.FechaCobranzaAutomatica)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cobranza_automatica");
            entity.Property(e => e.FechaCompra)
                .HasColumnType("datetime")
                .HasColumnName("fecha_compra");
            entity.Property(e => e.FechaMod)
                .HasColumnType("datetime")
                .HasColumnName("fecha_mod");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaPago1)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_1");
            entity.Property(e => e.FechaPago10)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_10");
            entity.Property(e => e.FechaPago2)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_2");
            entity.Property(e => e.FechaPago3)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_3");
            entity.Property(e => e.FechaPago4)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_4");
            entity.Property(e => e.FechaPago5)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_5");
            entity.Property(e => e.FechaPago6)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_6");
            entity.Property(e => e.FechaPago7)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_7");
            entity.Property(e => e.FechaPago8)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_8");
            entity.Property(e => e.FechaPago9)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_9");
            entity.Property(e => e.FechaRecuperacionCobranza)
                .HasColumnType("datetime")
                .HasColumnName("fecha_recuperacion_cobranza");
            entity.Property(e => e.FormaPago)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("forma_pago");
            entity.Property(e => e.IdEnfermera).HasColumnName("id_enfermera");
            entity.Property(e => e.IdMedio).HasColumnName("id_medio");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPaciente2).HasColumnName("id_paciente_2");
            entity.Property(e => e.IdPaciente3).HasColumnName("id_paciente_3");
            entity.Property(e => e.IdPaciente4).HasColumnName("id_paciente_4");
            entity.Property(e => e.IdPaciente5).HasColumnName("id_paciente_5");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPacienteLocal2).HasColumnName("id_paciente_local_2");
            entity.Property(e => e.IdPacienteLocal3).HasColumnName("id_paciente_local_3");
            entity.Property(e => e.IdPacienteLocal4).HasColumnName("id_paciente_local_4");
            entity.Property(e => e.IdPacienteLocal5).HasColumnName("id_paciente_local_5");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalOrigen).HasColumnName("id_sucursal_origen");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdUsuarioMod).HasColumnName("id_usuario_mod");
            entity.Property(e => e.IdUsuarioModifica).HasColumnName("id_usuario_modifica");
            entity.Property(e => e.MontoPago1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_1");
            entity.Property(e => e.MontoPago10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_10");
            entity.Property(e => e.MontoPago2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_2");
            entity.Property(e => e.MontoPago3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_3");
            entity.Property(e => e.MontoPago4)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_4");
            entity.Property(e => e.MontoPago5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_5");
            entity.Property(e => e.MontoPago6)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_6");
            entity.Property(e => e.MontoPago7)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_7");
            entity.Property(e => e.MontoPago8)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_8");
            entity.Property(e => e.MontoPago9)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_9");
            entity.Property(e => e.MostrarEnSucursales).HasColumnName("mostrar_en_sucursales");
            entity.Property(e => e.NoDisponiblePorMigracion).HasColumnName("no_disponible_por_migracion");
            entity.Property(e => e.NombrePaciente1)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_1");
            entity.Property(e => e.NombrePaciente2)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_2");
            entity.Property(e => e.NombrePaciente3)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_3");
            entity.Property(e => e.NombrePaciente4)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_4");
            entity.Property(e => e.NombrePaciente5)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_5");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.PagadoMitad).HasColumnName("pagado_mitad");
            entity.Property(e => e.PagadoMitadFecha)
                .HasColumnType("datetime")
                .HasColumnName("pagado_mitad_fecha");
            entity.Property(e => e.PagoUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_unitario");
            entity.Property(e => e.PagosXCubrir).HasColumnName("pagos_x_cubrir");
            entity.Property(e => e.PaqueteCompleto).HasColumnName("paquete_completo");
            entity.Property(e => e.ProvieneDeMigracion).HasColumnName("proviene_de_migracion");
            entity.Property(e => e.TarjetaCs)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cs");
            entity.Property(e => e.TarjetaCsT2)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cs_t2");
            entity.Property(e => e.TarjetaCvv)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cvv");
            entity.Property(e => e.TarjetaCvvT2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cvv_t2");
            entity.Property(e => e.TarjetaFechaVenc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_fecha_venc");
            entity.Property(e => e.TarjetaFechaVencT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_fecha_venc_t2");
            entity.Property(e => e.TarjetaNoPudoValidarCauto)
                .HasDefaultValue(false)
                .HasColumnName("tarjeta_no_pudo_validar_cauto");
            entity.Property(e => e.TarjetaNumero)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_numero");
            entity.Property(e => e.TarjetaNumeroT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_numero_t2");
            entity.Property(e => e.TarjetaPrimaria).HasColumnName("tarjeta_primaria");
            entity.Property(e => e.TarjetaTipo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_tipo");
            entity.Property(e => e.TarjetaTipoT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_tipo_t2");
            entity.Property(e => e.TipoCobranza)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza");
            entity.Property(e => e.TipoCobranza1)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_1");
            entity.Property(e => e.TipoCobranza10)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_10");
            entity.Property(e => e.TipoCobranza2)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_2");
            entity.Property(e => e.TipoCobranza3)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_3");
            entity.Property(e => e.TipoCobranza4)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_4");
            entity.Property(e => e.TipoCobranza5)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_5");
            entity.Property(e => e.TipoCobranza6)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_6");
            entity.Property(e => e.TipoCobranza7)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_7");
            entity.Property(e => e.TipoCobranza8)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_8");
            entity.Property(e => e.TipoCobranza9)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_9");
            entity.Property(e => e.TokenT1)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("token_t1");
            entity.Property(e => e.TokenT2)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("token_t2");
            entity.Property(e => e.TratCorporal).HasColumnName("trat_corporal");
            entity.Property(e => e.TratCorporal2).HasColumnName("trat_corporal_2");
            entity.Property(e => e.TratCorporal3).HasColumnName("trat_corporal_3");
            entity.Property(e => e.TratCorporal4).HasColumnName("trat_corporal_4");
            entity.Property(e => e.TratCorporal5).HasColumnName("trat_corporal_5");
            entity.Property(e => e.TratFacial).HasColumnName("trat_facial");
            entity.Property(e => e.TratFacial2).HasColumnName("trat_facial_2");
            entity.Property(e => e.TratFacial3).HasColumnName("trat_facial_3");
            entity.Property(e => e.TratFacial4).HasColumnName("trat_facial_4");
            entity.Property(e => e.TratFacial5).HasColumnName("trat_facial_5");
            entity.Property(e => e.TratLaser).HasColumnName("trat_laser");
            entity.Property(e => e.TratLaser2).HasColumnName("trat_laser_2");
            entity.Property(e => e.TratLaser3).HasColumnName("trat_laser_3");
            entity.Property(e => e.TratLaser4).HasColumnName("trat_laser_4");
            entity.Property(e => e.TratLaser5).HasColumnName("trat_laser_5");
        });

        modelBuilder.Entity<PaquetePaso>(entity =>
        {
            entity.ToTable("paquete_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anticipo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo");
            entity.Property(e => e.BorradoEnMigracion).HasColumnName("borrado_en_migracion");
            entity.Property(e => e.CantPagosSaldoVencido)
                .HasDefaultValue(0)
                .HasColumnName("cant_pagos_saldo_vencido");
            entity.Property(e => e.ClienteReferido).HasColumnName("cliente_referido");
            entity.Property(e => e.CobrableConekta)
                .HasDefaultValue(false)
                .HasColumnName("cobrable_conekta");
            entity.Property(e => e.CompartidoEnfermera).HasColumnName("compartido_enfermera");
            entity.Property(e => e.ConektaPromocionMsi).HasColumnName("conekta_promocion_msi");
            entity.Property(e => e.Contrato)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("contrato");
            entity.Property(e => e.CostoTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_total");
            entity.Property(e => e.CostoTotalCalculado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_total_calculado");
            entity.Property(e => e.Dni)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("dni");
            entity.Property(e => e.EsEuroskin).HasColumnName("es_euroskin");
            entity.Property(e => e.EsNegrita)
                .HasDefaultValue(false)
                .HasColumnName("es_negrita");
            entity.Property(e => e.EsReventa).HasColumnName("es_reventa");
            entity.Property(e => e.FechaActualizacionSaldos)
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion_saldos");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaCapturaIban)
                .HasColumnType("datetime")
                .HasColumnName("fecha_captura_iban");
            entity.Property(e => e.FechaCitaUltimatum)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cita_ultimatum");
            entity.Property(e => e.FechaCobranzaAutomatica)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cobranza_automatica");
            entity.Property(e => e.FechaCompra)
                .HasColumnType("datetime")
                .HasColumnName("fecha_compra");
            entity.Property(e => e.FechaConsultaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_consulta_interfaz");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaNegrita)
                .HasColumnType("datetime")
                .HasColumnName("fecha_negrita");
            entity.Property(e => e.FechaPago1)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_1");
            entity.Property(e => e.FechaPago10)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_10");
            entity.Property(e => e.FechaPago2)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_2");
            entity.Property(e => e.FechaPago3)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_3");
            entity.Property(e => e.FechaPago4)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_4");
            entity.Property(e => e.FechaPago5)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_5");
            entity.Property(e => e.FechaPago6)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_6");
            entity.Property(e => e.FechaPago7)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_7");
            entity.Property(e => e.FechaPago8)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_8");
            entity.Property(e => e.FechaPago9)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago_9");
            entity.Property(e => e.FechaRecuperacionCobranza)
                .HasColumnType("datetime")
                .HasColumnName("fecha_recuperacion_cobranza");
            entity.Property(e => e.FechaSigtePago)
                .HasColumnType("datetime")
                .HasColumnName("fecha_sigte_pago");
            entity.Property(e => e.FechaVtaComEnf)
                .HasColumnType("datetime")
                .HasColumnName("fecha_vta_com_enf");
            entity.Property(e => e.FirmaContrato)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("firma_contrato");
            entity.Property(e => e.FormaPago)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("forma_pago");
            entity.Property(e => e.Iban)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("iban");
            entity.Property(e => e.IbanErroneo).HasColumnName("iban_erroneo");
            entity.Property(e => e.IdClienteReferido).HasColumnName("id_cliente_referido");
            entity.Property(e => e.IdClienteRefirio).HasColumnName("id_cliente_refirio");
            entity.Property(e => e.IdEnfermera).HasColumnName("id_enfermera");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdMedio).HasColumnName("id_medio");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPaciente2).HasColumnName("id_paciente_2");
            entity.Property(e => e.IdPaciente3).HasColumnName("id_paciente_3");
            entity.Property(e => e.IdPaciente4).HasColumnName("id_paciente_4");
            entity.Property(e => e.IdPaciente5).HasColumnName("id_paciente_5");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPacienteLocal2).HasColumnName("id_paciente_local_2");
            entity.Property(e => e.IdPacienteLocal3).HasColumnName("id_paciente_local_3");
            entity.Property(e => e.IdPacienteLocal4).HasColumnName("id_paciente_local_4");
            entity.Property(e => e.IdPacienteLocal5).HasColumnName("id_paciente_local_5");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdSucursalOrigen).HasColumnName("id_sucursal_origen");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdUsuarioModifica).HasColumnName("id_usuario_modifica");
            entity.Property(e => e.IdUsuarioVtaComEnf).HasColumnName("id_usuario_vta_com_enf");
            entity.Property(e => e.MontoPago1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_1");
            entity.Property(e => e.MontoPago10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_10");
            entity.Property(e => e.MontoPago2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_2");
            entity.Property(e => e.MontoPago3)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_3");
            entity.Property(e => e.MontoPago4)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_4");
            entity.Property(e => e.MontoPago5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_5");
            entity.Property(e => e.MontoPago6)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_6");
            entity.Property(e => e.MontoPago7)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_7");
            entity.Property(e => e.MontoPago8)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_8");
            entity.Property(e => e.MontoPago9)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_pago_9");
            entity.Property(e => e.MostrarEnSucursales).HasColumnName("mostrar_en_sucursales");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.NoCuenta)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("no_cuenta");
            entity.Property(e => e.NoDisponiblePorMigracion).HasColumnName("no_disponible_por_migracion");
            entity.Property(e => e.NombrePaciente1)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_1");
            entity.Property(e => e.NombrePaciente2)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_2");
            entity.Property(e => e.NombrePaciente3)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_3");
            entity.Property(e => e.NombrePaciente4)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_4");
            entity.Property(e => e.NombrePaciente5)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_paciente_5");
            entity.Property(e => e.NumPagoSaldoVencido).HasColumnName("num_pago_saldo_vencido");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.PagadoMitad)
                .HasDefaultValue(false)
                .HasColumnName("pagado_mitad");
            entity.Property(e => e.PagadoMitadFecha)
                .HasColumnType("datetime")
                .HasColumnName("pagado_mitad_fecha");
            entity.Property(e => e.PagoUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pago_unitario");
            entity.Property(e => e.PagosXCubrir).HasColumnName("pagos_x_cubrir");
            entity.Property(e => e.PaqueteCompleto).HasColumnName("paquete_completo");
            entity.Property(e => e.PerdidaGarantia).HasColumnName("perdida_garantia");
            entity.Property(e => e.ProvieneDeMigracion).HasColumnName("proviene_de_migracion");
            entity.Property(e => e.SaldoTotal)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_total");
            entity.Property(e => e.SaldoVencido)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido");
            entity.Property(e => e.SaldoVencido5Dias)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo_vencido_5_dias");
            entity.Property(e => e.SugerirTc1).HasColumnName("sugerir_tc_1");
            entity.Property(e => e.TarjetaCs)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cs");
            entity.Property(e => e.TarjetaCsT2)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cs_t2");
            entity.Property(e => e.TarjetaCvv)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cvv");
            entity.Property(e => e.TarjetaCvvT2)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tarjeta_cvv_t2");
            entity.Property(e => e.TarjetaFechaVenc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_fecha_venc");
            entity.Property(e => e.TarjetaFechaVencT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_fecha_venc_t2");
            entity.Property(e => e.TarjetaNoPudoValidarCauto)
                .HasDefaultValue(false)
                .HasColumnName("tarjeta_no_pudo_validar_cauto");
            entity.Property(e => e.TarjetaNumero)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_numero");
            entity.Property(e => e.TarjetaNumeroT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_numero_t2");
            entity.Property(e => e.TarjetaPrimaria).HasColumnName("tarjeta_primaria");
            entity.Property(e => e.TarjetaTipo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_tipo");
            entity.Property(e => e.TarjetaTipoT2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tarjeta_tipo_t2");
            entity.Property(e => e.TipoCobranza)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza");
            entity.Property(e => e.TipoCobranza1)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_1");
            entity.Property(e => e.TipoCobranza10)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_10");
            entity.Property(e => e.TipoCobranza2)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_2");
            entity.Property(e => e.TipoCobranza3)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_3");
            entity.Property(e => e.TipoCobranza4)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_4");
            entity.Property(e => e.TipoCobranza5)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_5");
            entity.Property(e => e.TipoCobranza6)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_6");
            entity.Property(e => e.TipoCobranza7)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_7");
            entity.Property(e => e.TipoCobranza8)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_8");
            entity.Property(e => e.TipoCobranza9)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("tipo_cobranza_9");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo_cuenta");
            entity.Property(e => e.TokenT1)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("token_t1");
            entity.Property(e => e.TokenT2)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("token_t2");
            entity.Property(e => e.TratCorporal).HasColumnName("trat_corporal");
            entity.Property(e => e.TratCorporal2).HasColumnName("trat_corporal_2");
            entity.Property(e => e.TratCorporal3).HasColumnName("trat_corporal_3");
            entity.Property(e => e.TratCorporal4).HasColumnName("trat_corporal_4");
            entity.Property(e => e.TratCorporal5).HasColumnName("trat_corporal_5");
            entity.Property(e => e.TratFacial).HasColumnName("trat_facial");
            entity.Property(e => e.TratFacial2).HasColumnName("trat_facial_2");
            entity.Property(e => e.TratFacial3).HasColumnName("trat_facial_3");
            entity.Property(e => e.TratFacial4).HasColumnName("trat_facial_4");
            entity.Property(e => e.TratFacial5).HasColumnName("trat_facial_5");
            entity.Property(e => e.TratLaser).HasColumnName("trat_laser");
            entity.Property(e => e.TratLaser2).HasColumnName("trat_laser_2");
            entity.Property(e => e.TratLaser3).HasColumnName("trat_laser_3");
            entity.Property(e => e.TratLaser4).HasColumnName("trat_laser_4");
            entity.Property(e => e.TratLaser5).HasColumnName("trat_laser_5");
            entity.Property(e => e.TratamientoGratisReferido).HasColumnName("tratamiento_gratis_referido");
            entity.Property(e => e.VentaCompletaEnfermera).HasColumnName("venta_completa_enfermera");
        });

        modelBuilder.Entity<PaqueteServicio>(entity =>
        {
            entity.HasKey(e => e.IdLocal).HasName("PK_PAQUETE_SERVICIO");

            entity.ToTable("paquete_servicio");

            entity.HasIndex(e => e.IdLocalPaquete, "IDX_20170531_03");

            entity.HasIndex(e => e.IdPaquete, "IDX_20170531_04");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.AreaEnviada)
                .HasDefaultValue(1)
                .HasColumnName("area_enviada");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.EsGratis).HasColumnName("es_gratis");
            entity.Property(e => e.Estatus)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdLocalPaquete).HasColumnName("id_local_paquete");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteServicio).HasColumnName("id_paquete_servicio");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
        });

        modelBuilder.Entity<PaqueteServicioPaso>(entity =>
        {
            entity.ToTable("paquete_servicio_paso");

            entity.HasIndex(e => new { e.IdRemotoInterfazPaquete, e.IdSucursalInterfazPaquete, e.NoCajaInterfazPaquete }, "IDX_20170531_01");

            entity.HasIndex(e => e.IdPaquete, "IDX_20170531_02");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.EsGratis).HasColumnName("es_gratis");
            entity.Property(e => e.Estatus)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdLocalPaquete).HasColumnName("id_local_paquete");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdPacienteLocal).HasColumnName("id_paciente_local");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteServicio).HasColumnName("id_paquete_servicio");
            entity.Property(e => e.IdRemotoInterfazPaquete).HasColumnName("id_remoto_interfaz_paquete");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.IdSucursalInterfazPaquete).HasColumnName("id_sucursal_interfaz_paquete");
            entity.Property(e => e.NoCajaInterfazPaquete).HasColumnName("no_caja_interfaz_paquete");
        });

        modelBuilder.Entity<PaquetesEliminados>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("paquetes_eliminados");

            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.ToTable("parametro");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Abreviatura)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("abreviatura");
            entity.Property(e => e.AnticipoMinimo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo_minimo");
            entity.Property(e => e.AnticipoMinimoCauto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo_minimo_cauto");
            entity.Property(e => e.AnticipoMinimoReventa)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo_minimo_reventa");
            entity.Property(e => e.AnticipoMinimoSinIban)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("anticipo_minimo_sin_iban");
            entity.Property(e => e.Bloque)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.BloqueoPos).HasColumnName("bloqueo_pos");
            entity.Property(e => e.BloqueoPosAgenda).HasColumnName("bloqueo_pos_agenda");
            entity.Property(e => e.CapturaDniActivo).HasColumnName("captura_dni_activo");
            entity.Property(e => e.CapturaIbanActivo).HasColumnName("captura_iban_activo");
            entity.Property(e => e.CapturaIbanErroneo).HasColumnName("captura_iban_erroneo");
            entity.Property(e => e.CapturaNoCuentaActivo).HasColumnName("captura_no_cuenta_activo");
            entity.Property(e => e.CcRepresentanteLegal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cc_representante_legal");
            entity.Property(e => e.ClaveBloque).HasColumnName("clave_bloque");
            entity.Property(e => e.DescMax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("desc_max");
            entity.Property(e => e.DescMaxMonto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("desc_max_monto");
            entity.Property(e => e.DescMaxSueltas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("desc_max_sueltas");
            entity.Property(e => e.DescTipoMonto).HasColumnName("desc_tipo_monto");
            entity.Property(e => e.DiasMaximo1erPago).HasColumnName("dias_maximo_1er_pago");
            entity.Property(e => e.DiasMaximoDifEntrePagos).HasColumnName("dias_maximo_dif_entre_pagos");
            entity.Property(e => e.DiasMaximoPagos).HasColumnName("dias_maximo_pagos");
            entity.Property(e => e.DiasMaximoPagosContraloria).HasColumnName("dias_maximo_pagos_contraloria");
            entity.Property(e => e.DiasMaximoPagosDir).HasColumnName("dias_maximo_pagos_dir");
            entity.Property(e => e.DiasMaximoPagosV1).HasColumnName("dias_maximo_pagos_v1");
            entity.Property(e => e.DiasMaximoPagosV2).HasColumnName("dias_maximo_pagos_v2");
            entity.Property(e => e.DifVentasSemanaPasada)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("dif_ventas_semana_pasada");
            entity.Property(e => e.EfevooDeviceId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("efevoo_device_id");
            entity.Property(e => e.EfevooSecret)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("efevoo_secret");
            entity.Property(e => e.EmailsBloqueos)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("emails_bloqueos");
            entity.Property(e => e.EmailsNominaEnfermera)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("emails_nomina_enfermera");
            entity.Property(e => e.EmailsNotificacionPaqCostoInferior)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("emails_notificacion_paq_costo_inferior");
            entity.Property(e => e.EsCredibanco).HasColumnName("es_credibanco");
            entity.Property(e => e.EsEfevoo).HasColumnName("es_efevoo");
            entity.Property(e => e.EsEuroskin).HasColumnName("es_euroskin");
            entity.Property(e => e.FactorDescuentoPaquetes)
                .HasColumnType("decimal(18, 13)")
                .HasColumnName("factor_descuento_paquetes");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.HorasParaReventa).HasColumnName("horas_para_reventa");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Idioma)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("idioma");
            entity.Property(e => e.ImpresoraVoucher)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("impresora_voucher");
            entity.Property(e => e.MaxPagosV1).HasColumnName("max_pagos_v1");
            entity.Property(e => e.MaxPagosV2).HasColumnName("max_pagos_v2");
            entity.Property(e => e.MaxPagosVendedor).HasColumnName("max_pagos_vendedor");
            entity.Property(e => e.MaxPagosVerificador).HasColumnName("max_pagos_verificador");
            entity.Property(e => e.Moneda)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("moneda");
            entity.Property(e => e.MontoMaxPorcProtegido)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_max_porc_protegido");
            entity.Property(e => e.MontoMaximoCobro)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_maximo_cobro");
            entity.Property(e => e.MontoProtegido)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_protegido");
            entity.Property(e => e.Nit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nit");
            entity.Property(e => e.NoCaja).HasColumnName("no_caja");
            entity.Property(e => e.NoSesionesDefault).HasColumnName("no_sesiones_default");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("nombre_empresa");
            entity.Property(e => e.PagosDefault).HasColumnName("pagos_default");
            entity.Property(e => e.PermitirCitaPaqueteConNegrita).HasColumnName("permitir_cita_paquete_con_negrita");
            entity.Property(e => e.PermitirEnvioSmsAppWeb).HasColumnName("permitir_envio_sms_app_web");
            entity.Property(e => e.PorcParaProtegido)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("porc_para_protegido");
            entity.Property(e => e.PosProcesadorCobrosDefault)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("pos_procesador_cobros_default");
            entity.Property(e => e.PuertoTerminalNetpay)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("puerto_terminal_netpay");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("razon_social");
            entity.Property(e => e.ReciboLeyenda1)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("recibo_leyenda_1");
            entity.Property(e => e.ReciboLeyenda2)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("recibo_leyenda_2");
            entity.Property(e => e.RepresentanteLegal)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("representante_legal");
            entity.Property(e => e.ReventaMinimaPaquete)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("reventa_minima_paquete");
            entity.Property(e => e.Rfc)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("rfc");
            entity.Property(e => e.TerminalBsdIp)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("terminal_bsd_ip");
            entity.Property(e => e.TerminalIdMercadoPago)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("terminal_id_mercado_pago");
            entity.Property(e => e.TerminalIdNetpay)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("terminal_id_netpay");
            entity.Property(e => e.TipoDifEntrePagos)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("tipo_dif_entre_pagos");
            entity.Property(e => e.UsarConfiguracionDePagosBancomer).HasColumnName("usar_configuracion_de_pagos_bancomer");
            entity.Property(e => e.ValidaToku)
                .HasDefaultValue(0)
                .HasColumnName("valida_toku");
            entity.Property(e => e.ValidarTarjetaBinNoPermitidoPos).HasColumnName("validar_tarjeta_bin_no_permitido_pos");
            entity.Property(e => e.VentaMinimaPaquete)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("venta_minima_paquete");
            entity.Property(e => e.VersionSistema)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("version_sistema");
        });

        modelBuilder.Entity<ParametrosAnticipoMinimo>(entity =>
        {
            entity.HasKey(e => e.IdParametro).HasName("PK_parametros_anticipo_minimo_id");

            entity.ToTable("parametros_anticipo_minimo");

            entity.Property(e => e.IdParametro)
                .ValueGeneratedNever()
                .HasColumnName("id_parametro");
            entity.Property(e => e.Bloque)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
        });

        modelBuilder.Entity<ParametrosAnticipoMinimoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdParametroDetalle).HasName("PK_parametros_anticipo_minimo_detalle_id");

            entity.ToTable("parametros_anticipo_minimo_detalle");

            entity.Property(e => e.IdParametroDetalle)
                .ValueGeneratedNever()
                .HasColumnName("id_parametro_detalle");
            entity.Property(e => e.DiaFin).HasColumnName("dia_fin");
            entity.Property(e => e.DiaInicio).HasColumnName("dia_inicio");
            entity.Property(e => e.EsProtegido).HasColumnName("es_protegido");
            entity.Property(e => e.EsReventa).HasColumnName("es_reventa");
            entity.Property(e => e.IdBanco).HasColumnName("id_banco");
            entity.Property(e => e.IdParametro).HasColumnName("id_parametro");
            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.TipoCalculo).HasColumnName("tipo_calculo");
            entity.Property(e => e.Valor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("valor");
            entity.Property(e => e.ValorMax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("valor_max");
            entity.Property(e => e.ValorMin)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("valor_min");

            entity.HasOne(d => d.IdParametroNavigation).WithMany(p => p.ParametrosAnticipoMinimoDetalle)
                .HasForeignKey(d => d.IdParametro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_parametros_anticipo_minimo_detalle_id_parametro");

            entity.HasOne(d => d.IdTipoNavigation).WithMany(p => p.ParametrosAnticipoMinimoDetalle)
                .HasForeignKey(d => d.IdTipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_parametros_anticipo_minimo_detalle_id_tipo");
        });

        modelBuilder.Entity<ParametrosAnticipoPaso>(entity =>
        {
            entity.HasKey(e => e.IdParametroDetalle);

            entity.ToTable("parametros_anticipo_paso");

            entity.Property(e => e.IdParametroDetalle)
                .ValueGeneratedNever()
                .HasColumnName("id_parametro_detalle");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Bloque)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bloque");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.DiaFin).HasColumnName("dia_fin");
            entity.Property(e => e.DiaInicio).HasColumnName("dia_inicio");
            entity.Property(e => e.EsProtegido).HasColumnName("es_protegido");
            entity.Property(e => e.EsReventa).HasColumnName("es_reventa");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdBanco).HasColumnName("id_banco");
            entity.Property(e => e.IdParametro).HasColumnName("id_parametro");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.TipoCalculo).HasColumnName("tipo_calculo");
            entity.Property(e => e.Valor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("valor");
            entity.Property(e => e.ValorMax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("valor_max");
            entity.Property(e => e.ValorMin)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("valor_min");
        });

        modelBuilder.Entity<ParametrosGeneral>(entity =>
        {
            entity.HasKey(e => e.IdParametroGeneral);

            entity.ToTable("parametros_general");

            entity.Property(e => e.IdParametroGeneral)
                .ValueGeneratedNever()
                .HasColumnName("id_parametro_general");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.IdBloque).HasColumnName("id_bloque");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.ParametroGeneral)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("parametro_general");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("proveedor");
            entity.Property(e => e.Valor)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<Requerimiento>(entity =>
        {
            entity.HasKey(e => e.IdLocal);

            entity.ToTable("requerimiento");

            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.BloqueAlta)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque_alta");
            entity.Property(e => e.BloqueBorro)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque_borro");
            entity.Property(e => e.BloqueTermino)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque_termino");
            entity.Property(e => e.Concepto)
                .HasMaxLength(4056)
                .IsUnicode(false)
                .HasColumnName("concepto");
            entity.Property(e => e.Dirigido)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("dirigido");
            entity.Property(e => e.EsBorrado).HasColumnName("es_borrado");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaBorro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_borro");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaTermino)
                .HasColumnType("datetime")
                .HasColumnName("fecha_termino");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_vencimiento");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.IdRequerimiento).HasColumnName("id_requerimiento");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTipoFalla).HasColumnName("id_tipo_falla");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.IdUsuarioBorro).HasColumnName("id_usuario_borro");
            entity.Property(e => e.IdUsuarioTermino).HasColumnName("id_usuario_termino");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.Tipo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<RequerimientoPaso>(entity =>
        {
            entity.ToTable("requerimiento_paso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BloqueAlta)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque_alta");
            entity.Property(e => e.BloqueBorro)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque_borro");
            entity.Property(e => e.BloqueTermino)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("bloque_termino");
            entity.Property(e => e.Concepto)
                .HasMaxLength(4056)
                .IsUnicode(false)
                .HasColumnName("concepto");
            entity.Property(e => e.Dirigido)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("dirigido");
            entity.Property(e => e.EsBorrado).HasColumnName("es_borrado");
            entity.Property(e => e.FechaAlta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaBorro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_borro");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.FechaModificacionLocal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion_local");
            entity.Property(e => e.FechaTermino)
                .HasColumnType("datetime")
                .HasColumnName("fecha_termino");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_vencimiento");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.IdLocal).HasColumnName("id_local");
            entity.Property(e => e.IdRemotoInterfaz).HasColumnName("id_remoto_interfaz");
            entity.Property(e => e.IdRequerimiento).HasColumnName("id_requerimiento");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalInterfaz).HasColumnName("id_sucursal_interfaz");
            entity.Property(e => e.IdTipoFalla).HasColumnName("id_tipo_falla");
            entity.Property(e => e.IdUsuarioAlta).HasColumnName("id_usuario_alta");
            entity.Property(e => e.IdUsuarioBorro).HasColumnName("id_usuario_borro");
            entity.Property(e => e.IdUsuarioTermino).HasColumnName("id_usuario_termino");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.NoCajaInterfaz).HasColumnName("no_caja_interfaz");
            entity.Property(e => e.Tipo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<RespuestaNetpay>(entity =>
        {
            entity.ToTable("respuesta_netpay");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aid)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Arqc)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("ARQC");
            entity.Property(e => e.AuthCode)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.BankName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.CardNumber)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CardToken)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.CardType)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.CardTypeName)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.CustomerName)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.Cvm)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("CVM");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.MerchantId)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.OrderId)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Paciente)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("paciente");
            entity.Property(e => e.PosentryMode)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("POSEntryMode");
            entity.Property(e => e.Procesador)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("Netpay")
                .HasColumnName("procesador");
            entity.Property(e => e.Promocion)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("promocion");
            entity.Property(e => e.ResponseCode)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.ResponseMsg)
                .HasMaxLength(2048)
                .IsUnicode(false);
            entity.Property(e => e.ResponseText)
                .HasMaxLength(2048)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio);

            entity.ToTable("servicio");

            entity.Property(e => e.IdServicio)
                .ValueGeneratedNever()
                .HasColumnName("id_servicio");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Duracion).HasColumnName("duracion");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.IdTipoCita).HasColumnName("id_tipo_cita");
            entity.Property(e => e.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(e => e.MaxVentasPorCliente)
                .HasDefaultValue(1)
                .HasColumnName("max_ventas_por_cliente");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.PermitirVenta).HasColumnName("permitir_venta");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.SesionesPaquete).HasColumnName("sesiones_paquete");
            entity.Property(e => e.SesionesPaquetePrecio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sesiones_paquete_precio");
            entity.Property(e => e.TrDuracionLaser).HasColumnName("tr_duracion_laser");
            entity.Property(e => e.TrDuracionServicio).HasColumnName("tr_duracion_servicio");

            entity.HasOne(d => d.IdTipoCitaNavigation).WithMany(p => p.Servicio)
                .HasForeignKey(d => d.IdTipoCita)
                .HasConstraintName("FK_servicio_tipo_cita");

            entity.HasOne(d => d.IdTipoServicioNavigation).WithMany(p => p.Servicio)
                .HasForeignKey(d => d.IdTipoServicio)
                .HasConstraintName("FK_servicio_tipo_servicio");
        });

        modelBuilder.Entity<ServicioCombo>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK_servicio_combo_id");

            entity.ToTable("servicio_combo");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.Abreviatura)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("abreviatura");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdsServicio)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("ids_servicio");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
        });

        modelBuilder.Entity<ServicioSucursal>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);

            entity.ToTable("servicio_sucursal");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("precio");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ServicioSucursal)
                .HasForeignKey(d => d.IdServicio)
                .HasConstraintName("FK_servicio_sucursal_servicio");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.ServicioSucursal)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("FK_servicio_sucursal_sucursal");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal);

            entity.ToTable("sucursal");

            entity.Property(e => e.IdSucursal)
                .ValueGeneratedNever()
                .HasColumnName("id_sucursal");
            entity.Property(e => e.AgendaHoraFin)
                .HasColumnType("datetime")
                .HasColumnName("agenda_hora_fin");
            entity.Property(e => e.AgendaHoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("agenda_hora_inicio");
            entity.Property(e => e.AplicaPreciosBajos).HasColumnName("aplica_precios_bajos");
            entity.Property(e => e.AumentoPorcSesionExtendida)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("aumento_porc_sesion_extendida");
            entity.Property(e => e.AumentoSesionExtendida).HasColumnName("aumento_sesion_extendida");
            entity.Property(e => e.CaidaDomingo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("caida_domingo");
            entity.Property(e => e.CaidaSabado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("caida_sabado");
            entity.Property(e => e.CaidaViernes)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("caida_viernes");
            entity.Property(e => e.CantidadCitasEmpalmadas).HasColumnName("cantidad_citas_empalmadas");
            entity.Property(e => e.CiudadSucursal)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("ciudad_sucursal");
            entity.Property(e => e.CostoMaxVentaNuevaParaAplicarAnticipoPreciosBajos)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_max_venta_nueva_para_aplicar_anticipo_precios_bajos");
            entity.Property(e => e.CuotaMinParaNoPerderBono)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("cuota_min_para_no_perder_bono");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Direccion)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.DirectorComercial)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("director_comercial");
            entity.Property(e => e.DirectorComercialIdUsuarioSlack).HasColumnName("director_comercial_id_usuario_slack");
            entity.Property(e => e.EfevooDeviceId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("efevoo_device_id");
            entity.Property(e => e.EfevooSecret)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("efevoo_secret");
            entity.Property(e => e.EmailDirectorComercial)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("email_director_comercial");
            entity.Property(e => e.EmailSubdirector)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("email_subdirector");
            entity.Property(e => e.EmailsNotificacionToken)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("emails_notificacion_token");
            entity.Property(e => e.EmailsTokenPermitirSolicitarHasta6pagos)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("emails_token_permitir_solicitar_hasta_6pagos");
            entity.Property(e => e.EmailsTokenReventaEnCero)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("emails_token_reventa_en_cero");
            entity.Property(e => e.EsActiva).HasColumnName("es_activa");
            entity.Property(e => e.EsCredibanco).HasColumnName("es_credibanco");
            entity.Property(e => e.EsEfevoo).HasColumnName("es_efevoo");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.Gerente)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("gerente");
            entity.Property(e => e.GioInicioTurno1)
                .HasColumnType("datetime")
                .HasColumnName("gio_inicio_turno_1");
            entity.Property(e => e.GioInicioTurno2)
                .HasColumnType("datetime")
                .HasColumnName("gio_inicio_turno_2");
            entity.Property(e => e.GteCobranzaMinima)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gte_cobranza_minima");
            entity.Property(e => e.GteCuotaSemanalVenta)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gte_cuota_semanal_venta");
            entity.Property(e => e.HorarioDiferenteRecepcion).HasColumnName("horario_diferente_recepcion");
            entity.Property(e => e.HorarioEspecial).HasColumnName("horario_especial");
            entity.Property(e => e.HorarioEspecialDiferenteRecepcion).HasColumnName("horario_especial_diferente_recepcion");
            entity.Property(e => e.IdBloque).HasColumnName("id_bloque");
            entity.Property(e => e.IdGerente).HasColumnName("id_gerente");
            entity.Property(e => e.IdGerenteApoyo).HasColumnName("id_gerente_apoyo");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.IdUsuariosSlackToken)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("id_usuarios_slack_token");
            entity.Property(e => e.IdUsuariosSlackTokenPermitirSolicitarHasta6pagos)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("id_usuarios_slack_token_permitir_solicitar_hasta_6pagos");
            entity.Property(e => e.IdUsuariosSlackTokenReventaEnCero)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("id_usuarios_slack_token_reventa_en_cero");
            entity.Property(e => e.MontoSobreventa)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_sobreventa");
            entity.Property(e => e.PacienteMinPagadoHoyParaAnticipoReventaCero)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("paciente_min_pagado_hoy_para_anticipo_reventa_cero");
            entity.Property(e => e.PacienteMinPagadoParaAnticipoReventaCero)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("paciente_min_pagado_para_anticipo_reventa_cero");
            entity.Property(e => e.PasswordTerminalInterfaz)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("password_terminal_interfaz");
            entity.Property(e => e.PermitirReventaSinAnticipoMinimo).HasColumnName("permitir_reventa_sin_anticipo_minimo");
            entity.Property(e => e.PermitirSolicitarHasta6pagosXToken)
                .HasDefaultValue(false)
                .HasColumnName("permitir_solicitar_hasta_6pagos_x_token");
            entity.Property(e => e.PermitirTokenizacionConekta).HasColumnName("permitir_tokenizacion_conekta");
            entity.Property(e => e.PorcentajeComisionesMayor)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("porcentaje_comisiones_mayor");
            entity.Property(e => e.PorcentajeComisionesMenor)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("porcentaje_comisiones_menor");
            entity.Property(e => e.PotencialVenta)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("potencial_venta");
            entity.Property(e => e.Prefijo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("prefijo");
            entity.Property(e => e.SesionExtendida).HasColumnName("sesion_extendida");
            entity.Property(e => e.StoreidNetpay)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("storeid_netpay");
            entity.Property(e => e.StoreidTerminalInterfaz)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("storeid_terminal_interfaz");
            entity.Property(e => e.Subdirector)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("subdirector");
            entity.Property(e => e.SubdirectorIdUsuarioSlack).HasColumnName("subdirector_id_usuario_slack");
            entity.Property(e => e.TelefonoSucursal)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("telefono_sucursal");
            entity.Property(e => e.TerminalIdMercadoPago)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("terminal_id_mercado_pago");
            entity.Property(e => e.TiempoAdicionalPrimeraCita).HasColumnName("tiempo_adicional_primera_cita");
            entity.Property(e => e.TiempoMinimoPrimeraCita).HasColumnName("tiempo_minimo_primera_cita");
            entity.Property(e => e.TiempoMinimoSesionExtendida).HasColumnName("tiempo_minimo_sesion_extendida");
            entity.Property(e => e.Timezone)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("timezone");
            entity.Property(e => e.TipoTiempos)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo_tiempos");
            entity.Property(e => e.UrlWebTokenizacionConekta)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("url_web_tokenizacion_conekta");
            entity.Property(e => e.UsernameTerminalInterfaz)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("username_terminal_interfaz");
            entity.Property(e => e.ValidaToku)
                .HasDefaultValue(0)
                .HasColumnName("valida_toku");
            entity.Property(e => e.ValidarAreasMaxVentasPorCliente).HasColumnName("validar_areas_max_ventas_por_cliente");
            entity.Property(e => e.ValidarBinTarjetaAnticipoMinimo).HasColumnName("validar_bin_tarjeta_anticipo_minimo");
            entity.Property(e => e.ValidarBinTarjetaAnticipoMinimoMonto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("validar_bin_tarjeta_anticipo_minimo_monto");
            entity.Property(e => e.VendedorHoraCheckin)
                .HasColumnType("datetime")
                .HasColumnName("vendedor_hora_checkin");
            entity.Property(e => e.VentaMiminaBonoLd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("venta_mimina_bono_ld");
            entity.Property(e => e.VentaMinimaBono)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("venta_minima_bono");
        });

        modelBuilder.Entity<SucursalAgendaExterna>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);

            entity.ToTable("sucursal_agenda_externa");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalAfin).HasColumnName("id_sucursal_afin");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.SucursalAgendaExternaIdSucursalNavigation)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("FK_sucursal_agenda_externa_sucursal");

            entity.HasOne(d => d.IdSucursalAfinNavigation).WithMany(p => p.SucursalAgendaExternaIdSucursalAfinNavigation)
                .HasForeignKey(d => d.IdSucursalAfin)
                .HasConstraintName("FK_sucursal_agenda_externa_sucursal1");
        });

        modelBuilder.Entity<SucursalHorario>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);

            entity.ToTable("sucursal_horario");

            entity.Property(e => e.IdDetalle)
                .ValueGeneratedNever()
                .HasColumnName("id_detalle");
            entity.Property(e => e.Apertura)
                .HasColumnType("datetime")
                .HasColumnName("apertura");
            entity.Property(e => e.AperturaRecepcion)
                .HasColumnType("datetime")
                .HasColumnName("apertura_recepcion");
            entity.Property(e => e.BloqueoXComida).HasColumnName("bloqueo_x_comida");
            entity.Property(e => e.BloqueoXComidaMediaHora).HasColumnName("bloqueo_x_comida_media_hora");
            entity.Property(e => e.BloqueoXDescanso).HasColumnName("bloqueo_x_descanso");
            entity.Property(e => e.Cierre)
                .HasColumnType("datetime")
                .HasColumnName("cierre");
            entity.Property(e => e.CierreRecepcion)
                .HasColumnType("datetime")
                .HasColumnName("cierre_recepcion");
            entity.Property(e => e.Dia).HasColumnName("dia");
            entity.Property(e => e.FechaFinEspecial)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin_especial");
            entity.Property(e => e.FechaInicioEspecial)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio_especial");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
        });

        modelBuilder.Entity<SucursalesAfines>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("sucursales_afines");

            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalAfin).HasColumnName("id_sucursal_afin");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany()
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("FK_sucursales_afines_sucursal");

            entity.HasOne(d => d.IdSucursalAfinNavigation).WithMany()
                .HasForeignKey(d => d.IdSucursalAfin)
                .HasConstraintName("FK_sucursales_afines_sucursal1");
        });

        modelBuilder.Entity<TipoCita>(entity =>
        {
            entity.HasKey(e => e.IdTipoCita).HasName("PK_TIPO_CITA");

            entity.ToTable("tipo_cita");

            entity.Property(e => e.IdTipoCita).HasColumnName("id_tipo_cita");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumento);

            entity.ToTable("tipo_documento");

            entity.Property(e => e.IdTipoDocumento).HasColumnName("id_tipo_documento");
            entity.Property(e => e.Abreviatura)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("abreviatura");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsBorrado).HasColumnName("es_borrado");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
        });

        modelBuilder.Entity<TipoFalla>(entity =>
        {
            entity.HasKey(e => e.IdTipoFalla);

            entity.ToTable("tipo_falla");

            entity.Property(e => e.IdTipoFalla)
                .ValueGeneratedNever()
                .HasColumnName("id_tipo_falla");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.TiempoRespuesta).HasColumnName("tiempo_respuesta");
            entity.Property(e => e.TipoRequerimiento)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipo_requerimiento");
        });

        modelBuilder.Entity<TipoIdentificacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoIdentificacion).HasName("PK_tipo_identificacion_id");

            entity.ToTable("tipo_identificacion");

            entity.Property(e => e.IdTipoIdentificacion)
                .ValueGeneratedNever()
                .HasColumnName("id_tipo_identificacion");
            entity.Property(e => e.Clave)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
        });

        modelBuilder.Entity<TipoParametroAnticipoMinimo>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK_tipo_parametro_anticipo_minimo_id_tipo");

            entity.ToTable("tipo_parametro_anticipo_minimo");

            entity.Property(e => e.IdTipo)
                .ValueGeneratedNever()
                .HasColumnName("id_tipo");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<TipoServicio>(entity =>
        {
            entity.HasKey(e => e.IdTipoServicio);

            entity.ToTable("tipo_servicio");

            entity.Property(e => e.IdTipoServicio).HasColumnName("id_tipo_servicio");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
        });

        modelBuilder.Entity<TokenAutorizacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("token_autorizacion");

            entity.Property(e => e.IdDetalle)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_detalle");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdPaqueteLocal).HasColumnName("id_paquete_local");
            entity.Property(e => e.Tipo)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.Token)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("token");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK_USUARIO");

            entity.ToTable("usuario");

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedNever()
                .HasColumnName("id_usuario");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EsActivo).HasColumnName("es_activo");
            entity.Property(e => e.EsUsuarioSistema).HasColumnName("es_usuario_sistema");
            entity.Property(e => e.FechaInterfaz)
                .HasColumnType("datetime")
                .HasColumnName("fecha_interfaz");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalAgendaExterna).HasColumnName("id_sucursal_agenda_externa");
            entity.Property(e => e.IdUsuarioSlack).HasColumnName("id_usuario_slack");
            entity.Property(e => e.Login)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.MsjUsuarioInasistencia)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("msj_usuario_inasistencia");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("tipo_usuario");
            entity.Property(e => e.UsuarioInasistencia).HasColumnName("usuario_inasistencia");
        });

        modelBuilder.Entity<VersionesInstaladas>(entity =>
        {
            entity.ToTable("versiones_instaladas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Version)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("version");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
