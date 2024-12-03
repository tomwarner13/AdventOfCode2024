namespace AdventOfCode2024.Util;

public abstract class Problems
{
  protected abstract string TestInput { get; }

  private string FullInputFilePath => $"Day{Day}\\D{Day}.txt";

  protected abstract int Day { get; }

  protected abstract string Problem1(string[] input, bool isTestInput);
  protected abstract string Problem2(string[] input, bool isTestInput);

  public string Problem1TestInput()
  {
    var lines = TestInput.Split('\n').Select(s => s.Trim()).ToArray();
    return Problem1(lines, true);
  }

  public string Problem2TestInput()
  {
    var lines = TestInput.Split('\n').Select(s => s.Trim()).ToArray();
    return Problem2(lines, true);
  }

  public string Problem1FullInput()
  {
    var lines = File.ReadAllLines(FullInputFilePath);
    return Problem1(lines, false);
  }

  public string Problem2FullInput()
  {
    var lines = File.ReadAllLines(FullInputFilePath);
    return Problem2(lines, false);
  }
}