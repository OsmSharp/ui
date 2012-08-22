using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using Tools.TSPLIB.Problems;
using Tools.TSPLIB.Parser.Primitives;

namespace Tools.TSPLIB.Parser
{
    public class TSPLIBProblemParser
    {
        private const string TOKEN_NAME = "NAME:";
        private const string TOKEN_EOF = "EOF";
        private const string TOKEN_TYPE = "TYPE:";
        private const string TOKEN_DIMENSION = "DIMENSION:";
        private const string TOKEN_TYPE_VALUE_TSP = "TSP";
        private const string TOKEN_TYPE_VALUE_ATSP = "ATSP";
        private const string TOKEN_COMMENT = "COMMENT:";
        private const string TOKEN_EDGE_WEIGHT_TYPE = "EDGE_WEIGHT_TYPE:";
        private const string TOKEN_EDGE_WEIGHT_TYPE_VALUE_EUC_2D = "EUC_2D";
        private const string TOKEN_EDGE_WEIGHT_TYPE_EXPLICIT = "EXPLICIT";
        private const string TOKEN_EDGE_WEIGHT_FORMAT = "EDGE_WEIGHT_FORMAT:";
        private const string TOKEN_EDGE_WEIGHT_FORMAT_MATRIX = "MATRIX";
        private const string TOKEN_EDGE_WEIGHT_SECTION = "EDGE_WEIGHT_SECTION";
        private const string TOKEN_NODE_COORD_SECTION = "NODE_COORD_SECTION";

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFrom(string path)
        {
            return TSPLIBProblemParser.ParseFromFile(new FileInfo(path));
        }

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFromFile(FileInfo file)
        {
            TSPLIBProblemTypeEnum? problem_type = null;
            TSPLIBProblemWeightTypeEnum? weight_type = null;
            int size = -1;
            float[][] weights = null;
            string comment = string.Empty;
            string name = string.Empty;

            StreamReader reader = file.OpenText();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim().Replace(" :", ":");

                if (line.StartsWith(TOKEN_NAME))
                {
                    name = line.Replace(TOKEN_NAME, string.Empty).Trim();
                }
                else if (line.StartsWith(TOKEN_TYPE))
                {
                    string type = line.Replace(TOKEN_TYPE, string.Empty).Trim();
                    switch (type.ToUpper())
                    {
                        case TOKEN_TYPE_VALUE_TSP:
                            problem_type = TSPLIBProblemTypeEnum.TSP;
                            break;
                        case TOKEN_TYPE_VALUE_ATSP:
                            problem_type = TSPLIBProblemTypeEnum.ATSP;
                            break;
                    }
                }
                else if (line.StartsWith(TOKEN_COMMENT))
                {
                    comment = line.Replace(TOKEN_COMMENT, string.Empty).Trim();
                }
                else if (line.StartsWith(TOKEN_DIMENSION))
                {
                    string dimension = line.Replace(TOKEN_DIMENSION, string.Empty).Trim();
                    size = int.Parse(dimension);
                }
                else if (line.StartsWith(TOKEN_EDGE_WEIGHT_TYPE))
                {
                    string edge_weight_type = line.Replace(TOKEN_EDGE_WEIGHT_TYPE, string.Empty).Trim();
                    switch (edge_weight_type.ToUpper())
                    {
                        case TOKEN_EDGE_WEIGHT_TYPE_VALUE_EUC_2D:
                            weight_type = TSPLIBProblemWeightTypeEnum.Euclidian2D;
                            break;
                        case TOKEN_EDGE_WEIGHT_TYPE_EXPLICIT:
                            weight_type = TSPLIBProblemWeightTypeEnum.Explicit;
                            break;
                    }
                }
                else if (line.StartsWith(TOKEN_EDGE_WEIGHT_SECTION))
                {
                    weights = new float[size][];
                    weights[0] = new float[size];

                    int x = 0;
                    int y = 0;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine().Trim();

                        if (line.StartsWith(TOKEN_EOF))
                        {
                            break;
                        }
                        else
                        {
                            if (line != null && line.Length > 0)
                            {
                                string[] splitted_line = Regex.Split(line, @"\s+");
                                foreach (string weight_string in splitted_line)
                                {
                                    weights[x][y] = float.Parse(weight_string, CultureInfo.InvariantCulture);

                                    if (x == y)
                                    {
                                        weights[x][y] = 0;
                                    }

                                    if (y == size - 1)
                                    {
                                        x = x + 1;
                                        if (x < size)
                                        {
                                            weights[x] = new float[size];
                                            y = 0;
                                        }
                                    }
                                    else
                                    {
                                        y = y + 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (line.StartsWith(TOKEN_NODE_COORD_SECTION))
                {
                    List<Point> points = new List<Point>();
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine().Trim();

                        if (line.StartsWith(TOKEN_EOF))
                        {
                            break;
                        }
                        else
                        {
                            string[] splitted_line = Regex.Split(line, @"\s+");
                            int idx = (int)double.Parse(splitted_line[0]);
                            int x = (int)double.Parse(splitted_line[1]);
                            int y = (int)double.Parse(splitted_line[2]);

                            Point p = new Point(x, y);
                            points.Add(p);
                        }
                    }

                    weights = TSPLIBProblemParser.CalculateEuclideanWeights(points);
                }
            }

            return new TSPLIBProblem(name, comment, size, weights, weight_type.Value, problem_type.Value);
        }

        /// <summary>
        /// Calculate the euclidean weights.
        /// </summary>
        /// <returns></returns>
        private static float[][] CalculateEuclideanWeights(List<Point> points)
        {
            float[][] weigths = new float[points.Count][];

            for (int city1 = 0; city1 < points.Count; city1++)
            {
                weigths[city1] = new float[points.Count];
                for (int city2 = 0; city2 < points.Count; city2++)
                {
                    weigths[city1][city2] = (float)System.Math.Round(System.Math.Sqrt(
                                System.Math.Pow(points[city1].X - points[city2].X, 2) +
                                System.Math.Pow(points[city1].Y - points[city2].Y, 2)));
                }
            }

            return weigths;
        }

    }
}
