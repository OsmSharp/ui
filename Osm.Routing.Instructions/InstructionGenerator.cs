using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Instructions.LanguageGeneration;
using Osm.Routing.Core.Route;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions
{
    public class InstructionGenerator
    {
        public List<Instruction> Generate(OsmSharpRoute raw_route)
        {
            return this.Generate(raw_route, 
                new Osm.Routing.Instructions.LanguageGeneration.Defaults.SimpleEnglishLanguageGenerator());
        }

        public List<Instruction> Generate(OsmSharpRoute raw_route, ILanguageGenerator language_generator)
        {
            Osm.Routing.Core.ArcAggregation.ArcAggregator aggregator = 
                new Osm.Routing.Core.ArcAggregation.ArcAggregator();
            AggregatedPoint point = 
                aggregator.Aggregate(raw_route);

            return this.Generate(point, language_generator);
        }

        public List<Instruction> Generate(AggregatedPoint aggregate_point)
        {
            return this.Generate(aggregate_point, 
                new Osm.Routing.Instructions.LanguageGeneration.Defaults.SimpleEnglishLanguageGenerator());
        }

        public List<Instruction> Generate(AggregatedPoint point, ILanguageGenerator language_generator)
        {
            Osm.Routing.Core.ArcAggregation.Output.AggregatedPoint p = point;
            while (p != null && p.Next != null)
            {
                // print point.
                Tools.Core.Output.OutputTextStreamHost.WriteLine("Point: {0}:", p.Location);

                if (p.ArcsNotTaken != null)
                {
                    foreach (KeyValuePair<Tools.Math.Geo.Meta.RelativeDirection, Osm.Routing.Core.ArcAggregation.Output.AggregatedArc> arc in p.ArcsNotTaken)
                    {
                        Tools.Core.Output.OutputTextStreamHost.WriteLine("ArcNotTaken:{0} - {1}", arc.Key.Direction.ToString(), arc.Value.Name);
                    }
                }

                if (p.Points != null)
                {
                    foreach (PointPoi poi in p.Points)
                    {
                        string angle_string = "None";
                        if(poi.Angle != null)
                        {
                            angle_string = poi.Angle.Direction.ToString();
                        }
                        Tools.Core.Output.OutputTextStreamHost.WriteLine("Poi [{0}]:{1}", poi.Name, angle_string);
                        foreach (KeyValuePair<string, string> tag in poi.Tags)
                        {
                            Tools.Core.Output.OutputTextStreamHost.WriteLine("PoiTag: {0}->{1}", tag.Key, tag.Value);
                        }
                    }
                }

                // print arc.
                if (p.Angle != null)
                {
                    Tools.Core.Output.OutputTextStreamHost.WriteLine("Arc: {0}[{1}] {2}", p.Angle.Direction, p.Next.Distance, p.Next.Name);
                }
                else
                {
                    Tools.Core.Output.OutputTextStreamHost.WriteLine("Arc: {0}[{1}]", p.Next.Name, p.Next.Distance);
                }
                Tools.Core.Output.OutputTextStreamHost.WriteLine("");

                p = p.Next.Next;
            }
            if (p != null)
            {
                Tools.Core.Output.OutputTextStreamHost.WriteLine("Point: {0}:", p.Location);
            }

            MicroPlanning.MicroPlanner planner = new MicroPlanning.MicroPlanner(language_generator);
            return planner.Plan(point);
        }
    }
}
