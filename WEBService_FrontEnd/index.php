<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header("Location: login.php");
    exit();
}

$nombre = $_SESSION['usuario'];

$inicial = strtoupper(
    substr($nombre, 0, 1)
);
?>

<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1.0"
    >

    <title>AdminPersonal - Inicio</title>

    <link
        rel="stylesheet"
        href="css/styles.css"
    >
</head>

<body>

    <!-- Barra superior -->
    <header class="header">

        <div class="header-right">

            <span>
                <strong>
                    <?php
                    echo htmlspecialchars(
                        $nombre,
                        ENT_QUOTES,
                        "UTF-8"
                    );
                    ?>
                </strong>
            </span>

            <div class="avatar">
                <?php
                echo htmlspecialchars(
                    $inicial,
                    ENT_QUOTES,
                    "UTF-8"
                );
                ?>
            </div>

            <a
                href="logout.php"
                class="btn-logout"
            >
                Cerrar sesión
            </a>

        </div>

    </header>

    <!-- Contenido -->
    <main class="inicio">

        <div class="titulo-principal">

            <h1>
                SISTEMA DE WEBSERVICE
            </h1>

        </div>

        <div class="bienvenida">

            <h2>
                Bienvenido,
                <?php
                echo htmlspecialchars(
                    $nombre,
                    ENT_QUOTES,
                    "UTF-8"
                );
                ?>
            </h2>

            <p>
                Seleccione una de las opciones disponibles.
            </p>

        </div>

        <div class="opciones">

            <!-- CORE 1: Listado de puestos activos -->
            <a
                href="puestos.php"
                class="opcion"
            >
                <h2>
                    Puestos Activos
                </h2>
            </a>

            <!-- CORE 6: Detalle de puestos (buscar por código) -->
            <a
                href="detallePuesto.php"
                class="opcion"
            >
                <h2>
                    Detalle Puestos
                </h2>
            </a>

            <!-- CORE 3: Crear empleado -->
            <a
                href="crearEmpleado.php"
                class="opcion"
            >
                <h2>
                    Crear empleado
                </h2>
            </a>

            <!-- CORE 8: Ver oferente -->
            <a
                href="verOferente.php"
                class="opcion"
            >
                <h2>
                    Ver Oferente
                </h2>
            </a>

        </div>

    </main>

</body>

</html>