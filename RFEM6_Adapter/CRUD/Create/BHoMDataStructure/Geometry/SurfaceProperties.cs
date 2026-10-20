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

using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<ISurfaceProperty> surfaceProperties)
        {
            HashSet<int> usedNumbers = UsedNumbers<StructureCore.Thickness>();
            List<StructureCore.Thickness> existingThicknesses = GetObjectList(new StructureCore.Thickness());
            List<StructureCore.Thickness> thicknessesToCreate = new List<StructureCore.Thickness>();

            foreach (ISurfaceProperty surfaceProperty in surfaceProperties)
            {
                int matNo = FindMaterialNumber(surfaceProperty.Material);
                if (matNo <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Cannot create surface property '{surfaceProperty.Name}' because its material is not in RFEM6 and was not queued in this push.");
                    return false;
                }

                double thickness = BH.Engine.Structure.Query.ITotalThickness(surfaceProperty);
                StructureCore.Thickness existing = existingThicknesses.Concat(thicknessesToCreate).FirstOrDefault(t =>
                    t.Type == StructureCore.Thickness.Types.Type.Uniform
                    && t.Material == matNo
                    && Math.Abs(t.UniformThickness - thickness) < 1e-6
                    && (string.IsNullOrWhiteSpace(surfaceProperty.Name) || string.Equals(t.Name, surfaceProperty.Name, StringComparison.OrdinalIgnoreCase)));

                if (existing != null)
                {
                    surfaceProperty.SetRFEM6ID(existing.No);
                    continue;
                }

                surfaceProperty.SetRFEM6ID(ClaimNumber(surfaceProperty.GetRFEM6ID(), usedNumbers));
                thicknessesToCreate.Add(surfaceProperty.ToRFEM6(matNo));
            }

            CreateObjectList(thicknessesToCreate);
            return true;
        }
    }
}
