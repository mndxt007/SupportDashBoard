using SupportEngineerEfficiencyDashboard.Models;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SupportEngineerEfficiencyDashboard.Services
{
    public interface ICaseDataService
    {
        public Task<List<CaseModel>> FetchCasesAsync();
        public Task<List<NotesModel>> FetchNotesAsync();

        public Task<List<CommunicationModel>> FetchCommunicationsAsync();
    }

    public class CaseDataService : ICaseDataService
    {
        private readonly IWebHostEnvironment environment;
        public CaseDataService(IWebHostEnvironment environment)
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
                    State = case1[headers.IndexOf("StateAnnotation")],
                    ProgramType = case1[headers.IndexOf("ProgramType")],
                });
            };

            return caseList;
        }

        public async Task<List<CommunicationModel>> FetchCommunicationsAsync()
        {
            var interactionsjson = await File.ReadAllTextAsync(Path.Combine(environment.WebRootPath, "casedata", "interactions.json"));
            var interactionsData = JsonSerializer.Deserialize<InteractionsRoot>(interactionsjson);
            return interactionsData?.CaseInteractions?.OrderByDescending(interaction => interaction.CaseEmails[0].CreatedOn).ToList()!;
        }

        public async Task<List<NotesModel>> FetchNotesAsync()
        {
            var notesjson = await File.ReadAllTextAsync(Path.Combine(environment.WebRootPath, "casedata", "notes.json"));
            var notesRoot = JsonSerializer.Deserialize<NotesRoot>(notesjson);
            notesRoot?.CaseNotes.ForEach(x => x.Notes.OrderByDescending(y => y.CreatedOn));
            return notesRoot?.CaseNotes ?? [];
        }
    }
}
