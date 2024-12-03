// See https://aka.ms/new-console-template for more information

using AdventOfCode2024.Day2;
using AdventOfCode2024.Util;

var problems = new Day2Problems();
DoAllProblems(problems);


void DoAllProblems(Problems probs)
{
  TryPrintResult(probs.Problem1TestInput, "Problem 1 Test Input");
  TryPrintResult(probs.Problem1FullInput, "Problem 1 Full Input");
  TryPrintResult(probs.Problem2TestInput, "Problem 2 Test Input");
  TryPrintResult(probs.Problem2FullInput, "Problem 2 Full Input");
}

void TryPrintResult(Func<string> attempter, string description)
{
  try
  {
    var result = attempter();

    Console.WriteLine($"{description}:");
    Console.WriteLine(result);
  }
  catch (NotImplementedException)
  {
    Console.WriteLine($"{description} not implemented");
  }
  catch(Exception e)
  {
    Console.WriteLine($"{description} failed:");
    Console.WriteLine(e);
  }

  Console.WriteLine();
}