using System.Text.RegularExpressions;
using AdventOfCode2024.Util;

namespace AdventOfCode2024.Day16;

public class Day16Problems : Problems
{
  private int _iterations = 0; //naive was 663056, cut to 501357 by pruning if all valves are open, cut to 103665 by eliminating the most pointless cycles
  private Dictionary<string, int> _routeCache = new Dictionary<string, int>();
  private Dictionary<int, int> _bestFlowRateAtStep = new Dictionary<int, int>();

  protected override string TestInput => @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II";

  protected override int Day => 16;

  protected override string Problem1(string[] input, bool isTestInput)
  {
    _iterations = 0;

    var allRooms = input.Select(ParseLine).ToDictionary(r => r.Name, r => r);


    var currentStep = 0;
    var maxSteps = 30;
    var currentLocation = "AA";
    var currentFlowRate = 0;
    var openValves = new HashSet<string>();
    var totalValves = allRooms.Count(r => r.Value.FlowRate > 0);

    var bestRoute = FindBestRoute(
      currentLocation, allRooms, currentStep, maxSteps, openValves, currentFlowRate, new Dictionary<string, int>(), totalValves);
    return bestRoute.ToString();
  }

  protected override string Problem2(string[] input, bool isTestInput)
  {
    _iterations = 0;
    _routeCache = new Dictionary<string, int>();

    var allRooms = input.Select(ParseLine).ToDictionary(r => r.Name, r => r);


    var currentStep = 0;
    var maxSteps = 26;
    var currentLocation = "AA";
    var currentFlowRate = 0;
    var openValves = new HashSet<string>();
    var totalValves = allRooms.Count(r => r.Value.FlowRate > 0);

    _bestFlowRateAtStep.Clear();
    for (var i = 0; i <= maxSteps; i++)
    {
      _bestFlowRateAtStep[i] = 0;
    }

    var bestRoute = FindBestRouteWithTwoMovers(
      currentLocation,  currentLocation, allRooms, currentStep, maxSteps, openValves, currentFlowRate, new Dictionary<string, int>(), totalValves);
    return bestRoute.ToString();
  }

  private static Room ParseLine(string line)
  {
    var match = LineParser.Matches(line).First();

    var valveName = match.Groups[1].Value;
    var flow = int.Parse(match.Groups[2].Value);
    var exits = match.Groups[3].Value.Split(", ");

    return new Room(valveName, exits, flow);
  }

  private static readonly Regex LineParser =
    new(
      "Valve ([\\w]+) has flow rate=([\\d]+); tunnels? leads? to valves? ([\\w, ]+)",
      RegexOptions.Compiled);

  private int FindBestRoute(string currentLocation, Dictionary<string, Room> allRooms, int currentStep,
    int maxSteps, HashSet<string> openValves, int currentFlowRate, Dictionary<string, int> lastFlowRates,
    int totalValves)
  {
    _iterations++;

    if (currentStep == maxSteps) return 0;
    var nextStep = currentStep + 1;

    //check if we're in a pointless cycle
    if (lastFlowRates.TryGetValue(currentLocation, out var lastRateInRoom))
    {
      if (lastRateInRoom == currentFlowRate) return 0;
    }

    //check if we're slacking way too hard
    if (currentStep > 10 && currentFlowRate < 40 || currentStep > 20 && currentFlowRate < 30)
    {
      return 0;
    }



    //check if we're done
    if (openValves.Count == totalValves)
    {
      var roundsRemaining = maxSteps - currentStep;
      return currentFlowRate * roundsRemaining;
    }

    var newLastFlowRates = new Dictionary<string, int>(lastFlowRates);
    newLastFlowRates[currentLocation] = currentFlowRate;

    var possibilities = new List<int> { 0 };
    var currentValve = allRooms[currentLocation];

      
    //check if we can open the current valve
    if (!openValves.Contains(currentLocation) && currentValve.FlowRate > 0)
    {
      //add that as a possibility if so
      var nextOpenValves = new HashSet<string>(openValves) { currentLocation };
      var nextFlowRate = currentFlowRate + currentValve.FlowRate;
      possibilities.Add(FindBestRoute(currentLocation, allRooms, nextStep, maxSteps, nextOpenValves, nextFlowRate, newLastFlowRates, totalValves));
    }

    //try all possible routes out -- we may want to prune this if cycles are detected lol
    foreach (var connectedValve in currentValve.AllExits())
    {
      possibilities.Add(FindBestRoute(connectedValve, allRooms, nextStep, maxSteps, openValves, currentFlowRate, newLastFlowRates, totalValves));
    }

    return currentFlowRate + possibilities.Max();
  }

  private int FindBestRouteWithTwoMovers(string currentLocationA, string currentLocationB, Dictionary<string, Room> allRooms, int currentStep,
    int maxSteps, HashSet<string> openValves, int currentFlowRate, Dictionary<string, int> lastFlowRates,
    int totalValves)
  {
    var cacheKey = BuildRouteCacheKey(currentLocationA, currentLocationB, currentFlowRate, currentStep, openValves);
    if (_routeCache.TryGetValue(cacheKey, out var cachedResult)) return cachedResult;

    _iterations++;

    if (currentStep == maxSteps) return 0;
    var nextStep = currentStep + 1;
      
    //check if we're done
    if (openValves.Count == totalValves)
    {
      var roundsRemaining = maxSteps - currentStep;
      return currentFlowRate * roundsRemaining;
    }

    //check if we're in a pointless cycle
    if (lastFlowRates.TryGetValue(currentLocationA, out var lastRateInRoomA) && lastFlowRates.TryGetValue(currentLocationB, out var lastRateInRoomB))
    {
      if (lastRateInRoomA == currentFlowRate&& lastRateInRoomB == currentFlowRate) return 0;
    }

    //check if we're slacking way too hard
    if (currentStep > 5 && currentFlowRate < 10 ||
        currentStep > 10 && currentFlowRate < 60 ||
        currentStep > 15 && currentFlowRate < 70 ||
        currentStep > 20 && currentFlowRate < 80)
    {
      return 0;
    }

    var bestFlowRateAtCurrentStep = _bestFlowRateAtStep[currentStep];
    if((currentFlowRate + 35) < bestFlowRateAtCurrentStep) //this number reached through a series of guesses
    {
      return 0;
    }

    var newLastFlowRates = new Dictionary<string, int>(lastFlowRates);
    newLastFlowRates[currentLocationA] = currentFlowRate;
    newLastFlowRates[currentLocationB] = currentFlowRate;

    var possibilities = new List<int> { 0 };

    var currentValveA = allRooms[currentLocationA];
    var currentValveB = allRooms[currentLocationB];

    //check if we're in the same room; handle valve opening differently if so
    if (currentLocationA == currentLocationB)
    {
      //check if we can open the current valve -- only A attempts
      if (!openValves.Contains(currentLocationA) && currentValveA.FlowRate > 0)
      {
        //add that + all possible B moves if so
        var nextOpenValves = new HashSet<string>(openValves) { currentLocationA };
        var nextFlowRate = currentFlowRate + currentValveA.FlowRate; 
        foreach (var connectedValveB in currentValveB.AllExits())
          possibilities.Add(FindBestRouteWithTwoMovers(currentLocationA, connectedValveB, allRooms, nextStep, maxSteps, nextOpenValves, nextFlowRate, newLastFlowRates, totalValves));
      }
    }
    else
    {
      // can they both open a valve? if so add that as a possibility
      if ((!openValves.Contains(currentLocationA) && currentValveA.FlowRate > 0) &&
          (!openValves.Contains(currentLocationB) && currentValveB.FlowRate > 0))
      {
        var nextOpenValves = new HashSet<string>(openValves) { currentLocationA, currentLocationB };
        var nextFlowRate = currentFlowRate + currentValveA.FlowRate + currentValveB.FlowRate;
        possibilities.Add(FindBestRouteWithTwoMovers(currentLocationA, currentLocationB, allRooms, nextStep, maxSteps, nextOpenValves, nextFlowRate, newLastFlowRates, totalValves));
      }

      //try opening valves and adding moves separately
      if (!openValves.Contains(currentLocationA) && currentValveA.FlowRate > 0)
      {
        var nextOpenValves = new HashSet<string>(openValves) { currentLocationA };
        var nextFlowRate = currentFlowRate + currentValveA.FlowRate;
        foreach (var connectedValveB in currentValveB.AllExits())
          possibilities.Add(FindBestRouteWithTwoMovers(currentLocationA, connectedValveB, allRooms, nextStep, maxSteps, nextOpenValves, nextFlowRate, newLastFlowRates, totalValves));
      }

      if (!openValves.Contains(currentLocationB) && currentValveB.FlowRate > 0)
      {
        var nextOpenValves = new HashSet<string>(openValves) { currentLocationB };
        var nextFlowRate = currentFlowRate + currentValveB.FlowRate;
        foreach (var connectedValveA in currentValveA.AllExits())
          possibilities.Add(FindBestRouteWithTwoMovers(connectedValveA, currentLocationB, allRooms, nextStep, maxSteps, nextOpenValves, nextFlowRate, newLastFlowRates, totalValves));
      }
    }

    //handle all possible 2-move combos
    foreach (var connectedValveA in currentValveA.AllExits())
    {
      foreach (var connectedValveB in currentValveB.AllExits())
      {
        possibilities.Add(FindBestRouteWithTwoMovers(connectedValveA, connectedValveB, allRooms, nextStep, maxSteps, openValves, currentFlowRate, newLastFlowRates, totalValves));
      }
    }

    var result = currentFlowRate + possibilities.Max();
    _routeCache[cacheKey] = result;
    if (currentFlowRate > bestFlowRateAtCurrentStep)
    {
      _bestFlowRateAtStep[currentStep] = currentFlowRate;
    }

    return result;
  }

  private string BuildRouteCacheKey(string curA, string curB, int flowRate, int curStep, HashSet<string> openValves)
  {
    return $"{curA}|{curB}|{flowRate}|{curStep}|{string.Concat(openValves)}";
  }

  private class Room
  {
    public readonly string Name;
    private readonly HashSet<string> _exitTunnels;
    public readonly int FlowRate;

    public Room(string name, IEnumerable<string> exits, int flowRate)
    {
      Name = name;
      _exitTunnels = exits.ToHashSet();
      FlowRate = flowRate;
    }

    public bool HasExit(string other)
    {
      return _exitTunnels.Contains(other);
    }

    public IEnumerable<string> AllExits() => _exitTunnels;
  }
}