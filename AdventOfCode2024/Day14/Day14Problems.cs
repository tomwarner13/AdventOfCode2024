using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day14;

public class Day14Problems : Problems
{
  protected override string TestInput => @"p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3";

  protected override int Day => 14;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var xBound = isTestInput ? 11 : 101;
    var yBound = isTestInput ? 7 : 103;
    const int steps = 100;

    var totalTopRight = 0;
    var totalTopLeft = 0;
    var totalBotRight = 0;
    var totalBotLeft = 0;
    
    var xHalf = xBound / 2;
    var yHalf = yBound / 2;

    foreach (var line in input)
    {
      var guardInit = StringUtils.ExtractIntsFromString(line, true).ToArray();
      var xDest = (guardInit[0] + (guardInit[2] * steps)) % xBound;
      var yDest = (guardInit[1] + (guardInit[3] * steps)) % yBound;

      if(xDest < 0) xDest += xBound;
      if(yDest < 0) yDest += yBound;
      
      if (xDest < xHalf)
      {
        if (yDest < yHalf)
        {
          totalTopLeft++;
        }
        else if (yDest > yHalf)
        {
          totalBotLeft++;
        }
      }
      else if (xDest > xHalf)
      {
        if (yDest < yHalf)
        {
          totalTopRight++;
        }
        else if (yDest > yHalf)
        {
          totalBotRight++;
        }
      }
    }
    return (totalTopLeft * totalBotLeft * totalTopRight * totalBotRight).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }
}