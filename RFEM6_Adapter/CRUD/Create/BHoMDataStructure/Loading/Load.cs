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
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Loads;
using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;
using System.Security.Cryptography;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<ILoad> bhLoads)
        {
            //Checking presence of GeometricalLineLoads and getting all line numbers
            List<rfModel.line> allLineNumbers = new List<rfModel.line>();
            if (bhLoads.Where(l => l is GeometricalLineLoad).ToList().Count() > 0)
            {
                rfModel.object_with_children[] lineNumber = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
                allLineNumbers = lineNumber.Length > 1 ? lineNumber.ToList().Select(n => m_Model.get_line(n.no)).ToList().ToList() : new List<rfModel.line>();
            }

            foreach (ILoad bhLoad in bhLoads)
            {

                if (bhLoad is BarUniformlyDistributedLoad)
                {
                    var rfMemberLoad = (bhLoad as BarUniformlyDistributedLoad).ToRFEM6();
                    m_Model.set_member_load(bhLoad.Loadcase.GetRFEM6ID(), rfMemberLoad);
                }
                else if (bhLoad is PointLoad)
                {

                    var rfPointLoad = (bhLoad as PointLoad).ToRFEM6();
                    m_Model.set_nodal_load(bhLoad.Loadcase.GetRFEM6ID(), rfPointLoad);


                }
                else if (bhLoad is GeometricalLineLoad)
                {


                    Line loadedline= (bhLoad as GeometricalLineLoad).Location;
                    

                    var rfLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6();
                    m_Model.set_nodal_load(bhLoad.Loadcase.GetRFEM6ID(), rfLineLoad);


                }
            }

            return true;
        }

    }
}
