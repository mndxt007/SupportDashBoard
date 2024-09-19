using Microsoft.AspNetCore.Components;
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
        private CaseModel? selectedCase;
        protected override async Task OnInitializedAsync()
        {
            caseModels = await CaseService.FetchCasesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool first)
        {
            if (first)
            {
                await OpenSplashDefaultAsync();
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
            await Task.Delay(2000);

            // Update the splash screen content and simulate a second task
            splashScreen.UpdateLabels(loadingText: "Second task...");
            await Task.Delay(2000);

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
    }
}
