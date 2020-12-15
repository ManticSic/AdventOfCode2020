using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IList<int> entries = Parser.Parse(@"./input.txt").ToList();

            Result part1 = (
                               from entry1 in entries
                               from entry2 in entries
                               select new Result(entry1, entry2)
                           )
                .First(item => item.Sum == 2020);

            Console.WriteLine($"The result of part 1 is: {part1.Product}");

            Result part2 = (
                               from entry1 in entries
                               from entry2 in entries
                               from entry3 in entries
                               select new Result(entry1, entry2, entry3)
                           )
                .First(item => item.Sum == 2020);

            Console.WriteLine($"The result of part 2 is: {part2.Product}");
        }
    }

    internal class Result
    {
        public Result(params int[] items)
        {
            Sum = items.Sum();

            Product = items.Aggregate((item1, item2) => item1 * item2);
        }

        public int Sum { get; }

        public int Product { get; }
    }

    internal static class Parser
    {
        public static IEnumerable<int> Parse(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string             line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return Int32.Parse(line);
            }
        }
    }
}
