Console.WriteLine("Start");

Console.WriteLine("Reading input file");

string text = File.ReadAllText("./input.txt");

if (text.Length == 0)
{
    Console.WriteLine("Input file is empty");
    Environment.Exit(1);
}

// Console.WriteLine(text); // debug

Console.WriteLine("Parsing text into intervals");

string[] intervals = text.Split(",", StringSplitOptions.RemoveEmptyEntries);

// Console.WriteLine(string.Join("\n", intervals)); // debug

Console.WriteLine("Parsing intervals");

List<(long, long)> parsedIntervals = [];

foreach (string interval in intervals)
{
    string[] bounds = interval.Split("-", StringSplitOptions.TrimEntries);

    if (bounds.Length != 2)
    {
        Console.WriteLine($"Invalid interval: {interval}");
        Environment.Exit(1);
    }

    if (!long.TryParse(bounds[0], out var start))
    {
        throw new Exception("Invalid interval start");
    }

    if (!long.TryParse(bounds[1], out var end))
    {
        throw new Exception("Invalid interval end");
    }

    parsedIntervals.Add((start, end));
}

// Console.WriteLine(
//     string.Join(", ", parsedIntervals.Select(i => $"({i.Item1}, {i.Item2})"))
// ); // debug

long sum1 = 0;
long sum2 = 0;

Console.WriteLine("Detecting invalid IDs");

foreach (var (start, end) in parsedIntervals)
{
    for (long i = start; i <= end; i++)
    {
        string id = i.ToString();

        if (id.Length < 2)
        {
            continue;
        }

        for (var j = 1; j <= id.Length / 2; j++)
        {
            if (id.Length % j != 0) continue;

            var pattern = id[..j];
            var repeats = id.Length / j;

            var str = string.Concat(Enumerable.Repeat(pattern, repeats));

            if (str == id)
            {
                _ = long.TryParse(id, out var invalidId);
                sum2 += invalidId;
                // Console.WriteLine($"Invalid ID detected: {invalidId}");
                break;
            }
        }

        if (id.Length % 2 != 0)
        {
            continue;
        }

        var left = id[..(id.Length / 2)];
        var right = id[(id.Length / 2)..];

        if (left == right)
        {
            _ = long.TryParse(id, out var invalidId);
            sum1 += invalidId;
            // Console.WriteLine($"Invalid ID detected: {invalidId}");
        }
    }
}

Console.WriteLine($"Sum1: {sum1}");
Console.WriteLine($"Sum2: {sum2}");

Console.WriteLine("End");
