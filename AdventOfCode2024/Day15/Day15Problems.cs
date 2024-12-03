using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day15;

public class Day15Problems : Problems
{
  protected override string TestInput => @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3";

  protected override int Day => 15;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var validSensors = new List<Sensor>();
    var beaconsOnHorizontalRow = new HashSet<GridPoint>();
      
    var horizontal = isTestInput ? 10 : 2000000;

    foreach (var line in input)
    {
      var checkResults = ParseLine(line);
      if (checkResults.s.IsWithinVerticalRange(horizontal))
      {
        validSensors.Add(checkResults.s);
      }

      if (checkResults.b.Y == horizontal)
      {
        beaconsOnHorizontalRow.Add(checkResults.b);
      }
    }

    var longestDist = validSensors.Select(s => s.Distance).Max();
    var orderedHorizontally = validSensors.Select(s => s.Location.X).OrderBy(x => x).ToList();
    var lowestX = orderedHorizontally.First();
    var highestX = orderedHorizontally.Last();

    var min = lowestX - longestDist;
    var max = highestX + longestDist;

    var totalExcludedPoints = 0;
    for (var x = min; x <= max; x++)
    {
      var pointToCheck = new GridPoint(x, horizontal);
      if (validSensors.Any(s => s.IsWithinRange(pointToCheck)) & !beaconsOnHorizontalRow.Contains(pointToCheck))
      {
        totalExcludedPoints++;
      }
    }

    return totalExcludedPoints.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var allSensors = new List<Sensor>();
      
    var boundaries = isTestInput ? 20 : 4_000_000;

    foreach (var line in input)
    {
      var checkResults = ParseLine(line);
      allSensors.Add(checkResults.s);
    }
      
    foreach (var sensor in allSensors)
    {
      var pointsToCheck = sensor.GetPointsAroundPerimeter()
        .Where(p => p.X >= 0 && p.Y >= 0 && p.X <= boundaries && p.Y <= boundaries);

      foreach (var checkPoint in pointsToCheck)
      {
        if (!allSensors.Any(s => s.IsWithinRange(checkPoint)))
        {
          // match found
          var signal = ((long)checkPoint.X * 4_000_000) + checkPoint.Y; //this overflows on an int :'(
          return signal.ToString();
        }
      }
    }

    return "busted";
  }

  private static readonly Regex LineParser =
    new(
      "Sensor at x=([-\\d]+), y=([-\\d]+): closest beacon is at x=([-\\d]+), y=([-\\d]+)",
      RegexOptions.Compiled);

  private static (Sensor s, GridPoint b) ParseLine(string line)
  {
    var match = LineParser.Matches(line).First();
    var sX = int.Parse(match.Groups[1].Value);
    var sY = int.Parse(match.Groups[2].Value);
    var bX = int.Parse(match.Groups[3].Value);
    var bY = int.Parse(match.Groups[4].Value);

    var beacon = new GridPoint(bX, bY);

    return (new Sensor(new GridPoint(sX, sY), beacon), beacon);
  }

  private class Sensor
  {
    public readonly GridPoint Location;
    public readonly int Distance;

    public Sensor(GridPoint location, GridPoint beacon)
    {
      Location = location;
      Distance = CalculateDistanceToPoint(beacon);
    }

    public int CalculateDistanceToPoint(GridPoint other)
    {
      var xDist = Math.Abs(Location.X - other.X);
      var yDist = Math.Abs(Location.Y - other.Y);

      return xDist + yDist;
    }

    public bool IsWithinRange(GridPoint other)
    {
      return CalculateDistanceToPoint(other) <= Distance;
    }

    public bool IsWithinVerticalRange(int vertical)
    {
      return Math.Abs(vertical - Location.Y) <= Distance;
    }

    public IEnumerable<GridPoint> GetPointsAroundPerimeter()
    {
      var curY = Location.Y - Distance - 1;
      yield return new GridPoint(Location.X, Location.Y - Distance - 1);

      curY++;
      var curLeft = Location.X - 1;
      var curRight = Location.X + 1;

      //expand
      while (curY <= Location.Y)
      {
        yield return new GridPoint(curLeft, curY);
        yield return new GridPoint(curRight, curY);

        curY++;
        curLeft--;
        curRight++;
      }

      //contract
      while (curY <= Location.Y + Distance)
      {
        yield return new GridPoint(curLeft, curY);
        yield return new GridPoint(curRight, curY);

        curY++;
        curLeft++;
        curRight--;
      }

      yield return new GridPoint(Location.X, Location.Y + Distance + 1);
    }
  }
}