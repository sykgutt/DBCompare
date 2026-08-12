# Bugs y Mejoras — DBCompare

Inventario de hallazgos ordenados por severidad, con estado de corrección. Basado en la auditoría de agosto 2026 (que amplía el [análisis previo de julio 2026](analisis-proyecto.md)).

Leyenda de estado: ✅ Corregido en esta revisión · ⏳ Pendiente (documentado, no implementado).

## Resumen

| Severidad | Total | Corregidos | Pendientes |
|---|---|---|---|
| Crítica | 6 | 2 | 4 |
| Alta | 7 | 0 | 7 |
| Media | 10 | 0 | 10 |
| Baja | 8 | 1 | 7 |

## Críticos

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| C1 | Credenciales reales hardcodeadas como valores por defecto (servidor `10.90.7.12`, usuario `datadealercollectorDev`, password `OErSd8AtVrRw`; servidor `10.90.6.3`, usuario `Perezd7`, password `NissanDdc16`) | [`DBCompare/Utilidades.cs`](../DBCompare/Utilidades.cs) líneas 30-38 | ✅ Corregido — defaults vaciados |
| C2 | Credenciales hardcodeadas en prototipo (`POSTA` / `kanasz` / `chaaron`) | [`DBCompare/Form1.cs`](../DBCompare/Form1.cs) | ✅ Corregido — archivo eliminado (no estaba en el `.csproj`, era código muerto) |
| C3 | Contraseña por defecto precargada en el diseñador (`OErSd8AtVrRw`) | [`DBDocumentation/Main.Designer.cs`](../DBDocumentation/Main.Designer.cs) línea 270 (y usuario/servidor/BD en líneas cercanas) | ✅ Corregido — campos vaciados |
| C4 | Contraseñas persistidas en texto plano en `Setting.ini` y en los proyectos XML guardados (`ObjectCompare.cs` / `MDIMain.cs`) | [`DBCompare/Login.cs`](../DBCompare/Login.cs), [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs), [`MDIMain.cs`](../DBCompare/MDIMain.cs) | ⏳ Pendiente — requiere cifrado (`ProtectedData.Protect`) o eliminar la persistencia de password |
| C5 | SQL dinámico e inyección SQL: la lista de tablas seleccionadas se concatena sin escapar en `WHERE [FullName] IN (###)`, y la query resultante se ejecuta con `EXEC (@Query)` | [`DBCompare/DataCompare.cs`](../DBCompare/DataCompare.cs) líneas ~224, 389-399 | ⏳ Pendiente — parametrizar / validar nombres con regex |
| C6 | `BinaryFormatter` para serialización (`ObjectToByteArray`) — deserialización insegura, sin uso activo en el flujo principal | [`ObjectHelper/ObjectDB.cs`](../ObjectHelper/ObjectDB.cs) líneas ~1731-1740 | ⏳ Pendiente — eliminar el método (código muerto) |

**Nota sobre rotación de credenciales:** las contraseñas de C1–C3 ya quedaron expuestas en el historial de git antes de esta corrección. Eliminarlas del código actual no las invalida retroactivamente; se recomienda rotarlas en los servidores afectados y, si es viable, limpiar el historial del repositorio.

## Altos

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| A1 | Binding redirect incorrecto: `Microsoft.SqlServer.Smo` y `Microsoft.SqlServer.Management.Sdk.Sfc` redirigen a `17.0.0.0` en vez de `18.100.0.0` | [`DBCompare/app.config`](../DBCompare/app.config) | ✅ Corregido |
| A2 | Versión de SMO desalineada entre proyectos: `DBCompare` usaba `180.10.0` vs `181.12.0` en `ObjectHelper`/`DBDocumentation` | [`DBCompare/packages.config`](../DBCompare/packages.config), [`DBCompare/DBCompare.csproj`](../DBCompare/DBCompare.csproj) | ✅ Corregido — unificado a `181.12.0` |
| A3 | `SqlDataReader` devuelto después de cerrar la conexión que lo originó (`conn.Close()` antes del `return`); el reader queda inválido | [`DBCompare/Database.cs`](../DBCompare/Database.cs) método `EjecuteScalar`, líneas 71-87 | ⏳ Pendiente |
| A4 | Selección de script SQL por versión sin *fallback*: `"{Categoria}_" + ServerMajorVersion + ".sql"`; si el recurso no existe, `GetManifestResourceStream` devuelve `null` y el `StreamReader` lanza `NullReferenceException` | [`ObjectHelper/ScriptGenerator.cs`](../ObjectHelper/ScriptGenerator.cs) (patrón usado en ~11 categorías) | ⏳ Pendiente — implementar `ResolveScriptVersion(major) => min(major, maxEmbedded)` |
| A5 | `ScriptingOptions` (`so`) se pasa por referencia y se muta durante el fetch de DB1; la misma instancia se reutiliza para DB2, heredando flags deshabilitados de un servidor más antiguo | [`ObjectHelper/ScriptGenerator.cs`](../ObjectHelper/ScriptGenerator.cs) (p. ej. línea 50, `so.DataCompression = false`), consumido en [`DBCompare/ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) líneas ~76-88, 151 | ⏳ Pendiente — clonar `so` por servidor antes de cada fetch |
| A6 | Bug de asignación: al reconstruir `Server2` desde el proyecto XML se comprueba `login1 != ""` en lugar de `login2 != ""`, lo que puede forzar autenticación SQL/Windows incorrecta en el segundo servidor | [`DBCompare/MDIMain.cs`](../DBCompare/MDIMain.cs) línea ~236 | ⏳ Pendiente |
| A7 | Comparación de objetos por `Schema + Name` sin considerar `Type`: dos objetos de tipos distintos (p. ej. una tabla y una vista) con el mismo esquema y nombre colisionan en la clasificación | [`DBCompare/ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) método `CompareObjects()`, línea ~569 | ⏳ Pendiente |

## Medios

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| M1 | Excepciones tragadas sin log (`catch (Exception) { }` o variable descartada) | [`Database.cs`](../DBCompare/Database.cs):39, [`AutoCompleteTextBox.cs`](../DBCompare/AutoCompleteTextBox.cs):442, [`ObjectFetch.cs`](../DBCompare/ObjectFetch.cs):79,103,790,881, [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs):786 | ⏳ Pendiente |
| M2 | `CheckColumnsByTable` es un stub: devuelve un string formateado en lugar de comparar columnas y generar `ALTER TABLE` | [`DBCompare/ObjectCompare.cs`](../DBCompare/ObjectCompare.cs) líneas 634-645 | ⏳ Pendiente |
| M3 | `GenerateObjectDocumentationFile` es un stub: solo obtiene el tipo del objeto, no genera la página de detalle; los enlaces del índice HTML apuntan a páginas inexistentes | [`DBDocumentation/Main.cs`](../DBDocumentation/Main.cs) líneas 187-191 | ⏳ Pendiente |
| M4 | `DataCompare` usa una lista de tablas fija guardada en el INI (`Tablas`) en vez de descubrir tablas dinámicamente desde `sys.tables` / `INFORMATION_SCHEMA.TABLES` | [`DBCompare/DataCompare.cs`](../DBCompare/DataCompare.cs) | ⏳ Pendiente |
| M5 | `saveToolStripMenuItem_Click` vacío en el menú principal | [`DBCompare/MDIMain.cs`](../DBCompare/MDIMain.cs) | ⏳ Pendiente |
| M6 | Posible bug de datos: `IsIncluded` se asigna leyendo la columna `is_descending_key` en lugar de un flag real de columna incluida (`INCLUDE`) | [`ObjectHelper/ObjectDB.cs`](../ObjectHelper/ObjectDB.cs) línea 1529 | ⏳ Pendiente |
| M7 | Comparación de definiciones solo por normalización de espacios en blanco (`RemoveWhiteSpaces`); cambios de formato se reportan como diferencias y algunos cambios semánticos con igual espaciado podrían pasar inadvertidos | [`DBCompare/ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) | ⏳ Pendiente |
| M8 | Placeholder de contraseña hardcodeado en el script generado para `APPLICATION ROLE` (`WITH PASSWORD='fsdjfhkfasjhfkhjhklsaf465'`) | [`ObjectHelper/DBObjectType/Principal.cs`](../ObjectHelper/DBObjectType/Principal.cs) línea 41 | ⏳ Pendiente — documentar claramente que debe reemplazarse antes de ejecutar el script |
| M9 | `Table.Script()` (overload sin parámetros) fuerza `ServerMajorVersion = 9`, generando DDL con reglas de una versión obsoleta si se invoca sin pasar `ScriptingOptions` explícito | [`ObjectHelper/DBObjectType/Table.cs`](../ObjectHelper/DBObjectType/Table.cs) líneas 79-88 | ⏳ Pendiente |
| M10 | UI bloqueada durante operaciones largas y síncronas (fetch de esquema completo en `Login`/SMO, `CompareData` en `DataCompare`) | [`DBCompare/Login.cs`](../DBCompare/Login.cs), [`DBCompare/DataCompare.cs`](../DBCompare/DataCompare.cs) | ⏳ Pendiente |

## Bajos

| # | Hallazgo | Ubicación | Estado |
|---|---|---|---|
| B1 | Código muerto: prototipo `Form1` con credenciales embebidas, no referenciado en el `.csproj` | `DBCompare/Form1.cs`, `Form1.Designer.cs`, `Form1.resx` | ✅ Corregido — eliminado |
| B2 | `ObjectToByteArray` (BinaryFormatter) nunca invocado | [`ObjectHelper/ObjectDB.cs`](../ObjectHelper/ObjectDB.cs) | ⏳ Pendiente (relacionado con C6) |
| B3 | `DatabaseConnect.EjecuteScalar()` no referenciado fuera de la propia clase | [`DBCompare/Database.cs`](../DBCompare/Database.cs) | ⏳ Pendiente |
| B4 | `StreamReader` sin `using` al leer cada script embebido | [`ObjectHelper/ScriptGenerator.cs`](../ObjectHelper/ScriptGenerator.cs) líneas ~317-318 | ⏳ Pendiente |
| B5 | Bloques extensos de código comentado | [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs), [`ObjectFetch.cs`](../DBCompare/ObjectFetch.cs) | ⏳ Pendiente |
| B6 | Duplicación: `RefreshDatabaseList`/`RefreshServerList` repetidos entre `Login.cs` y `DBDocumentation/Main.cs` | ambos archivos | ⏳ Pendiente |
| B7 | `File.WriteAllText` sin encoding explícito (depende del ANSI del sistema) | [`DataCompare.cs`](../DBCompare/DataCompare.cs), [`ObjectCompare.cs`](../DBCompare/ObjectCompare.cs) | ⏳ Pendiente |
| B8 | 128+ scripts SQL versionados (`_7` a `_17`) sin pruebas de paridad entre versiones consecutivas | [`ObjectHelper/SQL/`](../ObjectHelper/SQL/) | ⏳ Pendiente |

## Correcciones aplicadas en esta revisión

1. **Eliminación de credenciales hardcodeadas** (C1, C2, C3):
   - [`DBCompare/Utilidades.cs`](../DBCompare/Utilidades.cs): los valores por defecto de `Setting.ini` (servidor, base de datos, usuario, password de `SetupDB1`/`SetupDB2`) quedan vacíos; las rutas de Beyond Compare/WinMerge/caché usan valores neutros.
   - [`DBDocumentation/Main.Designer.cs`](../DBDocumentation/Main.Designer.cs): se vaciaron los valores precargados de servidor, base de datos, usuario y contraseña.
   - Se eliminó `DBCompare/Form1.cs`, `Form1.Designer.cs` y `Form1.resx` (prototipo abandonado, no compilado, con credenciales embebidas).
   - Se añadió `Setting.ini` al [`.gitignore`](../.gitignore) para evitar que credenciales generadas localmente se suban al repositorio.
2. **Alineación de SMO** (A1, A2):
   - `DBCompare` ahora referencia `Microsoft.SqlServer.SqlManagementObjects 181.12.0`, igual que `ObjectHelper` y `DBDocumentation`.
   - Los `bindingRedirect` de `Microsoft.SqlServer.Smo` y `Microsoft.SqlServer.Management.Sdk.Sfc` en `DBCompare/app.config` ahora apuntan a `18.100.0.0`.

## Roadmap de mejoras (fuera del alcance de esta revisión)

### Seguridad
- Cifrar contraseñas en `Setting.ini` y en los proyectos XML (`ProtectedData.Protect` con `DataProtectionScope.CurrentUser`), o eliminar su persistencia y solicitar re-login.
- Parametrizar/validar el SQL dinámico de `DataCompare` (C5).
- Añadir `Encrypt=true` / `TrustServerCertificate` explícitos en los connection strings.

### Arquitectura
- Extraer la lógica de comparación (`CompareObjects`, `QueryDatos`) de los formularios a servicios independientes de la UI, testeables.
- Clonar `ScriptingOptions` por servidor antes de cada fetch (A5).
- Implementar *fallback* de versión de script SQL (A4) y evaluar consolidar los 128+ scripts versionados.

### Calidad y pruebas
- Crear un proyecto de tests (xUnit/NUnit) cubriendo `ScriptGenerator`, hidratación de `ObjectDB` y normalización de comparación.
- Sustituir los `catch` silenciosos por logging estructurado (M1).
- Configurar CI (build + tests) en GitHub Actions.

### Funcionalidad incompleta
- Completar `GenerateObjectDocumentationFile` (M3).
- Implementar `CheckColumnsByTable` (M2).
- Generalizar `DataCompare` para listar tablas dinámicamente (M4).

### Modernización (largo plazo)
- Migrar de .NET Framework 4.8 a .NET 8 (WinForms moderno).
- Sustituir acceso por `DataTable`/`DataView` por un micro-ORM (Dapper).
- Reemplazar INI por `appsettings.json` + `IConfiguration`.
