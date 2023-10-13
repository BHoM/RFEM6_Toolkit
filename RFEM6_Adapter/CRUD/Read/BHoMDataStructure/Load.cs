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
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<ILoad> ReadBarLoad(List<string> ids = null)
        {
            //Find all possible Load cases
            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            List<int> loadCaseIds = loadCaseMap.Keys.ToList();
            Dictionary<int, Bar> memberMap = this.GetCachedOrReadAsDictionary<int, Bar>();


            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER_LOAD);
            
            IEnumerable<rfModel.member_load> foundLoadCases = numbers.ToList().Select(n => m_Model.get_member_load(n.no, n.children[0]));


            //m_Model.get_
            //List<String> lcName = new List<string>();
            List<ILoad> loadCases = new List<ILoad>();
            foreach (rfModel.member_load memberLoad in foundLoadCases)
            {

                memberLoad.FromRFEM(memberLoad.members.ToList().Select(m => memberMap[m]), loadCaseMap[memberLoad.load_case]);

            }

            return loadCases;
        }

    }
}
