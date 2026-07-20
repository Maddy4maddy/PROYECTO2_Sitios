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
    public interface IWEBServiceCORE8
    {
        [OperationContract]
        [WebGet(UriTemplate = "ObtenerOferente?codigo={codigo}",
                ResponseFormat = WebMessageFormat.Json)]
        OferenteDTO ObtenerOferente(string codigo);
    }

    [DataContract]
    public class OferenteDTO
    {
        [DataMember]
        public string CodigoOferente { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string TipoIdentificacion { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public string FechaNacimiento { get; set; }

        [DataMember]
        public string Correo { get; set; }

        [DataMember]
        public string Telefono { get; set; }
    }
}