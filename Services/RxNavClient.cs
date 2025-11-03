using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Services;

public class RxNavClient
{
    private readonly HttpClient _http = new();

    public RxNavClient()
    {
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<string>> CheckInteractionsAsync(List<string> meds)
    {
        // Use RxNav interaction API: https://rxnav.nlm.nih.gov/RxNormAPIs.html
        // This is a simplified flow: get RxCUI for each drug name then call interaction/list
        var rxcuis = new List<string>();
        foreach (var med in meds)
        {
            var url = $"https://rxnav.nlm.nih.gov/REST/rxcui.json?name={Uri.EscapeDataString(med)}";
            var resp = await _http.GetStringAsync(url);
            dynamic obj = JsonConvert.DeserializeObject(resp)!;
            var rxcui = (string?)obj.idGroup.rxnormId?[0];
            if (!string.IsNullOrEmpty(rxcui)) rxcuis.Add(rxcui);
        }
        if (!rxcuis.Any()) return new List<string>{"No RxCUI found for provided names"};
        var rxcuiParam = string.Join("+", rxcuis);
        var interUrl = $"https://rxnav.nlm.nih.gov/REST/interaction/list.json?rxcuis={rxcuiParam}";
        var interResp = await _http.GetStringAsync(interUrl);
        dynamic interObj = JsonConvert.DeserializeObject(interResp)!;
        var matches = new List<string>();
        try
        {
            var fullInteractionTypeGroup = interObj.fullInteractionTypeGroup;
            foreach (var g in fullInteractionTypeGroup)
            {
                foreach (var it in g.fullInteractionType)
                {
                    var description = (string)it.interactionPair[0].description;
                    matches.Add(description);
                }
            }
        }
        catch {
            // ignore parsing errors
        }
        return matches.Any() ? matches : new List<string>{"No interactions found"};
    }
}
