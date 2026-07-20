
const URL_PUESTOS =
    "http://localhost:61932/WEBServiceCORE1.svc/ObtenerPuestosActivos";

// ==========================================
// OBTENER PUESTOS
// ==========================================

async function obtenerPuestos() {

    const tbody =
        document.getElementById("puestos-container");

    if (!tbody) return;

    tbody.innerHTML = `
        <tr>
            <td colspan="2" class="loading">
                Cargando puestos activos...
            </td>
        </tr>
    `;

    try {

        console.log("Intentando conectar a:", URL_PUESTOS);

        const response = await fetch(URL_PUESTOS, {

            method: "GET",

            mode: "cors",

            headers: {
                "Accept": "application/json"
            }

        });

        console.log(" Respuesta recibida:", response.status);

        if (!response.ok) {

            throw new Error(
                `Error HTTP: ${response.status} - ${response.statusText}`
            );

        }

        const text =
            await response.text();

        console.log("Texto recibido (primeros 100 caracteres):", text.substring(0, 100));

        let puestos = [];

        // ==================================
        // RESPUESTA XML
        // ==================================

        if (text.trim().startsWith("<")) {

            console.log("Parseando XML...");
            puestos = parseXML(text);

        }
        // ==================================
        // RESPUESTA JSON
        // ==================================

        else {

            console.log("Parseando JSON...");
            const data = JSON.parse(text);
            puestos = data.ObtenerPuestosActivosResult || data;

        }

        console.log("Puestos obtenidos:", puestos.length);

        // ==================================
        // ORDENAR POR ID ASCENDENTE
        // ==================================

        if (puestos && puestos.length > 0) {

            puestos.sort((a, b) => {

                const idA =
                    parseInt(a.Id || a.id || 0);

                const idB =
                    parseInt(b.Id || b.id || 0);

                return idA - idB;

            });

        }

        if (!puestos || puestos.length === 0) {

            tbody.innerHTML = `
                <tr>
                    <td colspan="2" class="loading">
                        No hay puestos activos disponibles
                    </td>
                </tr>
            `;

            return;

        }

        renderizarPuestos(
            tbody,
            puestos
        );

    }
    catch(error) {

        console.error("Error detallado:", error);

        let mensajeError = error.message;

        if (error.message.includes("Failed to fetch")) {
            mensajeError = "No se pudo conectar con el WebService. Verifica que el proyecto de Visual Studio esté ejecutándose.";
        }

        tbody.innerHTML = `

            <tr>

                <td colspan="2"
                    class="loading"
                    style="color:#dc3545">

                    Error al cargar puestos:<br>
                    <span style="font-size:0.9rem;color:#666;">
                        ${mensajeError}
                    </span>

                </td>

            </tr>

        `;

    }

}

// ==========================================
// PARSER XML WCF
// ==========================================

function parseXML(xmlString) {

    const parser =
        new DOMParser();

    const xml =
        parser.parseFromString(
            xmlString,
            "text/xml"
        );

    const items =
        xml.getElementsByTagName(
            "PuestoDTO"
        );

    const puestos = [];

    for (
        let i = 0;
        i < items.length;
        i++
    ) {

        const item =
            items[i];

        puestos.push({

            Id:
                item.getElementsByTagName("Id")[0]
                ?.textContent
                ||
                "",

            CodigoPuesto:
                item.getElementsByTagName("CodigoPuesto")[0]
                ?.textContent
                ||
                "",

            NombrePuesto:
                item.getElementsByTagName("NombrePuesto")[0]
                ?.textContent
                ||
                "",

            Salario:
                item.getElementsByTagName("Salario")[0]
                ?.textContent
                ||
                0,

            Estado:
                item.getElementsByTagName("Estado")[0]
                ?.textContent
                ||
                "",

            FechaCreacion:
                item.getElementsByTagName("FechaCreacion")[0]
                ?.textContent
                ||
                ""

        });

    }

    return puestos;

}

// ==========================================
// MOSTRAR TABLA (CORE 1 - Código y Nombre)
// ==========================================

function renderizarPuestos(tbody, puestos) {

    let html = "";

    puestos.forEach(puesto => {

        const codigo =
            puesto.CodigoPuesto
            ||
            puesto.codigoPuesto
            ||
            "N/A";

        const nombre =
            puesto.NombrePuesto
            ||
            puesto.nombrePuesto
            ||
            "Sin nombre";

        html += `

            <tr>

                <td>
                    <strong>${codigo}</strong>
                </td>

                <td>
                    ${nombre}
                </td>

            </tr>

        `;

    });

    tbody.innerHTML = html;

}

// ==========================================
// INICIO
// ==========================================

document.addEventListener(
    "DOMContentLoaded",
    () => {

        if (
            document.getElementById(
                "puestos-container"
            )
        ) {

            obtenerPuestos();

        }

    }
);