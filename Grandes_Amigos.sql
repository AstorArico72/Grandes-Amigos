-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Servidor: localhost
-- Tiempo de generación: 17-04-2025 a las 01:24:15
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
CREATE DATABASE IF NOT EXISTS `Grandes_Amigos` DEFAULT CHARACTER SET utf8 COLLATE utf8_bin;
USE `Grandes_Amigos`;

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

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Inscripciones`
--

CREATE TABLE `Inscripciones` (
  `ID_Evento` int(10) UNSIGNED NOT NULL,
  `ID_Inscrito` int(9) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Inscritos`
--

CREATE TABLE `Inscritos` (
  `Num_Documento` int(9) UNSIGNED NOT NULL COMMENT 'No es auto-incremental porque aquí va el número de DNI/LC/LE.',
  `Tipo_Documento` varchar(3) COLLATE utf8_bin NOT NULL COMMENT 'Aquí van abreviaciones como "DNI", "LE", y "LC".',
  `Correo` varchar(100) COLLATE utf8_bin NOT NULL COMMENT 'Pendiente: Acordar si es "nuleable" o no.',
  `Teléfono` varchar(14) COLLATE utf8_bin NOT NULL COMMENT '"+54 9 123 456 7890" son 14 caracteres.',
  `Asociación` varchar(255) COLLATE utf8_bin NOT NULL COMMENT 'Pendiente: Definir si ésta columna debería hacer referencia a otra tabla.',
  `Nombre` varchar(255) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Ministerios`
--

CREATE TABLE `Ministerios` (
  `ID` int(10) UNSIGNED NOT NULL,
  `Nombre` varchar(255) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Usuarios`
--

CREATE TABLE `Usuarios` (
  `ID` int(10) UNSIGNED NOT NULL,
  `Nombre_Usuario` varchar(255) COLLATE utf8_bin NOT NULL,
  `Clave` varchar(100) COLLATE utf8_bin NOT NULL,
  `ID_Ministerio` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Índices para tablas volcadas
--

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
  ADD KEY `ID_Inscrito` (`ID_Inscrito`),
  ADD KEY `ID_Evento` (`ID_Evento`);

--
-- Indices de la tabla `Inscritos`
--
ALTER TABLE `Inscritos`
  ADD PRIMARY KEY (`Num_Documento`);

--
-- Indices de la tabla `Ministerios`
--
ALTER TABLE `Ministerios`
  ADD PRIMARY KEY (`ID`);

--
-- Indices de la tabla `Usuarios`
--
ALTER TABLE `Usuarios`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `NombreUsuario` (`Nombre_Usuario`),
  ADD KEY `ID_Ministerio` (`ID_Ministerio`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `Eventos`
--
ALTER TABLE `Eventos`
  MODIFY `ID` int(11) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Ministerios`
--
ALTER TABLE `Ministerios`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `Usuarios`
--
ALTER TABLE `Usuarios`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `Eventos`
--
ALTER TABLE `Eventos`
  ADD CONSTRAINT `Ministerio-Evento` FOREIGN KEY (`ID_Ministerio`) REFERENCES `Ministerios` (`ID`);

--
-- Filtros para la tabla `Inscripciones`
--
ALTER TABLE `Inscripciones`
  ADD CONSTRAINT `Evento-Inscripción` FOREIGN KEY (`ID_Evento`) REFERENCES `Eventos` (`ID`),
  ADD CONSTRAINT `Inscrito-Inscripción` FOREIGN KEY (`ID_Inscrito`) REFERENCES `Inscritos` (`Num_Documento`);

--
-- Filtros para la tabla `Usuarios`
--
ALTER TABLE `Usuarios`
  ADD CONSTRAINT `Usuario-Ministerio` FOREIGN KEY (`ID_Ministerio`) REFERENCES `Ministerios` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
