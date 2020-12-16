using System;
using System.Drawing;
using System.IO;

namespace Day3
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Map       map         = new Map(@"./input.txt");
            Generator generator11 = new Generator(1, 1);
            Generator generator31 = new Generator(3, 1);
            Generator generator51 = new Generator(5, 1);
            Generator generator71 = new Generator(7, 1);
            Generator generator12 = new Generator(1, 2);

            TreeCounter treeCounter11 = new TreeCounter(map, generator11);
            TreeCounter treeCounter31 = new TreeCounter(map, generator31);
            TreeCounter treeCounter51 = new TreeCounter(map, generator51);
            TreeCounter treeCounter71 = new TreeCounter(map, generator71);
            TreeCounter treeCounter12 = new TreeCounter(map, generator12);

            long trees11 = treeCounter11.CountTrees();
            long trees31 = treeCounter31.CountTrees();
            long trees51 = treeCounter51.CountTrees();
            long trees71 = treeCounter71.CountTrees();
            long trees12 = treeCounter12.CountTrees();

            Console.WriteLine($"Found (1,1) {trees11} trees.");
            Console.WriteLine($"Found (3,1) {trees31} trees.");
            Console.WriteLine($"Found (5,1) {trees51} trees.");
            Console.WriteLine($"Found (7,1) {trees71} trees.");
            Console.WriteLine($"Found (1,2) {trees12} trees.");

            Console.WriteLine($"If you multiply all, you will get {trees11 * trees31 * trees51 * trees71 * trees12}.");
        }
    }

    internal class TreeCounter
    {
        private readonly Map       map;
        private readonly Generator generator;

        public TreeCounter(Map map, Generator generator)
        {
            this.map       = map;
            this.generator = generator;
        }

        public long CountTrees()
        {
            int trees = 0;

            for (int line = 0; line < map.Lines; line++)
            {
                Point point = generator.Next();

                if (map.IsTree(point))
                {
                    trees++;
                }
            }

            return trees;
        }
    }

    internal class Map
    {
        private readonly string[] lines;
        private readonly int      xWrap;

        public Map(string path)
        {
            lines = File.ReadAllLines(path);
            xWrap = lines[0].Length;
        }

        public int Lines => lines.Length;

        public bool IsTree(Point point)
        {
            if (point.Y >= Lines)
            {
                return false;
            }

            return lines[point.Y][point.X % xWrap] == '#';
        }
    }

    internal class Generator
    {
        private readonly int speedX;
        private readonly int speedY;
        private          int pointX;
        private          int pointY;

        public Generator(int speedX, int speedY)
        {
            this.speedX = speedX;
            this.speedY = speedY;
        }

        public Point Next()
        {
            Point point = new Point(pointX, pointY);

            pointX += speedX;
            pointY += speedY;

            return point;
        }
    }
}
