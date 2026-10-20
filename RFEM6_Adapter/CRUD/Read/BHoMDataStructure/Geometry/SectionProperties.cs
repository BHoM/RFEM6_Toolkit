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

            foreach (var section in allSections)
            {
                ISectionProperty bhSection;

                if (section.Type == StructureCore.CrossSection.Types.Type.StandardizedSteel)
                {
                    if (!materials.TryGetValue(section.Material, out IMaterialFragment sectionMaterials))
                        continue;

                    bhSection = section.FromRFEM_Standardized_Steel(sectionListLib, sectionMaterials);
                    bhSection.SetRFEM6ID(section.No);
                    sectionList.Add(bhSection);
                }
                else
                {
                    BH.Engine.Base.Compute.RecordWarning($"The section {section.Name} is not fully supported in v1 and will be read as ExplicitSection.");

                    if (!materials.TryGetValue(section.Material, out IMaterialFragment sectionMaterials))
                        continue;

                    bhSection = new ExplicitSection() { Name = section.Name, Material = sectionMaterials };
                    bhSection.SetRFEM6ID(section.No);
                    sectionList.Add(bhSection);
                }
            }

            sectionList = sectionList.OrderBy(x => x.GetRFEM6ID()).ToList();
            return sectionList;
        }
    }
}



