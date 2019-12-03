using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day1Part1 : IAdventOfCode
    {
        public string Calculate(string inputFile)
        {
            var masses = File.ReadAllLines("input/day1part1.txt");
            return masses.Select(int.Parse).Select(x => x / 3 - 2).Sum().ToString();
        }
    }
}