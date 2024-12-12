using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day12;

public class Day12Problems : Problems
{
  protected override string TestInput => @"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE";

  protected override int Day => 12;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var visitedPoints = new HashSet<GridPoint>();
    var totalPrice = 0L;

    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[y].Length; x++)
      {
        var point = new GridPoint(x, y);
        if (!visitedPoints.Contains(point))
        {
          var region = CalculateRegion(input[y][x], point, ref input, ref visitedPoints);
          totalPrice += region.perimeter * region.area;
        }
      }
    }
    
    return totalPrice.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  private static (int perimeter, int area) CalculateRegion(char seed, GridPoint point, ref string[] input,
    ref HashSet<GridPoint> visitedPoints)
  {
    if (!point.IsInBounds(input[0].Length, input.Length) || input[point.Y][point.X] != seed) return (1, 0);
    
    if (!visitedPoints.Add(point)) return (0, 0);

    var perimeter = 0;
    var area = 1;

    foreach (var dir in GridPoint.CardinalDirections)
    {
      var result = CalculateRegion(seed, point + dir, ref input, ref visitedPoints);
      perimeter += result.perimeter;
      area += result.area;
    }
    
    return (perimeter, area);
  }
}