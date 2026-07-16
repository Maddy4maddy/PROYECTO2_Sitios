using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PROYECTO2_WEBService
{
    [ServiceContract]
    public interface IService1
    {
        // Método para obtener todos los puestos activos
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "ObtenerPuestosActivos")]
        List<PuestoDTO> ObtenerPuestosActivos();

        // Método para obtener puesto por código
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "ObtenerPuestoPorCodigo?codigo={codigo}")]
        PuestoDTO ObtenerPuestoPorCodigo(string codigo);

        // Método para obtener puestos por rango de salario
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "ObtenerPuestosPorSalario?min={min}&max={max}")]
        List<PuestoDTO> ObtenerPuestosPorSalario(decimal min, decimal max);

        // Método para obtener todos los puestos (activos e inactivos)
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "ObtenerTodosLosPuestos")]
        List<PuestoDTO> ObtenerTodosLosPuestos();

        // Métodos existentes (mantener)
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    [DataContract]
    public class PuestoDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string CodigoPuesto { get; set; }

        [DataMember]
        public string NombrePuesto { get; set; }

        [DataMember]
        public decimal Salario { get; set; }

        [DataMember]
        public string Estado { get; set; } // "Activo" o "Inactivo"

        [DataMember]
        public string FechaCreacion { get; set; }
    }

    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}