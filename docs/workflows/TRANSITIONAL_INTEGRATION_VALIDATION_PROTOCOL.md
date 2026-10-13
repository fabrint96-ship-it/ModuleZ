# Protocolo transitorio de integración y validación de ModuleZ

## 1. Propósito, alcance y vigencia

Este protocolo permite evaluar PR dirigidos a `develop` mientras no existe un CI Unity independiente y obligatorio. GitHub es el Source of Truth para commits, diffs, PR y checks. El Project Owner conserva la autorización final y realiza manualmente los merges.

Se aplica a los cambios posteriores a la recuperación F1.5, sin declarar resuelta la protección de regresión pendiente. REC-007, F1.6 y EP-01 permanecen abiertos. La validación Unity local, aunque sea reproducible y esté documentada, **no constituye CI independiente**.

La vigencia termina únicamente mediante una decisión documentada del Project Owner, después de demostrar las condiciones de sustitución de la sección 13. Hasta entonces, cada PR debe identificar su categoría de riesgo y conservar las evidencias exigidas aquí.

## 2. Relación con el workflow canónico

Este documento complementa [AI_ASSISTED_DEVELOPMENT_WORKFLOW.md](AI_ASSISTED_DEVELOPMENT_WORKFLOW.md); no lo sustituye ni modifica sus roles, revisiones o autoridad de merge. Tampoco altera los nueve criterios originales de cierre de REC-007 definidos en [F1.6_REGRESSION_RUNTIME_HEALTH_CHARACTERIZATION.md](../recovery/F1.6_REGRESSION_RUNTIME_HEALTH_CHARACTERIZATION.md), en particular el criterio 7: CI independiente que ejecute el subconjunto Unity obligatorio en PR y `develop` con versión fijada y artefactos.

El check de GitHub Actions `Static architecture and wrapper guards` es obligatorio para PR a `develop`. Acredita validación estática y guardias del wrapper, no compilación ni ejecución Unity. Su éxito no debe presentarse como cumplimiento del criterio 7. Una excepción de integración autorizada para un PR tampoco modifica los criterios de REC-007 ni permite cerrar F1.6 o EP-01.

## 3. Clasificación de riesgos de PR

La clasificación se decide a partir del diff y de los contratos afectados **antes** de elegir pruebas; un PR mixto adopta la categoría más alta. Si el alcance cambia, se reclasifica y se actualiza la evidencia requerida.

| Categoría | Alcance típico | Riesgo principal |
| --- | --- | --- |
| D0 — documental | Texto o enlaces sin cambiar instrucciones ejecutables, contratos ni criterios de validación | Afirmaciones incorrectas o trazabilidad incompleta |
| D1 — validadores/tests | Tests, asmdefs de test, wrapper, filtros, ratchets o workflow estático | Falso PASS, cobertura omitida o aislamiento debilitado |
| D2 — funcional acotado | Cambio de producción limitado a un contrato o comportamiento identificado | Regresión en escena, transición, resultado o persistencia |
| D3 — estructural de alto impacto | Propiedad de SceneRoot/cámara/audio, autoridad de contexto/resultado, persistencia, estado compartido, escenas, paquetes, ProjectSettings o gates | Pérdida de invariantes recuperados o validación insegura |

Un cambio documental que altere instrucciones operativas de validación no es D0. Un cambio a guardias de persistencia o ejecución de Unity se trata como D3 aunque esté ubicado en `Tools/Validation/`.

## 4. Matriz de validaciones obligatorias

En **todas** las categorías son obligatorios el check estático de GitHub Actions, `git diff --check`, el examen del alcance y la revisión independiente del PR. La columna Unity indica evidencia adicional exigida para proponer integración; ninguna ejecución Unity se autoriza por el mero hecho de estar en esta tabla.

| Categoría | Validación local y Unity requerida | XML/log y aceptación manual |
| --- | --- | --- |
| D0 | Revisar enlaces, hechos, SHA y contradicciones. Unity: `NOT RUN`, justificado como cambio solo documental. | No se inventa XML Unity. ChatGPT revisa el texto; el Owner decide el merge. |
| D1 | Sintaxis PowerShell y guardias de invocación cuando proceda; validar filtrado y propagación de fallos. Si cambian tests, asmdefs o código ejecutado por Unity: `compile` y las suites `editmode`/`playmode` afectadas. Si cambia aislamiento, guardia de perfiles o salida: además `persistence` y `shutdown`. | XML y log separados para cada suite ejecutada, más exit code y guardia posterior al cierre; revisión del Owner para cambios de seguridad o de contrato de validación. |
| D2 | `static`, `compile`, `editmode`, `playmode` ordinario y tests dirigidos al contrato afectado. Añadir `persistence`/`shutdown` si se toca escritura, lectura, arranque o cierre. Smoke solo si el flujo de extremo a extremo afectado lo requiere y existe autorización independiente. | XML, logs, exit codes y recuentos verificables; aceptación manual del Owner para comportamiento visible o gameplay afectado. |
| D3 | Diferir por defecto. Para una excepción de alcance explícitamente aprobada: `static`, `compile`, `editmode`, `playmode`, suites dirigidas a todos los invariantes afectados, `persistence`/`shutdown` si corresponde y smoke autorizado cuando el flujo recuperado pueda cambiar. | Evidencia de checkout limpio, XML/log, guardia de perfiles, revisión arquitectónica y validación manual del Owner. La ausencia de evidencia de seguridad impide integrar. |

El Project Owner autoriza cada ejecución Unity y fija el SHA, alcance y etapas. Un requisito no ejecutado no se convierte en PASS por aprobación verbal. Si una validación prevista no es segura o no puede realizarse, se registra `BLOCKED`, se detiene la propuesta de merge y se decide expresamente si se reduce el alcance, se difiere el PR o se autoriza una excepción **solo** para un control no crítico. Nunca se exceptúan el check estático obligatorio, el aislamiento ni la integridad de perfiles.

## 5. Estados de evidencia

Cada control se registra individualmente con uno de estos cuatro estados y una razón:

- `PASS`: ejecutado sobre el SHA declarado y verificado con exit code, artefactos y condiciones de aceptación correspondientes.
- `FAIL`: ejecutado, pero incumplió una condición; no se reetiqueta por un resultado parcial favorable.
- `BLOCKED`: un prerrequisito técnico, de seguridad o de autorización impide ejecutarlo o obtener evidencia concluyente.
- `NOT RUN`: no se ejecutó; indicar si no aplica al diff, está pendiente de autorización o se excluyó deliberadamente.

Una validación no ejecutada no puede registrarse como PASS. Evidencias de otro commit o de una ejecución previa son contexto histórico, no el PASS del PR actual. Un XML verde sin exit code válido, log o verificación exigida de perfiles no acredita PASS.

## 6. Autorización de ejecuciones Unity

La autorización del Project Owner debe preceder a cada secuencia Unity e identificar checkout/SHA, versión de Editor, etapas permitidas, límite de repeticiones y condiciones de parada. Se prefiere un checkout limpio y desechable para validaciones de integración. No se ejecuta Unity en paralelo con otra validación incompatible. Smoke necesita autorización **independiente** y el opt-in explícito `smoke -EnableSmoke`; no queda autorizado por `playmode`, `tests`, `all` ni por una aprobación genérica de Unity.

Si una etapa falla, faltan artefactos o cambia un perfil real, detener la secuencia; no repetir automáticamente ni restaurar perfiles para producir un PASS aparente. La validación manual de gameplay por el Owner se registra por separado de los resultados automatizados.

## 7. Wrapper aislado obligatorio

Toda validación Unity debe utilizar [Invoke-ModuleZValidation.ps1](../../Tools/Validation/Invoke-ModuleZValidation.ps1), no una invocación directa de `Unity.exe`. El wrapper establece una raíz de persistencia aislada por proceso, comprueba rutas y ancestros reparse, sigue el proceso Unity hasta su salida, preserva exit codes y verifica externamente los perfiles reales. No se omiten esas guardias para facilitar un merge.

Las etapas `tests` y `all` **no incluyen** `shutdown` ni smoke. `tests` ejecuta EditMode y PlayMode ordinario; `all` añade `static` y `compile`. `persistence` es un subconjunto dirigido de PlayMode, no cobertura adicional independiente. `shutdown` es una etapa separada. Smoke está excluido de PlayMode ordinario y requiere `-EnableSmoke`.

## 8. Integridad de perfiles reales

Antes de lanzar Unity se registra, para `modulez_save.json` y `module_z_settings.json` del perfil real, existencia, tamaño, fecha de escritura UTC y SHA-256 si existen. La raíz aislada debe ser única, ajena al perfil real, no solapada y libre de ancestros reparse ambiguos. Si no puede establecerse esa separación, no se lanza Unity.

Después de la **salida completa del proceso Unity**, el wrapper compara de nuevo ambos perfiles y falla ante cualquier diferencia, incluso si el XML de tests indica éxito. El PR conserva el resultado de la guardia externa posterior al cierre junto con el exit code y los artefactos, sin publicar contenido ni rutas personales de los perfiles. No se hace backup/restauración de perfiles reales como sustituto del aislamiento. Una discrepancia bloquea el merge y se conserva la evidencia sin alterar los archivos afectados.

## 9. Condiciones de bloqueo del merge

No proponer `READY TO MERGE` cuando ocurra cualquiera de estos casos:

- Base o SHA incorrectos, diff fuera de alcance o cambios tracked inesperados.
- Check estático requerido ausente, pendiente o fallido.
- Validación obligatoria `FAIL` o `BLOCKED`, o `NOT RUN` sin justificación aceptable para la categoría y el alcance aprobados.
- Exit code no satisfactorio, XML/log ausente o incongruente, cero casos esperados descubiertos, caso fallido/ignorado inesperadamente, error Unity relevante o advertencia no explicada.
- Aislamiento no demostrable, guardia posterior al cierre ausente/fallida o perfil real modificado.
- Smoke necesario para el alcance pero sin autorización independiente o sin resultado concluyente.
- Revisión independiente `CHANGES REQUIRED`, aceptación manual requerida pendiente o autorización de merge del Owner ausente.

La falta de CI Unity independiente se declara como limitación abierta en cada PR pertinente; no se oculta tras el check estático ni tras una ejecución local. Una excepción documentada puede resolver una decisión de integración concreta cuando el riesgo sea aceptable, pero **no** altera ni satisface por sí misma el criterio 7 de REC-007.

## 10. Plantilla mínima de evidencias por PR

```text
Base SHA / Head SHA / PR URL:
Archivos cambiados y categoría de riesgo; contratos afectados:
Validaciones exigidas y motivo de cualquier exclusión:
GitHub Static architecture and wrapper guards: PASS | FAIL | BLOCKED | NOT RUN; enlace:
Validación local por etapa: estado; comando; SHA/checkout; Unity versión; exit code:
XML por suite: ruta/artefacto; descubiertos; aprobados; fallidos; ignorados:
Log por etapa: ruta/artefacto; errores y advertencias relevantes:
Perfiles reales: guardia externa posterior al cierre PASS | FAIL | BLOCKED | NOT RUN:
Smoke: autorización independiente; estado; XML/log; o motivo de NOT RUN:
Validación manual del Project Owner: alcance y estado:
Revisión ChatGPT: READY FOR INTEGRATION | CHANGES REQUIRED; observaciones:
CI Unity independiente: BLOCKED; motivo y riesgo residual:
Desviaciones, excepciones aprobadas y controles que no sustituyen:
Decisión final de merge del Project Owner:
```

Los artefactos generados permanecen fuera de `Assets` y no se incorporan al repositorio. El PR debe enlazar evidencia verificable, no solo afirmar que las pruebas pasaron.

## 11. Responsabilidades y autoridad

- **Codex:** clasifica provisionalmente el riesgo, implementa dentro del alcance autorizado, ejecuta solo validaciones autorizadas, presenta artefactos y limitaciones sin convertir `NOT RUN` en PASS, prepara PR y no hace merge.
- **ChatGPT:** revisa independientemente en GitHub diff, arquitectura, categoría, pruebas, checks, evidencias y desviaciones; indica `CHANGES REQUIRED` o `READY FOR INTEGRATION`. No concede por sí solo el merge.
- **Project Owner:** aprueba alcance y ejecuciones Unity/smoke, realiza validación manual cuando corresponde, acepta o rechaza riesgos residuales permitidos y efectúa el merge manual.
- **GitHub Actions:** ejecuta el gate estático obligatorio sobre el PR y conserva su resultado/artefactos. Actualmente no ejecuta Unity ni reemplaza la aceptación manual o el criterio 7 de REC-007.

## 12. Riesgos residuales

El CI Unity independiente continúa bloqueado; una ejecución Unity local depende de autorización, entorno y custodia de artefactos. La validación estática no demuestra comportamiento runtime. El aislamiento de rutas reduce, pero no elimina completamente, riesgos de carrera de sistema de archivos (TOCTOU); por eso se exige comprobación externa tras el cierre y parada ante ambigüedad. Smoke no cubre todos los resultados de gameplay ni sustituye revisión visual/manual. Ningún PR debe representar estos límites como cobertura ya obtenida.

## 13. Sustitución del protocolo

El protocolo podrá retirarse o revisarse únicamente cuando un CI Unity independiente, seguro y aprobado ejecute el subconjunto obligatorio en PR hacia `develop` **y** tras push a `develop`, con versión de Editor fijada, aislamiento de persistencia, exit codes, XML/log y artefactos verificables, y cuando el Project Owner apruebe formalmente la sustitución. Antes de ello deben demostrarse licenciamiento, frontera de confianza, protección del gate y comportamiento fail-closed, sin exponer credenciales al código de PR no confiable.

La sustitución no es automática ni implica por sí sola el cierre de REC-007: sus nueve criterios se evalúan individualmente con evidencia actual. Hasta esa decisión, REC-007, F1.6 y EP-01 siguen abiertos y este protocolo no modifica sus criterios de cierre.
