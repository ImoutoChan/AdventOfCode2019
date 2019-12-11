using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOdCode2019
{
    internal class Day9 : IAdventOfCodeDay
    {
        public string CalculatePart1(string inputFile)
        {
            var program = GetProgram(inputFile);

            var sb = new StringBuilder();
            long? result = 0;
            var runner = new IntCodeRunner9(program);
            while (result.HasValue)
            {
                result = runner.Run(1);
                sb.AppendLine(result.ToString());
            }

            return sb.ToString();
        }

        public string CalculatePart2(string inputFile)
        {
            var program = GetProgram(inputFile);

            var sb = new StringBuilder();
            long? result = 0;
            var runner = new IntCodeRunner9(program);
            while (result.HasValue)
            {
                result = runner.Run(2);
                sb.AppendLine(result.ToString());
            }

            return sb.ToString();
        }

        private static long[] GetProgram(string inputFile)
        {
            var programString = File.ReadAllLines(inputFile).First();
            //var programString = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var program = programString.Split(',').Select(long.Parse).ToArray();
            return program;
        }
    }



    public class IntCodeRunner9
    {
        private readonly IDictionary<long, long> _program;
        private long i = 0;
        private long _relativeBase = 0;


        public IntCodeRunner9(long[] program)
        {
            _program = new DefaultableDictionary<long, long>(
                program
                    .Select((x, i) => (x, i))
                    .ToDictionary(x => (long)x.i, x => (long)x.x), 
                0);
        }

        public long? Run(long input)
        {
            var program = _program;
            var inputUsed = false;

            while (i < _program.Count)
            {
                var opCode = program[i] % 100;
                var modeMem1 = program[i] / 100 % 10;
                var modeMem2 = program[i] / 1000 % 10;
                var modeMem3 = program[i] / 10000 % 10;
                long result = 0;

                if (opCode == 99)
                    return null;

                switch (opCode)
                {
                    case 1:
                        result = GetOp(modeMem1, i + 1) + GetOp(modeMem2, i + 2);
                        WriteOp(modeMem3, i + 3, result);
                        i += 4;
                        break;
                    case 2:
                        result = GetOp(modeMem1, i + 1) * GetOp(modeMem2, i + 2);
                        WriteOp(modeMem3, i + 3, result);
                        i += 4;
                        break;
                    case 3:
                        if (inputUsed)
                            return null;

                        WriteOp(modeMem1, i + 1, input);
                        inputUsed = true;
                        i += 2;
                        break;
                    case 4:
                        var res = GetOp(modeMem1, i + 1);
                        i += 2;
                        return res;
                    case 5:
                        i = GetOp(modeMem1, i + 1) != 0
                            ? GetOp(modeMem2, i + 2) 
                            : i + 3;
                        break;
                    case 6:
                        i = GetOp(modeMem1, i + 1) == 0
                            ? GetOp(modeMem2, i + 2) : i + 3;
                        break;
                    case 7:
                        result
                            = GetOp(modeMem1, i + 1) < GetOp(modeMem2, i + 2) ? 1 : 0;
                        WriteOp(modeMem3, i + 3, result);
                        i += 4;
                        break;
                    case 8:
                        result 
                            = GetOp(modeMem1, i + 1) == GetOp(modeMem2, i + 2) ? 1 : 0;
                        WriteOp(modeMem3, i + 3, result);
                        i += 4;
                        break;
                    case 9:
                        _relativeBase += GetOp(modeMem1, i + 1);
                        i += 2;
                        break;
                }
            }

            return null;

            long GetOp(long modeMem, long position)
            {
                switch (modeMem)
                {
                    case 0:
                        return program[program[position]];
                    case 1:
                        return program[position];
                    case 2:
                        return program[_relativeBase + program[position]];
                    default:
                        throw new NotImplementedException();
                }
            }

            void WriteOp(long modeMem, long position, long input)
            {
                switch (modeMem)
                {
                    case 0:
                        program[program[position]] = input;
                        break;
                    case 2:
                        program[_relativeBase + program[position]] = input;
                        break;
                    case 1:
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public class DefaultableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> dictionary;
            private readonly TValue defaultValue;

            public DefaultableDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
            {
                this.dictionary = dictionary;
                this.defaultValue = defaultValue;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                dictionary.Add(item);
            }

            public void Clear()
            {
                dictionary.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return dictionary.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                dictionary.CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return dictionary.Remove(item);
            }

            public int Count
            {
                get { return dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return dictionary.IsReadOnly; }
            }

            public bool ContainsKey(TKey key)
            {
                return dictionary.ContainsKey(key);
            }

            public void Add(TKey key, TValue value)
            {
                dictionary.Add(key, value);
            }

            public bool Remove(TKey key)
            {
                return dictionary.Remove(key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                if (!dictionary.TryGetValue(key, out value))
                {
                    value = defaultValue;
                }

                return true;
            }

            public TValue this[TKey key]
            {
                get
                {
                    TValue value; 
                    TryGetValue(key, out value); 
                    return value;
                }

                set { dictionary[key] = value; }
            }

            public ICollection<TKey> Keys
            {
                get { return dictionary.Keys; }
            }

            public ICollection<TValue> Values
            {
                get
                {
                    var values = new List<TValue>(dictionary.Values) {
                    defaultValue
                };
                    return values;
                }
            }
        }
    }
}