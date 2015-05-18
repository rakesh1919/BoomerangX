using System;
using System.Collections.Generic;
using BoomerangX.SyncEngine.Tasks;
using BoomerangX.Setup;

namespace BoomerangX.SyncEngine
{
    class Program
    {
        static void Main()
        {
            var taskFactory = new List<Action>();
            TaskLibrary.AccountInfo = new Account();
            TaskLibrary.AccountInfo.Initialize();
            var mlLibrary = TaskLibrary.GetTaskLibrary(LibraryType.AzureML);
            var axLibrary = TaskLibrary.GetTaskLibrary(LibraryType.AXJobs);
            taskFactory.Add(mlLibrary.GetExecutionTask());
            taskFactory.Add(axLibrary.GetExecutionTask());
            foreach(var task in taskFactory)
            {
                task();
            }
        }
    }
}
