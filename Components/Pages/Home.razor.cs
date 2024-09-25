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
        static readonly string rowstyle = "grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;height: 50px;align-items: center;";

        Func<CaseModel, string?> rowStyle = x => x.CaseAnalysisModel.Priority switch
        {
            10 => "background-color: #C62828;" + rowstyle, // Rich Red
            9 => "background-color: #EF5350; " + rowstyle, // Soft Red
            8 => "background-color: #FF8A65; " + rowstyle, // Warm Orange
            7 => "background-color: #FFAB91; " + rowstyle, // Soft Coral
            6 => "background-color: #FFCC80; " + rowstyle, // Light Orange
            5 => "background-color: #FFF59D; " + rowstyle, // Soft Yellow
            4 => "background-color: #DCE775; " + rowstyle, // Soft Yellow-Green
            3 => "background-color: #E6EE9C; " + rowstyle, // Soft Lime Green
            2 => "background-color: #C5E1A5; " + rowstyle, // Light Green
            1 => "background-color: #A5D6A7; " + rowstyle, // Soft Green
            _ => "background-color: white; " + rowstyle // Default for rankings 51 and above
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
                switch (caseModel.CaseNumber)
                {
                    case "6666666666666666":
                        caseModel.CaseAnalysisModel.Priority = 5;
                        caseModel.CaseAnalysisModel.Sentiment = "Neutral";
                        caseModel.CaseAnalysisModel.Action = "Follow up";
                        caseModel.CaseAnalysisModel.Health = 1;
                        break;
                    case "3333333333333333":
                        caseModel.CaseAnalysisModel.Priority = 3;
                        caseModel.CaseAnalysisModel.Sentiment = "Positive";
                        caseModel.CaseAnalysisModel.Action = "Closure";
                        caseModel.CaseAnalysisModel.Health = 3;
                        break;
                    case "5555555555555555":
                        caseModel.CaseAnalysisModel.Priority = 9;
                        caseModel.CaseAnalysisModel.Sentiment = "Frustrated";
                        caseModel.CaseAnalysisModel.Action = "Engage Esc/TA";
                        caseModel.CaseAnalysisModel.Health = 1;
                        break;
                    case "4444444444444444":
                        caseModel.CaseAnalysisModel.Priority = 6;
                        caseModel.CaseAnalysisModel.Sentiment = "Neutral";
                        caseModel.CaseAnalysisModel.Action = "Scoping";
                        caseModel.CaseAnalysisModel.Health = 2;
                        break;
                    case "2222222222222222":
                        caseModel.CaseAnalysisModel.Priority = 8;
                        caseModel.CaseAnalysisModel.Sentiment = "Negative";
                        caseModel.CaseAnalysisModel.Action = "Engage Esc/TA";
                        caseModel.CaseAnalysisModel.Health = 1;
                        break;
                }         
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
