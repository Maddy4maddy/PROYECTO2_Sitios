<?php
ini_set("display_errors", "1");
error_reporting(E_ALL);


$puestos = [];
$errorServicio = "";

$host = "zen-greider.138-59-135-33.plesk.page";
$puerto = "3306";
$baseDatos = "tiusr2pl_administracion_personal";
$usuario = "PiratasCorazon11";
$contrasena = "12328Piratas";

try {
    $conexionCore1 = new PDO(
        "mysql:host={$host};port={$puerto};dbname={$baseDatos};charset=utf8mb4",
        $usuario,
        $contrasena,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_TIMEOUT => 15
        ]
    );

    $sql = "
        SELECT
            id,
            codigo_puesto,
            nombre_puesto,
            salario,
            estado,
            fecha_creacion
        FROM puestos
        WHERE estado = 'Activo'
        ORDER BY nombre_puesto
    ";

    $consulta = $conexionCore1->prepare($sql);
    $consulta->execute();

    $puestos = $consulta->fetchAll();

} catch (PDOException $e) {
    $errorServicio =
        "No fue posible obtener los puestos activos del Core1. " .
        "Compruebe la conexión con la base de datos del servicio.";
}

/**
 * Muestra texto de forma segura en HTML.
 */
function escapar($valor)
{
    return htmlspecialchars(
        (string) $valor,
        ENT_QUOTES,
        "UTF-8"
    );
}
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">

    <title>Core1 - Puestos activos</title>

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1.0"
    >

    <link
        rel="stylesheet"
        href="../assets/css/portal.css"
    >
</head>

<body>

<aside class="sidebar">

    <div class="logo-circulo"></div>

    <h1>AdminPersonal</h1>

    <nav class="menu-lateral">

        <a href="../">
            Principal
        </a>

        <a href="../quienes-somos/">
            ¿Quiénes somos?
        </a>

        <a href="../nuestros-valores-fundamentales/">
            Nuestros valores fundamentales
        </a>

        <a href="../donde-nos-ubicamos/">
            ¿Dónde nos ubicamos?
        </a>

        <a href="../nuestros-servicios/">
            Nuestros servicios
        </a>

        <a href="../nuestro-equipo/">
            Nuestro equipo
        </a>

        <a href="../compromiso-con-la-comunidad/">
            Compromiso con la comunidad
        </a>

        <a href="../unete-a-nosotros/">
            Únete a nosotros
        </a>

        <a href="../beneficios-de-trabajar-con-nosotros/">
            Beneficios de trabajar con nosotros
        </a>

        <a href="../aut2_puestos/">
            Puestos disponibles
        </a>

    </nav>

</aside>

<div class="contenido-principal">

    <header class="barra-superior">

        <span>Admin</span>

        <div class="avatar-admin"></div>

    </header>

    <main class="area-contenido">

        <div class="titulo-morado">
            -CORE 1: PUESTOS ACTIVOS
        </div>

        <section class="tarjeta-contenido">

            <h2>Listado de puestos activos</h2>

            <p class="intro">
                Puestos activos registrados en el sistema de Recursos Humanos.
            </p>

            <?php if ($errorServicio !== ""): ?>

                <div class="mensaje-servicio-error">

                    <h3>No se pudo cargar la información</h3>

                    <p>
                        <?php echo escapar($errorServicio); ?>
                    </p>

                    <a
                        href="./"
                        class="btn-postular"
                    >
                        Intentar nuevamente
                    </a>

                </div>

            <?php elseif (count($puestos) > 0): ?>

                <div class="lista-puestos">

                    <?php foreach ($puestos as $puesto): ?>

                        <?php
                        $id = isset($puesto["id"])
                            ? (int) $puesto["id"]
                            : 0;

                        $codigo = trim(
                            (string) ($puesto["codigo_puesto"] ?? "")
                        );

                        $nombre = trim(
                            (string) ($puesto["nombre_puesto"] ?? "")
                        );

                        $salario = $puesto["salario"] ?? null;

                        $estado = trim(
                            (string) ($puesto["estado"] ?? "")
                        );

                        $fechaCreacion = trim(
                            (string) ($puesto["fecha_creacion"] ?? "")
                        );
                        ?>

                        <article class="puesto-item">

                            <h3>
                                <span class="nombre-puesto-core">
                                    <?php
                                    echo escapar(
                                        $nombre !== ""
                                            ? $nombre
                                            : "Puesto sin nombre"
                                    );
                                    ?>
                                </span>
                            </h3>

                            <?php if ($id > 0): ?>

                                <p>
                                    <strong>ID:</strong>
                                    <?php echo escapar($id); ?>
                                </p>

                            <?php endif; ?>

                            <?php if ($codigo !== ""): ?>

                                <p>
                                    <strong>Código:</strong>
                                    <?php echo escapar($codigo); ?>
                                </p>

                            <?php endif; ?>

                            <?php if (is_numeric($salario)): ?>

                                <p>
                                    <strong>Salario:</strong>

                                    ₡<?php
                                    echo number_format(
                                        (float) $salario,
                                        2,
                                        ",",
                                        "."
                                    );
                                    ?>
                                </p>

                            <?php endif; ?>

                            <?php if ($estado !== ""): ?>

                                <p>
                                    <strong>Estado:</strong>

                                    <span class="estado-activo">
                                        <?php echo escapar($estado); ?>
                                    </span>
                                </p>

                            <?php endif; ?>

                            <?php if ($fechaCreacion !== ""): ?>

                                <p class="fecha">
                                    <strong>Fecha de creación:</strong>

                                    <?php echo escapar($fechaCreacion); ?>
                                </p>

                            <?php endif; ?>

                        </article>

                    <?php endforeach; ?>

                </div>

            <?php else: ?>

                <div class="mensaje-vacio">
                    No existen puestos activos registrados.
                </div>

            <?php endif; ?>

        </section>

    </main>

</div>

</body>
</html>