using Microsoft.AspNetCore.Mvc;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<OpenAiClient>();
builder.Services.AddSingleton<RxNavClient>();
builder.Services.AddSingleton<MlMedTrainer>();

var app = builder.Build();

app.MapGet("/", () => "DotNet MedRec Decision Agent - running");

app.MapPost("/reconcile", async ([FromBody] MedListRequest req, RxNavClient rxapi, OpenAiClient openai, MlMedTrainer trainer) =>
{
    // Run duplicate detection model
    var dupReport = trainer.CheckDuplicates(req.Medications);
    // Check interactions via RxNav
    var interactions = await rxapi.CheckInteractionsAsync(req.Medications);
    // Compose prompt for explanation
    var prompt = $"Med reconciliation for patient:\nMed list:\n{string.Join("\n", req.Medications)}\n\nDuplicates:\n{dupReport}\n\nInteractions:\n{string.Join("\n", interactions)}";
    var explanation = await openai.ChatAsync(prompt);
    return Results.Ok(new { duplicates = dupReport, interactions, explanation });
});

app.Run();

record MedListRequest(List<string> Medications);
