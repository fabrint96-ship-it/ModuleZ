# ModuleZ — Roadmap Oficial de Desarrollo v2.0

- **Aprobado por el Project Owner:** 2026-10-14
- **Rama de integración:** `develop`
- **Source of Truth:** GitHub (`fabrint96-ship-it/ModuleZ`)

## Propósito y alcance

Este roadmap ordena la evolución de ModuleZ en dos líneas coordinadas: **Engineering Foundation** (arquitectura, seguridad de datos, validación y capacidad de entrega) y **Game Development** (experiencia jugable, contenido y presentación). Cada Episode debe producir entregables funcionales y técnicos verificables. La primera gran meta de gameplay es un **vertical slice jugable** en EP-03.

La tabla expresa dirección y estado, **no autoriza por sí misma** la implementación de Episodes futuros, cambios de arquitectura, ejecución Unity, releases ni merges. El alcance y los criterios de aceptación se definirán en slices pequeños, con pocas tareas Codex y límites explícitos. Las pruebas y validaciones acompañan a **cada** Episode; EP-10 no es el primer momento de hacer QA.

## Episodes

| Episode | Nombre | Estado | Resultado orientativo a demostrar, sujeto a definición posterior |
| --- | --- | --- | --- |
| EP-01 | Legacy Recovery & Runtime Stabilization | **OPEN** | Mantener verificables los invariantes de runtime recuperados y resolver obligaciones Foundation aún abiertas. |
| EP-02 | Application State & Session Lifecycle | **IN PROGRESS** | Aclarar ownership y ciclo de vida de estado/sesión sin perder compatibilidad ni seguridad de persistencia. |
| EP-03 | Playable Vertical Slice | **PLANNED** | Demostrar un recorrido jugable representativo de extremo a extremo, con evidencia funcional y técnica. |
| EP-04 | Open World & Exploration | **PLANNED** | Evolucionar exploración y mundo abierto con contratos de escena y regresión comprobables. |
| EP-05 | Duel Gameplay & Rival AI | **PLANNED** | Evolucionar duelos y rivales con comportamiento jugable y protección de reglas verificables. |
| EP-06 | Characters & Procedural Animation | **PLANNED** | Evolucionar personajes y animación con comprobaciones de integración y comportamiento. |
| EP-07 | UI, HUD & User Experience | **PLANNED** | Mejorar interfaces y flujos de usuario con aceptación funcional y visual apropiada. |
| EP-08 | Visual Identity, Audio & VFX | **PLANNED** | Establecer una presentación audiovisual coherente y validar su integración en runtime. |
| EP-09 | Progression, Content & Replayability | **PLANNED** | Ampliar progresión y contenido conservando contratos de guardado y resultados. |
| EP-10 | Performance, QA & Release Engineering | **PLANNED** | Consolidar rendimiento, calidad y preparación de release; no sustituye las pruebas de Episodes anteriores. |
| EP-11 | Steamworks & Public Release | **PLANNED** | Preparar integración de plataforma y publicación pública solo tras autorización y verificaciones específicas. |

Las descripciones son hitos orientativos, no diseños aprobados ni criterios de cierre completos. Las dos líneas se cruzan: una capacidad de juego necesita validación técnica y una mejora Foundation debe proteger un resultado de producto. Se priorizan slices acotados, reversibles cuando sea posible y trazables a un PR.

## Estado dentro de EP-02

| Slice | Nombre / alcance | Estado |
| --- | --- | --- |
| EP-02-01 | Application State Contracts & Ownership Baseline | **CLOSED** |
| EP-02-02 | Duel Transition Rollback & Session Lifecycle | **CLOSED — SCOPE LIMITED** |
| EP-02-03 | Application State Ownership & Persistence | **NEXT PLANNING** |
| EP-02-04 | Application State Integration & Hardening | **PROPOSED** |

EP-02-02 quedó cerrado únicamente dentro de su alcance delimitado por la [autorización final del Project Owner](recovery/EP-02-02_CLOSURE.md). Esto no completa EP-02 ni la migración integral de Application State. EP-02-03 y EP-02-04 requieren definición, revisión y autorización propias antes de implementarse.

## Obligaciones abiertas y lectura correcta de la evidencia

- **REC-008:** PARTIALLY MIGRATED / STABILIZED. Persisten estado y consumidores Legacy; no se declara migración completa.
- **REC-007:** IN PROGRESS. **F1.6:** OPEN. **EP-01:** OPEN.
- **Smoke canónico en la verificación de EP-02-02:** NOT RUN; las pruebas dirigidas no equivalen al smoke completo.
- **Independent Unity CI:** BLOCKED por requisitos pendientes de licencia/seguridad. El check estático de GitHub y la validación Unity local no sustituyen un CI Unity independiente.

Los estados de esta sección no cambian criterios de cierre previos ni reinterpretan resultados históricos. El [protocolo transitorio de integración](workflows/TRANSITIONAL_INTEGRATION_VALIDATION_PROTOCOL.md) sigue vigente mientras corresponda.

## Gobierno y modo de entrega

GitHub conserva commits, PR, checks y decisiones integradas como Source of Truth; `develop` es la rama de integración. [ChatGPT](workflows/AI_ASSISTED_DEVELOPMENT_WORKFLOW.md) actúa como **Architect / Director / Reviewer**; Codex, como **Implementer**. El **Project Owner** mantiene la autoridad final para merges, ejecuciones Unity, limpieza de ramas/worktrees/evidencias y cierre de slices o Episodes.

Cada slice sigue las puertas **DEFINE → IMPLEMENT → VERIFY → REVIEW → AUTHORIZE → MERGE** bajo el workflow canónico, que también contempla sincronización, limpieza, registro y cierre después del merge. Una aprobación del roadmap no reemplaza esas autorizaciones. Se registran por separado alcance, pruebas automatizadas, validación manual, riesgos y evidencia de perfiles cuando se ejecute Unity. `PASS`, `NOT RUN` y `BLOCKED` conservan significados distintos.

**Release**, **deployment** y **healthy production** son estados diferentes: disponer de una versión candidata no prueba su despliegue; desplegarla no prueba por sí solo salud operativa ni aceptación del producto. Cada transición requiere criterios y evidencia propios, definidos y aprobados cuando corresponda. Ningún estado `PLANNED` de esta tabla implica que esas transiciones hayan ocurrido.
