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

if (lines.Count == 0 || lines[0].Length == 0)
{
    Console.WriteLine("No lines read from input file");
    Environment.Exit(1);
}

// Part 1

int startCol = lines[0].IndexOf('S');

if (startCol == -1)
{
    Console.WriteLine("No starting point 'S' found in the first line");
    Environment.Exit(1);
}

Console.WriteLine($"Start at index: {startCol}");

var currentPositions = new HashSet<int> { startCol };
int totalSplits = 0;

for (int row = 1; row < lines.Count; row++)
{
    string line = lines[row];
    var nextPositions = new HashSet<int>();
    var processedSplitters = new HashSet<int>();

    foreach (var col in currentPositions)
    {
        if (col < 0 || col >= line.Length) continue;

        char ch = line[col];

        if (ch == '^')
        {
            if (processedSplitters.Contains(col))
            {
                continue; // skip already processed
            }

            processedSplitters.Add(col);

            int left = col - 1;
            int right = col + 1;

            if (left >= 0) nextPositions.Add(left);

            if (right < line.Length) nextPositions.Add(right);

            totalSplits += 1;
        }
        else if (ch == '.')
        {
            nextPositions.Add(col);
        }
        else
        {
            if (ch == 'S' && row == 0)
            {
                nextPositions.Add(col);
            }
            else
            {
                Console.WriteLine($"Invalid character '{ch}' at row {row}, col {col}");
                Environment.Exit(1);
            }
        }
    }

    currentPositions = nextPositions;
    if (currentPositions.Count == 0) break;
}

Console.WriteLine($"Total splits detected: {totalSplits}");

// Part 2

long totalTimelines = CountTimelines(lines);

Console.WriteLine($"Total timelines: {totalTimelines}");

Console.WriteLine("End");

static long CountTimelines(List<string> lines)
{
    if (lines == null || lines.Count == 0) return 0;

    int startCol = lines[0].IndexOf('S');

    if (startCol == -1)
        throw new ArgumentException("No 'S' in first line.");

    if (lines.Count == 1) return 1;

    var current = new Dictionary<int, long> { [startCol] = 1 };

    for (int r = 1; r < lines.Count; r++)
    {
        string line = lines[r];
        var next = new Dictionary<int, long>();

        foreach (var kv in current)
        {
            int col = kv.Key;
            long count = kv.Value;

            if (col < 0 || col >= line.Length) continue;

            char ch = line[col];

            if (ch == '.')
            {
                if (!next.ContainsKey(col))
                    next[col] = 0;

                next[col] += count;
            }
            else if (ch == '^')
            {
                int left = col - 1;
                int right = col + 1;

                if (left >= 0)
                {
                    if (!next.ContainsKey(left))
                        next[left] = 0;

                    next[left] += count;
                }

                if (right < line.Length)
                {
                    if (!next.ContainsKey(right))
                        next[right] = 0;

                    next[right] += count;
                }
            }
            else
            {
                if (ch == 'S' && r == 0)
                {
                    if (!next.ContainsKey(col))
                        next[col] = 0;

                    next[col] += count;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unexpected character '{ch}' at row {r}, col {col}.");
                }
            }
        }

        current = next;

        if (current.Count == 0) break;
    }

    long total = 0;

    foreach (var v in current.Values)
        total += v;

    return total;
}
