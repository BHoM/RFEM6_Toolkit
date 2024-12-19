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
using BH.Engine.Structure;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.Engine.Spatial;

namespace BH.Adapter.RFEM6
{
    public class PanelComparer : IEqualityComparer<Panel>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public PanelComparer()
        {

        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Panel panel0, Panel panel1)
        {
         
            if(panel0.Centroid().Distance(panel1.Centroid())<0.001) return true;

            return false;
        }

        /***************************************************/

        public int GetHashCode(Panel panel)
        {
            //Check whether the object is null
            return 0; 
        }

        /***************************************************/
    }
}





