/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.Engine.Base;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static IMaterialFragment FromRFEM(this rfModel.material rfMaterial)
        {

            string s = rfMaterial.generating_object_info;
            IMaterialFragment bhMaterial = null;

            String[] matParaArray = rfMaterial.comment.Split('|');

            if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_STEEL) || rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_REINFORCING_STEEL))
            {

                bhMaterial = BH.Engine.Library.Query.Match("Steel", rfMaterial.name.Split('|')[0], true, true).DeepClone() as IMaterialFragment;

            }

            else if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_CONCRETE))
            {

                bhMaterial = BH.Engine.Library.Query.Match("Concrete", rfMaterial.name.Split('|')[0], true, true).DeepClone() as IMaterialFragment;

            }

            else if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_TIMBER))
            {

                
                // Check for "Timber" Dataset to check for material
                String timberType = rfMaterial.name.Substring(0, 2).ToLower()=="gl"? "Glulam": "SawnTimber";

                
                bhMaterial = BH.Engine.Library.Query.Match(timberType, rfMaterial.name.Split('|')[0], true, true) as IMaterialFragment;

            }


            if (bhMaterial == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"Material {rfMaterial.name} could not be read and will be generated as GenericIsotropicMaterial with all parameters set to 0!");
                bhMaterial = new BH.oM.Structure.MaterialFragments.GenericIsotropicMaterial { Name = rfMaterial.name, Density = 0, DampingRatio = 0, PoissonsRatio = 0, ThermalExpansionCoeff = 0, YoungsModulus = 0 };

            }
            bhMaterial.SetRFEM6ID(rfMaterial.no);
            return bhMaterial;
        }

    }
}

