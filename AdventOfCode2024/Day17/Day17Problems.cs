using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day17;

public class Day17Problems : Problems
{
  protected override string TestInput => @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

  protected override int Day => 17;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var directions = ParseInput(input[0]);

    var chamber = new Chamber(7);

    var totalRocks = 0;
    var totalDirections = 0;

    while (totalRocks < 2022)
    {
      var newRockPosition = new GridPoint(2, chamber.TallestPoint + 4);

      var currentRock = GetNextBlock(totalRocks)(newRockPosition);

      var blockCanMove = true;

      while (blockCanMove)
      {
        var curDirection = directions[totalDirections % directions.Length];
        if (curDirection == Direction.Left)
        {
          //try move left
          var change = new GridPoint(-1, 0);
          if (chamber.CheckIfValidRockMove(currentRock, change))
          {
            currentRock.MoveLeft();
          }
        }
        else
        {
          //try move right
          var change = new GridPoint(1, 0);
          if (chamber.CheckIfValidRockMove(currentRock, change))
          {
            currentRock.MoveRight();
          }
        }

        totalDirections++;

        //attempt to move down, stop if unable
        var down = new GridPoint(0, -1);
        if (chamber.CheckIfValidRockMove(currentRock, down))
        {
          currentRock.MoveDown();
        }
        else
        {
          blockCanMove = false;
          chamber.AddRock(currentRock);
        }
      }

      totalRocks++;
    }

    return chamber.TallestPoint.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {

    var directions = ParseInput(input[0]);

    var chamber = new Chamber(7);

    var totalRocks = 0;
    var totalDirections = 0;
    var confirmedCyclePoint = 0;
    var tentativeCyclePoint = 0;
    var altitudeGainPerCycle = 0;

    while (confirmedCyclePoint == 0)
    {
      var newRockPosition = new GridPoint(2, chamber.TallestPoint + 4);

      var currentRock = GetNextBlock(totalRocks)(newRockPosition);

      var blockCanMove = true;

      while (blockCanMove)
      {
        var curDirection = directions[totalDirections % directions.Length];
        if (curDirection == Direction.Left)
        {
          //try move left
          var change = new GridPoint(-1, 0);
          if (chamber.CheckIfValidRockMove(currentRock, change))
          {
            currentRock.MoveLeft();
          }
        }
        else
        {
          //try move right
          var change = new GridPoint(1, 0);
          if (chamber.CheckIfValidRockMove(currentRock, change))
          {
            currentRock.MoveRight();
          }
        }

        //attempt to move down, stop if unable
        var down = new GridPoint(0, -1);
        if (chamber.CheckIfValidRockMove(currentRock, down))
        {
          currentRock.MoveDown();
        }
        else
        {
          blockCanMove = false;
          chamber.AddRock(currentRock);

          //check if we've found a cycle
          if (totalDirections % directions.Length == 0 && totalRocks % 5 == 0)
          {
            tentativeCyclePoint = totalRocks;
            altitudeGainPerCycle = chamber.TallestPoint;
          }

          //any of the next 10 rocks landing below altitude gain resets it
          if (tentativeCyclePoint > 0 && totalRocks - 10 < tentativeCyclePoint)
          {
            if (currentRock.GetLocation.Y < altitudeGainPerCycle)
            {
              tentativeCyclePoint = 0;
              altitudeGainPerCycle = 0;
            }
          }
          else if (tentativeCyclePoint > 0 && totalRocks - 10 >= tentativeCyclePoint)
          {
            //cycle confirmed
            confirmedCyclePoint = tentativeCyclePoint;
          }
        }
          
        totalDirections++;
      }

      totalRocks++;
    }

    //now do math to figure out how many cycles at found gain
    var totalCycles = 1000000000000 / confirmedCyclePoint;
    var cycledGains = totalCycles * altitudeGainPerCycle;
    var remainingRocks = 1000000000000 % confirmedCyclePoint;

    //reset
    totalRocks = 0;
    totalDirections = 0;
    chamber = new Chamber(7);

    while (totalRocks < remainingRocks)
    {
      var newRockPosition = new GridPoint(2, chamber.TallestPoint + 4);

      var currentRock = GetNextBlock(totalRocks)(newRockPosition);

      var blockCanMove = true;

      while (blockCanMove)
      {
        var curDirection = directions[totalDirections % directions.Length];
        if (curDirection == Direction.Left)
        {
          //try move left
          var change = new GridPoint(-1, 0);
          if (chamber.CheckIfValidRockMove(currentRock, change))
          {
            currentRock.MoveLeft();
          }
        }
        else
        {
          //try move right
          var change = new GridPoint(1, 0);
          if (chamber.CheckIfValidRockMove(currentRock, change))
          {
            currentRock.MoveRight();
          }
        }

        totalDirections++;

        //attempt to move down, stop if unable
        var down = new GridPoint(0, -1);
        if (chamber.CheckIfValidRockMove(currentRock, down))
        {
          currentRock.MoveDown();
        }
        else
        {
          blockCanMove = false;
          chamber.AddRock(currentRock);
        }
      }

      totalRocks++;
    }

    var remainingAltitudeGain = chamber.TallestPoint;
    return (cycledGains + remainingAltitudeGain).ToString();
  }

  private static Direction[] ParseInput(string line)
  {
    return line.Select(ParseChar).ToArray();
  }

  private static Direction ParseChar(char i)
  {
    return i switch
    {
      '>' => Direction.Right,
      '<' => Direction.Left
    };
  }

  private static Func<GridPoint, RockBlock> GetNextBlock(int i)
  {
    var p = i % 5;
    return p switch
    {
      0 => RockBlock.Horizontal,
      1 => RockBlock.Cross,
      2 => RockBlock.LShape,
      3 => RockBlock.Vertical,
      4 => RockBlock.Square
    };
  }

  private class RockBlock
  {
    private GridPoint _currentLocation;
    private readonly GridPoint[] _solidBits;

    private RockBlock(GridPoint currentLocation, GridPoint[] solidBits)
    {
      _currentLocation = currentLocation;
      _solidBits = solidBits;
    }

    public IEnumerable<GridPoint> GetSolidBits()
    {
      return _solidBits.Select(p => p + _currentLocation);
    }

    public GridPoint GetLocation => _currentLocation;

    public void MoveLeft()
    {
      _currentLocation = new GridPoint(_currentLocation.X - 1, _currentLocation.Y);
    }

    public void MoveRight()
    {
      _currentLocation = new GridPoint(_currentLocation.X + 1, _currentLocation.Y);
    }

    public void MoveDown()
    {
      _currentLocation = new GridPoint(_currentLocation.X, _currentLocation.Y - 1);
    }

    public static RockBlock Horizontal(GridPoint startLocation)
    {
      var solidBits = new[]
      {
        new GridPoint(0, 0),
        new GridPoint(1, 0),
        new GridPoint(2, 0),
        new GridPoint(3, 0)
      };

      return new RockBlock(startLocation, solidBits);
    }

    public static RockBlock Cross(GridPoint startLocation)
    {
      var solidBits = new[]
      {
        new GridPoint(1, 0),
        new GridPoint(0, 1),
        new GridPoint(1, 1),
        new GridPoint(2, 1),
        new GridPoint(1, 2)
      };

      return new RockBlock(startLocation, solidBits);
    }

    public static RockBlock LShape(GridPoint startLocation)
    {
      var solidBits = new[]
      {
        new GridPoint(0, 0),
        new GridPoint(1, 0),
        new GridPoint(2, 0),
        new GridPoint(2, 1),
        new GridPoint(2, 2)
      };

      return new RockBlock(startLocation, solidBits);
    }

    public static RockBlock Vertical(GridPoint startLocation)
    {
      var solidBits = new[]
      {
        new GridPoint(0, 0),
        new GridPoint(0, 1),
        new GridPoint(0, 2),
        new GridPoint(0, 3)
      };

      return new RockBlock(startLocation, solidBits);
    }

    public static RockBlock Square(GridPoint startLocation)
    {
      var solidBits = new[]
      {
        new GridPoint(0, 0),
        new GridPoint(1, 0),
        new GridPoint(0, 1),
        new GridPoint(1, 1)
      };

      return new RockBlock(startLocation, solidBits);
    }
  }

  private class Chamber
  {
    private readonly HashSet<GridPoint> _occupiedSpaces;
    private readonly int _width;
    public int TallestPoint { get; private set; }

    public Chamber(int width)
    {
      _width = width;
      _occupiedSpaces = new HashSet<GridPoint>();
      TallestPoint = 0;
    }

    public bool CheckSpace(GridPoint point)
    {
      if (point.X < 0 || point.X >= _width || point.Y <= 0)
        return false;
      return !_occupiedSpaces.Contains(point);
    }

    public bool CheckIfValidRockMove(RockBlock rock, GridPoint change)
    {
      return rock.GetSolidBits().Select(p => p + change).All(CheckSpace);
    }

    public void AddRock(RockBlock rock)
    {
      foreach (var solidBit in rock.GetSolidBits())
      {
        _occupiedSpaces.Add(solidBit);
        if (solidBit.Y > TallestPoint)
          TallestPoint = solidBit.Y;
      }
    }
  }

  private enum Direction
  {
    Left,
    Right
  }
}