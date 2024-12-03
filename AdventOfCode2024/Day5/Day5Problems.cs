using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day5;

public class Day5Problems : Problems
{
  protected override string TestInput => @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";

  protected override int Day => 5;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    return ParseAndTranslateSeedMaps(input).ToString();
    throw new NotImplementedException();
  }

  private static long ParseAndTranslateSeedMaps(string[] input)
  {
    var seeds = new List<List<long>>();
    var maps = new List<TranslationMap>();
    
    //parse input and build data
    foreach (var line in input)
    {
      if (seeds.Count == 0) //first line, add seeds
      {
        var rawSeeds = StringUtils.ExtractLongsFromString(line);
        seeds = rawSeeds.Select(i => new List<long> { i }).ToList();
      }
      else if(!string.IsNullOrWhiteSpace(line)) //we're building maps, and skipping whitespace lines
      {
        var isText = RegexUtils.BasicLetterRegex.IsMatch(line);
        if (isText) //start new map
        {
          maps.Add(new TranslationMap(line));
        }
        else //append to existing
        {
          maps.Last().AddItem(line);
        }
      }
    }
    
    //process seed translations
    foreach (var seed in seeds)
    {
      foreach (var map in maps)
      {
        var intermediateResult = map.TryTranslate(seed.Last());
        seed.Add(intermediateResult);
      }
    }
    
    //find seed with lowest location
    var lowestSeedStart = long.MaxValue;
    var lowestFinalLocation = long.MaxValue;

    foreach (var seed in seeds)
    {
      var finalLoc = seed.Last();
      if (finalLoc < lowestFinalLocation)
      {
        lowestSeedStart = seed[0];
        lowestFinalLocation = finalLoc;
      }
    }
    
    return lowestFinalLocation;
  }
  
  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  private class TranslationMap
  {
    public readonly string Name;
    private List<TranslationItem> _items = new();

    public TranslationMap(string name)
    {
      Name = name;
    }

    public void AddItem(string line)
    {
      _items.Add(new TranslationItem(line));
    }

    public long TryTranslate(long i)
    {
      var result = i;
      foreach (var item in _items)
      {
        result = item.TryTranslateLong(result);
        if (result != i) return result; //stop translating once moved
      }

      return result;
    }

    public override string ToString() => Name;
  }

  private class TranslationItem
  {
    public readonly long Start;
    public readonly long End;
    public readonly long TransformByAmount;

    public TranslationItem(string rawInput)
    {
      var numbers = StringUtils.ExtractLongsFromString(rawInput).ToArray();
      Start = numbers[1];
      End = (Start + numbers[2]) - 1;
      TransformByAmount = (numbers[0] - Start);
    }

    public long TryTranslateLong(long i)
    {
      if (i >= Start && i <= End) return i + TransformByAmount;
      return i;
    }
    
    public override string ToString() => $"{Start} | {End} | {TransformByAmount}";
  }
}