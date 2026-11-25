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
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        // Edges are created in RFEM as the lines produced by GetLineFromEdgeModule; an edge only takes over its line number.
        private bool CreateCollection(IEnumerable<Edge> edges)
        {
            bool success = true;

            foreach (Edge edge in edges)
            {
                List<RFEMLine> lines = RFEM6EdgeLines.Get(edge);
                if (lines.Count == 0 || lines.Any(l => l.GetRFEM6ID() <= 0))
                {
                    BH.Engine.Base.Compute.RecordError($"Edge {edge.BHoM_Guid} has no RFEM line. Only edges with Line, Polyline, Arc or Circle curves can be pushed.");
                    success = false;
                    continue;
                }

                edge.SetRFEM6ID(lines[0].GetRFEM6ID());

                if (edge.Support != null)
                    success &= QueueEdgeLineSupports(edge, edge.Support);

                if (edge.Release != null && !edge.Release.IsFullyFixedLineRelease())
                    success &= QueueEdgeLineReleases(edge, edge.Release);
            }

            return success;
        }
    }
}
