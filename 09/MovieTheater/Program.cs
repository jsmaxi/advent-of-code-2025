using System.Globalization;

static bool PointInPolygonOrOnBoundary(Pt p, List<Segment> segs)
{
    foreach (var s in segs)
    {
        if (PointOnSegment(p, s)) return true;
    }

    // Standard ray-casting to +X direction counting edge crossings.
    // Be careful with vertices: use half-open rule: count an edge if it crosses ray where y in [y1,y2) with y1<=y2.
    int crossings = 0;
    foreach (var s in segs)
    {
        long x1 = s.A.X, y1 = s.A.Y, x2 = s.B.X, y2 = s.B.Y;
        // Skip horizontal edges (they either lie on ray and were caught by PointOnSegment above)
        if (y1 == y2) continue;
        long ymin = Math.Min(y1, y2), ymax = Math.Max(y1, y2);
        // Check if ray at p.Y intersects vertical span [ymin, ymax) (half-open to avoid double counting)
        if (p.Y < ymin || p.Y >= ymax) continue;
        // Find x coordinate of edge at y = p.Y
        // Since segment is vertical: x = x1 (for axis-aligned polygon)
        long xAtY;
        if (x1 == x2) xAtY = x1;
        else
        {
            // Shouldn't happen in axis-aligned polygon, but handle just in case:
            // compute intersection x using linear interpolation
            double t = (double)(p.Y - y1) / (double)(y2 - y1);
            double xDouble = x1 + t * (x2 - x1);
            xAtY = (long)Math.Floor(xDouble);
        }
        if (xAtY > p.X) crossings++;
        else if (xAtY == p.X) // Intersection exactly at point -> boundary handled earlier
        {
            return true;
        }
    }
    return (crossings % 2) == 1;
}

static bool PointOnSegment(Pt p, Segment s)
{
    // Check collinearity and range for axis-aligned segments
    if (s.IsVertical)
    {
        if (p.X != s.A.X) return false;
        long ymin = Math.Min(s.A.Y, s.B.Y), ymax = Math.Max(s.A.Y, s.B.Y);
        return p.Y >= ymin && p.Y <= ymax;
    }
    else if (s.IsHorizontal)
    {
        if (p.Y != s.A.Y) return false;
        long xmin = Math.Min(s.A.X, s.B.X), xmax = Math.Max(s.A.X, s.B.X);
        return p.X >= xmin && p.X <= xmax;
    }
    else
    {
        // Shouldn't happen for this problem
        return false;
    }
}

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

// Part 1

var points = new List<(int x, int y)>();

foreach (var raw in lines)
{
    var line = raw?.Trim();

    if (string.IsNullOrEmpty(line)) continue;

    if (line.StartsWith("#")) continue;

    var parts = line.Split([','], StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != 2) continue;

    if (int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var x)
     && int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
    {
        points.Add((x, y));
    }
}

if (points.Count < 2)
{
    Console.WriteLine($"Part 1: {0}");
    return;
}

long maxArea = 0;
int n = points.Count;

for (int i = 0; i < n; ++i)
{
    var (x1, y1) = points[i];

    for (int j = i + 1; j < n; ++j)
    {
        var (x2, y2) = points[j];

        long width = Math.Abs((long)x1 - x2) + 1;
        long height = Math.Abs((long)y1 - y2) + 1;
        long area = width * height;

        if (area > maxArea) maxArea = area;
    }
}

Console.WriteLine($"Part 1: {maxArea}");

// Part 2

var reds = new List<Pt>();

foreach (var raw in lines)
{
    var line = raw?.Trim();

    if (string.IsNullOrEmpty(line)) continue;

    if (line.StartsWith("#")) continue;

    var parts = line.Split([','], StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != 2) continue;

    if (long.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var x)
     && long.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
    {
        reds.Add(new Pt(x, y));
    }
}

if (reds.Count < 2)
{
    Console.WriteLine("Part 2: 0");
    return;
}

// Build polygon segments (axis-aligned between consecutive reds, wrapping)
var segments = new List<Segment>();
for (int i = 0; i < reds.Count; i++)
{
    var a = reds[i];
    var b = reds[(i + 1) % reds.Count];

    if (a.X != b.X && a.Y != b.Y)
    {
        Console.Error.WriteLine("Invalid input: consecutive red tiles must be axis aligned.");
        return;
    }

    // Store segment with endpoints sorted (for easier overlap checks)
    if (a.X == b.X)
    {
        if (a.Y <= b.Y) segments.Add(new Segment(a, b));
        else segments.Add(new Segment(b, a));
    }
    else
    {
        if (a.X <= b.X) segments.Add(new Segment(a, b));
        else segments.Add(new Segment(b, a));
    }
}

maxArea = 0;
n = reds.Count;

// Precompute bounding box for a cheap rejection: rectangle outside global bbox cannot be inside polygon
long globalMinX = reds.Min(p => p.X), globalMaxX = reds.Max(p => p.X);
long globalMinY = reds.Min(p => p.Y), globalMaxY = reds.Max(p => p.Y);

for (int i = 0; i < n; i++)
{
    for (int j = i + 1; j < n; j++)
    {
        long left = Math.Min(reds[i].X, reds[j].X);
        long right = Math.Max(reds[i].X, reds[j].X);
        long top = Math.Min(reds[i].Y, reds[j].Y);
        long bottom = Math.Max(reds[i].Y, reds[j].Y);

        // Quick outside-of-global-bbox check (if rectangle completely outside polygon bbox skip)
        if (right < globalMinX || left > globalMaxX || bottom < globalMinY || top > globalMaxY)
            continue;

        long width = right - left + 1;
        long height = bottom - top + 1;
        long area = width * height;

        // Test: at least one corner inside polygon (or on boundary)
        bool cornerInside = PointInPolygonOrOnBoundary(new Pt(left, top), segments)
                         || PointInPolygonOrOnBoundary(new Pt(left, bottom), segments)
                         || PointInPolygonOrOnBoundary(new Pt(right, top), segments)
                         || PointInPolygonOrOnBoundary(new Pt(right, bottom), segments);
        if (!cornerInside) continue;

        // Test: no polygon segment crosses the rectangle interior.
        // A polygon segment crosses the rectangle interior iff:
        //  - for a vertical segment at x=sx: sx in (left, right) strict, and its y-range overlaps (top, bottom) with positive measure
        //  - for a horizontal segment at y=sy: sy in (top, bottom) strict, and its x-range overlaps (left, right) with positive measure
        bool bad = false;
        foreach (var s in segments)
        {
            if (s.IsVertical)
            {
                long sx = s.A.X;

                if (sx > left && sx < right)
                {
                    long segTop = Math.Min(s.A.Y, s.B.Y);
                    long segBottom = Math.Max(s.A.Y, s.B.Y);

                    if (Math.Max(segTop, top) < Math.Min(segBottom, bottom)) { bad = true; break; }
                }
            }
            else // Horizontal
            {
                long sy = s.A.Y;

                if (sy > top && sy < bottom)
                {
                    long segLeft = Math.Min(s.A.X, s.B.X);
                    long segRight = Math.Max(s.A.X, s.B.X);

                    if (Math.Max(segLeft, left) < Math.Min(segRight, right)) { bad = true; break; }
                }
            }
        }

        if (bad) continue;

        if (area > maxArea) maxArea = area;
    }
}

Console.WriteLine($"Part 2: {maxArea}");

Console.WriteLine("End");

struct Pt { public long X, Y; public Pt(long x, long y) { X = x; Y = y; } }
struct Segment { public Pt A, B; public bool IsVertical => A.X == B.X; public bool IsHorizontal => A.Y == B.Y; public Segment(Pt a, Pt b) { A = a; B = b; } }
