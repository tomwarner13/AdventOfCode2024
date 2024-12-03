using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day3;

public class Day3Problems : Problems
{
  private static readonly Regex BasicDigitRegex = new("\\d+", RegexOptions.Compiled);
  
  protected override string TestInput => @"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..";

  protected override int Day => 3;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    return FindAndAddPartNumbers(input).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    return EvaluateLinesForGearMatches(input).ToString();
  }

  private static int FindAndAddPartNumbers(string[] input)
  {
    var sum = 0;
    var lineNum = 0;
    
    foreach (var line in input)
    {
      if (lineNum == 0)
      {
        sum += EvaluateOneLineForAdjacentNumbers(string.Empty, line, input[lineNum + 1]);
      }
      else if (lineNum == input.Length - 1)
      {
        sum += EvaluateOneLineForAdjacentNumbers(input[lineNum - 1], line, string.Empty);
      }
      else
      {
        sum += EvaluateOneLineForAdjacentNumbers(input[lineNum - 1], line, input[lineNum + 1]);
      }

      lineNum++;
    }

    return sum;
  }

  private static int EvaluateLinesForGearMatches(string[] input)
  {
    var sum = 0;
    var n = 0;

    foreach (var line in input)
    {
      for (var i = 0; i < line.Length; i++)
      {
        var charToCheck = line[i];
        if (CharacterIsGear(charToCheck))
        {
          var foundNumbers = SearchAdjacentNumbersFromPosition(line, i);
          if(n > 0) foundNumbers.AddRange(SearchAdjacentNumbersFromPosition(input[n - 1], i));
          if(n < input.Length - 1) foundNumbers.AddRange(SearchAdjacentNumbersFromPosition(input[n + 1], i));

          if (foundNumbers.Count == 2) sum += foundNumbers[0] * foundNumbers[1];
        }
      }

      n++;
    }

    return sum;
  }

  private static int EvaluateOneLineForAdjacentNumbers(string lineAbove, string lineToEvaluate, string lineBelow)
  {
    var sum = 0;
    var currentNumber = string.Empty;
    var currentNumberHasAdjacentSymbol = false;
    
    for (var i = 0; i < lineToEvaluate.Length; i++)
    {
      if (CharacterIsDigit(lineToEvaluate[i]))
      {
        currentNumber += lineToEvaluate[i];
        
        if (!currentNumberHasAdjacentSymbol) //only check if we haven't found a match already
        {
          //check left characters for symbol matches
          if (i > 0 && !CharacterIsDigit(lineToEvaluate[i - 1]))
          {
            //check character to left
            if (CharacterIsSymbol(lineToEvaluate[i - 1])) currentNumberHasAdjacentSymbol = true;
            //check up left
            if(lineAbove.Length > 0 && CharacterIsSymbol(lineAbove[i - 1])) currentNumberHasAdjacentSymbol = true;
            //check down left
            if(lineBelow.Length > 0 && CharacterIsSymbol(lineBelow[i - 1])) currentNumberHasAdjacentSymbol = true;
          }
          
          //check right characters for symbol matches
          if (i < lineToEvaluate.Length - 1 && !CharacterIsDigit(lineToEvaluate[i + 1]))
          {
            //check character to right
            if (CharacterIsSymbol(lineToEvaluate[i + 1])) currentNumberHasAdjacentSymbol = true;
            //check up right
            if(lineAbove.Length > 0 && CharacterIsSymbol(lineAbove[i + 1])) currentNumberHasAdjacentSymbol = true;
            //check down right
            if(lineBelow.Length > 0 && CharacterIsSymbol(lineBelow[i + 1])) currentNumberHasAdjacentSymbol = true;
          }
          
          //check directly above and below
          if(lineAbove.Length > 0 && CharacterIsSymbol(lineAbove[i])) currentNumberHasAdjacentSymbol = true;
          if(lineBelow.Length > 0 && CharacterIsSymbol(lineBelow[i])) currentNumberHasAdjacentSymbol = true;
        }
      }
      else
      {
        //we've found a number that matches
        if (currentNumberHasAdjacentSymbol && currentNumber.Length > 0)
        {
          sum += int.Parse(currentNumber);
        }
        
        currentNumber = string.Empty;
        currentNumberHasAdjacentSymbol = false;
      }
    }
    
    //if the line ends on a match, still include it
    if (currentNumberHasAdjacentSymbol && currentNumber.Length > 0)
    {
      sum += int.Parse(currentNumber);
    }
    
    return sum;
  }

  private static List<int> SearchAdjacentNumbersFromPosition(string line, int position)
  {
    var foundStr = $"{line[position]}";
    var keepLooking = true;
    var searchPos = position - 1;
    
    //look left
    while (keepLooking && searchPos >= 0)
    {
      if (CharacterIsDigit(line[searchPos]))
      {
        foundStr = $"{line[searchPos]}{foundStr}";
      }
      else
      {
        keepLooking = false;
      }

      searchPos--;
    }

    //look right
    searchPos = position + 1;
    keepLooking = true;
    while (keepLooking && searchPos < line.Length)
    {
      if (CharacterIsDigit(line[searchPos]))
      {
        foundStr = $"{foundStr}{line[searchPos]}";
      }
      else
      {
        keepLooking = false;
      }

      searchPos++;
    }

    return BasicDigitRegex.Matches(foundStr)
      .Select(s => int.Parse(s.ToString()))
      .ToList();
  }

  private static bool CharacterIsDigit(char c) =>
    c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9';

  private static bool CharacterIsSymbol(char c) => 
    c != '.' && !CharacterIsDigit(c);

  private static bool CharacterIsGear(char c) => c == '*';
}