using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            IList<Password> passwordsPart1 = Parser.Parse<PasswordPart1>(@"./input.txt").ToList();

            int validPasswordPart1Count = passwordsPart1.Count(password => password.IsValid());

            Console.WriteLine($"Part1: Parsed all entries and found {validPasswordPart1Count} valid passwords.");

            IList<Password> passwordsPart2 = Parser.Parse<PasswordPart2>(@"./input.txt").ToList();

            int validPasswordPart2Count = passwordsPart2.Count(password => password.IsValid());

            Console.WriteLine($"Part2: Parsed all entries and found {validPasswordPart2Count} valid passwords.");
        }
    }

    internal abstract class Password
    {
        protected Password(string value, PasswordPolicySchema policySchema)
        {
            Value        = value;
            PolicySchema = policySchema;
        }

        public string Value { get; }

        public PasswordPolicySchema PolicySchema { get; }

        public abstract bool IsValid();
    }

    internal class PasswordPart1 : Password
    {
        public PasswordPart1(string value, PasswordPolicySchema policySchema) : base(value, policySchema)
        {
        }

        public override bool IsValid()
        {
            int charCount = Value.Count(valueChar => valueChar == PolicySchema.Character);

            return charCount >= PolicySchema.OccurrenceLower && charCount <= PolicySchema.OccurrenceHigher;
        }
    }

    internal class PasswordPart2 : Password
    {
        public PasswordPart2(string value, PasswordPolicySchema policySchema) : base(value, policySchema)
        {
        }

        public override bool IsValid()
        {
            int occurences = 0;

            if (Value.Length >= PolicySchema.OccurrenceLower && Value[PolicySchema.OccurrenceLower - 1] == PolicySchema.Character)
            {
                occurences++;
            }

            if (Value.Length >= PolicySchema.OccurrenceHigher &&
                Value[PolicySchema.OccurrenceHigher - 1] == PolicySchema.Character)
            {
                occurences++;
            }

            return occurences == 1;
        }
    }

    internal class PasswordPolicySchema
    {
        public PasswordPolicySchema(char character, int occurrenceLower, int occurrenceHigher)
        {
            Character        = character;
            OccurrenceLower  = occurrenceLower;
            OccurrenceHigher = occurrenceHigher;
        }

        public int OccurrenceLower { get; }

        public int OccurrenceHigher { get; }

        public char Character { get; }
    }

    internal static class Parser
    {
        public static IEnumerable<Password> Parse<TPassword>(string path) where TPassword : Password
        {
            using StreamReader reader = new StreamReader(path);
            string             line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return ParsePassword<TPassword>(line);
            }
        }

        private static Password ParsePassword<TPassword>(string line) where TPassword : Password
        {
            string[] parts = line.Split(' ');

            string occurrences        = parts[0];
            string characterWithColon = parts[1];
            string password           = parts[2];

            PasswordPolicySchema policySchema = ParsePasswordPolicy(occurrences, characterWithColon);

            return (Password) Activator.CreateInstance(typeof(TPassword), password, policySchema);
        }

        private static PasswordPolicySchema ParsePasswordPolicy(string occurrences, string characterWithColon)
        {
            string[] occurrencesAsStrings = occurrences.Split('-');

            int minOccurrence = Int32.Parse(occurrencesAsStrings[0]);
            int maxOccurrence = Int32.Parse(occurrencesAsStrings[1]);

            char character = characterWithColon.First();

            return new PasswordPolicySchema(character, minOccurrence, maxOccurrence);
        }
    }
}
