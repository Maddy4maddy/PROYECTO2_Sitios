using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Web;
using MySql.Data.MySqlClient;

namespace PROYECTO2_WEBService
{
    public class WEBServiceCORE1 : IService1
    {
        private string connectionString =
            ConfigurationManager
            .ConnectionStrings["MySQLConnection"]
            .ConnectionString;

        // ========================================
        // PUESTOS ACTIVOS (CORE 1)
        // ========================================

        public List<PuestoDTO> ObtenerPuestosActivos()
        {
            List<PuestoDTO> lista = new List<PuestoDTO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id,
                               codigo_puesto,
                               nombre_puesto,
                               salario,
                               estado,
                               fecha_creacion
                        FROM puestos
                        WHERE estado = 'Activo'
                        ORDER BY nombre_puesto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new PuestoDTO
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
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener puestos activos: {ex.Message}");
            }

            return lista;
        }

        // ========================================
        // PUESTO POR CODIGO (CORE 6)
        // ========================================

        public PuestoDTO ObtenerPuestoPorCodigo(string codigo)
        {
            PuestoDTO puesto = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id,
                               codigo_puesto,
                               nombre_puesto,
                               salario,
                               estado,
                               fecha_creacion
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
                throw new FaultException($"Error al obtener puesto: {ex.Message}");
            }

            return puesto;
        }

        // ========================================
        // PUESTOS POR SALARIO
        // ========================================

        public List<PuestoDTO> ObtenerPuestosPorSalario(decimal min, decimal max)
        {
            List<PuestoDTO> lista = new List<PuestoDTO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id,
                               codigo_puesto,
                               nombre_puesto,
                               salario,
                               estado,
                               fecha_creacion
                        FROM puestos
                        WHERE estado = 'Activo'
                        AND salario BETWEEN @min AND @max
                        ORDER BY salario DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@min", min);
                        cmd.Parameters.AddWithValue("@max", max);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new PuestoDTO
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
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener puestos por salario: {ex.Message}");
            }

            return lista;
        }

        // ========================================
        // TODOS LOS PUESTOS
        // ========================================

        public List<PuestoDTO> ObtenerTodosLosPuestos()
        {
            List<PuestoDTO> lista = new List<PuestoDTO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id,
                               codigo_puesto,
                               nombre_puesto,
                               salario,
                               estado,
                               fecha_creacion
                        FROM puestos
                        ORDER BY estado DESC, nombre_puesto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new PuestoDTO
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
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener todos los puestos: {ex.Message}");
            }

            return lista;
        }

        // ========================================
        // OPTIONS PARA CORS (Requerido por la interfaz)
        // ========================================

        public void Options()
        {
            // El CORS se maneja en Web.config
            // Este método solo existe para cumplir con la interfaz
            WebOperationContext.Current.OutgoingResponse.StatusCode =
                System.Net.HttpStatusCode.OK;
        }

        // ========================================
        // MÉTODOS ORIGINALES DEL WCF
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
    }
}