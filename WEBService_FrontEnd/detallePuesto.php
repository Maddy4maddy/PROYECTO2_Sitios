<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header("Location: login.php");
    exit();
}

$nombre = $_SESSION['usuario'];
$inicial = strtoupper(substr($nombre, 0, 1));

// OBTENER EL CÓDIGO DE LA URL
$codigoPuesto = isset($_GET['codigo']) ? $_GET['codigo'] : '';


if (empty($codigoPuesto)) {
    header("Location: puestos_core6.php");
    exit();
}
?>
<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>AdminPersonal - Detalle del Puesto</title>
    <link rel="stylesheet" href="css/styles.css">
</head>

<body>

    <header class="header">
        <div class="header-left">
            <span class="user-name"><strong><?php echo htmlspecialchars($nombre, ENT_QUOTES, "UTF-8"); ?></strong></span>
        </div>
        <div class="header-right">
            <a href="logout.php" class="btn-logout">Cerrar sesión</a>
            <div class="avatar"><?php echo htmlspecialchars($inicial, ENT_QUOTES, "UTF-8"); ?></div>
        </div>
    </header>

    <div class="contenedor-principal">

        <aside class="sidebar">
            <div class="logo-sidebar">
                <div class="logo-circulo"></div>
                <h2>AdminPersonal</h2>
            </div>
            <nav class="menu-lateral">
                <a href="index.php">Principal</a>
                <a href="puestos.php">Puestos Activos</a>
                <a href="puestos_core6.php" class="activo">Detalle Puestos</a>
            </nav>
        </aside>

        <main class="contenido">

            <div class="titulo-seccion">
                Detalle del Puesto
                <span class="sub-titulo">Información completa del puesto seleccionado</span>
            </div>

            <div class="detalle-contenedor" id="detalle-puesto">
                <div class="loading">Cargando información del puesto...</div>
            </div>

            <div class="acciones">
                <a href="puestos_core6.php" class="btn-volver">← Volver a la lista</a>
            </div>

        </main>

    </div>

    <script>
        // Variable global con el código del puesto
        var codigoPuesto = '<?php echo $codigoPuesto; ?>';
        console.log(" Código recibido en detallePuesto:", codigoPuesto);
        console.log(" URL completa:", window.location.href);
    </script>
    <script src="js/detallePuesto.js"></script>

</body>

</html>