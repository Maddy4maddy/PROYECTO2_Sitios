using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PROYECTO2_WEBService
{
    [ServiceContract]
    public interface IWEBServiceCORE3
    {
        /*
         * Petición previa que realiza el navegador antes del POST.
         * Es necesaria porque el frontend se ejecuta en localhost:8000
         * y el servicio WCF en localhost:61932.
         */
        [OperationContract]
        [WebInvoke(
            Method = "OPTIONS",
            UriTemplate = "CrearEmpleado"
        )]
        void OpcionesCrearEmpleado();

        /*
         * Registra un nuevo empleado.
         */
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "CrearEmpleado"
        )]
        CrearEmpleadoResponse CrearEmpleado(
            CrearEmpleadoRequest request
        );
    }

    [DataContract]
    public class CrearEmpleadoRequest
    {
        [DataMember(IsRequired = true)]
        public string NumeroEmpleado { get; set; }

        [DataMember(IsRequired = true)]
        public string Identificacion { get; set; }

        [DataMember(IsRequired = true)]
        public string TipoIdentificacion { get; set; }

        [DataMember(IsRequired = true)]
        public string NombreCompleto { get; set; }

        [DataMember(IsRequired = true)]
        public string FechaNacimiento { get; set; }

        [DataMember(IsRequired = true)]
        public string Correo { get; set; }

        [DataMember(IsRequired = true)]
        public string Telefono { get; set; }

        [DataMember(IsRequired = true)]
        public int IdPuesto { get; set; }

        [DataMember(IsRequired = true)]
        public string FechaContratacion { get; set; }

        [DataMember]
        public string Estado { get; set; }
    }

    [DataContract]
    public class CrearEmpleadoResponse
    {
        [DataMember]
        public bool Exito { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public int IdEmpleado { get; set; }

        [DataMember]
        public string NumeroEmpleado { get; set; }
    }
}