using SupportEngineerEfficiencyDashboard.Models;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SupportEngineerEfficiencyDashboard.Services
{
    public interface ICaseService
    {
        public Task<List<CaseModel>> FetchCasesAsync();
        public Task<List<CaseNote>> FetchNotesAsync();

        public Task<List<CaseInteraction>> FetchInteractionsAsync();
    }

    public class CaseService : ICaseService
    {
        private readonly IWebHostEnvironment environment;
        public CaseService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        public async Task<List<CaseModel>> FetchCasesAsync()
        {
            var cases = await File.ReadAllTextAsync(Path.Combine(environment.WebRootPath, "casedata", "cases.json"));
            var caseRoot = JsonSerializer.Deserialize<CaseRoot>(cases);

            var caseproperties = caseRoot.TableParameters[0].TableParameterResult;
            var headers = caseRoot.TableParameters[0].HeaderNames;

            var caseList = new List<CaseModel>();
            foreach (var case1 in caseproperties)
            {
                caseList.Add(new CaseModel
                {
                    CaseNumber = case1[headers.IndexOf("CaseNumber")],
                    Title = case1[headers.IndexOf("Title")],
                    IssueDescription = case1[headers.IndexOf("IssueDescription")],
                    CaseAge = TimeSpan.FromMinutes(Double.Parse(case1[headers.IndexOf("CaseAge")])),
                    ProgramType = case1[headers.IndexOf("ProgramType")],
                });
            };

            return caseList;
        }

        public async Task<List<CaseInteraction>> FetchInteractionsAsync()
        {
            var interactionsjson = await File.ReadAllTextAsync(Path.Combine(environment.WebRootPath, "casedata", "interactions.json"));
            var interactionsData = JsonSerializer.Deserialize<InteractionsRoot>(interactionsjson);
            return interactionsData?.CaseInteractions?.OrderByDescending(interaction => interaction.Interaction[0].CreatedOn).ToList()!;
        }

        public async Task<List<CaseNote>> FetchNotesAsync()
        {
            var notesjson = await File.ReadAllTextAsync(Path.Combine(environment.WebRootPath, "casedata", "notes.json"));
            var notesRoot = JsonSerializer.Deserialize<NotesRoot>(notesjson);
            notesRoot?.CaseNotes.ForEach(x => x.Notes.OrderByDescending(y => y.CreatedOn));
            return notesRoot?.CaseNotes ?? [];
        }
    }
}
