using System.Text.Json.Serialization;

namespace SupportEngineerEfficiencyDashboard.Models
{
    public class CaseModel
    {
        public string CaseNumber { get; set; }
        public string Title { get; set; }
        public string IssueDescription { get; set; }
        public TimeSpan CaseAge { get; set; }
        public string ProgramType { get; set; }
    }

    public class TableParameters
    {
        [JsonPropertyName("header_names")]
        public List<string> HeaderNames { get; set; }
        [JsonPropertyName("table_parameter_result")]
        public List<List<string>> TableParameterResult { get; set; }
        [JsonPropertyName("row_count")]
        public int RowCount { get; set; }
    }

    public class CaseRoot
    {
        [JsonPropertyName("table_parameters")]
        public List<TableParameters> TableParameters { get; set; }
        [JsonPropertyName("query_result")]
        public String QueryResult { get; set; }
    }
}