using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using BoomerangX.SyncEngine.AzureML;
using BoomerangX.SyncEngine.Tasks;
using BoomerangX.Setup;

namespace MockOrchestartor.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class MachineLearning : IMockOrchestratorService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public string GetLastOperationResult()
        {
            throw new NotImplementedException();
        }

        public RecommendedItem GetRecommendation(CatalogItem item)
        {
            throw new NotImplementedException();
        }

        public void SetupMachineLearning(Guid userIdentifier)
        {
            var taskFactory = new List<Action>();
            TaskLibrary.AccountInfo = new Account();
            TaskLibrary.AccountInfo.Initialize();
            var mlLibrary = TaskLibrary.GetTaskLibrary(LibraryType.AzureML);
            var axLibrary = TaskLibrary.GetTaskLibrary(LibraryType.AXJobs);
            taskFactory.Add(mlLibrary.GetExecutionTask());
            taskFactory.Add(axLibrary.GetExecutionTask());
            foreach (var task in taskFactory)
            {
                task();
            }
        }
    }
}
