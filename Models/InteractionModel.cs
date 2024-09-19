using System.Text.Json.Serialization;

namespace SupportEngineerEfficiencyDashboard.Models
{
    public class CaseInteraction
    {
        [JsonPropertyName("CaseNumber")]
        public string CaseNumber { get; set; }
        [JsonPropertyName("Interaction")]
        public List<InteractionModel> Interaction { get; set; }

    }

    public class InteractionsRoot
    {
        [JsonPropertyName("InteractionsRoot")]
        public List<CaseInteraction> CaseInteractions { get; set; }
    }


    public class InteractionModel
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