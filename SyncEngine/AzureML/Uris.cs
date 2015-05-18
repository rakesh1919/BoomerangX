namespace BoomerangX.SyncEngine.AzureML
{
    /// <summary>
    /// holds the API uri.
    /// </summary>
    public static class Uris
    {

        public const string GetAllModelsUrl = "GetAllModels?apiVersion=%271.0%27";

        public const string CreateModelUrl = "CreateModel?modelName=%27{0}%27&apiVersion=%271.0%27";
        public const string DeleteModelUrl = "DeleteModel?id=%27{0}%27&apiVersion=%271.0%27";
        public const string ImportCatalog =
            "ImportCatalogFile?modelId=%27{0}%27&filename=%27{1}%27&apiVersion=%271.0%27";


        public const string ImportUsage =
            "ImportUsageFile?modelId=%27{0}%27&filename=%27{1}%27&apiVersion=%271.0%27";

        public const string BuildModel =
            "BuildModel?modelId=%27{0}%27&userDescription=%27{1}%27&apiVersion=%271.0%27";

        public const string BuildStatuses = "GetModelBuildsStatus?modelId=%27{0}%27&onlyLastBuild={1}&apiVersion=%271.0%27";

        public const string GetRecommendation =
            "ItemRecommend?modelId=%27{0}%27&itemIds=%27{1}%27&numberOfResults={2}&includeMetadata={3}&apiVersion=%271.0%27";

        public const string UpdateModel = "UpdateModel?id=%27{0}%27&apiVersion=%271.0%27";

    }
}
