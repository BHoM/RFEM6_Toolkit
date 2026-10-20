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
using System.Linq;

using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static RFEMNodalSupport FromRFEM(this TypesForNodes.NodalSupport support)
        {
            Constraint6DOF constraint = FromRFEMStiffnesses(
                ConstraintNameFromRfem(support.Name, support.UserDefinedNameEnabled),
                support.SpringX,
                support.SpringY,
                support.SpringZ,
                support.RotationalRestraintX,
                support.RotationalRestraintY,
                support.RotationalRestraintZ);
            constraint.SetRFEM6ID(support.No);

            RFEMNodalSupport nodalSupport = new RFEMNodalSupport { Constraint = constraint, nodesIDs = support.Nodes.ToList() };
            nodalSupport.SetRFEM6ID(support.No);
            return nodalSupport;
        }

        public static RFEMLineSupport FromRFEM(this TypesForLines.LineSupport support)
        {
            Constraint6DOF constraint = FromRFEMStiffnesses(
                ConstraintNameFromRfem(support.Name, support.UserDefinedNameEnabled),
                support.SpringX,
                support.SpringY,
                support.SpringZ,
                support.RotationalRestraintX,
                support.RotationalRestraintY,
                support.RotationalRestraintZ);
            constraint.SetRFEM6ID(support.No);

            RFEMLineSupport lineSupport = new RFEMLineSupport { Constraint = constraint, nodesIDs = support.Lines.ToList() };
            lineSupport.SetRFEM6ID(support.No);
            return lineSupport;
        }

        public static RFEMHinge FromRFEM(this TypesForMembers.MemberHinge hinge)
        {
            Constraint6DOF constraint = FromRFEMStiffnesses(hinge.Name, hinge.AxialReleaseN, hinge.AxialReleaseVy, hinge.AxialReleaseVz, hinge.MomentReleaseMt, hinge.MomentReleaseMy, hinge.MomentReleaseMz);

            RFEMHinge rfemHinge = new RFEMHinge { Constraint = constraint };
            rfemHinge.SetRFEM6ID(hinge.No);
            return rfemHinge;
        }

        /// <summary>
        /// Uses the RFEM user-defined support name when <paramref name="userDefinedNameEnabled"/> is set; otherwise leaves BHoM <see cref="Constraint6DOF.Name"/> empty.
        /// </summary>
        private static string ConstraintNameFromRfem(string name, bool userDefinedNameEnabled)
        {
            if (!userDefinedNameEnabled || string.IsNullOrWhiteSpace(name))
                return null;

            return name.Trim();
        }

        /// <summary>
        /// Infinite stiffness is fixed, zero is free and anything else is a spring with that stiffness.
        /// </summary>
        private static Constraint6DOF FromRFEMStiffnesses(string name, double x, double y, double z, double xx, double yy, double zz)
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = ToDOFType(x),
                TranslationY = ToDOFType(y),
                TranslationZ = ToDOFType(z),
                RotationX = ToDOFType(xx),
                RotationY = ToDOFType(yy),
                RotationZ = ToDOFType(zz),
                TranslationalStiffnessX = SpringStiffness(x),
                TranslationalStiffnessY = SpringStiffness(y),
                TranslationalStiffnessZ = SpringStiffness(z),
                RotationalStiffnessX = SpringStiffness(xx),
                RotationalStiffnessY = SpringStiffness(yy),
                RotationalStiffnessZ = SpringStiffness(zz),
            };
        }

        private static DOFType ToDOFType(double stiffness)
        {
            if (double.IsPositiveInfinity(stiffness))
                return DOFType.Fixed;

            return stiffness == 0 ? DOFType.Free : DOFType.Spring;
        }

        private static double SpringStiffness(double stiffness)
        {
            return ToDOFType(stiffness) == DOFType.Spring ? stiffness : 0;
        }
    }
}
