/**
 * verOferente.js
 * CORE 8: Busca y muestra la información de un oferente por código
 */

const URL_OFERENTE =
    "http://localhost:61932/WEBServiceCORE8.svc/ObtenerOferente?codigo=";

async function buscarOferente() {

    const container = document.getElementById("detalle-oferente");
    const input = document.getElementById("codigoOferente");
    const codigo = input.value.trim();

    if (!codigo) {
        container.innerHTML = `
            <div class="error">
                Por favor, ingrese un código de oferente
            </div>
        `;
        return;
    }

    container.innerHTML = `
        <div class="loading">Buscando oferente...</div>
    `;

    try {

        const response = await fetch(URL_OFERENTE + codigo, {
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
        let oferente = null;

        if (text.trim().startsWith("<")) {
            oferente = parseXMLOferente(text);
        } else {
            const data = JSON.parse(text);
            oferente = data.ObtenerOferenteResult || data;
        }

        if (!oferente || Object.keys(oferente).length === 0) {
            container.innerHTML = `
                <div class="error">
                    No se encontró un oferente con el código: <strong>${codigo}</strong>
                </div>
            `;
            return;
        }

        renderizarOferente(container, oferente);

    } catch (error) {
        console.error("Error:", error);
        container.innerHTML = `
            <div class="error">
                Error al buscar el oferente: ${error.message}
            </div>
        `;
    }

}

function parseXMLOferente(xmlString) {
    const parser = new DOMParser();
    const xml = parser.parseFromString(xmlString, "text/xml");
    const item = xml.getElementsByTagName("OferenteDTO")[0];

    if (!item) return null;

    return {
        CodigoOferente: item.getElementsByTagName("CodigoOferente")[0]?.textContent || "",
        Identificacion: item.getElementsByTagName("Identificacion")[0]?.textContent || "",
        TipoIdentificacion: item.getElementsByTagName("TipoIdentificacion")[0]?.textContent || "",
        NombreCompleto: item.getElementsByTagName("NombreCompleto")[0]?.textContent || "",
        FechaNacimiento: item.getElementsByTagName("FechaNacimiento")[0]?.textContent || "",
        Correo: item.getElementsByTagName("Correo")[0]?.textContent || "",
        Telefono: item.getElementsByTagName("Telefono")[0]?.textContent || ""
    };
}

function renderizarOferente(container, oferente) {
    container.innerHTML = `
        <div class="detalle-card">
            <h3 class="detalle-titulo">Información del Oferente</h3>

            <div class="detalle-grid">
                <div class="detalle-campo">
                    <label>Código del Oferente</label>
                    <span class="detalle-valor">${oferente.CodigoOferente || "N/A"}</span>
                </div>
                <div class="detalle-campo">
                    <label>Identificación</label>
                    <span class="detalle-valor">${oferente.Identificacion || "N/A"}</span>
                </div>
                <div class="detalle-campo">
                    <label>Tipo de Identificación</label>
                    <span class="detalle-valor">${oferente.TipoIdentificacion || "N/A"}</span>
                </div>
                <div class="detalle-campo">
                    <label>Nombre Completo</label>
                    <span class="detalle-valor">${oferente.NombreCompleto || "N/A"}</span>
                </div>
                <div class="detalle-campo">
                    <label>Fecha de Nacimiento</label>
                    <span class="detalle-valor">${oferente.FechaNacimiento || "N/A"}</span>
                </div>
                <div class="detalle-campo">
                    <label>Correo Electrónico</label>
                    <span class="detalle-valor">${oferente.Correo || "N/A"}</span>
                </div>
                <div class="detalle-campo">
                    <label>Teléfono</label>
                    <span class="detalle-valor">${oferente.Telefono || "N/A"}</span>
                </div>
            </div>
        </div>
    `;
}

document.addEventListener("DOMContentLoaded", function() {
    const input = document.getElementById("codigoOferente");
    if (input) {
        input.addEventListener("keypress", function(e) {
            if (e.key === "Enter") {
                buscarOferente();
            }
        });
    }
});