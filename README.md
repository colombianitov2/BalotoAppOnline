📦 Descripción completa del proyecto para GitHub
🎲 Generador de Baloto Online
Generador de Baloto Online es una aplicación de escritorio para Windows (32/64 bits) que ayuda a analizar, predecir y verificar resultados del juego Baloto (Colombia). Utiliza estadísticas históricas, frecuencia por posición, algoritmos inteligentes y extracción automática de datos desde la web oficial para ofrecer sugerencias de apuestas informadas y herramientas útiles a los jugadores.

✨ Funcionalidades principales
1. Generadores de números
Automático (inteligente) – Combina frecuencia histórica, números calientes recientes y evita números fríos para ofrecer una combinación estadísticamente robusta.

Resultado por frecuencia – Sugiere los números que más han aparecido en cada posición (balota 1 a 5 y súper balota) a lo largo de todo el historial.

Sugerencia Gaussiana – Versión aleatoria ponderada por frecuencias (los números populares tienen más probabilidad, pero no siempre salen los mismos).

2. Gestión de datos
Ingresar sorteos manualmente – Añade sorteos con fecha, 5 balotas (1-43 distintas) y súper balota (1-16).

Exportar / Importar – Guarda y carga el historial completo en formato .txt.

Historial – Lista completa de sorteos, ordenados por fecha, con opción de eliminar registros.

3. Análisis y estadísticas
Tabla de análisis – Frecuencia por posición (global, mensual o anual) indicando cuántas veces ha salido cada número en cada columna, más el número “más repetido” de cada una.

Combinaciones más repetidas – Muestra qué grupos de 5 balotas principales han aparecido juntos en varios sorteos (solo si hay repeticiones).

Exportar tablas a CSV – Guarda cualquier tabla de estadísticas en un archivo .csv para usarlo en Excel.

4. Utilidades
Verificador de tiquetes – Permite ingresar 5 balotas + súper balota y te indica si esa combinación exacta ha salido alguna vez, mostrando las fechas y la cantidad de aciertos.

Actualización automática desde la web oficial – Conecta a https://www.baloto.com/resultados, descarga todos los sorteos históricos de Baloto (ignorando Revancha) y los añade a tu base de datos local sin duplicar.

Simulador de probabilidad – Ingresa tu combinación y la aplicación generará sorteos aleatorios hasta que coincida con la tuya, mostrando cuántos intentos se necesitaron (para entender lo improbable que es acertar).

5. Interfaz y ayuda
Ventana principal – Botones redondeados, reloj en tiempo real (sin segundos), label de progreso durante actualizaciones web.

Pestaña "Ayuda" – Explicación detallada de cada función.

Pestaña "Créditos" – Reconoce a los colaboradores (Ernesto Pernett Cuesta, Claude, DeepSeek, Gemini).

🛠 Tecnologías utilizadas
Lenguaje: C# 9.0 (compatible con .NET Framework 4.7.2)

Framework: Windows Forms (.NET Framework 4.7.2)

Paquetes NuGet:

Newtonsoft.Json – Persistencia de datos.

PuppeteerSharp – Extracción de datos desde la web oficial.

HtmlAgilityPack – Parseo de HTML obtenido.

Instalador: Inno Setup 6.7.1 (script incluido en el repositorio).

Arquitectura: Soporta x86 y x64 (compilación AnyCPU).

📥 Instalación
Opción A – Instalador automático (recomendado)
Descarga el archivo GeneradorBalotoOnline_Setup.exe desde la sección Releases.

Ejecútalo como usuario normal (no necesita permisos de administrador).

Sigue las instrucciones. La aplicación se instalará en %LocalAppData%\Programs\Generador de Baloto Online.

Al finalizar, puedes ejecutarla desde el acceso directo del escritorio o del menú inicio.

Opción B – Manual (portable)
Descarga el último .zip del repositorio que contiene la carpeta Release con el .exe y las .dll.

Descomprime en la ubicación que prefieras.

Ejecuta BalotoAppOnline.exe.

⚠️ Nota importante – Primera ejecución
La primera vez que uses la función "Actualizar desde web", la aplicación descargará automáticamente Chromium (unos 150 MB). Esto es necesario para que PuppeteerSharp pueda renderizar la página oficial. La descarga solo ocurre una vez; las siguientes actualizaciones serán rápidas.

🚀 Cómo usar la aplicación
Actualizar datos
Haz clic en “Actualizar desde web” (botón verde). La app recorrerá todas las páginas de resultados oficiales y agregará los sorteos de Baloto que no estén en tu base de datos.

Generar números
Elige entre los tres botones amarillos:

Automático → combinación inteligente.

Resultado por frecuencia → moda por posición.

Sugerencia Gaussiana → aleatorio ponderado.

Aparecerá un mensaje con los números sugeridos.

Verificar si tu tiquete ya salió
Haz clic en “Verificador de tiquetes” → ingresa tus 5 balotas y súper balota → la app buscará en el historial y te dirá si esa combinación ha aparecido antes y en qué fechas.

Analizar frecuencias
Ve a “Tabla de análisis”. Navega por las pestañas: frecuencia global, mensual, anual o combinaciones repetidas. Puedes exportar cualquier tabla a CSV.

Simular probabilidad
“Simulador de probabilidad” : ingresa una combinación y la aplicación generará sorteos aleatorios hasta acertar. Muestra los intentos necesarios (ideal para comprender la dificultad del juego).

Gestionar historial
“Historial”: lista todos los sorteos con opción de eliminar.

“Ingresar datos”: añadir un sorteo manual.

“Exportar/Importar”: mover datos entre dispositivos o hacer respaldos.

📁 Estructura del repositorio
text
BalotoAppOnline/
├── BalotoAppOnline.csproj
├── Program.cs
├── Form1.cs
├── AcercaDeForm.cs
├── DatosBaloto.cs
├── EstadisticasForm.cs
├── HistorialForm.cs
├── IngresoForm.cs
├── RoundedButton.cs
├── Sorteo.cs
├── VerificadorForm.cs
├── SimuladorProbabilidadForm.cs
├── WebScraper.cs
├── packages.config
├── Script_InnoSetup.iss          (script del instalador)
└── README.md                     (este archivo)
🤝 Colaboradores
Ernesto Pernett Cuesta – Idea, dirección, diseño e ingeniería mecánica aplicada al desarrollo.

Claude (Anthropic) – Desarrollo de software, interfaz y correcciones.

Codex (OpenAI) – Asistencia en generación de código, empaquetado, verificación de compilación y preparación del repositorio.

DeepSeek (IA) – Asistencia técnica, revisión de algoritmos y documentación.

Gemini (Google) – Asesoría en imagen, diseño y consultas variadas.

📄 Licencia
Todos los derechos reservados.

Este proyecto no concede permiso para vender, revender, modificar, redistribuir, sublicenciar ni publicar versiones derivadas de la aplicación sin autorización escrita del autor.

Copyright (c) 2026 Ernesto Pernett Cuesta.

No se garantiza que las predicciones aumenten la probabilidad de ganar el Baloto. El juego sigue siendo aleatorio. Úsalo como herramienta de entretenimiento y análisis estadístico.

📧 Contacto
Si tienes sugerencias, errores o preguntas, puedes abrir un Issue en este repositorio o contactar al autor a través de su perfil de GitHub.

🎯 ¿Quieres probarlo?
Descarga el instalador desde la sección Releases o clona el repositorio y compila con Visual Studio 2022. ¡Buena suerte y juega con responsabilidad!

Generador de Baloto Online – Tu aliado estadístico para entender el juego.