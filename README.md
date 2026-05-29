# BalotoAppOnline

Aplicacion de escritorio para Windows que analiza sorteos historicos del Baloto y genera sugerencias estadisticas.

## Actualizaciones desde GitHub

La opcion `Configuracion > Actualizar` consulta el ultimo release publicado en:

`https://github.com/colombianitov2/BalotoAppOnline/releases/latest`

Para que el boton funcione:

1. Crea un release en GitHub con un tag superior a la version instalada, por ejemplo `v1.0.1`.
2. Adjunta al release el instalador o paquete actualizado (`.exe`, `.msi` o `.zip`).
3. La app descargara el primer paquete compatible en la carpeta `Downloads` del usuario.

La version instalada se toma de `Properties/AssemblyInfo.cs`.

## Comentarios

La opcion `Configuracion > Ayuda / comentarios` usa la clave `FeedbackEndpoint` de `App.config` para enviar comentarios por HTTP POST JSON a un servicio privado.

Si `FeedbackEndpoint` queda vacio, la app abre el cliente de correo del usuario con el comentario listo para enviar. Para ocultar completamente el correo destino y enviar directo desde la app, configura un endpoint propio que reenvie el mensaje al correo de soporte.
