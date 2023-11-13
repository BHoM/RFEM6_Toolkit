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
using BH.Engine.Structure;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<ILoad> bhLoads)
        {
            ////Checking presence of GeometricalLineLoads and getting all line numbers if yes it is required to read all lines from RFEM6
            //NodeDistanceComparer nodeDistanceComparer = new NodeDistanceComparer(3);
            //Dictionary<Node, Dictionary<Node, int>> nestedNodeToIDMap = new Dictionary<Node, Dictionary<Node, int>>(nodeDistanceComparer);
            //List<rfModel.line> allLineNumbers = new List<rfModel.line>();
            
            ////If necessary fill the NodeToIDMap
            //if (bhLoads.Where(l => l is GeometricalLineLoad).ToList().Count() > 0)
            //{

            //    rfModel.object_with_children[] lineNumber = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
            //    allLineNumbers = lineNumber.Length > 1 ? lineNumber.ToList().Select(n => m_Model.get_line(n.no)).ToList().ToList() : new List<rfModel.line>();
                
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

            //}

            foreach (ILoad bhLoad in bhLoads)
            {

                if (bhLoad is BarUniformlyDistributedLoad)
                {

                    updateLoadIdDictionary(bhLoad);
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    member_load member_load = (bhLoad as BarUniformlyDistributedLoad).ToRFEM6(id);
                    var rfMemberLoad = member_load;
                    m_Model.set_member_load(bhLoad.Loadcase.GetRFEM6ID(), rfMemberLoad);
                }
                else if (bhLoad is PointLoad)
                {
                    nodal_load_load_type nodalLoadType= MomentOfForceLoad(bhLoad as PointLoad);
                    if (nodalLoadType == 0) continue;

                    updateLoadIdDictionary(bhLoad);
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    nodal_load rfPointLoad = (bhLoad as PointLoad).ToRFEM6(nodalLoadType,id);
                    m_Model.set_nodal_load(bhLoad.Loadcase.GetRFEM6ID(), rfPointLoad);


                }
                //else if (bhLoad is GeometricalLineLoad)
                //{
                //    Node n0 = new Node() { Position = (bhLoad as GeometricalLineLoad).Location.Start };
                //    Node n1 = new Node() { Position = (bhLoad as GeometricalLineLoad).Location.End };

                //    int lineNo = nestedNodeToIDMap[n0][n1];

                //    line_load rfLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(new List<int>() { lineNo });
                //    m_Model.set_line_load(bhLoad.Loadcase.GetRFEM6ID(), rfLineLoad);

                //}

                //else if (bhLoad is AreaUniformlyDistributedLoad) {

                //    surface_load rfemAreaLoad=(bhLoad as AreaUniformlyDistributedLoad).ToRFEM6();
                //    m_Model.set_surface_load(bhLoad.Loadcase.GetRFEM6ID(), rfemAreaLoad);

                //}

            }

            return true;
        }


        private nodal_load_load_type MomentOfForceLoad(PointLoad bhPointLoad) {


            bool momentHasBeenSet = !(bhPointLoad.Moment.X == 0 && bhPointLoad.Moment.Y == 0 && bhPointLoad.Moment.Z == 0);
            bool forceHasBeenSet = !(bhPointLoad.Force.X == 0 && bhPointLoad.Force.Y == 0 && bhPointLoad.Force.Z == 0);
            nodal_load_load_type nodalLoadType = nodal_load_load_type.LOAD_TYPE_FORCE;

            if (momentHasBeenSet && forceHasBeenSet)
            {

                BH.Engine.Base.Compute.RecordWarning($"The Point Load bh {bhPointLoad} does both include definitions for Moment Vector and Force Vector. Pleas Try to seperate those and push them individually!");
                return 0;
            }
            else if (momentHasBeenSet)
            {
                return  nodal_load_load_type.LOAD_TYPE_MOMENT;

            }
            else if (forceHasBeenSet)
            {
                return  nodal_load_load_type.LOAD_TYPE_FORCE;

            }
            else {

                return 0;
            }

            
        }

    }

}
