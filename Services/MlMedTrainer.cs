using Microsoft.ML;

namespace Services;

public class MlMedTrainer
{
    // Simple duplicate detection using token overlap: placeholder for a real ML.NET model
    public string CheckDuplicates(List<string> meds)
    {
        var lower = meds.Select(m => m.ToLowerInvariant()).ToList();
        var dups = lower.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        return dups.Any() ? string.Join(", ", dups) : "No duplicates detected";
    }
}
