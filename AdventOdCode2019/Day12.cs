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
        private readonly Moon[] _moons;

        public UniverseState(Moon[] moons)
        {
            _moons = moons;
        }

        public UniverseState ApplyGravity()
        {
            var newState = _moons.ToArray();
            for (var i = 0; i < newState.Length; i++)
            {
                for (var j = 0; j < newState.Length; j++)
                {
                    if (j == i)
                        continue;
                    newState[i] = newState[i].ApplyGravity(newState[j]);
                }
            }
            return new UniverseState(newState);
        }

        public UniverseState MoveSystem()
        {
            var newState = _moons.ToArray();
            for (var index = 0; index < newState.Length; index++)
                newState[index] = newState[index].Move();

            return new UniverseState(newState);
        }

        public int GetEnergy() => _moons.Sum(x => x.GetEnergy());

        public bool Equals(UniverseState other)
        {
            return Equals(_moons, other._moons);
        }

        public override bool Equals(object obj)
        {
            return obj is UniverseState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_moons != null ? GetHashCode(_moons) : 0);
        }

        private int GetHashCode(Moon[] moons)
        {
            unchecked
            {
                var hashCode = moons[0].GetHashCode();
                hashCode = (hashCode * 397) ^ moons[1].GetHashCode();
                hashCode = (hashCode * 397) ^ moons[2].GetHashCode();
                hashCode = (hashCode * 397) ^ moons[3].GetHashCode();
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

        public bool Equals(Moon other)
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