using System.Text;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day25;

public class Day25Problems : Problems
{
  protected override string TestInput => @"";

  protected override int Day => 25;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var output = new StringBuilder();
    var currentPositionsToReplace = new HashSet<int>();
    var futurePositionsToReplace = new HashSet<int>();
    
    for (var i = 0; i < input.Length; i++)
    {
      var line = input[i];
      
      if (IsTabLine(line))
      {
        for (var j = 0; j < line.Length; j++)
        {
          if (currentPositionsToReplace.Contains(j))
          {
            output.Append('4');
          }
          else
          {
            var c = line[j];
            switch (c)
            {
              case '1':
                output.Append('0');
                break;
              case '2':
                output.Append('1');
                break;
              case '3':
                output.Append('2');
                break;
              case '4':
                output.Append('3');
                break;
              case '0':
                output.Append('-');
                futurePositionsToReplace.Add(j);
                break;
              default:
                output.Append(c);
                break;
            }
          }
        }

        currentPositionsToReplace = futurePositionsToReplace;
        futurePositionsToReplace = new HashSet<int>();
        output.AppendLine();
      }
      else
      {
        output.AppendLine(line);
      }
    }

    return output.ToString();
  }

  private static bool IsTabLine(string line) => line.Contains('|');

  protected override string Problem2(string[] input, bool isTestInput)
  {    
    throw new NotImplementedException();
  }
}