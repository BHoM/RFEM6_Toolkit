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
using BH.oM.Structure.SectionProperties;

namespace BH.Adapter.RFEM6
{
    public class RFEMSectionComparer : IEqualityComparer<ISectionProperty>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMSectionComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(ISectionProperty section1, ISectionProperty section2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(section1, section2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(section1, null) || Object.ReferenceEquals(section2, null))
                return false;
            if (!(section1 is GenericSection || section2 is GenericSection))
            {
                Convert.AlterSectionName(section1);
                Convert.AlterSectionName(section2);
            }

            bool sectionNameDoesMatch = String.Equals(section1.Name, section2.Name);
            bool materialDoesMatch = String.Equals(section1.Material.Name, section2.Material.Name);

            return sectionNameDoesMatch && materialDoesMatch;

        }

        /***************************************************/
        /***************************************************/

        public int GetHashCode(ISectionProperty section)
        {
            return 0;
        }


        /***************************************************/


    }



}





