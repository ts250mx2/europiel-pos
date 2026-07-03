# Plan de migración EuropielPOS: VB.NET / .NET Framework 4.0 → C# / .NET 8

## Radiografía del proyecto actual

| Aspecto | Estado |
|---|---|
| Lenguaje / Framework | VB.NET, .NET Framework 4.0 (sin soporte desde 2016) |
| UI | WinForms, 58 formularios |
| Tamaño | ~78,000 líneas de código (sin designers) |
| Proyectos | EuropielPOS (POS), EuropielPOS_Job, EuropielPOS_UploadDocumentos, SQL2012_Installer, ComponenteEscaneo, EuropielPOSBrasil |
| Reportes | **93 archivos Crystal Reports (.rpt)** |
| Datos | Entity Framework 6 + SQL Server local (2012) |
| Despliegue | ClickOnce con **40+ perfiles de publicación** (por país / procesador de pago) |
| Pagos | PinpadConnector + Stripe, MercadoPago, Kushki, Toku, Credibanco, EfevooPay, Nemuru, Smartlink |
| UI de terceros | Infragistics v22.1, Syncfusion (Grid, Schedule) |
| Otros | AWS S3, Chilkat, iTextSharp, EPPlus, ActiveX (csXImage escaneo, SHDocVw browser), servicios SOAP |
| Config de compilador | `Option Strict Off` ⚠️ (mucho late binding — punto de dolor en la conversión) |

## Bloqueadores identificados (lo que NO migra directo a .NET 8)

1. **Crystal Reports** — SAP no lo soporta en .NET Core/8. Los 93 .rpt hay que reescribirlos.
2. **ActiveX** (`csXImage` escaneo, `SHDocVw` browser embebido) — no funcionan en .NET 8. Reemplazos: WIA/NTwain para escaneo, WebView2 para browser.
3. **Servicios SOAP** (Service References) — hay que regenerar clientes con `dotnet-svcutil` o migrar a REST.
4. **`Option Strict Off`** — el código con late binding se convierte a C# como `dynamic` o falla; requiere limpieza manual.
5. **EF6 → EF Core** — el modelo (¿EDMX?) requiere regenerarse.
6. **Infragistics/Syncfusion** — sí existen para .NET 8, pero hay que verificar licencias vigentes.
7. **SQL Server 2012** — sin soporte desde julio 2022; aprovechar para subir a SQL 2022 Express o LocalDB.

Lo que sí sobrevive casi intacto: lógica de negocio, Newtonsoft.Json, AWS SDK, EPPlus, ClickOnce (soportado en .NET 8), la arquitectura offline con BD local.

---

## Fase 0 — Preparación (2-3 semanas)

- [ ] Poner el código en **Git** (hoy está en TFS/TFVC de `develtec.visualstudio.com`).
- [ ] **Inventario de los 40+ publish profiles**: documentar qué difiere entre cada uno (app.config, flags, endpoints). Meta: entender si son 40 productos o 1 producto con 40 configuraciones.
- [ ] Identificar los 10-15 flujos críticos (venta, cobro con pinpad, corte de caja, cancelación, sincronización) y escribir **pruebas de caracterización** (aunque sean de integración manual documentada).
- [ ] Activar `Option Strict On` como warning y medir cuántos errores salen — eso dimensiona el esfuerzo de la Fase 2.
- [ ] Congelar features nuevas en lo posible durante la migración (o mantener rama dual con disciplina).

## Fase 1 — Retarget a .NET Framework 4.8 (1-2 semanas)

Paso intermedio de bajo riesgo, aún en VB.NET:

- [ ] Cambiar `TargetFrameworkVersion` de v4.0 → v4.8 en los 4+ proyectos.
- [ ] Actualizar paquetes NuGet compatibles (Newtonsoft, EF 6.4, AWS SDK).
- [ ] Compilar, probar flujos críticos, **publicar a una sucursal piloto**.

Beneficio: valida el pipeline de pruebas/despliegue antes de tocar el código, y 4.8 es prerequisito para las herramientas de migración.

## Fase 2 — Conversión VB.NET → C# (4-8 semanas)

- [ ] Convertir proyecto por proyecto con **[Code Converter de icsharpcode](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.CodeConverter)** (gratuito, ~90-95% automático). Orden sugerido: `EuropielPOS_UploadDocumentos` (pequeño) → `EuropielPOS_Job` → `EuropielPOS`.
- [ ] Resolver a mano lo que la herramienta no traduce bien: late binding (`Option Strict Off`), `My.*` (My.Settings, My.Computer), handles de eventos con `Handles`, módulos VB.
- [ ] Regla de oro: **conversión literal, cero refactoring en esta fase**. El diff debe ser "mismo comportamiento, otro lenguaje".
- [ ] Al cierre de cada proyecto convertido: compilar, correr flujos críticos, comparar contra la versión VB.

## Fase 3 — Migración a .NET 8 (4-8 semanas)

- [ ] Convertir los .csproj a formato **SDK-style** (con .NET Upgrade Assistant).
- [ ] Retarget a `net8.0-windows` con `<UseWindowsForms>true</UseWindowsForms>`.
- [ ] Reemplazos obligados:
  - **EF6 → EF Core 8**: regenerar modelo con scaffolding desde la BD existente.
  - **SOAP**: regenerar clientes con `dotnet-svcutil` (o aprovechar para migrar los servicios a REST si controlan el backend).
  - **SHDocVw/WebBrowser → WebView2**.
  - **csXImage (escaneo) → NTwain o WIA**.
  - **Infragistics/Syncfusion**: actualizar a versiones .NET 8 (Syncfusion tiene community license si aplica; los controles cambian poco de API).
  - Verificar que **PinpadConnector** y **Chilkat** tengan build .NET 8 (Chilkat sí; el pinpad depende del proveedor — si es solo interop COM/serial, funciona).
- [ ] ClickOnce sigue funcionando en .NET 8 — mantener el esquema de despliegue actual.

## Fase 4 — Reportes: Crystal → alternativa moderna (6-10 semanas, paralelizable con Fase 3)

Es el trabajo más voluminoso (93 reportes) pero mecánico. Opciones:

| Opción | Pros | Contras |
|---|---|---|
| **FastReport .NET** | Importador automático de .rpt, diseñador visual | Licencia (~$800 USD/dev) |
| **QuestPDF** | Gratis (revenue < $1M), código C# puro, excelente para tickets | Todo se reescribe a mano, sin diseñador |
| **Microsoft RDLC (ReportViewer)** | Gratis, conversores rpt→rdlc disponibles | Tecnología estancada |

Recomendación: **FastReport** por el importador de .rpt (los 93 reportes no se reescriben desde cero), o **QuestPDF** si el presupuesto manda y la mayoría son tickets/recibos simples (muchos parecen variantes: `repReciboCaja` × 6 países).

- [ ] Clasificar los 93 .rpt: muchos son **variantes por país del mismo reporte** → consolidar en plantillas parametrizadas (probablemente queden 25-35 reales).
- [ ] Migrar primero los de ticket/recibo (flujo de venta), después clínicos/administrativos.

## Fase 5 — Consolidación multi-país/multi-pago (opcional pero muy recomendada)

Los 40+ publish profiles son deuda técnica seria: 40 builds del mismo producto.

- [ ] Mover diferencias por país/procesador a **configuración en runtime** (BD o appsettings por tenant) en lugar de compilaciones separadas.
- [ ] Patrón strategy para procesadores de pago (Stripe, Kushki, Toku... detrás de una interfaz `IPaymentProcessor`).
- [ ] Meta: **un solo build, N configuraciones**. Reduce el costo de cada release de 40 publicaciones a 1.

## Fase 6 — Modernización de UI (opcional, incremental)

Solo después de estabilizar todo lo anterior:

- Temas modernos para WinForms (bajo costo), o
- Migración gradual de pantallas clave a **Blazor Hybrid** (WinForms host + WebView2) si quieren UI táctil/moderna sin big-bang.

---

## Resumen de esfuerzo

| Fase | Duración estimada | Riesgo |
|---|---|---|
| 0. Preparación | 2-3 sem | Bajo |
| 1. Retarget 4.8 | 1-2 sem | Bajo |
| 2. VB → C# | 4-8 sem | Medio (late binding) |
| 3. .NET 8 | 4-8 sem | Medio-alto (EF Core, SOAP, ActiveX) |
| 4. Reportes | 6-10 sem (paralelo) | Medio (volumen) |
| 5. Multi-tenant | 4-6 sem | Medio |
| 6. UI | Incremental | Bajo |

**Total: ~6-9 meses** con 1-2 desarrolladores dedicados, con el POS operando en producción todo el tiempo (cada fase termina en un estado desplegable).

## Principios del plan

1. **Nunca hay big-bang**: cada fase entrega algo desplegable a una sucursal piloto.
2. **Una variable a la vez**: primero lenguaje (misma plataforma), luego plataforma (mismo código), nunca ambas juntas.
3. **Los flujos de dinero se prueban primero**: venta, cobro, corte, cancelación — en cada fase.
4. **La versión VB queda congelada como respaldo** hasta que la C# lleve 1-2 meses estable en producción.
