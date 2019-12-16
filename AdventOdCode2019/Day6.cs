using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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
            IReadOnlyCollection<(string, string)> nodes)
        {
            var nearPoints = nodes
                             .SelectMany(x => new[] {x.Item1, x.Item2})
                             .Distinct()
                             .ToDictionary(
                                 x => x,
                                 x => nodes.Where(y => y.Item1 == x).Select(y => y.Item2)
                                           .Union(nodes.Where(y => y.Item2 == x).Select(y => y.Item1)));

            var graph = new BreadthFirstSearch<string>(s => nearPoints[s]);

            var result = graph.GetShortestPathLength(me.Item1, s => s == sun.Item1);

            return result;
        }
    }
}