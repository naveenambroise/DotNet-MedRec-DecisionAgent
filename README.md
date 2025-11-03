# DotNet-MedRec-DecisionAgent

Medication reconciliation decision agent using .NET 8, ML.NET, the RxNav (NLM) drug interaction API and OpenAI for explanation generation.

Prerequisites
- .NET 8 SDK
- `OPENAI_API_KEY` env var for LLM explainers (not required to run interaction checks)

Run (PowerShell):

```powershell
dotnet restore
dotnet run --project DotNet.MedRec.DecisionAgent.csproj
# POST to http://localhost:5000/reconcile with JSON {"medications": ["aspirin", "warfarin"]}
```

Notes
- This project uses RxNav REST APIs (no key required) to perform real drug interaction checks.
