using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using BoomerangX.Logging;
using BoomerangX.SyncEngine.AzureML;
using BoomerangX.Utils;
using BoomerangX.Setup;
using System.Threading.Tasks;

namespace BoomerangX.SyncEngine.Tasks
{
    public class AzureMLTaskLibrary : TaskLibrary
    {
        private HttpClient httpClient;
        private const string RootUri = "https://api.datamarket.azure.com/amla/recommendations/v2/";
        private string modelName = string.Empty;
        private bool inited;
        private Logger log = new Logger();
        private ModelBuildStatus lastBuildStatus;
        private string lastModelId;

        public AzureMLTaskLibrary(Account accountInfo) : base(accountInfo)
        {
        }

        public HttpClient HttpClient
        {
            get
            {
                return httpClient;
            }

            set
            {
                httpClient = value;
            }
        }

        public string catalogFile { get; private set; }
        public string usageFile { get; private set; }



        /// <summary>
        /// Initialize the sample app
        /// </summary>
        /// <param name="username">the username (email)</param>
        /// <param name="accountKey"></param>
        private void Init(string username, string accountKey)
        {
            if (!inited)
            {
                HttpClient = new HttpClient();
                var pass = GeneratePass(username, accountKey);
                log.Information("Generated AccessKey: {0}", pass);
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", pass);
                HttpClient.BaseAddress = new Uri(RootUri);
                inited = true;
                catalogFile = AccountInfo.CatalogFile;
                usageFile = AccountInfo.UsageFile;
            }
        }

        /// <summary>
        /// create the model with the given name.
        /// </summary>
        /// <returns>The model id</returns>
        private string CreateModel(string modelName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(Uris.CreateModelUrl, modelName));
            var response = HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to create model {1}, \n reason {2}",
                    response.StatusCode, modelName, response.ReasonPhrase));
            }

            //process response if success
            string modelId = null;

            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, "//a:entry/a:content/m:properties/d:Id");
            if (node != null)
                modelId = node.InnerText;

            return modelId;
        }

        private void DeleteModel(string modelId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, String.Format(Uris.DeleteModelUrl, modelId));
            var response = HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to create model {1}, \n reason {2}",
                    response.StatusCode, modelName, response.ReasonPhrase));
            }

        }


        /// <summary>
        /// create the model with the given name.
        /// </summary>
        /// <returns>The model id</returns>
        private string GetModels()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Uris.GetAllModelsUrl);
            var response = HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to create model {1}, \n reason {2}",
                    response.StatusCode, modelName, response.ReasonPhrase));
            }

            //process response if success
            string modelId = null;

            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, "//a:entry/a:content/m:properties/d:Id");
            if (node != null)
                modelId = node.InnerText;

            return modelId;
        }

        /// <summary>
        /// Trigger a build for the given model
        /// </summary>
        /// <param name="modelId">the model id</param>
        /// <param name="buildDescription">a description for the build</param>
        /// <returns>the id of the triggered build</returns>
        private string BuildModel(string modelId, string buildDescription)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(Uris.BuildModel, modelId, buildDescription));
            var response = HttpClient.SendAsync(request).Result;


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to start build for model {1}, \n reason {2}",
                    response.StatusCode, modelId, response.ReasonPhrase));
            }
            string buildId = null;
            //process response if success
            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, "//a:entry/a:content/m:properties/d:Id");
            if (node != null)
                buildId = node.InnerText;

            return buildId;

        }

        /// <summary>
        /// Retrieve the build status for the given build
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="buildId"></param>
        /// <returns></returns>
        private ModelBuildStatus GetBuidStatus(string modelId, string buildId)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, String.Format(Uris.BuildStatuses, modelId, false));
            var response = HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to retrieve build for status for model {1} and build id {2}, \n reason {3}",
                    response.StatusCode, modelId, buildId, response.ReasonPhrase));
            }
            string buildStatusStr = null;
            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, string.Format("//a:entry/a:content/m:properties[d:BuildId='{0}']/d:Status", buildId));
            if (node != null)
                buildStatusStr = node.InnerText;

            ModelBuildStatus buildStatus;
            if (!Enum.TryParse(buildStatusStr, true, out buildStatus))
            {
                throw new Exception(string.Format("Failed to parse build status for value {0} of build {1} for model {2}", buildStatusStr, buildId, modelId));
            }

            return buildStatus;
        }


        /// <summary>
        /// Update model information
        /// </summary>
        /// <param name="modelId">the id of the model</param>
        /// <param name="description">the model description (optional)</param>
        /// <param name="activeBuildId">the id of the build to be active (optional)</param>
        private void UpdateModel(string modelId, string description, string activeBuildId)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, String.Format(Uris.UpdateModel, modelId));

            var sb = new StringBuilder("<ModelUpdateParams xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendFormat("<Description>{0}</Description>", description);
            }
            if (!string.IsNullOrEmpty(activeBuildId))
            {
                sb.AppendFormat("<ActiveBuildId>{0}</ActiveBuildId>", activeBuildId);
            }
            sb.Append("</ModelUpdateParams>");

            request.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));
            var response = HttpClient.SendAsync(request).Result;


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to update model for model {1}, \n reason {2}",
                    response.StatusCode, modelId, response.ReasonPhrase));
            }

        }
        /// <summary>
        /// Retrieve recommendation for the given item(s)
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="itemIdList"></param>
        /// <param name="numberOfResult">the number of result to include in the response</param>
        /// <param name="includeMetadata">true, means meta data will be returned too</param>
        /// <returns>a collection of recommended items</returns>
        public IEnumerable<RecommendedItem> GetRecommendation(string modelId, List<string> itemIdList, int numberOfResult,
            bool includeMetadata = false)
        {
            //For direct calls to GetReco
            if (!inited)
            {
                Task initTask = Task.Run(() =>
                {
                    Init(AccountInfo.UserName, AccountInfo.AccountKey);
                });
                initTask.Wait();
                lastBuildStatus = ModelBuildStatus.Success;
            }

            if(string.IsNullOrEmpty(modelId))
            {
                modelId = "88a91592-b831-47d9-9049-02e9a33ee23e";
            }

            var request = new HttpRequestMessage(HttpMethod.Get,
                String.Format(Uris.GetRecommendation, modelId, string.Join(",", itemIdList), numberOfResult,
                    includeMetadata));
            var response = HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    String.Format(
                        "Error {0}: Failed to retrieve recommendation for item list {1} and model {2}, \n reason {3}",
                        response.StatusCode, string.Join(",", itemIdList), modelId, response.ReasonPhrase));
            }
            var recoList = new List<RecommendedItem>();


            var nodeList = XmlUtils.ExtractXmlElementList(response.Content.ReadAsStreamAsync().Result, "//a:entry/a:content/m:properties");

            foreach (var node in (nodeList))
            {
                var item = new RecommendedItem();
                //cycle through the recommended items
                foreach (var child in ((XmlElement)node).ChildNodes)
                {
                    //cycle through properties
                    var nodeName = ((XmlNode)child).LocalName;
                    switch (nodeName)
                    {
                        case "Id":
                            item.Id = ((XmlNode)child).InnerText;
                            break;
                        case "Name":
                            item.Name = ((XmlNode)child).InnerText;
                            break;
                        case "Rating":
                            item.Rating = ((XmlNode)child).InnerText;
                            break;
                        case "Reasoning":
                            item.Reasoning = ((XmlNode)child).InnerText;
                            break;
                    }

                }
                recoList.Add(item);
            }
            return recoList;
        }


        /// <summary>
        /// Invoke some recommendation on 
        /// </summary>
        /// <param name="modelId">the model id</param>
        /// <param name="seedItems">a list of item to get recommendation</param>
        /// <param name="useList">if true all the item are used to get a recommendation on the whole set,
        ///  if false use each item of the list to get resommendations</param>
        private void InvokeRecommendations(string modelId, List<CatalogItem> seedItems, bool useList)
        {
            if (useList)
            {
                var recoItems = GetRecommendation(modelId, seedItems.Select(i => i.Id).ToList(), 10);
                log.Information("\tRecommendations for [{0}]", string.Join("],[", seedItems));
                foreach (var recommendedItem in recoItems)
                {
                    log.Information("\t  {0}", recommendedItem);
                }
            }
            else
            {
                foreach (var item in seedItems)
                {
                    var recoItems = GetRecommendation(modelId, new List<string> { item.Id }, 10);
                    log.Information("Recommendation for '{0}'", item);
                    foreach (var recommendedItem in recoItems)
                    {
                        log.Information("\t  {0}", recommendedItem);
                    }
                    log.Information("\n");
                }
            }
        }

        /// <summary>
        /// Generate the key to allow accessing DM API
        /// </summary>
        /// <param name="email">the user email</param>
        /// <param name="accountKey">the user account key</param>
        /// <returns></returns>
        private string GeneratePass(string email, string accountKey)
        {
            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", email, accountKey));
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// Import the given file (catalog/usage) to the given model. 
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="filePath"></param>
        /// <param name="importUri"></param>
        /// <returns></returns>
        private ImportReport ImportFile(string modelId, string filePath, string importUri)
        {
            var filestream = new FileStream(filePath, FileMode.Open);
            var fileName = Path.GetFileName(filePath);
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(importUri, modelId, fileName));

            request.Content = new StreamContent(filestream);
            var response = HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    String.Format("Error {0}: Failed to import file {1}, for model {2} \n reason {3}",
                        response.StatusCode, filePath, modelId, response.ReasonPhrase));
            }

            //process response if success
            var nodeList = XmlUtils.ExtractXmlElementList(response.Content.ReadAsStreamAsync().Result,
                "//a:entry/a:content/m:properties/*");

            var report = new ImportReport { Info = fileName };
            foreach (XmlNode node in nodeList)
            {
                if ("LineCount".Equals(node.LocalName))
                {
                    report.LineCount = int.Parse(node.InnerText);
                }
                if ("ErrorCount".Equals(node.LocalName))
                {
                    report.ErrorCount = int.Parse(node.InnerText);
                }
            }
            return report;
        }

        private void SetupML()
        {
            DeleteModel("b3740c8b-4acd-4140-9459-fccad247e59a");
            var rand = new Random();
            modelName = string.Format("modelTest_{0}", rand.Next());

            //Create a model container
            log.Information("\nCreating model container {0}...", modelName);
            lastModelId = CreateModel(modelName);
            log.Information("\tModel '{0}' created with ID: {1}", modelName, lastModelId);

            //Import data to the container
            log.Information("\nImporting catalog and usage data...");
            log.Information("\timport catalog...");
            var report = ImportFile(lastModelId, catalogFile, Uris.ImportCatalog);
            log.Information("\t{0}", report);
            log.Information("\timport usage...");
            report = ImportFile(lastModelId, usageFile, Uris.ImportUsage);
            log.Information("\t{0}", report);

            //Trigger a build to produce a recommendation model.
            log.Information("\nTrigger build for model '{0}'", lastModelId);
            var buildId = BuildModel(lastModelId, "build of " + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            log.Information("\ttriggered build id '{0}'", buildId);
            log.Information("\nMonitoring build '{0}'", buildId);
            //monitor the current triggered build
            lastBuildStatus = ModelBuildStatus.Create;
            bool monitor = true;
            while (monitor)
            {
                lastBuildStatus = GetBuidStatus(lastModelId, buildId);

                log.Information("\tbuild '{0}' (model '{1}'): status {2}", buildId, lastModelId, lastBuildStatus);
                if (lastBuildStatus != ModelBuildStatus.Error && lastBuildStatus != ModelBuildStatus.Cancelled && lastBuildStatus != ModelBuildStatus.Success)
                {
                    log.Information(" --> will check in 5 sec...");
                    Thread.Sleep(5000);
                }
                else
                {
                    monitor = false;
                }
            }

            log.Information("\n\tBuild {0} ended with status {1}", buildId, lastBuildStatus);

            //The below api is more meaningful when you want to give a cetain build id to be an active build.
            //currently this app has a single build which is already active.
            log.Information("\nUpdating model description to 'new model'");
            UpdateModel(lastModelId, "new model", null);
            if (lastBuildStatus != ModelBuildStatus.Success)
            {
                log.Error("Build {0} did not end successfully, the sample app will stop here.", buildId);
            }
        }

        private void DemoRecommendations()
        {
            if (!inited)
            {
                Task initTask = Task.Run(() =>
                {
                    Init(AccountInfo.UserName, AccountInfo.AccountKey);
                });
                initTask.Wait();
                lastBuildStatus = ModelBuildStatus.Success;
                lastModelId = "88a91592-b831-47d9-9049-02e9a33ee23e";
            }

            if (lastBuildStatus == ModelBuildStatus.Success)
            {
                log.Information("\nGetting some recommendations...");

                // get recommendations
                var seedItems = new List<CatalogItem>
                    {
                    // These item data were extracted from the catalog file in the resource folder.
                    new CatalogItem() {Id = "5637145088", Name = "Satellite Speaker Model 01"},
                        new CatalogItem() {Id = "5637145144", Name = "Center Channel Speaker Model 01"}
                    };

                log.Information("\t for single seed item");
                // show usage for single item
                InvokeRecommendations(lastModelId, seedItems, false);
                log.Information("\n\n\t for a set of seed item");
                // show usage for a list of items
                InvokeRecommendations(lastModelId, seedItems, true);
            }
        }
        public override Action GetExecutionTask()
        {
            return delegate ()
            {
                Task initTask = Task.Run(() =>
                {
                    Init(AccountInfo.UserName, AccountInfo.AccountKey);
                });
                initTask.Wait();
                try
                {
                    SetupML();
                    DemoRecommendations();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            };
        }
    }
}