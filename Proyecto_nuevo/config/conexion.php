<?php

$host = "zen-greider.138-59-135-33.plesk.page";
$puerto = "3306";
$usuario = "PiratasCorazon11";
$password = "12328Piratas";
$baseDatos = "tiusr2pl_administracion_personal";

try {
    $conexion = new PDO(
        "mysql:host=$host;port=$puerto;dbname=$baseDatos;charset=utf8mb4",
        $usuario,
        $password
    );

    $conexion->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $conexion->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
    $conexion->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);

} catch (PDOException $e) {
    die("Error de conexión con la base de datos.");
}

?>