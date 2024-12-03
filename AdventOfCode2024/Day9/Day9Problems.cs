using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day9;

public class Day9Problems : Problems
{
  protected override string TestInput => @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";

  protected override int Day => 9;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    long sum = 0;
    foreach (var line in input)
    {
      var rawList = StringUtils.ExtractIntsFromString(line, true).ToList();
      var nextNumber = RecursivelyAddToSequence(rawList);
      sum += nextNumber;
    }

    return sum.ToString();
  }

  private static int RecursivelyAddToSequence(List<int> sequence)
  {
    var result = new List<int>();
    var allZeroes = true;
    int? prevValue = null;

    foreach (var item in sequence)
    {
      if (item != 0) allZeroes = false;

      if (prevValue != null)
      {
        result.Add(item - prevValue.Value);
      }

      prevValue = item;
    }

    if (allZeroes)
    {
      return 0;
    }

    var nextResult = RecursivelyAddToSequence(result);
    return nextResult + sequence.Last();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    long sum = 0;
    foreach (var line in input)
    {
      var rawList = StringUtils.ExtractIntsFromString(line, true).ToList();
      var nextNumber = RecursivelyAddToBeginningOfSequence(rawList);
      sum += nextNumber;
    }

    return sum.ToString();
  }
  
  private static int RecursivelyAddToBeginningOfSequence(List<int> sequence)
  {
    var result = new List<int>();
    var allZeroes = true;
    int? prevValue = null;

    foreach (var item in sequence)
    {
      if (item != 0) allZeroes = false;

      if (prevValue != null)
      {
        result.Add(item - prevValue.Value);
      }

      prevValue = item;
    }

    if (allZeroes)
    {
      return 0;
    }

    var nextResult = RecursivelyAddToBeginningOfSequence(result);
    return sequence.First() - nextResult;
  }
}