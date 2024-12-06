﻿namespace AdventOfCode2024.Util;

public static class StringUtils
{
  public static IEnumerable<int> ExtractIntsFromString(string input, bool includeNegative = false)
  {
    var regex = includeNegative ? RegexUtils.BasicDigitNegativeRegex : RegexUtils.BasicDigitRegex;
    var matches = regex.Matches(input);
    return matches.Select(s => int.Parse(s.ToString()));
  }
  
  public static IEnumerable<string> ExtractWordsFromString(string input)
  {
    var matches = RegexUtils.BasicLetterRegex.Matches(input);
    return matches.Select(s => s.ToString());
  }
  
  public static IEnumerable<long> ExtractLongsFromString(string input, bool includeNegative = false)
  {
    var regex = includeNegative ? RegexUtils.BasicDigitNegativeRegex : RegexUtils.BasicDigitRegex;
    var matches = regex.Matches(input);
    return matches.Select(s => long.Parse(s.ToString()));
  }
  
  public static IEnumerable<string> ExtractAlphanumericsFromString(string input)
  {
    var matches = RegexUtils.BasicWordRegex.Matches(input);
    return matches.Select(s => s.ToString());
  }
}