# ms-documentation
## Descripción

Este proyecto en C# creara un microservicio del tipo API donde proveera todos los certificados / documentos necesarios para la red academica SysAcad

## Funcionalidades

- Manejo de API para solicitar información requerida por la web.
- Generacion de documentos en varios formatos necesarios
    - Formato de documento portatil (PDF)
    - Texto de OpenDocument (ODT)
    - Microsoft Word (DOCX)

## Requisitos

### Para ejecutar la aplicación:
##### Nativo
- Sistema operativo compatible (Windows o Linux)
- Permisos de ejecución para el archivo (`ms-documentation` o `ms-documentation.exe`)
- Microservicios que proveen la información de los certificados a generar
##### Docker
- Imagen de docker del programa
- Red externa creada (para comunicar con los otros microservicios)
- Traefik (contenedor de Docker)
- Microservicios que proveen la información de los certificados a generar
- (Opcional) Redis o DragonflyDB para cache


### Para compilar o desarrollar:
- [.NET SDK](https://dotnet.microsoft.com/download)
- Visual Studio o cualquier editor compatible con C# (por ejemplo, Visual Studio Code con la extensión de C#)
- Docker (en caso de buscar desarrollarlo en contenedores)

## Ejecución

#### Docker
1. Crear la imagen en Docker `docker build -t ms-documentation .`
2. Montar los contenedores de los microservicios en la misma red
3. Configurar el archivo `.env` ubicado en la carpeta `Docker` con la informacion correspondiente a los microservicios necesarios
4. Modificar la red con la utilizada en `Docker/docker-compose.yml` (actualmente ms-red)
5. Componer el contenedor con `docker compose up`
- Para hacer solicitudes de certificados se realizaran a `documentacion.universidad.localhost/api/v1/certificate/{tipo_documento}/{id_alumno}`. Donde `tipo_documento` sera el tipo de formato del certificado (pdf/docx/odt) y `id_alumno` sera el identificador del alumno en el microservicio de alumnos


## Desarrollador

Esta aplicación fue desarrollada por:

- Ignacio Bianchi – Desarrollo completo
