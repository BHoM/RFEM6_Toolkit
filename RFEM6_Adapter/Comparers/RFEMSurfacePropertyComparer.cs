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
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public class RFEMSurfacePropertyComparer : IEqualityComparer<ISurfaceProperty>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMSurfacePropertyComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(ISurfaceProperty surfaceProp1, ISurfaceProperty surfaceProp2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(surfaceProp1, surfaceProp2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(surfaceProp1, null) || Object.ReferenceEquals(surfaceProp2, null))
                return false;

            var material1 = surfaceProp1.Material.Name;
            var material2 = surfaceProp2.Material.Name;
            var type1 = surfaceProp1.GetType().Name;
            var type2 = surfaceProp2.GetType().Name;
            var thickness1 = Engine.Base.Query.PropertyValue(surfaceProp1, "Thickness");
            var thickness2 = Engine.Base.Query.PropertyValue(surfaceProp2, "Thickness");

            if (material1.Equals(material2) && type1.Equals(type2) && (thickness1.Equals(thickness2))) return true;


            return false;

        }

        /***************************************************/

        public int GetHashCode(ISurfaceProperty section)
        {
            return 0;
        }


        /***************************************************/


    }



}




