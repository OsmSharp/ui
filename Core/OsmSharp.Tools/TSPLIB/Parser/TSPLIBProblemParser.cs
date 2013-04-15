// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using OsmSharp.Tools.TSPLIB.Problems;
using OsmSharp.Tools.TSPLIB.Parser.Primitives;

namespace OsmSharp.Tools.TSPLIB.Parser
{
    /// <summary>
    /// Parses TSP LIB problems.
    /// </summary>
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
            return TSPLIBProblemParser.ParseFrom(path, 0, 0);
        }

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFrom(FileInfo info)
        {
            return TSPLIBProblemParser.ParseFrom(info, 0, 0);
        }

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFrom(StreamReader reader)
        {
            return TSPLIBProblemParser.ParseFrom(reader, 0, 0);
        }

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFrom(string path, int? first, int? last)
        {
            return TSPLIBProblemParser.ParseFrom(new FileInfo(path), first, last);
        }

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFrom(FileInfo info, int? first, int? last)
        {
            return TSPLIBProblemParser.ParseFrom(info.OpenText(), first, last);
        }

        /// <summary>
        /// Parses a TSP-lib file and creates a TSPLIBProblem.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static TSPLIBProblem ParseFrom(StreamReader reader, int? first, int? last)
        {
            TSPLIBProblemTypeEnum? problem_type = null;
            TSPLIBProblemWeightTypeEnum? weight_type = null;
            int size = -1;
            double[][] weights = null;
            string comment = string.Empty;
            string name = string.Empty;

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
                    weights = new double[size][];
                    weights[0] = new double[size];

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
                                            weights[x] = new double[size];
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

            return new TSPLIBProblem(name, comment, size, weights, weight_type.Value, problem_type.Value,
                first, last);
        }

        /// <summary>
        /// Calculate the euclidean weights.
        /// </summary>
        /// <returns></returns>
        private static double[][] CalculateEuclideanWeights(List<Point> points)
        {
            double[][] weigths = new double[points.Count][];

            for (int city1 = 0; city1 < points.Count; city1++)
            {
                weigths[city1] = new double[points.Count];
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
