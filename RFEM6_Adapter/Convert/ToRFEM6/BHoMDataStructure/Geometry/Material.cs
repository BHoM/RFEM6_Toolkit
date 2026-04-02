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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;


using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static rfModel.material ToRFEM6(this IMaterialFragment material)
        {
            rfModel.material_material_type materialType = GetMaterialType(material);
            string materialName = GetMaterialName(material);

            Object bhComment = "";
            if (material.CustomData.Count != 0)
            {
                material.CustomData.TryGetValue("Comment", out bhComment);
            }



            rfModel.material rfMaterial = new rfModel.material
            {
                no = material.GetRFEM6ID(),
                name = materialName,
                comment = (String)(bhComment==null||bhComment.Equals("") ? "" : $"BHComment:{bhComment}"),
                material_type = materialType
            };

            

            return rfMaterial;

        }


        private static rfModel.material_material_type GetMaterialType(IMaterialFragment material) {

            switch (material.GetType().Name)
            {
                case "Steel":
                    return rfModel.material_material_type.TYPE_STEEL;
                case "Concrete":
                    return rfModel.material_material_type.TYPE_CONCRETE;
                case "Timber":
                case "Glulam":
                    return rfModel.material_material_type.TYPE_TIMBER;
                default:
                    return rfModel.material_material_type.TYPE_STEEL;
            }
        
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
                    return material.Name.Replace(" ", ""); ;
                default:
                    return material.Name;
            }
        }   
    
    }
}



