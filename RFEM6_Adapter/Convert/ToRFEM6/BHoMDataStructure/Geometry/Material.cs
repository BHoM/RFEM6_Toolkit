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

using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Base;
using BH.oM.Structure.MaterialFragments;


namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static string ToRFEM6MaterialName(this IMaterialFragment material)
        {
            return GetMaterialName(material);
        }

        public static StructureCore.Material ToRFEM6(this IMaterialFragment material, int materialNo = 0)
        {
            string materialName = GetMaterialName(material);

            StructureCore.Material rfMaterial = new StructureCore.Material
            {
                Name = materialName,
            };

            if (materialNo > 0)
                rfMaterial.No = materialNo;

            if (material is IBHoMObject bhomMaterial)
            {
                string comment = bhomMaterial.ToRFEM6Comment();
                if (!string.IsNullOrWhiteSpace(comment))
                    rfMaterial.Comment = comment;
            }

            return rfMaterial;
        }
        
        private static String GetMaterialName(IMaterialFragment material)
        {
            switch (material.GetType().Name)
            {
                case "Steel":
                case "Concrete":
                    return material.Name;
                case "Timber":
                case "Glulam":
                    return material.Name.Replace(" ", "");
                default:
                    return material.Name;
            }
        }   
    
    }
}



