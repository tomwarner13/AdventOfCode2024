using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day14;

public class Day14Problems : Problems
{
  protected override string TestInput => @"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9";

  protected override int Day => 14;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var grid = new RockFormations();

    AddFormations(grid, input);

    var grains = 0;
    var lowestPoint = grid.LowestPoint;
    var canDrop = true;

    while (canDrop)
    {
      var moving = true;
      var currentGrain = new GridPoint(500, 0);

      while (moving && canDrop)
      {
        //try move directly down
        var pointBelow = new GridPoint(currentGrain.X, currentGrain.Y + 1);
        var pointLeft = new GridPoint(currentGrain.X - 1, currentGrain.Y + 1);
        var pointRight = new GridPoint(currentGrain.X + 1, currentGrain.Y + 1);
        if (grid.CheckPoint(pointBelow))
        {
          currentGrain = pointBelow;
        }
        else if (grid.CheckPoint(pointLeft))
        {
          currentGrain = pointLeft;
        }
        else if (grid.CheckPoint(pointRight))
        {
          currentGrain = pointRight;
        }
        else //come to rest
        {
          grid.AddPoint(currentGrain);
          moving = false;
          grains++;
        }

        //check if we're below the grid
        if (currentGrain.Y > lowestPoint)
        {
          canDrop = false;
        }
      }
    }

    return grains.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var grid = new RockFormations();

    AddFormations(grid, input);

    var grains = 0;
    var lowestPoint = grid.LowestPoint;
    var canDrop = true;

    grid.AddFloor(lowestPoint + 2);

    while (canDrop)
    {
      var moving = true;
      var currentGrain = new GridPoint(500, 0);

      while (moving && canDrop)
      {
        //try move directly down
        var pointBelow = new GridPoint(currentGrain.X, currentGrain.Y + 1);
        var pointLeft = new GridPoint(currentGrain.X - 1, currentGrain.Y + 1);
        var pointRight = new GridPoint(currentGrain.X + 1, currentGrain.Y + 1);
        if (grid.CheckPoint(pointBelow))
        {
          currentGrain = pointBelow;
        }
        else if (grid.CheckPoint(pointLeft))
        {
          currentGrain = pointLeft;
        }
        else if (grid.CheckPoint(pointRight))
        {
          currentGrain = pointRight;
        }
        else //come to rest
        {
          grid.AddPoint(currentGrain);
          moving = false;
          grains++;
        }

        //check if we're at the start
        if (currentGrain.Y == 0 && currentGrain.X == 500)
        {
          canDrop = false;
        }
      }
    }

    return grains.ToString();
  }

  private string[] ParseLineToRaw(string line)
  {
    return line.Split(" -> ");
  }

  private void AddFormations(RockFormations grid, string[] lines)
  {
    foreach (var line in lines)
    {
      AddLine(grid, ParseLineToRaw(line));
    }
  }

  private void AddLine(RockFormations grid, string[] points)
  {
    var lastPoint = ParsePoint(points[0]);

    for (var i = 1; i < points.Length; i++)
    {
      var curPoint = ParsePoint(points[i]);
      grid.AddPointRange(lastPoint, curPoint);
      lastPoint = curPoint;
    }
  }

  private GridPoint ParsePoint(string raw)
  {
    var parts = raw.Split(',');
    return new GridPoint(int.Parse(parts[0]), int.Parse(parts[1]));
  }

  private class RockFormations
  {
    private readonly HashSet<GridPoint> _occupiedSpaces;
    private int _floor;

    public RockFormations()
    {
      _occupiedSpaces = new HashSet<GridPoint>();
      LowestPoint = 0;
      _floor = -1;
    }

    public void AddPointRange(GridPoint start, GridPoint end)
    {
      var startX = Math.Min(start.X, end.X);
      var startY = Math.Min(start.Y, end.Y);

      var endX = Math.Max(start.X, end.X);
      var endY = Math.Max(start.Y, end.Y);

      if (startX == endX) //vertical
      {
        for (var y = startY; y <= endY; y++)
        {
          AddPoint(new GridPoint(startX, y));
        }

        return;
      }

      if (startY == endY) //horizontal
      {
        for (var x = startX; x <= endX; x++)
        {
          AddPoint(new GridPoint(x, startY));
        }

        return;
      }

      throw new ArgumentException("can't do 3d range");
    }

    public void AddPoint(GridPoint point)
    {
      _occupiedSpaces.Add(point);

      if (point.Y > LowestPoint)
        LowestPoint = point.Y;
    }

    public void AddFloor(int floorLevel)
    {
      _floor = floorLevel;
    }

    public bool CheckPoint(GridPoint point)
    {
      if (_floor > 0 && point.Y >= _floor)
      {
        return false;
      }

      return !_occupiedSpaces.Contains(point);
    }

    public int LowestPoint { get; private set; }
  }
}