/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
            //return false;
            bool equal = false;

            var lst1 = panel1.ExternalEdges.Select(e => (e.Curve)).ToList();
            PolyCurve pol1 = Engine.Geometry.Create.PolyCurve(lst1);

            var lst2 = panel2.ExternalEdges.Select(e => (e.Curve)).ToList();
            PolyCurve pol2 = Engine.Geometry.Create.PolyCurve(lst2);

            try
            {
                pol1 = pol1.Close();
                pol2 = pol2.Close();

            }
            catch
            {

                BH.Engine.Base.Compute.RecordError($"Not all Curve segments of either panel {panel1.Name} or panel {panel2.Name} are joined!");

            }

            List<PolyCurve> polCurveList2 = new List<PolyCurve>() { pol2 };

            var compare = Engine.Geometry.Compute.BooleanDifference(pol1, polCurveList2);

            if (compare.Count.Equals(0)) equal = true;

            return equal;
        }

        /***************************************************/

        public int GetHashCode(Panel line)
        {

            return 0;

        }


        ///***************************************************/
        ///**** Private Fields                            ****/
        ///***************************************************/

        //private NodeDistanceComparer m_nodeComparer;


        ///***************************************************/
    }
}






