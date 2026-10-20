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
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Base;
using BH.oM.Base;
using System.Text.RegularExpressions;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static ISectionProperty FromRFEM_Standardized_Steel(this StructureCore.CrossSection rfSection, List<IBHoMObject> bhSections, IMaterialFragment sectionMaterials)
        {
            String sectionName = new String((rfSection.Name.Where(c => !char.IsWhiteSpace(c)).ToArray())).Split()[0];
            sectionName = sectionName.Split('|')[0];
            sectionName = Regex.Replace(sectionName, @"[^a-zA-Z0-9]", "");

            Dictionary<int, HashSet<IBHoMObject>> scorsDict_test = new Dictionary<int, HashSet<IBHoMObject>>();

            foreach (var z in bhSections)
            {
                HashSet<IBHoMObject> value;
                int key = BH.Engine.Search.Compute.MatchScore(sectionName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", ""));
                if (scorsDict_test.TryGetValue(key, out value))
                {
                    scorsDict_test[key].Add((ISectionProperty)z);
                }
                else
                {
                    scorsDict_test[key] = new HashSet<IBHoMObject>() { z };
                }
            }

            var sortedSectNames_test = scorsDict_test.OrderByDescending(z => z.Key).ToDictionary(z => z.Key, z => z.Value);
            var result = sortedSectNames_test.Values.First().ToList()[0];

            if (sortedSectNames_test.Keys.First() < 85)
            {
                result = new ExplicitSection() { Name = rfSection.Name.Split('|')[0], Material = sectionMaterials };
                BH.Engine.Base.Compute.RecordWarning($"It is likely that the RFEM6 section {result.Name} has no corresponding element in the BHoM dataset. It will be read as ExplicitSection instead.");
            }

            result = result.DeepClone();
            ((ISectionProperty)result).Material = sectionMaterials;
            result.SetRFEM6ID(rfSection.No);
            return (ISectionProperty)result;
        }
    }
}



