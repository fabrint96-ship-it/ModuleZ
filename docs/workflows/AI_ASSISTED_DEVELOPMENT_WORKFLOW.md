# ModuleZ — Workflow de Desarrollo Asistido por IA v1.0

## Estado

- Versión: 1.0
- Proyecto: ModuleZ
- Estado: CANONICAL
- Base de integración: `develop`
- Source of Truth técnico: GitHub

## 1. Propósito

ModuleZ adopta un workflow de desarrollo asistido por IA con separación explícita de autoridad, implementación, evidencia, revisión e integración.

Flujo operativo canónico:

```text
DEFINE
  ↓
IMPLEMENT
  ↓
VERIFY
  ↓
REVIEW
  ↓
AUTHORIZE
  ↓
MERGE
  ↓
SYNCHRONIZE
  ↓
CLEAN
  ↓
RECORD
  ↓
CLOSE
```

Este workflow no sustituye la arquitectura ni las reglas de Foundation de ModuleZ. Las aplica al proceso de implementación.

## 2. Roles oficiales

### User / Project Owner

Mantiene la autoridad final sobre ModuleZ.

Responsabilidades:
- aprobar alcance y decisiones importantes;
- autorizar operaciones sensibles o destructivas;
- realizar validación funcional/manual cuando corresponda;
- hacer el merge manual de PR;
- aprobar releases y decisiones de producto.

### ChatGPT — Architect / Director / Reviewer

Responsabilidades:
- analizar el estado real del proyecto;
- consultar GitHub cuando sea necesario;
- definir el siguiente slice;
- definir objetivo, alcance, restricciones y criterios de aceptación;
- producir el Codex Task;
- revisar PR, diff, arquitectura, tests, CI, scope y desviaciones;
- mantener continuidad entre slices y Episodes.

Regla:

> ChatGPT define qué debe hacerse y cómo debe encajar; no sustituye al implementador ni al Project Owner.

### Codex — Implementer

Responsabilidades:
- inspeccionar el repositorio real;
- crear la rama de trabajo;
- implementar únicamente el slice autorizado;
- crear/modificar archivos y tests necesarios;
- ejecutar validaciones;
- revisar el diff;
- commit y push a la rama de trabajo;
- crear el PR.

Codex NO hace merge a `develop` ni `main`.

### GitHub — Source of Truth

Cuando exista discrepancia entre conversación y repositorio:

> GitHub repository state > conversación.

GitHub conserva ramas, commits, diffs, PR, CI, tags, releases e historial.

### CI — Independent Validator

CI valida desde un checkout limpio y reproducible. Debe crecer progresivamente con ModuleZ e incluir las comprobaciones automatizables relevantes para Unity, arquitectura, tests, assets, escenas y builds.

CI no sustituye la validación manual de gameplay, UX, render, hardware o comportamiento visual.

## 3. Unidad de desarrollo: Slice

Cada Episode se divide en slices pequeños, verificables y cerrables de forma independiente.

```text
EP-XX
├── S-00
├── S-01
├── S-02
└── ...
```

Cada slice sigue:

```text
Branch
→ Implementation
→ Tests/Validation
→ Commit
→ Push
→ PR
→ CI
→ ChatGPT Review
→ User Manual Merge
→ Post-merge Verification
→ Local Synchronization
→ Branch Cleanup
→ CLOSED
```

Un slice debe entregar una capacidad concreta. No se usarán slices como autorización implícita para refactorizaciones amplias no relacionadas.

## 4. Estado inicial obligatorio

Antes de implementar un slice, la rama base local debe estar sincronizada y limpia:

```powershell
git switch develop
git pull --ff-only origin develop
git status
```

Resultado requerido:

```text
On branch develop
Your branch is up to date with 'origin/develop'.
nothing to commit, working tree clean
```

Si el árbol está sucio, no se comienza el slice hasta clasificar y resolver ese estado.

## 5. Especificación obligatoria del Slice

ChatGPT debe definir antes de la implementación:

1. OBJECTIVE
2. SCOPE
3. ARCHITECTURAL CONSTRAINTS
4. IMPLEMENTATION TASK
5. ALLOWED AREAS
6. FORBIDDEN CHANGES
7. AUTOMATED VALIDATION
8. MANUAL VALIDATION
9. ACCEPTANCE CRITERIA
10. EXPECTED PR

Las reglas de Foundation vigentes son restricciones superiores del slice aunque no se repitan íntegramente en cada task.

## 6. Codex Task

Formato canónico:

```text
CODEX TASK — [ID] — [NAME]

OBJECTIVE
...

SCOPE
...

ARCHITECTURAL CONSTRAINTS
...

IMPLEMENTATION TASK
...

ALLOWED AREAS
...

FORBIDDEN CHANGES
...

AUTOMATED VALIDATION
...

MANUAL VALIDATION
...

ACCEPTANCE CRITERIA
...

EXPECTED PR
Branch: feature/... | fix/... | refactor/... | test/... | chore/... | docs/...
Preferred commit: <Conventional Commit>
PR title: <title>
Base: develop

Do NOT merge the PR.
Stop when the PR is ready and report:
- branch
- commit SHA
- PR number
- PR URL
- changed-file count
- tests
- validation
- CI
- deviations
```

## 7. Estrategia Git

```text
main
└── develop
    ├── feature/*
    ├── fix/*
    ├── refactor/*
    ├── test/*
    ├── chore/*
    └── docs/*
```

- `main`: releases estables.
- `develop`: integración del desarrollo.
- `feature/*`: funcionalidad.
- `fix/*`: correcciones.
- `refactor/*`: refactorizaciones.
- `test/*`: pruebas.
- `chore/*`: tooling/mantenimiento.
- `docs/*`: documentación.

Release:

```text
develop → release/* → main → sync back to develop
```

Hotfix:

```text
main → hotfix/* → main → develop
```

No se permite push directo rutinario a `main` o `develop` como sustituto del flujo PR.

## 8. Commits

ModuleZ utiliza Conventional Commits.

Ejemplos:

```text
feat(openworld): add interaction detector
fix(duel): prevent duplicate duel bootstrap
refactor(runtime): introduce scene bootstrap boundary
test(runtime): cover scene transition contract
chore(ci): add Unity validation workflow
docs(episode): close EP-01 documentation
```

Un slice produce uno o pocos commits coherentes. Evitar microcommits innecesarios y commits que mezclen trabajo fuera de alcance.

## 9. Autoridad y permisos de implementación

### Operaciones rutinarias

Pueden realizarse dentro del slice autorizado:
- crear/modificar archivos dentro del repositorio;
- crear rama de trabajo;
- ejecutar herramientas del proyecto;
- tests, validadores y builds;
- fixtures temporales aisladas;
- commit normal;
- push normal a la rama de trabajo;
- crear PR.

### Operaciones sensibles

Requieren revisión humana o quedan fuera de la autoridad rutinaria:
- force push;
- push directo a `main` o `develop`;
- merge a `main` o `develop`;
- eliminación prematura de ramas;
- destrucción de datos persistentes;
- modificación de secretos/credenciales;
- cambios destructivos no previstos;
- operaciones fuera del repositorio.

Principio:

```text
Routine Operation     → automatic
Sensitive Operation   → human review
Destructive Operation → explicit authorization
```

## 10. Validación en cuatro niveles

### Nivel 1 — Implementer

Ejecutar toda validación local automatizable definida por el slice, incluyendo cuando exista soporte:
- compilation;
- formatting;
- architecture validation;
- unit/EditMode tests;
- PlayMode/integration tests;
- build;
- `git diff --check`.

No se deben inventar comandos o suites que ModuleZ todavía no haya implementado.

### Nivel 2 — GitHub Actions / CI

Checkout limpio y validación reproducible. Las capacidades de CI se incorporarán progresivamente conforme ModuleZ implemente sus validadores y automatización Unity.

### Nivel 3 — Integración real controlada

Cuando corresponda: Unity runtime, filesystem, Git, persistence, platform adapters, build pipeline y otros adapters controlados. Evitar dependencia innecesaria de servicios externos reales.

### Nivel 4 — Project Owner

Validación humana cuando sea necesaria:
- gameplay;
- UX/UI;
- render visual;
- audio;
- controller/hardware;
- flujo funcional real;
- comportamiento en Unity Editor/build.

> Automated validation ≠ Manual validation.

## 11. Pull Request

Codex prepara el PR pero no lo fusiona.

Base requerida para slices normales: `develop`.

El PR debe documentar como mínimo:
- Summary;
- Architecture;
- Implementation;
- Tests;
- Validation;
- Known limitations;
- Manual validation;
- Out of scope.

## 12. Review independiente

El informe del implementador no constituye aprobación.

ChatGPT debe revisar directamente en GitHub, cuando las capacidades disponibles lo permitan:
- estado del PR;
- base/head;
- commits;
- diff y archivos cambiados;
- alcance;
- arquitectura;
- tests;
- CI;
- errores y desviaciones;
- scope creep;
- cambios inesperados.

Resultado:

```text
CHANGES REQUIRED
```

o:

```text
READY FOR INTEGRATION
```

## 13. Merge

Condiciones objetivo:

```text
IMPLEMENTATION: COMPLETE
AUTOMATED VALIDATION: PASSED
ARCHITECTURAL REVIEW: PASSED
SCOPE REVIEW: PASSED
CI: PASSED or explicitly NOT AVAILABLE with recorded reason
MANUAL VALIDATION: PASSED when required
PR: READY TO MERGE
```

El Project Owner realiza el merge manual.

ChatGPT y Codex no deben convertir `PR ready` en autorización implícita para merge automático.

## 14. Verificación post-merge

Después del merge, verificar en GitHub:
- PR merged;
- merge commit;
- `develop` actualizado;
- CI post-merge cuando exista.

Solo entonces la implementación se considera integrada.

## 15. Sincronización local

```powershell
git switch develop
git pull --ff-only origin develop
git status
```

Debe terminar limpio y sincronizado.

## 16. Limpieza de rama

Solo después de verificar el merge:

```powershell
git branch -d <branch>
git push origin --delete <branch>   # solo si continúa existiendo remotamente
git fetch --prune
git branch -a
```

Nunca eliminar una rama como sustituto de verificar la integración.

## 17. Cierre del Slice

Formato canónico:

```text
[SLICE-ID] — [NAME]
IMPLEMENTATION: COMPLETE
VALIDATION: PASSED
PR: MERGED
INTEGRATED INTO develop: YES
LOCAL BRANCH: DELETED
REMOTE BRANCH: DELETED
LOCAL develop: CLEAN AND SYNCHRONIZED
STATUS: CLOSED
```

Cuando una comprobación no sea aplicable debe indicarse explícitamente; no se transforma `UNKNOWN` en `PASS`.

## 18. Cierre del Episode

Cuando todos los slices estén integrados, la documentación final del Episode se prepara en una rama `docs/*`, se revisa mediante PR a `develop`, se valida y el Project Owner realiza el merge.

Cierre:

```text
EP-XX
IMPLEMENTATION: CLOSED
DOCUMENTATION: CLOSED
STATUS: CLOSED
```

El siguiente Episode comienza preferentemente en un nuevo chat de ChatGPT.

## 19. Release

```text
develop
↓
release/vX.Y.Z
↓
final validation
↓
PR → main
↓
manual merge
↓
tag vX.Y.Z
↓
GitHub Release
↓
sync main → develop
```

No confundir:

```text
Episode complete
≠ Release complete
≠ Deployment complete
≠ Healthy production
```

## 20. Adaptación específica a ModuleZ / Unity

Este workflow hereda la Foundation de ModuleZ y añade estas reglas operativas:

1. Unity está fijado actualmente a 6000.3.13f1; un slice no actualiza Unity incidentalmente.
2. Los archivos `.meta` forman parte de la identidad de assets y deben preservarse al mover assets.
3. No se editarán escenas/assets serializados de forma masiva sin necesidad y validación.
4. La validación Unity puede requerir EditMode, PlayMode, scene validators, canonical smoke y build según el slice.
5. El canonical smoke de recuperación es:
   `Boot → MainMenu → Start/Continue → OpenWorld → Move → Interact → Start Duel → Player action → AI action → Finish/abandon → Return OpenWorld → Save → MainMenu`.
6. En recuperación Legacy se mantiene `Observe → Introduce replacement boundary → Migrate consumers → Verify → Remove legacy`.
7. No se crea nueva dependencia de runtime/build/distribution respecto a TBD.
8. Steam permanece detrás de contratos de plataforma; ningún slice de recuperación introduce SDK Steam incidentalmente.
9. Las reglas de Foundation y los futuros validators/CI gates prevalecen sobre la conveniencia del agente.
10. Un resultado visual o de gameplay que requiera Unity se marca `MANUAL VALIDATION REQUIRED` hasta que el Project Owner lo valide.

## 21. Principios fundamentales

```text
ChatGPT ≠ Implementer
Codex ≠ Architect authority
GitHub = Source of Truth
CI ≠ Manual validation
Implementation ≠ Integration
Integration ≠ Closure
Observation ≠ Mutation
Capability ≠ Authority
PR ready ≠ PR merged
Merged ≠ Local synchronized
Episode implemented ≠ Episode documented
Release ≠ Deployment
```

Regla central:

> DEFINE → IMPLEMENT → VERIFY → REVIEW → AUTHORIZE → MERGE → SYNCHRONIZE → CLEAN → RECORD → CLOSE
