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
using BH.oM.Structure.Elements;
using BH.Engine.Structure;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public class EdgeComparer : IEqualityComparer<Edge>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public EdgeComparer()
        {

        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Edge edge0, Edge edge1)
        {
         
            RFEMLineSupportComparer lineSupportComparer = new RFEMLineSupportComparer();
            Constraint6DOFComparer constraint6DOFComparer = new Constraint6DOFComparer();
            bool constraintEquals=constraint6DOFComparer.Equals(edge0.Support, edge1.Support);

            RFEMLine rfemLine0 = RFEMLineFromEdge(edge0);
            
            RFEMLine rfemLine1 = RFEMLineFromEdge(edge1);

            RFEMLineComparer lineComparer = new RFEMLineComparer(); 
            bool lineEquals = lineComparer.Equals(rfemLine0, rfemLine1);    

            return constraintEquals && lineEquals;
        }

        /***************************************************/

        public int GetHashCode(Edge edge)
        {
            //Check whether the object is null
            return 0; 
        }

        public static RFEMLine RFEMLineFromEdge(Edge edge) {

            RFEMLine rfLine = null;

            if (edge.Curve is Line line)
            {
                rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = line.Start }, new Node { Position = line.End } },Curve=edge.Curve };
            }

            if (edge.Curve is Polyline polyline)
            {
                List<Node> controlNodes = new List<Node>();
                polyline.ControlPoints.ForEach(p => controlNodes.Add(new Node { Position = p }));

                rfLine = new RFEMLine() { Nodes = controlNodes, Curve = edge.Curve };
            }

            //Add for other line types and add convert in terms of nodes accordingly
            else if (edge.Curve is Arc arc)
            {

                double radius = 0;
                //BH.oM.Geometry.CoordinateSystem.Cartesian coordSyst = new oM.Geometry.CoordinateSystem.Cartesian();
                double angle = 0;
                double[] x_VectorArr = new double[3];
                double[] y_VectorArr = new double[3];

                Point[] pts = arc.ControlPoints().ToArray();
                Vector x_Vector = Engine.Geometry.Create.Vector(arc.Centre(), arc.StartPoint()).Normalise();
                Vector tempVector = Engine.Geometry.Create.Vector(arc.Centre(), arc.PointAtLength(0.5) * arc.Length()).Normalise();

                Vector z_Vector = Engine.Geometry.Query.CrossProduct(x_Vector, tempVector).Normalise();
                Vector y_Vector = Engine.Geometry.Query.CrossProduct(x_Vector, z_Vector).Normalise();

                x_VectorArr = new double[] { x_Vector.X, x_Vector.Y, x_Vector.Z };
                y_VectorArr = new double[] { y_Vector.X, y_Vector.Y, y_Vector.Z };

                angle = arc.Angle();
                //radius = arc.Radius();
                radius = arc.Radius;

                rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = pts[0] }, new Node { Position = pts[2] }, new Node { Position = pts[4] }, new Node { Position = arc.Centre() } }, Curve = edge.Curve };

            }

            else if (edge.Curve is Circle circle1)
            {
                List<Node> controlePoints = new List<Node>();
                controlePoints.Add(new Node { Position = circle1.Centre });
                rfLine = new RFEMLine() { Nodes = controlePoints, Curve = edge.Curve };

                //rfLine = new RFEMLine() { Nodes = controlePoints, Radius = circle1.Radius, Normal = new double[] { circle1.Normal.X, circle1.Normal.Y, circle1.Normal.Z }, LineType = RFEMLineType.Circle };
            }
            if (edge.Support != null) { rfLine.Support = edge.Support; }

            return rfLine;

        }

        /***************************************************/
    }
}





