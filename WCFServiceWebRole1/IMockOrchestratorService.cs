using BoomerangX.SyncEngine.AzureML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MockOrchestartor.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IMockOrchestratorService
    {

        [OperationContract]
        string GetLastOperationResult();

        [OperationContract]
        void SetupMachineLearning(Guid userIdentifier);

        [OperationContract]
        RecommendedItem GetRecommendation(CatalogItem item);

        // TODO: Add your service operations here
    }
}
