using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.TSP;
using OsmSharp.Units.Time;
using OsmSharp.Math.TSP.LocalSearch.HillClimbing3Opt;
using OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.InterImprovements;
using OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.CheapestInsertion;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Logging;

namespace OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.VNS
{
    /// <summary>
    /// Uses a Variable Neighbourhood Search technique.
    /// </summary>
    public class GuidedVNS : RouterMaxTime
    {
        /// <summary>
        /// Holds the router.
        /// </summary>
        private readonly Router _router;

        /// <summary>
        /// Holds the lambda value.
        /// </summary>
        private readonly float _lambda;

        /// <summary>
        /// The threshold percentage.
        /// </summary>
        private readonly float _thresholdPercentage;

        /// <summary>
        /// Creates a new Guided Variable Neighbourhood Search solver.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="max"></param>
        /// <param name="deliveryTime"></param>
        /// <param name="thresholdPrecentage"></param>
        /// <param name="lambda"></param>
        public GuidedVNS(Router router, Second max, Second deliveryTime, float thresholdPrecentage, float lambda)
            : base(max, deliveryTime)
        {
            _router = router;
            _thresholdPercentage = thresholdPrecentage;
            _lambda = lambda;

            _intraImprovements = new List<IImprovement>();
            //_intra_improvements.Add(
            //    new ArbitraryInsertionSolver());
            _intraImprovements.Add(
                new HillClimbing3OptSolver(true, true));

            _interImprovements = new List<IInterImprovement>();
            _interImprovements.Add(
                new RelocateImprovement());
            _interImprovements.Add(
                new ExchangeInterImprovement());
            //_inter_improvements.Add(
            //    new TwoOptInterImprovement());
            _interImprovements.Add(
                new RelocateExchangeInterImprovement());
            _interImprovements.Add(
                new CrossExchangeInterImprovement());
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "VNS";
            }
        }

        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            float lambda = _lambda;

            var vrpRouter =
                new CheapestInsertionSolverWithImprovements(
                    _router, problem.Max.Value, problem.DeliveryTime.Value, 5, 0.10f, true, _thresholdPercentage);
            MaxTimeSolution originalSolution = vrpRouter.Solve(
                problem);

            for (int roundX = 0; roundX < originalSolution.Count; roundX++)
            { // keep looping on rounds.
                for (int roundY = 0; roundY < roundX; roundY++)
                { // keep looping on rounds with a smaller index not equal to the current round.
                    if (roundX != roundY)
                    { // routes are different.
                        if (this.Overlaps(problem, originalSolution.Route(roundX), originalSolution.Route(roundY)))
                        { // only check routes that overlap.
                            double tau = double.MinValue;
                            var penalizations = new Dictionary<Edge, int>();

                            //bool improvement = true;
                            //while (improvement)
                            //{ // keep looping until no more improvement is found.
                            //    improvement = false;

                            while (true)
                            { // keep trying to improve until the tau limit is exceeded.
                                // calculate the route sizes before.
                                double route1ActualBefore = problem.Time(originalSolution.Route(roundX));
                                double route2ActualBefore = problem.Time(originalSolution.Route(roundY));

                                // copy orignal solution.
                                var solution = (originalSolution.Clone() as MaxTimeSolution);

                                // apply penalties.
                                foreach (KeyValuePair<Edge, int> penalty in penalizations)
                                {
                                    problem.Penalize(penalty.Key, (double)penalty.Value * lambda);
                                }

                                // apply the inter-route improvements.
                                int countBefore = solution.Route(roundX).Count + solution.Route(roundY).Count;
                                int countAfter;
                                if (this.ImproveInterRoute(problem, solution, roundX, roundY, problem.Max.Value))
                                { // the improve inter route succeeded.
                                    if (!solution.IsValid())
                                    {
                                        throw new Exception();
                                    }
                                    //improvement_inter = true;

                                    countAfter = solution.Route(roundX).Count + solution.Route(roundY).Count;
                                    if (countBefore != countAfter)
                                    {
                                        throw new Exception();
                                    }

                                    // apply the intra-route improvements.
                                    solution[roundX] = this.ImproveIntraRoute(problem, solution.Route(roundX), solution[roundX]);
                                    solution[roundY] = this.ImproveIntraRoute(problem, solution.Route(roundY), solution[roundY]);

                                    // recalculate weights.
                                    solution[roundX] = problem.Time(solution.Route(roundX));
                                    solution[roundY] = problem.Time(solution.Route(roundY));
                                }

                                // check customer counts.
                                countAfter = solution.Route(roundX).Count + solution.Route(roundY).Count;
                                if (countBefore != countAfter)
                                {
                                    throw new Exception();
                                }

                                // undo the penalizations.
                                problem.ResetPenalizations();

                                // check against the orginal objective function.
                                double route1ActualAfter = problem.Time(solution.Route(roundX));
                                double route2ActualAfter = problem.Time(solution.Route(roundY));
                                if (route1ActualAfter + route2ActualAfter < route1ActualBefore + route2ActualBefore - 0.001 &&
                                    route1ActualAfter < problem.Max.Value && route2ActualAfter < problem.Max.Value)
                                { // there is improvement!
                                    originalSolution = solution;

                                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.VNS.GuidedVNS", TraceEventType.Information,
                                        "IMPROVEMENT: {0}->{1}", route1ActualBefore + route2ActualBefore, route1ActualAfter + route2ActualAfter);
                                }

                                // select arc to be penalized.
                                IRoute route1 = originalSolution.Route(roundX);
                                IRoute route2 = originalSolution.Route(roundY);
                                double u = double.MinValue;
                                Edge penalizingEdge = new Edge();
                                double totalP = 0;
                                foreach (Edge edge in route1.Edges())
                                {
                                    int edgeP;
                                    if (!penalizations.TryGetValue(edge, out edgeP))
                                    {
                                        edgeP = 0;
                                    }
                                    totalP = totalP + edgeP;
                                    double edgeU = ((lambda * (double)edgeP) + problem.WeightMatrix[edge.From][edge.To]) /
                                        ((double)edgeP + 1.0);
                                    if (u <= edgeU)
                                    {
                                        penalizingEdge = edge;
                                        u = edgeU;
                                    }
                                }
                                foreach (Edge edge in route2.Edges())
                                {
                                    int edgeP;
                                    if (!penalizations.TryGetValue(edge, out edgeP))
                                    {
                                        edgeP = 0;
                                    }
                                    totalP = totalP + edgeP;
                                    double edgeU = ((lambda * (double)edgeP) + problem.WeightMatrix[edge.From][edge.To]) /
                                        ((double)edgeP + 1.0);
                                    if (u <= edgeU)
                                    {
                                        penalizingEdge = edge;
                                        u = edgeU;
                                    }
                                }

                                // actually penalize the edge.
                                int p;
                                if (!penalizations.TryGetValue(penalizingEdge, out p))
                                {
                                    p = 1;
                                }
                                else
                                {
                                    p++;
                                }
                                penalizations[penalizingEdge] = p;

                                // evaluate or set tau.
                                if (tau > double.MinValue)
                                { // evaluate if penalizations should end.
                                    if (tau <= lambda * totalP)
                                    { // the penalization should end!
                                        break;
                                    }
                                }
                                else
                                { // first edge being penalized.
                                    tau = lambda * problem.WeightMatrix[penalizingEdge.From][penalizingEdge.To] / 10;
                                }
                            }
                        }
                    }
                }
            }
            return originalSolution;
        }

        #region Improvement Heurstics

        /// <summary>
        /// Holds the intra-route improvements;
        /// </summary>
        private readonly List<IImprovement> _intraImprovements;

        /// <summary>
        /// Holds the inter-route improvements.
        /// </summary>
        private readonly List<IInterImprovement> _interImprovements;

        /// <summary>
        /// Apply some improvements within one route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="currentWeight"></param>
        private double ImproveIntraRoute(OsmSharp.Math.VRP.Core.IProblemWeights problem, IRoute route, 
            double currentWeight)
        {
            bool improvement = true;
            double newWeight = currentWeight;
            while (improvement)
            { // keep trying while there are still improvements.
                improvement = false;

                // loop over all improvement operations.
                foreach (IImprovement improvementOperation in _intraImprovements)
                { // try the current improvement operations.
                    double difference;
                    if (improvementOperation.Improve(problem, route, out difference))
                    { // there was an improvement.
                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.VNS.GuidedVNS", TraceEventType.Information,
                            "Intra-improvement found {0} {1}->{2}",
                            improvementOperation.Name, newWeight, newWeight + difference);

                        // check if the route is valid.
                        if (!route.IsValid())
                        {
                            throw new Exception();
                        }

                        // update the weight.
                        newWeight = newWeight + difference;

                        improvement = true;

                        break;
                    }
                }
            }
            return newWeight;
        }

        /// <summary>
        /// Apply some improvements between the given routes and returns the resulting weight.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route2Idx"></param>
        /// <param name="max"></param>
        /// <param name="route1Idx"></param>
        /// <returns></returns>
        private bool ImproveInterRoute(MaxTimeProblem problem, MaxTimeSolution solution, 
            int route1Idx, int route2Idx, double max)
        {
            // get the routes.
            IRoute route1 = solution.Route(route1Idx);
            IRoute route2 = solution.Route(route2Idx);

            int countBefore = route1.Count + route2.Count;

            // loop over all improvement operations.
            bool globalImprovement = false;
            foreach (IInterImprovement improvementOperation in _interImprovements)
            { // try the current improvement operations.
                bool improvement = true;
                while (improvement)
                { // keep looping when there is improvement.
                    improvement = false;
                    double totalBefore = problem.Time(solution.Route(route1Idx)) +
                        problem.Time(solution.Route(route2Idx));
                    if (improvementOperation.Improve(problem, solution, route1Idx, route2Idx, max))
                    { // there was an improvement.
                        improvement = true;
                        globalImprovement = true;

                        // check if the route is valid.
                        if (!route1.IsValid())
                        {
                            throw new Exception();
                        }
                        if (!route2.IsValid())
                        {
                            throw new Exception();
                        }

                        int countAfter = route1.Count + route2.Count;
                        if (countBefore != countAfter)
                        {
                            throw new Exception();
                        }

                        double totalAfter = problem.Time(solution.Route(route1Idx)) +
                            problem.Time(solution.Route(route2Idx));
                        if (totalAfter >= totalBefore)
                        {
                            throw new Exception("this is not an improvement!");
                        }

                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.VNS.GuidedVNS", TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2} ({3}->{4})",
                            route1Idx, route2Idx, improvementOperation.Name, totalBefore, totalAfter);

                        // recalculate weights.
                        solution[route1Idx] = problem.Time(solution.Route(route1Idx));
                        solution[route2Idx] = problem.Time(solution.Route(route2Idx));

                        //break;
                    }
                    else if (!improvementOperation.IsSymmetric &&
                        improvementOperation.Improve(problem, solution, route2Idx, route1Idx, max))
                    { // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        globalImprovement = true;


                        // check if the route is valid.
                        if (!route1.IsValid())
                        {
                            throw new Exception();
                        }
                        if (!route2.IsValid())
                        {
                            throw new Exception();
                        }

                        int countAfter = route1.Count + route2.Count;
                        if (countBefore != countAfter)
                        {
                            throw new Exception();
                        }

                        double totalAfter = problem.Time(solution.Route(route1Idx)) +
                            problem.Time(solution.Route(route2Idx));
                        if (totalAfter >= totalBefore)
                        {
                            throw new Exception("this is not an improvement!");
                        }

                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.Optimization.VRP.WithDepot.MaxTime.VNS.GuidedVNS", TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2} ({3}->{4})",
                            route2Idx, route1Idx, improvementOperation.Name, totalBefore, totalAfter);

                        // recalculate weights.
                        solution[route1Idx] = problem.Time(solution.Route(route1Idx));
                        solution[route2Idx] = problem.Time(solution.Route(route2Idx));

                        //break;
                    }
                }
            }
            return globalImprovement;
        }

        #endregion
    }
}