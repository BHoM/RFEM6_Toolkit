/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *
 *
 * The BHoM is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3.0 of the License, or
 * (at your option) any later version.
 *
 * The BHoM is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BH.Engine.Geometry;
using BH.oM.Adapter;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    [Description("Builds RFEM lines for panel and opening edges: straight segments, polylines split per segment, arcs and circles.")]
    public class GetLineFromEdgeModule : IGetDependencyModule<Edge, RFEMLine>
    {
        private const double PositionTolerance = 1e-4;

        public IEnumerable<RFEMLine> GetDependencies(IEnumerable<Edge> objects)
        {
            List<RFEMLine> lines = new List<RFEMLine>();

            foreach (Edge edge in objects)
            {
                List<RFEMLine> edgeLines = RFEMLinesFromEdge(edge);
                if (edgeLines.Count == 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Edge {edge.BHoM_Guid} has curve type '{edge.Curve?.GetType().Name ?? "null"}'. Only Line, Polyline, Arc and Circle are supported.");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(edge.Name))
                {
                    foreach (RFEMLine rfLine in edgeLines)
                        rfLine.Name = edge.Name;
                }

                RFEM6EdgeLines.Set(edge, edgeLines);
                lines.AddRange(edgeLines);
            }

            return lines;
        }

        private static List<RFEMLine> RFEMLinesFromEdge(Edge edge)
        {
            switch (edge.Curve)
            {
                case Line line:
                    return new List<RFEMLine>
                    {
                        new RFEMLine
                        {
                            Curve = line,
                            Nodes = new List<Node> { new Node { Position = line.Start }, new Node { Position = line.End } },
                        },
                    };

                case Polyline polyline:
                    return PolylineSegments(polyline);

                case Arc arc:
                    return new List<RFEMLine> { ArcLine(arc) };

                case Circle circle:
                    return new List<RFEMLine>
                    {
                        new RFEMLine
                        {
                            Curve = circle,
                            Nodes = new List<Node> { new Node { Position = circle.Centre } },
                        },
                    };

                default:
                    return new List<RFEMLine>();
            }
        }

        /// <summary>
        /// RFEM opening and surface boundaries use one line per straight segment. A closed polyline drops its repeated closing point.
        /// </summary>
        private static List<RFEMLine> PolylineSegments(Polyline polyline)
        {
            List<Point> points = polyline.ControlPoints?.ToList() ?? new List<Point>();
            if (points.Count < 2)
                return new List<RFEMLine>();

            if (points.Count > 2 && points[0].Distance(points[points.Count - 1]) < PositionTolerance)
                points.RemoveAt(points.Count - 1);

            List<RFEMLine> segments = new List<RFEMLine>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point start = points[i];
                Point end = points[i + 1];
                Line segment = Create.Line(start, end);
                segments.Add(new RFEMLine
                {
                    Curve = segment,
                    Nodes = new List<Node> { new Node { Position = start }, new Node { Position = end } },
                });
            }

            return segments;
        }

        private static RFEMLine ArcLine(Arc arc)
        {
            Point[] pts = arc.ControlPoints().ToArray();
            return new RFEMLine
            {
                Curve = arc,
                Nodes = new List<Node>
                {
                    new Node { Position = pts[0] },
                    new Node { Position = pts[2] },
                    new Node { Position = pts[4] },
                },
            };
        }
    }
}
