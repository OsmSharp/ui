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
using OsmSharp.Tools.Math.TSP.Problems;

namespace OsmSharp.Tools.Math.TSP
{
    /// <summary>
    /// Converts TSP problems and routes.
    /// </summary>
    /// <remarks>
    /// References: TRANSFORMING ASYMMETRIC INTO SYMMETRIC TRAVELING SALESMAN PROBLEMS
    /// Roy JONKER and Ton VOLGENANT (1983)
    /// </remarks>
    public static class Convertor
    {
        /// <summary>
        /// Converts a symmetric to an asymmetric problem.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public static IProblem ConvertToSymmetric(this IProblem problem)
        {
            int n = problem.Size;
            double[][] weights = new double[n * 2][];
            double M = double.MinValue;

            for (int x = 0; x < n * 2; x++)
            {
                weights[x] = new double[n * 2];
            }

            // calculate M
            for (int i = 0; i < problem.Size; i++)
            {
                for (int j = 0; j < problem.Size; j++)
                {
                    double weight = problem.Weight(i, j);
                    if (weight > M)
                    {
                        M = weight;
                    }
                }
            }

            double m_to_be = 100;
            while (M > m_to_be)
            {
                m_to_be = m_to_be * 10;
            }
            M = m_to_be;

            // create C with negative M
            double[][] old_weights = new double[n][];
            for (int i = 0; i < n; i++)
            {
                old_weights[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        old_weights[i][j] = 0;//;
                    }
                    else
                    {
                        old_weights[i][j] = problem.WeightMatrix[i][j] + M;//problem.WeightMatrix[i][j];
                    }
                }
            }

            for (int i = 0; i < problem.Size; i++)
            {
                for (int j = 0; j < problem.Size; j++)
                {
                    weights[n + i][j] = old_weights[i][j];
                    weights[i][n + j] = old_weights[j][i];

                    weights[i][j] = M * 100;
                    weights[n + i][n + j] = M * 100;
                }
            }
            return MatrixProblem.CreateSTSP(weights);
        }

        /// <summary>
        /// Adds a virtual depot with weight equal to zero to all customers.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public static IProblem AddVirtualDepot(this IProblem problem)
        {
            double[][] new_weights = new double[problem.Size + 1][];

            for (int r = 0; r < problem.Size + 1; r++)
            {
                new_weights[r] = new double[problem.Size + 1];

                for(int c = 0; c < problem.Size; c++)
                {
                    if (c == 0 || r == 0)
                    {
                        new_weights[r][c] = 0;
                    }
                    else
                    {
                        new_weights[r][c] = problem.WeightMatrix[r - 1][c - 1];
                    }
                }                
            }

            return MatrixProblem.CreateATSPOpen(new_weights, 0);
        }
    }
}
