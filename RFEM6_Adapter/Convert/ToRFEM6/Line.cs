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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Geometry;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.line ToRFEM6(this Line line, int lineNo, Node startNode, Node endNode)
        {


            rfModel.line rfLine = new rfModel.line()
            {
                no = lineNo,
                definition_nodes = new int[] { startNode.GetRFEM6ID(), endNode.GetRFEM6ID() },
                comment = "lines for beams",
                type = rfModel.line_type.TYPE_POLYLINE,
                typeSpecified = true,
            };

        
            return rfLine;

        }

        public static rfModel.line ToRFEM6(this RFEMLine rfemLine)
        {

            List<Node> nodes = rfemLine.Nodes;

            rfModel.line rfLine = new rfModel.line();

            if (rfemLine.LineType is RFEMLineType.Polyline)
            {

                rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    definition_nodes = rfemLine.Nodes.Select(x => x.GetRFEM6ID()).ToArray(),
                    type = rfModel.line_type.TYPE_POLYLINE,
                };
            }

            if (rfemLine.LineType is RFEMLineType.Arc)
            {

                Node n00 = rfemLine.Nodes.ToArray()[0];
                Node mid = rfemLine.Nodes.ToArray()[1];
                Node n11 = rfemLine.Nodes.ToArray()[2];


                rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    definition_nodes = new int[] { n00.GetRFEM6ID(), n11.GetRFEM6ID() },
                    type = rfModel.line_type.TYPE_ARC,
                    typeSpecified = true,
                    arc_control_point = new rfModel.vector_3d() { x = mid.Position.X, y = mid.Position.Y, z = mid.Position.Z },
                    arc_control_point_objectSpecified = true,
                    arc_alpha_adjustment_target = rfModel.line_arc_alpha_adjustment_target.ALPHA_ADJUSTMENT_TARGET_BEGINNING_OF_ARC,
                    arc_alpha_adjustment_targetSpecified = true

                };

                rfLine.SetPropertyValue("type", rfModel.line_type.TYPE_ARC);
                
            }

            if (rfemLine.LineType is RFEMLineType.Circle)
            {

                Node circleNodes = rfemLine.Nodes.ToArray()[0];

                rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    type = rfModel.line_type.TYPE_CIRCLE,
                    typeSpecified = true,
                    circle_center = new rfModel.vector_3d { x = circleNodes.Position.X, y = circleNodes.Position.Y, z = circleNodes.Position.Z },
                    circle_normal = new rfModel.vector_3d { x = rfemLine.Normal[0], y = rfemLine.Normal[1], z = rfemLine.Normal[2] },
                    circle_radius = rfemLine.Radius,
                    circle_radiusSpecified = true,

                };

            }

            return rfLine;

        }

    }
}
