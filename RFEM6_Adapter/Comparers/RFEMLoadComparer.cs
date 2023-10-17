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
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public class RFEMLoadComparer : IEqualityComparer<ILoad>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMLoadComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(ILoad load0, ILoad load1)
        {

            if(load0.GetHashCode().Equals(load1.GetHashCode())) return true;

                LoadCaseComparer loadCaseComparer = new LoadCaseComparer();
            if (!loadCaseComparer.Equals(load0.Loadcase, load1.Loadcase)) return false;

            if (load0.Axis != load1.Axis) return false;

            if (load0.Projected != load1.Projected) return false;

            if (!load0.GetType().Equals(load1.GetType())) return false; 



            return true;

        }

        /***************************************************/

        public int GetHashCode(ILoad load)
        {

            //return surfaceSupport.GetHashCode();

            return 0;
            
        }


        /***************************************************/


    }



}




