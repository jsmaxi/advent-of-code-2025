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

if (lines.Count < 2)
{
    Console.WriteLine("Input file must have at least two lines.");
    return;
}

Console.WriteLine("Parsing lines and counting totals");

int totalRows = lines.Count - 1;
var rows = new List<long[]>();
string[] operations = lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
int totalCols = operations.Length;

// Part 1

for (int r = 0; r < totalRows; r++)
{
    var parts = lines[r].Split(' ', StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != totalCols)
    {
        Console.WriteLine($"Row {r} has {parts.Length} columns, expected {totalCols}.");
        return;
    }

    rows.Add(parts.Select(p => long.Parse(p)).ToArray());
}

long total = 0;

for (int col = 0; col < totalCols; col++)
{
    string op = operations[col];
    var colValues = new List<long>();

    for (int r = 0; r < totalRows; r++)
        colValues.Add(rows[r][col]);

    long result;

    if (op == "+")
    {
        result = 0;
        foreach (var v in colValues) result += v;
    }
    else if (op == "*")
    {
        result = 1;
        foreach (var v in colValues) result = checked(result * v);
    }
    else
    {
        Console.WriteLine($"Not supported operation: {op}");
        return;
    }

    total = checked(total + result);
}

// Part 2

int colCount = lines[0].Length;
char[,] grid = new char[totalRows, colCount];

for (int r = 0; r < totalRows; r++)
{
    for (int c = 0; c < colCount; c++)
    {
        grid[r, c] = lines[r][c];
    }
}

var numberCols = new List<(int, int)>();
var inNumber = false;
var startCol = 0;

for (var c = 0; c < colCount; c++)
{
    var isSpaceColumn = true;

    for (var r = 0; r < totalRows; r++)
    {
        if (grid[r, c] != ' ')
        {
            isSpaceColumn = false;
            break;
        }
    }

    if (!inNumber && !isSpaceColumn)
    {
        inNumber = true;
        startCol = c;
    }
    else if (inNumber && isSpaceColumn)
    {
        inNumber = false;
        numberCols.Add((startCol, c - 1));
    }
}

if (inNumber)
{
    numberCols.Add((startCol, colCount - 1));
}

long grandTotal = 0;

numberCols.Reverse();

foreach (var (start, end) in numberCols)
{
    var numbers = new List<long>();

    for (var c = start; c <= end; c++)
    {
        string numStr = "";

        for (var r = 0; r < totalRows; r++)
        {
            if (Char.IsDigit(grid[r, c]))
                numStr += grid[r, c];
        }

        if (!string.IsNullOrEmpty(numStr))
            numbers.Add(long.Parse(numStr));
    }

    char op = ' ';

    for (var c = start; c <= end; c++)
    {
        char candidate = lines[^1][c];

        if (candidate == '+' || candidate == '*')
        {
            op = candidate;
            break;
        }
    }

    long result = (op == '+') ? numbers.Sum() : numbers.Aggregate(1L, (a, b) => a * b);

    grandTotal += result;
}

Console.WriteLine($"Total for part 1: {total}");
Console.WriteLine($"Total for part 2: {grandTotal}");

Console.WriteLine("End");
