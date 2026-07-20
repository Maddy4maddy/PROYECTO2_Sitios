/**
 CORE6
 */

const URL_DETALLE =
    "http://localhost:61932/WEBServiceCORE6.svc/ObtenerPuestoPorCodigo?codigo=";

async function obtenerDetallePuesto() {

    const container = document.getElementById("detalle-puesto");
    if (!container) return;

    // OBTENER EL CÓDIGO DESDE LA VARIABLE GLOBAL
    const codigo = window.codigoPuesto;

    console.log("Buscando puesto con código:", codigo);

    if (!codigo || codigo === "") {
        container.innerHTML = `
            <div class="error">
                No se recibió ningún código de puesto.
                <br><br>
                <a href="puestos_core6.php" class="btn-volver" style="display:inline-block;margin-top:10px;">
                    ← Volver a la lista
                </a>
            </div>
        `;
        return;
    }

    try {

        const url = URL_DETALLE + codigo;
        console.log(" Consultando URL:", url);

        const response = await fetch(url, {
            method: "GET",
            mode: "cors",
            headers: {
                "Accept": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        const text = await response.text();
        console.log("Respuesta recibida (primeros 100 caracteres):", text.substring(0, 100));

        let puesto = null;

        // Respuesta XML
        if (text.trim().startsWith("<")) {
            puesto = parseXMLDetalle(text);
        }
        // Respuesta JSON
        else {
            const data = JSON.parse(text);
            puesto = data.ObtenerPuestoPorCodigoResult || data;
        }

        console.log("Puesto parseado:", puesto);

        if (!puesto || Object.keys(puesto).length === 0) {
            container.innerHTML = `
                <div class="error">
                    No se encontró el puesto con código: ${codigo}
                    <br><br>
                    <a href="puestos_core6.php" class="btn-volver" style="display:inline-block;margin-top:10px;">
                        ← Volver a la lista
                    </a>
                </div>
            `;
            return;
        }

        renderizarDetalle(container, puesto);

    } catch (error) {
        console.error("Error:", error);
        container.innerHTML = `
            <div class="error">
                Error al cargar el puesto: ${error.message}
                <br><br>
                <a href="puestos_core6.php" class="btn-volver" style="display:inline-block;margin-top:10px;">
                    ← Volver a la lista
                </a>
            </div>
        `;
    }

}

function parseXMLDetalle(xmlString) {
    const parser = new DOMParser();
    const xml = parser.parseFromString(xmlString, "text/xml");
    const item = xml.getElementsByTagName("PuestoDTO")[0];

    if (!item) return null;

    return {
        Id: item.getElementsByTagName("Id")[0]?.textContent || "",
        CodigoPuesto: item.getElementsByTagName("CodigoPuesto")[0]?.textContent || "",
        NombrePuesto: item.getElementsByTagName("NombrePuesto")[0]?.textContent || "",
        Salario: item.getElementsByTagName("Salario")[0]?.textContent || 0,
        Estado: item.getElementsByTagName("Estado")[0]?.textContent || "",
        FechaCreacion: item.getElementsByTagName("FechaCreacion")[0]?.textContent || ""
    };
}

function renderizarDetalle(container, puesto) {
    const salario = Number(puesto.Salario || 0).toLocaleString("es-CR", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    container.innerHTML = `
        <div class="detalle-card">
            <div class="detalle-campo">
                <label>Código del Puesto</label>
                <span class="detalle-valor">${puesto.CodigoPuesto || "N/A"}</span>
            </div>
            <div class="detalle-campo">
                <label>Nombre del Puesto</label>
                <span class="detalle-valor">${puesto.NombrePuesto || "N/A"}</span>
            </div>
            <div class="detalle-campo">
                <label>Salario</label>
                <span class="detalle-valor">₡ ${salario}</span>
            </div>
            <div class="detalle-campo">
                <label>Estado</label>
                <span class="detalle-valor estado-${puesto.Estado?.toLowerCase() || "activo"}">
                    ${puesto.Estado || "Activo"}
                </span>
            </div>
            <div class="detalle-campo">
                <label>Fecha de Creación</label>
                <span class="detalle-valor">${puesto.FechaCreacion || "N/A"}</span>
            </div>
        </div>
    `;
}

// ==========================================
// INICIO 
// ==========================================

document.addEventListener("DOMContentLoaded", function() {
    console.log(" DOM cargado, buscando contenedor...");
    
    if (document.getElementById("detalle-puesto")) {
        console.log("Contenedor encontrado, cargando detalle...");
        obtenerDetallePuesto();
    } else {
        console.error("No se encontró #detalle-puesto");
    }
});