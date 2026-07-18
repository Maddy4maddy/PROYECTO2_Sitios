using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;

namespace PROYECTO2_WEBService
{
    public class WEBServiceCORE3 : IWEBServiceCORE3
    {
        private readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["MySQLConnection"]
                .ConnectionString;

        /*
         * Atiende la solicitud OPTIONS que envía el navegador
         * antes de realizar el POST con JSON.
         */
        public void OpcionesCrearEmpleado()
        {
            AgregarEncabezadosCors();

            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current
                    .OutgoingResponse.StatusCode =
                    HttpStatusCode.OK;
            }
        }

        /*
         * Registra un nuevo empleado.
         */
        public CrearEmpleadoResponse CrearEmpleado(
            CrearEmpleadoRequest request
        )
        {
            AgregarEncabezadosCors();

            CrearEmpleadoResponse respuesta =
                new CrearEmpleadoResponse
                {
                    Exito = false,
                    Mensaje = "",
                    IdEmpleado = 0,
                    NumeroEmpleado = ""
                };

            try
            {
                string errorValidacion =
                    ValidarSolicitud(request);

                if (!string.IsNullOrWhiteSpace(errorValidacion))
                {
                    EstablecerEstadoHttp(
                        HttpStatusCode.BadRequest
                    );

                    respuesta.Mensaje =
                        errorValidacion;

                    return respuesta;
                }

                DateTime fechaNacimiento;
                DateTime fechaContratacion;

                bool nacimientoConvertido =
                    DateTime.TryParseExact(
                        request.FechaNacimiento.Trim(),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fechaNacimiento
                    );

                bool contratacionConvertida =
                    DateTime.TryParseExact(
                        request.FechaContratacion.Trim(),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fechaContratacion
                    );

                if (
                    !nacimientoConvertido ||
                    !contratacionConvertida
                )
                {
                    EstablecerEstadoHttp(
                        HttpStatusCode.BadRequest
                    );

                    respuesta.Mensaje =
                        "Las fechas recibidas no tienen un formato válido.";

                    return respuesta;
                }

                string estado =
                    string.IsNullOrWhiteSpace(request.Estado)
                        ? "Activo"
                        : request.Estado.Trim();

                using (
                    MySqlConnection conexion =
                        new MySqlConnection(connectionString)
                )
                {
                    conexion.Open();

                    using (
                        MySqlTransaction transaccion =
                            conexion.BeginTransaction()
                    )
                    {
                        try
                        {
                            if (
                                !ExistePuesto(
                                    conexion,
                                    transaccion,
                                    request.IdPuesto
                                )
                            )
                            {
                                throw new InvalidOperationException(
                                    "El puesto seleccionado no existe."
                                );
                            }

                            string conflicto =
                                BuscarConflictoEmpleado(
                                    conexion,
                                    transaccion,
                                    request
                                );

                            if (!string.IsNullOrWhiteSpace(conflicto))
                            {
                                throw new InvalidOperationException(
                                    conflicto
                                );
                            }

                            const string sqlInsertar = @"
                                INSERT INTO empleados
                                (
                                    numero_empleado,
                                    identificacion,
                                    tipo_identificacion,
                                    nombre_completo,
                                    fecha_nacimiento,
                                    correo,
                                    telefono,
                                    id_puesto,
                                    fecha_contratacion,
                                    estado
                                )
                                VALUES
                                (
                                    @numero_empleado,
                                    @identificacion,
                                    @tipo_identificacion,
                                    @nombre_completo,
                                    @fecha_nacimiento,
                                    @correo,
                                    @telefono,
                                    @id_puesto,
                                    @fecha_contratacion,
                                    @estado
                                );
                            ";

                            using (
                                MySqlCommand comando =
                                    new MySqlCommand(
                                        sqlInsertar,
                                        conexion,
                                        transaccion
                                    )
                            )
                            {
                                comando.Parameters.Add(
                                    "@numero_empleado",
                                    MySqlDbType.VarChar,
                                    30
                                ).Value =
                                    request.NumeroEmpleado.Trim();

                                comando.Parameters.Add(
                                    "@identificacion",
                                    MySqlDbType.VarChar,
                                    50
                                ).Value =
                                    request.Identificacion.Trim();

                                comando.Parameters.Add(
                                    "@tipo_identificacion",
                                    MySqlDbType.VarChar,
                                    30
                                ).Value =
                                    request.TipoIdentificacion.Trim();

                                comando.Parameters.Add(
                                    "@nombre_completo",
                                    MySqlDbType.VarChar,
                                    150
                                ).Value =
                                    request.NombreCompleto.Trim();

                                comando.Parameters.Add(
                                    "@fecha_nacimiento",
                                    MySqlDbType.Date
                                ).Value =
                                    fechaNacimiento.Date;

                                comando.Parameters.Add(
                                    "@correo",
                                    MySqlDbType.VarChar,
                                    150
                                ).Value =
                                    request.Correo
                                        .Trim()
                                        .ToLowerInvariant();

                                comando.Parameters.Add(
                                    "@telefono",
                                    MySqlDbType.VarChar,
                                    20
                                ).Value =
                                    request.Telefono.Trim();

                                comando.Parameters.Add(
                                    "@id_puesto",
                                    MySqlDbType.Int32
                                ).Value =
                                    request.IdPuesto;

                                comando.Parameters.Add(
                                    "@fecha_contratacion",
                                    MySqlDbType.Date
                                ).Value =
                                    fechaContratacion.Date;

                                comando.Parameters.Add(
                                    "@estado",
                                    MySqlDbType.VarChar,
                                    10
                                ).Value =
                                    estado;

                                comando.ExecuteNonQuery();

                                respuesta.IdEmpleado =
                                    Convert.ToInt32(
                                        comando.LastInsertedId
                                    );
                            }

                            transaccion.Commit();

                            respuesta.Exito = true;

                            respuesta.Mensaje =
                                "Empleado creado con éxito.";

                            respuesta.NumeroEmpleado =
                                request.NumeroEmpleado.Trim();

                            EstablecerEstadoHttp(
                                HttpStatusCode.Created
                            );

                            return respuesta;
                        }
                        catch
                        {
                            try
                            {
                                transaccion.Rollback();
                            }
                            catch
                            {
                                // La transacción ya no se encontraba activa.
                            }

                            throw;
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                EstablecerEstadoHttp(
                    HttpStatusCode.Conflict
                );

                respuesta.Mensaje =
                    ex.Message;

                return respuesta;
            }
            catch (MySqlException ex)
            {
                EstablecerEstadoHttp(
                    HttpStatusCode.InternalServerError
                );

                if (ex.Number == 1062)
                {
                    respuesta.Mensaje =
                        "No se pudo crear el empleado porque existe información duplicada.";
                }
                else if (ex.Number == 1146)
                {
                    respuesta.Mensaje =
                        "La tabla empleados no existe en la base de datos utilizada por el servicio.";
                }
                else if (ex.Number == 1049)
                {
                    respuesta.Mensaje =
                        "La base de datos configurada en el servicio no existe.";
                }
                else if (ex.Number == 1054)
                {
                    respuesta.Mensaje =
                        "La estructura de la tabla empleados no coincide con la esperada por Core3.";
                }
                else
                {
                    /*
                     * Durante el desarrollo se incluye el número del error
                     * para facilitar la identificación del problema.
                     */
                    respuesta.Mensaje =
                        "Ocurrió un error al registrar el empleado en la base de datos. " +
                        "Código MySQL: " +
                        ex.Number +
                        ".";
                }

                return respuesta;
            }
            catch (Exception)
            {
                EstablecerEstadoHttp(
                    HttpStatusCode.InternalServerError
                );

                respuesta.Mensaje =
                    "Ocurrió un error inesperado al crear el empleado.";

                return respuesta;
            }
        }

        /*
         * Valida la información recibida.
         */
        private string ValidarSolicitud(
            CrearEmpleadoRequest request
        )
        {
            if (request == null)
            {
                return "Debe enviar la información del empleado.";
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.NumeroEmpleado
                )
            )
            {
                return "El número de empleado es obligatorio.";
            }

            if (
                request.NumeroEmpleado.Trim().Length > 30
            )
            {
                return "El número de empleado no puede superar los 30 caracteres.";
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.Identificacion
                )
            )
            {
                return "La identificación es obligatoria.";
            }

            if (
                request.Identificacion.Trim().Length > 50
            )
            {
                return "La identificación no puede superar los 50 caracteres.";
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.TipoIdentificacion
                )
            )
            {
                return "El tipo de identificación es obligatorio.";
            }

            string tipoIdentificacion =
                request.TipoIdentificacion.Trim();

            if (
                tipoIdentificacion != "Cédula de identidad" &&
                tipoIdentificacion != "DIMEX" &&
                tipoIdentificacion != "Pasaporte"
            )
            {
                return "El tipo de identificación no es válido.";
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.NombreCompleto
                )
            )
            {
                return "El nombre completo es obligatorio.";
            }

            if (
                request.NombreCompleto.Trim().Length > 150
            )
            {
                return "El nombre completo no puede superar los 150 caracteres.";
            }

            DateTime fechaNacimiento;

            bool nacimientoValido =
                DateTime.TryParseExact(
                    request.FechaNacimiento?.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fechaNacimiento
                );

            if (!nacimientoValido)
            {
                return "La fecha de nacimiento debe tener el formato yyyy-MM-dd.";
            }

            if (
                fechaNacimiento.Date >=
                DateTime.Today
            )
            {
                return "La fecha de nacimiento debe ser anterior a la fecha actual.";
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.Correo
                )
            )
            {
                return "El correo es obligatorio.";
            }

            if (
                request.Correo.Trim().Length > 150
            )
            {
                return "El correo no puede superar los 150 caracteres.";
            }

            if (
                !Regex.IsMatch(
                    request.Correo.Trim(),
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
                )
            )
            {
                return "El formato del correo no es válido.";
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.Telefono
                )
            )
            {
                return "El teléfono es obligatorio.";
            }

            if (
                !Regex.IsMatch(
                    request.Telefono.Trim(),
                    @"^[0-9+\-\s]{8,20}$"
                )
            )
            {
                return "El formato del teléfono no es válido.";
            }

            if (request.IdPuesto <= 0)
            {
                return "Debe indicar un puesto válido.";
            }

            DateTime fechaContratacion;

            bool contratacionValida =
                DateTime.TryParseExact(
                    request.FechaContratacion?.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fechaContratacion
                );

            if (!contratacionValida)
            {
                return "La fecha de contratación debe tener el formato yyyy-MM-dd.";
            }

            if (
                fechaContratacion.Date <
                fechaNacimiento.Date
            )
            {
                return "La fecha de contratación no puede ser anterior a la fecha de nacimiento.";
            }

            if (
                !string.IsNullOrWhiteSpace(
                    request.Estado
                )
            )
            {
                string estado =
                    request.Estado.Trim();

                if (
                    estado != "Activo" &&
                    estado != "Inactivo"
                )
                {
                    return "El estado debe ser Activo o Inactivo.";
                }
            }

            return "";
        }

        /*
         * Comprueba que el puesto seleccionado existe.
         *
         * Core1 utiliza la columna id de la tabla puestos,
         * por eso aquí se mantiene el mismo nombre.
         */
        private bool ExistePuesto(
            MySqlConnection conexion,
            MySqlTransaction transaccion,
            int idPuesto
        )
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM puestos
                WHERE id = @id_puesto;
            ";

            using (
                MySqlCommand comando =
                    new MySqlCommand(
                        sql,
                        conexion,
                        transaccion
                    )
            )
            {
                comando.Parameters.Add(
                    "@id_puesto",
                    MySqlDbType.Int32
                ).Value =
                    idPuesto;

                int cantidad =
                    Convert.ToInt32(
                        comando.ExecuteScalar()
                    );

                return cantidad > 0;
            }
        }

        /*
         * Comprueba que los datos únicos no estén registrados.
         */
        private string BuscarConflictoEmpleado(
            MySqlConnection conexion,
            MySqlTransaction transaccion,
            CrearEmpleadoRequest request
        )
        {
            const string sql = @"
                SELECT
                    numero_empleado,
                    identificacion,
                    correo,
                    telefono
                FROM empleados
                WHERE numero_empleado = @numero_empleado
                   OR identificacion = @identificacion
                   OR correo = @correo
                   OR telefono = @telefono
                LIMIT 1;
            ";

            using (
                MySqlCommand comando =
                    new MySqlCommand(
                        sql,
                        conexion,
                        transaccion
                    )
            )
            {
                comando.Parameters.Add(
                    "@numero_empleado",
                    MySqlDbType.VarChar,
                    30
                ).Value =
                    request.NumeroEmpleado.Trim();

                comando.Parameters.Add(
                    "@identificacion",
                    MySqlDbType.VarChar,
                    50
                ).Value =
                    request.Identificacion.Trim();

                comando.Parameters.Add(
                    "@correo",
                    MySqlDbType.VarChar,
                    150
                ).Value =
                    request.Correo
                        .Trim()
                        .ToLowerInvariant();

                comando.Parameters.Add(
                    "@telefono",
                    MySqlDbType.VarChar,
                    20
                ).Value =
                    request.Telefono.Trim();

                using (
                    MySqlDataReader lector =
                        comando.ExecuteReader()
                )
                {
                    if (!lector.Read())
                    {
                        return "";
                    }

                    if (
                        string.Equals(
                            Convert.ToString(
                                lector["numero_empleado"]
                            ),
                            request.NumeroEmpleado.Trim(),
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        return "El número de empleado ya está registrado.";
                    }

                    if (
                        string.Equals(
                            Convert.ToString(
                                lector["identificacion"]
                            ),
                            request.Identificacion.Trim(),
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        return "La identificación ya pertenece a un empleado.";
                    }

                    if (
                        string.Equals(
                            Convert.ToString(
                                lector["correo"]
                            ),
                            request.Correo
                                .Trim()
                                .ToLowerInvariant(),
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        return "El correo ya pertenece a un empleado.";
                    }

                    return "El teléfono ya pertenece a un empleado.";
                }
            }
        }

        /*
         * Agrega los encabezados necesarios para que el frontend
         * de localhost:8000 consuma directamente el servicio.
         */
        private void AgregarEncabezadosCors()
        {
            if (WebOperationContext.Current == null)
            {
                return;
            }

            WebOperationContext.Current
                .OutgoingResponse.Headers[
                    "Access-Control-Allow-Origin"
                ] = "*";

            WebOperationContext.Current
                .OutgoingResponse.Headers[
                    "Access-Control-Allow-Methods"
                ] = "POST, OPTIONS";

            WebOperationContext.Current
                .OutgoingResponse.Headers[
                    "Access-Control-Allow-Headers"
                ] = "Content-Type, Accept";

            WebOperationContext.Current
                .OutgoingResponse.Headers[
                    "Access-Control-Max-Age"
                ] = "86400";
        }

        /*
         * Establece el código HTTP sin provocar error
         * si el método se ejecuta fuera de un contexto web.
         */
        private void EstablecerEstadoHttp(
            HttpStatusCode estado
        )
        {
            if (WebOperationContext.Current == null)
            {
                return;
            }

            WebOperationContext.Current
                .OutgoingResponse.StatusCode =
                    estado;
        }
    }
}