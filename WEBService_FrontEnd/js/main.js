/**
 * main.js - Consume el WebService y muestra los puestos en la tabla
 */

async function obtenerPuestos() {
    const tbody = document.getElementById('puestos-container');
    if (!tbody) return;

    tbody.innerHTML = `
        <tr>
            <td colspan="3" class="loading"> Cargando puestos activos...</td>
        </tr>
    `;

    try {
        const url = 'http://localhost:61932/WEBServiceCORE1.svc/ObtenerPuestosActivos';
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        const text = await response.text();
        
        let puestos = [];

        if (text.trim().startsWith('<')) {
            puestos = parseXML(text);
        } else {
            const data = JSON.parse(text);
            puestos = data.ObtenerPuestosActivosResult || data;
        }

        // ORDENAR POR ID (ASCENDENTE)
        puestos.sort((a, b) => {
            const idA = parseInt(a.Id || a.id || 0);
            const idB = parseInt(b.Id || b.id || 0);
            return idA - idB;
        });

        if (!puestos || puestos.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="3" class="loading">📭 No hay puestos activos disponibles</td>
                </tr>
            `;
            return;
        }

        renderizarPuestos(tbody, puestos);

    } catch (error) {
        console.error('Error:', error);
        tbody.innerHTML = `
            <tr>
                <td colspan="3" class="loading" style="color: #dc3545;">
                     Error al cargar los puestos: ${error.message}
                </td>
            </tr>
        `;
    }
}

/**
 * Parsea XML y extrae los puestos
 */
function parseXML(xmlString) {
    const parser = new DOMParser();
    const xml = parser.parseFromString(xmlString, 'text/xml');
    
    const items = xml.getElementsByTagName('PuestoDTO');
    const puestos = [];

    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        
        const id = item.getElementsByTagName('Id')[0]?.textContent || '';
        const codigoPuesto = item.getElementsByTagName('CodigoPuesto')[0]?.textContent || '';
        const nombrePuesto = item.getElementsByTagName('NombrePuesto')[0]?.textContent || '';
        const salario = item.getElementsByTagName('Salario')[0]?.textContent || 0;
        const estado = item.getElementsByTagName('Estado')[0]?.textContent || '';
        const fechaCreacion = item.getElementsByTagName('FechaCreacion')[0]?.textContent || '';

        puestos.push({
            Id: id,
            CodigoPuesto: codigoPuesto,
            NombrePuesto: nombrePuesto,
            Salario: salario,
            Estado: estado,
            FechaCreacion: fechaCreacion
        });
    }

    return puestos;
}

/**
 * Renderiza los puestos en la tabla
 */
function renderizarPuestos(tbody, puestos) {
    if (!puestos || puestos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="3" class="loading">📭 No hay puestos activos disponibles</td>
            </tr>
        `;
        return;
    }

    let html = '';
    puestos.forEach(puesto => {
        const id = puesto.Id || puesto.id || 'N/A';
        const nombre = puesto.NombrePuesto || puesto.nombrePuesto || 'Sin nombre';
        const salario = puesto.Salario || puesto.salario || 0;

        html += `
            <tr>
                <td>${id}</td>
                <td>${nombre}</td>
                <td>₡ ${formatearSalario(salario)}</td>
            </tr>
        `;
    });

    tbody.innerHTML = html;
}

/**
 * Formatea el salario con separadores de miles
 */
function formatearSalario(valor) {
    return Number(valor).toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

// ==========================================
// EJECUTAR CUANDO LA PÁGINA ESTÉ CARGADA
// ==========================================

document.addEventListener('DOMContentLoaded', function() {
    if (document.getElementById('puestos-container')) {
        obtenerPuestos();
    }
});