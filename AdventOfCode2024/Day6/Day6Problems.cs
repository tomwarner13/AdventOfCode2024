using System.Net.Http.Headers;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day6;

public class Day6Problems : Problems
{
  protected override string TestInput => @"Time:      7  15   30
Distance:  9  40  200";

  protected override int Day => 6;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    return GetRaceTimesMultiplier(input).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var translatedInput = new string[2];
    translatedInput[0] = 
      string.Join("", StringUtils.ExtractIntsFromString(input[0]).Select(i => i.ToString()).ToArray());    
    translatedInput[1] = 
      string.Join("", StringUtils.ExtractIntsFromString(input[1]).Select(i => i.ToString()).ToArray());
    
    return GetRaceTimesMultiplier(translatedInput).ToString();
  }

  private static long GetRaceTimesMultiplier(string[] input)
  {
    var raceTimes = StringUtils.ExtractLongsFromString(input[0]).ToArray();
    var raceRecords = StringUtils.ExtractLongsFromString(input[1]).ToArray();

    var i = 0;
    var sum = (long)1;
    foreach (var totalTime in raceTimes)
    {
      var record = raceRecords[i];
      var timeBounds = GetWinningTimeRange(totalTime, record);
      var lowestBound = (long)Math.Ceiling(timeBounds.low);
      var highestBound = (long)Math.Floor(timeBounds.high);

      //these are exclusionary ranges so if the doubles started as whole numbers, add to them
      if (timeBounds.low % 1 == 0) lowestBound++;
      if (timeBounds.high % 1 == 0) highestBound--;
      
      var timeRange = highestBound - lowestBound + 1;
      sum *= timeRange;
      i++;
    }

    return sum;
  }

  private static (double low, double high) GetWinningTimeRange(double totalTime, double recordDistance)
  {
    //quadratic formula: a = -1, b = totalTime, c = -recordDistance 
    var lowestBound = (-totalTime + Math.Sqrt((totalTime * totalTime) - (-4 * -recordDistance))) / -2;
    var highestBound = (-totalTime - Math.Sqrt((totalTime * totalTime) - (-4 * -recordDistance))) / -2;

    return (lowestBound, highestBound);
  }
}