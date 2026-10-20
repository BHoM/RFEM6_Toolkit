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
using System.Collections.Generic;
using System.Linq;

using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool QueueEdgeLineSupports(Edge edge, Constraint6DOF support)
        {
            bool success = true;

            foreach (RFEMLine segment in LineSegmentsForEdge(edge))
            {
                int lineNo = FindBestMatchingLineNumber(segment);
                if (lineNo <= 0)
                {
                    success = false;
                    continue;
                }

                QueueLineSupport(support, lineNo);
            }

            return success;
        }

        private bool QueueEdgeLineReleases(Edge edge, Constraint4DOF release)
        {
            bool success = true;

            foreach (RFEMLine segment in LineSegmentsForEdge(edge))
            {
                int lineNo = FindBestMatchingLineNumber(segment);
                if (lineNo <= 0)
                {
                    success = false;
                    continue;
                }

                QueueLineRelease(release, lineNo);
            }

            return success;
        }

        private void QueueLineSupport(Constraint6DOF constraint, int lineNo)
        {
            double[] stiffnesses = constraint.ToRFEM6Stiffnesses();

            TypesForLines.LineSupport support = m_createQueue.OfType<TypesForLines.LineSupport>().FirstOrDefault(s =>
                Convert.StiffnessesMatch(stiffnesses, new[] { s.SpringX, s.SpringY, s.SpringZ, s.RotationalRestraintX, s.RotationalRestraintY, s.RotationalRestraintZ }));

            if (support == null)
            {
                support = constraint.ToRFEM6LineSupport(ClaimNumber(0, UsedNumbers<TypesForLines.LineSupport>()));
                constraint.SetRFEM6ID(support.No);
                CreateObject(support);
            }

            if (!support.Lines.Contains(lineNo))
                support.Lines.Add(lineNo);
        }

        private void QueueLineRelease(Constraint4DOF release, int lineNo)
        {
            double[] stiffnesses = release.ToRFEM6LineReleaseStiffnesses();

            TypesForSpecialObjects.LineReleaseType releaseType = m_createQueue.OfType<TypesForSpecialObjects.LineReleaseType>().FirstOrDefault(t =>
                Convert.StiffnessesMatch(stiffnesses, new[] { t.TranslationalReleaseUX, t.TranslationalReleaseUY, t.TranslationalReleaseUZ, t.RotationalReleasePhiX }));

            if (releaseType == null)
            {
                releaseType = release.ToRFEM6LineReleaseType(ClaimNumber(0, UsedNumbers<TypesForSpecialObjects.LineReleaseType>()));
                CreateObject(releaseType);
            }

            StructureAdvanced.LineRelease lineRelease = m_createQueue.OfType<StructureAdvanced.LineRelease>().FirstOrDefault(r =>
                r.LineReleaseType == releaseType.No && r.Lines.Contains(lineNo));

            if (lineRelease != null)
                return;

            lineRelease = new StructureAdvanced.LineRelease
            {
                No = ClaimNumber(0, UsedNumbers<StructureAdvanced.LineRelease>()),
                LineReleaseType = releaseType.No,
            };
            lineRelease.Lines.Add(lineNo);
            CreateObject(lineRelease);
        }
    }
}
