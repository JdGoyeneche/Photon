# WILD YOSHI SANCTUARY RACING (Unity + Photon Fusion)

Juego multijugador en tiempo real desarrollado en Unity con Photon Fusion, lo que permite que varios jugadores compitan en una misma pista, sincronizando posición, movimiento y estado en red de forma estable.

Este proyecto está enfocado en demostrar el uso de networking en videojuegos con Unity y arquitectura cliente-servidor tipo Host/Client.


---

##  Descripción del juego

WILD YOSHI SANCTUARY RACING es un prototipo de carrera multijugador donde los jugadores pueden:

- Crear o unirse a una partida
- Conectarse a un Host en tiempo real
- Ver a otros jugadores moverse en el mapa
- Controlar un vehículo sincronizado por red

El sistema está construido con Photon Fusion para garantizar baja latencia y sincronización estable.

---

##  Tecnologías utilizadas

- Unity 
- Photon Fusion Networking
- C# (Programación principal)
- TextMeshPro (UI)
- Unity Scene Management
- Input System de Unity

---

##  Características principales

###  Multiplayer
- Sistema Host / Client
- Conexión en tiempo real
- Spawn automático de jugadores
- Sincronización de posiciones

###  Jugabilidad
- Movimiento de vehículo fluido
- Cámara en tercera persona para jugador local
- Control independiente por jugador

###  Networking
- Uso de `NetworkRunner`
- Prefabs sincronizados con `NetworkObject`
- Autoridad de input (`InputAuthority`)
- Callbacks con `INetworkRunnerCallbacks`

###  Sistema técnico
- Control de conexión y desconexión
- Manejo de escena automática
- Detección de host activo

---

##  Requisitos del sistema

Antes de ejecutar el proyecto asegúrate de tener:

- Unity Hub instalado
- Unity 6
- Proyecto abierto correctamente
- Cuenta en Photon Engine
- App ID de Photon Fusion configurado
- Conexión estable a Internet

---

##  Instalación y ejecución

### 1. Abrir el proyecto en Unity
- Abrir Unity Hub
- Seleccionar Open Project
- Elegir la carpeta del proyecto

---

### 2. Configurar Photon Fusion

Ruta dentro del proyecto:

Assets/Photon/Fusion/Resources

Pasos:
- Abrir el archivo de configuración de Fusion
- Pegar tu App ID de Photon Engine
- Guardar cambios

---

### 3. Configurar escenas del juego

Ir a:

File > Build Settings > Scenes In Build

Agregar:
- Escena de menú
- Escena principal del juego
- Escena Game over

---

### 4. Ejecutar el juego

- Presionar Play en Unity
- Seleccionar modo de conexión:
  - Host: crea la sala
  - Client: se une a la sala
- Esperar sincronización de jugadores

---

##  Controles

- W A S D → Movimiento del vehículo   

---

##  Arquitectura del sistema

El juego utiliza una arquitectura basada en Photon Fusion:

- NetworkRunner → Control principal de la red
- NetworkObject → Objetos sincronizados
- InputAuthority → Control del jugador local
- State Synchronization → Actualización de posición en tiempo real
- Host Migration (base preparada)

---

## Problemas comunes y soluciones

###  No conecta a la partida
- Verifica que el App ID de Photon sea correcto
- Revisa conexión a internet
- Asegúrate de no estar bloqueado por firewall

###  No aparecen otros jugadores
- El prefab debe tener `NetworkObject`
- Debe estar registrado en Fusion
- El Host debe estar activo

###  Errores al iniciar el juego
- Verifica que las escenas estén en Build Settings
- Confirma que la escena inicial sea correcta

###  El jugador no se mueve
- Revisa InputAuthority
- Verifica scripts de movimiento

---

##  Conceptos aprendidos en el proyecto

- Networking en Unity con Photon Fusion
- Sincronización de estado en tiempo real
- Arquitectura cliente-servidor
- Manejo de prefabs en red
- Control de autoridad de jugador
- Gestión de escenas multijugador

---

##  Futuras mejoras

- Sistema de checkpoints y vueltas
- Sistema de ranking en tiempo real
- Lobby avanzado con UI profesional
- Animaciones del vehículo
- Sonidos y efectos de motor
- Sistema de reinicio de partida
- Chat entre jugadores

---

##  Notas importantes

- El Host es el servidor principal de la partida
- Los clientes dependen del Host para sincronización
- La cámara solo se activa para el jugador local
- Todos los objetos en red deben estar registrados en Fusion

---


## Desarrollado por:

- Luis Miguel Guerrero  
- Juan David Goyeneche  