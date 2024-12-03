using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day2;

public class Day2Problems : Problems
{
  private static readonly Regex RedRegex = new("(\\d+) red", RegexOptions.Compiled);
  private static readonly Regex BlueRegex = new("(\\d+) blue", RegexOptions.Compiled);
  private static readonly Regex GreenRegex = new("(\\d+) green", RegexOptions.Compiled);

  protected override string TestInput => @"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green";

  protected override int Day => 2;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    return CalculateTotalPossibleGames(input).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    return CalculateTotalMinCubePower(input).ToString();
  }

  private static int CalculateTotalPossibleGames(string[] input)
  {
    var idSum = 0;
    const int totalRed = 12;
    const int totalBlue = 14;
    const int totalGreen = 13;

    foreach (var inputLine in input)
    {
      var newGame = new CubeGame(inputLine);
      if (newGame.IsPossible(totalRed, totalBlue, totalGreen)) idSum += newGame.Id;
    }
      
    return idSum;
  }
    
  private static int CalculateTotalMinCubePower(string[] input)
  {
    var sum = 0;

    foreach (var inputLine in input)
    {
      var newGame = new CubeGame(inputLine);
      sum += newGame.CalculateMinimumCubePower();
    }
      
    return sum;
  }

  private class CubeGame
  {
    public int Id { get; }
    public List<GameResult> Results { get; }

    public CubeGame(string rawInput)
    {
      var parts = rawInput.Split(':');
      var rawId = parts[0].Replace("Game ", "");
      Id = int.Parse(rawId);

      var rawResults = parts[1].Split(';');
      Results = rawResults.Select(s => new GameResult(s)).ToList();
    }
      
    public bool IsPossible(int redTotal, int blueTotal, int greenTotal)
    {
      return Results.All(r => r.IsPossible(redTotal, blueTotal, greenTotal));
    }

    public int CalculateMinimumCubePower()
    {
      var minRed = 0;
      var minBlue = 0;
      var minGreen = 0;

      foreach (var result in Results)
      {
        if (result.RedCubes > minRed) minRed = result.RedCubes;
        if (result.BlueCubes > minBlue) minBlue = result.BlueCubes;
        if (result.GreenCubes > minGreen) minGreen = result.GreenCubes;
      }

      return minRed * minBlue * minGreen;
    }
  }

  private class GameResult
  {
    public int RedCubes { get; }
    public int BlueCubes { get; }
    public int GreenCubes { get; }

    public GameResult(string rawInput)
    {
      var redMatch = RedRegex.Match(rawInput);
      var blueMatch = BlueRegex.Match(rawInput);
      var greenMatch = GreenRegex.Match(rawInput);

      RedCubes = redMatch.Success ? int.Parse(redMatch.Groups[1].ToString()) : 0;
      BlueCubes = blueMatch.Success ? int.Parse(blueMatch.Groups[1].ToString()) : 0;
      GreenCubes = greenMatch.Success ? int.Parse(greenMatch.Groups[1].ToString()) : 0;
    }

    public bool IsPossible(int redTotal, int blueTotal, int greenTotal)
      => RedCubes <= redTotal && BlueCubes <= blueTotal && GreenCubes <= greenTotal;
  }
}