using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PROYECTO2_WEBService
{
    [ServiceContract]
    public interface IWEBSERVICEcore4
    {

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "Login")]
        LoginResponse Login(LoginRequest request);


        [OperationContract]
        [WebInvoke(
            Method = "OPTIONS",
            UriTemplate = "*")]
        void Options();

    }


    [DataContract]
    public class LoginRequest
    {
        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public string Contrasena { get; set; }
    }


    [DataContract]
    public class LoginResponse
    {
        [DataMember]
        public bool Exito { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public int IdUsuario { get; set; }

        [DataMember]
        public string Nombre { get; set; }
    }
}