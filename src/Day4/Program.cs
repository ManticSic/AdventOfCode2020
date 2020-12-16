using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Day4
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            IEnumerable<IEnumerable<KeyValuePair<string, string>>> passportsAsKeyValuePairs = Parser.Parse(@"./input.txt");

            PassportValidation simplePassportValidation = new PassportValidation();
            PassportFactory    passportFactoryPart1     = new PassportFactory(simplePassportValidation);
            IList<Passport>    passportsPart1           = GetPassports(passportsAsKeyValuePairs, passportFactoryPart1);

            Console.WriteLine($"Able to parse {passportsPart1.Count} passports without validation.");

            PassportValidation advancedPassportValidation = new AdvancedPassportValidation();
            PassportFactory    passportFactoryPart2       = new PassportFactory(advancedPassportValidation);
            IList<Passport>    passportsPart2             = GetPassports(passportsAsKeyValuePairs, passportFactoryPart2);

            Console.WriteLine($"Able to parse {passportsPart2.Count} passports with validation.");
        }

        private static IList<Passport> GetPassports(
            IEnumerable<IEnumerable<KeyValuePair<string, string>>> passportsAsKeyValuePairs,
            PassportFactory                                        passportFactory)
        {
            IList<Passport> passports = new List<Passport>();
            foreach (IEnumerable<KeyValuePair<string, string>> passportAsKeyValuePairs in passportsAsKeyValuePairs)
            {
                try
                {
                    passports.Add(passportFactory.From(passportAsKeyValuePairs));
                }
                catch (FormatException exception)
                {
                    Console.WriteLine(exception.Message);
                    // ignore exception
                }
            }

            return passports;
        }
    }

    internal class PassportValidation
    {
        public IEnumerable<PropertyMap> GetPropertyMaps()
        {
            return new[]
                   {
                       new PropertyMap("byr", nameof(Passport.BirthYear), Existence.Requiered, ValidateBirthYear),
                       new PropertyMap("iyr", nameof(Passport.IssueYear), Existence.Requiered, ValidateIssueYear),
                       new PropertyMap("eyr", nameof(Passport.ExpirationYear), Existence.Requiered, ValidateExpirationYear),
                       new PropertyMap("hgt", nameof(Passport.Height), Existence.Requiered, ValidateHeight),
                       new PropertyMap("hcl", nameof(Passport.HairColor), Existence.Requiered, ValidateHairColor),
                       new PropertyMap("ecl", nameof(Passport.EyeColor), Existence.Requiered, ValidateEyeColor),
                       new PropertyMap("pid", nameof(Passport.Id), Existence.Requiered, ValidateId),
                       new PropertyMap("cid", nameof(Passport.CountryId), Existence.Optional, ValidateCountryId)
                   };
        }

        protected virtual bool ValidateBirthYear(string value)
        {
            return true;
        }

        protected virtual bool ValidateIssueYear(string value)
        {
            return true;
        }

        protected virtual bool ValidateExpirationYear(string value)
        {
            return true;
        }

        protected virtual bool ValidateHeight(string value)
        {
            return true;
        }

        protected virtual bool ValidateHairColor(string value)
        {
            return true;
        }

        protected virtual bool ValidateEyeColor(string value)
        {
            return true;
        }

        protected virtual bool ValidateId(string value)
        {
            return true;
        }

        protected virtual bool ValidateCountryId(string value)
        {
            return true;
        }
    }

    internal class AdvancedPassportValidation : PassportValidation
    {
        private const int HeightCmMin = 150;
        private const int HeightCmMax = 193;
        private const int HeightInMin = 59;
        private const int HeightInMax = 76;

        private static readonly string[] AllowedEyeColors =
        {
            "amb",
            "blu",
            "brn",
            "gry",
            "grn",
            "hzl",
            "oth"
        };

        private static readonly Regex IdRegex     = new Regex(@"^\d{9}$");
        private static readonly Regex HexRegex    = new Regex(@"^#([0-9a-f]{6})$");
        private static readonly Regex HeightRegex = new Regex(@"^(?<value>\d+)(?<unit>in|cm)$");

        protected override bool ValidateBirthYear(string value)
        {
            return ValidateYear(value, 1920, 2002);
        }

        protected override bool ValidateIssueYear(string value)
        {
            return ValidateYear(value, 2010, 2020);
        }

        protected override bool ValidateExpirationYear(string value)
        {
            return ValidateYear(value, 2020, 2030);
        }

        protected override bool ValidateHeight(string value)
        {
            Match match = HeightRegex.Match(value);
            if (!match.Success)
            {
                return false;
            }

            string unit = match.Groups["unit"].Value;
            int    size = Int32.Parse(match.Groups["value"].Value);
            return unit switch
                   {
                       "cm" => size >= HeightCmMin && size <= HeightCmMax,
                       "in" => size >= HeightInMin && size <= HeightInMax,
                       _    => throw new NotImplementedException("Unknown height unit.")
                   };
        }

        protected override bool ValidateHairColor(string value)
        {
            return HexRegex.IsMatch(value);
        }

        protected override bool ValidateEyeColor(string value)
        {
            return AllowedEyeColors.Contains(value);
        }

        protected override bool ValidateId(string value)
        {
            return IdRegex.IsMatch(value);
        }

        private bool ValidateYear(string value, int minInclusive, int maxInclusive)
        {
            int yearAsNumber = Int32.Parse(value);
            return yearAsNumber >= minInclusive && yearAsNumber <= maxInclusive;
        }
    }

    internal static class Parser
    {
        public static IEnumerable<IEnumerable<KeyValuePair<string, string>>> Parse(string path)
        {
            List<List<string>> passportsAsLines = GetPassportsAsLines(path);
            List<string> passportsAsString = passportsAsLines
                                             .Select(passportAsLines =>
                                                         passportAsLines.Aggregate((value, nextItem) => value + ' ' + nextItem))
                                             .ToList();

            List<IEnumerable<KeyValuePair<string, string>>> passportsAsKeyValuePairs = passportsAsString.Select(
                    passportAsString => passportAsString.Split(' ')
                                                        .Select(entry => entry.Split(':'))
                                                        .Select(entry => new KeyValuePair<string, string>(entry[0], entry[1]))
                )
                .ToList();

            return passportsAsKeyValuePairs;
        }

        private static List<List<string>> GetPassportsAsLines(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string             line;

            List<List<string>> passportsAsLines = new List<List<string>>
                                                  {
                                                      new List<string>()
                                                  };

            while ((line = reader.ReadLine()) != null)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    passportsAsLines.Add(new List<string>());
                }
                else
                {
                    passportsAsLines[^1].Add(line);
                }
            }

            return passportsAsLines;
        }
    }

    internal class PassportFactory
    {
        private readonly IEnumerable<PropertyMap> propertyMaps;

        public PassportFactory(PassportValidation passportValidation)
        {
            propertyMaps = passportValidation.GetPropertyMaps();
        }

        public Passport From(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            IList<KeyValuePair<string, string>> valuePairList = keyValuePairs.ToList();
            Passport                            passport      = new Passport();

            foreach (PropertyMap propertyMap in propertyMaps)
            {
                if (valuePairList.Any(internalKeyValuePair => internalKeyValuePair.Key == propertyMap.Key))
                {
                    string value = valuePairList.First(internalKeyValuePair => internalKeyValuePair.Key == propertyMap.Key).Value;

                    if (propertyMap.IsValueValid(value))
                    {
                        propertyMap.PropertyInfo.SetValue(passport, value);
                    }
                    else
                    {
                        throw new FormatException($"The Value '{value}' is invalid for property {propertyMap.PropertyName}.");
                    }
                }
                else
                {
                    if (propertyMap.Existence == Existence.Requiered)
                    {
                        throw new FormatException(
                            $"Cannot parse passport. Missing required property {propertyMap.PropertyName}.");
                    }
                }
            }

            return passport;
        }
    }

    internal class Passport
    {
        public string BirthYear      { get; private set; }
        public string IssueYear      { get; private set; }
        public string ExpirationYear { get; private set; }
        public string Height         { get; private set; }
        public string HairColor      { get; private set; }
        public string EyeColor       { get; private set; }
        public string Id             { get; private set; }
        public string CountryId      { get; private set; }
    }

    internal class PropertyMap
    {
        private readonly Func<string, bool> validation;

        public PropertyMap(string key, string propertyName, Existence existence, Func<string, bool> validation = null)
        {
            this.validation = validation;
            Key             = key;
            PropertyName    = propertyName;
            Existence       = existence;
            PropertyInfo    = typeof(Passport).GetProperty(propertyName);
        }

        public string       Key          { get; }
        public string       PropertyName { get; }
        public Existence    Existence    { get; }
        public PropertyInfo PropertyInfo { get; }

        public bool IsValueValid(string value)
        {
            return validation?.Invoke(value) ?? true;
        }
    }

    internal enum Existence
    {
        Optional,
        Requiered
    }
}
