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

using BH.oM.Structure.Constraints;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static TypesForNodes.NodalSupport ToRFEM6NodalSupport(this Constraint6DOF constraint, int supportNo)
        {
            double[] k = constraint.ToRFEM6Stiffnesses();

            TypesForNodes.NodalSupport rfSupport = new TypesForNodes.NodalSupport
            {
                No = supportNo,
                SpringX = k[0],
                SpringY = k[1],
                SpringZ = k[2],
                RotationalRestraintX = k[3],
                RotationalRestraintY = k[4],
                RotationalRestraintZ = k[5],
            };

            if (!string.IsNullOrWhiteSpace(constraint.Name))
            {
                rfSupport.UserDefinedNameEnabled = true;
                rfSupport.Name = constraint.Name;
            }

            return rfSupport;
        }

        public static TypesForLines.LineSupport ToRFEM6LineSupport(this Constraint6DOF constraint, int supportNo)
        {
            double[] k = constraint.ToRFEM6Stiffnesses();

            TypesForLines.LineSupport rfSupport = new TypesForLines.LineSupport
            {
                No = supportNo,
                SpringX = k[0],
                SpringY = k[1],
                SpringZ = k[2],
                RotationalRestraintX = k[3],
                RotationalRestraintY = k[4],
                RotationalRestraintZ = k[5],
            };

            if (!string.IsNullOrWhiteSpace(constraint.Name))
            {
                rfSupport.UserDefinedNameEnabled = true;
                rfSupport.Name = constraint.Name;
            }

            return rfSupport;
        }

        public static TypesForMembers.MemberHinge ToRFEM6MemberHinge(this Constraint6DOF release, int hingeNo)
        {
            double[] k = release.ToRFEM6Stiffnesses();

            TypesForMembers.MemberHinge rfHinge = new TypesForMembers.MemberHinge
            {
                No = hingeNo,
                AxialReleaseN = k[0],
                AxialReleaseVy = k[1],
                AxialReleaseVz = k[2],
                MomentReleaseMt = k[3],
                MomentReleaseMy = k[4],
                MomentReleaseMz = k[5],
            };

            if (!string.IsNullOrWhiteSpace(release.Name))
            {
                rfHinge.UserDefinedNameEnabled = true;
                rfHinge.Name = release.Name;
            }

            return rfHinge;
        }

        /// <summary>
        /// RFEM stiffness per DOF in the order X, Y, Z, XX, YY, ZZ: fixed is infinite, free is zero, springs keep their stiffness.
        /// </summary>
        public static double[] ToRFEM6Stiffnesses(this Constraint6DOF constraint)
        {
            return new double[]
            {
                ToRFEM6Stiffness(constraint.TranslationX, constraint.TranslationalStiffnessX),
                ToRFEM6Stiffness(constraint.TranslationY, constraint.TranslationalStiffnessY),
                ToRFEM6Stiffness(constraint.TranslationZ, constraint.TranslationalStiffnessZ),
                ToRFEM6Stiffness(constraint.RotationX, constraint.RotationalStiffnessX),
                ToRFEM6Stiffness(constraint.RotationY, constraint.RotationalStiffnessY),
                ToRFEM6Stiffness(constraint.RotationZ, constraint.RotationalStiffnessZ),
            };
        }

        public static bool IsFullyFixed(this Constraint6DOF constraint)
        {
            return constraint.ToRFEM6Stiffnesses().All(double.IsPositiveInfinity);
        }

        public static TypesForSpecialObjects.LineReleaseType ToRFEM6LineReleaseType(this Constraint4DOF release, int typeNo)
        {
            double[] k = release.ToRFEM6LineReleaseStiffnesses();

            TypesForSpecialObjects.LineReleaseType rfType = new TypesForSpecialObjects.LineReleaseType
            {
                No = typeNo,
                TranslationalReleaseUX = k[0],
                TranslationalReleaseUY = k[1],
                TranslationalReleaseUZ = k[2],
                RotationalReleasePhiX = k[3],
            };

            if (!string.IsNullOrWhiteSpace(release.Name))
            {
                rfType.UserDefinedNameEnabled = true;
                rfType.Name = release.Name;
            }

            return rfType;
        }

        /// <summary>
        /// Line releases in RFEM use u_x, u_y, u_z and φ_x (see LineReleaseType in the Dlubal API).
        /// </summary>
        public static double[] ToRFEM6LineReleaseStiffnesses(this Constraint4DOF release)
        {
            return new double[]
            {
                ToRFEM6Stiffness(release.TranslationX, release.TranslationalStiffnessX),
                ToRFEM6Stiffness(release.TranslationY, release.TranslationalStiffnessY),
                ToRFEM6Stiffness(release.TranslationZ, release.TranslationalStiffnessZ),
                ToRFEM6Stiffness(release.RotationX, release.RotationalStiffnessX),
            };
        }

        public static bool IsFullyFixedLineRelease(this Constraint4DOF release)
        {
            return release.ToRFEM6LineReleaseStiffnesses().All(double.IsPositiveInfinity);
        }

        public static bool StiffnessesMatch(double[] a, double[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (double.IsInfinity(a[i]) || double.IsInfinity(b[i]))
                {
                    if (!a[i].Equals(b[i]))
                        return false;
                }
                else if (Math.Abs(a[i] - b[i]) > 1e-6 * Math.Max(1.0, Math.Abs(a[i])))
                {
                    return false;
                }
            }

            return true;
        }

        private static double ToRFEM6Stiffness(DOFType dof, double stiffness)
        {
            switch (dof)
            {
                case DOFType.Free:
                    return 0;
                case DOFType.Spring:
                    return stiffness;
                case DOFType.Fixed:
                    return double.PositiveInfinity;
                default:
                    BH.Engine.Base.Compute.RecordWarning($"DOFType {dof} is not supported by the RFEM6 adapter and will be pushed as Fixed.");
                    return double.PositiveInfinity;
            }
        }
    }
}
