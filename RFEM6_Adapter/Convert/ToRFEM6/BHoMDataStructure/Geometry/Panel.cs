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
using System.Linq;

using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static StructureCore.Surface ToRFEM6(this Panel bhPanel)
        {
            StructureCore.Surface rfSurface = new StructureCore.Surface
            {
                No = bhPanel.GetRFEM6ID(),
                Type = StructureCore.Surface.Types.Type.Standard,
                Geometry = StructureCore.Surface.Types.Geometry.Plane,
                Thickness = bhPanel.Property.GetRFEM6ID(),
            };
            foreach (int lineNo in bhPanel.ExternalEdges.SelectMany(e => RFEM6EdgeLines.BoundaryLineNumbers(e)).Distinct())
                rfSurface.BoundaryLines.Add(lineNo);

            string comment = bhPanel.ToRFEM6Comment();
            if (comment != null)
                rfSurface.Comment = comment;

            return rfSurface;
        }
    }
}
