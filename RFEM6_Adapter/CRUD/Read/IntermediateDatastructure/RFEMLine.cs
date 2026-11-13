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

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using StructureCore = Dlubal.Api.Rfem.StructureCore;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<RFEMLine> ReadLines(List<string> ids = null)
        {

            List<RFEMLine> lineList = new List<RFEMLine>();

            List<StructureCore.Line> allRfLInes = GetObjectList(new StructureCore.Line()).Where(l => l.No > 0).ToList();

            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();
            Dictionary<int, Constraint6DOF> supports = ReadLineSupports().ToDictionary(s => s.GetRFEM6ID(), s => s.Constraint);

            if (ids == null)
            {
                foreach (StructureCore.Line rfLine in allRfLInes)
                {
                    RFEMLine geometry = Convert.FromRFEM(rfLine, nodes);
                    if (geometry?.Curve == null)
                        continue;

                    RFEMLine l = geometry;

                    if (!string.IsNullOrWhiteSpace(rfLine.Comment))
                        l.Name = rfLine.Comment;

                    if (rfLine.Support > 0 && supports.TryGetValue(rfLine.Support, out Constraint6DOF support))
                    {
                        l.Support = support;
                        l.supportID = rfLine.Support;
                    }

                    l.SetRFEM6ID(rfLine.No);
                    lineList.Add(l);
                }
            }

            return lineList;
        }

    }
}


