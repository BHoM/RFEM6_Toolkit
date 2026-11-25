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
using System.Linq;

using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using StructureCore = Dlubal.Api.Rfem.StructureCore;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        /// <summary>
        /// Builds RFEM line geometry from the RFEM API line and model nodes (shared by pull read and line matching).
        /// </summary>
        internal static RFEMLine FromRFEM(StructureCore.Line rfLine, Dictionary<int, Node> nodes)
        {
            if (rfLine == null || nodes == null)
                return null;

            ICurve curve = null;
            List<Node> lineNodes = new List<Node>();

            if (rfLine.Type == StructureCore.Line.Types.Type.Polyline)
            {
                foreach (int nodeNo in rfLine.DefinitionNodes)
                {
                    if (!nodes.TryGetValue(nodeNo, out Node nd) || nd == null)
                        return null;

                    lineNodes.Add(nd);
                }

                if (lineNodes.Count < 2)
                    return null;

                List<Point> polylinePoints = lineNodes.Select(n => n.Position).ToList();
                if (polylinePoints.Count == 2)
                    curve = Create.Line(polylinePoints[0], polylinePoints[1]);
                else
                    curve = Create.Polyline(polylinePoints);
            }
            else if (rfLine.Type == StructureCore.Line.Types.Type.Arc && rfLine.DefinitionNodes.Count >= 2)
            {
                if (!nodes.TryGetValue(rfLine.DefinitionNodes[0], out Node n0) || n0 == null
                    || !nodes.TryGetValue(rfLine.DefinitionNodes[1], out Node n1) || n1 == null)
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
    }
}
