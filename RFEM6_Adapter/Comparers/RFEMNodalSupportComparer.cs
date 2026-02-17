/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.Engine.Structure;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;

namespace BH.Adapter.RFEM6
{
    public class RFEMNodalSupportComparer : IEqualityComparer<RFEMNodalSupport>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMNodalSupportComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(RFEMNodalSupport support1, RFEMNodalSupport support2)
        {

            Constraint6DOF c1 = support1.Constraint;
            Constraint6DOF c2 = support2.Constraint;

            //Constraint6DOFComparer comparer = new Constraint6DOFComparer();
            //return comparer.Equals(constraint1, constraint2);

            var nodeList1 = c1.PropertyValue("NodeList");
            var nodeList2 = c2.PropertyValue("NodeList");

            return (nodeList1 is null && nodeList2 is null)
                && c1.TranslationalStiffnessX == c2.TranslationalStiffnessX
                && c1.TranslationalStiffnessY == c2.TranslationalStiffnessY
                && c1.TranslationalStiffnessZ == c2.TranslationalStiffnessZ
                && c1.TranslationX == c2.TranslationX
                && c1.TranslationY == c2.TranslationY
                && c1.TranslationZ == c2.TranslationZ
                && c1.RotationalStiffnessX == c2.RotationalStiffnessX
                && c1.RotationalStiffnessY == c2.RotationalStiffnessY
                && c1.RotationalStiffnessZ == c2.RotationalStiffnessZ
                && c1.RotationX == c2.RotationX
                && c1.RotationY == c2.RotationY
                && c1.RotationZ == c2.RotationZ;


        }

        /***************************************************/

        public int GetHashCode(RFEMNodalSupport surfaceSupport)
        {

            //return surfaceSupport.GetHashCode();

            return 0;

        }


        /***************************************************/


    }



}







