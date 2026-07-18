/**
 * crearEmpleado.js
 *
 * Carga los puestos activos directamente desde Core1
 * y registra empleados directamente mediante Core3.
 */

// =====================================================
// DIRECCIONES DE LOS WEB SERVICES
// =====================================================

const URL_PUESTOS_CORE1 =
    "http://localhost:61932/WEBServiceCORE1.svc/ObtenerPuestosActivos";

const URL_CREAR_EMPLEADO_CORE3 =
    "http://localhost:61932/WEBServiceCORE3.svc/CrearEmpleado";


// =====================================================
// INICIO
// =====================================================

document.addEventListener(
    "DOMContentLoaded",
    function () {

        configurarFechas();

        cargarPuestosActivos();

        const formulario =
            document.getElementById(
                "formCrearEmpleado"
            );

        if (formulario) {
            formulario.addEventListener(
                "submit",
                crearEmpleado
            );
        }
    }
);


// =====================================================
// CONFIGURAR FECHAS
// =====================================================

function configurarFechas() {

    const hoy =
        new Date()
            .toISOString()
            .split("T")[0];

    const fechaNacimiento =
        document.getElementById(
            "fechaNacimiento"
        );

    const fechaContratacion =
        document.getElementById(
            "fechaContratacion"
        );

    if (fechaNacimiento) {
        fechaNacimiento.max = hoy;
    }

    if (fechaContratacion) {
        fechaContratacion.value = hoy;
    }
}


// =====================================================
// CARGAR PUESTOS DIRECTAMENTE DESDE CORE1
// =====================================================

async function cargarPuestosActivos() {

    const selectPuestos =
        document.getElementById(
            "idPuesto"
        );

    if (!selectPuestos) {
        return;
    }

    selectPuestos.disabled = true;

    selectPuestos.innerHTML = `
        <option value="">
            Cargando puestos activos...
        </option>
    `;

    try {

        const response =
            await fetch(
                URL_PUESTOS_CORE1,
                {
                    method: "GET",
                    mode: "cors",

                    headers: {
                        "Accept": "application/json"
                    }
                }
            );

        if (!response.ok) {
            throw new Error(
                `Core1 respondió con HTTP ${response.status}.`
            );
        }

        const texto =
            await response.text();

        const puestos =
            convertirRespuestaPuestos(
                texto
            );

        selectPuestos.innerHTML = `
            <option value="">
                Seleccione un puesto
            </option>
        `;

        if (
            !Array.isArray(puestos) ||
            puestos.length === 0
        ) {
            selectPuestos.innerHTML = `
                <option value="">
                    No existen puestos activos
                </option>
            `;

            mostrarMensaje(
                "Core1 no devolvió puestos activos.",
                "error"
            );

            return;
        }

        puestos.forEach(
            function (puesto) {

                const id =
                    puesto.Id ??
                    puesto.id ??
                    "";

                const codigo =
                    puesto.CodigoPuesto ??
                    puesto.codigoPuesto ??
                    puesto.codigo_puesto ??
                    "";

                const nombre =
                    puesto.NombrePuesto ??
                    puesto.nombrePuesto ??
                    puesto.nombre_puesto ??
                    "Puesto sin nombre";

                if (
                    id === "" ||
                    Number(id) <= 0
                ) {
                    return;
                }

                const opcion =
                    document.createElement(
                        "option"
                    );

                opcion.value =
                    String(id);

                opcion.textContent =
                    codigo !== ""
                        ? `${codigo} - ${nombre}`
                        : nombre;

                selectPuestos.appendChild(
                    opcion
                );
            }
        );

        if (
            selectPuestos.options.length <= 1
        ) {
            selectPuestos.innerHTML = `
                <option value="">
                    No existen puestos válidos
                </option>
            `;

            return;
        }

        selectPuestos.disabled = false;

    } catch (error) {

        console.error(
            "Error al cargar los puestos:",
            error
        );

        selectPuestos.innerHTML = `
            <option value="">
                Error al cargar puestos
            </option>
        `;

        mostrarMensaje(
            "No fue posible cargar los puestos activos desde Core1.",
            "error"
        );
    }
}


// =====================================================
// CONVERTIR RESPUESTA DE CORE1
// =====================================================

function convertirRespuestaPuestos(texto) {

    const contenido =
        texto.trim();

    if (contenido === "") {
        return [];
    }

    if (contenido.startsWith("<")) {
        return convertirXMLPuestos(
            contenido
        );
    }

    const datos =
        JSON.parse(contenido);

    if (
        datos &&
        Array.isArray(
            datos.ObtenerPuestosActivosResult
        )
    ) {
        return datos
            .ObtenerPuestosActivosResult;
    }

    if (
        datos &&
        Array.isArray(datos.d)
    ) {
        return datos.d;
    }

    if (
        datos &&
        typeof datos.d === "string"
    ) {

        const contenidoD =
            JSON.parse(datos.d);

        return Array.isArray(contenidoD)
            ? contenidoD
            : [];
    }

    return Array.isArray(datos)
        ? datos
        : [];
}


// =====================================================
// CONVERTIR XML DE CORE1
// =====================================================

function convertirXMLPuestos(xmlTexto) {

    const parser =
        new DOMParser();

    const xml =
        parser.parseFromString(
            xmlTexto,
            "text/xml"
        );

    const errorXML =
        xml.querySelector(
            "parsererror"
        );

    if (errorXML) {
        return [];
    }

    const elementos =
        xml.getElementsByTagName(
            "PuestoDTO"
        );

    const puestos = [];

    for (
        let i = 0;
        i < elementos.length;
        i++
    ) {

        const elemento =
            elementos[i];

        puestos.push({
            Id:
                obtenerTextoXML(
                    elemento,
                    "Id"
                ),

            CodigoPuesto:
                obtenerTextoXML(
                    elemento,
                    "CodigoPuesto"
                ),

            NombrePuesto:
                obtenerTextoXML(
                    elemento,
                    "NombrePuesto"
                )
        });
    }

    return puestos;
}


function obtenerTextoXML(
    elemento,
    nombreEtiqueta
) {

    const etiquetas =
        elemento.getElementsByTagName(
            nombreEtiqueta
        );

    if (
        !etiquetas ||
        etiquetas.length === 0
    ) {
        return "";
    }

    return etiquetas[0].textContent ?? "";
}


// =====================================================
// CREAR EMPLEADO DIRECTAMENTE MEDIANTE CORE3
// =====================================================

async function crearEmpleado(event) {

    event.preventDefault();

    limpiarMensaje();

    const formulario =
        event.currentTarget;

    const boton =
        document.getElementById(
            "btnCrearEmpleado"
        );

    if (!formulario.checkValidity()) {
        formulario.reportValidity();
        return;
    }

    const datosEmpleado = {
        NumeroEmpleado:
            obtenerValor(
                "numeroEmpleado"
            ),

        Identificacion:
            obtenerValor(
                "identificacion"
            ),

        TipoIdentificacion:
            obtenerValor(
                "tipoIdentificacion"
            ),

        NombreCompleto:
            obtenerValor(
                "nombreCompleto"
            ),

        FechaNacimiento:
            obtenerValor(
                "fechaNacimiento"
            ),

        Correo:
            obtenerValor(
                "correo"
            ),

        Telefono:
            obtenerValor(
                "telefono"
            ),

        IdPuesto:
            Number(
                obtenerValor(
                    "idPuesto"
                )
            ),

        FechaContratacion:
            obtenerValor(
                "fechaContratacion"
            ),

        Estado:
            obtenerValor(
                "estado"
            )
    };

    const errorValidacion =
        validarDatosEmpleado(
            datosEmpleado
        );

    if (errorValidacion !== "") {
        mostrarMensaje(
            errorValidacion,
            "error"
        );

        return;
    }

    if (boton) {
        boton.disabled = true;

        boton.textContent =
            "Creando empleado...";
    }

    mostrarMensaje(
        "Procesando la información del empleado...",
        "cargando"
    );

    try {

        const response =
            await fetch(
                URL_CREAR_EMPLEADO_CORE3,
                {
                    method: "POST",
                    mode: "cors",

                    headers: {
                        "Content-Type":
                            "application/json",

                        "Accept":
                            "application/json"
                    },

                    body:
                        JSON.stringify(
                            datosEmpleado
                        )
                }
            );

        const texto =
            await response.text();

        let respuesta;

        try {
            respuesta =
                convertirRespuestaCore3(
                    texto
                );
        } catch (errorConversion) {

            console.error(
                "Respuesta recibida de Core3:",
                texto
            );

            throw new Error(
                "Core3 devolvió una respuesta que no se pudo interpretar."
            );
        }

        if (!response.ok) {
            throw new Error(
                respuesta.Mensaje ||
                `Core3 respondió con HTTP ${response.status}.`
            );
        }

        if (
            !respuesta ||
            respuesta.Exito !== true
        ) {
            throw new Error(
                respuesta?.Mensaje ||
                "Core3 no pudo crear el empleado."
            );
        }

        mostrarMensaje(
            respuesta.Mensaje ||
            "Empleado creado con éxito",
            "exito"
        );

        formulario.reset();

        configurarFechas();

        await cargarPuestosActivos();

    } catch (error) {

        console.error(
            "Error al crear empleado:",
            error
        );

        let mensaje =
            error.message ||
            "Error al comunicarse con Core3.";

        if (
            mensaje === "Failed to fetch" ||
            mensaje.includes("NetworkError")
        ) {
            mensaje =
                "No fue posible conectarse directamente con Core3. " +
                "Compruebe que Visual Studio esté ejecutándose en el puerto 61932.";
        }

        mostrarMensaje(
            mensaje,
            "error"
        );

    } finally {

        if (boton) {
            boton.disabled = false;

            boton.textContent =
                "Crear empleado";
        }
    }
}


// =====================================================
// CONVERTIR RESPUESTA DE CORE3
// =====================================================

function convertirRespuestaCore3(texto) {

    const contenido =
        texto.trim();

    if (contenido === "") {
        return {
            Exito: false,
            Mensaje:
                "Core3 no devolvió una respuesta.",
            IdEmpleado: 0,
            NumeroEmpleado: ""
        };
    }

    if (contenido.startsWith("<")) {
        return convertirXMLCore3(
            contenido
        );
    }

    const datos =
        JSON.parse(contenido);

    if (
        datos &&
        datos.CrearEmpleadoResult
    ) {
        return datos.CrearEmpleadoResult;
    }

    if (
        datos &&
        datos.d
    ) {

        if (
            typeof datos.d === "string"
        ) {
            return JSON.parse(
                datos.d
            );
        }

        return datos.d;
    }

    return datos;
}


// =====================================================
// CONVERTIR XML DE CORE3
// =====================================================

function convertirXMLCore3(xmlTexto) {

    const parser =
        new DOMParser();

    const xml =
        parser.parseFromString(
            xmlTexto,
            "text/xml"
        );

    const errorXML =
        xml.querySelector(
            "parsererror"
        );

    if (errorXML) {
        return {
            Exito: false,
            Mensaje:
                "Core3 devolvió un XML inválido.",
            IdEmpleado: 0,
            NumeroEmpleado: ""
        };
    }

    return {
        Exito:
            obtenerValorXMLGlobal(
                xml,
                "Exito"
            ).toLowerCase() === "true",

        Mensaje:
            obtenerValorXMLGlobal(
                xml,
                "Mensaje"
            ),

        IdEmpleado:
            Number(
                obtenerValorXMLGlobal(
                    xml,
                    "IdEmpleado"
                )
            ),

        NumeroEmpleado:
            obtenerValorXMLGlobal(
                xml,
                "NumeroEmpleado"
            )
    };
}


function obtenerValorXMLGlobal(
    xml,
    etiqueta
) {

    const elementos =
        xml.getElementsByTagName(
            etiqueta
        );

    if (
        !elementos ||
        elementos.length === 0
    ) {
        return "";
    }

    return elementos[0].textContent ?? "";
}


// =====================================================
// VALIDACIONES
// =====================================================

function validarDatosEmpleado(datos) {

    if (
        datos.NumeroEmpleado === "" ||
        datos.Identificacion === "" ||
        datos.TipoIdentificacion === "" ||
        datos.NombreCompleto === "" ||
        datos.FechaNacimiento === "" ||
        datos.Correo === "" ||
        datos.Telefono === "" ||
        datos.IdPuesto <= 0 ||
        datos.FechaContratacion === "" ||
        datos.Estado === ""
    ) {
        return "Todos los campos son obligatorios.";
    }

    if (
        datos.NumeroEmpleado.length > 30
    ) {
        return "El número de empleado no puede superar los 30 caracteres.";
    }

    if (
        datos.Identificacion.length > 50
    ) {
        return "La identificación no puede superar los 50 caracteres.";
    }

    if (
        datos.NombreCompleto.length > 150
    ) {
        return "El nombre completo no puede superar los 150 caracteres.";
    }

    const expresionCorreo =
        /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (
        !expresionCorreo.test(
            datos.Correo
        )
    ) {
        return "El correo electrónico no tiene un formato válido.";
    }

    const expresionTelefono =
        /^[0-9+\-\s]{8,20}$/;

    if (
        !expresionTelefono.test(
            datos.Telefono
        )
    ) {
        return "El teléfono no tiene un formato válido.";
    }

    const fechaNacimiento =
        new Date(
            datos.FechaNacimiento +
            "T00:00:00"
        );

    const fechaContratacion =
        new Date(
            datos.FechaContratacion +
            "T00:00:00"
        );

    if (
        Number.isNaN(
            fechaNacimiento.getTime()
        )
    ) {
        return "La fecha de nacimiento no es válida.";
    }

    if (
        Number.isNaN(
            fechaContratacion.getTime()
        )
    ) {
        return "La fecha de contratación no es válida.";
    }

    const hoy =
        new Date();

    hoy.setHours(
        0,
        0,
        0,
        0
    );

    if (
        fechaNacimiento >= hoy
    ) {
        return "La fecha de nacimiento debe ser anterior a la fecha actual.";
    }

    if (
        fechaContratacion <
        fechaNacimiento
    ) {
        return "La fecha de contratación no puede ser anterior a la fecha de nacimiento.";
    }

    if (
        datos.Estado !== "Activo" &&
        datos.Estado !== "Inactivo"
    ) {
        return "El estado seleccionado no es válido.";
    }

    return "";
}


// =====================================================
// FUNCIONES AUXILIARES
// =====================================================

function obtenerValor(id) {

    const elemento =
        document.getElementById(id);

    return elemento
        ? elemento.value.trim()
        : "";
}


function mostrarMensaje(
    mensaje,
    tipo
) {

    const contenedor =
        document.getElementById(
            "mensajeEmpleado"
        );

    if (!contenedor) {
        return;
    }

    contenedor.className =
        "mensaje-formulario " +
        tipo;

    contenedor.textContent =
        mensaje;
}


function limpiarMensaje() {

    const contenedor =
        document.getElementById(
            "mensajeEmpleado"
        );

    if (!contenedor) {
        return;
    }

    contenedor.className =
        "mensaje-formulario";

    contenedor.textContent =
        "";
}