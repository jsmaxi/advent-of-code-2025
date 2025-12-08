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

if (lines.Count == 0)
{
    Console.WriteLine("Input file is empty.");
    Environment.Exit(1);
}

// Kruskal's algorithm

int K = 1000;
var points = new List<(long x, long y, long z)>();

foreach (var line in lines)
{
    var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
    if (parts.Length != 3) continue;
    long x = long.Parse(parts[0]);
    long y = long.Parse(parts[1]);
    long z = long.Parse(parts[2]);
    points.Add((x, y, z));
}

int n = points.Count;

if (n == 0)
{
    Console.WriteLine("No points read.");
    return;
}

// edges with squared distances
var edges = new List<(long dist, int a, int b)>
{
    Capacity = n * (n - 1) / 2
};

for (int i = 0; i < n; i++)
{
    for (int j = i + 1; j < n; j++)
    {
        long dx = points[i].x - points[j].x;
        long dy = points[i].y - points[j].y;
        long dz = points[i].z - points[j].z;
        long dist2 = dx * dx + dy * dy + dz * dz;
        edges.Add((dist2, i, j));
    }
}

edges.Sort((e1, e2) =>
{
    int cmp = e1.dist.CompareTo(e2.dist);
    if (cmp != 0) return cmp;
    cmp = e1.a.CompareTo(e2.a);
    if (cmp != 0) return cmp;
    return e1.b.CompareTo(e2.b);
});

// Part 1

var uf = new UnionFind(n);

int take = Math.Min(K, edges.Count);

for (int idx = 0; idx < take; idx++)
{
    var e = edges[idx];
    uf.Union(e.a, e.b);
}

var rootToSize = new Dictionary<int, int>();

for (int i = 0; i < n; i++)
{
    int r = uf.Find(i);
    if (!rootToSize.ContainsKey(r)) rootToSize[r] = 0;
    rootToSize[r]++;
}

var sizes = rootToSize.Values.OrderByDescending(s => s).ToArray();

// Console.WriteLine($"Processed first {take} shortest edges (K = {K}). Found {sizes.Length} circuits."); // debug
// for (int i = 0; i < sizes.Length; i++)
// {
//     Console.WriteLine($" Circuit {i + 1}: size {sizes[i]}"); // debug
// }

long product = 1;

for (int i = 0; i < 3; i++) product *= sizes[i]; // multiply top3

Console.WriteLine($"Product of sizes of 3 largest circuits: {product}");

// Part 2

var uf2 = new UnionFind(n);
int components = n;
int lastA = -1, lastB = -1;
long lastDist2 = -1;

foreach (var e in edges)
{
    bool merged = uf2.Union(e.a, e.b);

    if (merged)
    {
        components--;
        lastA = e.a;
        lastB = e.b;
        lastDist2 = e.dist;
    }

    if (components == 1)
    {
        break;
    }
}

if (components != 1)
{
    Console.WriteLine("Error: processed all edges but graph is still not fully connected.");
}
else
{
    var pA = points[lastA];
    var pB = points[lastB];

    // Console.WriteLine($"Index {lastA}: ({pA.x},{pA.y},{pA.z})"); // debug
    // Console.WriteLine($"Index {lastB}: ({pB.x},{pB.y},{pB.z})"); // debug
    // Console.WriteLine($"Squared distance between them: {lastDist2}"); // debug

    long xProduct = pA.x * pB.x;

    Console.WriteLine($"Product of X coordinates: {pA.x} * {pB.x} = {xProduct}");
}

Console.WriteLine("End");

class UnionFind
{
    private readonly int[] parent;
    private readonly int[] size;

    public UnionFind(int n)
    {
        parent = new int[n];
        size = new int[n];
        for (int i = 0; i < n; i++)
        {
            parent[i] = i;
            size[i] = 1;
        }
    }

    public int Find(int x)
    {
        while (parent[x] != x)
        {
            parent[x] = parent[parent[x]];
            x = parent[x];
        }
        return x;
    }

    public bool Union(int a, int b)
    {
        int ra = Find(a), rb = Find(b);
        if (ra == rb) return false;
        if (size[ra] < size[rb]) { var t = ra; ra = rb; rb = t; }
        parent[rb] = ra;
        size[ra] += size[rb];
        return true;
    }

    public int ComponentSize(int x) => size[Find(x)];
}
