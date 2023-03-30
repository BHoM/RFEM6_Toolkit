﻿/*
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
using BH.oM.Structure.Constraints;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<RFEMLine> ReadLines(List<string> ids = null)
        {

            List<RFEMLine> lineList = new List<RFEMLine>();

            var lineNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
            var allRfLInes = lineNumbers.ToList().Select(n => m_Model.get_line(n.no));

            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();

            if (ids == null)
            {
                foreach (rfModel.line rfLine in allRfLInes)
                {

                    List<Node> lineNodes = new List<Node>();

                    if (rfLine.type is rfModel.line_type.TYPE_POLYLINE) {

                        Node node0;
                        nodes.TryGetValue(rfLine.definition_nodes[0], out node0);

                        Node node1;
                        nodes.TryGetValue(rfLine.definition_nodes[1], out node1);

                        lineNodes = new List<Node>() { node0, node1 };

                    }

                    else if (rfLine.type is rfModel.line_type.TYPE_ARC) 
                    {

                        Node n0;
                        nodes.TryGetValue(rfLine.definition_nodes[0], out n0);

                        Node n1;
                        nodes.TryGetValue(rfLine.definition_nodes[1], out n1);

                        lineNodes = new List<Node>() { n0, n1 };

                        //Point mid = Engine.Geometry.Create.Point(rfLine.arc_control_point_x, rfLine.arc_control_point_y, rfLine.arc_control_point_z);

                        //Arc arc = Engine.Geometry.Create.Arc(n0.Position, mid, n1.Position);

                    }

                    RFEMLine l = new RFEMLine { Nodes = lineNodes, LineType = (RFEMLineType)Convert.FromRFEM(rfLine.type)};
                    l.SetRFEM6ID(rfLine.no);
                    lineList.Add(l);
                }
            }

            return lineList;
        }

    }
}
