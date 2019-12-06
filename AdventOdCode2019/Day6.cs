using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day6 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var orbits = File.ReadAllLines(inputFile);


            var dic = new List<(string, string)>();

            var nodes = orbits
                .Select(x => (x.Split(')').First(), x.Split(')').Last())).ToList();

            var orbitsCount = new Dictionary<string, int>();

            foreach(var node in nodes.Select(x => x.Item2).Distinct())
            {
                orbitsCount[node] = GetOrbitsForNode(node, nodes);
            }

            return orbitsCount.Select(x => x.Value).Sum().ToString();
        }

        private int GetOrbitsForNode(string node, List<(string, string)> nodes)
        {
            var nodeOrbit = nodes.First(x => x.Item2 == node);
            var queue = new Queue<(string, string)>();

            queue.Enqueue(nodeOrbit);
            var result = 0;
            while (queue.Any())
            {
                var element = queue.Dequeue();
                result++;

                nodes.Where(x => x.Item2 == element.Item1).ToList().ForEach(x => queue.Enqueue(x));
            }

            return result;
        }

        public string CalculatePart2(string inputFile)
        {
            var orbits = File.ReadAllLines(inputFile);


            var dic = new List<(string, string)>();

            var nodes = orbits
                .Select(x => (x.Split(')').First(), x.Split(')').Last())).ToList();

            var me = nodes.First(x => x.Item2 == "YOU");
            var sun = nodes.First(x => x.Item2 == "SAN");

            var result = GetPathToSun(me, sun, nodes);

            return result.ToString();
        }

        private int GetPathToSun(
            (string, string) me, 
            (string, string) sun, 
            List<(string, string)> nodes)
        {
            var queue = new Queue<(string Element, int Depth)> ();
            var alreadyChecked = new List<string>();

            queue.Enqueue((me.Item1, 0));

            var result = 0;
            var depthCounter = 0;
            while (queue.Any())
            {
                var element = queue.Dequeue();

                if (sun.Item1 == element.Element)
                    return element.Depth;

                nodes.Where(x => x.Item2 == element.Element)
                    .Where(x => !alreadyChecked.Contains(x.Item1))
                    .ToList()
                    .ForEach(x =>
                    {
                        queue.Enqueue((x.Item1, element.Depth + 1));
                        alreadyChecked.Add(x.Item1);
                    });

                nodes.Where(x => x.Item1 == element.Element)
                    .Where(x => !alreadyChecked.Contains(x.Item2))
                    .ToList()
                    .ForEach(x =>
                    {
                        queue.Enqueue((x.Item2, element.Depth + 1));
                        alreadyChecked.Add(x.Item2);
                    });

                depthCounter++;
            }

            return 0;
        }
    }
}