using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PROYECTO2_WEBService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IWEBServiceCORE6" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IWEBServiceCORE6
    {
        [OperationContract]
        void DoWork();
    }
}
