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
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<RFEMHinge> rfemHinges)
        {
        
            foreach (RFEMHinge release in rfemHinges)
            {
                rfModel.member_hinge hinge=release.ToRFEM6();

                //rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER_HINGE);
                //List<rfModel.member_hinge> foundMemberHinges = numbers.ToList().Select(n => m_Model.get_member_hinge(n.no)).ToList();
                //var foundHinges = foundMemberHinges.Where(s => ComparerRFEMHingeAndBHoMConstraint(s, release.Constraint)).FirstOrDefault();
                //foundHinges.mem = foundHinges.lines.Append(rfLine.no).ToArray();
                ////m_Model.set_line_support(foundLineSupport);


                m_Model.set_member_hinge(hinge);
            }
            return true;
        }

        //private static bool ComparerRFEMHingeAndBHoMConstraint(rfModel.member_hinge rfMemberHinge, Constraint6DOF bhConstraint)
        //{

        //    if (!tranlate(rfMemberHinge.axial_release_n).Equals(bhConstraint.TranslationX)) { return false; }
        //    if (!tranlate(rfMemberHinge.axial_release_vy).Equals(bhConstraint.TranslationY)) { return false; }
        //    if (!tranlate(rfMemberHinge.axial_release_vz).Equals(bhConstraint.TranslationZ)) { return false; }
        //    if (!tranlate(rfMemberHinge.moment_release_mt).Equals(bhConstraint.RotationX)) { return false; }
        //    if (!tranlate(rfMemberHinge.moment_release_my).Equals(bhConstraint.RotationY)) { return false; }
        //    if (!tranlate(rfMemberHinge.moment_release_mz).Equals(bhConstraint.RotationZ)) { return false; }

        //    return true;
        //}


    }
}



