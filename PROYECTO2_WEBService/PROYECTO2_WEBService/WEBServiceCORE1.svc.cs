using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web.Script.Serialization;

namespace PROYECTO2_WEBService
{
    public class WEBServiceCORE1 : IService1
    {
        // Cadena de conexión desde Web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;

        /// <summary>
        /// Obtiene todos los puestos ACTIVOS (Estado = 'Activo')
        /// </summary>
        public List<PuestoDTO> ObtenerPuestosActivos()
        {
            List<PuestoDTO> listaPuestos = new List<PuestoDTO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id, codigo_puesto, nombre_puesto, salario, estado, fecha_creacion
                        FROM puestos
                        WHERE estado = 'Activo'
                        ORDER BY nombre_puesto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PuestoDTO puesto = new PuestoDTO
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CodigoPuesto = reader["codigo_puesto"].ToString(),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    Salario = Convert.ToDecimal(reader["salario"]),
                                    Estado = reader["estado"].ToString(),
                                    FechaCreacion = reader["fecha_creacion"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["fecha_creacion"]).ToString("yyyy-MM-dd HH:mm:ss") :
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                };
                                listaPuestos.Add(puesto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener los puestos activos: {ex.Message}");
            }

            return listaPuestos;
        }

        /// <summary>
        /// Obtiene un puesto específico por su código
        /// </summary>
        public PuestoDTO ObtenerPuestoPorCodigo(string codigo)
        {
            PuestoDTO puesto = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id, codigo_puesto, nombre_puesto, salario, estado, fecha_creacion
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
                                        Convert.ToDateTime(reader["fecha_creacion"]).ToString("yyyy-MM-dd HH:mm:ss") :
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

        /// <summary>
        /// Obtiene puestos por rango de salario (solo activos)
        /// </summary>
        public List<PuestoDTO> ObtenerPuestosPorSalario(decimal min, decimal max)
        {
            List<PuestoDTO> listaPuestos = new List<PuestoDTO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id, codigo_puesto, nombre_puesto, salario, estado, fecha_creacion
                        FROM puestos
                        WHERE estado = 'Activo' AND salario BETWEEN @min AND @max
                        ORDER BY salario DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@min", min);
                        cmd.Parameters.AddWithValue("@max", max);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PuestoDTO puesto = new PuestoDTO
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CodigoPuesto = reader["codigo_puesto"].ToString(),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    Salario = Convert.ToDecimal(reader["salario"]),
                                    Estado = reader["estado"].ToString(),
                                    FechaCreacion = reader["fecha_creacion"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["fecha_creacion"]).ToString("yyyy-MM-dd HH:mm:ss") :
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                };
                                listaPuestos.Add(puesto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener los puestos por salario: {ex.Message}");
            }

            return listaPuestos;
        }

        /// <summary>
        /// Obtiene TODOS los puestos (activos e inactivos)
        /// </summary>
        public List<PuestoDTO> ObtenerTodosLosPuestos()
        {
            List<PuestoDTO> listaPuestos = new List<PuestoDTO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT id, codigo_puesto, nombre_puesto, salario, estado, fecha_creacion
                        FROM puestos
                        ORDER BY estado DESC, nombre_puesto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PuestoDTO puesto = new PuestoDTO
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CodigoPuesto = reader["codigo_puesto"].ToString(),
                                    NombrePuesto = reader["nombre_puesto"].ToString(),
                                    Salario = Convert.ToDecimal(reader["salario"]),
                                    Estado = reader["estado"].ToString(),
                                    FechaCreacion = reader["fecha_creacion"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["fecha_creacion"]).ToString("yyyy-MM-dd HH:mm:ss") :
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                };
                                listaPuestos.Add(puesto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException($"Error al obtener todos los puestos: {ex.Message}");
            }

            return listaPuestos;
        }

        // Métodos existentes (NO MODIFICAR)
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