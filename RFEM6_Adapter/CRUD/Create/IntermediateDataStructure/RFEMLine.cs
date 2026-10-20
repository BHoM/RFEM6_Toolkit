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
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

using System.Security.Cryptography;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using Dlubal.Api.Common;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<RFEMLine> rfemLines)
        {
            return QueueLines(rfemLines);
        }

        private bool UpdateObjects(IEnumerable<RFEMLine> rfemLines)
        {
            return QueueLines(rfemLines);
        }

        private bool QueueLines(IEnumerable<RFEMLine> rfemLines)
        {
            HashSet<int> usedNumbers = UsedNumbers<StructureCore.Line>();
            List<StructureCore.Line> linesToCreate = new List<StructureCore.Line>();

            // Lines already in the model with the same definition (nodes, arc control point or circle) are reused rather than duplicated.
            Dictionary<string, int> existingLines = new Dictionary<string, int>();
            foreach (StructureCore.Line rfLine in GetObjectList(new StructureCore.Line()).Where(l => l.No > 0))
            {
                string key = LineKey(rfLine);
                if (key != null && !existingLines.ContainsKey(key))
                    existingLines[key] = rfLine.No;
            }

            foreach (RFEMLine rfemLine in rfemLines)
            {
                StructureCore.Line rfLine = rfemLine.ToRFEM6();
                if (rfLine.Type != StructureCore.Line.Types.Type.Circle
                    && (rfLine.DefinitionNodes.Count < 2 || rfLine.DefinitionNodes.Any(n => n <= 0)))
                {
                    BH.Engine.Base.Compute.RecordError($"RFEM line {rfemLine.GetRFEM6ID()} has no valid geometry. Only Line, Polyline, Arc and Circle edges can be pushed.");
                    return false;
                }

                string key = LineKey(rfLine);
                if (key != null && existingLines.TryGetValue(key, out int existingNo))
                {
                    rfemLine.SetRFEM6ID(existingNo);
                    continue;
                }

                int lineNo = ClaimNumber(rfemLine.GetRFEM6ID(), usedNumbers);
                rfemLine.SetRFEM6ID(lineNo);
                rfLine.No = lineNo;
                if (key != null)
                    existingLines[key] = lineNo;

                linesToCreate.Add(rfLine);
            }

            CreateObjectList(linesToCreate);
            return true;
        }

        /// <summary>
        /// Direction-independent key of an RFEM line definition. Coordinates are rounded to the millimetre, matching the node position key.
        /// </summary>
        private static string LineKey(StructureCore.Line rfLine)
        {
            List<int> nodes = rfLine.DefinitionNodes.ToList();

            switch (rfLine.Type)
            {
                case StructureCore.Line.Types.Type.Polyline when nodes.Count >= 2:
                    string forward = string.Join(",", nodes);
                    nodes.Reverse();
                    string reverse = string.Join(",", nodes);
                    return "P:" + (string.CompareOrdinal(forward, reverse) <= 0 ? forward : reverse);

                case StructureCore.Line.Types.Type.Arc when nodes.Count >= 2 && rfLine.ArcControlPoint != null:
                    Vector3d cp = rfLine.ArcControlPoint;
                    return $"A:{Math.Min(nodes[0], nodes[1])},{Math.Max(nodes[0], nodes[1])}:{PositionKey(cp.X, cp.Y, cp.Z)}";

                case StructureCore.Line.Types.Type.Circle when rfLine.CircleCenter != null:
                    Vector3d c = rfLine.CircleCenter;
                    return $"C:{PositionKey(c.X, c.Y, c.Z)}:{Math.Round(rfLine.CircleRadius, 3):F3}:{CircleNormalKey(rfLine.CircleNormal)}";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Unit normal with a canonical sign, since a flipped normal describes the same circle.
        /// </summary>
        private static string CircleNormalKey(Vector3d normal)
        {
            double x = normal?.X ?? 0, y = normal?.Y ?? 0, z = normal?.Z ?? 1;
            double length = Math.Sqrt(x * x + y * y + z * z);
            if (length < 1e-9)
                return "0,0,1";

            x /= length; y /= length; z /= length;
            double first = Math.Abs(x) > 1e-6 ? x : Math.Abs(y) > 1e-6 ? y : z;
            if (first < 0)
            {
                x = -x; y = -y; z = -z;
            }

            return $"{Math.Round(x, 3) + 0.0:F3},{Math.Round(y, 3) + 0.0:F3},{Math.Round(z, 3) + 0.0:F3}";
        }
    }
}
