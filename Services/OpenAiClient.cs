using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Services;

public class OpenAiClient
{
    private readonly HttpClient _http = new();
    private readonly string _key;

    public OpenAiClient(IConfiguration config)
    {
        _key = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? config["OPENAI_API_KEY"] ?? string.Empty;
        if (string.IsNullOrEmpty(_key)) Console.WriteLine("OPENAI_API_KEY not set; chat calls will fail");
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _key);
    }

    public async Task<string> ChatAsync(string prompt)
    {
        var payload = new { model = "gpt-4o-mini", messages = new[] { new { role = "user", content = prompt } } };
        var resp = await _http.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json"));
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadAsStringAsync();
        dynamic obj = JsonConvert.DeserializeObject(body)!;
        return (string)obj.choices[0].message.content;
    }
}
