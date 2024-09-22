using Microsoft.FluentUI.AspNetCore.Components;
using SupportEngineerEfficiencyDashboard.Models;
using System.Diagnostics;

namespace SupportEngineerEfficiencyDashboard.Components.Pages
{
    partial class Home
    {
        private IDialogReference? _dialog;
        private List<CaseModel> caseModels = new();
        private List<CommunicationModel> communicationModel = new();
        private List<NotesModel> notesModel = new();

        private CaseModel? selectedCase;
        private bool isCaseSelected => selectedCase == null;
        bool loading = true;

        protected override async Task OnInitializedAsync()
        {
            caseModels = await CaseService.FetchCasesAsync();
            communicationModel = await CaseService.FetchCommunicationsAsync();
            notesModel = await CaseService.FetchNotesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                await OpenSplashDefaultAsync();
                await FetchCaseAnalysisAsync(caseModels);
                loading = false;
                StateHasChanged();
            }
        }

        private async Task OpenSplashDefaultAsync()
        {
            Debug.WriteLine($"Open default SplashScreen for 4 seconds");
            DialogParameters<SplashScreenContent> parameters = new()
            {
                Content = new()
                {
                    DisplayTime = 0,    // See Task.Delay below
                    Title = "Analysing Cases...",
                    LoadingText = "Loading...",
                    Logo = FluentSplashScreen.LOGO,

                },
                PreventDismissOnOverlayClick = true,
                Modal = false,
                Width = "400px",
                Height = "400px",
            };
            _dialog = await DialogService.ShowSplashScreenAsync(parameters);

            var splashScreen = (SplashScreenContent)_dialog.Instance.Content;

            // Simulate a first task
            await Task.Delay(500);

            // Update the splash screen content and simulate a second task
            splashScreen.UpdateLabels(loadingText: "Second task...");
            await Task.Delay(500);

            await _dialog.CloseAsync();

            DialogResult result = await _dialog.Result;
            await HandleDefaultSplashAsync(result);
        }

        private async Task HandleDefaultSplashAsync(DialogResult result)
        {
            await Task.Run(() => Debug.WriteLine($"Default splash closed"));
        }

        private void HandleRowFocus(FluentDataGridRow<CaseModel> row)
        {
            Debug.WriteLine($"[Custom comparer] Row focused: {row.Item?.CaseNumber}");
            selectedCase = row.Item;
        }

        private async Task FetchCaseAnalysisAsync(List<CaseModel> caseModels)
        {
            caseModels.ForEach(item => {
                item.CommunicationModel = communicationModel.Find(x => x.CaseNumber == item.CaseNumber)!;
                item.NotesModel = notesModel.Find(x => x.CaseNumber == item.CaseNumber)!;
            });

            await Parallel.ForEachAsync(caseModels, async (caseModel, cancellationToken) =>
            {
                await ProcessCaseAsync(caseModel, cancellationToken);
            });
        }

        private async Task ProcessCaseAsync(CaseModel caseModel, CancellationToken cts)
        {
            caseModel.CaseAnalysisModel = await AnalysisService.AnalyzeCase(caseModel);
            await InvokeAsync(StateHasChanged);
        }

        private GridSort<CaseModel> sortByPriority = GridSort<CaseModel>
            .ByAscending(p => p.CaseAnalysisModel.Priority).ThenAscending(p => p.CaseAnalysisModel.Priority);


        private GridSort<CaseModel> sortBySentiment = GridSort<CaseModel>
            .ByAscending(p => p.CaseAnalysisModel.Priority).ThenAscending(p => p.CaseAnalysisModel.Sentiment);


        private GridSort<CaseModel> sortByAction = GridSort<CaseModel>
            .ByAscending(p => p.CaseAnalysisModel.Action).ThenAscending(p => p.CaseAnalysisModel.Action);
    }
}
