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
using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        private const double BoundaryChainSnapTolerance = 0.003;

        /// <summary>
        /// Copies an edge so boundary orientation and snapping do not mutate shared pull cache edges used on other surfaces.
        /// </summary>
        public static Edge CloneEdgeForBoundary(Edge source)
        {
            if (source == null)
                return null;

            Edge copy = new Edge
            {
                Curve = CloneCurveGeometry(source.Curve),
                Name = source.Name,
                Support = source.Support,
                Release = source.Release,
            };
            copy.SetRFEM6ID(source.GetRFEM6ID());
            return copy;
        }

        private static ICurve CloneCurveGeometry(ICurve curve)
        {
            if (curve == null)
                return null;

            Line line = curve as Line;
            if (line != null)
                return new Line { Start = line.Start, End = line.End };

            Polyline polyline = curve as Polyline;
            if (polyline != null)
                return new Polyline { ControlPoints = polyline.ControlPoints.ToList() };

            Arc arc = curve as Arc;
            if (arc != null)
                return arc.Flip().Flip();

            Circle circle = curve as Circle;
            if (circle != null)
                return Create.Circle(circle.Centre, circle.Radius);

            return curve;
        }

        /// <summary>
        /// Projects panel or opening boundary edges onto a common plane, then orders, orients and snaps them into one closed chain.
        /// BHoM joins edges with IJoin and requires exact end-to-start matches; small RFEM coordinate drift otherwise gives "not closed" / "not planar" errors.
        /// The edge list is left in its original order and orientation if the edges do not form a single closed loop.
        /// </summary>
        public static void NormalisePlanarBoundaryEdges(this IList<Edge> edges, string ownerDescription = null, double tolerance = Tolerance.Distance)
        {
            if (edges == null || edges.Count == 0 || edges.Any(e => e?.Curve == null))
                return;

            List<Point> fitPoints = edges.SelectMany(e => e.Curve.IControlPoints()).ToList();
            if (fitPoints.Count >= 3)
            {
                Plane plane = fitPoints.FitPlane(tolerance);
                if (plane != null)
                {
                    foreach (Edge edge in edges)
                        edge.Curve = edge.Curve.IProject(plane);
                }
            }

            if (edges.Count == 1)
                return;

            double snapTolerance = Math.Max(tolerance, BoundaryChainSnapTolerance);

            List<(Edge edge, ICurve curve)> chain = ChainBoundaryEdges(edges, snapTolerance);
            if (chain == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"{ownerDescription ?? "Boundary"} edges do not form a single closed loop within {snapTolerance} m; boundary edges were left unchanged.");
                return;
            }

            for (int i = 0; i < chain.Count; i++)
            {
                int next = (i + 1) % chain.Count;
                SnapJunction(chain, i, next);
            }

            for (int i = 0; i < chain.Count; i++)
            {
                chain[i].edge.Curve = chain[i].curve;
                edges[i] = chain[i].edge;
            }
        }

        private static List<(Edge edge, ICurve curve)> ChainBoundaryEdges(IList<Edge> edges, double tolerance)
        {
            List<Edge> remaining = edges.Skip(1).ToList();
            List<(Edge edge, ICurve curve)> chain = new List<(Edge, ICurve)> { (edges[0], edges[0].Curve) };

            Point chainStart = edges[0].Curve.IStartPoint();
            Point chainEnd = edges[0].Curve.IEndPoint();
            bool firstEdgeFlipped = false;

            while (remaining.Count > 0)
            {
                Edge best = null;
                bool flip = false;
                double bestDistance = double.MaxValue;

                foreach (Edge candidate in remaining)
                {
                    double toStart = chainEnd.Distance(candidate.Curve.IStartPoint());
                    double toEnd = chainEnd.Distance(candidate.Curve.IEndPoint());

                    if (toStart < bestDistance) { bestDistance = toStart; best = candidate; flip = false; }
                    if (toEnd < bestDistance) { bestDistance = toEnd; best = candidate; flip = true; }
                }

                if (best == null || bestDistance > tolerance)
                {
                    if (chain.Count == 1 && !firstEdgeFlipped)
                    {
                        firstEdgeFlipped = true;
                        chain[0] = (chain[0].edge, chain[0].curve.IFlip());
                        Point swap = chainStart;
                        chainStart = chainEnd;
                        chainEnd = swap;
                        continue;
                    }

                    return null;
                }

                ICurve curve = flip ? best.Curve.IFlip() : best.Curve;
                chain.Add((best, curve));
                remaining.Remove(best);
                chainEnd = curve.IEndPoint();
            }

            if (chainEnd.Distance(chainStart) > tolerance)
                return null;

            return chain;
        }

        private static void SnapJunction(List<(Edge edge, ICurve curve)> chain, int current, int next)
        {
            ICurve currentCurve = chain[current].curve;
            ICurve nextCurve = chain[next].curve;

            Point end = currentCurve.IEndPoint();
            Point start = nextCurve.IStartPoint();

            bool currentEditable = IsEndpointEditable(currentCurve);
            bool nextEditable = IsEndpointEditable(nextCurve);

            Point snap;
            if (currentEditable && nextEditable)
                snap = Create.Point((end.X + start.X) / 2, (end.Y + start.Y) / 2, (end.Z + start.Z) / 2);
            else if (currentEditable)
                snap = start;
            else if (nextEditable)
                snap = end;
            else
                return;

            if (currentEditable)
                chain[current] = (chain[current].edge, WithEndPoint(currentCurve, snap));
            if (nextEditable)
                chain[next] = (chain[next].edge, WithStartPoint(chain[next].curve, snap));
        }

        private static bool IsEndpointEditable(ICurve curve)
        {
            return curve is Line || (curve is Polyline polyline && polyline.ControlPoints.Count > 1);
        }

        private static ICurve WithStartPoint(ICurve curve, Point point)
        {
            Line line = curve as Line;
            if (line != null)
                return new Line { Start = point, End = line.End };

            Polyline polyline = curve as Polyline;
            if (polyline != null && polyline.ControlPoints.Count > 0)
            {
                List<Point> controlPoints = polyline.ControlPoints.ToList();
                controlPoints[0] = point;
                return new Polyline { ControlPoints = controlPoints };
            }

            return curve;
        }

        private static ICurve WithEndPoint(ICurve curve, Point point)
        {
            Line line = curve as Line;
            if (line != null)
                return new Line { Start = line.Start, End = point };

            Polyline polyline = curve as Polyline;
            if (polyline != null && polyline.ControlPoints.Count > 0)
            {
                List<Point> controlPoints = polyline.ControlPoints.ToList();
                controlPoints[controlPoints.Count - 1] = point;
                return new Polyline { ControlPoints = controlPoints };
            }

            return curve;
        }
    }
}
