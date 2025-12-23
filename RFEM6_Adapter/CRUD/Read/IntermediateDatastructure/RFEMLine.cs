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
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<RFEMLine> ReadLines(List<string> ids = null)
        {

            List<RFEMLine> lineList = new List<RFEMLine>();

            var lineNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE).Where(l => l.no > 0);
            var allRfLInes = lineNumbers.ToList().Select(n => m_Model.get_line(n.no));
            
            //var lineSupportNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
            //var lineSupports = lineNumbers.ToList().Select(n => m_Model.get_line(n.no));


            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();

            if (ids == null)
            {
                foreach (rfModel.line rfLine in allRfLInes)
                {
             
                    ICurve Curve = null;
                    double radius = 0;
                    //BH.oM.Geometry.CoordinateSystem.Cartesian coordSyst = new oM.Geometry.CoordinateSystem.Cartesian();
                    double angle = 0;
                    double[] x_VectorArr = new double[3];
                    double[] y_VectorArr = new double[3];

                    List<Node> lineNodes = new List<Node>();

                    if (rfLine.type is rfModel.line_type.TYPE_POLYLINE)
                    {


                        foreach (var n in rfLine.definition_nodes.ToList())
                        {

                            Node nd;
                            nodes.TryGetValue(n, out nd);

                            lineNodes.Add(nd);

                            

                        }
                        // Intentionally added Error
                        Curve= new Polyline { ControlPoints=(lineNodes.Select(n=>n.Position)).ToList()};

                    }

                    else if (rfLine.type is rfModel.line_type.TYPE_ARC)
                    {

                        Node n0;
                        nodes.TryGetValue(rfLine.definition_nodes[0], out n0);

                        Node n1;
                        nodes.TryGetValue(rfLine.definition_nodes[1], out n1);

                        //Point start = Engine.Geometry.Create.Point(rfLine.arc_control_point, rfLine.arc_control_point_y, rfLine.arc_control_point_z);
                        Point mid = Engine.Geometry.Create.Point(rfLine.arc_control_point_x, rfLine.arc_control_point_y, rfLine.arc_control_point_z);
                        Point centre = Engine.Geometry.Create.Point(rfLine.arc_center_x, rfLine.arc_center_y, rfLine.arc_center_z);
                        Point end = Engine.Geometry.Create.Point(rfLine.arc_control_point_x, rfLine.arc_control_point_y, rfLine.arc_control_point_z);



                        lineNodes = new List<Node>() { n0, new Node { Position = mid }, n1 };
                        //lineNodes = new List<Node>() { n0, new Node { Position = mid }, n1, new Node { Position = centre } };


                        //coordSyst=BH.Engine.Geometry.Create.CartesianCoordinateSystem(centre,Engine.Geometry.Create.Vector(centre,mid), Engine.Geometry.Create.Vector(n0.Position, n1.Position));

                        Vector x_Vector = Engine.Geometry.Create.Vector(centre, n0.Position).Normalise();
                        Vector tempVector = Engine.Geometry.Create.Vector(centre, mid).Normalise();

                        Vector z_Vector = Engine.Geometry.Query.CrossProduct(x_Vector, tempVector).Normalise();
                        Vector y_Vector = Engine.Geometry.Query.CrossProduct(x_Vector, z_Vector).Normalise();



                        x_VectorArr = new double[] { x_Vector.X, x_Vector.Y, x_Vector.Z };
                        y_VectorArr = new double[] { y_Vector.X, y_Vector.Y, y_Vector.Z };



                        angle = rfLine.arc_alpha;
                        radius = rfLine.arc_radius;

                        Curve = Engine.Geometry.Create.Arc(n1.Position,mid,n0.Position).Flip();    

                    }

                    else if (rfLine.type is rfModel.line_type.TYPE_CIRCLE)
                    {

                        lineNodes = new List<Node>() { new Node { Position = new Point { X = rfLine.circle_center.x, Y = rfLine.circle_center.y, Z = rfLine.circle_center.z } } };
                        radius = rfLine.circle_radius;

                        Curve = Engine.Geometry.Create.Circle(lineNodes[0].Position, radius);
                    }

                    RFEMLine l = new RFEMLine {Curve=Curve,Nodes = lineNodes };

                    if (rfLine.support != 0 && rfLine != null) { l.supportID = rfLine.support; };

                    // if (radius > 0) l.Radius = radius;
                    l.SetRFEM6ID(rfLine.no);
                    lineList.Add(l);
                }
            }

            return lineList;
        }

    }
}



