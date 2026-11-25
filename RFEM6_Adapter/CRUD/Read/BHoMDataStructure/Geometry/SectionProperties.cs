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
using BH.oM.Spatial.ShapeProfiles;

using BH.Engine.Base;
using BH.oM.Base;
using System.Text.RegularExpressions;
using StructureCore = Dlubal.Api.Rfem.StructureCore;


namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<ISectionProperty> sectionList = new List<ISectionProperty>();

            List<IBHoMObject> sectionListLib = BH.Engine.Library.Query.Library("Structure\\SectionProperties").ToList();
            Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();
            var allSections = GetObjectList(new StructureCore.CrossSection());
            List<string> guessedMaterialSections = new List<string>();

            foreach (var section in allSections)
            {
                ISectionProperty bhSection;

                if (!materials.TryGetValue(section.Material, out IMaterialFragment sectionMaterialsForParametric))
                {
                    sectionMaterialsForParametric = GuessSectionMaterial(section, materials.Values);
                    guessedMaterialSections.Add(section.Name);
                }

                ISectionProperty parametricSection = Convert.TryCreateSectionFromParametricRfemName(section.Name, sectionMaterialsForParametric);
                if (parametricSection != null)
                {
                    parametricSection.SetRFEM6ID(section.No);
                    sectionList.Add(parametricSection);
                    continue;
                }

                if (section.Type == StructureCore.CrossSection.Types.Type.StandardizedSteel)
                {
                    bhSection = section.FromRFEM_Standardized_Steel(sectionListLib, sectionMaterialsForParametric);
                    bhSection.SetRFEM6ID(section.No);
                    sectionList.Add(bhSection);
                }
                else
                {
                    BH.Engine.Base.Compute.RecordWarning($"The section {section.Name} is not fully supported in v1 and will be read as ExplicitSection.");

                    bhSection = new ExplicitSection() { Name = section.Name, Material = sectionMaterialsForParametric };
                    bhSection.SetRFEM6ID(section.No);
                    sectionList.Add(bhSection);
                }
            }

            if (guessedMaterialSections.Count > 0)
                BH.Engine.Base.Compute.RecordWarning($"RFEM6 did not report the material of {guessedMaterialSections.Count} cross-section(s), which happens when the RFEM version does not match the Dlubal API version of the adapter (RFEM 6.15). Their material was guessed from the section type: {string.Join(", ", guessedMaterialSections.Distinct())}.");

            sectionList = sectionList.OrderBy(x => x.GetRFEM6ID()).ToList();
            return sectionList;
        }

        private static IMaterialFragment GuessSectionMaterial(StructureCore.CrossSection section, IEnumerable<IMaterialFragment> materials)
        {
            bool isSteelSection = section.Type == StructureCore.CrossSection.Types.Type.StandardizedSteel
                || section.Type == StructureCore.CrossSection.Types.Type.ParametricThinWalled;

            IMaterialFragment guess = isSteelSection
                ? materials.FirstOrDefault(m => m is Steel)
                : materials.FirstOrDefault(m => m is Concrete);

            return guess ?? materials.FirstOrDefault();
        }
    }
}



