using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOdCode2019
{
    internal class Day14 : IAdventOfCodeDay
    {
        private static Dictionary<string, int> _excesses = new Dictionary<string, int>();

        public string CalculatePart1(string inputFile)
        {            
            var reactions = GetReactions(inputFile).ToList();

            var fuelReaction = reactions.Single(x => x.Result.Key == "FUEL");

            var result = GetCost(1, fuelReaction, reactions);

            return result.ToString();
        }

        private static int GetCost(int elementCount, Reaction reaction, IReadOnlyCollection<Reaction> reactions)
        {
            var reqs = reaction.Requirements;
            var resultCost = 0;


            if (_excesses.TryGetValue(reaction.Result.Key, out var excessCount))
            {
                elementCount -= excessCount;

                if (elementCount < 0)
                {
                    _excesses[reaction.Result.Key] = Math.Abs(elementCount);
                    return 0;
                }

                _excesses[reaction.Result.Key] = 0;
            }

            var reactionsCount = elementCount / reaction.Result.Count;
            var excess = elementCount % reaction.Result.Count;
            if (excess != 0)
            {
                reactionsCount++;

                _excesses.TryGetValue(reaction.Result.Key, out var current);
                _excesses[reaction.Result.Key] = current + (reaction.Result.Count - excess);
            }

            foreach (var requirement in reqs)
            {
                if (requirement.Key == "ORE")
                    resultCost += requirement.Count * reactionsCount;
                else
                {
                    var reqReaction = reactions.Single(x => x.Result.Key == requirement.Key);
                    resultCost += GetCost(requirement.Count * reactionsCount, reqReaction, reactions);
                }
            }

            return resultCost;
        }

        public string CalculatePart2(string inputFile)
        {
            var reactions = GetReactions(inputFile).ToList();
            _excesses = new Dictionary<string, int>();

            var fuelReaction = reactions.Single(x => x.Result.Key == "FUEL");

            var oreTotal = 1000000000000;
            var fuels = 0;

            var sw = new Stopwatch();
            sw.Start();
            do
            {
                var cost = GetCost(1, fuelReaction, reactions);
                oreTotal -= cost;
                fuels++;

                if (fuels % 10000 == 0)
                {
                    Console.WriteLine(oreTotal + " " + sw.ElapsedMilliseconds);
                    sw.Reset();
                    sw.Start();
                }

            } while (oreTotal > 0);

            if (oreTotal < 0)
                fuels--;

            return fuels.ToString();
        }

        private static IEnumerable<Reaction> GetReactions(string inputFile)
        {
            var allLines = File.ReadAllLines(inputFile);

            //            var allLines = @"157 ORE => 5 NZVS
            //165 ORE => 6 DCFZ
            //44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
            //12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
            //179 ORE => 7 PSHF
            //177 ORE => 5 HKGWZ
            //7 DCFZ, 7 PSHF => 2 XJWVT
            //165 ORE => 2 GPVTF
            //3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT".Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in allLines)
            {
                var split = line.Split(new[] {"=>"}, StringSplitOptions.RemoveEmptyEntries);
                var key = GetElement(split.Last());
                var values = split.First().Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
                yield return new Reaction(key, values.Select(GetElement).ToList());
            }
        }

        private static ReactionElement GetElement(string str)
        {
            var split = str.Trim().Split(' ');
            return new ReactionElement(int.Parse(split.First()), split.Last());
        }
    }

    class Reaction
    {
        public Reaction(ReactionElement result, IReadOnlyCollection<ReactionElement> requirements)
        {
            Result = result;
            Requirements = requirements;
        }

        public ReactionElement Result { get; }

        public IReadOnlyCollection<ReactionElement> Requirements { get; }
    }

    [DebuggerDisplay("{Key} {Count}")]
    class ReactionElement
    {
        public ReactionElement(int count, string key)
        {
            Count = count;
            Key = key;
        }

        public int Count { get; }

        public string Key { get; }
    }
}