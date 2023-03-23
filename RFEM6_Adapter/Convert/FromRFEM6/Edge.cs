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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static Edge FromRFEM(this rfModel.line rfLine, Dictionary<int, Node> nodeDict)
        {

            var type = rfLine.type.ToString();
            ICurve curve = null;


            if (type.Equals("TYPE_ARC")) {

                Node n0 = nodeDict[rfLine.definition_nodes[0]];
                Node n1 = nodeDict[rfLine.definition_nodes[1]];

                Point mid = Engine.Geometry.Create.Point(rfLine.arc_control_point_x, rfLine.arc_control_point_y, rfLine.arc_control_point_z);

                curve = Engine.Geometry.Create.Arc(n0.Position, mid, n1.Position);
          

            }
            else if(type.Equals("TYPE_POLYLINE")){

                Node n0 = nodeDict[rfLine.definition_nodes[0]];
                Node n1 = nodeDict[rfLine.definition_nodes[1]];

                curve = new Line {Start=n0.Position,End=n1.Position};

            }

            Edge edge = new Edge {Curve=curve};
            edge.SetRFEM6ID(rfLine.no);

            return edge;
        }

    }
}
