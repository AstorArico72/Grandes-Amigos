[Click here to see the English translation](#English)

# Página para el programa "Grandes Amigos"

## Por [Astor Aricó](https://www.linkedin.com/in/astor-aric%C3%B3-71155a30b/) y [Fermín Fernández](https://www.linkedin.com/in/fernandez-fermin-dev/)

---

**"Grandes Amigos"** es una iniciativa del Gobierno de San Luis, destinada a promover el bienestar de personas de tercera edad en la provincia, con actividades educativas y recreativas.

En éste repositorio hay una página web que complementa ésa iniciativa, ofreciendo a los adultos mayores la posibilidad de ver las actividades de Grandes Amigos e inscribirse a las mismas.

También se ha provisto un mapa de los Centros de Jubilados de San Luis, de parte de un tercero, y noticias de diarios nacionales mediante un feed RSS.

Además, personas que representan a los Ministerios podrán interactuar con la API mediante el Panel Admin, permitiendo así que puedan cargar nuevos eventos, editarlos, y borrarlos, además de gestionar usuarios.

Ésto fue hecho como parte de la Práctica Profesional Tutelada en la [Universidad de La Punta](https://www.ulp.edu.ar/), terminando así nuestra formación como **Técnico Universitario en Desarrollo de Software**, y adquiriendo conocimientos en el desarrollo de aplicaciones web que pueden usarse en el mundo real.

Él back-end consiste en una **API REST** hecha con **.NET Core 8**, **Entity Framework Core**, y **ASP.NET Core**, siguiendo el patrón **.NET MVC**, que conecta con una base de datos **MySQL**, usando **JSONWebToken** para autorizar el acceso. El front-end consiste en una SPA hecha a base de vistas **ASP.NET Razor**, embellecidas con **Bootstrap CSS**, y código **JavaScript** para interactuar con el back-end.

## Funciones
*Para el adulto mayor:*
- Registrarse con correo y contraseña
- Ver los eventos Grandes Amigos
- Inscribirse a los eventos
- Reiniciar la contraseña

*Para el administrador:*
- Crear, editar, y borrar eventos
- Crear, editar, y borrar usuarios

## Limitaciones

Ésta aplicación web no está lista para un entorno de producción, porque queda implementar ésto en versiones futuras:
- Cumplir prácticas estándar de seguridad
- Cargar noticias desde el Panel Admin
- Limitar el acceso de cada administrador a el ministerio al que representa
- Refactorizar a .NET Core 10

## Instalación

Es necesario tener soporte **.NET Core 8** y **MySQL**. Es recomendable instalar phpMyAdmin para tener un control más granular sobre la base de datos.

1. [Descargar el código de la última versión.](https://github.com/AstorArico72/Grandes-Amigos/releases/tag/v0.2)
2. Abrir la consola de comandos en el directorio donde se instaló la aplicación web.
3. Iniciar el servidor MySQL.
4. Compilar la aplicación web, con éste comando en la consola: ´dotnet build´
5. Iniciar la aplicación web, con éste comando en la consola: ´dotnet run´

---

# English

# Webpage for the "Grandes Amigos" Program

## By [Astor Aricó](https://www.linkedin.com/in/astor-aric%C3%B3-71155a30b/) and [Fermín Fernández](https://www.linkedin.com/in/fernandez-fermin-dev/)

**"Grandes Amigos"** is an initiative of the San Luis Government, intended to promote the well-being of elderly people in the province, with educational and recreational activities.

In this repository, there is a web page complementing said initiative, offering the possibility to see Grandes Amigos activities to the elderly, as well as to sign up to them.

A map of Centers for the Retired in the province has been provided from a third party, as well as news from national newspapers, via an RSS feed.

Also, people representing the Ministries will be able to interact with the API via the Admin Panel, thus allowing them to upload new events, editing them, and deleting them, as well as managing users.

This was made as part of the Professional Practice at the [University of La Punta](https://www.ulp.edu.ar/), thus finishing our formation as **Associate in Software Development**, acquiring knowledge in the development of web apps with real-world usability.

The back-end consists of a **REST API** made with **.NET Core 8**, **Entity Framework Core**, and **ASP.NET Core**, following the **.NET MVC** pattern. This API connects with a **MySQL** database, using **JSONWebToken** to authorize the accesses. The front-end consists of an SPA based on **ASP.NET Razor** views, embellished with **Bootstrap CSS**, and **JavaScript** code to interact with the back-end.

## Functions
*For the elderly person:*
- Register with email and password
- See the Grandes Amigos events
- Sign up for these events
- Reset their password

*For the admin:*
- Create, edit, and delete events.
- Create, edit, and delete users.

## Limitations

This web app is not ready for a production environment, as the following is yet to be implemented in future releases:
- Fulfill standard safety practices
- Uploading news from the Admin Panel
- Limiting admin access to the ministry they represent.
- Refactor to .NET Core 10

## Installation

**.NET Core 8** and **MySQL** support are required. phpMyAdmin is recommended to have more granular control over the database.

1. [Download the latest release's source code.](https://github.com/AstorArico72/Grandes-Amigos/releases/tag/v0.2)
2. Open the command prompt in the directory where the web app was installed.
3. Boot the MySQL server.
4. Compile the web app with the following command: ´dotnet build´
5. Start the web app with the following command: ´dotnet run´

### The web app is available only in Spanish.
