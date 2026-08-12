# Secuencia — Generación de documentación (DBDocumentation)

Aplicación secundaria que genera un índice HTML navegable del esquema de una base de datos.

## Diagrama de secuencia

```mermaid
sequenceDiagram
    actor U as Usuario
    participant M as DBDocumentation.Main
    participant ODB as ObjectDB
    participant SG as ScriptGenerator
    participant DB as SQL Server
    participant PG as PropertyGrid ScriptingOptions
    participant FS as Sistema de archivos
    participant HTML as Documentation.html

    U->>M: Ejecutar DBDocumentation
    M->>U: Formulario: servidor, BD, credenciales
    U->>PG: Seleccionar categorias a documentar
    U->>M: Click Generar

    M->>M: Task.Factory.StartNew
    M->>ODB: FetchObjects(so)
    ODB->>SG: GenerateScript(so)
    SG-->>ODB: SQL batch
    ODB->>DB: Ejecutar batch
    DB-->>ODB: DataSet multi-resultset
    ODB-->>M: Objetos hidratados (evento ObjectFetched)

    loop Por cada objeto
        M->>M: GenerateObjectDocumentationFile(obj)
        Note over M: Stub actual: solo obtiene obj.GetType(),<br/>no genera contenido de la pagina
    end

    M->>FS: Copiar assets CSS/imagenes desde Template/
    M->>HTML: Generar indice (TOC) colapsable con enlaces por objeto
    M->>U: Abrir/mostrar Documentation.html
    U->>HTML: Click en un objeto del indice
    HTML--xU: Enlace roto (pagina de detalle no existe)
```

## Notas de implementación

- El fetch reutiliza el mismo `ObjectDB`/`ScriptGenerator` que `DBCompare`, ejecutado en background con `Task.Factory.StartNew` y actualización de UI vía `Invoke` (sin comprobación consistente de `InvokeRequired`).
- `GenerateObjectDocumentationFile` (en [`Main.cs`](../DBDocumentation/Main.cs)) es un stub: obtiene el tipo del objeto pero no escribe ninguna página HTML de detalle.
- El resultado final es un índice (`toc.html`) cuyos enlaces apuntan a páginas de objeto que nunca se generan, por lo que la funcionalidad de documentación está incompleta en la práctica.
- El formulario `Main` de `DBDocumentation` tenía, antes de esta revisión, credenciales de ejemplo precargadas en el diseñador (ver corrección aplicada en [bugs-y-mejoras.md](bugs-y-mejoras.md)).

Ver hallazgos relacionados en [bugs-y-mejoras.md](bugs-y-mejoras.md) (sección "DBDocumentation").
