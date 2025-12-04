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

Console.WriteLine("Counting rolls of paper");

if (lines.Count == 0)
{
    Console.WriteLine("No lines found in input file.");
    Environment.Exit(1);
}

int w = lines[0].Length;
int h = lines.Count;

if (w != h)
{
    Console.WriteLine("Input is not a square grid.");
    Environment.Exit(1);
}

// Console.WriteLine($"Width: {w}, Height: {h}"); // debug

var totalRolls = 0;

var adjacent = new List<(int, int)>
{
    (-1, -1), (-1, 0), (-1, 1),
    (0, -1),          (0, 1),
    (1, -1),  (1, 0),  (1, 1)
};

// Part 1

for (int i = 0; i < lines.Count; i++)
{
    var line = lines[i];

    for (int j = 0; j < line.Length; j++)
    {
        var roll = line[j];

        if (roll != '@') continue;

        var countAdjacent = 0;

        for (int k = 0; k < adjacent.Count; k++)
        {
            var (dx, dy) = adjacent[k];
            int nx = j + dx;
            int ny = i + dy;

            if (nx >= 0 && nx < w && ny >= 0 && ny < h)
            {
                if (lines[ny][nx] == '@')
                {
                    countAdjacent++;
                }
            }
        }

        if (countAdjacent < 4)
        {
            totalRolls++;
        }
    }
}

// Part 2

int totalRemoved = 0;

while (true)
{
    var toRemove = new List<(int, int)>();

    for (int i = 0; i < lines.Count; i++)
    {
        var line = lines[i];

        for (int j = 0; j < line.Length; j++)
        {
            var roll = line[j];

            if (roll != '@') continue;

            var countAdjacent = 0;

            for (int k = 0; k < adjacent.Count; k++)
            {
                var (dx, dy) = adjacent[k];
                int nx = j + dx;
                int ny = i + dy;

                if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                {
                    if (lines[ny][nx] == '@')
                    {
                        countAdjacent++;
                    }
                }
            }

            if (countAdjacent < 4)
            {
                toRemove.Add((j, i));
            }
        }
    }

    if (toRemove.Count == 0)
    {
        break;
    }
    else
    {
        toRemove.ForEach(pos =>
        {
            var (x, y) = pos; // (col, row)
            var chars = lines[y].ToCharArray();
            chars[x] = '.';
            lines[y] = new string(chars);
        });

        totalRemoved += toRemove.Count;
    }
}

Console.WriteLine($"Total rolls of paper that can be accessed in part 1: {totalRolls}");
Console.WriteLine($"Total rolls of paper removed in part 2: {totalRemoved}");

Console.WriteLine("End");
