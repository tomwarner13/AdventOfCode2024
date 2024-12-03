using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day11;

public class Day11Problems : Problems
{
  protected override string TestInput => @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1";

  protected override int Day => 11;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var monkeyList = new List<Monkey>();

    Monkey curMonkey = null;

    foreach (var rawLine in input)
    {
      var line = rawLine.Trim();

      if (line.StartsWith("Monkey "))
      {
        curMonkey = new Monkey();
      }
      else if (line.StartsWith("Starting items:"))
      {
        curMonkey.AddItems(ParseStartingItems(line));
      }
      else if (line.StartsWith("Operation: new = "))
      {
        curMonkey.SetInspection(ParseOperation(line));
      }
      else if (line.StartsWith("Test: divisible by "))
      {
        var raw = line.Replace("Test: divisible by ", "");
        curMonkey.SetTestDivisor(int.Parse(raw));
      }
      else if (line.StartsWith("If true: throw to monkey "))
      {
        var raw = line.Replace("If true: throw to monkey ", "");
        curMonkey.SetTrueTarget(int.Parse(raw));
      }
      else if (line.StartsWith("If false: throw to monkey "))
      {
        var raw = line.Replace("If false: throw to monkey ", "");
        curMonkey.SetFalseTarget(int.Parse(raw));
      }
      else if (string.IsNullOrWhiteSpace(line))
      {
        monkeyList.Add(curMonkey);
      }
      else
      {
        throw new ArgumentException("invalid: " + line);
      }
    }

    //there's a last monkey without corresponding whitespace
    monkeyList.Add(curMonkey);

    var monkeys = monkeyList.ToArray();

    const int totalRounds = 20;

    for (var i = 0; i < totalRounds; i++)
    {
      foreach (var monkey in monkeys)
      {
        Perform1Round(monkey, monkeys);
      }
    }

    var inspectCounts = monkeys.Select(m => m.GetActivity());

    var orderedCounts = inspectCounts.OrderByDescending(i => i).ToArray();

    return (orderedCounts[0] * orderedCounts[1]).ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    var monkeyList = new List<Monkey>();

    Monkey curMonkey = null;
    var commonMultiple = 1;

    foreach (var rawLine in input)
    {
      var line = rawLine.Trim();

      if (line.StartsWith("Monkey "))
      {
        curMonkey = new Monkey();
      }
      else if (line.StartsWith("Starting items:"))
      {
        curMonkey.AddItems(ParseStartingItems(line));
      }
      else if (line.StartsWith("Operation: new = "))
      {
        curMonkey.SetInspection(ParseOperation(line));
      }
      else if (line.StartsWith("Test: divisible by "))
      {
        var raw = line.Replace("Test: divisible by ", "");
        var divisor = int.Parse(raw);
        commonMultiple *= divisor;

        curMonkey.SetTestDivisor(divisor);
      }
      else if (line.StartsWith("If true: throw to monkey "))
      {
        var raw = line.Replace("If true: throw to monkey ", "");
        curMonkey.SetTrueTarget(int.Parse(raw));
      }
      else if (line.StartsWith("If false: throw to monkey "))
      {
        var raw = line.Replace("If false: throw to monkey ", "");
        curMonkey.SetFalseTarget(int.Parse(raw));
      }
      else if (string.IsNullOrWhiteSpace(line))
      {
        monkeyList.Add(curMonkey);
      }
      else
      {
        throw new ArgumentException("invalid: " + line);
      }
    }

    //there's a last monkey without corresponding whitespace
    monkeyList.Add(curMonkey);

    var monkeys = monkeyList.ToArray();

    const int totalRounds = 10000;

    for (var i = 0; i < totalRounds; i++)
    {
      foreach (var monkey in monkeys)
      {
        Perform1Round(monkey, monkeys, commonMultiple);
      }
    }

    var inspectCounts = monkeys.Select(m => m.GetActivity());

    var orderedCounts = inspectCounts.OrderByDescending(i => i).ToArray();

    return ((long)orderedCounts[0] * (long)orderedCounts[1]).ToString();
  }

  private static void Perform1Round(Monkey curMonkey, Monkey[] allMonkeys, int commonMultiple = 0)
  {
    while (curMonkey.CanInspect())
    {
      var result = curMonkey.InspectItem(commonMultiple);
      allMonkeys[result.targetMonkey].AddItem(result.returnedItemLevel);
    }
  }

  private static IEnumerable<int> ParseStartingItems(string input)
  {
    var raw = input.Replace("Starting items: ", "");
    var items = raw.Split(", ");
    return items.Select(int.Parse);
  }

  private static Func<long, long> ParseOperation(string input)
  {
    var raw = input.Replace("Operation: new = ", "");
    var parts = raw.Split(' ');
    Func<long, long> lastOperand;
    if (parts[2] == "old")
    {
      lastOperand = i => i;
    }
    else
    {
      lastOperand = i => int.Parse(parts[2]);
    }

    return parts[1] switch
    {
      "+" => i => i + lastOperand(i),
      "-" => i => i - lastOperand(i),
      "*" => i => i * lastOperand(i),
      "/" => i => i / lastOperand(i)
    };
  }

  private class Monkey
  {
    private readonly Queue<long> _items;
    private Func<long, long> _inspection;
    private int _testDivisor;
    private int _trueTarget;
    private int _falseTarget;
    private int _inspectCount;

    public Monkey()
    {
      _items = new Queue<long>();
      _inspection = i => i;
      _testDivisor = 0;
      _trueTarget = 0;
      _falseTarget = 0;
      _inspectCount = 0;
    }

    public bool CanInspect() => _items.TryPeek(out _);

    public (int targetMonkey, long returnedItemLevel) InspectItem(int commonMultiple = 0)
    {
      _inspectCount++;
      var item = _items.Dequeue();
      item = _inspection(item); //inspect
      if (commonMultiple == 0)
      {
        item /= 3; //get bored
      }
      else
      {
        item %= commonMultiple; //manage size using fancy nerd math
      }

      if (item % _testDivisor == 0)
      {
        return (_trueTarget, item);
      }
      return (_falseTarget, item);
    }

    public void AddItem(long item)
    {
      _items.Enqueue(item);
    }

    public void AddItems(IEnumerable<int> items)
    {
      foreach (var item in items)
      {
        _items.Enqueue(item);
      }
    }

    public void SetInspection(Func<long, long> ins)
    {
      _inspection = ins;
    }

    public void SetTestDivisor(int t)
    {
      _testDivisor = t;
    }

    public void SetTrueTarget(int t)
    {
      _trueTarget = t;
    }

    public void SetFalseTarget(int f)
    {
      _falseTarget = f;
    }

    public int GetActivity() => _inspectCount;
  }
}