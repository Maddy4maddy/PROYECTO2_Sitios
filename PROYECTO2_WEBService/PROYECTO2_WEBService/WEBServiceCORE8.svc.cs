using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace PROYECTO2_WEBService
{
    public class WEBServiceCORE8 : IWEBServiceCORE8
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

        public OferenteDTO ObtenerOferente(string codigo)
        {
            OferenteDTO oferente = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    //  Buscar por codigo_oferente
                    string query = @"
                        SELECT 
                            o.codigo_oferente,
                            o.identificacion,
                            o.tipo_identificacion,
                            o.nombre_completo,
                            o.fecha_nacimiento,
                            o.correo,
                            o.telefono
                        FROM oferentes o
                        WHERE o.codigo_oferente = @codigo";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oferente = new OferenteDTO
                                {
                                    CodigoOferente = reader["codigo_oferente"]?.ToString() ?? "",
                                    Identificacion = reader["identificacion"]?.ToString() ?? "",
                                    TipoIdentificacion = reader["tipo_identificacion"]?.ToString() ?? "",
                                    NombreCompleto = reader["nombre_completo"]?.ToString() ?? "",
                                    FechaNacimiento = reader["fecha_nacimiento"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["fecha_nacimiento"]).ToString("yyyy-MM-dd") : "",
                                    Correo = reader["correo"]?.ToString() ?? "",
                                    Telefono = reader["telefono"]?.ToString() ?? ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener el oferente: {ex.Message}");
            }

            return oferente;
        }
    }
}