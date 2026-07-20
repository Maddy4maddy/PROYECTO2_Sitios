<?php
session_start();

if (!isset($_SESSION["usuario"])) {
    header("Location: login.php");
    exit();
}

$nombre = $_SESSION["usuario"];

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

    <title>
        AdminPersonal - Crear empleado
    </title>

    <link
        rel="stylesheet"
        href="css/styles.css"
    >

    <style>
        .formulario-contenedor {
            background: #ffffff;
            border-radius: 12px;
            padding: 32px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, .2);
        }

        .formulario-empleado {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 22px;
        }

        .grupo-formulario {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .grupo-formulario.ancho-completo {
            grid-column: 1 / -1;
        }

        .grupo-formulario label {
            font-weight: bold;
            font-size: 16px;
        }

        .grupo-formulario input,
        .grupo-formulario select {
            width: 100%;
            padding: 13px;
            border: 1px solid #b8b8b8;
            border-radius: 7px;
            background: #ffffff;
            font-size: 16px;
        }

        .grupo-formulario input:focus,
        .grupo-formulario select:focus {
            border-color: #2d1866;
            outline: none;
            box-shadow: 0 0 4px rgba(45, 24, 102, .35);
        }

        .acciones-formulario {
            grid-column: 1 / -1;
            display: flex;
            gap: 14px;
            margin-top: 10px;
        }

        .btn-crear,
        .btn-cancelar {
            display: inline-block;
            padding: 13px 22px;
            border: none;
            border-radius: 7px;
            font-size: 16px;
            font-weight: bold;
            text-decoration: none;
            cursor: pointer;
        }

        .btn-crear {
            background: #2d1866;
            color: #ffffff;
        }

        .btn-crear:hover {
            background: #42258c;
        }

        .btn-crear:disabled {
            background: #777777;
            cursor: not-allowed;
        }

        .btn-cancelar {
            background: #747474;
            color: #ffffff;
        }

        .btn-cancelar:hover {
            background: #555555;
        }

        .mensaje-formulario {
            grid-column: 1 / -1;
            display: none;
            padding: 16px;
            border-radius: 7px;
            font-size: 16px;
            line-height: 1.5;
        }

        .mensaje-formulario.exito {
            display: block;
            background: #dff5e5;
            border-left: 5px solid #198754;
            color: #125d38;
        }

        .mensaje-formulario.error {
            display: block;
            background: #fde2e5;
            border-left: 5px solid #c82333;
            color: #9d1723;
        }

        .mensaje-formulario.cargando {
            display: block;
            background: #ece8f8;
            border-left: 5px solid #2d1866;
            color: #2d1866;
        }

        @media (max-width: 800px) {
            .formulario-empleado {
                grid-template-columns: 1fr;
            }

            .grupo-formulario.ancho-completo,
            .acciones-formulario,
            .mensaje-formulario {
                grid-column: 1;
            }

            .acciones-formulario {
                flex-direction: column;
            }

            .btn-crear,
            .btn-cancelar {
                width: 100%;
                text-align: center;
            }
        }
    </style>
</head>

<body>

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
            Crear nuevo empleado
        </div>

        <div class="formulario-contenedor">

            <form
                id="formCrearEmpleado"
                class="formulario-empleado"
                novalidate
            >

                <div class="grupo-formulario">

                    <label for="numeroEmpleado">
                        Número de empleado:
                    </label>

                    <input
                        type="text"
                        id="numeroEmpleado"
                        name="numeroEmpleado"
                        maxlength="30"
                        placeholder="Ejemplo: EMP-0001"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="identificacion">
                        Identificación:
                    </label>

                    <input
                        type="text"
                        id="identificacion"
                        name="identificacion"
                        maxlength="50"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="tipoIdentificacion">
                        Tipo de identificación:
                    </label>

                    <select
                        id="tipoIdentificacion"
                        name="tipoIdentificacion"
                        required
                    >

                        <option value="">
                            Seleccione
                        </option>

                        <option value="Cédula de identidad">
                            Cédula de identidad
                        </option>

                        <option value="DIMEX">
                            DIMEX
                        </option>

                        <option value="Pasaporte">
                            Pasaporte
                        </option>

                    </select>

                </div>

                <div class="grupo-formulario">

                    <label for="nombreCompleto">
                        Nombre completo:
                    </label>

                    <input
                        type="text"
                        id="nombreCompleto"
                        name="nombreCompleto"
                        maxlength="150"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="fechaNacimiento">
                        Fecha de nacimiento:
                    </label>

                    <input
                        type="date"
                        id="fechaNacimiento"
                        name="fechaNacimiento"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="correo">
                        Correo electrónico:
                    </label>

                    <input
                        type="email"
                        id="correo"
                        name="correo"
                        maxlength="150"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="telefono">
                        Teléfono:
                    </label>

                    <input
                        type="text"
                        id="telefono"
                        name="telefono"
                        maxlength="20"
                        placeholder="Ejemplo: 88888888"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="idPuesto">
                        Puesto:
                    </label>

                    <select
                        id="idPuesto"
                        name="idPuesto"
                        required
                    >

                        <option value="">
                            Cargando puestos activos...
                        </option>

                    </select>

                </div>

                <div class="grupo-formulario">

                    <label for="fechaContratacion">
                        Fecha de contratación:
                    </label>

                    <input
                        type="date"
                        id="fechaContratacion"
                        name="fechaContratacion"
                        required
                    >

                </div>

                <div class="grupo-formulario">

                    <label for="estado">
                        Estado:
                    </label>

                    <select
                        id="estado"
                        name="estado"
                        required
                    >

                        <option value="Activo">
                            Activo
                        </option>

                        <option value="Inactivo">
                            Inactivo
                        </option>

                    </select>

                </div>

                <div
                    id="mensajeEmpleado"
                    class="mensaje-formulario"
                    role="alert"
                ></div>

                <div class="acciones-formulario">

                    <button
                        type="submit"
                        id="btnCrearEmpleado"
                        class="btn-crear"
                    >
                        Crear empleado
                    </button>

                    <a
                        href="index.php"
                        class="btn-cancelar"
                    >
                        Cancelar
                    </a>

                </div>

            </form>

        </div>

    </main>

</div>

<script src="js/crearEmpleado.js"></script>

</body>
</html>