/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<IMaterialFragment> ReadMaterial(List<string> ids = null)
        {


            List<IMaterialFragment> materialList = new List<IMaterialFragment>();
            rfModel.object_with_children[] materialsNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
            List<rfModel.material> allMaterials = new List<rfModel.material>();

            foreach (var n in materialsNumbers)
            {

                allMaterials.Add(m_Model.get_material(n.no));

            }


            if (ids == null)
            {

                foreach (var rfMaterial in allMaterials)
                {

                    IMaterialFragment material = rfMaterial.FromRFEM();
                    if (material != null)
                    {

                        materialList.Add(material);

                    }

                }

            }

            return materialList;
        }

    }
}
