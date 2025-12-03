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

Console.WriteLine("Calculating batteries joltage");

// part 1

long sum1 = 0;

for (var i = 0; i < lines.Count; i++)
{
    var bank = lines[i];

    if (bank.Length < 2)
    {
        Console.WriteLine("Invalid bank (line) length");
        Environment.Exit(1);
    }

    char battery1 = bank[0];
    char battery2 = bank[1];

    for (var j = 1; j < bank.Length - 1; j++)
    {
        // chars are stored in ascending numeric order
        if (bank[j] > battery1)
        {
            battery1 = bank[j];
            battery2 = bank[j + 1];
        }
        if (bank[j + 1] > battery2)
        {
            battery2 = bank[j + 1];
        }
    }

    if (!long.TryParse(string.Concat(battery1, battery2), out var joltage))
    {
        Console.WriteLine("Unable to parse joltage");
        Environment.Exit(1);
    }

    // Console.WriteLine($"Joltage: {joltage}"); // debug

    sum1 += joltage;
}

// part 2

long sum2 = 0;

for (var i = 0; i < lines.Count; i++)
{
    var bank = lines[i];
    var len = bank.Length;
    const int k = 12;

    if (len < k)
    {
        Console.WriteLine("Invalid bank (line) length");
        Environment.Exit(1);
    }

    var result = new char[k];
    int start = 0;

    for (int pos = 0; pos < k; pos++)
    {
        int end = len - (k - pos);
        char best = '0';
        int bestIdx = -1;

        for (int j = start; j <= end; j++)
        {
            char c = bank[j];
            if (c > best)
            {
                best = c;
                bestIdx = j;
                if (best == '9') break;
            }
        }

        result[pos] = best;
        start = bestIdx + 1;
    }

    if (!long.TryParse(result, out var joltage))
    {
        Console.WriteLine("Unable to parse joltage");
        Environment.Exit(1);
    }

    // Console.WriteLine($"Joltage: {joltage}"); // debug

    sum2 += joltage;
}

Console.WriteLine($"Total output joltage part1: {sum1}");
Console.WriteLine($"Total output joltage part2: {sum2}");

Console.WriteLine("End");
