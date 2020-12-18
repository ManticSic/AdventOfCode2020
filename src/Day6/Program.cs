using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            List<List<string>> answersPerGroupList = Parser.Parse("./input.txt");
            int sumOfUniqueAnswers = answersPerGroupList.Select(group => String.Join("", group))
                                                        .Select(answers => answers.ToCharArray())
                                                        .Select(answers => answers.Distinct())
                                                        .Select(uniqueAnswers => uniqueAnswers.Count())
                                                        .Sum();

            Console.WriteLine($"The sum of unique answers is {sumOfUniqueAnswers},");

            int sumOfSameAnswers = answersPerGroupList.Select(answersPerGroup =>
                                                              {
                                                                  if (!answersPerGroup.Any())
                                                                  {
                                                                      return new char[0];
                                                                  }

                                                                  if (answersPerGroup.Count == 1)
                                                                  {
                                                                      return answersPerGroup.First().ToCharArray();
                                                                  }

                                                                  IEnumerable<char[]> answerPerGroupAsArray =
                                                                      answersPerGroup.Select(answer => answer.ToCharArray());
                                                                  return answerPerGroupAsArray
                                                                         .Skip(1)
                                                                         .Aggregate(answerPerGroupAsArray.First(),
                                                                                    (result, current) =>
                                                                                    {
                                                                                        return result.Intersect(current)
                                                                                            .ToArray();
                                                                                    })
                                                                         .ToArray();
                                                              })
                                                      .Select(sameAnswers => sameAnswers.Length)
                                                      .Sum();

            Console.WriteLine($"The sum of same answers is {sumOfSameAnswers},");
        }
    }

    internal static class Parser
    {
        public static List<List<string>> Parse(string path)
        {
            using StreamReader reader = new StreamReader(path);
            string             line;

            List<List<string>> answersPerGroup = new List<List<string>>();
            List<string>       group           = null;
            while ((line = reader.ReadLine()) != null)
            {
                group ??= new List<string>();

                if (String.IsNullOrWhiteSpace(line))
                {
                    answersPerGroup.Add(group);
                    group = null;
                }
                else
                {
                    group.Add(line);
                }
            }

            answersPerGroup.Add(group);

            return answersPerGroup;
        }
    }
}
