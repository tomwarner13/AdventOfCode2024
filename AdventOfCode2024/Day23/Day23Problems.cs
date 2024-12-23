using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day23;

public class Day23Problems : Problems
{
  protected override string TestInput => @"kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn";

  protected override int Day => 23;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var pointsWithTs = new HashSet<string>();
    var map = new Dictionary<string, HashSet<string>>();
    var uniqueResults = new HashSet<string>();

    foreach (var line in input)
    {
      var points = line.Split('-');
      for (var i = 0; i < 2; i++ )
      {
        var point = points[i];
        var other = points[i == 0 ? 1 : 0];
        
        if(point.StartsWith('t')) pointsWithTs.Add(point);
        if (map.TryGetValue(point, out var connections)) connections.Add(other);
        else map[point] = [other];
      }
    }

    foreach (var tPoint in pointsWithTs)
    {
      foreach (var firstDegreeConnection in map[tPoint])
      {
        foreach (var secondDegreeConnection in map[firstDegreeConnection].Where(c => c != tPoint))
        {
          //check if this one connects back to the start
          if(map[secondDegreeConnection].Contains(tPoint)) 
            uniqueResults.Add(MakeSortedKey(tPoint, firstDegreeConnection, secondDegreeConnection));
        }
      }
    }

    return uniqueResults.Count.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    //just do some mf recursion here
    //for each point in map, go through its connections -- confirm recursively that each one is a)
    //  not already in set (so we don't get stuck in a -> b -> a loop) and connected to every computer in set
    //  fail (return existing points) as soon as a computer is found without connection to all. bubbling up,
    //  return longest series of points.
    
    throw new NotImplementedException();
  }

  private static string MakeSortedKey(string p1, string p2, string p3)
  {
    var arr = new[] { p1, p2, p3 };
    Array.Sort(arr);
    return string.Join("|", arr);
  }
}