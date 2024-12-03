using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day12;

public class Day12Problems : Problems
{
  protected override string TestInput => @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi";

  protected override int Day => 12;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    GridPoint start = new(-1, -1);
    GridPoint end = new(-1, -1);
    char[][] grid = new char[input.Length][];
    var y = 0;

    foreach (var line in input)
    {
      grid[y] = new char[line.Length];

      var x = 0;
      foreach (var c in line)
      {
        switch (c)
        {
          case 'S':
            start = new GridPoint(x, y);
            grid[y][x] = 'a';
            break;
          case 'E':
            end = new GridPoint(x, y);
            grid[y][x] = 'z';
            break;
          default:
            grid[y][x] = c;
            break;
        }

        x++;
      }

      y++;
    }

    //now convert grid into graph
    var graph = BuildGraph(grid);

    var result = DoDijkstra(start, end, graph);

    return result.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    GridPoint start = new(-1, -1);
    GridPoint end = new(-1, -1);
    char[][] grid = new char[input.Length][];
    var y = 0;

    foreach (var line in input)
    {
      grid[y] = new char[line.Length];

      var x = 0;
      foreach (var c in line)
      {
        switch (c)
        {
          case 'S':
            start = new GridPoint(x, y);
            grid[y][x] = 'a';
            break;
          case 'E':
            end = new GridPoint(x, y);
            grid[y][x] = 'z';
            break;
          default:
            grid[y][x] = c;
            break;
        }

        x++;
      }

      y++;
    }

    //now convert grid into graph
    var graph = BuildReverseGraph(grid);

    var result = DoDijkstraToElevation(end, 'a', graph);

    return result.ToString();
  }

  private int DoDijkstra(GridPoint start, GridPoint end, Dictionary<GridPoint, PointInfo> graph)
  {
    var visitedPoints = new HashSet<GridPoint>();
    graph[start].BestDistance = 0;
    var currentPoint = start;

    while (true)
    {
      var currentDistance = graph[currentPoint].BestDistance;

      //check if we've made it to the end
      if (currentPoint == end)
      {
        return currentDistance;
      }

      //update known distances as necessary
      foreach (var neighbor in graph[currentPoint].ReachableNeighbors.Where(n => !visitedPoints.Contains(n)))
      {
        if (graph[neighbor].BestDistance > currentDistance + 1)
        {
          graph[neighbor].BestDistance = currentDistance + 1;
        }
      }

      //mark current point visited
      visitedPoints.Add(currentPoint);

      //grab next point (shortest existing unvisited path)
      var nextPoint = (GridPoint?) null;
      var minDist = int.MaxValue;
      foreach (var pointInfo in graph)
      {
        if (!visitedPoints.Contains(pointInfo.Key) && pointInfo.Value.BestDistance < minDist)
        {
          nextPoint = pointInfo.Key;
          minDist = pointInfo.Value.BestDistance;
        }
      }

      currentPoint = nextPoint.Value;
    }
  }

  private int DoDijkstraToElevation(GridPoint start, char endElevation, Dictionary<GridPoint, PointInfo> graph)
  {
    var visitedPoints = new HashSet<GridPoint>();
    graph[start].BestDistance = 0;
    var currentPoint = start;

    while (true)
    {
      var currentDistance = graph[currentPoint].BestDistance;

      //check if we've made it to the destination elevation
      if (graph[currentPoint].Height == endElevation)
      {
        return currentDistance;
      }

      //update known distances as necessary
      foreach (var neighbor in graph[currentPoint].ReachableNeighbors.Where(n => !visitedPoints.Contains(n)))
      {
        if (graph[neighbor].BestDistance > currentDistance + 1)
        {
          graph[neighbor].BestDistance = currentDistance + 1;
        }
      }

      //mark current point visited
      visitedPoints.Add(currentPoint);

      //grab next point (shortest existing unvisited path)
      var nextPoint = (GridPoint?)null;
      var minDist = int.MaxValue;
      foreach (var pointInfo in graph)
      {
        if (!visitedPoints.Contains(pointInfo.Key) && pointInfo.Value.BestDistance < minDist)
        {
          nextPoint = pointInfo.Key;
          minDist = pointInfo.Value.BestDistance;
        }
      }

      currentPoint = nextPoint.Value;
    }
  }

  private static Dictionary<GridPoint, PointInfo> BuildGraph(char[][] grid)
  {
    var y = 0;
    var result = new Dictionary<GridPoint, PointInfo>();

    foreach (var row in grid)
    {
      var x = 0;
      foreach (var c in row)
      {
        var point = new GridPoint(x, y);
        var reachableNeighbors = GetReachableNeighbors(point, grid);

        result[point] = new PointInfo(reachableNeighbors, c);
        x++;
      }

      y++;
    }

    return result;
  }
  private static Dictionary<GridPoint, PointInfo> BuildReverseGraph(char[][] grid)
  {
    var y = 0;
    var result = new Dictionary<GridPoint, PointInfo>();

    foreach (var row in grid)
    {
      var x = 0;
      foreach (var c in row)
      {
        var point = new GridPoint(x, y);
        var reachableNeighbors = GetReverseReachableNeighbors(point, grid);

        result[point] = new PointInfo(reachableNeighbors, c);
        x++;
      }

      y++;
    }

    return result;
  }

  private static List<GridPoint> GetReverseReachableNeighbors(GridPoint p, char[][] grid)
  {
    var results = new List<GridPoint>();
    var curH = grid[p.Y][p.X];

    //search up
    if (p.Y > 0)
    {
      var testPoint = new GridPoint(p.X, p.Y - 1);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c >= (int)curH - 1)
        results.Add(testPoint);
    }

    //search down
    if (p.Y < grid.Length - 1)
    {
      var testPoint = new GridPoint(p.X, p.Y + 1);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c >= (int)curH - 1)
        results.Add(testPoint);
    }

    //search left
    if (p.X > 0)
    {
      var testPoint = new GridPoint(p.X - 1, p.Y);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c >= (int)curH - 1)
        results.Add(testPoint);
    }

    //search right
    if (p.X < grid[0].Length - 1)
    {
      var testPoint = new GridPoint(p.X + 1, p.Y);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c >= (int)curH - 1)
        results.Add(testPoint);
    }

    return results;
  }

  private static List<GridPoint> GetReachableNeighbors(GridPoint p, char[][] grid)
  {
    var results = new List<GridPoint>();
    var curH = grid[p.Y][p.X];

    //search up
    if (p.Y > 0)
    {
      var testPoint = new GridPoint(p.X, p.Y - 1);
      var c = grid[testPoint.Y][testPoint.X];
      if((int)c <= (int)curH + 1)
        results.Add(testPoint);
    }

    //search down
    if (p.Y < grid.Length - 1)
    {
      var testPoint = new GridPoint(p.X, p.Y + 1);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c <= (int)curH + 1)
        results.Add(testPoint);
    }

    //search left
    if (p.X > 0)
    {
      var testPoint = new GridPoint(p.X - 1, p.Y);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c <= (int)curH + 1)
        results.Add(testPoint);
    }

    //search right
    if (p.X < grid[0].Length - 1)
    {
      var testPoint = new GridPoint(p.X + 1, p.Y);
      var c = grid[testPoint.Y][testPoint.X];
      if ((int)c <= (int)curH + 1)
        results.Add(testPoint);
    }

    return results;
  }

  private class PointInfo
  {
    public int BestDistance { get; set; }
    public readonly List<GridPoint> ReachableNeighbors;
    public readonly char Height;

    public PointInfo(List<GridPoint> reachableNeighbors, char height)
    {
      BestDistance = int.MaxValue;
      ReachableNeighbors = reachableNeighbors;
      Height = height;
    }
  }

  private struct GridPoint : IEquatable<GridPoint>
  {
    public readonly int X;
    public readonly int Y;

    public GridPoint(int x, int y)
    {
      X = x;
      Y = y;
    }

    public bool Equals(GridPoint other)
    {
      return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
      return obj is GridPoint other && Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(X, Y);
    }

    public static bool operator ==(GridPoint a, GridPoint b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(GridPoint a, GridPoint b)
    {
      return !a.Equals(b);
    }
  }
}