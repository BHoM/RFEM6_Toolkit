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

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Loads;
using BH.Engine.Structure;
using BH.oM.Geometry;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Update Node                               ****/
        /***************************************************/

        private bool UpdateObjects(IEnumerable<ILoad> loads)
        {
            bool success = true;

            foreach (ILoad bhLoad in loads)
            {

                if (bhLoad is BarUniformlyDistributedLoad)
                {
                    //var rfMemberLoad = (bhLoad as BarUniformlyDistributedLoad).ToRFEM6();
                    //m_Model.set_member_load(bhLoad.Loadcase.GetRFEM6ID(), rfMemberLoad);
                }
                else if (bhLoad is PointLoad)
                {
                    nodal_load_load_type nodalLoadType = (nodal_load_load_type)MomentOrForceLoad(bhLoad as PointLoad);
                    if (nodalLoadType == 0) continue;

                    //var rfPointLoad = (bhLoad as PointLoad).ToRFEM6(nodalLoadType);
                    //m_Model.set_nodal_load(bhLoad.Loadcase.GetRFEM6ID(), rfPointLoad);

                }
                //else if (bhLoad is GeometricalLineLoad)
                //{
                //    NodeDistanceComparer nodeDistanceComparer = new NodeDistanceComparer();
                //    Dictionary<Node, Dictionary<Node, int>> nestedNodeToIDMap = new Dictionary<Node, Dictionary<Node, int>>();
                //    Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
                //    rfModel.object_with_children[] lineNumber = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
                //    var allLineNumbers = lineNumber.Length > 1 ? lineNumber.ToList().Select(n => m_Model.get_line(n.no)).ToList().ToList() : new List<rfModel.line>();

                //    foreach (rfModel.line l in allLineNumbers)
                //    {

                //        Node n0 = m_Model.get_node(l.definition_nodes[0]).FromRFEM();
                //        Node n1 = m_Model.get_node(l.definition_nodes[1]).FromRFEM();

                //        if (!nestedNodeToIDMap.ContainsKey(n0))
                //        {
                //            Dictionary<Node, int> innterDictionary = new Dictionary<Node, int>(nodeDistanceComparer);
                //            innterDictionary.Add(n1, l.no);
                //            nestedNodeToIDMap.Add(n0, innterDictionary);
                //        }
                //        else
                //        {
                //            nestedNodeToIDMap[n0].Add(n1, l.no);
                //        }
                //    }

                //    Line a=(bhLoad as GeometricalLineLoad).Location;

                //    var rfPointLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(new List<int> { nestedNodeToIDMap[new Node() { Position = a.Start }][new Node() { Position = a.End }] });
                //    m_Model.set_line_load(bhLoad.Loadcase.GetRFEM6ID(), rfPointLoad);

                //}
            }

            return success;
        }

    }
}



