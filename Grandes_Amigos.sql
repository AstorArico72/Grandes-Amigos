-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 17-09-2025 a las 04:40:45
-- Versión del servidor: 10.4.28-MariaDB
-- Versión de PHP: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `grandes_amigos`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `administradores`
--

CREATE TABLE `administradores` (
  `ID` int(9) UNSIGNED NOT NULL,
  `Nombre_Usuario` varchar(255) NOT NULL,
  `Clave` varchar(100) NOT NULL,
  `ID_Ministerio` int(10) UNSIGNED NOT NULL,
  `Email` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `administradores`
--

INSERT INTO `administradores` (`ID`, `Nombre_Usuario`, `Clave`, `ID_Ministerio`, `Email`) VALUES
(1, 'Dummy', 'Lm3qrNTqMvJuUsvtB20NUItRh//9VjZjeqAoqA1G1G4=', 1, 'dummy@example.net'),
(2, 'DMY', 'q/sMjWB73dxjPRaYObCucPVt0ubfG/iZ+fMIrPN6pmc=', 1, 'dmy@example.net'),
(4, 'Fermin', 'trXZyRuZn3XriDEkP1oWEruRPMehmPu5P2GI7qLjOaU=', 1, 'fermin2049@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `eventos`
--

CREATE TABLE `eventos` (
  `ID` int(11) UNSIGNED NOT NULL,
  `Título` varchar(255) NOT NULL,
  `Descripción` varchar(4096) NOT NULL COMMENT 'Hay que acordar el largo aquí.',
  `Fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `Foto` varchar(255) NOT NULL COMMENT 'Aquí se guarda la URL de la foto. El archivo se guarda en el servidor.',
  `ID_Ministerio` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `eventos`
--

INSERT INTO `eventos` (`ID`, `Título`, `Descripción`, `Fecha`, `Foto`, `ID_Ministerio`) VALUES
(1, 'Dummy', 'Lorem ipsum dolor sit amet, consector auspiacitng wsapcdmvngijfvjmsefcidnildnftrvun4eil the4rail fthderlih dilm', '2025-04-21 19:00:00', 'https://uploads.teachablecdn.com/attachments/GuHkPGomRSa2Bds59ir9_WhatsApp-Image-2024-07-27-at-16.49.37-1-1.jpg', 1),
(2, 'Edited Dummy', 'Not the original description.', '2025-04-27 00:00:00', 'https://uploads.teachablecdn.com/attachments/lwdWDijxTymEjAKARqEg_WhatsApp-Image-2024-07-27-at-16.49.34-1.jpeg', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inscripciones`
--

CREATE TABLE `inscripciones` (
  `ID` int(10) UNSIGNED NOT NULL,
  `ID_Evento` int(10) UNSIGNED NOT NULL,
  `ID_Inscrito` int(9) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `inscripciones`
--

INSERT INTO `inscripciones` (`ID`, `ID_Evento`, `ID_Inscrito`) VALUES
(2, 2, 0),
(5, 1, 16777216),
(6, 2, 16777216),
(7, 1, 33010203);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `ministerios`
--

CREATE TABLE `ministerios` (
  `ID` int(10) UNSIGNED NOT NULL,
  `Nombre` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `ministerios`
--

INSERT INTO `ministerios` (`ID`, `Nombre`) VALUES
(1, 'Dummy');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `noticias`
--

CREATE TABLE `noticias` (
  `ID` int(11) NOT NULL,
  `Título` tinytext NOT NULL,
  `Autor` varchar(100) NOT NULL,
  `Enlace` varchar(1000) NOT NULL,
  `Fecha_Publicación` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Contenido` text DEFAULT NULL,
  `Categoría` varchar(50) DEFAULT 'General',
  `ImagenUrl` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `noticias`
--

INSERT INTO `noticias` (`ID`, `Título`, `Autor`, `Enlace`, `Fecha_Publicación`, `Contenido`, `Categoría`, `ImagenUrl`) VALUES
(23, 'Ganaron 5000 dólares a la semana de por vida en un concurso, pero la empresa quebró y esto pasará con el premio', 'Desconocido', 'https://www.lanacion.com.ar/estados-unidos/ganaron-5000-dolares-a-la-semana-de-por-vida-en-un-concurso-pero-la-empresa-quebro-esto-pasara-con-nid06092025/', '2025-09-15 08:45:00', 'La quiebra de Publishers Clearing House dejó sin cobrar a ganadores del premio “para siempre”. Algunos acumulan deudas millonarias mientras otros lograron salvarse al cobrar en un solo pago.', 'Estados Unidos', 'https://resizer.glanacion.com/resizer/v2/7QLZOGWDX5CKZPXA2ASDP2NJMQ.jpg?auth=e3f5fb98f6dad0562f488b39bbceb31b0ab5d0bffc5617d45b16e092416681e1&smart=true&width=2000&height=1333'),
(24, 'Por la cadena nacional, Milei presentará el Presupuesto 2026, en un clima de tensión con gobernadores y dudas por la economía', 'Desconocido', 'https://www.lanacion.com.ar/politica/por-la-cadena-nacional-milei-presentara-el-presupuesto-2026-en-un-clima-de-tension-con-gobernadores-nid14092025/', '2025-09-15 06:25:54', 'El Presidente grabará por la tarde el mensaje en la Casa Rosada, que se emitirá desde las 21; habrá reuniones de las mesas del gobierno, que busca reponerse de la derrota electoral en la provincia de Buenos Aires; en medio de la incertidumbre, mantienen bajo siete llaves los fundamentos del proyecto', 'Política', 'https://resizer.glanacion.com/resizer/v2/4GP5LNDBXVGLZPQA2BY5RDQCHQ.JPG?auth=93aa02503a4ad5ffd5c3ccec8b085c2849162304944ab67a3c640a26e76de52d&smart=true&width=2000&height=1460'),
(25, 'Ricardo López Murphy planteó que el Gobierno no hizo modificaciones en el gabinete “porque no tiene recambio”', 'Desconocido', 'https://www.lanacion.com.ar/politica/ricardo-lopez-murphy-senalo-que-el-gobierno-no-hizo-modificaciones-en-el-gabinete-porque-no-tiene-nid15092025/', '2025-09-15 06:17:27', 'El economista advirtió que los problemas de la administración libertaria se deben a una “mala gestión” tanto económica como política', 'Política', 'https://resizer.glanacion.com/resizer/v2/3EMXBWHFDRGYFCMUFD7SNZFSQA.PNG?auth=60df9c563eab83f2d94fca7125c73adc5205fd7f8784dfc5c6a03f51cb73497a&smart=true&width=501&height=347'),
(26, 'Protestas en países asiáticos', 'Desconocido', 'https://www.lanacion.com.ar/opinion/protestas-en-paises-asiaticos-nid15092025/', '2025-09-15 06:08:00', '', 'Opinión', NULL),
(27, 'El Gobierno está bajo la lupa, hasta de los propios', 'Desconocido', 'https://www.lanacion.com.ar/politica/el-gobierno-esta-bajo-la-lupa-hasta-de-los-propios-nid14092025/', '2025-09-15 06:05:41', 'Las disputas internas en el espacio libertario entre karinistas y santicaputistas no encuentra solución, a pesar de algunas frágiles treguas', 'Política', 'https://resizer.glanacion.com/resizer/v2/AEXSGC5AEFEFRBS24Z6DOF6YCA.JPG?auth=dfa334aa4bae9447818cc53f4f726adc97aca84ebd9e11b09fddd19eb4bcfdba&smart=true&width=2000&height=1333'),
(28, 'El último de la casta', 'Desconocido', 'https://www.lanacion.com.ar/opinion/el-ultimo-de-la-casta-nid15092025/', '2025-09-15 06:05:00', '', 'Opinión', NULL),
(29, 'Límites al celular en el aula', 'Desconocido', 'https://www.lanacion.com.ar/editoriales/limites-al-celular-en-el-aula-nid15092025/', '2025-09-15 06:05:00', '', 'Editoriales', 'https://resizer.glanacion.com/resizer/v2/VXRDL7OBRRGKHHUY6E6POYCRYA.JPG?auth=e5f964365e8acdc4899d94ad190f47567378371c52ce99cdff6ef558f8c91cfe&smart=true&width=5616&height=3744'),
(30, 'Cartas de lectores: Mandantes, sospechas, verdad y república', 'Desconocido', 'https://www.lanacion.com.ar/opinion/carta-de-lectores/cartas-de-lectores-mandantes-rever-politicas-sospechas-nid15092025/', '2025-09-15 06:05:00', '', 'Carta de lectores', 'https://resizer.glanacion.com/resizer/v2/ZOJDMDHWCNE53CTUQSLR45KYLQ.jpg?auth=24cb198386eec7a88e26ac69cb2c3ec58cab2ef2ef4ecacb8a62c8b1b27df659&smart=true&width=1183&height=863'),
(31, 'Penas leves por delitos aberrantes', 'Desconocido', 'https://www.lanacion.com.ar/editoriales/penas-leves-por-delitos-aberrantes-nid15092025/', '2025-09-15 06:05:00', 'Es imperioso proteger a los menores de edad de las perversas redes de pedofilia, elevando castigos y habilitando la identificación de abusadores', 'Editoriales', 'https://resizer.glanacion.com/resizer/v2/UI44DVLZWNDWTNVPXQSN34AVRM.jpg?auth=fd494ac232ede936e91009a398160027358ac3a9c888b26bc08147904b9c6ba7&smart=true&width=2000&height=1333'),
(32, 'Divorcio gris: cada vez más gente se anima a separarse después de los 50', 'Desconocido', 'https://www.lanacion.com.ar/sabado/divorcio-gris-cada-vez-mas-gente-se-anima-a-separarse-despues-de-los-50-nid15092025/', '2025-09-15 06:02:00', 'A contramano de los temores y prejuicios de antes, muchos hombres y mujeres dan vuelta la página amorosa en la segunda mitad de la vida', 'Sábado', 'https://resizer.glanacion.com/resizer/v2/7SCTHRS54FFOHNU7EUAO64VGSE.jpg?auth=01243b30e6524cec2ea2510b32773b6143359d0019f940ff143b6511dd812e94&smart=true&width=2000&height=1333'),
(33, 'Solo en Off | “¡Choque los cinco!”: el barrilete sueco que le devolvió la alegría al Presidente ', 'Desconocido', 'https://www.lanacion.com.ar/politica/solo-en-off-choque-los-cinco-el-barrilete-sueco-que-le-devolvio-la-alegria-al-presidente-nid15092025/', '2025-09-15 06:01:00', 'Macri saludó a Larreta pero tomó café con Calcaterra; Carrió sin novio pero con rating; “Toto” Caputo esperaba una inflación más baja', 'Política', 'https://resizer.glanacion.com/resizer/v2/HC2GVR6REZG7RGG4IPJSDGSRZM.jpg?auth=fb001207151b0bb7bd06ab2e1e557a806cb62c419e4f045801be61f03da52a8a&smart=true&width=2000&height=1333'),
(34, 'La fortaleza de los partidos políticos en Uruguay', 'Desconocido', 'https://www.lanacion.com.ar/opinion/la-fortaleza-de-los-partidos-politicos-en-uruguay-nid15092025/', '2025-09-15 06:00:00', 'El país tiene una democracia estable basada en colectividades partidarias que atraviesan su historia y mantienen vigencia', 'Opinión', 'https://resizer.glanacion.com/resizer/v2/BSVVPNCIF5DE3L72WINJVZODTY.jpg?auth=2a9be773919e7f485809bb50eca20084083a2fe75761229ad54b76f78d556016&smart=true&width=2000&height=1333'),
(35, 'La agenda de la TV del lunes: las ligas de Europa y Argentina en el Mundial de vóleibol', 'Desconocido', 'https://www.lanacion.com.ar/deportes/futbol/la-agenda-de-la-tv-del-lunes-las-ligas-de-europa-y-argentina-en-el-mundial-de-voleibol-nid14092025/', '2025-09-15 05:54:31', 'La actividad deportiva en el inicio de la semana, disponible a través de las pantallas', 'Fútbol', 'https://resizer.glanacion.com/resizer/v2/PJH6F6AAY5FXFN3RJ2GAHOD4SY.jpg?auth=7e030ceb9d86879c1750cc84d6a7afd9ed632ffbabb1040f9e68756c4d2a2d11&smart=true&width=4261&height=2841'),
(36, 'Obras de renovación integral: cierran un acceso a la autopista Dellepiane sentido al centro', 'Desconocido', 'https://www.lanacion.com.ar/sociedad/obras-de-renovacion-integral-cierran-desde-manana-un-acceso-a-la-autopista-dellepiane-sentido-al-nid14092025/', '2025-09-15 05:49:21', 'Los trabajos se realizan primero sobre las colectoras para después empezar con las intervenciones sobre la traza principal, que incorporará un metrobús', 'Sociedad', 'https://resizer.glanacion.com/resizer/v2/A2IIJ4J2Q5DN3FSPEU3GYKNMJE.jpeg?auth=7629acdb8aa72da2df8e03aa75edb67d7f4e67c529d4e4d7522c1e8f502eb6f5&smart=true&width=1600&height=930'),
(37, 'Las posiciones del torneo Clausura, la clasificación a las copas y la lucha por la permanencia', 'Desconocido', 'https://www.lanacion.com.ar/deportes/futbol/las-posiciones-del-torneo-clausura-la-clasificacion-a-las-copas-y-la-lucha-por-la-permanencia-nid14092025/', '2025-09-15 05:47:52', 'Así está el panorama tras ocho fechas del campeonato', 'Fútbol', 'https://resizer.glanacion.com/resizer/v2/UNX6FTZ565BZLAIV5GZRVXOKO4.jpg?auth=fedcb1f37099e5861b21bf5c60c7bf91e0108e2dd788bca1052591060084dfef&smart=true&width=2000&height=1333'),
(38, 'Martín Redrado habló sobre la “interrogante” que definirá si el Gobierno puede o no contener al dólar en la banda', 'Desconocido', 'https://www.lanacion.com.ar/economia/martin-redrado-hablo-sobre-la-interrogante-que-definira-si-el-gobierno-puede-o-no-contener-al-dolar-nid14092025/', '2025-09-15 05:43:47', 'El extitular del Banco Central planteó que la disponibilidad real de divisas será determinante para enfrentar la presión cambiaria y se refirió al papel que cumplirá el FMI', 'Economía', 'https://resizer.glanacion.com/resizer/v2/2YSBBQGIS5D2DNT2FJTLLCNKHU.jpg?auth=a79f24846674bad439b181600215f480527df1b489f4d26a494854e406cdf7e3&smart=true&width=1112&height=743'),
(39, 'Premios Emmy 2025: los mejores looks de la alfombra roja', 'Desconocido', 'https://www.lanacion.com.ar/espectaculos/personajes/premios-emmy-2025-los-mejores-looks-de-la-alfombra-roja-nid14092025/', '2025-09-15 05:39:26', 'Este domingo, las celebrities marcaron tendencia en la 77ª entrega de los Premios Emmy, la ceremonia que reconoce a lo más destacado de la televisión norteamericana', 'Personajes', 'https://resizer.glanacion.com/resizer/v2/X7NTI2NNNFCHJNEYA2AKCUBTBE.JPG?auth=9dbd2d0e2b73dcd979dcccaa72800bd49b8287fcbf8ff28ebcbb7f6e6b4b919a&smart=true&width=2000&height=1333');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `recuperación`
--

CREATE TABLE `recuperación` (
  `Token_Recuperación` varchar(256) NOT NULL,
  `ID_Usuario` int(9) UNSIGNED DEFAULT NULL,
  `ID_Admin` int(10) UNSIGNED DEFAULT NULL,
  `Válido_Hasta` datetime NOT NULL,
  `Rol` varchar(6) NOT NULL COMMENT 'Sirve para diferenciar si quien intenta recuperar el acceso es o no un admin. Como los usuarios van a tener IDs de 6-8 dígitos, y los admins de 3 como máximo, no es necesario tener una tabla aparte.'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `recuperación`
--

INSERT INTO `recuperación` (`Token_Recuperación`, `ID_Usuario`, `ID_Admin`, `Válido_Hasta`, `Rol`) VALUES
('EAT45p/WLAIr0hNwnOxSGx9F92ZBa8QBElvxOxTlKSjB+isvu9v2MX0s3q2j6b7DXLe8oa7HTuV1UPnZEORLAw==', NULL, 4, '2025-09-16 23:17:24', 'admin'),
('XmBn9dbwfahJ9HKjwjEoa6VE+6/o6A8bQam6iGGNQojccD0yk2mImdA1MVHJJTTT3PfMxfTLeoJrUJjqA91VYA==', NULL, 4, '2025-09-16 23:34:04', 'admin'),
('reFoqP2ZwaqZamjwfpS5wpWAS+SveeVBBMRsEUjEMxOz3/ViBhyvYZlYs80JdPgWpJ6Hj33GMLsYISGtngmGvA==', NULL, 4, '2025-09-16 23:45:20', 'admin'),
('u+1rlBQUCrzgLQPaRueD6sfMLS9ySPzpKMn7L35TlFNR8CGsCb/sPCiff8J1dmvSz4aYnntGkAfJIpuuYedEEg==', NULL, 4, '2025-09-16 23:28:50', 'admin'),
('yZQE/Zmn7Yo6u2lJINQEV5XOBeOZ4FeD5v0qaeeaAVJwlfReW7HcgQg8XUD6DH0FXSqOAbEvldZu2kK8XG8wbQ==', NULL, 4, '2025-09-16 23:09:42', 'admin');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Num_Documento` int(9) UNSIGNED NOT NULL COMMENT 'No es auto-incremental porque aquí va el número de DNI/LC/LE.',
  `Tipo_Documento` varchar(3) NOT NULL COMMENT 'Aquí van abreviaciones como "DNI", "LE", y "LC".',
  `Correo` varchar(100) NOT NULL,
  `Teléfono` varchar(14) NOT NULL COMMENT '"+54 9 123 456 7890" son 14 caracteres.',
  `Asociación` varchar(255) NOT NULL COMMENT 'Pendiente: Definir si ésta columna debería hacer referencia a otra tabla.',
  `Nombre` varchar(255) NOT NULL,
  `Clave` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Num_Documento`, `Tipo_Documento`, `Correo`, `Teléfono`, `Asociación`, `Nombre`, `Clave`) VALUES
(0, 'DMY', 'dummy@example.net', '+5492651412843', 'Dummy', 'Dummy', ''),
(7239, 'DMY', 'arico@example.net', '+5492664685713', 'Dummies United', 'Arico', 'V4IFiWi9MARvwypeqBXVM5UwwALAO8NUVj9gWNlQntc='),
(1234567, 'LC', 'jperez@example.net', '+549000123456', 'DOSEP', 'Juan Pérez', 'dfsHw8czf3xXR7STWqG/gxwcEkw4N4igQ5BZTbc/RC0='),
(7654321, 'DNI', 'peres@example.net', '1190988776', 'Dummies United', 'Pedro Peres', 'U/L8KiKHEUaFFE+NocB0ayn2nn/UUEWWTVtZe3uJQGg='),
(16777216, 'DNI', 'dumdum@example.com', '1009885590', 'Dummies United', 'Test Dummy', '77JpPI3KMeUPKLhGplPQQKPcc/uEgFC+wNv0DjI7Aqw='),
(33010203, 'DNI', 'fermin@gmail.com', '2664010203', 'Abuelitos', 'Fermin', 'EEiSt7vQl0R/a5CyhWQaIDbOLOniPtP/wX9Cs2vVPdA=');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `administradores`
--
ALTER TABLE `administradores`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `NombreUsuario` (`Nombre_Usuario`),
  ADD UNIQUE KEY `Correo-Admin` (`Email`),
  ADD KEY `ID_Ministerio` (`ID_Ministerio`);

--
-- Indices de la tabla `eventos`
--
ALTER TABLE `eventos`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `ID_Ministerio` (`ID_Ministerio`);

--
-- Indices de la tabla `inscripciones`
--
ALTER TABLE `inscripciones`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `ID_Inscrito` (`ID_Inscrito`),
  ADD KEY `ID_Evento` (`ID_Evento`);

--
-- Indices de la tabla `ministerios`
--
ALTER TABLE `ministerios`
  ADD PRIMARY KEY (`ID`);

--
-- Indices de la tabla `noticias`
--
ALTER TABLE `noticias`
  ADD PRIMARY KEY (`ID`);

--
-- Indices de la tabla `recuperación`
--
ALTER TABLE `recuperación`
  ADD PRIMARY KEY (`Token_Recuperación`),
  ADD KEY `ID_Usuario` (`ID_Usuario`),
  ADD KEY `ID_Admin` (`ID_Admin`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Num_Documento`),
  ADD UNIQUE KEY `Correo-Usuario` (`Correo`) USING BTREE;

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `administradores`
--
ALTER TABLE `administradores`
  MODIFY `ID` int(9) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `eventos`
--
ALTER TABLE `eventos`
  MODIFY `ID` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `inscripciones`
--
ALTER TABLE `inscripciones`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `ministerios`
--
ALTER TABLE `ministerios`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `noticias`
--
ALTER TABLE `noticias`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=40;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `administradores`
--
ALTER TABLE `administradores`
  ADD CONSTRAINT `Usuario-Ministerio` FOREIGN KEY (`ID_Ministerio`) REFERENCES `ministerios` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `eventos`
--
ALTER TABLE `eventos`
  ADD CONSTRAINT `Ministerio-Evento` FOREIGN KEY (`ID_Ministerio`) REFERENCES `ministerios` (`ID`);

--
-- Filtros para la tabla `inscripciones`
--
ALTER TABLE `inscripciones`
  ADD CONSTRAINT `Evento-Inscripción` FOREIGN KEY (`ID_Evento`) REFERENCES `eventos` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Inscrito-Inscripción` FOREIGN KEY (`ID_Inscrito`) REFERENCES `usuarios` (`Num_Documento`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `recuperación`
--
ALTER TABLE `recuperación`
  ADD CONSTRAINT `FK_Recuperación_Admin` FOREIGN KEY (`ID_Admin`) REFERENCES `administradores` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Recuperación_ibfk_1` FOREIGN KEY (`ID_Usuario`) REFERENCES `usuarios` (`Num_Documento`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
