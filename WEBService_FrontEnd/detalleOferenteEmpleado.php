<?php
session_start();

if (!isset($_SESSION['usuario'])) {
    header("Location: login.php");
    exit();
}

$nombre = $_SESSION['usuario'];
$inicial = strtoupper(substr($nombre, 0, 1));

$codigo = $_GET['codigo'] ?? "";
?>

<!DOCTYPE html>
<html lang="es">

<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">

<title>AdminPersonal - Crear Empleado</title>

<link rel="stylesheet" href="css/styles.css">

</head>

<body>

<header class="header">

    <div class="header-left">
        <span class="user-name">
            <strong>
                <?php echo htmlspecialchars($nombre, ENT_QUOTES, "UTF-8"); ?>
            </strong>
        </span>
    </div>

    <div class="header-right">

        <a href="logout.php" class="btn-logout">
            Cerrar sesión
        </a>

        <div class="avatar">
            <?php echo htmlspecialchars($inicial, ENT_QUOTES, "UTF-8"); ?>
        </div>

    </div>

</header>


<div class="contenedor-principal">


    <aside class="sidebar">

        <div class="logo-sidebar">

            <div class="logo-circulo"></div>

            <h2>
                AdminPersonal
            </h2>

        </div>


        <nav class="menu-lateral">

            <a href="index.php">
                Principal
            </a>

        </nav>

    </aside>



    <main class="contenido">


        <div class="titulo-seccion">

            Crear Empleado

            <span class="sub-titulo">
                Registro del oferente como empleado
            </span>

        </div>



        <div id="detalle-empleado" class="detalle-contenedor">

            <div class="loading">
                Cargando información...
            </div>

        </div>


    </main>


</div>



<script>

const codigoOferente =
"<?php echo htmlspecialchars($codigo, ENT_QUOTES, 'UTF-8'); ?>";

</script>


<script src="js/detalleOferenteEmpleado.js"></script>


</body>

</html>