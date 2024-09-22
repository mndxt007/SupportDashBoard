using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.SemanticKernel;
using SupportEngineerEfficiencyDashboard.Components;
using SupportEngineerEfficiencyDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

var kernel = builder.Services.AddKernel();
kernel.Plugins.AddFromPromptDirectory("Helpers\\Plugins");

//for capturing requests in Fiddler
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpClient("default")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        CheckCertificateRevocationList = false
    });
}


builder.Services.AddAzureOpenAIChatCompletion(
         deploymentName: builder.Configuration["OpenAI:Deployment"]!,
         endpoint: builder.Configuration["OpenAI:Endpoint"]!,
         apiKey: builder.Configuration["OpenAI:Key"]!);

builder.Services.AddSingleton<ICaseDataService, CaseDataService>();
builder.Services.AddSingleton<CaseAnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
