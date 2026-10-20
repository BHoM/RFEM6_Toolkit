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
        private bool CreateCollection(IEnumerable<Panel> bhPanels)
        {
            HashSet<int> usedNumbers = UsedNumbers<StructureCore.Surface>();
            List<StructureCore.Surface> surfaces = new List<StructureCore.Surface>();
            Dictionary<string, int> existingSurfaces = ExistingByBoundary(GetObjectList(new StructureCore.Surface()), s => s.No, s => s.BoundaryLines);

            foreach (Panel bhPanel in bhPanels)
            {
                if (bhPanel.Property == null)
                {
                    BH.Engine.Base.Compute.RecordError($"Panel {bhPanel.BHoM_Guid} has no surface property, so it cannot be pushed to RFEM6.");
                    return false;
                }

                if (bhPanel.Property.GetRFEM6ID() <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"The surface property '{bhPanel.Property.Name}' of panel {bhPanel.BHoM_Guid} was not created in RFEM6, so the panel cannot be pushed.");
                    return false;
                }

                List<int> boundary = bhPanel.ExternalEdges.SelectMany(e => RFEM6EdgeLines.BoundaryLineNumbers(e)).Distinct().ToList();
                if (boundary.Count == 0 || boundary.Any(n => n <= 0))
                {
                    BH.Engine.Base.Compute.RecordError($"Panel {bhPanel.BHoM_Guid} has no valid external boundary lines. Check that its edges are Line, Polyline, Arc or Circle and were created in RFEM6.");
                    return false;
                }

                if (existingSurfaces.TryGetValue(BoundaryKey(boundary), out int existingNo))
                {
                    bhPanel.SetRFEM6ID(existingNo);
                    LinkOpeningsToSurface(bhPanel.Openings, existingNo);
                    continue;
                }

                int surfaceNo = ClaimNumber(bhPanel.GetRFEM6ID(), usedNumbers);
                bhPanel.SetRFEM6ID(surfaceNo);
                LinkOpeningsToSurface(bhPanel.Openings, surfaceNo);
                surfaces.Add(bhPanel.ToRFEM6());
            }

            CreateObjectList(surfaces);
            return true;
        }

        /// <summary>
        /// Numbers of existing RFEM objects keyed by their set of boundary lines, so a push does not create a second object on the same lines.
        /// </summary>
        private static Dictionary<string, int> ExistingByBoundary<T>(IEnumerable<T> objects, System.Func<T, int> number, System.Func<T, IEnumerable<int>> boundaryLines)
        {
            Dictionary<string, int> existing = new Dictionary<string, int>();
            foreach (T obj in objects)
            {
                string key = BoundaryKey(boundaryLines(obj));
                if (number(obj) > 0 && key.Length > 0 && !existing.ContainsKey(key))
                    existing[key] = number(obj);
            }
            return existing;
        }

        private static string BoundaryKey(IEnumerable<int> lineNos)
        {
            return string.Join(",", lineNos.Where(n => n > 0).Distinct().OrderBy(n => n));
        }

        /// <summary>
        /// Openings are created before panels but must reference the parent surface number in the same create batch.
        /// </summary>
        private void LinkOpeningsToSurface(IEnumerable<Opening> openings, int surfaceNo)
        {
            if (openings == null || surfaceNo <= 0)
                return;

            foreach (Opening bhOpening in openings)
            {
                int openingNo = bhOpening.GetRFEM6ID();
                if (openingNo <= 0)
                    continue;

                foreach (StructureCore.Opening rfOpening in m_createQueue.OfType<StructureCore.Opening>().Where(o => o.No == openingNo))
                {
                    if (!rfOpening.Surfaces.Contains(surfaceNo))
                        rfOpening.Surfaces.Add(surfaceNo);
                }
            }
        }
    }
}
