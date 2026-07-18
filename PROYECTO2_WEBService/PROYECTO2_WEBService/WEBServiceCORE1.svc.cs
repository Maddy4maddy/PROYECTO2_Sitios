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
        // CONFIGURAR CORS
        // ========================================

        private void ConfigurarCors()
        {

            WebOperationContext.Current
            .OutgoingResponse
            .Headers.Add(
                "Access-Control-Allow-Origin",
                "http://localhost:8000"
            );


            WebOperationContext.Current
            .OutgoingResponse
            .Headers.Add(
                "Access-Control-Allow-Methods",
                "GET, OPTIONS"
            );


            WebOperationContext.Current
            .OutgoingResponse
            .Headers.Add(
                "Access-Control-Allow-Headers",
                "Content-Type"
            );

        }



        // ========================================
        // PUESTOS ACTIVOS
        // ========================================

        public List<PuestoDTO> ObtenerPuestosActivos()
        {

            ConfigurarCors();


            List<PuestoDTO> lista =
                new List<PuestoDTO>();


            try
            {

                using (MySqlConnection conn =
                    new MySqlConnection(connectionString))
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
                    WHERE estado='Activo'
                    ORDER BY nombre_puesto";


                    using (MySqlCommand cmd =
                        new MySqlCommand(query, conn))
                    {

                        using (MySqlDataReader reader =
                            cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                lista.Add(new PuestoDTO
                                {

                                    Id =
                                    Convert.ToInt32(reader["id"]),


                                    CodigoPuesto =
                                    reader["codigo_puesto"].ToString(),


                                    NombrePuesto =
                                    reader["nombre_puesto"].ToString(),


                                    Salario =
                                    Convert.ToDecimal(reader["salario"]),


                                    Estado =
                                    reader["estado"].ToString(),


                                    FechaCreacion =
                                    reader["fecha_creacion"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["fecha_creacion"])
                                    .ToString("yyyy-MM-dd HH:mm:ss")
                                    :
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

                                });

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                throw new FaultException(
                    $"Error al obtener puestos activos: {ex.Message}");

            }


            return lista;

        }




        // ========================================
        // PUESTO POR CODIGO
        // ========================================

        public PuestoDTO ObtenerPuestoPorCodigo(string codigo)
        {

            ConfigurarCors();


            PuestoDTO puesto = null;


            try
            {

                using (MySqlConnection conn =
                    new MySqlConnection(connectionString))
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
                    WHERE codigo_puesto=@codigo";


                    using (MySqlCommand cmd =
                        new MySqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue(
                            "@codigo",
                            codigo);


                        using (MySqlDataReader reader =
                            cmd.ExecuteReader())
                        {

                            if (reader.Read())
                            {

                                puesto = new PuestoDTO
                                {

                                    Id =
                                    Convert.ToInt32(reader["id"]),


                                    CodigoPuesto =
                                    reader["codigo_puesto"].ToString(),


                                    NombrePuesto =
                                    reader["nombre_puesto"].ToString(),


                                    Salario =
                                    Convert.ToDecimal(reader["salario"]),


                                    Estado =
                                    reader["estado"].ToString(),


                                    FechaCreacion =
                                    Convert.ToDateTime(reader["fecha_creacion"])
                                    .ToString("yyyy-MM-dd HH:mm:ss")

                                };

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                throw new FaultException(
                    $"Error al obtener puesto: {ex.Message}");

            }


            return puesto;

        }




        // ========================================
        // POR SALARIO
        // ========================================

        public List<PuestoDTO> ObtenerPuestosPorSalario(
            decimal min,
            decimal max)
        {

            ConfigurarCors();


            List<PuestoDTO> lista =
                new List<PuestoDTO>();


            try
            {

                using (MySqlConnection conn =
                    new MySqlConnection(connectionString))
                {

                    conn.Open();


                    string query = @"
                    SELECT *
                    FROM puestos
                    WHERE estado='Activo'
                    AND salario BETWEEN @min AND @max
                    ORDER BY salario DESC";


                    using (MySqlCommand cmd =
                        new MySqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@min", min);
                        cmd.Parameters.AddWithValue("@max", max);


                        using (MySqlDataReader reader =
                            cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                lista.Add(new PuestoDTO
                                {

                                    Id =
                                    Convert.ToInt32(reader["id"]),


                                    CodigoPuesto =
                                    reader["codigo_puesto"].ToString(),


                                    NombrePuesto =
                                    reader["nombre_puesto"].ToString(),


                                    Salario =
                                    Convert.ToDecimal(reader["salario"]),


                                    Estado =
                                    reader["estado"].ToString(),


                                    FechaCreacion =
                                    Convert.ToDateTime(reader["fecha_creacion"])
                                    .ToString("yyyy-MM-dd HH:mm:ss")

                                });

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                throw new FaultException(
                    $"Error al obtener por salario: {ex.Message}");

            }


            return lista;

        }




        // ========================================
        // TODOS LOS PUESTOS
        // ========================================

        public List<PuestoDTO> ObtenerTodosLosPuestos()
        {

            ConfigurarCors();


            List<PuestoDTO> lista =
                new List<PuestoDTO>();


            try
            {

                using (MySqlConnection conn =
                    new MySqlConnection(connectionString))
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
                    ORDER BY estado DESC,nombre_puesto";



                    using (MySqlCommand cmd =
                        new MySqlCommand(query, conn))
                    {

                        using (MySqlDataReader reader =
                            cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                lista.Add(new PuestoDTO
                                {

                                    Id =
                                    Convert.ToInt32(reader["id"]),


                                    CodigoPuesto =
                                    reader["codigo_puesto"].ToString(),


                                    NombrePuesto =
                                    reader["nombre_puesto"].ToString(),


                                    Salario =
                                    Convert.ToDecimal(reader["salario"]),


                                    Estado =
                                    reader["estado"].ToString(),


                                    FechaCreacion =
                                    Convert.ToDateTime(reader["fecha_creacion"])
                                    .ToString("yyyy-MM-dd HH:mm:ss")

                                });

                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                throw new FaultException(
                    $"Error al obtener todos los puestos: {ex.Message}");

            }


            return lista;

        }




        // ========================================
        // OPTIONS CORS
        // ========================================

        public void Options()
        {

            WebOperationContext.Current
            .OutgoingResponse
            .StatusCode =
            System.Net.HttpStatusCode.OK;


            ConfigurarCors();

        }




        // ========================================
        // MÉTODOS ORIGINALES
        // ========================================

        public string GetData(int value)
        {
            return string.Format(
                "You entered: {0}",
                value);
        }



        public CompositeType GetDataUsingDataContract(
            CompositeType composite)
        {

            if (composite == null)
            {
                throw new ArgumentNullException(
                    "composite");
            }


            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }


            return composite;

        }

    }
}