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
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {
            //NOTE:A geometric object has, in general, a parent_no = 0. The parent_no parameter becomes significant for example with loads.
            foreach (Node bhNode in bhNodes)
            {
                rfModel.node rfNode = bhNode.ToRFEM6();
                m_Model.set_node(rfNode);

                if (bhNode.Support != null)
                {
                    rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
                    List<rfModel.nodal_support> foundSupports = numbers.ToList().Select(n => m_Model.get_nodal_support(n.no)).ToList();
                    var foundRFNodalSupport = foundSupports.Where(s => ComparerRFEMSupportAndBHoMConstraint(s, bhNode.Support)).FirstOrDefault();
                    foundRFNodalSupport.nodes = foundRFNodalSupport.nodes.Append(rfNode.no).ToArray();
                    m_Model.set_nodal_support(foundRFNodalSupport);

                }

            }
            return true;
        }


        private static bool ComparerRFEMSupportAndBHoMConstraint(rfModel.nodal_support rfSupport, Constraint6DOF bhConstraint)
        {

            if (!Translate(rfSupport.spring_x).Equals(bhConstraint.TranslationX)) 
                return false;
            if (!Translate(rfSupport.spring_y).Equals(bhConstraint.TranslationY)) 
                return false;
            if (!Translate(rfSupport.spring_z).Equals(bhConstraint.TranslationZ)) 
                return false;
            if (!Translate(rfSupport.rotational_restraint_x).Equals(bhConstraint.RotationX)) 
                return false;
            if (!Translate(rfSupport.rotational_restraint_y).Equals(bhConstraint.RotationY)) 
                return false;
            if (!Translate(rfSupport.rotational_restraint_z).Equals(bhConstraint.RotationZ)) 
                return false;

            return true;
        }

        private static DOFType Translate(double input)
        {
            if (input.Equals(double.PositiveInfinity))
                return DOFType.Fixed;
            else return
                    DOFType.Free;
        }
    }
}
