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
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Geometry;

using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Base;
using Dlubal.Api.Common;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static StructureCore.Line ToRFEM6(this Line line, int lineNo, Node startNode, Node endNode)
        {


            StructureCore.Line rfLine = new StructureCore.Line
            {
                No = lineNo,
                Type = StructureCore.Line.Types.Type.Polyline,
                DefinitionNodes = { startNode.GetRFEM6ID(), endNode.GetRFEM6ID() },
            };

            return rfLine;

        }

        public static StructureCore.Line ToRFEM6(this RFEMLine rfemLine)
        {

            List<Node> nodes = rfemLine.Nodes;

            StructureCore.Line rfLine = new StructureCore.Line();


            if (rfemLine.Curve is Polyline || rfemLine.Curve is Line)
            {
                rfLine = new StructureCore.Line
                {
                    No = rfemLine.GetRFEM6ID(),
                    Type = StructureCore.Line.Types.Type.Polyline,
                };
                rfLine.DefinitionNodes.AddRange(rfemLine.Nodes.Select(x => x.GetRFEM6ID()));
            }


            if (rfemLine.Curve is Arc)
            {

                Node n00 = rfemLine.Nodes.ToArray()[0];

                Point mid = Engine.Geometry.Query.PointAtLength((Arc)rfemLine.Curve, 0.5);

                Node n11 = rfemLine.Nodes.ToArray()[2];


                rfLine = new StructureCore.Line()
                {
                    No = rfemLine.GetRFEM6ID(),
                    Type = StructureCore.Line.Types.Type.Arc,
                    ArcAlphaAdjustmentTarget = StructureCore.Line.Types.ArcAlphaAdjustmentTarget.BeginningOfArc,
                };
                rfLine.DefinitionNodes.Add(n11.GetRFEM6ID());
                rfLine.DefinitionNodes.Add(n00.GetRFEM6ID());
                rfLine.ArcControlPoint = new Vector3d() { X = mid.X, Y = mid.Y, Z = mid.Z };

            }

            if (rfemLine.Curve is Circle c)
            {
                Vector normal = c.Normal;
                if (normal == null || BH.Engine.Geometry.Query.Length(normal) < 1e-6)
                    normal = new Vector { X = 0, Y = 0, Z = 1 };

                rfLine = new StructureCore.Line()
                {
                    No = rfemLine.GetRFEM6ID(),
                    Type = StructureCore.Line.Types.Type.Circle,
                    CircleRadius = c.Radius,
                    CircleCenter = new Vector3d { X = c.Centre.X, Y = c.Centre.Y, Z = c.Centre.Z },
                    CircleNormal = new Vector3d { X = normal.X, Y = normal.Y, Z = normal.Z },
                };
            }

            string comment = rfemLine.ToRFEM6Comment();
            if (comment != null)
                rfLine.Comment = comment;

            return rfLine;

        }

    }
}



