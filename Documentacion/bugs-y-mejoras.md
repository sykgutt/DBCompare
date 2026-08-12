# Bugs y Mejoras — DBCompare

Inventario de hallazgos ordenados por severidad, con estado de corrección. Basado en la auditoría de agosto 2026 (que amplía el [análisis previo de julio 2026](analisis-proyecto.md)).

Leyenda de estado: ✅ Corregido en esta revisión · 📝 Limitación documentada / aceptada · ⏳ Diferido (fuera de alcance de código de aplicación).

## Resumen

| Severidad | Total | Corregidos | Documentados / diferidos |
|---|---|---|---|
| Crítica | 6 | 6 | 0 |
| Alta | 8 | 8 | 0 |
| Media | 10 | 9 | 1 (M7) |
| Baja | 8 | 6 | 2 (B5, B8) |

No quedan bugs de aplicación pendientes de implementación. B5 (código comentado) y B8 (paridad de scripts SQL) quedan como deuda no bloqueante. El roadmap de modernización (.NET 8, Dapper, CI, proyecto de tests) sigue fuera de alcance.

## Críticos

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| C1 | Credenciales reales hardcodeadas como valores por defecto (servidor `10.90.7.12`, usuario `datadealercollectorDev`, password `OErSd8AtVrRw`; servidor `10.90.6.3`, usuario `Perezd7`, password `NissanDdc16`) | [`DBCompare/Utilidades.cs`](../DBCompare/Utilidades.cs) líneas 30-38 | ✅ Corregido — defaults vaciados |
| C2 | Credenciales hardcodeadas en prototipo (`POSTA` / `kanasz` / `chaaron`) | [`DBCompare/Form1.cs`](../DBCompare/Form1.cs) | ✅ Corregido — archivo eliminado (no estaba en el `.csproj`, era código muerto) |
| C3 | Contraseña por defecto precargada en el diseñador (`OErSd8AtVrRw`) | [`DBDocumentation/Main.Designer.cs`](../DBDocumentation/Main.Designer.cs) línea 270 (y usuario/servidor/BD en líneas cercanas) | ✅ Corregido — campos vaciados |
| C4 | Contraseñas persistidas en texto plano en `Setting.ini` y en los proyectos XML guardados (`ObjectCompare.cs` / `MDIMain.cs`) | [`DBCompare/Login.cs`](../DBCompare/Login.cs), [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs), [`MDIMain.cs`](../DBCompare/MDIMain.cs) | ✅ Corregido — `CredentialProtector` (DPAPI `CurrentUser`); `GetIni`/`SetIni` cifran la clave `Password`; XML guarda/carga cifrado; valores antiguos en texto plano se aceptan al leer |
| C5 | SQL dinámico e inyección SQL: la lista de tablas seleccionadas se concatena sin escapar en `WHERE [FullName] IN (###)`, y la query resultante se ejecuta con `EXEC (@Query)` | [`DBCompare/DataCompare.cs`](../DBCompare/DataCompare.cs) | ✅ Corregido — `SanitizeTableInList` valida `^\[[^\]]+\]\.\[[^\]]+\]$` y reescribe literales `N'...'` |
| C6 | `BinaryFormatter` para serialización (`ObjectToByteArray`) — deserialización insegura, sin uso activo en el flujo principal | [`ObjectHelper/ObjectDB.cs`](../ObjectHelper/ObjectDB.cs) | ✅ Corregido — método eliminado (junto con B2) |

**Nota sobre rotación de credenciales:** las contraseñas de C1–C3 ya quedaron expuestas en el historial de git antes de esta corrección. Eliminarlas del código actual no las invalida retroactivamente; se recomienda rotarlas en los servidores afectados y, si es viable, limpiar el historial del repositorio.

## Altos

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| A1 | Binding redirect incorrecto: `Microsoft.SqlServer.Smo` y `Microsoft.SqlServer.Management.Sdk.Sfc` redirigen a `17.0.0.0` en vez de `18.100.0.0` | [`DBCompare/app.config`](../DBCompare/app.config) | ✅ Corregido |
| A2 | Versión de SMO desalineada entre proyectos: `DBCompare` usaba `180.10.0` vs `181.12.0` en `ObjectHelper`/`DBDocumentation` | [`DBCompare/packages.config`](../DBCompare/packages.config), [`DBCompare/DBCompare.csproj`](../DBCompare/DBCompare.csproj) | ✅ Corregido — unificado a `181.12.0` |
| A3 | `SqlDataReader` devuelto después de cerrar la conexión que lo originó (`conn.Close()` antes del `return`); el reader queda inválido | [`DBCompare/Database.cs`](../DBCompare/Database.cs) método `EjecuteScalar` | ✅ Corregido — método eliminado (no se usaba) |
| A4 | Selección de script SQL por versión sin *fallback*: `"{Categoria}_" + ServerMajorVersion + ".sql"`; si el recurso no existe, `GetManifestResourceStream` devuelve `null` y el `StreamReader` lanza `NullReferenceException` | [`ObjectHelper/ScriptGenerator.cs`](../ObjectHelper/ScriptGenerator.cs) | ✅ Corregido — `GetVersionedResourceScript` busca desde la versión del servidor hasta 7 |
| A5 | `ScriptingOptions` (`so`) se pasa por referencia y se muta durante el fetch de DB1; la misma instancia se reutiliza para DB2, heredando flags deshabilitados de un servidor más antiguo | [`DBCompare/ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) | ✅ Corregido — se clona `scriptOpt` por servidor (`ICloneable`) antes de cada fetch |
| A6 | Bug de asignación: al reconstruir `Server2` desde el proyecto XML se comprueba `login1 != ""` en lugar de `login2 != ""`, lo que puede forzar autenticación SQL/Windows incorrecta en el segundo servidor | [`DBCompare/MDIMain.cs`](../DBCompare/MDIMain.cs) | ✅ Corregido — ahora usa `login2` |
| A7 | Comparación de objetos por `Schema + Name` sin considerar `Type`: dos objetos de tipos distintos (p. ej. una tabla y una vista) con el mismo esquema y nombre colisionan en la clasificación | [`DBCompare/ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) método `CompareObjects()` | ✅ Corregido — `query1`/`query2` usan la misma clave `Type.Schema.Name` que `query3`/`query4` |
| A8 | Dependencia de SMO instalado con SSMS (GAC / publisher policy) al ejecutar desde `bin` | [`DBCompare/app.config`](../DBCompare/app.config), [`DBDocumentation/app.config`](../DBDocumentation/app.config), `.csproj` | ✅ Corregido en la pasada SMO — `publisherPolicy apply="no"`, Copy Local, `SpecificVersion`, guía en [despliegue.md](despliegue.md) |

## Medios

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| M1 | Excepciones tragadas sin log (`catch (Exception) { }` o variable descartada) | [`Database.cs`](../DBCompare/Database.cs), [`AutoCompleteTextBox.cs`](../DBCompare/AutoCompleteTextBox.cs), [`ObjectFetch.cs`](../DBCompare/ObjectFetch.cs), [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs) | ✅ Corregido — `Trace.WriteLine` |
| M2 | `CheckColumnsByTable` es un stub: devuelve un string formateado en lugar de comparar columnas y generar `ALTER TABLE` | [`DBCompare/ObjectCompare.cs`](../DBCompare/ObjectCompare.cs) | ✅ Corregido — tablas modificadas usan el mismo `DROP` + `CREATE` que el resto, con comentario explícito (no se emite `ALTER COLUMN` por columna) |
| M3 | `GenerateObjectDocumentationFile` es un stub: solo obtiene el tipo del objeto, no genera la página de detalle; los enlaces del índice HTML apuntan a páginas inexistentes | [`DBDocumentation/Main.cs`](../DBDocumentation/Main.cs) | ✅ Corregido — genera `{ObjectId}.html` con tipo, nombre, descripción y script |
| M4 | `DataCompare` usa una lista de tablas fija guardada en el INI (`Tablas`) en vez de descubrir tablas dinámicamente desde `sys.tables` / `INFORMATION_SCHEMA.TABLES` | [`DBCompare/DataCompare.cs`](../DBCompare/DataCompare.cs) | ✅ Ya cubierto — `Query()` lista `sys.tables`; el INI solo recuerda los checks de la sesión anterior |
| M5 | `saveToolStripMenuItem_Click` vacío en el menú principal | [`DBCompare/MDIMain.cs`](../DBCompare/MDIMain.cs) | ✅ Corregido — Save y Save As delegan en `ObjectCompare.Save()` |
| M6 | Posible bug de datos: `IsIncluded` se asigna leyendo la columna `is_descending_key` en lugar de un flag real de columna incluida (`INCLUDE`) | [`ObjectHelper/ObjectDB.cs`](../ObjectHelper/ObjectDB.cs) | ✅ Corregido — lee `is_included_column` |
| M7 | Comparación de definiciones solo por normalización de espacios en blanco (`RemoveWhiteSpaces`); cambios de formato se reportan como diferencias y algunos cambios semánticos con igual espaciado podrían pasar inadvertidos | [`DBCompare/ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) | 📝 Limitación aceptada — la clasificación ignora ruido de formato; DiffPlex sigue mostrando el diff visual completo |
| M8 | Placeholder de contraseña hardcodeado en el script generado para `APPLICATION ROLE` (`WITH PASSWORD='fsdjfhkfasjhfkhjhklsaf465'`) | [`ObjectHelper/DBObjectType/Principal.cs`](../ObjectHelper/DBObjectType/Principal.cs) | ✅ Corregido — placeholder `*** REPLACE_PASSWORD_BEFORE_EXECUTE ***` (SQL Server exige `PASSWORD` y el valor real no está en metadatos) |
| M9 | `Table.Script()` (overload sin parámetros) fuerza `ServerMajorVersion = 9`, generando DDL con reglas de una versión obsoleta si se invoca sin pasar `ScriptingOptions` explícito | [`ObjectHelper/DBObjectType/Table.cs`](../ObjectHelper/DBObjectType/Table.cs) | ✅ Corregido — ya no fuerza la versión 9; el fetch principal usa `Script(so)` con la versión del servidor |
| M10 | UI bloqueada durante operaciones largas y síncronas (fetch de esquema completo en `Login`/SMO, `CompareData` en `DataCompare`) | [`DBCompare/Login.cs`](../DBCompare/Login.cs), [`DBCompare/DataCompare.cs`](../DBCompare/DataCompare.cs) | ✅ Corregido — `CompareData` corre en `Task.Run` (lista de tablas leída en el hilo UI); Login ya usaba cursor de espera al enumerar servidores |

## Bajos

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| B1 | Código muerto: prototipo `Form1` con credenciales embebidas, no referenciado en el `.csproj` | `DBCompare/Form1.cs`, `Form1.Designer.cs`, `Form1.resx` | ✅ Corregido — eliminado |
| B2 | `ObjectToByteArray` (BinaryFormatter) nunca invocado | [`ObjectHelper/ObjectDB.cs`](../ObjectHelper/ObjectDB.cs) | ✅ Corregido — eliminado (junto con C6) |
| B3 | `DatabaseConnect.EjecuteScalar()` no referenciado fuera de la propia clase | [`DBCompare/Database.cs`](../DBCompare/Database.cs) | ✅ Corregido — método eliminado (junto con A3) |
| B4 | `StreamReader` sin `using` al leer cada script embebido | [`ObjectHelper/ScriptGenerator.cs`](../ObjectHelper/ScriptGenerator.cs) | ✅ Corregido — `using` en `GetResourceScript` / `GetVersionedResourceScript` |
| B5 | Bloques extensos de código comentado | [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs), [`ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) | ⏳ Diferido — limpieza cosmética, no afecta runtime; se deja como deuda de mantenimiento |
| B6 | Duplicación: `RefreshDatabaseList`/`RefreshServerList` repetidos entre `Login.cs` y `DBDocumentation/Main.cs` | ambos archivos | ✅ Corregido — extraído a [`ObjectHelper/SmoCatalog.cs`](../ObjectHelper/SmoCatalog.cs) |
| B7 | `File.WriteAllText` sin encoding explícito (depende del ANSI del sistema) | [`DataCompare.cs`](../DBCompare/DataCompare.cs), [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs) | ✅ Corregido — `Encoding.UTF8` |
| B8 | 128+ scripts SQL versionados (`_7` a `_17`) sin pruebas de paridad entre versiones consecutivas | [`ObjectHelper/SQL/`](../ObjectHelper/SQL/) | ⏳ Diferido — mitigado por `GetVersionedResourceScript` (A4); tests de paridad requieren un proyecto de test |

## Correcciones aplicadas en esta revisión

1. **Eliminación de credenciales hardcodeadas** (C1, C2, C3):
   - [`DBCompare/Utilidades.cs`](../DBCompare/Utilidades.cs): los valores por defecto de `Setting.ini` (servidor, base de datos, usuario, password de `SetupDB1`/`SetupDB2`) quedan vacíos; las rutas de Beyond Compare/WinMerge/caché usan valores neutros.
   - [`DBDocumentation/Main.Designer.cs`](../DBDocumentation/Main.Designer.cs): se vaciaron los valores precargados de servidor, base de datos, usuario y contraseña.
   - Se eliminó `DBCompare/Form1.cs`, `Form1.Designer.cs` y `Form1.resx` (prototipo abandonado, no compilado, con credenciales embebidas).
   - Se añadió `Setting.ini` al [`.gitignore`](../.gitignore) para evitar que credenciales generadas localmente se suban al repositorio.
2. **Alineación de SMO** (A1, A2, A8) e **independencia de SSMS**:
   - `DBCompare` referencia `Microsoft.SqlServer.SqlManagementObjects 181.12.0`, igual que `ObjectHelper` y `DBDocumentation`.
   - `bindingRedirect` a `18.100.0.0` más `<publisherPolicy apply="no"/>` en `DBCompare/app.config` y `DBDocumentation/app.config`.
   - Copy Local (`Private=True`) y `SpecificVersion=True` en las refs SMO; `AutoGenerateBindingRedirects` en los tres `.csproj`.
   - Guía de copia de `bin`: [despliegue.md](despliegue.md).
3. **Bugs altos** (A3, A4, A5, A6, A7) y fugas menores (B3, B4):
   - Eliminado `EjecuteScalar` (reader inválido y código muerto).
   - Fallback de scripts SQL versionados en `ScriptGenerator`.
   - Corregido `login2` al cargar un proyecto XML.
   - `ScriptingOptions` clonado por servidor.
   - Comparación de objetos por `Type.Schema.Name`.
4. **Cifrado de secretos y SQL** (C4, C5, C6/B2):
   - [`DBCompare/CredentialProtector.cs`](../DBCompare/CredentialProtector.cs): DPAPI `CurrentUser`; prefijo `DPAPI:`.
   - Login persiste servidor/BD/usuario/password cifrado al conectar con éxito.
   - Validación de nombres de tabla en `DataCompare.QueryDatos`.
   - Eliminado `BinaryFormatter` / `ObjectToByteArray`.
   - Connection strings de `Database.cs` con `Encrypt=True;TrustServerCertificate=True`.
5. **Medios y bajos de aplicación** (M1–M6, M8–M10, B6, B7):
   - Logging con `Trace` en catches vacíos.
   - Documentación HTML de detalle, Save/Save As, `IsIncluded`, placeholder de application role, `Table.Script()` sin forzar v9.
   - `SmoCatalog` compartido; escrituras UTF-8; `CompareData` en segundo plano.

## Roadmap de mejoras (fuera del alcance de esta revisión)

### Arquitectura
- Extraer la lógica de comparación (`CompareObjects`, `QueryDatos`) de los formularios a servicios independientes de la UI, testeables.
- Evaluar consolidar los 128+ scripts versionados (el fallback por versión ya está en `ScriptGenerator`).

### Calidad y pruebas
- Crear un proyecto de tests (xUnit/NUnit) cubriendo `ScriptGenerator`, hidratación de `ObjectDB` y normalización de comparación (cubre B8).
- Configurar CI (build + tests) en GitHub Actions.
- Limpieza de bloques comentados en `ObjectCompare` / `ObjectFetch` (B5).

### Modernización (largo plazo)
- Migrar de .NET Framework 4.8 a .NET 8 (WinForms moderno).
- Sustituir acceso por `DataTable`/`DataView` por un micro-ORM (Dapper).
- Reemplazar INI por `appsettings.json` + `IConfiguration`.
