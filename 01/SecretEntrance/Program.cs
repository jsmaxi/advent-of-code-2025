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

Console.WriteLine("Parsing lines and counting zeros");

int countZeros = 0;
int count_0x434C49434B = 0;
int idx = 50;

for (int i = 0; i < lines.Count; i++)
{
    var match = Regex.Match(lines[i], @"^(R|L)(\d+)$");
    if (!match.Success)
        throw new FormatException("Invalid line format.");

    char direction = match.Groups[1].Value[0];
    int value = int.Parse(match.Groups[2].Value);

    for (int j = 0; j < value; j++)
    {
        idx = direction == 'R' ? idx + 1 : idx - 1;
        if (idx == -1)
        {
            idx = 99;
        }
        else if (idx == 100)
        {
            idx = 0;
        }
        if (idx == 0)
        {
            count_0x434C49434B += 1;
        }
    }

    if (idx == 0)
    {
        countZeros++;
    }
}

Console.WriteLine($"The number of times the dial is left pointing at 0: {countZeros}");
Console.WriteLine($"Count of 0x434C49434B: {count_0x434C49434B}");

Console.WriteLine("End");
