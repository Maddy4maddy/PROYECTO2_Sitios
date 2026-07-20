const URL_OFERENTE =
"http://localhost:61932/WEBServiceCORE8.svc/ObtenerOferente?codigo=";


const URL_PUESTOS =
"http://localhost:61932/WEBServiceCORE6.svc/ObtenerPuestosActivos";


const URL_CREAR_EMPLEADO =
"http://localhost:61932/WEBServiceCORE3.svc/CrearEmpleado";


let oferenteActual = null;


document.addEventListener("DOMContentLoaded", () => {

    cargarDatos();

});



async function cargarDatos(){

    const container =
    document.getElementById("detalle-empleado");


    try{

        const oferente =
        await obtenerOferente();


        const puestos =
        await obtenerPuestos();


        oferenteActual = oferente;


        mostrarFormulario(
            oferente,
            puestos
        );


    }catch(error){

        container.innerHTML = `
            <div class="error">
                ${error.message}
            </div>
        `;

    }

}



async function obtenerOferente(){


    const response =
    await fetch(
        URL_OFERENTE + codigoOferente
    );


    if(!response.ok){

        throw new Error(
            "No se pudo obtener el oferente"
        );

    }


    const data =
    await response.json();


    return data.ObtenerOferenteResult || data;

}



async function obtenerPuestos(){


    const response =
    await fetch(URL_PUESTOS);


    if(!response.ok){

        throw new Error(
            "No se pudieron cargar los puestos"
        );

    }


    return await response.json();

}



function mostrarFormulario(oferente, puestos){


const container =
document.getElementById("detalle-empleado");



let opciones = `
<option value="">
Seleccione un puesto
</option>
`;



puestos.forEach(p => {

    opciones += `
        <option value="${p.Id}">
            ${p.NombrePuesto}
        </option>
    `;

});



container.innerHTML = `


<div class="detalle-card">


<h3 class="detalle-titulo">
Información del Oferente
</h3>



<div class="detalle-grid">


<div class="detalle-campo">
<label>Código</label>
<span class="detalle-valor">
${oferente.CodigoOferente}
</span>
</div>


<div class="detalle-campo">
<label>Identificación</label>
<span class="detalle-valor">
${oferente.Identificacion}
</span>
</div>


<div class="detalle-campo">
<label>Tipo Identificación</label>
<span class="detalle-valor">
${oferente.TipoIdentificacion}
</span>
</div>


<div class="detalle-campo">
<label>Nombre Completo</label>
<span class="detalle-valor">
${oferente.NombreCompleto}
</span>
</div>


<div class="detalle-campo">
<label>Fecha Nacimiento</label>
<span class="detalle-valor">
${oferente.FechaNacimiento}
</span>
</div>


<div class="detalle-campo">
<label>Correo</label>
<span class="detalle-valor">
${oferente.Correo}
</span>
</div>


<div class="detalle-campo">
<label>Teléfono</label>
<span class="detalle-valor">
${oferente.Telefono}
</span>
</div>


</div>


<hr>


<h3 class="detalle-titulo">
Datos del Empleado
</h3>



<div class="detalle-grid">


<div class="detalle-campo">

<label>
Número Empleado
</label>

<input id="numeroEmpleado"
type="text">

</div>



<div class="detalle-campo">

<label>
Puesto
</label>

<select id="idPuesto">

${opciones}

</select>

</div>



<div class="detalle-campo">

<label>
Fecha Contratación
</label>

<input id="fechaContratacion"
type="date">

</div>



<div class="detalle-campo">

<label>
Estado
</label>

<select id="estado">

<option value="Activo">
Activo
</option>

<option value="Inactivo">
Inactivo
</option>

</select>

</div>


</div>



<div class="acciones-detalle">


<button class="btn-crear"
onclick="crearEmpleado()">

Crear empleado

</button>


<button
onclick="cancelar()">

Cancelar

</button>


</div>



</div>

`;

}


function cancelar(){

window.location.href =
"verOferente.php";

}


async function crearEmpleado(){

    const numeroEmpleado =
    document.getElementById("numeroEmpleado").value.trim();

    const idPuesto =
    document.getElementById("idPuesto").value;

    const fechaContratacion =
    document.getElementById("fechaContratacion").value;

    const estado =
    document.getElementById("estado").value;


    if(!numeroEmpleado ||
       !idPuesto ||
       !fechaContratacion){

        alert("Complete todos los datos del empleado");
        return;
    }


    const empleado = {

        NumeroEmpleado: numeroEmpleado,

        Identificacion:
        oferenteActual.Identificacion,

        TipoIdentificacion:
        oferenteActual.TipoIdentificacion,

        NombreCompleto:
        oferenteActual.NombreCompleto,

        FechaNacimiento:
        oferenteActual.FechaNacimiento,

        Correo:
        oferenteActual.Correo,

        Telefono:
        oferenteActual.Telefono,

        IdPuesto:
        parseInt(idPuesto),

        FechaContratacion:
        fechaContratacion,

        Estado:
        estado
    };


    try{

        const response =
        await fetch(URL_CREAR_EMPLEADO,{

            method:"POST",

            headers:{
                "Content-Type":"application/json"
            },

            body:
            JSON.stringify(empleado)

        });


        if(!response.ok){

            throw new Error(
                "Error HTTP: " + response.status
            );

        }


        const resultado =
        await response.json();


        if(resultado.Exito){

            alert(
                "Empleado creado correctamente. Número: "
                + resultado.NumeroEmpleado
            );


            window.location.href =
            "verOferente.php";

        }
        else{

            alert(resultado.Mensaje);

        }


    }catch(error){

        console.error(error);

        alert(
            "Error al crear empleado: "
            + error.message
        );

    }

}