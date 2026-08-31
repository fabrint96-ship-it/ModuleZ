# Module Z — Duel3D

## Descripción

Duel3D es el sistema de combate principal de Module Z.

Dos jugadores (Player e IA) colocan piezas Z tridimensionales dentro de un tablero 3D. El objetivo es gestionar el espacio, provocar eliminaciones y terminar el duelo con ventaja frente al rival.

Todo el sistema está construido completamente mediante código y utiliza estructuras modulares independientes.

---

# Flujo del juego

```text
OpenWorld
    ↓
NPC Rival
    ↓
Duel3D
    ↓
Victoria / Derrota / Abandono
    ↓
OpenWorld
```

---

# Controles

## Movimiento

```text
Flecha izquierda   -> mover X-
Flecha derecha     -> mover X+
Flecha arriba      -> mover Z+
Flecha abajo       -> mover Z-
```

## Altura

```text
R -> subir
F -> bajar
```

## Rotación

```text
Q -> Yaw -
E -> Yaw +

Z -> Pitch -
X -> Pitch +

C -> Roll -
V -> Roll +
```

## Colocación

```text
Espacio -> colocar pieza Z
```

## Cámara

```text
Botón derecho ratón -> orbitar
Rueda ratón         -> zoom
T                   -> reset cámara
```

## Menú pausa

```text
ESC -> abrir/cerrar pausa
```

---

# Reglas actuales

## Pieza

Cada jugada coloca una pieza Z compuesta por:

```text
4 cubos
```

## Restricción de terminaciones

Después de colocar una pieza:

```text
Las terminaciones de la última pieza del mismo color quedan bloqueadas.
```

No puede colocarse una nueva pieza tocando dichas posiciones.

Excepción:

```text
Si esos cubos fueron eliminados posteriormente,
las posiciones vuelven a ser válidas.
```

---

## Eliminaciones

Después de cada jugada:

```text
Se revisa todo el tablero 3D.
```

Se buscan líneas rectas únicamente en:

```text
Eje X
Eje Y
Eje Z
```

No se revisan:

```text
Diagonales
```

---

## Regla de eliminación

Actualmente:

```text
5 cubos consecutivos
```

provocan eliminación.

Ejemplos:

```text
XXXXX
```

↓

```text
(se eliminan los 5)
```

```text
XXXXXX
```

↓

```text
(se eliminan 5 y queda 1)
```

```text
XXXXXXXXXX
```

↓

```text
(se eliminan 10)
```

---

# IA

La IA utiliza escalado progresivo.

No existen niveles fijos.

La dificultad depende de:

```text
Rivales derrotados
+
Duelos ganados
```

Parámetros escalados:

```text
searchDepth
randomness
reactionDelay
maxMovesEvaluated
blockPlayerWeight
clearOwnColorWeight
centerControlWeight
verticalControlWeight
dangerPenaltyWeight
```

---

# Match Config

Cada duelo utiliza:

```text
Duel3DMatchConfig
```

Configuración:

```text
tamaño tablero
duración
cámara
colores
IA
debug
reglas visuales
```

Proveedor:

```text
Duel3DMatchConfigProvider
```

---

# Estructura de carpetas

```text
Assets/ModuleZ/Duel3D
├── AI
├── Audio
├── Board
├── Core
├── Feedback
├── Pieces
├── Rules
├── Runtime
├── UI
└── Visuals
```

---

# Scripts principales

## Runtime

```text
Duel3DRuntimeBuilder
```

Control principal del duelo.

---

## Tablero

```text
Duel3DBoardGrid
Duel3DBoardBoundsBuilder
```

---

## Piezas

```text
ZPiece3DShape
ZPiece3DRotationState
Duel3DPiecePlacement
Duel3DPieceVisualBuilder
```

---

## IA

```text
Duel3DAIController
Duel3DAISettings
Duel3DAIDifficultyScaler
Duel3DAIDebugInfo
```

---

## Feedback

```text
Duel3DGameFeedbackManager
Duel3DFeedbackController
Duel3DAudioFeedbackController
Duel3DResultVisualController
```

---

## Resultado

```text
Duel3DResultManager
```

Gestiona:

```text
Victoria
Derrota
Abandono
Retorno al OpenWorld
Desbloqueos
```

---

# Estado actual

Completado:

```text
Tablero 3D
Rotación 3D
Preview transparente
IA progresiva
HUD
Cámara orbital
Sistema de eliminación
Sistema de restricciones
Sistema de feedback
Sistema de resultado
Integración OpenWorld
```

Pendiente:

```text
Partículas avanzadas
VFX de victoria
SFX definitivos
Modelos visuales finales
Balance IA
Guardado avanzado
```
