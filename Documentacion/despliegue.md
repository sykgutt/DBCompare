# Despliegue — copiar `bin` a cualquier PC Windows

DBCompare **no requiere SSMS** ni SQL Server instalado en la máquina cliente. Las librerías SMO salen del paquete NuGet `Microsoft.SqlServer.SqlManagementObjects 181.12.0` y se copian junto al ejecutable.

## Requisitos de la máquina destino

| Requisito | Obligatorio |
|---|---|
| Windows (x64 o x86) | Sí |
| .NET Framework **4.8** | Sí |
| SSMS | No |
| SQL Server local | No (solo hace falta red hasta los servidores a comparar) |
| Beyond Compare o WinMerge | Solo si se usa el diff externo |

El ejecutable se compila como **x86**. En Windows de 64 bits corre bajo WOW64.

## Cómo publicar

1. Compilar `DBCompare` en Visual Studio (configuración `Release|x86`, o `Debug|x86` para pruebas).
2. Copiar **toda** la carpeta de salida, por ejemplo:
   - `DBCompare\bin\x86\Release\`
   - o `DBCompare\bin\Debug\` según cómo esté configurado el proyecto
3. Pegar esa carpeta en la PC destino y ejecutar `DBCompare.exe`.

No hace falta copiar el código fuente ni la carpeta `packages\`. Sí hace falta el `.exe.config` (es el `app.config` compilado): sin él el CLR puede cargar SMO desde el GAC de SSMS y volver el error de versiones.

## Checklist de archivos en la carpeta copiada

Debe existir, como mínimo:

- `DBCompare.exe`
- `DBCompare.exe.config` (binding redirects + `publisherPolicy apply="no"` para SMO)
- `ObjectHelper.dll`
- `Microsoft.SqlServer.Smo.dll`
- `Microsoft.SqlServer.SmoExtended.dll`
- `Microsoft.SqlServer.Management.Sdk.Sfc.dll`
- `Microsoft.SqlServer.ConnectionInfo.dll`
- `Microsoft.SqlServer.SqlEnum.dll`
- `Microsoft.SqlServer.SqlClrProvider.dll`
- `Microsoft.SqlServer.SqlWmiManagement.dll`
- `Microsoft.Data.SqlClient.dll`
- `Microsoft.Data.SqlClient.SNI.x86.dll` (nativo; el exe es x86)
- `DiffPlex.dll`
- `DifferenceEngine.dll`

Si falta `SNI.x86.dll`, las conexiones vía `Microsoft.Data.SqlClient` (dependencia de SMO) fallan aunque el código propio use `System.Data.SqlClient`.

## Independencia de SSMS

El problema histórico era cargar SMO desde el GAC que instala SSMS (versiones 17/18/19 distintas → `FileLoadException`).

Mitigación actual:

- Referencias NuGet con `HintPath` local, `Private=True` (Copy Local) y `SpecificVersion=True`.
- `DBCompare.exe.config` y `DBDocumentation.exe.config` declaran `<publisherPolicy apply="no"/>` y `bindingRedirect` a `18.100.0.0` para los assemblies SMO usados.
- El listado de servidores (`SmoApplication.EnumAvailableSqlServers`) usa WMI/red; si el firewall bloquea WMI la lista puede salir vacía, pero el usuario puede escribir el nombre del servidor a mano.

## DBDocumentation

Mismo modelo: copiar `DBDocumentation\bin\...` completo, incluido `DBDocumentation.exe.config`. Tampoco necesita SSMS.
