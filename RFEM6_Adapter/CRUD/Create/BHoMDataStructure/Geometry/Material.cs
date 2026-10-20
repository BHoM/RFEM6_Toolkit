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

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<IMaterialFragment> materialFragments)
        {
            return QueueMaterials(materialFragments);
        }

        private bool UpdateObjects(IEnumerable<IMaterialFragment> materials)
        {
            return QueueMaterials(materials);
        }

        private bool QueueMaterials(IEnumerable<IMaterialFragment> materialFragments)
        {
            List<StructureCore.Material> existingMaterials = GetObjectList(new StructureCore.Material());
            HashSet<int> usedNumbers = new HashSet<int>(existingMaterials.Select(m => m.No).Where(n => n > 0));

            foreach (IMessage queued in m_createQueue)
            {
                if (queued is StructureCore.Material queuedMaterial && queuedMaterial.No > 0)
                    usedNumbers.Add(queuedMaterial.No);
            }

            foreach (IMaterialFragment bhMaterial in materialFragments)
            {
                string materialName = bhMaterial.ToRFEM6MaterialName();

                if (string.IsNullOrWhiteSpace(materialName))
                {
                    BH.Engine.Base.Compute.RecordError($"Cannot push material with empty name (type: {bhMaterial.GetType().Name}).");
                    return false;
                }

                StructureCore.Material existingMaterial = existingMaterials
                    .FirstOrDefault(m => MaterialNamesMatch(m.Name, materialName));

                if (existingMaterial != null)
                {
                    bhMaterial.SetRFEM6ID(existingMaterial.No);
                    continue;
                }

                int materialNo = bhMaterial.GetRFEM6ID();
                if (materialNo <= 0 || usedNumbers.Contains(materialNo))
                    materialNo = GetNextFreeNumber(usedNumbers);

                usedNumbers.Add(materialNo);
                bhMaterial.SetRFEM6ID(materialNo);

                // C# cantilever: Name only. Keep a local ID for sections in this push;
                // only send No when RFEM cannot default the new material to 1.
                StructureCore.Material rfMaterial = new StructureCore.Material { Name = materialName };
                if (materialNo != 1)
                    rfMaterial.No = materialNo;

                CreateObject(rfMaterial);
            }

            return true;
        }

        /// <summary>
        /// Number of the RFEM material with the same name, either in the model or queued in this push.
        /// </summary>
        private int FindMaterialNumber(IMaterialFragment material)
        {
            if (material == null)
                return 0;

            string materialName = material.ToRFEM6MaterialName();
            StructureCore.Material match = GetObjectList(new StructureCore.Material())
                .Concat(m_createQueue.OfType<StructureCore.Material>())
                .FirstOrDefault(m => m.No > 0 && MaterialNamesMatch(m.Name, materialName));

            if (match != null)
                return match.No;

            // A material queued with No=1 omits the number, so fall back to the id assigned in this push.
            return material.GetRFEM6ID();
        }

        private static bool MaterialNamesMatch(string rfemMaterialName, string bhMaterialName)
        {
            if (string.IsNullOrWhiteSpace(rfemMaterialName) || string.IsNullOrWhiteSpace(bhMaterialName))
                return false;

            return rfemMaterialName.Split('|')[0].Trim().Equals(bhMaterialName, StringComparison.OrdinalIgnoreCase);
        }

    }
}
