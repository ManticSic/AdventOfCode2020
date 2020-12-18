using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day7
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string shinyGold = "shiny gold";

            IList<Bag>    bags          = Parser.Parse("./input.txt");
            BagCalculator bagCalculator = new BagCalculator(bags);

            IEnumerable<Bag> bagsWhichCanRecursiveHoldColor = bagCalculator.FindBagsWhichCanRecursiveHold(shinyGold);

            Console.WriteLine($"Totaly {bagsWhichCanRecursiveHoldColor.Count()} can hold {shinyGold} recursive.");

            IEnumerable<Bag> bagsWhichAreHoldByShinyGold = bagCalculator.FindBagsWhichAreHoldBy(shinyGold);

            Console.WriteLine($"For one {shinyGold} bag, we have a total of {bagsWhichAreHoldByShinyGold.Count()} bags.");
        }
    }

    internal class BagCalculator
    {
        private readonly IDictionary<string, Bag> bags;

        public BagCalculator(IList<Bag> bags)
        {
            this.bags = bags.ToDictionary(bag => bag.Color, bag => bag);
        }

        public IEnumerable<Bag> FindBagsWhichCanRecursiveHold(string color)
        {
            return FindBagsWhichCanRecursiveHold(color, bags.Select(grp => grp.Value).ToList());
        }

        public IEnumerable<Bag> FindBagsWhichAreHoldBy(string color)
        {
            Bag bag = GetBagByColor(color);
            return FindBagsWhichAreHoldBy(bag);
        }

        public IEnumerable<Bag> FindBagsWhichAreHoldBy(Bag bag)
        {
            List<Bag> result = new List<Bag>();

            foreach ((string contentColor, int contentCount) in bag.Content)
            {
                Bag contentBag = GetBagByColor(contentColor);

                result.AddTimes(contentBag, contentCount);

                if (contentBag.Content.Any())
                {
                    result.AddTimesRange(FindBagsWhichAreHoldBy(contentBag), contentCount);
                }
            }

            return result;
        }

        private IEnumerable<Bag> FindBagsWhichCanRecursiveHold(string color, IList<Bag> childBags)
        {
            return childBags.Where(bag => CanBagHoldColorRecursive(bag, color));
        }

        private bool CanBagHoldColorRecursive(Bag bag, string color)
        {
            if (CanBagHoldColor(bag, color))
            {
                return true;
            }

            return bag.Content.Select(grp => grp.Key)
                      .Select(childBagColor => GetBagByColor(childBagColor))
                      .Any(childBag => CanBagHoldColorRecursive(childBag, color));
        }

        private bool CanBagHoldColor(Bag bag, string color)
        {
            return bag.Content.ContainsKey(color);
        }

        private Bag GetBagByColor(string color)
        {
            return bags[color];
        }
    }

    internal static class Parser
    {
        public static IList<Bag> Parse(string path)
        {
            string[] rules = File.ReadAllLines(path);

            BagFactory factory = new BagFactory();

            return rules.Select(factory.Parse)
                        .ToList();
        }
    }

    internal class BagFactory
    {
        private const           string ContentSeparator = ",";
        private const           string RuleSeparator = "contain";
        private const           string RedundantBagsKeyword = "bags";
        private const           string RedundantLineEndingChar = ".";
        private static readonly Regex  ContentRegex = new Regex(@"^(?<count>\d+) (?<color>\w+\s\w+) bags?|(?<no>no other bags)$");

        public Bag Parse(string rule)
        {
            string[] parts = rule.Split(RuleSeparator);

            string     color   = FormatBagColor(parts[0]);
            BagBuilder builder = new BagBuilder(color);

            parts[1].Replace(RedundantLineEndingChar, String.Empty)
                    .Split(ContentSeparator)
                    .Select(content => content.Trim())
                    .Select(content =>
                            {
                                Match match = ContentRegex.Match(content.Trim());

                                if (!String.IsNullOrWhiteSpace(match.Groups["no"].Value))
                                {
                                    return null;
                                }
                                else
                                {
                                    string contentColor = FormatBagColor(match.Groups["color"].Value);
                                    int    count        = Int32.Parse(match.Groups["count"].Value);
                                    return new BagBuilder.BagContent(contentColor, count);
                                }
                            })
                    .ToList()
                    .ForEach(content =>
                             {
                                 if (content == null)
                                 {
                                     return;
                                 }

                                 builder.Add(content.Color, content.Count);
                             });

            return builder.Build();
        }

        private string FormatBagColor(string input)
        {
            return input.Replace(RedundantBagsKeyword, String.Empty)
                        .Trim();
        }
    }

    internal class BagBuilder
    {
        public BagBuilder(string color)
        {
            Color = color;
        }

        private string Color { get; }

        private IDictionary<string, int> Content { get; } = new Dictionary<string, int>();

        public void Add(string color, int count)
        {
            Content.Add(color, count);
        }

        public Bag Build()
        {
            return new Bag(Color, Content);
        }

        public class BagContent
        {
            public BagContent(string color, int count)
            {
                Color = color;
                Count = count;
            }

            public string Color { get; }

            public int Count { get; }
        }
    }

    internal class Bag
    {
        public Bag(string color, IDictionary<string, int> content)
        {
            Color   = color;
            Content = content;
        }

        public string Color { get; }

        public IDictionary<string, int> Content { get; }
    }

    internal static class ListExtensions
    {
        public static void AddTimes<T>(this List<T> list, T item, int times)
        {
            for (int index = 0; index < times; index++)
            {
                list.Add(item);
            }
        }

        public static void AddTimesRange<T>(this List<T> list, IEnumerable<T> items, int times)
        {
            T[] arr = items.ToArray();
            for (int index = 0; index < times; index++)
            {
                list.AddRange(arr);
            }
        }
    }
}
