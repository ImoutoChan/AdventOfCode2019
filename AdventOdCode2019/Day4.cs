using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOdCode2019
{
    internal class Day4 : IAdventOfCodeDay
    {
        public string CalculatePart1(string _)
        {
            var range = 367479..893698;
            var resultCounter = 0;
            for (var i = range.Start.Value; i <= range.End.Value; i++)
            {
                var digits = i.ToString();

                if (IsDigitsNotDecreasing(digits.ToCharArray()) 
                    && HasDigitGroupAtLeastByTwo(digits.ToCharArray()))
                    resultCounter++;
            }

            return resultCounter.ToString();
        }

        public string CalculatePart2(string _)
        {
            var range = 367479..893698;
            var resultCounter = 0;
            for (var i = range.Start.Value; i <= range.End.Value; i++)
            {
                var digits = i.ToString();

                if (IsDigitsNotDecreasing(digits.ToCharArray()) 
                    && HasDigitGroupExactByTwo(digits.ToCharArray()))
                    resultCounter++;
            }

            return resultCounter.ToString();
        }

        private bool HasDigitGroupAtLeastByTwo(IReadOnlyList<char> digits)
        {
            var lastDigit = '-';
            for (var i = 0; i < digits.Count; i++)
            {
                var currentDigit = digits[i];
                if (currentDigit == lastDigit)
                    return true;

                lastDigit = currentDigit;
            }

            return false;
        }

        private bool HasDigitGroupExactByTwo(IReadOnlyList<char> digits)
        {
            var lastDigit = '-';
            var lastCounter = 0;
            for (var i = 0; i < digits.Count; i++)
            {
                var currentDigit = digits[i];
                if (currentDigit == lastDigit)
                {
                    lastCounter++;
                }
                else
                {
                    lastDigit = currentDigit;

                    if (lastCounter == 1)
                        return true;
                    lastCounter = 0;
                }
            }

            if (lastCounter == 1)
                return true;
            
            return false;
        }

        private bool IsDigitsNotDecreasing(IReadOnlyList<char> digits)
        {
            for (var i = 0; i < digits.Count - 1; i++)
                if (digits[i] > digits[i + 1])
                    return false;

            return true;
        }
    }
}