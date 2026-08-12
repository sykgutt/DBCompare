# Secuencia — Comparación de esquema (objetos)

Flujo principal de la aplicación: comparar la estructura (DDL) de dos bases de datos.

## Diagrama de secuencia

```mermaid
sequenceDiagram
    actor U as Usuario
    participant MDI as MDIMain
    participant L as Login
    participant OF as ObjectFetch
    participant ODB as ObjectDB
    participant SG as ScriptGenerator
    participant DB1 as SQL Server DB1
    participant DB2 as SQL Server DB2
    participant OC as ObjectCompare
    participant SV as ScriptView
    participant EXT as Beyond Compare / WinMerge

    U->>MDI: Nueva comparacion de objetos
    MDI->>OC: Abrir ObjectCompare hijo MDI
    OC->>L: Mostrar Login (ObjectCompare_Shown)
    U->>L: Servidor1, BD1, credenciales
    U->>L: Servidor2, BD2, credenciales
    U->>L: Seleccionar ScriptingOptions
    L->>OF: Crear ObjectFetch con ambos servidores + opciones

    OF->>OF: Task DB1 (Script)
    OF->>ODB: FetchObjects(so)
    ODB->>SG: GenerateScript(so)
    SG-->>ODB: SQL batch + mapa de ResultSets
    ODB->>DB1: Ejecutar batch (SqlDataAdapter.Fill)
    DB1-->>ODB: DataSet multi-resultset
    ODB->>ODB: Metodos Get* hidratan DBObjectType
    ODB-->>OF: Objetos + Script() por objeto (evento ObjectFetched)

    OF->>OF: ContinueWith - Task DB2 (Script)
    OF->>ODB: FetchObjects(so) [misma instancia so]
    ODB->>SG: GenerateScript(so)
    SG-->>ODB: SQL batch
    ODB->>DB2: Ejecutar batch
    DB2-->>ODB: DataSet multi-resultset
    ODB-->>OF: Objetos + Script() por objeto

    OF->>OF: ContinueWith - CompareObjects()
    OF->>OF: Clasificar por Schema+Name en 4 buckets
    Note over OF: 1=solo DB1, 2=solo DB2,<br/>3=iguales (sin espacios), 4=diferentes
    OF-->>OC: DataTable con columna ResultSet

    OC->>U: Lista agrupada de diferencias (ListView)
    U->>OC: Seleccionar objeto
    OC->>OC: showCode() - diff inline con DiffPlex
    OC->>U: Resaltado verde/rosa/gris

    alt Diff con herramienta externa
        U->>OC: Doble clic en objeto
        OC->>EXT: Abrir Beyond Compare / WinMerge con .txt temporales
    end

    alt Generar script de sincronizacion
        U->>OC: Marcar objetos + boton DB1 a DB2 (o inverso)
        OC->>OC: Concatenar DROP+CREATE segun tipo
        OC->>SV: Mostrar script generado
        SV->>U: DDL de sincronizacion
    end

    alt Guardar proyecto
        U->>MDI: Guardar como
        MDI->>MDI: Serializar servidores+credenciales+objetos a XML
    end
```

## Notas de implementación

- El fetch de DB1 y DB2 se ejecuta como una cadena de `Task.Factory.StartNew().ContinueWith(...).ContinueWith(...)` en [`ObjectFetch.cs`](../DBCompare/ObjectFetch.cs); no hay `TaskScheduler.FromCurrentSynchronizationContext()` explícito ni observación centralizada de excepciones no controladas.
- La misma instancia de `ScriptingOptions` (`so`) se reutiliza para ambos fetches. Si el primer servidor es de una versión antigua de SQL Server y deshabilita un flag (p. ej. `DataCompression`), esa mutación persiste para el segundo fetch aunque el segundo servidor sea más reciente.
- La clasificación en los 4 buckets compara únicamente por `Schema + Name` (ver `CompareObjects()` en `ObjectFetch.cs`), sin considerar el `Type`. Dos objetos de tipos distintos con el mismo esquema y nombre se tratarían como el mismo objeto.
- La igualdad entre definiciones ("bucket 3") se determina normalizando espacios en blanco (`RemoveWhiteSpaces`), no por comparación semántica del DDL.
- El guardado de proyecto en XML persiste las contraseñas de conexión en texto plano junto con la lista de objetos comparados.

Ver hallazgos relacionados en [bugs-y-mejoras.md](bugs-y-mejoras.md) (sección "Comparación de objetos").
