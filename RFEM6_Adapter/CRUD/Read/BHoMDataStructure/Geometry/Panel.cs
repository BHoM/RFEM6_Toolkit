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

using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private List<Panel> ReadPanels(List<string> ids = null)
        {
            Dictionary<int, Edge> edges = this.GetCachedOrReadAsDictionary<int, Edge>();
            Dictionary<int, ISurfaceProperty> properties = this.GetCachedOrReadAsDictionary<int, ISurfaceProperty>();
            List<(Opening, List<int>)> openings = ReadOpeningsWithSurfaces();

            List<Panel> panels = new List<Panel>();
            foreach (StructureCore.Surface rfSurface in GetObjectList(new StructureCore.Surface()).Where(s => s.No > 0))
            {
                if (rfSurface.Type != StructureCore.Surface.Types.Type.Standard || rfSurface.Geometry != StructureCore.Surface.Types.Geometry.Plane)
                    BH.Engine.Base.Compute.RecordWarning($"Surface {rfSurface.No} is a {rfSurface.Geometry} {rfSurface.Type} surface. It is read as a plane panel with its boundary lines and thickness only.");

                properties.TryGetValue(rfSurface.Thickness, out ISurfaceProperty property);
                List<Opening> surfaceOpenings = openings.Where(o => o.Item2.Contains(rfSurface.No)).Select(o => o.Item1).ToList();

                panels.Add(rfSurface.FromRFEM(edges, property, surfaceOpenings));
            }

            return panels;
        }
    }
}
