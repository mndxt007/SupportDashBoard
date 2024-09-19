using System.Text.Json.Serialization;

namespace SupportEngineerEfficiencyDashboard.Models
{
    public class NotesModel
    {
        [JsonPropertyName("CaseNumber")]
        public string CaseNumber { get; set; }
        [JsonPropertyName("Notes")]
        public List<NoteItem> Notes { get; set; }
    }

    public class NotesRoot
    {
        [JsonPropertyName("CaseNotes")]
        public List<NotesModel> CaseNotes { get; set; }
    }

    public class NoteItem
    {
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string UpdatedBy { get; set; }
        //public string UpdatedOn { get; set; }
        //public string id { get; set; }
    }
}