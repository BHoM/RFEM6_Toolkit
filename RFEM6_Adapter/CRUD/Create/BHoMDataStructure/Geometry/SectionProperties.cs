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

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<ISectionProperty> sectionProperties)
        {
            return QueueSections(sectionProperties);
        }

        private bool UpdateObjects(IEnumerable<ISectionProperty> sections)
        {
            return QueueSections(sections);
        }

        private bool QueueSections(IEnumerable<ISectionProperty> sectionProperties)
        {
            List<StructureCore.CrossSection> existingSections = GetObjectList(new StructureCore.CrossSection());
            HashSet<int> usedNumbers = new HashSet<int>(existingSections.Select(s => s.No).Where(n => n > 0));
            List<StructureCore.CrossSection> sectionsToCreate = new List<StructureCore.CrossSection>();

            foreach (IMessage queued in m_createQueue)
            {
                if (queued is StructureCore.CrossSection queuedSection && queuedSection.No > 0)
                    usedNumbers.Add(queuedSection.No);
            }

            bool success = true;

            foreach (ISectionProperty section in sectionProperties)
            {
                string sectionName = section.ToRFEM6SectionName();
                if (string.IsNullOrWhiteSpace(sectionName))
                {
                    BH.Engine.Base.Compute.RecordError($"Cannot push section of type {section.GetType().Name}: it has no Name and no dimensions on the section or its SectionProfile/Profile that the adapter can turn into an RFEM name (R_M1 or CIRCLE_M1).");
                    success = false;
                    continue;
                }

                StructureCore.CrossSection existingSection = existingSections
                    .FirstOrDefault(s => SectionNamesMatch(s.Name, sectionName));

                if (existingSection != null)
                {
                    section.SetRFEM6ID(existingSection.No);
                    continue;
                }

                int matNo = FindMaterialNumber(section.Material);
                if (matNo <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Cannot create section '{sectionName}' because material '{section.Material.ToRFEM6MaterialName()}' is not in RFEM6 and was not queued in this push.");
                    success = false;
                    continue;
                }

                int secNo = section.GetRFEM6ID();
                if (secNo <= 0 || usedNumbers.Contains(secNo))
                    secNo = GetNextFreeNumber(usedNumbers);

                usedNumbers.Add(secNo);
                section.SetRFEM6ID(secNo);
                section.Name = sectionName;
                sectionsToCreate.Add(section.ToRFEM6(matNo, section.Material.GetType().Name));
            }

            CreateObjectList(sectionsToCreate);
            return success;
        }

        /// <summary>
        /// Number of the RFEM cross-section for this BHoM section, from its fragment, the model snapshot, or the create queue.
        /// </summary>
        private int FindSectionNumber(ISectionProperty section)
        {
            if (section == null)
                return 0;

            int id = section.GetRFEM6ID();
            if (id > 0)
                return id;

            string sectionName = section.ToRFEM6SectionName();
            if (string.IsNullOrWhiteSpace(sectionName))
                return 0;

            StructureCore.CrossSection match = GetObjectList(new StructureCore.CrossSection())
                .Concat(m_createQueue.OfType<StructureCore.CrossSection>())
                .FirstOrDefault(s => s.No > 0 && SectionNamesMatch(s.Name, sectionName));

            return match?.No ?? 0;
        }

        private static bool SectionNamesMatch(string rfemSectionName, string bhSectionName)
        {
            if (string.IsNullOrWhiteSpace(rfemSectionName) || string.IsNullOrWhiteSpace(bhSectionName))
                return false;

            return rfemSectionName.Split('|')[0].Trim().Equals(bhSectionName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
