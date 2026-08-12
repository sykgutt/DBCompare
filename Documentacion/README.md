# Documentación DBCompare

Índice de la documentación técnica del proyecto **DBCompare** (solución `DBScripter.sln`).

## Contenido

| Documento | Descripción |
|---|---|
| [documento-tecnico.md](documento-tecnico.md) | Descripción técnica completa: stack, proyectos, clases principales, persistencia, dependencias |
| [arquitectura.md](arquitectura.md) | Diagrama de componentes y flujo de datos general (Mermaid) |
| [secuencia-comparacion-objetos.md](secuencia-comparacion-objetos.md) | Diagrama de secuencia: login → fetch → comparación de esquema → diff → script |
| [secuencia-comparacion-datos.md](secuencia-comparacion-datos.md) | Diagrama de secuencia: comparación de datos de tablas |
| [secuencia-documentacion.md](secuencia-documentacion.md) | Diagrama de secuencia: generación de documentación HTML (DBDocumentation) |
| [bugs-y-mejoras.md](bugs-y-mejoras.md) | Inventario de bugs por severidad y roadmap de mejoras |
| [analisis-proyecto.md](analisis-proyecto.md) | Análisis previo (julio 2026); ver nota de vigencia al inicio del archivo |

## Cómo leer esta documentación

1. Empieza por [documento-tecnico.md](documento-tecnico.md) para entender qué hace la aplicación y cómo está construida.
2. Revisa [arquitectura.md](arquitectura.md) para la vista de componentes.
3. Los tres documentos de secuencia detallan los flujos de uso principales.
4. [bugs-y-mejoras.md](bugs-y-mejoras.md) es la referencia para priorizar trabajo futuro; incluye qué se corrigió ya y qué queda pendiente.

## Estado de las correcciones aplicadas

En esta revisión (agosto 2026) se corrigieron los hallazgos críticos de seguridad más urgentes (credenciales hardcodeadas y desalineación de SMO). El resto de hallazgos —incluidos otros críticos como las contraseñas persistidas en INI/XML y la inyección SQL en `DataCompare`— quedan documentados y priorizados en [bugs-y-mejoras.md](bugs-y-mejoras.md), pendientes de implementación.
