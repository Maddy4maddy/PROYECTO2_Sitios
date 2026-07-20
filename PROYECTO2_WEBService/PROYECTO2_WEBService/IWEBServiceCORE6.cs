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
    public interface IWEBServiceCORE6
    {
        [OperationContract]
        [WebGet(UriTemplate = "ObtenerPuestoPorCodigo?codigo={codigo}",
                ResponseFormat = WebMessageFormat.Json)]
        PuestoDTO ObtenerPuestoPorCodigo(string codigo);
    }
}