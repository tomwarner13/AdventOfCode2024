using System.Runtime.CompilerServices;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day17;

public class Day17Problems : Problems
{
  protected override string TestInput => @"Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0";

  protected override int Day => 17;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    var registerA = StringUtils.ExtractIntsFromString(input[0]).First();
    var registerB = StringUtils.ExtractIntsFromString(input[1]).First();
    var registerC = StringUtils.ExtractIntsFromString(input[2]).First();
    
    var instructions = StringUtils.ExtractIntsFromString(input[4]).ToArray();
    
    var compy = new ChronospatialComputer(registerA, registerB, registerC, instructions);
    var output = compy.Operate();
    return string.Join(',', output.Select(n => n.ToString()));
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    throw new NotImplementedException();
  }

  private class ChronospatialComputer
  {
    public int RegisterA { get; set; }
    public int RegisterB { get; set; }
    public int RegisterC { get; set; }

    private int[] _input;
    private int _pointer;
    private List<int> _output = new();

    public ChronospatialComputer(int a, int b, int c, int[] input)
    {
      RegisterA = a;
      RegisterB = b;
      RegisterC = c;
      _input = input;
      _pointer = 0;
    }

    public List<int> Operate()
    {
      while (_pointer < _input.Length)
      {     
        var opcode = _input[_pointer];
        var rawOperand = _input[_pointer + 1]; //could this throw? probably lol
        Iterate(opcode, rawOperand);
      }
      
      //halt when trying to read opcode past end of program
      return _output;
    }

    private void Iterate(int opcode, int rawOperand)
    {
      switch (opcode)
      {
        case 0:
          Adv(rawOperand);
          break;
        case 1:
          Bxl(rawOperand);
          break;
        case 2:
          Bst(rawOperand);
          break;
        case 3:
          Jnz(rawOperand);
          break;
        case 4:
          Bxc(rawOperand);
          break;
        case 5:
          Out(rawOperand);
          break;
        case 6:
          Bdv(rawOperand);
          break;
        case 7:
          Cdv(rawOperand);
          break;
        default:
          throw new ThisShouldNeverHappenException($"opcode out of range {opcode}");
      }
    }

    private void Adv(int rawOperand)
    {
      RegisterA = (int)(RegisterA / Math.Pow(2, ComboOperand(rawOperand)));
      _pointer += 2;
    }
    
    private void Bxl(int rawOperand)
    {
      RegisterB ^= rawOperand;
      _pointer += 2;
    }
    
    private void Bst(int rawOperand)
    {
      RegisterB = ComboOperand(rawOperand) % 8;
      _pointer += 2;
    }
    
    private void Jnz(int rawOperand)
    {
      if (RegisterA == 0)
      {
        _pointer += 2;
        return;
      }

      _pointer = rawOperand;
    }
    
    private void Bxc(int rawOperand)
    {
      RegisterB ^= RegisterC;
      _pointer += 2;
    }
    
    private void Out(int rawOperand)
    {
      _output.Add(ComboOperand(rawOperand) % 8);
      _pointer += 2;
    }
    
    private void Bdv(int rawOperand)
    {
      RegisterB = (int)(RegisterA / Math.Pow(2, ComboOperand(rawOperand)));
      _pointer += 2;
    }
    
    private void Cdv(int rawOperand)
    {
      RegisterC = (int)(RegisterA / Math.Pow(2, ComboOperand(rawOperand)));
      _pointer += 2;
    }

    private int ComboOperand(int rawOperand)
    {
      if (rawOperand is < 0 or > 6)
        throw new ThisShouldNeverHappenException($"operand out of range: {rawOperand}");
      
      if (rawOperand <= 3)
        return rawOperand;

      if (rawOperand == 4)
        return RegisterA;
      
      if (rawOperand == 5)
        return RegisterB;
      
      if (rawOperand == 6)
        return RegisterC;
      
      throw new ThisShouldNeverHappenException($"operand impossibly out of range: {rawOperand}");
    }
  }
}