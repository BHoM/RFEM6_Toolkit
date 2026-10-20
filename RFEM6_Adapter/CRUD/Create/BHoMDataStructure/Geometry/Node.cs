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
using BH.Engine.Base;
using BH.Engine.Structure;
using BH.oM.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {
            return QueueNodes(bhNodes);
        }

        private bool UpdateObjects(IEnumerable<Node> nodes)
        {
            return QueueNodes(nodes);
        }

        private bool QueueNodes(IEnumerable<Node> bhNodes)
        {
            HashSet<int> usedNumbers = UsedNumbers<StructureCore.Node>();
            List<StructureCore.Node> nodesToCreate = new List<StructureCore.Node>();

            // Duplicate nodes break surface boundaries in RFEM, so nodes already in the model at the same position are reused.
            Dictionary<string, int> existingNodes = new Dictionary<string, int>();
            foreach (StructureCore.Node rfNode in GetObjectList(new StructureCore.Node()).Where(n => n.No > 0))
            {
                string key = PositionKey(rfNode.Coordinate1, rfNode.Coordinate2, rfNode.Coordinate3);
                if (!existingNodes.ContainsKey(key))
                    existingNodes[key] = rfNode.No;
            }

            foreach (Node bhNode in bhNodes)
            {
                int nodeNo;
                if (bhNode.Position != null && existingNodes.TryGetValue(PositionKey(bhNode.Position.X, bhNode.Position.Y, bhNode.Position.Z), out int existingNo))
                {
                    nodeNo = existingNo;
                    bhNode.SetRFEM6ID(nodeNo);
                }
                else
                {
                    nodeNo = ClaimNumber(bhNode.GetRFEM6ID(), usedNumbers);
                    bhNode.SetRFEM6ID(nodeNo);
                    nodesToCreate.Add(bhNode.ToRFEM6());
                }

                if (bhNode.Support != null)
                    QueueNodalSupport(bhNode.Support, nodeNo);
            }

            CreateObjectList(nodesToCreate);
            return true;
        }

        /// <summary>
        /// Coordinates rounded to the millimetre, matching the tolerance of the node comparer.
        /// </summary>
        private static string PositionKey(double x, double y, double z)
        {
            return $"{Math.Round(x, 3) + 0.0:F3},{Math.Round(y, 3) + 0.0:F3},{Math.Round(z, 3) + 0.0:F3}";
        }

        /// <summary>
        /// Adds the node to a support with the same stiffnesses queued in this push, or queues a new support.
        /// Supports already in RFEM are not reused because adding nodes to them would need an extra update request.
        /// </summary>
        private void QueueNodalSupport(Constraint6DOF constraint, int nodeNo)
        {
            double[] stiffnesses = constraint.ToRFEM6Stiffnesses();

            TypesForNodes.NodalSupport support = m_createQueue.OfType<TypesForNodes.NodalSupport>().FirstOrDefault(s =>
                Convert.StiffnessesMatch(stiffnesses, new[] { s.SpringX, s.SpringY, s.SpringZ, s.RotationalRestraintX, s.RotationalRestraintY, s.RotationalRestraintZ }));

            if (support == null)
            {
                support = constraint.ToRFEM6NodalSupport(ClaimNumber(0, UsedNumbers<TypesForNodes.NodalSupport>()));
                constraint.SetRFEM6ID(support.No);
                CreateObject(support);
            }

            if (!support.Nodes.Contains(nodeNo))
                support.Nodes.Add(nodeNo);
        }
    }
}













