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
using BH.oM.Structure.Elements;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Structure;
using System.Linq;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Adapter.RFEM6
{
    public class RFEMLineComparer : IEqualityComparer<RFEMLine>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMLineComparer()
        {
            m_nodeComparer = new NodeDistanceComparer();
        }

        /***************************************************/

        public RFEMLineComparer(int decimals)
        {
            m_nodeComparer = new NodeDistanceComparer(decimals);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(RFEMLine line1, RFEMLine line2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(line1, line2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(line1, null) || Object.ReferenceEquals(line2, null))
                return false;


            //if (line1.LineType != line2.LineType)
            if ((!line1.Curve.GetType().Equals(line2.Curve.GetType())))
            {
                if (!((line1.Curve is Polyline) && (line2.Curve is Line) || (line1.Curve is Line) && (line2.Curve is Polyline))) { return false; }
                //return false;
            }

            if (line1.Nodes.Count != line2.Nodes.Count)
                return false;

            bool equal = true;

            //As Arcs needs differnt test as polylines/Poly curves the has been split into two parts
            if ((line1.Curve is Arc))
            {
                Arc arc1 = line1.Curve as Arc;
                Arc arc2 = line2.Curve as Arc;

                if (Math.Abs(arc1.Radius - arc2.Radius) > 0.001)
                    return false;
                //Check for equality of normal
                if (!(((arc1.Normal().Normalise().Distance(arc2.Normal().Normalise()) < 0.001)) || ((arc2.Normal().Normalise().Reverse().Distance(arc1.Normal().Normalise()) < 0.001))))
                    return false;




                Point startPoint1 = Engine.Geometry.Query.StartPoint(arc1);
                Point endPoint1 = Engine.Geometry.Query.EndPoint(arc1);
                Point startPoint2 = Engine.Geometry.Query.StartPoint(arc2);
                Point endPoint2 = Engine.Geometry.Query.EndPoint(arc2);

                ////Check for equality of start and end point of arc and flipped arc
                ////if ((startPoint1.Distance(startPoint2) > 0.001 || endPoint1.Distance(endPoint2) > 0.001) && (startPoint1.Distance(endPoint2) > 0.001 || endPoint1.Distance(startPoint2) > 0.001))
                //if (((startPoint1.Distance(startPoint2) > 0.001 && endPoint1.Distance(endPoint2) > 0.001)) || (endPoint1.Distance(startPoint2) > 0.001) && (startPoint1.Distance(endPoint2) > 0.001))
                //    return false;

                if (arc1.CoordinateSystem.Origin.Distance(arc2.CoordinateSystem.Origin) > 0.001) { return false; };


                List<Point> pts = new List<Point>() { Engine.Geometry.Query.StartPoint(arc1), Engine.Geometry.Query.EndPoint(arc1) };
                List<Point> pts2 = new List<Point>() { Engine.Geometry.Query.StartPoint(arc2), Engine.Geometry.Query.EndPoint(arc2) };
                for (int i = 0; i < pts.Count; i++)
                {


                    if (!(pts[i].Distance(pts2[i]) > 0.001) && (pts[1 - i].Distance(pts2[i]) > 0.001)) { return false; };
                    //if (pts[1-i].Distance(pts2[i]) > 0.001) { return false; };

                }


            }
            else if ((line1.Curve is Circle))
            {


                Circle c0 = line1.Curve as Circle;
                Circle c1 = line2.Curve as Circle;

                if (Math.Abs(c0.Radius - c1.Radius) > 0.001) { return false; }


                if (c0.Normal.Normalise().Distance(c1.Normal.Normalise()) > 0.001) { return false; };

                if (c0.Centre.Distance(c1.Centre) > 0.001) { return false; };

            }
            else
            {



                for (int i = 0; i < line1.Nodes.Count; i++)
                {
                    if (!m_nodeComparer.Equals(line1.Nodes[i], line2.Nodes[i]))
                    {
                        equal = false;
                        break;
                    }
                }


            };


            if (equal)
                return true;

            equal = true;
            int lastIndex = line2.Nodes.Count - 1;
            for (int i = 0; i < line1.Nodes.Count; i++)
            {
                if (!m_nodeComparer.Equals(line1.Nodes[i], line2.Nodes[lastIndex - i]))
                {
                    equal = false;
                    break;
                }
            }
            return equal;
        }

        /***************************************************/

        public int GetHashCode(RFEMLine line)
        {
            //Check whether the object is null
            //if (Object.ReferenceEquals(line, null)) return 0;

            //return m_nodeComparer.GetHashCode(line.Nodes.First()) ^ m_nodeComparer.GetHashCode(line.Nodes.Last());
            return 0;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private NodeDistanceComparer m_nodeComparer;


        /***************************************************/
    }
}




