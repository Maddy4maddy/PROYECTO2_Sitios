<?php
ini_set('display_errors', 1);
error_reporting(E_ALL);

require_once "../config/conexion.php";

if ($_SERVER["REQUEST_METHOD"] !== "POST") {
    header("Location: ../aut2_puestos/");
    exit;
}

$id_puesto = filter_input(INPUT_POST, "id_puesto", FILTER_VALIDATE_INT);

$identificacion = trim($_POST["identificacion"] ?? "");
$tipo_identificacion = trim($_POST["tipo_identificacion"] ?? "");
$nombre_completo = trim($_POST["nombre_completo"] ?? "");
$fecha_nacimiento = trim($_POST["fecha_nacimiento"] ?? "");
$correo = strtolower(trim($_POST["correo"] ?? ""));
$telefono = trim($_POST["telefono"] ?? "");

$tiposIdentificacionPermitidos = [
    "Cédula de identidad",
    "DIMEX",
    "Pasaporte"
];

$rutaArchivo = null;
$archivoGuardado = false;

/*
|--------------------------------------------------------------------------
| Validación de datos obligatorios
|--------------------------------------------------------------------------
*/

if (
    !$id_puesto ||
    $id_puesto <= 0 ||
    $identificacion === "" ||
    $tipo_identificacion === "" ||
    $nombre_completo === "" ||
    $fecha_nacimiento === "" ||
    $correo === "" ||
    $telefono === ""
) {
    die("Todos los campos son obligatorios.");
}

if (!in_array($tipo_identificacion, $tiposIdentificacionPermitidos, true)) {
    die("El tipo de identificación seleccionado no es válido.");
}

if (!filter_var($correo, FILTER_VALIDATE_EMAIL)) {
    die("El formato del correo no es válido.");
}

if (!preg_match('/^[0-9+\-\s]{8,20}$/', $telefono)) {
    die("El formato del teléfono no es válido.");
}

/*
|--------------------------------------------------------------------------
| Validación de fecha de nacimiento
|--------------------------------------------------------------------------
*/

$fechaNacimientoObjeto = DateTime::createFromFormat("Y-m-d", $fecha_nacimiento);
$erroresFecha = DateTime::getLastErrors();

if (
    !$fechaNacimientoObjeto ||
    (
        $erroresFecha !== false &&
        (
            $erroresFecha["warning_count"] > 0 ||
            $erroresFecha["error_count"] > 0
        )
    )
) {
    die("La fecha de nacimiento no es válida.");
}

$fechaActual = new DateTime();

if ($fechaNacimientoObjeto > $fechaActual) {
    die("La fecha de nacimiento no puede ser posterior a la fecha actual.");
}

/*
|--------------------------------------------------------------------------
| Validación del currículum
|--------------------------------------------------------------------------
*/

if (
    !isset($_FILES["curriculum"]) ||
    $_FILES["curriculum"]["error"] !== UPLOAD_ERR_OK
) {
    die("Debe adjuntar el currículum en formato PDF.");
}

$archivo = $_FILES["curriculum"];

$nombreOriginal = $archivo["name"];
$archivoTemporal = $archivo["tmp_name"];
$tamanoArchivo = (int) $archivo["size"];
$tipoMimeReportado = $archivo["type"] ?? "";

$extension = strtolower(pathinfo($nombreOriginal, PATHINFO_EXTENSION));

if ($extension !== "pdf") {
    die("El currículum debe ser únicamente un archivo PDF.");
}

/*
 * Máximo permitido: 5 MB.
 */
$tamanoMaximo = 5 * 1024 * 1024;

if ($tamanoArchivo <= 0 || $tamanoArchivo > $tamanoMaximo) {
    die("El currículum no puede superar los 5 MB.");
}

/*
 * El servidor no tiene habilitada la extensión Fileinfo,
 * por eso se valida el tipo reportado por la carga.
 */
$tiposMimePermitidos = [
    "application/pdf",
    "application/x-pdf",
    "application/octet-stream"
];

if (!in_array($tipoMimeReportado, $tiposMimePermitidos, true)) {
    die("El archivo seleccionado no es un PDF válido.");
}

/*
 * Validación adicional de la firma inicial del PDF.
 * Un archivo PDF verdadero normalmente comienza con %PDF-.
 */
$archivoAbierto = fopen($archivoTemporal, "rb");

if ($archivoAbierto === false) {
    die("No se pudo verificar el archivo PDF.");
}

$firmaPDF = fread($archivoAbierto, 5);
fclose($archivoAbierto);

if ($firmaPDF !== "%PDF-") {
    die("El archivo seleccionado no es un PDF válido.");
}

try {
    $conexion->beginTransaction();

    /*
    |--------------------------------------------------------------------------
    | Comprobar que el puesto existe y está activo
    |--------------------------------------------------------------------------
    */

    $sqlPuesto = "SELECT id
                  FROM puestos
                  WHERE id = :id_puesto
                    AND estado = 'Activo'
                  LIMIT 1";

    $consultaPuesto = $conexion->prepare($sqlPuesto);
    $consultaPuesto->execute([
        ":id_puesto" => $id_puesto
    ]);

    if (!$consultaPuesto->fetch()) {
        throw new Exception(
            "El puesto seleccionado no existe o ya no está activo."
        );
    }

    /*
    |--------------------------------------------------------------------------
    | Buscar coincidencias de identificación, correo o teléfono
    |--------------------------------------------------------------------------
    */

    $sqlValidar = "SELECT
                        identificacion,
                        tipo_identificacion,
                        nombre_completo,
                        fecha_nacimiento,
                        correo,
                        telefono,
                        codigo_oferente
                   FROM oferentes
                   WHERE identificacion = :identificacion
                      OR correo = :correo
                      OR telefono = :telefono";

    $consultaValidar = $conexion->prepare($sqlValidar);
    $consultaValidar->execute([
        ":identificacion" => $identificacion,
        ":correo" => $correo,
        ":telefono" => $telefono
    ]);

    $coincidencias = $consultaValidar->fetchAll();
    $oferenteExistente = null;

    foreach ($coincidencias as $coincidencia) {
        if ($coincidencia["identificacion"] === $identificacion) {
            $oferenteExistente = $coincidencia;
            continue;
        }

        if (strcasecmp(trim($coincidencia["correo"]), $correo) === 0) {
            throw new Exception(
                "El correo electrónico ya pertenece a otro oferente."
            );
        }

        if (trim($coincidencia["telefono"]) === $telefono) {
            throw new Exception(
                "El teléfono ya pertenece a otro oferente."
            );
        }
    }

    /*
    |--------------------------------------------------------------------------
    | Validar los datos del oferente ya existente
    |--------------------------------------------------------------------------
    */

    if ($oferenteExistente) {
        if (
            strcasecmp(
                trim($oferenteExistente["nombre_completo"]),
                $nombre_completo
            ) !== 0 ||
            strcasecmp(
                trim($oferenteExistente["correo"]),
                $correo
            ) !== 0 ||
            trim($oferenteExistente["telefono"]) !== $telefono ||
            $oferenteExistente["tipo_identificacion"] !== $tipo_identificacion ||
            $oferenteExistente["fecha_nacimiento"] !== $fecha_nacimiento
        ) {
            throw new Exception(
                "La identificación ingresada ya está registrada con otros datos."
            );
        }
    }

    /*
    |--------------------------------------------------------------------------
    | Evitar una postulación duplicada al mismo puesto
    |--------------------------------------------------------------------------
    */

    $sqlDuplicada = "SELECT id_postulacion
                     FROM postulaciones_puestos
                     WHERE identificacion = :identificacion
                       AND id_puesto = :id_puesto
                     LIMIT 1";

    $consultaDuplicada = $conexion->prepare($sqlDuplicada);
    $consultaDuplicada->execute([
        ":identificacion" => $identificacion,
        ":id_puesto" => $id_puesto
    ]);

    if ($consultaDuplicada->fetch()) {
        throw new Exception(
            "Esta persona ya se encuentra postulada para el puesto seleccionado."
        );
    }

    /*
    |--------------------------------------------------------------------------
    | Preparar la carpeta en el servidor donde está alojado el AUT
    |--------------------------------------------------------------------------
    */

    $carpetaDestino = dirname(__DIR__) . "/uploads/curriculums/";

    if (!is_dir($carpetaDestino)) {
        if (!mkdir($carpetaDestino, 0755, true)) {
            throw new Exception(
                "No se pudo crear la carpeta para guardar el currículum."
            );
        }
    }

    if (!is_writable($carpetaDestino)) {
        throw new Exception(
            "La carpeta de currículums no tiene permisos de escritura."
        );
    }

    /*
    |--------------------------------------------------------------------------
    | Crear un nombre único para el PDF
    |--------------------------------------------------------------------------
    */

    $identificacionLimpia = preg_replace(
        '/[^A-Za-z0-9]/',
        '',
        $identificacion
    );

    $nombreArchivo = "cv_" .
                     $identificacionLimpia . "_" .
                     date("Ymd_His") . "_" .
                     bin2hex(random_bytes(4)) .
                     ".pdf";

    $rutaArchivo = $carpetaDestino . $nombreArchivo;

    if (!move_uploaded_file($archivoTemporal, $rutaArchivo)) {
        throw new Exception("No se pudo guardar el currículum.");
    }

    $archivoGuardado = true;

    /*
    |--------------------------------------------------------------------------
    | Insertar o actualizar al oferente
    |--------------------------------------------------------------------------
    */

    if (!$oferenteExistente) {
        $codigoOferente = "OFE" . strtoupper(bin2hex(random_bytes(8)));

        $sqlOferente = "INSERT INTO oferentes (
                            identificacion,
                            tipo_identificacion,
                            nombre_completo,
                            fecha_nacimiento,
                            correo,
                            telefono,
                            postulado,
                            codigo_oferente
                        )
                        VALUES (
                            :identificacion,
                            :tipo_identificacion,
                            :nombre_completo,
                            :fecha_nacimiento,
                            :correo,
                            :telefono,
                            1,
                            :codigo_oferente
                        )";

        $consultaOferente = $conexion->prepare($sqlOferente);
        $consultaOferente->execute([
            ":identificacion" => $identificacion,
            ":tipo_identificacion" => $tipo_identificacion,
            ":nombre_completo" => $nombre_completo,
            ":fecha_nacimiento" => $fecha_nacimiento,
            ":correo" => $correo,
            ":telefono" => $telefono,
            ":codigo_oferente" => $codigoOferente
        ]);
    } else {
        $sqlActualizar = "UPDATE oferentes
                          SET postulado = 1
                          WHERE identificacion = :identificacion";

        $consultaActualizar = $conexion->prepare($sqlActualizar);
        $consultaActualizar->execute([
            ":identificacion" => $identificacion
        ]);
    }

    /*
    |--------------------------------------------------------------------------
    | Registrar la postulación
    |--------------------------------------------------------------------------
    */

    $sqlPostulacion = "INSERT INTO postulaciones_puestos (
                            identificacion,
                            id_puesto,
                            curriculum,
                            fecha_postulacion
                       )
                       VALUES (
                            :identificacion,
                            :id_puesto,
                            :curriculum,
                            NOW()
                       )";

    $consultaPostulacion = $conexion->prepare($sqlPostulacion);
    $consultaPostulacion->execute([
        ":identificacion" => $identificacion,
        ":id_puesto" => $id_puesto,
        ":curriculum" => $nombreArchivo
    ]);

    $conexion->commit();

    header("Location: mensaje.php");
    exit;

} catch (Throwable $e) {
    if ($conexion->inTransaction()) {
        $conexion->rollBack();
    }

    /*
     * Si el PDF fue guardado pero falló la base de datos,
     * se elimina para no dejar archivos huérfanos.
     */
    if (
        $archivoGuardado &&
        $rutaArchivo !== null &&
        file_exists($rutaArchivo)
    ) {
        unlink($rutaArchivo);
    }

    die("Error: " . $e->getMessage());
}
?>