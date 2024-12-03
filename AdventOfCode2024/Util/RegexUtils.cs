using System.Text.RegularExpressions;

namespace AdventOfCode2024.Util;

public static class RegexUtils
{
  public static readonly Regex BasicDigitRegex = new("\\d+", RegexOptions.Compiled);
  public static readonly Regex BasicDigitNegativeRegex = new("[0-9\\-]+", RegexOptions.Compiled);
  public static readonly Regex BasicWordRegex = new("\\w+", RegexOptions.Compiled);
  public static readonly Regex BasicLetterRegex = new("[A-Za-z]+", RegexOptions.Compiled);
}