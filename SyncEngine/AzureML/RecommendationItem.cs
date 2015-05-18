using System.Runtime.Serialization;

namespace BoomerangX.SyncEngine.AzureML
{
    /// <summary>
    /// Utility class holding a recommended item information.
    /// </summary>
    [DataContract]
    public class RecommendedItem
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Rating { get; set; }

        [DataMember]
        public string Reasoning { get; set; }

        [DataMember]
        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Id: {1}, Rating: {2}, Reasoning: {3}", Name, Id, Rating, Reasoning);
        }
    }
}
