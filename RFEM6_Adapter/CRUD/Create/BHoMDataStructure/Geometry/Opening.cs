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

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Opening> bhOpenings)
        {
            HashSet<int> usedNumbers = UsedNumbers<StructureCore.Opening>();
            List<StructureCore.Opening> openings = new List<StructureCore.Opening>();
            Dictionary<string, int> existingOpenings = ExistingByBoundary(GetObjectList(new StructureCore.Opening()), o => o.No, o => o.BoundaryLines);

            foreach (Opening bhOpening in bhOpenings)
            {
                List<int> boundary = bhOpening.Edges.SelectMany(e => RFEM6EdgeLines.BoundaryLineNumbers(e)).Distinct().ToList();
                if (boundary.Count == 0 || boundary.Any(n => n <= 0))
                {
                    BH.Engine.Base.Compute.RecordError($"Opening {bhOpening.BHoM_Guid} has no valid boundary lines. Check that its edges are Line, Polyline, Arc or Circle and were created in RFEM6.");
                    return false;
                }

                if (existingOpenings.TryGetValue(BoundaryKey(boundary), out int existingNo))
                {
                    bhOpening.SetRFEM6ID(existingNo);
                    continue;
                }

                bhOpening.SetRFEM6ID(ClaimNumber(bhOpening.GetRFEM6ID(), usedNumbers));
                openings.Add(bhOpening.ToRFEM6());
            }

            CreateObjectList(openings);
            return true;
        }
    }
}
