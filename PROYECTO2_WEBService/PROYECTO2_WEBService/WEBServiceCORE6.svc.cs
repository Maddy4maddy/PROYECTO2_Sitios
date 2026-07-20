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
    public class WEBServiceCORE6 : IWEBServiceCORE6
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

        public PuestoDTO ObtenerPuestoPorCodigo(string codigo)
        {
            PuestoDTO puesto = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id, codigo_puesto, nombre_puesto, salario, 
                               estado, fecha_creacion
                        FROM puestos
                        WHERE codigo_puesto = @codigo";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                puesto = new PuestoDTO
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CodigoPuesto = reader["codigo_puesto"].ToString(),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    Salario = Convert.ToDecimal(reader["salario"]),
                                    Estado = reader["estado"].ToString(),
                                    FechaCreacion = reader["fecha_creacion"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["fecha_creacion"])
                                        .ToString("yyyy-MM-dd HH:mm:ss") :
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener el puesto: {ex.Message}");
            }

            return puesto;
        }
    }
}