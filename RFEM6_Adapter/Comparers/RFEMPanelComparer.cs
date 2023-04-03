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

namespace BH.Adapter.RFEM6
{
    public class RFEMPanelComparer : IEqualityComparer<Panel>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        //public RFEMLineComparer()
        //{
        //    m_nodeComparer = new NodeDistanceComparer();
        //}

        ///***************************************************/

        //public RFEMLineComparer(int decimals)
        //{
        //    m_nodeComparer = new NodeDistanceComparer(decimals);
        //}


        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public bool Equals(Panel panel1, Panel panel2)
        {

            bool equal = false;

            var lst1 = panel1.ExternalEdges.Select(e => (e.Curve)).ToList();
            PolyCurve pol1=Engine.Geometry.Create.PolyCurve(lst1);
           // List<PolyCurve> polCurveList1= new List<PolyCurve>() {pol1};
            //var joinedCurve1 = Engine.Geometry.Compute.Join(polCurveList1);

            var lst2 = panel2.ExternalEdges.Select(e => (e.Curve)).ToList();
            PolyCurve pol2 = Engine.Geometry.Create.PolyCurve(lst2);
            List<PolyCurve> polCurveList2 = new List<PolyCurve>() { pol2 };

            var compare = Engine.Geometry.Compute.BooleanDifference(pol1,polCurveList2);

            if (compare.Count.Equals(0)) equal = true;



            ////Check whether the compared objects reference the same data.
            //if (Object.ReferenceEquals(line1, line2)) return true;

            ////Check whether any of the compared objects is null.
            //if (Object.ReferenceEquals(line1, null) || Object.ReferenceEquals(line2, null))
            //    return false;


            //if (line1.LineType != line2.LineType)
            //    return false;

            //if(line1.Nodes.Count != line2.Nodes.Count) 
            //    return false;

            //bool equal = true;
            //for (int i = 0; i < line1.Nodes.Count; i++)
            //{
            //    if (!m_nodeComparer.Equals(line1.Nodes[i], line2.Nodes[i]))
            //    { 
            //        equal = false;
            //        break;
            //    }
            //}

            //if(equal)
            //    return true;

            //equal = true;
            //int lastIndex = line2.Nodes.Count -1;
            //for (int i = 0; i < line1.Nodes.Count; i++)
            //{
            //    if (!m_nodeComparer.Equals(line1.Nodes[i], line2.Nodes[lastIndex - i]))
            //    {
            //        equal = false;
            //        break;
            //    }
            // }
            return equal;
        }

        /***************************************************/

        public int GetHashCode(Panel line)
        {
            //Check whether the object is null
            //if (Object.ReferenceEquals(line, null)) return 0;

            //return m_nodeComparer.GetHashCode(line.Nodes.First()) ^ m_nodeComparer.GetHashCode(line.Nodes.Last());

            return 0;

        }


        ///***************************************************/
        ///**** Private Fields                            ****/
        ///***************************************************/

        //private NodeDistanceComparer m_nodeComparer;


        ///***************************************************/
    }
}




