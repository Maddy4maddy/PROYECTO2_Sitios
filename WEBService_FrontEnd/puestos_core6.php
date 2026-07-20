<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header("Location: login.php");
    exit();
}

$nombre = $_SESSION['usuario'];
$inicial = strtoupper(substr($nombre, 0, 1));
?>
<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>AdminPersonal - Puestos (CORE 6)</title>
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
                Detalle Puestos
                <span class="sub-titulo">Seleccione un puesto para ver sus detalles</span>
            </div>

            <div class="tabla-contenedor">
                <table class="tabla">
                    <thead>
                        <tr>
                            <th>Nombre del Puesto</th>
                        </tr>
                    </thead>
                    <tbody id="puestos-core6-container">
                        <tr>
                            <td class="loading">Cargando puestos activos...</td>
                        </tr>
                    </tbody>
                </table>
            </div>

        </main>

    </div>

    <script src="js/main_core6.js"></script>

</body>

</html>