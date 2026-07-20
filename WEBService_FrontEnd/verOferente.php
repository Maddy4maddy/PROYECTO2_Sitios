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
    <title>AdminPersonal - Ver Oferente</title>
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
            </nav>
        </aside>

        <main class="contenido">

            <div class="titulo-seccion">
                Ver Oferente
                <span class="sub-titulo">Ingrese el código del oferente para ver su información</span>
            </div>

            <!-- Buscador -->
            <div class="busqueda-contenedor">
                <div class="busqueda-form">
                    <label for="codigoOferente">Código del Oferente:</label>
                    <input type="text" id="codigoOferente" 
                           placeholder="Ej: Oferente01, Oferente02..." />
                    <button onclick="buscarOferente()">Buscar</button>
                </div>
                <div style="margin-top:10px;font-size:0.85rem;color:#888;">
                    Ingrese el código del oferente (ej: Oferente01, Oferente02, etc.)
                </div>
            </div>

            <!-- Resultado -->
            <div class="detalle-contenedor" id="detalle-oferente">
                <div class="mensaje-inicial">Ingrese un código para buscar</div>
            </div>

        </main>

    </div>

    <script src="js/verOferente.js"></script>

</body>

</html>