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
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Bar> ReadBars(List<string> ids = null)
        {

            List<Bar> barList = new List<Bar>();

            var barNumber = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER);

            List<rfModel.member> allRfMembers = new List<rfModel.member>();

            foreach (var n in barNumber)
            {

                allRfMembers.Add(m_Model.get_member(n.no));

            }

            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();
            Dictionary<int, RFEMLine> lines = this.GetCachedOrReadAsDictionary<int, RFEMLine>();
            Dictionary<int, ISectionProperty> sections = this.GetCachedOrReadAsDictionary<int, ISectionProperty>();
            Dictionary<int, RFEMHinge> hinges = this.GetCachedOrReadAsDictionary<int, RFEMHinge>();


            foreach (var rfMember in allRfMembers)
            {

                Node node0 = null;
                nodes.TryGetValue(rfMember.nodes[0], out node0);

                Node node1 = null;
                nodes.TryGetValue(rfMember.nodes[1], out node1);

                ISectionProperty section = null;
                sections.TryGetValue(rfMember.section_end, out section);

                RFEMHinge hingeStart = null;
                RFEMHinge hingeEnd = null;
                BarRelease barRelease = new BarRelease();

                hinges.TryGetValue(rfMember.member_hinge_start, out hingeStart);
                hinges.TryGetValue(rfMember.member_hinge_end, out hingeEnd);
                
                if (hingeStart != null) { barRelease.StartRelease = hingeStart.Constraint; }
                if (hingeEnd != null) { barRelease.EndRelease = hingeEnd.Constraint; }



                Bar bhBar = rfMember.FromRFEM(node0, node1, section);
                bhBar.Release = barRelease;

                barList.Add(bhBar);

            }

            return barList;
        }

    }
}



