using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.SemanticKernel;
using SupportEngineerEfficiencyDashboard.Models;
using System.Text.Json;

namespace SupportEngineerEfficiencyDashboard.Services
{
    public class CaseAnalysisService
    {
        private readonly Kernel _kernel;
        private readonly ICaseDataService _caseService;

        public CaseAnalysisService(Kernel kernel, ICaseDataService caseService)
        {
            _kernel = kernel;
            _caseService = caseService;
        }

        public async Task<CaseAnalysisModel> AnalyzeCase(CaseModel caseModel)
        {
            var argument = new KernelArguments()
            {
                ["actions"] = JsonSerializer.Serialize(new Actions()),
                ["emails"] = JsonSerializer.Serialize(caseModel.CommunicationModel.CaseEmails),
                ["notes"] = JsonSerializer.Serialize(caseModel.NotesModel.Notes),
            };

            var caseAnalysis = await _kernel.InvokeAsync("plugins", "notes_summary", argument);

            return JsonSerializer.Deserialize<CaseAnalysisModel>(caseAnalysis.ToString()) ?? new();
        }
    }
}
