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

using BH.oM.Base;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static LoadCombination FromRFEM(this RfLoading.LoadCombination rfCombination, Dictionary<int, Loadcase> loadcases)
        {
            List<Tuple<double, ICase>> factoredCases = new List<Tuple<double, ICase>>();

            foreach (RfLoading.LoadCombination.Types.ItemsRow item in rfCombination.Items?.Rows ?? Enumerable.Empty<RfLoading.LoadCombination.Types.ItemsRow>())
            {
                if (loadcases.TryGetValue(item.LoadCase, out Loadcase loadcase))
                    factoredCases.Add(new Tuple<double, ICase>(item.Factor, loadcase));
                else
                    BH.Engine.Base.Compute.RecordWarning($"Load combination {rfCombination.No} references load case {item.LoadCase}, which could not be read. It is left out of the combination.");
            }

            LoadCombination bhCombination = new LoadCombination
            {
                Name = rfCombination.Name,
                Number = rfCombination.No,
                LoadCases = factoredCases,
            };
            bhCombination.SetRFEM6ID(rfCombination.No);
            return bhCombination;
        }
    }
}
