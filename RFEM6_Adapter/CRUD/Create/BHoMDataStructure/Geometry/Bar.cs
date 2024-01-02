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
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Base;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Structure.Constraints;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Bar> bhBars)
        {
            foreach (Bar bhBar in bhBars)
            {

                rfModel.member rfMember = bhBar.ToRFEM6();

                if (bhBar.Release != null)
                {

                    rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER_HINGE);
                    List<rfModel.member_hinge> foundMemberHinges = numbers.ToList().Select(n => m_Model.get_member_hinge(n.no)).ToList();
                    var foundHinges0 = foundMemberHinges.Where(s => ComparerRFEMHingeAndBHoMConstraint(s, bhBar.Release.StartRelease)).FirstOrDefault();
                    rfMember.member_hinge_start = foundHinges0.no;
                    rfMember.member_hinge_startSpecified = true;
                    var foundHinges1 = foundMemberHinges.Where(s => ComparerRFEMHingeAndBHoMConstraint(s, bhBar.Release.EndRelease)).FirstOrDefault();
                    rfMember.member_hinge_end = foundHinges1.no;
                    rfMember.member_hinge_endSpecified = true;

                    //m_Model.set_line_support(foundLineSupport);
                }

                m_Model.set_member(rfMember);

            }

            return true;
        }

        private static bool ComparerRFEMHingeAndBHoMConstraint(rfModel.member_hinge rfMemberHinge, Constraint6DOF bhConstraint)
        {

            if (!Translate(rfMemberHinge.axial_release_n).Equals(bhConstraint.TranslationX))
                return false;
            if (!Translate(rfMemberHinge.axial_release_vy).Equals(bhConstraint.TranslationY))
                return false;
            if (!Translate(rfMemberHinge.axial_release_vz).Equals(bhConstraint.TranslationZ))
                return false;
            if (!Translate(rfMemberHinge.moment_release_mt).Equals(bhConstraint.RotationX))
                return false;
            if (!Translate(rfMemberHinge.moment_release_my).Equals(bhConstraint.RotationY)) 
                return false;
            if (!Translate(rfMemberHinge.moment_release_mz).Equals(bhConstraint.RotationZ))
                return false;

            return true;
        }

    }
}

