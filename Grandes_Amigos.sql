-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Servidor: localhost
-- Tiempo de generación: 09-09-2025 a las 22:22:31
-- Versión del servidor: 10.4.21-MariaDB
-- Versión de PHP: 8.0.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `Grandes_Amigos`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Administradores`
--

CREATE TABLE `Administradores` (
  `ID` int(9) UNSIGNED NOT NULL,
  `Nombre_Usuario` varchar(255) COLLATE utf8_bin NOT NULL,
  `Clave` varchar(100) COLLATE utf8_bin NOT NULL,
  `ID_Ministerio` int(10) UNSIGNED NOT NULL,
  `Email` varchar(100) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `Administradores`
--

INSERT INTO `Administradores` (`ID`, `Nombre_Usuario`, `Clave`, `ID_Ministerio`, `Email`) VALUES
(1, 'Dummy', 'Lm3qrNTqMvJuUsvtB20NUItRh//9VjZjeqAoqA1G1G4=', 1, 'dummy@example.net'),
(2, 'DMY', 'q/sMjWB73dxjPRaYObCucPVt0ubfG/iZ+fMIrPN6pmc=', 1, 'dmy@example.net');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Eventos`
--

CREATE TABLE `Eventos` (
  `ID` int(11) UNSIGNED NOT NULL,
  `Título` varchar(255) COLLATE utf8_bin NOT NULL,
  `Descripción` varchar(4096) COLLATE utf8_bin NOT NULL COMMENT 'Hay que acordar el largo aquí.',
  `Fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `Foto` varchar(255) COLLATE utf8_bin NOT NULL COMMENT 'Aquí se guarda la URL de la foto. El archivo se guarda en el servidor.',
  `ID_Ministerio` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `Eventos`
--

INSERT INTO `Eventos` (`ID`, `Título`, `Descripción`, `Fecha`, `Foto`, `ID_Ministerio`) VALUES
(1, 'Dummy', 'Lorem ipsum dolor sit amet, consector auspiacitng wsapcdmvngijfvjmsefcidnildnftrvun4eil the4rail fthderlih dilm', '2025-04-21 19:00:00', '/Medios/Null.png', 1),
(2, 'Edited Dummy', 'Not the original description.', '2025-04-27 00:00:00', '/Medios/Nada.png', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Inscripciones`
--

CREATE TABLE `Inscripciones` (
  `ID` int(10) UNSIGNED NOT NULL,
  `ID_Evento` int(10) UNSIGNED NOT NULL,
  `ID_Inscrito` int(9) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `Inscripciones`
--

INSERT INTO `Inscripciones` (`ID`, `ID_Evento`, `ID_Inscrito`) VALUES
(2, 2, 0),
(5, 1, 16777216),
(6, 2, 16777216);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Ministerios`
--

CREATE TABLE `Ministerios` (
  `ID` int(10) UNSIGNED NOT NULL,
  `Nombre` varchar(255) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `Ministerios`
--

INSERT INTO `Ministerios` (`ID`, `Nombre`) VALUES
(1, 'Dummy');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Noticias`
--

CREATE TABLE `Noticias` (
  `ID` int(11) NOT NULL,
  `Título` tinytext COLLATE utf8_bin NOT NULL,
  `Autor` varchar(100) COLLATE utf8_bin NOT NULL,
  `Enlace` varchar(1000) COLLATE utf8_bin NOT NULL,
  `Fecha_Publicación` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Contenido` text COLLATE utf8_bin DEFAULT NULL,
  `Categoría` varchar(50) COLLATE utf8_bin DEFAULT 'General',
  `ImagenUrl` varchar(1000) COLLATE utf8_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `Noticias`
--

INSERT INTO `Noticias` (`ID`, `Título`, `Autor`, `Enlace`, `Fecha_Publicación`, `Contenido`, `Categoría`, `ImagenUrl`) VALUES
(1, 'Transportistas para discapacidad en crisis: les pagan $541 por km cuando deberían ser $1400', 'Andy Ferreyra', 'https://www.perfil.com/noticias/cordoba/transportistas-para-discapacidad-en-crisis-les-pagan-541-por-km-cuando-deberian-ser-1400.phtml', '2025-06-04 00:25:08', '<p><img src=\"https://fotos.perfil.com/2025/06/03/trim/540/304/transporte-discapacidad-2-2035988.jpg\" alt=\"transporte-discapacidad-2\" /></p>Familias y transportistas exigen soluciones inmediatas para evitar el recorte de servicios esenciales. El arancel está congelado desde diciembre, por lo que aseguran tener las tarifas más bajas de la historia. <a href=\"https://www.perfil.com/noticias/cordoba/transportistas-para-discapacidad-en-crisis-les-pagan-541-por-km-cuando-deberian-ser-1400.phtml\">Leer más</a>', 'General', NULL),
(2, 'Lorenzo Musetti brilla en Roland Garros 2025: ¿cuánto ganará por su histórica participación?', 'Laleska Villanueva', 'https://www.perfil.com/noticias/deportes/lorenzo-musetti-brilla-en-roland-garros-2025-cuanto-ganara-por-su-historica-participacion.phtml', '2025-06-04 00:20:00', '<p><img src=\"https://fotos.perfil.com/2025/06/03/trim/540/304/lorenzo-musetti-2035825.jpg\" alt=\"Lorenzo Musetti\" /></p>A sus 23 años, el italiano Lorenzo Musetti alcanzó por primera vez las semifinales de Roland Garros y, además del reconocimiento deportivo, se lleva una jugosa recompensa económica que refleja su gran momento en el circuito profesional. <a href=\"https://www.perfil.com/noticias/deportes/lorenzo-musetti-brilla-en-roland-garros-2025-cuanto-ganara-por-su-historica-participacion.phtml\">Leer más</a>', 'General', NULL),
(3, 'Tras el lanzamiento de Cristina Fernández como candidata, Karina Milei mantuvo una sugestiva reunión con José Luis Espert en Casa Rosada', 'Giselle Leclercq', 'https://www.perfil.com/noticias/politica/tras-el-lanzamiento-de-cristina-fernandez-como-candidata-karina-milei-mantuvo-una-sugestiva-reunion-con-jose-luis-espert-en-casa-rosada.phtml', '2025-06-04 01:35:00', '<p><img src=\"https://fotos.perfil.com/2025/06/03/trim/540/304/karina-milei-y-espert-20250603-2035998.jpg\" alt=\"karina Milei y Espert 20250603\" /></p>El Presidente dijo una y otra vez que el diputado será quien encabece la lista en octubre, pero el anuncio de la expresidenta abre todo tipo de especulaciones. ¿Cambia la estrategia de La Libertad Avanza? <a href=\"https://www.perfil.com/noticias/politica/tras-el-lanzamiento-de-cristina-fernandez-como-candidata-karina-milei-mantuvo-una-sugestiva-reunion-con-jose-luis-espert-en-casa-rosada.phtml\">Leer más</a>', 'General', NULL),
(4, 'La construcción le reclamó a Milei por las rutas mientras Transporte lanzó la licitación de 741 km de corredores viales', 'Diego Cirici Quiroga', 'https://www.perfil.com/noticias/politica/la-construccion-le-reclamo-a-milei-por-las-rutas-mientras-transporte-lanzo-la-licitacion-de-741-km-de-corredores-viales.phtml', '2025-06-04 01:34:00', '<p><img src=\"https://fotos.perfil.com/2025/06/02/trim/540/304/obra-publica-2034997.jpg\" alt=\"Obra pública\" /></p>La Cámara del sector advirtió que el modelo actual no garantiza peajes accesibles sin presencia estatal. La Secretaría de Transporte, por su parte, formalizó el llamado para la Etapa I de concesiones de la Red Federal. <a href=\"https://www.perfil.com/noticias/politica/la-construccion-le-reclamo-a-milei-por-las-rutas-mientras-transporte-lanzo-la-licitacion-de-741-km-de-corredores-viales.phtml\">Leer más</a>', 'General', NULL),
(5, 'Tow sobre la candidatura de Cristina Fernández: “Quiere plantarle a Kicillof \'o vamos juntos, o yo arrastro por mi lado\'”', 'Liliana Opanasuk', 'https://www.perfil.com/noticias/canal-e/tow-sobre-la-candidatura-de-crsitina-fernandez-quiere-plantarle-a-kicillof-o-vamos-juntos-o-yo-arrastro-por-mi-lado.phtml', '2025-06-04 01:20:03', '<p><img src=\"https://fotos.perfil.com/2025/06/03/trim/540/304/cristina-fernandez-2036016.jpg\" alt=\"Cristina Fernández\" /></p>La expresidenta encabezará la boleta en la tercera sección de Buenos Aires, apostando al arrastre electoral y tensionando la interna del peronismo. <a href=\"https://www.perfil.com/noticias/canal-e/tow-sobre-la-candidatura-de-crsitina-fernandez-quiere-plantarle-a-kicillof-o-vamos-juntos-o-yo-arrastro-por-mi-lado.phtml\">Leer más</a>', 'General', NULL),
(6, 'Luego de advertir que \"esta noche ocurriría una gran sorpresa\", Irán atacó con misiles supersónicos a Israel', 'Luciana Mina', 'https://www.perfil.com/noticias/internacional/iran-advirito-que-esta-noche-ocurrira-una-gran-sorpresa-que-el-mundo-recordara-durante-siglos.phtml', '2025-06-18 22:35:00', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/sistemas-de-defensa-aerea-israelies-se-activaron-para-interceptar-misiles-iranies-en-tel-aviv-20250617-2044927.jpg\" alt=\"Sistemas de defensa aérea israelíes se activaron para interceptar misiles iraníes en Tel Aviv 20250617\" /></p>El mensaje fue transmitido a través de la televisión estatal, al mismo tiempo que el general iraní Abdolrahim Musavi instó a los civiles a evacuar Tel Aviv y Hifa. De acuerdo a un comunicado de la Guardia Revolucionaria, \"tomaron el control del cielo israelí\". <a href=\"https://www.perfil.com/noticias/internacional/iran-advirito-que-esta-noche-ocurrira-una-gran-sorpresa-que-el-mundo-recordara-durante-siglos.phtml\">Leer más</a>', 'General', NULL),
(7, 'ARCA impide traer un lavarropa o una heladera de Chile, pero ahora se podrá regresar en un auto importado', 'Hernán Martín', 'https://www.perfil.com/noticias/economia/arca-impide-traer-un-lavarropa-o-una-heladera-de-chile-pero-ahora-se-podra-regresar-en-un-auto-importado.phtml', '2025-06-18 22:32:00', '<p><img src=\"https://fotos.perfil.com/2025/06/01/trim/540/304/010625sturzeneggernag-2034295.jpg\" alt=\"010625_sturzenegger_na_g\" /></p>Lo anunció Federico Sturzenegger, ministro de Desregulación y Transformación del Estado de la Nación. Habrá un nuevo esquema basado en la figura del Certificado de Seguridad Vehicular (CSV), que reemplazaría a la LCM en el marco de la ley Nacional de Tránsito.  <a href=\"https://www.perfil.com/noticias/economia/arca-impide-traer-un-lavarropa-o-una-heladera-de-chile-pero-ahora-se-podra-regresar-en-un-auto-importado.phtml\">Leer más</a>', 'General', NULL),
(8, 'Los pasajes en avión a Brasil podrían aumentar 25% en 2026 por una reforma tributaria', 'Luis Machado', 'https://www.perfil.com/noticias/economia/los-pasajes-en-avion-a-brasil-podrian-aumentar-25-en-2026-por-una-reforma-tributaria.phtml', '2025-06-18 22:19:07', '<p><img src=\"https://fotos.perfil.com/2024/07/01/trim/540/304/rio-de-janeiro-con-ninos-1828376.jpg\" alt=\"Rio de Janeiro con niños\" /></p>El país vecino implementará un nuevo esquema de Impuesto al Valor Agregado (IVA) dual, que generaría un incremento para las aerolíneas. <a href=\"https://www.perfil.com/noticias/economia/los-pasajes-en-avion-a-brasil-podrian-aumentar-25-en-2026-por-una-reforma-tributaria.phtml\">Leer más</a>', 'General', NULL),
(9, 'Marcha por Cristina Kirchner: de la unidad al amontonamiento', 'Javier Calvo', 'https://www.perfil.com/noticias/columnistas/marcha-por-cristina-kirchner-de-la-unidad-al-amontonamiento.phtml', '2025-06-18 22:16:00', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/marcha-por-cristina-08-2045319.jpg\" alt=\"MARCHA POR CRISTINA 08\" /></p>Probablemente sea la principal concentración del peronismo desde que CFK dejó el gobierno. Pero cabe preguntarse si la desigual presencia de la dirigencia peronista no augura que la expectativa kirchnerista respecto a la unidad sea apenas una ilusión.  <a href=\"https://www.perfil.com/noticias/columnistas/marcha-por-cristina-kirchner-de-la-unidad-al-amontonamiento.phtml\">Leer más</a>', 'General', NULL),
(10, 'Pedro Sánchez, cada vez más acorralado por los escándalos que estallan a su alrededor', 'Eduardo Reina', 'https://www.perfil.com/noticias/opinion/pedro-sanchez-cada-vez-mas-acorralado-por-los-escandalos-que-estallan-a-su-alrededor.phtml', '2025-06-18 22:15:00', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/pedro-sanchez-cada-vez-mas-acorralado-por-los-escandalos-que-estallan-a-su-alrededor-2045432.jpg\" alt=\"Pedro Sánchez, cada vez más acorralado por los escándalos que estallan a su alrededor\" /></p>El presidente español se presentó ante el Congreso para dar explicaciones por los escándalos de corrupción que acorralan al gobierno y culpó al Partido Popular por hechos del pasado. <a href=\"https://www.perfil.com/noticias/opinion/pedro-sanchez-cada-vez-mas-acorralado-por-los-escandalos-que-estallan-a-su-alrededor.phtml\">Leer más</a>', 'General', NULL),
(11, 'Passalacqua lanzó el “Ahora PyMEs”: “Este programa no es el remedio a todas las enfermedades, pero es un enorme paliativo”', 'José Pérez', 'https://www.perfil.com/noticias/nea/passalacqua-lanzo-el-ahora-pymes-este-programa-no-es-el-remedio-a-todas-las-enfermedades-pero-es-un-enorme-paliativo.phtml', '2025-06-18 22:13:22', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/18-06-2025-passalacqua-misiones-ahora-pymes-2045440.jpg\" alt=\"18-06-2025 Passalacqua Misiones Ahora Pymes\" /></p>\"No existen soluciones mágicas pero sí soluciones creativas y en conjunto\", aseguró el gobernador de Misiones. <a href=\"https://www.perfil.com/noticias/nea/passalacqua-lanzo-el-ahora-pymes-este-programa-no-es-el-remedio-a-todas-las-enfermedades-pero-es-un-enorme-paliativo.phtml\">Leer más</a>', 'General', NULL),
(12, 'QUINIELA de hoy 18 de junio de 2025 EN VIVO: resultados de la Nacional y PROVINCIA', 'Romina Veloso', 'https://www.perfil.com/noticias/juegos/quiniela-de-hoy-18-de-junio-de-2025-en-vivo-resultados-de-la-nacional-y-provincia.phtml', '2025-06-18 22:12:00', '<p><img src=\"https://fotos.perfil.com/2023/12/18/trim/540/304/sorteos-de-quiniela-1721593.jpg\" alt=\"Sorteos de quiniela\" /></p>Conoce los números ganadores de la quiniela hoy, 18 de junio de 2025. ¿Cuáles encabezaron cada tanda? <a href=\"https://www.perfil.com/noticias/juegos/quiniela-de-hoy-18-de-junio-de-2025-en-vivo-resultados-de-la-nacional-y-provincia.phtml\">Leer más</a>', 'General', NULL),
(13, '\"Vamos a volver\", dólares alquilados y \"saben que pierden\": cinco frases de Cristina Kirchner en su mensaje a la militancia', 'Gabriel Irungaray', 'https://www.perfil.com/noticias/politica/estoy-en-mi-casa-firme-y-tranquila-cinco-frases-de-cristina-kirchner-en-su-mensaje-a-la-militancia-en-plaza-de-mayo.phtml', '2025-06-18 22:05:00', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/cristina-kirchner-2045105.jpg\" alt=\"Cristina Kirchner\" /></p>Mientras cumple prisión domiciliaria y en medio de una multitudinaria movilización en su apoyo, la ex presidenta envió un mensaje en el que sostuvo: \"¿En serio alguien puede pensar que este modelo es sostenible en el tiempo?\".  <a href=\"https://www.perfil.com/noticias/politica/estoy-en-mi-casa-firme-y-tranquila-cinco-frases-de-cristina-kirchner-en-su-mensaje-a-la-militancia-en-plaza-de-mayo.phtml\">Leer más</a>', 'General', NULL),
(14, 'Marcha por Cristina Kirchner: habló la expresidenta en medio de un fuerte operativo y una gran concentración de militantes', 'Felipe Leibovich', 'https://www.perfil.com/noticias/politica/marcha-por-cristina-kirchner-minuto-a-minuto-la-movilizacion-a-plaza-de-mayo.phtml', '2025-06-18 22:00:00', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/marcha-contra-la-condena-a-cristina-kirchner-2045138.jpg\" alt=\"Marcha contra la condena a Cristina Kirchner\" /></p>Mientras cumple la prisión domiciliaria en su departamento de Constitución, envió un mensaje de voz para que se transmita en Plaza de Mayo, donde se concentró una masiva movilización.  <a href=\"https://www.perfil.com/noticias/politica/marcha-por-cristina-kirchner-minuto-a-minuto-la-movilizacion-a-plaza-de-mayo.phtml\">Leer más</a>', 'General', NULL),
(15, 'Incertidumbre por la prisión domiciliaria de Cristina Kirchner: “La casa de Cristina hoy es una celda”', 'Alejandro Dubini', 'https://www.perfil.com/noticias/canal-e/incertidumbre-por-la-presion-domiciliaria-de-cristina-kirchner-la-casa-de-cristina-hoy-es-una-celda-afirmo-un-abogado-penalista.phtml', '2025-06-18 21:57:42', '<p><img src=\"https://fotos.perfil.com/2025/06/18/trim/540/304/cristina-kirchner-2045434.jpg\" alt=\"Cristina Kirchner\" /></p>Según expresó el abogado penalista, Gastón Francone, “la prisión domiciliaria es un beneficio y no es un derecho”. <a href=\"https://www.perfil.com/noticias/canal-e/incertidumbre-por-la-presion-domiciliaria-de-cristina-kirchner-la-casa-de-cristina-hoy-es-una-celda-afirmo-un-abogado-penalista.phtml\">Leer más</a>', 'General', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Recuperación`
--

CREATE TABLE `Recuperación` (
  `Token_Recuperación` varchar(64) COLLATE utf8_bin NOT NULL,
  `ID_Usuario` int(9) UNSIGNED NOT NULL,
  `Válido_Hasta` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Rol` varchar(6) COLLATE utf8_bin NOT NULL COMMENT 'Sirve para diferenciar si quien intenta recuperar el acceso es o no un admin. Como los usuarios van a tener IDs de 6-8 dígitos, y los admins de 3 como máximo, no es necesario tener una tabla aparte.'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Usuarios`
--

CREATE TABLE `Usuarios` (
  `Num_Documento` int(9) UNSIGNED NOT NULL COMMENT 'No es auto-incremental porque aquí va el número de DNI/LC/LE.',
  `Tipo_Documento` varchar(3) COLLATE utf8_bin NOT NULL COMMENT 'Aquí van abreviaciones como "DNI", "LE", y "LC".',
  `Correo` varchar(100) COLLATE utf8_bin NOT NULL,
  `Teléfono` varchar(14) COLLATE utf8_bin NOT NULL COMMENT '"+54 9 123 456 7890" son 14 caracteres.',
  `Asociación` varchar(255) COLLATE utf8_bin NOT NULL COMMENT 'Pendiente: Definir si ésta columna debería hacer referencia a otra tabla.',
  `Nombre` varchar(255) COLLATE utf8_bin NOT NULL,
  `Clave` varchar(255) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Volcado de datos para la tabla `Usuarios`
--

INSERT INTO `Usuarios` (`Num_Documento`, `Tipo_Documento`, `Correo`, `Teléfono`, `Asociación`, `Nombre`, `Clave`) VALUES
(0, 'DMY', 'dummy@example.net', '+5492651412843', 'Dummy', 'Dummy', ''),
(7239, 'DMY', 'arico@example.net', '+5492664685713', 'Dummies United', 'Arico', 'V4IFiWi9MARvwypeqBXVM5UwwALAO8NUVj9gWNlQntc='),
(1234567, 'LC', 'jperez@example.net', '+549000123456', 'DOSEP', 'Juan Pérez', 'dfsHw8czf3xXR7STWqG/gxwcEkw4N4igQ5BZTbc/RC0='),
(7654321, 'DNI', 'peres@example.net', '1190988776', 'Dummies United', 'Pedro Peres', 'U/L8KiKHEUaFFE+NocB0ayn2nn/UUEWWTVtZe3uJQGg='),
(16777216, 'DNI', 'dumdum@example.com', '1009885590', 'Dummies United', 'Test Dummy', '77JpPI3KMeUPKLhGplPQQKPcc/uEgFC+wNv0DjI7Aqw=');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `Administradores`
--
ALTER TABLE `Administradores`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `NombreUsuario` (`Nombre_Usuario`),
  ADD UNIQUE KEY `Correo-Admin` (`Email`),
  ADD KEY `ID_Ministerio` (`ID_Ministerio`);

--
-- Indices de la tabla `Eventos`
--
ALTER TABLE `Eventos`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `ID_Ministerio` (`ID_Ministerio`);

--
-- Indices de la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `ID_Inscrito` (`ID_Inscrito`),
  ADD KEY `ID_Evento` (`ID_Evento`);

--
-- Indices de la tabla `Ministerios`
--
ALTER TABLE `Ministerios`
  ADD PRIMARY KEY (`ID`);

--
-- Indices de la tabla `Noticias`
--
ALTER TABLE `Noticias`
  ADD PRIMARY KEY (`ID`);

--
-- Indices de la tabla `Recuperación`
--
ALTER TABLE `Recuperación`
  ADD PRIMARY KEY (`Token_Recuperación`),
  ADD KEY `ID_Usuario` (`ID_Usuario`);

--
-- Indices de la tabla `Usuarios`
--
ALTER TABLE `Usuarios`
  ADD PRIMARY KEY (`Num_Documento`),
  ADD UNIQUE KEY `Correo-Usuario` (`Correo`) USING BTREE;

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `Administradores`
--
ALTER TABLE `Administradores`
  MODIFY `ID` int(9) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `Eventos`
--
ALTER TABLE `Eventos`
  MODIFY `ID` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `Ministerios`
--
ALTER TABLE `Ministerios`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `Noticias`
--
ALTER TABLE `Noticias`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `Administradores`
--
ALTER TABLE `Administradores`
  ADD CONSTRAINT `Usuario-Ministerio` FOREIGN KEY (`ID_Ministerio`) REFERENCES `Ministerios` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `Eventos`
--
ALTER TABLE `Eventos`
  ADD CONSTRAINT `Ministerio-Evento` FOREIGN KEY (`ID_Ministerio`) REFERENCES `Ministerios` (`ID`);

--
-- Filtros para la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  ADD CONSTRAINT `Evento-Inscripción` FOREIGN KEY (`ID_Evento`) REFERENCES `Eventos` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Inscrito-Inscripción` FOREIGN KEY (`ID_Inscrito`) REFERENCES `Usuarios` (`Num_Documento`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `Recuperación`
--
ALTER TABLE `Recuperación`
  ADD CONSTRAINT `Admin-Recuperación` FOREIGN KEY (`ID_Usuario`) REFERENCES `Administradores` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Recuperación_ibfk_1` FOREIGN KEY (`ID_Usuario`) REFERENCES `Usuarios` (`Num_Documento`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
