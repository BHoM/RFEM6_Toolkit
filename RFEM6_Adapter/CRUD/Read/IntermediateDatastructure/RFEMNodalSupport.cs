/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;
using Newtonsoft.Json;
using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<RFEMNodalSupport> ReadRFEMNodalSupports(List<string> ids = null)
        {

            List<RFEMNodalSupport> constraintList = new List<RFEMNodalSupport>();

            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
            IEnumerable<rfModel.nodal_support> foundSupports = numbers.ToList().Select(n => m_Model.get_nodal_support(n.no));

            foreach (rfModel.nodal_support s in foundSupports)
            {
                RFEMNodalSupport rfConstraint = Convert.FromRFEM(s);
                constraintList.Add(rfConstraint);
            }

  
            

            return constraintList;
        }



    }
}


