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
using BH.oM.Base;
using BH.oM.Adapter;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.RFEM6;
using System.Collections;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.Security.Cryptography;

namespace BH.Adapter.RFEM6
{
    [Description("Dependency module for fetching all Loadcase stored in a list of Loadcombinations.")]
    public class GetLineFromEdgeModule : IGetDependencyModule<Edge, RFEMLine>
    {

        //TODO 
        //Make this work for polylines ....you need to split them and run the method over each curve
        public IEnumerable<RFEMLine> GetDependencies(IEnumerable<Edge> objects)
        {
            List< RFEMLine> lines = new List<RFEMLine>();
    
            foreach (Edge edge in objects)
            {
                

                RFEMLine rfLine =null;

                if(edge.Curve is Line line)
                {
                    rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = line.Start }, new Node { Position = line.End } }};

                    rfLine.Curve = line;

                }

                if (edge.Curve is Polyline polyline)
                {
                    List<Node> controlNodes=new List<Node>();
                    polyline.ControlPoints.ForEach(p=>controlNodes.Add(new Node {Position=p }));
               
                    rfLine = new RFEMLine() { Nodes = controlNodes};


                    rfLine.Curve = polyline;
                }

                //Add for other line types and add convert in terms of nodes accordingly
                else if (edge.Curve is Arc arc)
                {

                    double radius = 0;
                    //BH.oM.Geometry.CoordinateSystem.Cartesian coordSyst = new oM.Geometry.CoordinateSystem.Cartesian();
                    double angle = 0;
                    double[] x_VectorArr = new double[3];
                    double[] y_VectorArr = new double[3];

                    Point[] pts=arc.ControlPoints().ToArray();
                    Vector x_Vector = Engine.Geometry.Create.Vector(arc.Centre(), arc.StartPoint()).Normalise();
                    Vector tempVector = Engine.Geometry.Create.Vector(arc.Centre(), arc.PointAtLength(0.5)*arc.Length()).Normalise();

                    Vector z_Vector = Engine.Geometry.Query.CrossProduct(x_Vector, tempVector).Normalise();
                    Vector y_Vector = Engine.Geometry.Query.CrossProduct(x_Vector, z_Vector).Normalise();



                    x_VectorArr = new double[] { x_Vector.X, x_Vector.Y, x_Vector.Z };
                    y_VectorArr = new double[] { y_Vector.X, y_Vector.Y, y_Vector.Z };

                    angle = arc.Angle();
                    radius = arc.Radius();

                    //rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = pts[0] }, new Node { Position = pts[2] },  new Node { Position = pts[4] }, new Node { Position = arc.Centre() } },Angle=angle,Radius=radius, X_Vector= x_VectorArr, Y_Vector= y_VectorArr, LineType = RFEMLineType.Arc };
                    //rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = pts[0] }, new Node { Position = pts[2] },  new Node { Position = pts[4] } },Angle=angle, X_Vector= x_VectorArr, Y_Vector= y_VectorArr, LineType = RFEMLineType.Arc };
                    rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = pts[0] }, new Node { Position = pts[2] },  new Node { Position = pts[4] } }};


                    rfLine.Curve = arc;
                    
                }

                else if (edge.Curve is Circle circle1)
                {
                    List<Node> controlePoints = new List<Node>();
                    controlePoints.Add(new Node { Position = circle1.Centre });
                    rfLine = new RFEMLine() { Nodes = controlePoints};
                    //rfLine = new RFEMLine() { Nodes = controlePoints, Radius = circle1.Radius, Normal = new double[] { circle1.Normal.X, circle1.Normal.Y, circle1.Normal.Z }, LineType = RFEMLineType.Circle };

                    rfLine.Curve = circle1;
                }

                if (edge.Support != null) { rfLine.Support = edge.Support; }

                edge.Fragments.Add(rfLine);

               

                lines.Add(rfLine);
            }

            return lines;
        }
    }
}