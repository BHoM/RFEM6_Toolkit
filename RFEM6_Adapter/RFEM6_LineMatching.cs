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
using System;
using System.Collections.Generic;
using System.Linq;

using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private const double LineMatchTolerance = 0.003;

        /// <summary>
        /// Finds the RFEM line number whose geometry best matches the target (model lines and lines queued in this push).
        /// </summary>
        protected int FindBestMatchingLineNumber(RFEMLine target)
        {
            if (target?.Curve == null)
                return 0;

            RFEMLineComparer comparer = new RFEMLineComparer();
            List<(int No, RFEMLine Geometry)> candidates = CollectLineGeometryCandidates();

            List<int> exact = candidates.Where(c => comparer.Equals(target, c.Geometry)).Select(c => c.No).ToList();
            if (exact.Count > 0)
                return exact[0];

            (int No, double Distance)? best = null;
            foreach ((int No, RFEMLine Geometry) candidate in candidates)
            {
                double distance = LineGeometryDistance(target, candidate.Geometry);
                if (best == null || distance < best.Value.Distance)
                    best = (candidate.No, distance);
            }

            if (best == null || best.Value.Distance > LineMatchTolerance)
            {
                BH.Engine.Base.Compute.RecordError("No RFEM line was found that matches the edge curve closely enough to assign a line support or line release.");
                return 0;
            }

            return best.Value.No;
        }

        private List<(int No, RFEMLine Geometry)> CollectLineGeometryCandidates()
        {
            Dictionary<int, Node> nodes = NodeDictionaryForLines();
            HashSet<int> seen = new HashSet<int>();
            List<(int, RFEMLine)> candidates = new List<(int, RFEMLine)>();

            void Add(StructureCore.Line rfLine)
            {
                if (rfLine == null || rfLine.No <= 0 || seen.Contains(rfLine.No))
                    return;

                RFEMLine geometry = RfemLineToComparisonGeometry(rfLine, nodes);
                if (geometry?.Curve == null)
                    return;

                seen.Add(rfLine.No);
                candidates.Add((rfLine.No, geometry));
            }

            foreach (StructureCore.Line rfLine in GetObjectList(new StructureCore.Line()).Where(l => l.No > 0))
                Add(rfLine);

            foreach (StructureCore.Line rfLine in m_createQueue.OfType<StructureCore.Line>())
                Add(rfLine);

            return candidates;
        }

        private Dictionary<int, Node> NodeDictionaryForLines()
        {
            Dictionary<int, Node> nodes = new Dictionary<int, Node>();
            foreach (StructureCore.Node rfNode in GetObjectList(new StructureCore.Node()).Where(n => n.No > 0))
                nodes[rfNode.No] = rfNode.FromRFEM();

            foreach (StructureCore.Node rfNode in m_createQueue.OfType<StructureCore.Node>().Where(n => n.No > 0))
                nodes[rfNode.No] = rfNode.FromRFEM();

            return nodes;
        }

        private static RFEMLine RfemLineToComparisonGeometry(StructureCore.Line rfLine, Dictionary<int, Node> nodes)
        {
            if (rfLine == null)
                return null;

            ICurve curve = null;
            List<Node> lineNodes = new List<Node>();

            if (rfLine.Type == StructureCore.Line.Types.Type.Polyline)
            {
                foreach (int nodeNo in rfLine.DefinitionNodes)
                {
                    if (!nodes.TryGetValue(nodeNo, out Node nd))
                        return null;

                    lineNodes.Add(nd);
                }

                if (lineNodes.Count == 2)
                    curve = Create.Line(lineNodes[0].Position, lineNodes[1].Position);
                else if (lineNodes.Count > 2)
                    curve = Create.Polyline(lineNodes.Select(n => n.Position).ToList());
            }
            else if (rfLine.Type == StructureCore.Line.Types.Type.Arc && rfLine.DefinitionNodes.Count >= 2)
            {
                if (!nodes.TryGetValue(rfLine.DefinitionNodes[0], out Node n0) || !nodes.TryGetValue(rfLine.DefinitionNodes[1], out Node n1))
                    return null;

                Point mid = Create.Point(rfLine.ArcControlPoint.X, rfLine.ArcControlPoint.Y, rfLine.ArcControlPoint.Z);
                lineNodes = new List<Node> { n0, new Node { Position = mid }, n1 };
                curve = Create.Arc(n1.Position, mid, n0.Position).Flip();
            }
            else if (rfLine.Type == StructureCore.Line.Types.Type.Circle)
            {
                Point centre = Create.Point(rfLine.CircleCenter.X, rfLine.CircleCenter.Y, rfLine.CircleCenter.Z);
                lineNodes = new List<Node> { new Node { Position = centre } };
                curve = Create.Circle(centre, rfLine.CircleRadius);
            }

            if (curve == null)
                return null;

            return new RFEMLine { Curve = curve, Nodes = lineNodes };
        }

        private static double LineGeometryDistance(RFEMLine a, RFEMLine b)
        {
            if (a?.Curve == null || b?.Curve == null)
                return double.MaxValue;

            if (new RFEMLineComparer().Equals(a, b))
                return 0;

            if (a.Curve is Line lineA && b.Curve is Line lineB)
            {
                Line reversed = Create.Line(lineB.End, lineB.Start);
                return Math.Min(LineEndpointDistance(lineA, lineB), LineEndpointDistance(lineA, reversed));
            }

            if (a.Curve is Polyline polyA && b.Curve is Polyline polyB)
            {
                List<Point> ptsA = polyA.ControlPoints;
                List<Point> ptsB = polyB.ControlPoints;
                if (ptsA.Count != ptsB.Count)
                    return double.MaxValue;

                double forward = 0;
                for (int i = 0; i < ptsA.Count; i++)
                    forward = Math.Max(forward, ptsA[i].Distance(ptsB[i]));

                double reverse = 0;
                for (int i = 0; i < ptsA.Count; i++)
                    reverse = Math.Max(reverse, ptsA[i].Distance(ptsB[ptsB.Count - 1 - i]));

                return Math.Min(forward, reverse);
            }

            return double.MaxValue;
        }

        private static double LineEndpointDistance(Line a, Line b)
        {
            return a.Start.Distance(b.Start) + a.End.Distance(b.End);
        }

        private static IEnumerable<RFEMLine> LineSegmentsForEdge(Edge edge)
        {
            List<RFEMLine> fromPush = RFEM6EdgeLines.Get(edge);
            if (fromPush.Count > 0)
                return fromPush;

            RFEMLine fromCurve = EdgeComparer.RFEMLineFromEdge(edge);
            return fromCurve != null ? new[] { fromCurve } : Enumerable.Empty<RFEMLine>();
        }
    }
}
