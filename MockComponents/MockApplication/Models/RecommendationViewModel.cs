using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoomerangX.SyncEngine.AzureML;

namespace MockApplication.Models
{
    public class RecommendationViewModel
    {
        public string Name { get; set; }

        public string Rating { get; set; }
        public string Reasoning { get; set; }
        public string Id { get; set; }
    }
}