using System;
using BoomerangX.Setup;

namespace BoomerangX.SyncEngine.Tasks
{
    public class AxJobsTaskLibrary : TaskLibrary
    {
        public AxJobsTaskLibrary(Account accountInfo) : base(accountInfo)
        {
        }

        public override Action GetExecutionTask()
        {
            return delegate ()
            {
                //nothing yet
            };
        }
    }
}