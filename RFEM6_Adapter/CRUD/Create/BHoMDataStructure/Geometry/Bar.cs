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

using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Bar> bhBars)
        {
            return QueueMembers(bhBars);
        }

        private bool UpdateObjects(IEnumerable<Bar> bars)
        {
            return QueueMembers(bars);
        }

        private bool QueueMembers(IEnumerable<Bar> bhBars)
        {
            HashSet<int> usedNumbers = UsedNumbers<StructureCore.Member>();
            List<StructureCore.Member> members = new List<StructureCore.Member>();
            bool success = true;

            // RFEM allows only one member per line, so a bar on a line that already carries a member reuses that member.
            Dictionary<int, int> existingMembersByLine = new Dictionary<int, int>();
            foreach (StructureCore.Member rfMember in GetObjectList(new StructureCore.Member()).Where(m => m.No > 0 && m.Line > 0))
            {
                if (!existingMembersByLine.ContainsKey(rfMember.Line))
                    existingMembersByLine[rfMember.Line] = rfMember.No;
            }
            int reusedCount = 0;

            foreach (Bar bhBar in bhBars)
            {
                if (bhBar.Start == null || bhBar.End == null)
                {
                    BH.Engine.Base.Compute.RecordError($"Bar {bhBar.BHoM_Guid} is missing a start or end node and was not pushed.");
                    success = false;
                    continue;
                }

                int sectionNo = FindSectionNumber(bhBar.SectionProperty);
                if (sectionNo <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Bar {bhBar.BHoM_Guid} has no RFEM6 cross-section. Push its section property first or give the section a name RFEM recognises.");
                    success = false;
                    continue;
                }

                bhBar.SectionProperty.SetRFEM6ID(sectionNo);

                int lineNo = bhBar.FindFragment<RFEMLine>()?.GetRFEM6ID() ?? 0;
                if (existingMembersByLine.TryGetValue(lineNo, out int existingNo))
                {
                    bhBar.SetRFEM6ID(existingNo);
                    reusedCount++;
                    continue;
                }

                bhBar.SetRFEM6ID(ClaimNumber(bhBar.GetRFEM6ID(), usedNumbers));
                StructureCore.Member member = bhBar.ToRFEM6();
                if (lineNo > 0)
                    existingMembersByLine[lineNo] = member.No;

                if (bhBar.Release?.StartRelease != null && !bhBar.Release.StartRelease.IsFullyFixed())
                    member.MemberHingeStart = GetOrQueueMemberHinge(bhBar.Release.StartRelease);
                if (bhBar.Release?.EndRelease != null && !bhBar.Release.EndRelease.IsFullyFixed())
                    member.MemberHingeEnd = GetOrQueueMemberHinge(bhBar.Release.EndRelease);

                members.Add(member);
            }

            if (reusedCount > 0)
                BH.Engine.Base.Compute.RecordNote($"{reusedCount} bar(s) lie on RFEM lines that already have a member. The existing members were kept and not modified.");

            CreateObjectList(members);
            return success;
        }

        /// <summary>
        /// Members reference hinges by number, so hinges already in RFEM can be reused without an update request.
        /// </summary>
        private int GetOrQueueMemberHinge(Constraint6DOF release)
        {
            double[] stiffnesses = release.ToRFEM6Stiffnesses();

            TypesForMembers.MemberHinge hinge = GetObjectList(new TypesForMembers.MemberHinge())
                .Concat(m_createQueue.OfType<TypesForMembers.MemberHinge>())
                .FirstOrDefault(h => Convert.StiffnessesMatch(stiffnesses, new[] { h.AxialReleaseN, h.AxialReleaseVy, h.AxialReleaseVz, h.MomentReleaseMt, h.MomentReleaseMy, h.MomentReleaseMz }));

            if (hinge == null)
            {
                hinge = release.ToRFEM6MemberHinge(ClaimNumber(0, UsedNumbers<TypesForMembers.MemberHinge>()));
                CreateObject(hinge);
            }

            return hinge.No;
        }
    }
}
