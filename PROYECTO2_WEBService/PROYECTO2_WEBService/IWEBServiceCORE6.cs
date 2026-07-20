using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PROYECTO2_WEBService
{
    [ServiceContract]
    public interface IWEBServiceCORE6
    {
        [OperationContract]
        [WebGet(
            UriTemplate = "ObtenerPuestoPorCodigo?codigo={codigo}",
            ResponseFormat = WebMessageFormat.Json)]
        PuestoDTO ObtenerPuestoPorCodigo(string codigo);


        [OperationContract]
        [WebGet(
            UriTemplate = "ObtenerPuestosActivos",
            ResponseFormat = WebMessageFormat.Json)]
        List<PuestoDTO> ObtenerPuestosActivos();
    }
}