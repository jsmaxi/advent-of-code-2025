using System.Text.RegularExpressions;

Console.WriteLine("Start");

Console.WriteLine("Reading input file");

List<string> lines = [];
try
{
    using StreamReader sr = new("./input.txt");
    string? line;
    while ((line = sr.ReadLine()) != null)
    {
        // Console.WriteLine(line); // debug
        lines.Add(line);
    }
}
catch (Exception e)
{
    Console.WriteLine("Error while reading input file: " + e.Message);
    Environment.Exit(1);
}

Console.WriteLine("Parsing lines");

int emptyIndex = lines.FindIndex(string.IsNullOrWhiteSpace);

if (emptyIndex == -1)
{
    throw new FormatException("Input file does not contain an empty line.");
}

Console.WriteLine($"Empty line found at index: {emptyIndex}"); // debug

List<string> fresh = [.. lines.Take(emptyIndex)];
List<string> available = [.. lines.Skip(emptyIndex + 1)];

Console.WriteLine($"Fresh ingredient IDs list has {fresh.Count} lines.");
Console.WriteLine($"Available ingredient IDs list has {available.Count} lines.");

List<(long, long)> freshRanges = [];

foreach (var f in fresh)
{
    var parts = f.Split('-');
    var start = long.Parse(parts[0]);
    var end = long.Parse(parts[1]);

    if (start > end)
        throw new FormatException("Invalid range! Start is greater than end.");

    freshRanges.Add((start, end));
}

// Console.WriteLine($"Fresh ranges count: {freshRanges.Count}"); // debug

// Part 1

var freshCount = available.Count(a =>
{
    var availableId = long.Parse(a);
    return freshRanges.Any(r => availableId >= r.Item1 && availableId <= r.Item2);
});

// Part 2

var ordered = freshRanges.OrderBy(r => r.Item1);

List<(long, long)> merged = [];

foreach (var range in ordered)
{
    if (merged.Count == 0)
    {
        merged.Add(range); // initial
        continue;
    }

    var last = merged[merged.Count - 1];

    if (range.Item1 <= last.Item2 + 1) // overlapping
    {
        // merge with last range
        merged[merged.Count - 1] = (last.Item1, Math.Max(last.Item2, range.Item2));
    }
    else
    {
        merged.Add(range);
    }
}

var countMerged = merged.Sum(r => r.Item2 - r.Item1 + 1);

Console.WriteLine($"Number of available fresh ingredient IDs (part 1): {freshCount}");
Console.WriteLine($"Total number of fresh ingredient IDs (part2): {countMerged}");

Console.WriteLine("End");
