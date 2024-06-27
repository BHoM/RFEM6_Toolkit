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
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public class LoadCaseComparer : IEqualityComparer<Loadcase>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public LoadCaseComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Loadcase loadcase0, Loadcase loadcase1)
        {

            //Checks if one Name is equals to Null and one is not
            if ((loadcase0.Name == null && loadcase1.Name != null)|| (loadcase0.Name != null && loadcase1.Name == null))
            {
                return false;
            }

            //Checks if both Names are equal and not null or both are null
            if (!(loadcase0.Name?.Equals(loadcase1.Name) ?? loadcase1.Name==null)) {
                return false;
            
            }
            if(!loadcase0.Number.Equals(loadcase1.Number)) return false;


            if (!loadcase0.Nature.Equals(loadcase1.Nature)) { return false; }

            return true;

        }

        /***************************************************/

        public int GetHashCode(Loadcase loadcase)
        {

            //return surfaceSupport.GetHashCode();

            return 0;

        }


        /***************************************************/


    }



}





