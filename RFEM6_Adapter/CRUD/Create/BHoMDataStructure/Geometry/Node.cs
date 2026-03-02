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

using BH.Engine.Base;
using BH.Engine.Structure;
using BH.oM.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {

            //Read all support from RFEM model + Removint the added nodes
            HashSet<Constraint6DOF> constraints = this.GetCachedOrRead<RFEMNodalSupport>().Select(n => n.Constraint).ToHashSet(new Constraint6DOFComparer());
            constraints = constraints.Where(c => !(c.PropertyValue("NodeList") is null)).ToHashSet();

            //Create map of support and node list to be able to update supports with new nodes if support already exist
            Dictionary<Constraint6DOF, HashSet<int>> constraintToNodeMap = new Dictionary<Constraint6DOF, HashSet<int>>(new Constraint6DOFComparer());
            foreach (Constraint6DOF c in constraints)
            {
                constraintToNodeMap[c] = new HashSet<int>((List<int>)c.PropertyValue("NodeList"));
            }

            bool supportInBHNodes = false;

            foreach (Node bhNode in bhNodes)
            {
                rfModel.node rfNode = bhNode.ToRFEM6();

                m_Model.set_node(rfNode);

                if (bhNode.Support is null) { 
                    continue; 
                }
                else 
                {
                    
                    supportInBHNodes = true;

                    //if support already exist, add index to map and update support with new node list
                    constraintToNodeMap.TryGetValue(bhNode.Support, out HashSet<int> nodeList);

                    //if support does not exist, create new support and add to map and RFEM model
                    if (nodeList is null)
                    {

                        //Add node index to list and add support to map
                        nodeList = new HashSet<int>() { rfNode.no };

                        rfModel.nodal_support rfNodalSupport = bhNode.Support.ToRFEM6();
                        rfNodalSupport.nodes = nodeList.ToArray();
                        int no = m_Model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT, 0);
                        rfNodalSupport.no = no;
                        bhNode.Support.SetRFEM6ID(no);
                        constraintToNodeMap[bhNode.Support] = nodeList;
                        m_Model.set_nodal_support(rfNodalSupport);
                    }
                    else
                    {
                        constraintToNodeMap[bhNode.Support].Add(rfNode.no);
                        var comparer = new Constraint6DOFComparer();
                        Constraint6DOF found = constraintToNodeMap.Keys.Where(n => comparer.Equals(n, bhNode.Support)).First();
                        rfModel.nodal_support rfNodalSupport = m_Model.get_nodal_support(found.GetRFEM6ID());
                        rfNodalSupport.nodes = constraintToNodeMap[bhNode.Support].ToArray();
                        m_Model.set_nodal_support(rfNodalSupport);

                    }
                }
            }


            if (supportInBHNodes) BH.Engine.Base.Compute.RecordWarning(
@"Please check at least one of the nodes pushed to RFEM6 has a support assigned. At this stage this might result in duplicate nodes.
To clear the RFEM model please do the following:
Remove duplicates: Tools > Model Check > Identical Nodes
Renumbering: Tools > Renumber > Automatically"
);

            return true;
        }
    }
}



