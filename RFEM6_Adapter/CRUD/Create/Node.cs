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
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {

            //NOTE:A geometric object has, in general, a parent_no = 0. The parent_no parameter becomes significant for example with loads.
            int nodeId = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_NODE, 0);

            int counter = 0;

            for (int i = 0; i < bhNodes.Count(); i++)
            {
                Node bhNode = bhNodes.ToList()[i];

                rfModel.node rfNd= getNodeFromRFModel(bhNode);
               
                if (rfNd == null) {


                    rfModel.node rfNode = new rfModel.node()
                    {
                        no = nodeId + i -counter,
                        coordinates = new rfModel.vector_3d() { x = bhNode.Position.X, y = bhNode.Position.Y, z = bhNode.Position.Z },
                        coordinate_system_type = rfModel.node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "concrete part"
                    };
                    model.set_node(rfNode);

                }

                if (bhNode.Support!=null)
                {

                    int nodalSupportId = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT, 0);

                    rfModel.node constraintLocationNode = getNodeFromRFModel(bhNode);

                    rfModel.nodal_support existinConstraint = checkForExistenceOfConstraint(bhNode.Support);
                    if (existinConstraint == null)
                    {

                        rfModel.nodal_support rfNodelSuport = Convert.ToRFEM6(bhNode, constraintLocationNode.no, nodalSupportId);
                        model.set_nodal_support(rfNodelSuport);

                    }
                    else
                    {
                        rfModel.node rfNode = getNodeFromRFModel(bhNode);
                        List<int> constrrainNodesNo = existinConstraint.nodes.ToList();
                        constrrainNodesNo.Add(rfNode.no);
                        existinConstraint.nodes=constrrainNodesNo.ToArray();
                        model.delete_object(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT, existinConstraint.no, 0);
                        model.set_nodal_support(existinConstraint);
                    }
                }
            }

            return true;
        }

        public rfModel.node getNodeFromRFModel(Node bhNode)
        {

            rfModel.object_with_children[] numbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODE);
            IEnumerable<rfModel.node> foundNode=numbers.ToList().Select(n => model.get_node(n.no));

            IEnumerable<rfModel.node> collectedNode = foundNode.Where(n => (n.coordinate_1.Equals(bhNode.Position.X) && n.coordinate_2.Equals(bhNode.Position.Y) && n.coordinate_3.Equals(bhNode.Position.Z)));

            if (collectedNode.ToList().Count>0)
            {
                return collectedNode.ToList().First(); 
            }

            return null;
        }

        private rfModel.nodal_support checkForExistenceOfConstraint(BH.oM.Structure.Constraints.Constraint6DOF constraint)
        {

            rfModel.object_with_children[] supporNo = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
            IEnumerable<rfModel.nodal_support> supports = supporNo.ToList().Select(n => model.get_nodal_support(n.no));

            foreach (rfModel.nodal_support s in supports)
            {
               bool x = s.spring.x.Equals(Convert.stiffnessTranslationBHToRF("" + constraint.TranslationX));
               bool y = s.spring.y.Equals(Convert.stiffnessTranslationBHToRF("" + constraint.TranslationY));
               bool z = s.spring.z.Equals(Convert.stiffnessTranslationBHToRF("" + constraint.TranslationZ));
               bool xx = s.rotational_restraint.x.Equals(Convert.stiffnessTranslationBHToRF("" + constraint.RotationX));
               bool yy = s.rotational_restraint.y.Equals(Convert.stiffnessTranslationBHToRF("" + constraint.RotationY));
               bool zz = s.rotational_restraint.z.Equals(Convert.stiffnessTranslationBHToRF("" + constraint.RotationZ));

                if (x&&y&&z&&xx&&yy&&zz)
                {
                    return s;
                }

            }

            return null;
        }

    }
}
