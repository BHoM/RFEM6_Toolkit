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



            //if (rfemLine.LineType is RFEMLineType.Polyline)
            if (rfemLine.Curve is Polyline || rfemLine.Curve is Line)
            {

                rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    definition_nodes = rfemLine.Nodes.Select(x => x.GetRFEM6ID()).ToArray(),
                    type = rfModel.line_type.TYPE_POLYLINE,
                };
            }

            //if (rfemLine.LineType is RFEMLineType.Arc)

            if (rfemLine.Curve is Arc)
            {

                Node n00 = rfemLine.Nodes.ToArray()[0];

                Point mid = Engine.Geometry.Query.PointAtLength((Arc)rfemLine.Curve, 0.5);

                Node n11 = rfemLine.Nodes.ToArray()[2];


                rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    definition_nodes = new int[] { n11.GetRFEM6ID(), n00.GetRFEM6ID() },
                    type = rfModel.line_type.TYPE_ARC,
                    typeSpecified = true,
                    arc_control_point = new rfModel.vector_3d() { x = mid.X, y = mid.Y, z = mid.Z },
                    //arc_control_point = new rfModel.vector_3d() { x = mid.Position.X, y = mid.Position.Y, z = mid.Position.Z },
                    arc_control_point_objectSpecified = true,
                    arc_alpha_adjustment_target = rfModel.line_arc_alpha_adjustment_target.ALPHA_ADJUSTMENT_TARGET_BEGINNING_OF_ARC,
                    arc_alpha_adjustment_targetSpecified = true,


                };

                rfLine.SetPropertyValue("type", rfModel.line_type.TYPE_ARC);

            }

            //if (rfemLine.LineType is RFEMLineType.Circle)
            if (rfemLine.Curve is Circle c)

            {
                 


                Point Centre = c.Centre;

                var normal = (rfemLine.Curve as Circle).Normal;
                double rardius = (rfemLine.Curve as Circle).Radius;

                rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    type = rfModel.line_type.TYPE_CIRCLE,
                    typeSpecified = true,
                    circle_center = new rfModel.vector_3d { x = Centre.X, y = Centre.Y, z = Centre.Z },
                    circle_normal = new rfModel.vector_3d { x = normal.X, y = normal.Y, z = normal.Z },
                    circle_radius = rardius,
                    circle_radiusSpecified = true,

                };

            }

            return rfLine;

        }

    }
}
