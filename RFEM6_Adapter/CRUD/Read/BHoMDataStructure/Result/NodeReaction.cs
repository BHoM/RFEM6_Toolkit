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
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Loads;
using Dlubal.WS.Rfem6.Model;
using System.Xml.Linq;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private String ReadNodeReaction(List<string> ids = null)
        {
            //IEnumerable<rfModel.load_case> foundLoadCases = numbers.ToList().Select(n => m_Model.get_load_case(n.no));
            rfModel.calculation_result result = m_Model.calculate_all(false);

            var nodeSupport0 = m_Model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, 1, 1);
            var nodeSupport1 = m_Model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, 1, 2);
            var nodeSupport2 = m_Model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, 1, 3);


            bool succeded = result.succeeded;

            rfModel.get_results_for_nodes_support_forcesResponse response;

            return null;
        }

    }
}

