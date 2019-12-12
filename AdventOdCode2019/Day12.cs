using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day12 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var moons = GetMoons(inputFile);
            
            for (int i = 0; i < 1000; i++)
            {
                for (var index = 0; index < moons.Length; index++)
                {
                    var otherMoons = moons.Except(new[] { moons[index] });

                    foreach (var otherMoon in otherMoons)
                        moons[index] = moons[index].ApplyGravity(otherMoon);
                }

                for (var index = 0; index < moons.Length; index++)
                {
                    moons[index] = moons[index].Move();
                }
            }
            
            return moons.Sum(x => x.GetEnergy()).ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var moons = GetMoons(inputFile);

            for (int i = 0; i < 1000; i++)
            {
                foreach (var moon in moons)
                {
                    var otherMoons = moons.Except(new[] { moon });

                    foreach (var otherMoon in otherMoons)
                        moon.ApplyGravity(otherMoon);
                }

                foreach (var moon in moons)
                    moon.Move();
            }

            return moons.Sum(x => x.GetEnergy()).ToString();
        }

        private static Moon[] GetMoons(string inputFile)
        {
            var moonConfiguration = File.ReadAllLines(inputFile);
            //var programString = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var moons = moonConfiguration
                          .Select(x => x.Trim('<', '>'))
                          .Select(x => x
                                       .Split(',')
                                       .Select(y => y.Split('=').Last())
                                       .Select(int.Parse)
                                       .ToArray())
                          .Select(x => new D3Point(x[0], x[1], x[2]))
                          .Select(x => new Moon(x, D3Point.Default))
                          .ToArray();
            return moons;
        }
    }


    [DebuggerDisplay("Pos= {Position.X} {Position.Y} {Position.Z} Vel = {Velocity.X} {Velocity.Y} {Velocity.Z}")]
    internal struct Moon
    {
        public Moon(D3Point position, D3Point velocity)
            => (Position, Velocity) = (position, velocity);

        public D3Point Position { get; private set; }

        public D3Point Velocity { get; private set; }

        public Moon ApplyGravity(Moon byMoon)
        {
            return new Moon(Position, Velocity.Mutate(
                GetMutator(Position.X, byMoon.Position.X),
                GetMutator(Position.Y, byMoon.Position.Y),
                GetMutator(Position.Z, byMoon.Position.Z)));

            int GetMutator(int local, int remote)
                => local < remote 
                       ? 1 
                       : local > remote 
                           ? -1 
                           : 0;
        }

        public Moon Move() => new Moon(Position.Mutate(Velocity), Velocity);

        public int GetEnergy() => Position.GetEnergy() * Velocity.GetEnergy();
    }

    internal struct D3Point
    {
        public D3Point(int x, int y, int z) 
            => (X, Y, Z) = (x, y, z);

        public static D3Point Default => new D3Point(0, 0, 0);

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public D3Point Mutate(int xMutator, int yMutator, int zMutator) 
            => new D3Point(X + xMutator, Y + yMutator, Z + zMutator);

        public D3Point Mutate(D3Point mutator)
            => new D3Point(X + mutator.X, Y + mutator.Y, Z + mutator.Z);

        public int GetEnergy() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
    }
}