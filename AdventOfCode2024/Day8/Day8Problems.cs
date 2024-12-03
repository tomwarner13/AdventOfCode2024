using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day8;

public class Day8Problems : Problems
{
  protected override string TestInput => @"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)";

  protected override int Day => 8;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var moveInstructions = input[0];

    var map = new Dictionary<string, (string left, string right)>();
    const string endNode = "ZZZ";
    var currentNode = "AAA";

    var currentStep = 0;
    
    //parse and setup dictionary
    foreach (var line in input.Take(new Range(2, input.Length))) //does this throw lol
    {
      var parts = StringUtils.ExtractWordsFromString(line).ToArray();
      map.Add(parts[0], (parts[1], parts[2]));
    }

    while (currentNode != endNode)
    {
      var nodeDetails = map[currentNode];
      var currentDirectionIndex = currentStep % moveInstructions.Length;
      var currentDirection = moveInstructions[currentDirectionIndex];

      currentNode = currentDirection switch
      {
        'R' => nodeDetails.right,
        'L' => nodeDetails.left,
        _ => throw new ThisShouldNeverHappenException("invalid direction")
      };

      currentStep++;
    }

    return currentStep.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    //problem 2 test input is different
    if (isTestInput)
    {
      const string p2Test = @"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)";
      
      input = p2Test.Split('\n').Select(s => s.Trim()).ToArray();
    }
    
    
    var moveInstructions = input[0];

    var map = new Dictionary<string, (string left, string right)>();
    var currentNodes = new List<string>();

    var currentStep = 0;
    
    //parse and setup dictionary
    foreach (var line in input.Take(new Range(2, input.Length))) //does this throw lol
    {
      var parts = StringUtils.ExtractAlphanumericsFromString(line).ToArray();
      map.Add(parts[0], (parts[1], parts[2]));
      if(parts[0][2] == 'A') currentNodes.Add(parts[0]);
    }

    while (ShouldContinue(currentNodes))
    {
      
      var currentDirectionIndex = currentStep % moveInstructions.Length;
      var currentDirection = moveInstructions[currentDirectionIndex];

      var nextNodes = new List<string>();
      foreach (var currentNode in currentNodes)
      {
        var nodeDetails = map[currentNode];

        nextNodes.Add(currentDirection switch
        {
          'R' => nodeDetails.right,
          'L' => nodeDetails.left,
          _ => throw new ThisShouldNeverHappenException("invalid direction")
        });
      }

      currentNodes = nextNodes;
      currentStep++;
    }

    return currentStep.ToString();
  }

  private static bool ShouldContinue(IEnumerable<string> currentNodes) =>
    currentNodes.Any(n => n[2] != 'Z');
}