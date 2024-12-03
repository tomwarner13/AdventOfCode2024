using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day7;

public class Day7Problems : Problems
{
  protected override string TestInput => @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483";


  protected override int Day => 7;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    return DoHandCalculations(input, false).ToString();
  }

  private static int DoHandCalculations(string[] input, bool hasJokers)
  {
    var hands = new List<PokerHand>();

    foreach (var line in input)
    {
      var parts = line.Split(' ');
      var bid = int.Parse(parts[1]);
      hands.Add(new PokerHand(parts[0], bid, hasJokers));
    }
    
    hands.Sort();

    var sum = 0;
    for (var i = 1; i <= hands.Count; i++)
    {
      sum += (i * hands[i - 1].BidAmount);
    }

    return sum;
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    return DoHandCalculations(input, true).ToString();
  }

  private class PokerHand : IComparable<PokerHand>
  {
    public readonly int[] Cards;

    public readonly HandType Type;

    public readonly int BidAmount;

    public PokerHand(string rawLine, int bidAmount, bool hasJokers = false) //line will be 5 chars here, so like 'AKKT5'
    {
      if (hasJokers)
      {
        Cards = rawLine.Select(CharToNumWithJokers).ToArray();

        Type = DetermineTypeWithJokers(Cards);
      }
      else
      {
        Cards = rawLine.Select(CharToNum).ToArray();

        Type = DetermineType(Cards);
      }

      BidAmount = bidAmount;
    }
    
    public int CompareTo(PokerHand? other)
    {
      if (other == null) return 1;

      if (Type != other.Type)
      {
        return Type > other.Type ? 1 : -1;
      }

      //compare by highest card (in order)
      for (var i = 0; i < Cards.Length; i++)
      {
        var thisCard = Cards[i];
        var otherCard = other.Cards[i];
          
        if (thisCard > otherCard) return 1;
        if (thisCard < otherCard) return -1;
      }
      
      //apparently whatever .NET uses for the default sort algo sometimes compares items to themselves!
      return 0;
    }

    private static int CharToNum(char c)
    {
      return c switch
      {
        'T' => 10,
        'J' => 11,
        'Q' => 12,
        'K' => 13,
        'A' => 14,
        _ => int.Parse(c.ToString())
      };
    }
    
    private static int CharToNumWithJokers(char c)
    {
      return c switch
      {
        'T' => 10,
        'J' => 1,
        'Q' => 12,
        'K' => 13,
        'A' => 14,
        _ => int.Parse(c.ToString())
      };
    }

    private static HandType DetermineType(int[] cards)
    {
      var uniqueCards = cards.ToHashSet();
      var uniqueCount = uniqueCards.Count;

      switch (uniqueCount)
      {
        case 1:
          return HandType.Fives;
        
        case 2: //fours or FH
          var oneSetCount = cards.Count(c => c == cards[0]);
          if (oneSetCount == 4 || oneSetCount == 1) return HandType.Fours;
          return HandType.FullHouse;
        
        case 3: //2 pair or 3 of a kind
          foreach (var testCard in uniqueCards)
          {
            if (cards.Count(c => c == testCard) == 3) return HandType.Threes;
          }
          return HandType.TwoPair;
        
        case 4:
          return HandType.OnePair;
        
        case 5:
          return HandType.HighCard;
        
        default:
          throw new ThisShouldNeverHappenException("something terribly wrong");
      }
    }
    
    
    private static HandType DetermineTypeWithJokers(int[] cards)
    {
      //find most common non-joker card, replace all jokers with it
      var uniqueCards = cards.Where(c => c!= 1).ToHashSet();
      
      if (uniqueCards.Count == 0) return HandType.Fives; //all jokers, ha ha ha ha ha ha ha ha
      if (uniqueCards.Count == 5) return HandType.HighCard; //no jokers, no fun

      var mostCommonCardDetails =
        uniqueCards.Select(c => (c, cards.Count(card => card == c)))
          .MaxBy(d => d.Item2);

      var commonCardNum = mostCommonCardDetails.c;

      var replacedArray = cards
        .Select(c => (c == 1 ? commonCardNum : c))
        .ToArray();

      return DetermineType(replacedArray);
    }
  }

  private enum HandType
  {
    HighCard = 0,
    OnePair = 1,
    TwoPair = 2,
    Threes = 3,
    FullHouse = 4,
    Fours = 5,
    Fives = 6
  }
}