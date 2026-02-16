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
using System.Linq;
using System.Text;
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Geometry;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        private enum Axis { X, Y, Z, XX, YY, ZZ }

        public static rfModel.nodal_support ToRFEM6(this Constraint6DOF constraint)
        {
            rfModel.nodal_support rfConstraint = new rfModel.nodal_support();
            rfConstraint.no = constraint.GetRFEM6ID();
            rfConstraint.spring_x = TranslateStiffnessFromRFEM(constraint, Axis.X);
            rfConstraint.spring_xSpecified = true;
            rfConstraint.spring_y = TranslateStiffnessFromRFEM(constraint, Axis.Y);
            rfConstraint.spring_ySpecified = true;
            rfConstraint.spring_z = TranslateStiffnessFromRFEM(constraint, Axis.Z);
            rfConstraint.spring_zSpecified = true;
            rfConstraint.rotational_restraint_x = TranslateStiffnessFromRFEM(constraint, Axis.XX);
            rfConstraint.rotational_restraint_xSpecified = true;
            rfConstraint.rotational_restraint_y = TranslateStiffnessFromRFEM(constraint, Axis.YY);
            rfConstraint.rotational_restraint_ySpecified = true;
            rfConstraint.rotational_restraint_z = TranslateStiffnessFromRFEM(constraint, Axis.ZZ);
            rfConstraint.rotational_restraint_zSpecified = true;

            return rfConstraint;
        }


        private static double TranslateStiffnessFromRFEM(Constraint6DOF constraint, Axis axis)
        {

            var movement = DOFType.Fixed;
            double stiffness = 0;

            switch (axis)
            {
                case Axis.X:
                    movement = constraint.TranslationX;
                    stiffness = constraint.TranslationalStiffnessX;
                    break;
                case Axis.Y:
                    movement = constraint.TranslationY;
                    stiffness = constraint.TranslationalStiffnessY;
                    break;
                case Axis.Z:
                    movement = constraint.TranslationZ;
                    stiffness = constraint.TranslationalStiffnessZ;
                    break;
                case Axis.XX:
                    movement = constraint.RotationX;
                    stiffness = constraint.RotationalStiffnessX;
                    break;
                case Axis.YY:
                    movement = constraint.RotationY;
                    stiffness = constraint.RotationalStiffnessY;
                    break;
                case Axis.ZZ:
                    movement = constraint.RotationZ;
                    stiffness = constraint.RotationalStiffnessZ;
                    break;

                default:
                    BH.Engine.Base.Compute.RecordWarning($"Invalid axis provided: {axis} \n Only X, Y, and Z are have been implemented at this stage.");
                    return 0;
            }

            if (movement == DOFType.Fixed)
            {
                return 0;
            }
            else if (movement == DOFType.Free)
            {
                return Double.PositiveInfinity;
            }
            else if (movement == DOFType.Spring)
            {
                return stiffness;
            }
            else
            {
                BH.Engine.Base.Compute.RecordWarning($"Invalid DOFType provided: {movement} \n Only Fixed, Free, and Spring have been implemented at this stage.");
                return 0;

            }

        }
    }
}



