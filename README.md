# BalotoAppOnline

Generador de Baloto Online es una aplicacion de escritorio para Windows que analiza sorteos historicos del Baloto, genera sugerencias estadisticas y ofrece utilidades como historial, verificador de tiquetes, actualizacion desde la web y ayuda/comentarios.

## Descarga e instalacion

La forma recomendada de instalar la aplicacion es desde la seccion Releases del repositorio.

El actualizador integrado de la aplicacion consulta el ultimo release publicado en GitHub y descarga el instalador `.exe` mas reciente cuando existe una version nueva.

## Actualizacion

Para que el boton **Actualizar** funcione correctamente:

1. Publica un release nuevo en GitHub con un tag superior al instalado, por ejemplo `v1.0.1`.
2. Adjunta al release el instalador `.exe` generado para la version nueva.
3. La aplicacion comparara la version local con el release mas reciente y descargara el ejecutable publicado.

## Comentarios

La opcion **Ayuda / comentarios** de la aplicacion permite enviar observaciones desde la interfaz sin mostrar el correo de soporte en pantalla.

## Tecnologias

- C#
- Windows Forms
- .NET Framework 4.7.2
- PuppeteerSharp
- HtmlAgilityPack

## Nota

El repositorio debe mantenerse enfocado en el codigo fuente y en los releases. El archivo de instalacion final debe publicarse como asset del release, no como ZIP del proyecto.
