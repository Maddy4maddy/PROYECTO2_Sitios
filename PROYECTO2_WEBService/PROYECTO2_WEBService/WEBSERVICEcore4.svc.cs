using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PROYECTO2_WEBService
{
    public class WEBSERVICEcore4 : IWEBSERVICEcore4
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

        // ========================================
        // LOGIN
        // ========================================

        public LoginResponse Login(LoginRequest request)
        {
            LoginResponse respuesta = new LoginResponse();

            try
            {
                if (request == null ||
                    string.IsNullOrWhiteSpace(request.Usuario) ||
                    string.IsNullOrWhiteSpace(request.Contrasena))
                {
                    respuesta.Exito = false;
                    respuesta.Mensaje = "Usuario y/o contraseña incorrectos.";
                    return respuesta;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            id_usuario,
                            nombre_completo,
                            contrasena,
                            intentos_fallidos,
                            bloqueado,
                            estado
                        FROM usuarios
                        WHERE nombre_usuario = @usuario";

                    int idUsuario = 0;
                    string nombreCompleto = "";
                    string contrasenaBD = "";
                    int intentos = 0;
                    bool bloqueado = false;
                    string estado = "";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", request.Usuario);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                respuesta.Exito = false;
                                respuesta.Mensaje = "Usuario y/o contraseña incorrectos.";
                                return respuesta;
                            }

                            idUsuario = Convert.ToInt32(reader["id_usuario"]);
                            nombreCompleto = reader["nombre_completo"].ToString();
                            contrasenaBD = reader["contrasena"].ToString();
                            intentos = Convert.ToInt32(reader["intentos_fallidos"]);
                            bloqueado = Convert.ToBoolean(reader["bloqueado"]);
                            estado = reader["estado"].ToString();
                        }
                    }

                    if (estado.ToLower() != "activo")
                    {
                        respuesta.Exito = false;
                        respuesta.Mensaje = "El usuario se encuentra inactivo.";
                        return respuesta;
                    }

                    if (bloqueado)
                    {
                        respuesta.Exito = false;
                        respuesta.Mensaje = "El usuario se encuentra bloqueado.";
                        return respuesta;
                    }

                    string contrasenaIngresada = EncriptarSHA256(request.Contrasena);

                    if (contrasenaIngresada != contrasenaBD)
                    {
                        intentos++;

                        string sqlIntentos = @"
                            UPDATE usuarios
                            SET intentos_fallidos = @intentos
                            WHERE id_usuario = @idUsuario";

                        using (MySqlCommand cmdIntentos = new MySqlCommand(sqlIntentos, conn))
                        {
                            cmdIntentos.Parameters.AddWithValue("@intentos", intentos);
                            cmdIntentos.Parameters.AddWithValue("@idUsuario", idUsuario);
                            cmdIntentos.ExecuteNonQuery();
                        }

                        if (intentos >= 3)
                        {
                            string sqlBloquear = @"
                                UPDATE usuarios
                                SET bloqueado = 1,
                                    estado = 'bloqueado'
                                WHERE id_usuario = @idUsuario";

                            using (MySqlCommand cmdBloquear = new MySqlCommand(sqlBloquear, conn))
                            {
                                cmdBloquear.Parameters.AddWithValue("@idUsuario", idUsuario);
                                cmdBloquear.ExecuteNonQuery();
                            }

                            respuesta.Exito = false;
                            respuesta.Mensaje = "Usuario bloqueado por exceder el número de intentos.";
                            return respuesta;
                        }

                        respuesta.Exito = false;
                        respuesta.Mensaje = "Usuario y/o contraseña incorrectos.";
                        return respuesta;
                    }

                    string sqlReset = @"
                        UPDATE usuarios
                        SET intentos_fallidos = 0,
                            bloqueado = 0,
                            estado = 'activo'
                        WHERE id_usuario = @idUsuario";

                    using (MySqlCommand cmdReset = new MySqlCommand(sqlReset, conn))
                    {
                        cmdReset.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmdReset.ExecuteNonQuery();
                    }

                    respuesta.Exito = true;
                    respuesta.IdUsuario = idUsuario;
                    respuesta.Nombre = nombreCompleto;
                    respuesta.Mensaje = "Inicio de sesión exitoso.";
                }
            }
            catch (Exception ex)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = ex.Message;
            }

            // ==========================================
            // CORS ELIMINADO - Se maneja en Web.config
            // ==========================================

            return respuesta;
        }

        // ========================================
        // SHA256
        // ========================================

        private string EncriptarSHA256(string contrasena)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        // ========================================
        // MÉTODOS BASE DEL WCF
        // ========================================

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }

            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }

            return composite;
        }

        // ========================================
        // OPTIONS PARA CORS (Requerido por la interfaz)
        // ========================================

        public void Options()
        {
            // El CORS se maneja en Web.config
            // Este método solo existe para cumplir con la interfaz
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
                    System.Net.HttpStatusCode.OK;
            }
        }
    }
}