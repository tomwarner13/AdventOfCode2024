using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day10;

public class Day10Problems : Problems
{
  protected override string TestInput => @"7-F7-
.FJ|7
SJLL7
|F--J
LJ.LJ";

  protected override int Day => 10;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var map = input;
    //have to find index of start
    var startIndex = new GridPoint();
    var startFound = false;
    for(var i = 0; i < input.Length; i++)
    {
      var foundSIndex = input[i].IndexOf('S');
      if (foundSIndex != -1)
      {
        startIndex = new GridPoint(foundSIndex, i);
        i = input.Length;
        startFound = true;
      }
    }

    if (!startFound) throw new ThisShouldNeverHappenException("start never found!");
    
    //find the two directions which connect to the start point, embark from there
    var connectedPoints = CheckForAdjacentConnections(map, startIndex).ToList();

    if (connectedPoints.Count != 2) throw new ThisShouldNeverHappenException($"connectedpoints count wrong!");
    
    var firstRoute = connectedPoints[0];
    var firstRoutePrev = startIndex;
    var secondRoute = connectedPoints[1];
    var secondRoutePrev = startIndex;
    var stepsTaken = 1;

    while (firstRoute != secondRoute)
    {
      var firstRouteNext = FindNextPoint(firstRoute, firstRoutePrev, map);
      var secondRouteNext = FindNextPoint(secondRoute, secondRoutePrev, map);

      firstRoutePrev = firstRoute;
      firstRoute = firstRouteNext;

      secondRoutePrev = secondRoute;
      secondRoute = secondRouteNext;
      
      stepsTaken++;
    }

    return stepsTaken.ToString();
  }

  private static GridPoint FindNextPoint(GridPoint current, GridPoint previous, string[] map)
  {
    var possibilities = GetAdjacentConnectedPoints(map[current.Y][current.X]);
    return possibilities.p1 + current != previous ? possibilities.p1 + current : possibilities.p2 + current;
  }

  private static IEnumerable<GridPoint> CheckForAdjacentConnections(string[] map, GridPoint target)
  {
    var possibleAdjacencies = new GridPoint[]
    {
      new(-1, 0),
      new(1, 0),
      new(0, -1),
      new(0, 1)
    };

    foreach (var possibleAdjacency in possibleAdjacencies)
    {
      var checkPoint = target + possibleAdjacency;
      //check if in bounds first
      if (checkPoint.X >= 0
          && checkPoint.X < map[0].Length
          && checkPoint.Y >= 0
          && checkPoint.Y < map.Length)
      {
        var charToCheck = map[checkPoint.Y][checkPoint.X];
        if (IsPipeChar(charToCheck))
        {
          var points = GetAdjacentConnectedPoints(charToCheck);
          if (checkPoint + points.p1 == target || checkPoint + points.p2 == target)
          {
            yield return checkPoint;
          }
        }
      }
    }
  }
  
  private static (GridPoint p1, GridPoint p2) GetAdjacentConnectedPoints(char c)
  {
    return c switch
    {
      '|' => (new GridPoint(0, -1), new GridPoint(0, 1)),
      '-' => (new GridPoint(-1, 0), new GridPoint(1, 0)),
      'L' => (new GridPoint(0, -1), new GridPoint(1, 0)),
      'J' => (new GridPoint(-1, 0), new GridPoint(0, -1)),
      '7' => (new GridPoint(-1, 0), new GridPoint(0, 1)),
      'F' => (new GridPoint(0, 1), new GridPoint(1, 0)),
      _ => throw new ThisShouldNeverHappenException($"attempted to get connections to character '{c}'")
    };
  }

  private static bool IsPipeChar(char c)
    => c is '|' or '-' or 'L' or 'J' or '7' or 'F';

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }
}