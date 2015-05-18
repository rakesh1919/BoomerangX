using System;
using BoomerangX.Utils;
using BoomerangX.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BoomerangX.Setup
{
    public class Account
    {
        private static string username = "";
        private static string accountKey = "";
        private bool initialized;
        private Logger log;

        public string UserName
        {
            get
            {
                CheckInitialized();
                return username;
            }
            set
            {
                username = value;
            }
        }

        public string AccountKey
        {
            get
            {
                CheckInitialized();
                return accountKey;
            }
            set
            {
                accountKey = value;
            }
        }

        public string CatalogFile { get; set; }
        public string UsageFile { get; set; }

        private void CheckInitialized()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Account setup must be performed before retrieving paramater values");
            }
        }

        public void Initialize()
        {
            //Hardcode the parameters for now
            UserName = "rakesh1919@hotmail.com";
            AccountKey = "Dg+gIbruJaw05IQE1YPi4c4GQe8kjWdQEV2BoYQkU4k";
            initialized = true;
            CatalogFile = @"\\axpstore\scratch\rakpar\Resources\dax_catalog.csv";
            UsageFile = @"\\axpstore\scratch\rakpar\Resources\dax_usage.csv";
            log = new Logger();
            //var blobUtils = new BlobOperationUtils(log);
            //blobUtils.CreateAndConfigure(UserName);
        }

        public void UploadFiles()
        {
           using(var blobUtils = new BlobOperationUtils(log))
            {
                List<Task<string>> fileTasks = new List<Task<string>>(2);
                fileTasks.Add(blobUtils.UploadFileAsync(CatalogFile, UserName));
                fileTasks.Add(blobUtils.UploadFileAsync(UsageFile, UserName));
                Task.WaitAll(fileTasks.ToArray());
                if(fileTasks[0].Status != TaskStatus.Faulted)
                {
                    CatalogFile = fileTasks[0].Result;
                }

                if(fileTasks[1].Status != TaskStatus.Faulted)
                {
                    UsageFile = fileTasks[1].Result;
                }
            }
        }
    }
}
