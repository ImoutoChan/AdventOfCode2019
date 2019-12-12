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

            var uni = new UniverseState(moons.ToArray());

            for (int i = 0; i < 1000; i++)
            {
                uni = uni.ApplyGravity();
                uni = uni.MoveSystem();
            }

            return uni.GetEnergy().ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var moons = GetMoons(inputFile);

            var uni = new UniverseState(moons.ToArray());
            var hashset = new HashSet<UniverseState>();
            var counter = 0;

            var sw = new Stopwatch();
            sw.Start();
            while(true)
            {
                counter++;
                if (counter % 100_000 == 0)
                {
                    Console.WriteLine(counter + " " + sw.ElapsedMilliseconds);
                    sw.Reset();
                    sw.Start();
                }


                uni = uni.ApplyGravity().MoveSystem();
                if (hashset.Contains(uni))
                    return counter.ToString();

                hashset.Add(uni);
            }
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

    internal struct UniverseState
    {
        private readonly Moon _moon1;
        private readonly Moon _moon2;
        private readonly Moon _moon3;
        private readonly Moon _moon4;

        public UniverseState(Moon[] moons)
        {
            _moon1 = moons[0];
            _moon2 = moons[1];
            _moon3 = moons[2];
            _moon4 = moons[3];
        }

        public UniverseState(UniverseState state)
        {
            _moon1 = state._moon1;
            _moon2 = state._moon2;
            _moon3 = state._moon3;
            _moon4 = state._moon4;
        }

        public UniverseState(Moon moon1, Moon moon2, Moon moon3, Moon moon4)
        {
            _moon1 = moon1;
            _moon2 = moon2;
            _moon3 = moon3;
            _moon4 = moon4;
        }

        public UniverseState ApplyGravity()
        {
            var moon1 = _moon1;
            var moon2 = _moon2;
            var moon3 = _moon3;
            var moon4 = _moon4;

            moon1 = moon1.ApplyGravity(moon2);
            moon1 = moon1.ApplyGravity(moon3);
            moon1 = moon1.ApplyGravity(moon4);
            moon2 = moon2.ApplyGravity(moon1);
            moon2 = moon2.ApplyGravity(moon3);
            moon2 = moon2.ApplyGravity(moon4);
            moon3 = moon3.ApplyGravity(moon1);
            moon3 = moon3.ApplyGravity(moon2);
            moon3 = moon3.ApplyGravity(moon4);
            moon4 = moon4.ApplyGravity(moon1);
            moon4 = moon4.ApplyGravity(moon2);
            moon4 = moon4.ApplyGravity(moon3);

            return new UniverseState(moon1, moon2, moon3, moon4);
        }

        public UniverseState MoveSystem() 
            => new UniverseState(_moon1.Move(), _moon2.Move(), _moon3.Move(), _moon4.Move());

        public int GetEnergy() 
            => _moon1.GetEnergy() 
               + _moon2.GetEnergy() 
               + _moon3.GetEnergy() 
               + _moon4.GetEnergy();

        public bool Equals(UniverseState other)
        {
            return _moon1.Equals(other._moon1) 
                   && _moon2.Equals(other._moon2) 
                   && _moon3.Equals(other._moon3) 
                   && _moon4.Equals(other._moon4);
        }

        public override bool Equals(object obj)
        {
            return obj is UniverseState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _moon1.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.GetHashCode();
                return hashCode;
            }
        }
    }

    [DebuggerDisplay("Pos= {Position.X} {Position.Y} {Position.Z} Vel = {Velocity.X} {Velocity.Y} {Velocity.Z}")]
    internal struct Moon
    {
        public Moon(D3Point position, D3Point velocity)
            => (Position, Velocity) = (position, velocity);

        public D3Point Position { get; }

        public D3Point Velocity { get; }

        public readonly Moon ApplyGravity(Moon byMoon)
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

        public readonly Moon Move() => new Moon(Position.Mutate(Velocity), Velocity);

        public readonly int GetEnergy() => Position.GetEnergy() * Velocity.GetEnergy();

        public readonly bool Equals(Moon other)
        {
            return Position.Equals(other.Position) && Velocity.Equals(other.Velocity);
        }

        public override bool Equals(object obj)
        {
            return obj is Moon other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ Velocity.GetHashCode();
            }
        }
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

        public bool Equals(D3Point other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is D3Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }
    }
}