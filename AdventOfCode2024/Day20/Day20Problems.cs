using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day20;

public class Day20Problems : Problems
{
  protected override string TestInput => @"###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############";

  protected override int Day => 20;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var startPoint = new GridPoint();
    var endPoint = new GridPoint();
    var emptyPoints = new HashSet<GridPoint>();
    var minimumCheatThreshold = isTestInput ? 10 : 100; //with test input, we should see 10 cheats over threshold
    DebugMode = isTestInput;

    StringUtils.ReadInputGrid(input, (c, x, y) =>
    {
      if (c != '#')
      {
        var point = new GridPoint(x, y);
        emptyPoints.Add(point);
        switch (c)
        {
          case 'S':
            startPoint = point;
            break;
          case 'E':
            endPoint = point;
            break;
        }
      }
    });
    
    var route = GetRouteAndDistances(startPoint, endPoint, emptyPoints);
    Debug($"route found: {route.Count}");

    var pointsToCheck = route
      .Where(p => p.Value + 2 + minimumCheatThreshold < route.Count)
      .OrderBy(p => p.Value);

    var cheatPointsFound = 0;
    var visitedPoints = new HashSet<GridPoint>();
    
    foreach (var point in pointsToCheck)
    {
      foreach (var direction in GridPoint.CardinalDirections)
      {
        var neighbor = point.Key + direction;
        if (!emptyPoints.Contains(neighbor))
        {
          var dest = neighbor + direction;
          if (!visitedPoints.Contains(dest) && route.TryGetValue(dest, out var jumpDistance))
          {
            Debug($"Found jump: from {point.Key}:{point.Value} to {dest}:{jumpDistance}");
            var cheatDist = jumpDistance - point.Value - 2;
            if (cheatDist >= minimumCheatThreshold)
            {
              Debug($"adding above as point: cheatDist: {cheatDist}");
              cheatPointsFound++;
            }
          }
        }
      }
      visitedPoints.Add(point.Key);
    }

    return cheatPointsFound.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {    
    throw new NotImplementedException();
  }

  //dijkstra running (perfectly path-optimized) victory laps out here
  private static Dictionary<GridPoint, int> GetRouteAndDistances(GridPoint start, GridPoint end,
    HashSet<GridPoint> emptyPoints)
  {
    var route = new Dictionary<GridPoint, int>();
    
    var knownByDistance = new Dictionary<GridPoint, int>
    {
      [start] = 0
    };
    var currentPoint = start;

    while (route.Count < emptyPoints.Count)
    {
      var currentDistance = knownByDistance[currentPoint];

      if (currentPoint == end)
      {
        route[currentPoint] = currentDistance;
        return route;
      }

      var possiblePoints = 
        GridPoint.CardinalDirections.Select(p => currentPoint + p);
      
      var distanceToPoint = currentDistance + 1;

      foreach (var point in possiblePoints
                 .Where(p => 
                   emptyPoints.Contains(p) && !route.ContainsKey(p)))
      {
        if (!knownByDistance.TryGetValue(point, out var knownDistance) || knownDistance > distanceToPoint)
        {
          knownByDistance[point] = distanceToPoint;
        }
      }
      
      route.Add(currentPoint, currentDistance);
      knownByDistance.Remove(currentPoint);

      currentPoint = knownByDistance.MinBy(r => r.Value).Key;
    }

    throw new ThisShouldNeverHappenException();
  }
}