using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day15;

public class Day15Problems : Problems
{
  protected override string TestInput => @"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^";

  protected override int Day => 15;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var robotPos = new GridPoint(0, 0);
    var walls = new HashSet<GridPoint>();
    var boxes = new HashSet<GridPoint>();

    var parsingMap = true;
    var y = 0;

    while (parsingMap)
    {
      var line = input[y];
      if (!string.IsNullOrWhiteSpace(line))
      {
        for (var x = 0; x < line.Length; x++)
        {
          switch (line[x])
          {
            case '#': walls.Add(new GridPoint(x, y)); break;
            case 'O': boxes.Add(new GridPoint(x, y)); break;
            case '@': robotPos = new GridPoint(x, y); break;
          }
        }
      }
      else
      {
        parsingMap = false;
      }
      
      y++;
    }

    while (y < input.Length)
    {
      var line = input[y];
      foreach (var c in line)
      {
        var dir = c switch
        {
          '^' => GridPoint.Up,
          'v' => GridPoint.Down,
          '>' => GridPoint.Right,
          '<' => GridPoint.Left
        };
        AttemptOneMove(ref robotPos, ref walls, ref boxes, dir);
      }

      y++;
    }

    var total = 0;
    foreach (var box in boxes)
    {
      total += box.X + (box.Y * 100);
    }
    
    return total.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  private static void AttemptOneMove(ref GridPoint robot, ref HashSet<GridPoint> walls, ref HashSet<GridPoint> boxes,
    GridPoint direction)
  {
    var attemptedPosition = robot + direction;

    if (walls.Contains(attemptedPosition))
    {
      //no-op
    }
    else if (boxes.Contains(attemptedPosition))
    {
      //try moving boxes
      var attemptedBoxMove = attemptedPosition + direction;
      while (boxes.Contains(attemptedBoxMove))
        attemptedBoxMove += direction;

      if (!walls.Contains(attemptedBoxMove))
      {
        boxes.Remove(attemptedPosition);
        boxes.Add(attemptedBoxMove);
        robot = attemptedPosition;
      }
    }
    else
    {
      robot = attemptedPosition;
    }
  }
}