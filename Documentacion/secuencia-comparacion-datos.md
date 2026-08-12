# Secuencia — Comparación de datos

Flujo independiente del de objetos: compara el contenido (filas) de tablas seleccionadas entre dos bases de datos.

## Diagrama de secuencia

```mermaid
sequenceDiagram
    actor U as Usuario
    participant MDI as MDIMain
    participant DC as DataCompare
    participant INI as Setting.ini
    participant DBC as DatabaseConnect
    participant DB1 as SQL Server DB1
    participant DB2 as SQL Server DB2
    participant FS as Sistema de archivos
    participant EXT as Beyond Compare / WinMerge

    U->>MDI: Abrir comparacion de datos
    MDI->>DC: Abrir DataCompare hijo MDI
    DC->>INI: LoadData - leer lista de tablas (DataCompare.Tablas)
    DC->>U: Mostrar lista de tablas configuradas

    U->>DC: Marcar tablas a comparar
    U->>DC: Click Comparar (CompareData)

    DC->>DC: QueryDatos(tablasSeleccionadas)
    Note over DC: Construye SQL dinamico con tablas temporales<br/>#TABLAS #Data #Result y EXEC(@Query)

    DC->>DBC: Datos(1, sql)
    DBC->>INI: Leer SetupDB1 (server, usuario, password)
    DBC->>DB1: Ejecutar SQL dinamico
    DB1-->>DBC: Filas concatenadas por tabla
    DBC-->>DC: List<Data>

    DC->>FS: Escribir "Data [tabla].txt" en carpeta DB1

    DC->>DBC: Datos(2, sql)
    DBC->>INI: Leer SetupDB2 (server, usuario, password)
    DBC->>DB2: Ejecutar SQL dinamico
    DB2-->>DBC: Filas concatenadas por tabla
    DBC-->>DC: List<Data>

    DC->>FS: Escribir "Data [tabla].txt" en carpeta DB2

    DC->>EXT: Process.Start sobre carpeta DB1 vs carpeta DB2
    EXT->>U: Diff visual de los archivos exportados
```

## Notas de implementación

- Este flujo es **independiente** del de comparación de objetos: usa su propia conexión (`DatabaseConnect`, basada en `Setting.ini`) en lugar de `ObjectDB`/SMO.
- El SQL de comparación se construye concatenando el listado de tablas seleccionadas directamente en la cláusula `WHERE [FullName] IN (###)` (ver `QueryDatos()` en [`DataCompare.cs`](../DBCompare/DataCompare.cs)), sin parametrizar ni escapar comillas.
- La query final se ejecuta de forma dinámica con `EXEC (@Query)` dentro de un cursor T-SQL.
- Todo el proceso (`CompareData()`) se ejecuta de forma síncrona en el hilo de la UI: consultas a ambos servidores y escritura de archivos bloquean la interfaz durante el tiempo que tome el fetch.
- El comparador externo se invoca sobre **directorios completos** (uno por base de datos), no como un diff inline dentro de la aplicación.
- Existe un script `SQL_DATA_COMPARE.sql` embebido en `ObjectHelper` con una lógica equivalente que **no se usa**; esta duplicación implica mantener la misma lógica en dos lugares.

Ver hallazgos relacionados en [bugs-y-mejoras.md](bugs-y-mejoras.md) (sección "Comparación de datos").
