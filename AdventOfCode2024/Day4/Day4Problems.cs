using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day4;

public class Day4Problems : Problems
{
  protected override string TestInput => @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";

  protected override int Day => 4;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    return AddWinningNumberPowers(input).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    return AddWinningNumberPowersWithCopyLogic(input).ToString();
  }

  private static int AddWinningNumberPowers(string[] input)
  {
    var sum = 0;
    foreach (var line in input)
    {
      var partWeCareAbout = line.Split(':')[1];
      var segments = partWeCareAbout.Split('|');
      var winningNumbers = StringUtils.ExtractIntsFromString(segments[0]).ToHashSet();
      var cardNumbers = StringUtils.ExtractIntsFromString(segments[1]).ToList();
      var totalWinners = cardNumbers.Count(c => winningNumbers.Contains(c));
      if (totalWinners > 0) sum += (int)Math.Pow(2, (totalWinners - 1));
    }

    return sum;
  }
  
  private static int AddWinningNumberPowersWithCopyLogic(string[] input)
  {
    var sum = 0;
    var totalScratchcards = 0;
    var index = 0;
    var copyMultipliers = new int[input.Length];
    
    foreach (var line in input)
    {
      var partWeCareAbout = line.Split(':')[1];
      var segments = partWeCareAbout.Split('|');
      var winningNumbers = StringUtils.ExtractIntsFromString(segments[0]).ToHashSet();
      var cardNumbers = StringUtils.ExtractIntsFromString(segments[1]).ToList();
      var totalWinners = cardNumbers.Count(c => winningNumbers.Contains(c));
      
      //still win copies of non-winning cards
      var currentMultiplier = copyMultipliers[index] + 1;
      totalScratchcards += currentMultiplier;
      
      if (totalWinners > 0)
      {
        var cardSum = (int)Math.Pow(2, (totalWinners - 1));
        sum += cardSum * currentMultiplier;
        
        for (var i = index + 1; i <= index + totalWinners; i++)
        {
          copyMultipliers[i] += currentMultiplier;
        }
      }

      index++;
    }
    return totalScratchcards;
  }
}