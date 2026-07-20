/**
 * CORE 6: Listado de puestos activos (solo nombre como enlace)
 * Usa CORE 1 para obtener los datos
 */

const URL_PUESTOS =
    "http://localhost:61932/WEBServiceCORE1.svc/ObtenerPuestosActivos";

async function obtenerPuestosCore6() {

    const tbody = document.getElementById("puestos-core6-container");
    if (!tbody) return;

    tbody.innerHTML = `
        <tr>
            <td class="loading"> Cargando puestos activos...</td>
        </tr>
    `;

    try {

        const response = await fetch(URL_PUESTOS, {
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
        let puestos = [];

        if (text.trim().startsWith("<")) {
            puestos = parseXMLCore6(text);
        } else {
            const data = JSON.parse(text);
            puestos = data.ObtenerPuestosActivosResult || data;
        }

        puestos.sort((a, b) => {
            const idA = parseInt(a.Id || a.id || 0);
            const idB = parseInt(b.Id || b.id || 0);
            return idA - idB;
        });

        if (!puestos || puestos.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td class="loading">No hay puestos activos disponibles</td>
                </tr>
            `;
            return;
        }

        renderizarPuestosCore6(tbody, puestos);

    } catch (error) {
        console.error("Error:", error);
        tbody.innerHTML = `
            <tr>
                <td class="loading" style="color:#dc3545">
                    Error al cargar puestos: ${error.message}
                </td>
            </tr>
        `;
    }

}

function parseXMLCore6(xmlString) {
    const parser = new DOMParser();
    const xml = parser.parseFromString(xmlString, "text/xml");
    const items = xml.getElementsByTagName("PuestoDTO");
    const puestos = [];

    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        puestos.push({
            Id: item.getElementsByTagName("Id")[0]?.textContent || "",
            CodigoPuesto: item.getElementsByTagName("CodigoPuesto")[0]?.textContent || "",
            NombrePuesto: item.getElementsByTagName("NombrePuesto")[0]?.textContent || "",
            Salario: item.getElementsByTagName("Salario")[0]?.textContent || 0,
            Estado: item.getElementsByTagName("Estado")[0]?.textContent || "",
            FechaCreacion: item.getElementsByTagName("FechaCreacion")[0]?.textContent || ""
        });
    }
    return puestos;
}

function renderizarPuestosCore6(tbody, puestos) {
    let html = "";
    
    puestos.forEach(puesto => {
        const codigo = puesto.CodigoPuesto || puesto.codigoPuesto || "";
        const nombre = puesto.NombrePuesto || puesto.nombrePuesto || "Sin nombre";
        
        html += `
            <tr>
                <td>
                    <a href="detallePuesto.php?codigo=${codigo}" class="enlace-puesto">
                        ${nombre}
                    </a>
                </td>
            </tr>
        `;
    });
    
    tbody.innerHTML = html;
}

document.addEventListener("DOMContentLoaded", () => {
    if (document.getElementById("puestos-core6-container")) {
        obtenerPuestosCore6();
    }
});