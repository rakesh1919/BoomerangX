using System.Runtime.Serialization;

namespace BoomerangX.SyncEngine.AzureML
{
    /// <summary>
    /// hold catalog item info  (partial)
    /// </summary>
    [DataContract]
    public class CatalogItem
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}", Id, Name);
        }
    }
}
