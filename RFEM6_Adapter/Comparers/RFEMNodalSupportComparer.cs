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
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

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

            Constraint6DOF constraint1 = support1.Constraint;
            Constraint6DOF constraint2 = support2.Constraint;

            if (!constraint1.TranslationalStiffnessX.Equals(constraint2.TranslationalStiffnessX)) return false;
            if (!constraint1.TranslationalStiffnessY.Equals(constraint2.TranslationalStiffnessY)) return false;
            if (!constraint1.TranslationalStiffnessZ.Equals(constraint2.TranslationalStiffnessZ)) return false;
            if (!constraint1.TranslationX.Equals(constraint2.TranslationX)) return false;
            if (!constraint1.TranslationY.Equals(constraint2.TranslationY)) return false;
            if (!constraint1.TranslationZ.Equals(constraint2.TranslationZ)) return false;

            if (!constraint1.RotationalStiffnessX.Equals(constraint2.RotationalStiffnessX)) return false;
            if (!constraint1.RotationalStiffnessY.Equals(constraint2.RotationalStiffnessY)) return false;
            if (!constraint1.RotationalStiffnessZ.Equals(constraint2.RotationalStiffnessZ)) return false;
            if (!constraint1.RotationX.Equals(constraint2.RotationX)) return false;
            if (!constraint1.RotationY.Equals(constraint2.RotationY)) return false;
            if (!constraint1.RotationZ.Equals(constraint2.RotationZ)) return false;



            return true;

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




