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

using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Loadcase> bhLoadcases)
        {
            List<RfLoading.LoadCase> existingLoadCases = GetObjectList(new RfLoading.LoadCase());
            HashSet<int> usedNumbers = UsedNumbers<RfLoading.LoadCase>();
            int analysisSettingsNo = 0;

            // Numbered load cases first, so unnumbered ones cannot take their numbers.
            foreach (Loadcase loadcase in bhLoadcases.OrderBy(l => l.Number < 1))
            {
                RfLoading.LoadCase existing = loadcase.Number >= 1
                    ? existingLoadCases.FirstOrDefault(l => l.No == loadcase.Number)
                    : existingLoadCases.FirstOrDefault(l => !string.IsNullOrWhiteSpace(loadcase.Name) && l.Name == loadcase.Name);

                if (existing != null)
                {
                    loadcase.Number = existing.No;
                    loadcase.SetRFEM6ID(existing.No);
                    continue;
                }

                loadcase.Number = ClaimNumber(loadcase.Number, usedNumbers);
                loadcase.SetRFEM6ID(loadcase.Number);

                if (loadcase.Number == 1 && loadcase.Nature != LoadNature.Dead)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Load case {loadcase.Name} has number 1 but is not a dead load. RFEM6 expects load case 1 to be permanent, so its nature is changed to {LoadNature.Dead}.");
                    loadcase.Nature = LoadNature.Dead;
                }

                if (analysisSettingsNo <= 0)
                    analysisSettingsNo = GetOrQueueLinearStaticAnalysisSettings();

                CreateObject(loadcase.ToRFEM6(analysisSettingsNo));
            }

            return true;
        }

        private int GetOrQueueLinearStaticAnalysisSettings()
        {
            RfLoading.StaticAnalysisSettings settings = GetObjectList(new RfLoading.StaticAnalysisSettings())
                .Concat(m_createQueue.OfType<RfLoading.StaticAnalysisSettings>())
                .FirstOrDefault(s => s.AnalysisType == RfLoading.StaticAnalysisSettings.Types.AnalysisType.GeometricallyLinear);

            if (settings == null)
            {
                settings = new RfLoading.StaticAnalysisSettings
                {
                    No = ClaimNumber(0, UsedNumbers<RfLoading.StaticAnalysisSettings>()),
                    AnalysisType = RfLoading.StaticAnalysisSettings.Types.AnalysisType.GeometricallyLinear,
                };
                CreateObject(settings);
            }

            return settings.No;
        }
    }
}
