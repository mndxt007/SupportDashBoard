using Microsoft.FluentUI.AspNetCore.Components;
using SupportEngineerEfficiencyDashboard.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

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

        Func<CaseModel, string?> rowStyle = x => x.CaseAnalysisModel.Priority switch
        {
            10 => "background-color: #C62828; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Rich Red
            9 => "background-color: #EF5350; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Soft Red
            8 => "background-color: #FF8A65; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Warm Orange
            7 => "background-color: #FFAB91; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Soft Coral
            6 => "background-color: #FFCC80; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Light Orange
            5 => "background-color: #FFF59D; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Soft Yellow
            4 => "background-color: #DCE775; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Soft Yellow-Green
            3 => "background-color: #E6EE9C; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Soft Lime Green
            2 => "background-color: #C5E1A5; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Light Green
            1 => "background-color: #A5D6A7; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr", // Soft Green
            _ => "background-color: white; grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr" // Default for rankings 51 and above
        };


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
                    DisplayTime = 4000,    // See Task.Delay below
                    Title = "Support Engineer DashBoard",
                    SubTitle = "Prioritized view of your Case Bin.",
                    LoadingText = "Processing........",
                    Message = (Microsoft.AspNetCore.Components.MarkupString?)"Please wait while we analyse the cases.",

                },
                PreventDismissOnOverlayClick = true,
                Modal = true,
            };
            _dialog = await DialogService.ShowSplashScreenAsync(parameters);

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
