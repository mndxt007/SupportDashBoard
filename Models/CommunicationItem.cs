using System.Text.Json.Serialization;

namespace SupportEngineerEfficiencyDashboard.Models
{
    public class CommunicationModel
    {
        [JsonPropertyName("CaseNumber")]
        public string CaseNumber { get; set; }
        [JsonPropertyName("Interaction")]
        public List<CommunicationItem> Interaction { get; set; }

    }

    public class InteractionsRoot
    {
        [JsonPropertyName("InteractionsRoot")]
        public List<CommunicationModel> CaseInteractions { get; set; }
    }

    public class CommunicationItem
    {
        public string FromAddress { get; set; }
        public List<string> ToAddresses { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Direction { get; set; }
        public string Summary { get; set; }
        //public string UpdatedOn { get; set; }
        //public string id { get; set; }
    }
}