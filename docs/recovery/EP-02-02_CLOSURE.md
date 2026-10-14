# EP-02-02 — Propuesta de cierre pendiente de autorización del Project Owner

## Identificación y alcance

EP-02-02 estabiliza el rollback de la transición OpenWorld→Duel, el ciclo de vida de la sesión transitoria y las decisiones de New Game/Continue ante guardados ausentes, inválidos o no aplicables. También explicita el resultado de carga sin cambiar el esquema de persistencia. Este documento **prepara una decisión de cierre; no declara EP-02-02 CLOSED** ni autoriza un merge.

El baseline de esta propuesta es `develop`/`origin/develop` en `4d1966e287d38f730fb7a0a76bcc167cf8054873`. La implementación de [PR #35](https://github.com/fabrint96-ship-it/ModuleZ/pull/35) se validó en `f459de5993dc7fa9b804a3c1add5c08a2781362d` y se integró mediante `58690f483db2ae643b738859e3297ec68cb50182`. El [registro de integración y validación de PR #36](EP-02-02_INTEGRATION_VALIDATION_RECORD.md) se integró mediante `4d1966e287d38f730fb7a0a76bcc167cf8054873`. GitHub confirma ambos PR como MERGED. La [caracterización de implementación](EP-02-02_DUEL_TRANSITION_SESSION_LIFECYCLE.md) y el [baseline contractual EP-02-01](EP-02-01_APPLICATION_STATE_CONTRACTS_OWNERSHIP_BASELINE.md) conservan su contexto histórico; esta propuesta no los modifica retrospectivamente.

## Contratos entregados

- La ruta productiva de duelo conserva un `Attempt` de `DuelTransitionBridge` durante la carga diferida. Identidad y revisión de `ModuleZDuelSessionState` impiden que un intento obsoleto deshaga uno posterior. `Cancel` libera solo las nueve proyecciones Legacy que todavía conservan los valores publicados por su intento; una escritura ajena distinta se preserva. Si el rollback Legacy queda incompleto, se limpia la sesión poseída en lugar de restaurar una sesión previa ambigua.
- La sesión publica su payload antes de activarse y aumenta su revisión con `StartDuel` y `Clear`. El interactable cancela su intento pendiente al deshabilitarse o destruirse. La falta de `ModuleZSceneController` y una excepción síncrona de carga en la escena de origen admiten rollback acotado.
- `ModuleZSaveManager.TryLoadGame()` distingue `Success`, `NotFound`, `InvalidData`, `ReadError` y `ApplyError`; `LoadGame()` sigue como envoltorio de compatibilidad. La validación comprueba gramática JSON completa, propiedades obligatorias del objeto raíz, tipos conocidos y valores deserializados antes de aplicar el guardado. Ante fallo de aplicación se restauran los valores capturados de GameState y logros en memoria.
- New Game conserva la confirmación en dos pulsaciones: solo un borrado satisfactorio precede al reset, limpieza de sesión y navegación. Continue limpia una sesión residual y navega únicamente después de una carga satisfactoria; un fallo mantiene el flujo en el menú. `DuelContext` y `DuelResult` conservan su autoridad aceptada. No se cambian nombre de archivo, formato ni esquema del guardado.

Estos son los contratos del código integrado, no una afirmación de migración completa de `ModuleZGameState` ni de eliminación de los campos Legacy.

## Evidencia de validación

VERIFY-01 usó Unity `6000.3.13f1`, el wrapper oficial con aislamiento por proceso y un worktree detached en el SHA exacto de implementación. Las pruebas dirigidas de Persistence son un **subconjunto** de PlayMode, no cobertura independiente adicional.

| Control | Estado | Evidencia y límite |
| --- | --- | --- |
| GitHub static requerido de PR #35 | **PASS** | [Static architecture and wrapper guards](https://github.com/fabrint96-ship-it/ModuleZ/actions/runs/38550368760/job/115806567170). |
| Static local | **PASS con WARN** | 32 checks: 30 PASS, 2 WARN heurísticos (`STATIC-020`, `STATIC-021`), 0 FAIL. |
| Unity Compile | **PASS** | Exit 0; 0 errores de compilación. |
| Unity EditMode | **PASS** | Exit 0; 45/45 globales, de ellas 44/44 ModuleZ; 0 fallos/ignoradas. |
| Unity PlayMode | **PASS** | Exit 0; 28/28; 0 fallos/ignoradas. Incluye New Game, Continue y regresiones existentes. |
| Unity Persistence | **PASS** | Exit 0; 5/5 de PlayMode dirigidas. |
| Unity Shutdown | **PASS** | Exit 0; 1/1; autosave de salida solo en raíz aislada. |
| Smoke canónico | **NOT RUN** | No autorizado en VERIFY-01; no se infiere PASS. |
| CI Unity independiente | **BLOCKED** | Requisitos de licencia/seguridad pendientes; el check estático de GitHub no ejecuta Unity. |

Los XML y logs locales de VERIFY-01 se conservaron fuera del worktree de validación en `artifacts/validation/pr35-verify01/` del checkout principal. El [registro de integración](EP-02-02_INTEGRATION_VALIDATION_RECORD.md) fija las rutas y resultados detallados. Los logs incluyen advertencias C# sin error de compilación, un error transitorio de actualización del token seguido de resolución de Unity Personal y mensajes de cierre de hilos; las etapas terminaron con exit 0 y XML satisfactorio. No se ejecuta Unity de nuevo para esta propuesta D0.

En la preparación documental CLOSE-02 se volvió a ejecutar únicamente el validador estático de repositorio: **PASS**, 30 PASS / 2 WARN heurísticos / 0 FAIL, con resultados separados en `artifacts/validation/ep02-02-close02/`. Esto no es una nueva ejecución de Unity ni reemplaza VERIFY-01.

## Integridad de perfiles y CLEAN-01

Tras **cada** proceso Unity, la guardia externa y la comprobación independiente conservaron existencia, tamaño, fecha de modificación UTC y SHA-256 de los perfiles reales:

| Archivo | Tamaño | Fecha UTC | SHA-256 antes/después |
| --- | ---: | --- | --- |
| `modulez_save.json` | 3744 bytes | `2026-10-11T18:44:32.9357444Z` | `FAAD98E4BC84E89D79A31770A9921FC1A633782377B2DCF397B48AC2528F0B6B` |
| `module_z_settings.json` | 59 bytes | `2026-09-02T03:07:45.1036943Z` | `8522EB940EAC8C52A1AAA66122DF1410BC3A3AF6E20737DF033FFA4A8F076FCA` |

VERIFY-01 aplicó cuatro recuperaciones *expresamente autorizadas*, limitadas a `ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`, cuando HEAD, índice y blob normalizado del working tree coincidían y era el único tracked marcado. No constituyeron cambios de producción ni de ProjectSettings en PR #35.

CLEAN-01 figura **completado y verificado por el Project Owner**. En la comprobación previa a esta propuesta seguían registrados los worktrees detached `ModuleZ-EP02-02-PR35-f459de5-VERIFY01-20261014` y `ModuleZ-F16-E2E-372a4a3-20261013`, además de `refs/backup/s1-6-05b-pre-sync`. Esta propuesta no modifica ni elimina worktrees, respaldos, `.vscode/`, evidencias o perfiles. La comprobación de presencia no equivale a repetir CLEAN-01 ni a una nueva validación Unity.

## Límites y obligaciones abiertas

- El bridge no revierte un fallo de carga de escena comunicado de forma asíncrona después de que la invocación síncrona haya retornado.
- Sin revisión/propiedad por campo Legacy, una escritura ajena del *mismo valor* no se distingue de la publicación del intento.
- El snapshot de carga en memoria no deshace efectos externos de suscriptores arbitrarios de notificaciones de logros.
- No se ejecutó el Smoke canónico durante VERIFY-01. Sus pasos no deben etiquetarse PASS por los resultados PlayMode dirigidos.
- Unity CI independiente sigue BLOCKED. La validación local aislada y el gate estático no cumplen por sí solos el criterio 7 de REC-007.
- REC-008 sigue **PARTIALLY MIGRATED / STABILIZED**: persisten proyecciones y consumidores Legacy. REC-007, F1.6 y EP-01 siguen abiertos; este documento no altera sus criterios de cierre ni los resuelve.

## Disposición propuesta, no ejecutada

La implementación, revisión, integración y validación aislada de EP-02-02 tienen evidencia registrada. Se propone al Project Owner decidir el cierre **solo después** de revisar este PR documental y aceptar expresamente las limitaciones y obligaciones anteriores. Hasta esa decisión, el estado es **CLOSURE PROPOSAL PENDING OWNER AUTHORIZATION — EP-02-02 CLOSE NOT AUTHORIZED**. Este PR no realiza el cierre, no publica una release y no autoriza su propio merge.
