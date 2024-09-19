using System.Text.Json.Serialization;

namespace SupportEngineerEfficiencyDashboard.Models
{
    public class CaseNote
    {
        [JsonPropertyName("CaseNumber")]
        public string CaseNumber { get; set; }
        [JsonPropertyName("Notes")]
        public List<NoteModel> Notes { get; set; }

    }

    public class NotesRoot
    {
        [JsonPropertyName("CaseNotes")]
        public List<CaseNote> CaseNotes { get; set; }
    }

    public class NoteModel
    {
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        //public string UpdatedBy { get; set; }
        //public string UpdatedOn { get; set; }
        //public string id { get; set; }
    }
}