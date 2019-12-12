using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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

        //public string CalculatePart2(string inputFile)
        //{
        //    var moons = GetMoons(inputFile);

        //    var uni = new UniverseState(moons.ToArray());
        //    var hashset = new HashSet<UniverseState>();
        //    var counter = 0;

        //    var sw = new Stopwatch();
        //    sw.Start();
        //    while(true)
        //    {
        //        counter++;
        //        if (counter % 100_000 == 0)
        //        {
        //            Console.WriteLine(counter + " " + sw.ElapsedMilliseconds);
        //            sw.Reset();
        //            sw.Start();
        //        }


        //        uni.ApplyGravity();
        //        uni.MoveSystem();

        //        if (hashset.Contains(uni))
        //            return counter.ToString();

        //        hashset.Add(uni);
        //    }
        //}

        public string CalculatePart2(string inputFile)
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
            
            var hashset = new HashSet<(int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int,
                int, int, int, int)>();
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

                var tuple = GetTuple(state);
                if (hashset.Contains(tuple))
                    return (counter - 1).ToString();

                hashset.Add(tuple);
            }
        }

        private (int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int) GetTuple(int[,] state) => (state[0, 0], state[0, 1], state[0, 2], state[0, 3], state[0, 4], state[0, 5], state[1, 0], state[1, 1], state[1, 2], state[1, 3], state[1, 4], state[1, 5], state[2, 0], state[2, 1], state[2, 2], state[2, 3], state[2, 4], state[2, 5], state[3, 0], state[3, 1], state[3, 2], state[3, 3], state[3, 4], state[3, 5]);

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

    class ArrayComparer : IEqualityComparer<int[,]>
    {
        public bool Equals(int[,] x, int[,] y)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (x[i, j] != y[i, j])
                        return false;
                    
                }
            }

            return true;
        }

        public int GetHashCode(int[,] obj)
        {
            
            unchecked
            {
                var hashCode = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        hashCode = (hashCode * 397) ^ obj[i, j].GetHashCode();
                    }
                }
                return hashCode;
            }
        }
    }

    internal class UniverseState
    {
        protected bool Equals(UniverseState other)
        {
            return Equals(_moon1, other._moon1) 
                   && Equals(_moon2, other._moon2) 
                   && Equals(_moon3, other._moon3) 
                   && Equals(_moon4, other._moon4);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((UniverseState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;
                hashCode = (hashCode * 397) ^ _moon1.Position.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon1.Position.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon1.Position.Z.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon1.Velocity.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon1.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon1.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.Position.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.Position.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.Position.Z.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.Velocity.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon2.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.Position.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.Position.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.Position.Z.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.Velocity.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon3.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.Position.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.Position.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.Position.Z.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.Velocity.X.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.Velocity.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ _moon4.Velocity.Y.GetHashCode();
                return hashCode;
            }
        }

        private Moon _moon1;
        private Moon _moon2;
        private Moon _moon3;
        private Moon _moon4;

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

        public void ApplyGravity()
        {
            _moon1.ApplyGravity(_moon2);
            _moon1.ApplyGravity(_moon3);
            _moon1.ApplyGravity(_moon4);
            _moon2.ApplyGravity(_moon1);
            _moon2.ApplyGravity(_moon3);
            _moon2.ApplyGravity(_moon4);
            _moon3.ApplyGravity(_moon1);
            _moon3.ApplyGravity(_moon2);
            _moon3.ApplyGravity(_moon4);
            _moon4.ApplyGravity(_moon1);
            _moon4.ApplyGravity(_moon2);
            _moon4.ApplyGravity(_moon3);
        }

        public void MoveSystem()
        {
            _moon1.Move();
            _moon2.Move();
            _moon3.Move();
            _moon4.Move();
        }

        public int GetEnergy() 
            => _moon1.GetEnergy() 
               + _moon2.GetEnergy() 
               + _moon3.GetEnergy() 
               + _moon4.GetEnergy();
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
                return ((Position != null ? Position.GetHashCode() : 0) * 397) ^ (Velocity != null ? Velocity.GetHashCode() : 0);
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