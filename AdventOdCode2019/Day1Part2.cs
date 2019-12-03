using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day1Part2 : IAdventOfCode
    {
        public string Calculate(string inputFile)
        {
            var masses = File.ReadAllLines("input/day1part1.txt").Select(int.Parse);
            var result = masses.Select(GetFuelForMassRecursive).Sum();

            return result.ToString();

            int GetFuelForMassRecursive(int x)
            {
                var totalFuel = 0;
                var currentFuel = GetFuelForMass(x);
                
                while (currentFuel > 0)
                {
                    totalFuel += currentFuel;
                    currentFuel = GetFuelForMass(currentFuel);
                }

                return totalFuel;
            }
            
            int GetFuelForMass(int x) => x / 3 - 2;
        }
    }
}