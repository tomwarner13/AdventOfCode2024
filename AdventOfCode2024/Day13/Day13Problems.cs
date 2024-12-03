using AdventOfCode2024.Util;
using Newtonsoft.Json.Linq;

namespace AdventOfCode2024.Day13;

public class Day13Problems : Problems
{
  protected override string TestInput => @"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]";

  protected override int Day => 13;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    JArray packet1 = null;
    JArray packet2 = null;
    var i = 0;
    var pairIndex = 1;
    var result = 0;

    foreach (var line in input)
    {
      i = i % 3;

      switch (i)
      {
        case 0:
          packet1 = ParseLine(line);
          break;
        case 1:
          packet2 = ParseLine(line);
          break;
        case 2:
          //do packet comparisons here
          if (CheckOrdering(packet1, packet2) == Ordering.Correct)
          {
            result += pairIndex;
          }

          pairIndex++;
          break;
      }

      i++;
    }

    //for final line
    if (CheckOrdering(packet1, packet2) == Ordering.Correct)
    {
      result += pairIndex;
    }

    return result.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    const string divider1 = "[[2]]";
    const string divider2 = "[[6]]";

    var parsedLines = new List<MessageLine>
    {
      new(divider1),
      new(divider2)
    };

    foreach (var line in input)
    {
      if(!string.IsNullOrWhiteSpace(line))
        parsedLines.Add(new MessageLine(line));
    }

    parsedLines.Sort();

    var idx1 = parsedLines.FindIndex(l => l.Raw == divider1) + 1;
    var idx2 = parsedLines.FindIndex(l => l.Raw == divider2) + 1;

    return (idx1 * idx2).ToString();
  }

  private class MessageLine : IComparable<MessageLine>
  {
    public readonly string Raw;
    public readonly JToken Parsed;

    public MessageLine(string raw)
    {
      Raw = raw;
      Parsed = ParseLine(raw);
    }

    public int CompareTo(MessageLine? other)
    {
      var compareResult = CheckOrdering(Parsed, other.Parsed);

      return compareResult switch
      {
        Ordering.Correct => -1,
        Ordering.Equal => 0,
        Ordering.Wrong => 1,
      };
    }

    public override string ToString() => Raw;
  }

  private static Ordering CheckOrdering(JToken leftPacket, JToken rightPacket)
  {
    var currentResult = Ordering.Equal;
    var curIndex = 0;

    while (currentResult == Ordering.Equal)
    {
      JToken leftVal = null;
      JToken rightVal = null;

      try
      {
        leftVal = TryGetAtKey(curIndex, leftPacket);
        rightVal = TryGetAtKey(curIndex, rightPacket);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

      if (leftVal == null && rightVal == null)
      {
        //have reached end of both lists, return equal
        return Ordering.Equal;
      }

      if (leftVal == null && rightVal != null)
      {
        return Ordering.Correct;
      }

      if (leftVal != null && rightVal == null)
      {
        return Ordering.Wrong;
      }

      if (leftVal.Type == JTokenType.Integer && rightVal.Type == JTokenType.Integer)
      {
        currentResult = CheckIntOrdering(leftVal, rightVal);
      }
      else
      {
        //box ints if necessary then compare as packets
        currentResult = CheckOrdering(BoxIfNecessary(leftVal), BoxIfNecessary(rightVal));
      }

      curIndex++;
    }

    return currentResult;
  }

  private static JToken TryGetAtKey(int index, JToken obj)
  {
    var childCount = obj.Children().Count();
    return index > childCount - 1 ? null : obj[index];
  }

  private static JToken BoxIfNecessary(JToken token)
  {
    return token.Type == JTokenType.Integer ? new JArray(token) : token;
  }

  private static Ordering CheckIntOrdering(JToken leftToken, JToken rightToken)
  {
    var left = leftToken.ToObject<int>();
    var right = rightToken.ToObject<int>();

    if (left < right)
    {
      return Ordering.Correct;
    }

    if (left == right)
    {
      return Ordering.Equal;
    }

    if (left > right)
    {
      return Ordering.Wrong;
    }

    throw new ArgumentException();
  }

  private enum Ordering
  {
    Correct,
    Equal,
    Wrong
  }

  private static JArray ParseLine(string line)
  {
    return JArray.Parse(line);
  }
}