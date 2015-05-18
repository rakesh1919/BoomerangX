using System;
using BoomerangX.Setup;

namespace BoomerangX.SyncEngine.Tasks
{
    public abstract class TaskLibrary
    {
        private static Account currentAccountInfo;
        private LibraryType libraryInstanceType = LibraryType.Default;

        public static Account AccountInfo
        {
            get
            {
                return currentAccountInfo;
            }
            set
            {
                currentAccountInfo = value;
            }
        }

        public TaskLibrary(Account accountInfo)
        {
            currentAccountInfo = accountInfo;
        }

        public static TaskLibrary GetTaskLibrary(LibraryType libraryType)
        {
            switch(libraryType)
            {
                case LibraryType.AzureML:
                    return new AzureMLTaskLibrary(currentAccountInfo);
                case LibraryType.AXJobs:
                    return new AxJobsTaskLibrary(currentAccountInfo);
            }

            return null;
        }
        
        public LibraryType LibraryInstanceType
        {
            get
            {
                return libraryInstanceType;
            }
        }

        public abstract Action GetExecutionTask();
    }
}
