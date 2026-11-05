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

using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using StructureCore = Dlubal.Api.Rfem.StructureCore;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static Panel FromRFEM(
            this StructureCore.Surface rfSurface,
            Dictionary<int, Edge> edges,
            ISurfaceProperty surfaceProperty,
            List<Opening> openings)
        {
            List<Edge> externalEdges = new List<Edge>();

            foreach (int lineNo in rfSurface.BoundaryLines)
            {
                if (edges.TryGetValue(lineNo, out Edge edge) && edge != null)
                    externalEdges.Add(CloneEdgeForBoundary(edge));
                else
                    BH.Engine.Base.Compute.RecordWarning($"Surface {rfSurface.No} boundary references line {lineNo}, which was not read as an edge.");
            }

            Panel panel = new Panel
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Property = surfaceProperty,
            };

            panel.ExternalEdges.NormalisePlanarBoundaryEdges($"Surface {rfSurface.No}");

            panel.SetRFEM6ID(rfSurface.No);

            if (!string.IsNullOrWhiteSpace(rfSurface.Comment))
                panel.Name = rfSurface.Comment;

            return panel;
        }
    }
}
