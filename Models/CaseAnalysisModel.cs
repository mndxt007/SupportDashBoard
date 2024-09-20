namespace SupportEngineerEfficiencyDashboard.Models
{
    public class CaseAnalysisModel
    {
        public string Action { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Sentiment { get; set; } = string.Empty;
        public int Priority { get; set; } = int.MinValue;
        public int Health { get; set; } = int.MinValue;
    }
}
