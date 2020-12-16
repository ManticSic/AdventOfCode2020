using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day5
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            SeatFactory factory = new SeatFactory();
            ImmutableSortedSet<int> seatIds = File.ReadAllLines("./input.txt")
                                                  .Select(factory.From)
                                                  .Select(seat => seat.Id)
                                                  .ToImmutableSortedSet();

            int maxId = seatIds.Max();

            Console.WriteLine($"The highest seat id is {maxId}.");

            int searchedId = Enumerable
                             .Range(seatIds.Min(), seatIds.Count + 1)
                             .SingleOrDefault(id => !seatIds.Contains(id));

            Console.WriteLine($"My seat id is {searchedId}.");
        }
    }

    internal class SeatFactory
    {
        private static readonly Regex BinarySpacePartitionRegex = new Regex(@"^(?<Row>[FB]{7})(?<Column>[RL]{3})$");

        private static readonly Dictionary<char, char> BinaryMap = new Dictionary<char, char>
                                                                   {
                                                                       {'F', '0'},
                                                                       {'B', '1'},
                                                                       {'L', '0'},
                                                                       {'R', '1'}
                                                                   };

        public Seat From(string input)
        {
            Match match = BinarySpacePartitionRegex.Match(input);

            if (!match.Success)
            {
                throw new FormatException("Input does not match the expected pattern!");
            }

            string rowBinary    = TransformToBinary(match.Groups["Row"].Value);
            string columnBinary = TransformToBinary(match.Groups["Column"].Value);

            int row    = Convert.ToInt32(rowBinary, 2);
            int column = Convert.ToInt32(columnBinary, 2);

            return new Seat(row, column);
        }

        private string TransformToBinary(string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char @char in input)
            {
                builder.Append(BinaryMap[@char]);
            }

            return builder.ToString();
        }
    }

    internal class Seat
    {
        public Seat(int row, int column)
        {
            Row    = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public int Id => Row * 8 + Column;
    }
}
