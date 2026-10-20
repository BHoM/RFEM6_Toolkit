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

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using BH.oM.Adapters.RFEM6;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Node> ReadNodes(List<string> ids = null)
        {

            List<Node> nodeList = new List<Node>();

            List<StructureCore.Node> allRfNodes = GetObjectList(new StructureCore.Node()).Where(n => n.No != 0).ToList();
            Dictionary<int, Constraint6DOF> supports = ReadNodalSupports().ToDictionary(s => s.GetRFEM6ID(), s => s.Constraint);

            if (ids == null)
            {
                foreach (StructureCore.Node rfNode in allRfNodes)
                {
                    Node node = rfNode.FromRFEM();
                    if (rfNode.Support > 0 && supports.TryGetValue(rfNode.Support, out Constraint6DOF support))
                        node.Support = support;
                    nodeList.Add(node);
                }
            }

            return nodeList;
        }

    }
}



