using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day12 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var moons = GetMoons(inputFile);

            var state = new int[4, 6];

            for (var i = 0; i < moons.Length; i++)
            {
                state[i, 0] = moons[i].Position.X;
                state[i, 1] = moons[i].Position.Y;
                state[i, 2] = moons[i].Position.Z;
                state[i, 3] = moons[i].Velocity.X;
                state[i, 4] = moons[i].Velocity.Y;
                state[i, 5] = moons[i].Velocity.Z;
            }
            
            for (int p = 0; p < 1000; p++)
            {
                // gravity
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (i == j)
                            continue;

                        for (int k = 0; k < 3; k++)
                        {
                            if (state[i, k] > state[j, k])
                                state[i, 3 + k] += -1;
                            else if (state[i, k] < state[j, k])
                                state[i, 3 + k] += 1;
                        }

                    }
                }

                // move
                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        state[i, k] += state[i, 3 + k];
                    }
                }
            }

            return GetEnergy(state).ToString();
        }

        private int GetEnergy(int[,] state)
        {
            var sum = 0;
            for (int i = 0; i < 4; i++)
            {
                sum +=
                    (Math.Abs(state[i, 0])
                     + Math.Abs(state[i, 1])
                     + Math.Abs(state[i, 2]))
                    * (Math.Abs(state[i, 3])
                       + Math.Abs(state[i, 4])
                       + Math.Abs(state[i, 5]));

            }

            return sum;
        }

        public string CalculatePart2(string inputFile)
        {
            var moons = GetMoons(inputFile);
            
            var state = new int[4, 6];
            var initialState = new int[4, 6];

            for (var i = 0; i < moons.Length; i++)
            {
                state[i, 0] = moons[i].Position.X;
                state[i, 1] = moons[i].Position.Y;
                state[i, 2] = moons[i].Position.Z;
                state[i, 3] = moons[i].Velocity.X;
                state[i, 4] = moons[i].Velocity.Y;
                state[i, 5] = moons[i].Velocity.Z;
                initialState[i, 0] = moons[i].Position.X;
                initialState[i, 1] = moons[i].Position.Y;
                initialState[i, 2] = moons[i].Position.Z;
                initialState[i, 3] = moons[i].Velocity.X;
                initialState[i, 4] = moons[i].Velocity.Y;
                initialState[i, 5] = moons[i].Velocity.Z;
            }

            var counter = 0;
            var xCycle = 0;
            var yCycle = 0;
            var zCycle = 0;

            var sw = new Stopwatch();
            sw.Start();
            while(true)
            {
                counter++;
                if (counter % 1_000_000 == 0)
                {
                    Console.WriteLine(counter + " " + sw.ElapsedMilliseconds);
                    sw.Reset();
                    sw.Start();
                }

                // gravity
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (i == j)
                            continue;

                        for (int k = 0; k < 3; k++)
                        {
                            if (state[i, k] > state[j, k])
                                state[i, 3 + k] += -1;
                            else if (state[i, k] < state[j, k])
                                state[i, 3 + k] += 1;
                        }

                    }
                }

                // move
                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        state[i, k] += state[i, 3 + k];
                    }
                }

                // check
                for (int i = 0; i < 4; i++)
                {
                    if (xCycle == 0 && state[0, 3] == 0 && state[1, 3] == 0 && state[2, 3] == 0)
                        xCycle = counter * 2;
                    if (yCycle == 0 && state[0, 4] == 0 && state[1, 4] == 0 && state[2, 4] == 0)
                        yCycle = counter * 2;
                    if (zCycle == 0 && state[0, 5] == 0 && state[1, 5] == 0 && state[2, 5] == 0)
                        zCycle = counter * 2;
                }

                if (xCycle != 0 && yCycle != 0 && zCycle != 0)
                    break;
            }

            return FindLcm(xCycle, yCycle, zCycle).ToString();
        }

        private long FindLcm(params long[] values)
        {
            if (values.Length > 2)
                return FindLcm(values[0], FindLcm(values.Skip(1).ToArray()));

            return values[0] * values[1] / FindGcd(values[0], values[1]);
        }

        static long FindGcd(long a, long b)
        {
            if (b == 0)
                return a;
            return FindGcd(b, a % b);
        }

        private static Moon[] GetMoons(string inputFile)
        {
            var moonConfiguration = File.ReadAllLines(inputFile);
            //var moonConfiguration = 
            //    "<x=-1, y=0, z=2>\r\n<x=2, y=-10, z=-7>\r\n<x=4, y=-8, z=8>\r\n<x=3, y=5, z=-1>"
            //    .Split(new [] {Environment.NewLine}, StringSplitOptions.None);


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
    internal class Moon
    {
        protected bool Equals(Moon other)
        {
            return Equals(Position, other.Position) && Equals(Velocity, other.Velocity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Moon) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Position != null 
                            ? Position.GetHashCode() : 0) * 397) ^ (Velocity != null ? Velocity.GetHashCode() 
                           : 0);
            }
        }

        public Moon(D3Point position, D3Point velocity)
            => (Position, Velocity) = (position, velocity);

        public D3Point Position { get; }

        public D3Point Velocity { get; }

        public void ApplyGravity(Moon byMoon)
        {
            Velocity.Mutate(
                GetMutator(Position.X, byMoon.Position.X),
                GetMutator(Position.Y, byMoon.Position.Y),
                GetMutator(Position.Z, byMoon.Position.Z));

            int GetMutator(int local, int remote)
                => local < remote 
                       ? 1 
                       : local > remote 
                           ? -1 
                           : 0;
        }

        public void Move() => Position.Mutate(Velocity);

        public int GetEnergy() => Position.GetEnergy() * Velocity.GetEnergy();
    }

    internal class D3Point
    {
        protected bool Equals(D3Point other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((D3Point) obj);
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

        public D3Point(int x, int y, int z) 
            => (X, Y, Z) = (x, y, z);

        public static D3Point Default => new D3Point(0, 0, 0);

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Z { get; private set; }

        public void Mutate(int xMutator, int yMutator, int zMutator) 
            => (X, Y, Z) = (X + xMutator, Y + yMutator, Z + zMutator);

        public void Mutate(D3Point mutator)
            => (X, Y, Z) = (X + mutator.X, Y + mutator.Y, Z + mutator.Z);

        public int GetEnergy() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
    }
}