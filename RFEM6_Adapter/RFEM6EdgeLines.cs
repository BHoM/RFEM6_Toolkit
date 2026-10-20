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

using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    /// <summary>
    /// A polyline edge is split into several RFEM lines. BHoM only allows one RFEMLine fragment per edge, so the full set is stored on CustomData during push.
    /// </summary>
    internal static class RFEM6EdgeLines
    {
        internal const string CustomDataKey = "RFEM6_EdgeLines";

        internal static void Set(Edge edge, IList<RFEMLine> lines)
        {
            if (edge == null || lines == null || lines.Count == 0)
                return;

            edge.CustomData[CustomDataKey] = lines;
            edge.Fragments.Add(lines[0]);
        }

        internal static List<RFEMLine> Get(Edge edge)
        {
            if (edge?.CustomData != null && edge.CustomData.TryGetValue(CustomDataKey, out object stored) && stored is IList<RFEMLine> list)
                return list.ToList();

            RFEMLine single = edge?.FindFragment<RFEMLine>();
            return single != null ? new List<RFEMLine> { single } : new List<RFEMLine>();
        }

        internal static IEnumerable<int> BoundaryLineNumbers(Edge edge)
        {
            List<int> numbers = Get(edge).Select(l => l.GetRFEM6ID()).Where(n => n > 0).ToList();
            if (numbers.Count > 0)
                return numbers;

            int edgeNo = edge?.GetRFEM6ID() ?? 0;
            return edgeNo > 0 ? new[] { edgeNo } : Enumerable.Empty<int>();
        }
    }
}
