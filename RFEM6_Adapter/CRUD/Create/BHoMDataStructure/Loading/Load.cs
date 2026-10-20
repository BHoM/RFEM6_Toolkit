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
using System.Reflection;

using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        // BHoM caches every created object by its id, so a load that is not queued must make this method return false.
        private bool CreateCollection(IEnumerable<ILoad> bhLoads)
        {
            bool success = true;

            foreach (ILoad bhLoad in bhLoads)
            {
                int loadCase = bhLoad.Loadcase.ToRFEM6Number();
                if (loadCase <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Load '{bhLoad.Name}' has no load case number, so it cannot be pushed to RFEM6.");
                    success = false;
                    continue;
                }

                int loadNo;
                switch (bhLoad)
                {
                    case PointLoad pointLoad:
                        loadNo = QueueLoads(loadCase, new[] { pointLoad.ToRFEM6(ElementNumbers(pointLoad.Objects?.Elements)) });
                        break;
                    case BarUniformlyDistributedLoad barLoad:
                        loadNo = QueueLoads(loadCase, barLoad.ToRFEM6(ElementNumbers(barLoad.Objects?.Elements)));
                        break;
                    case AreaUniformlyDistributedLoad areaLoad:
                        Polygon polygon = areaLoad.CustomData.Values.OfType<Polygon>().FirstOrDefault();
                        if (polygon != null)
                            loadNo = QueueLoads(loadCase, areaLoad.ToRFEM6(polygon, ElementNumbers(areaLoad.Objects?.Elements)));
                        else
                            loadNo = QueueLoads(loadCase, areaLoad.ToRFEM6(ElementNumbers(areaLoad.Objects?.Elements)));
                        break;
                    case GeometricalLineLoad lineLoad:
                        loadNo = QueueLineLoad(lineLoad, loadCase);
                        break;
                    default:
                        BH.Engine.Base.Compute.RecordError($"Loads of type {bhLoad.GetType().Name} are not supported by the RFEM6 adapter.");
                        loadNo = 0;
                        break;
                }

                if (loadNo > 0)
                    bhLoad.SetRFEM6ID(loadNo);
                else
                    success = false;
            }

            return success;
        }

        private int QueueLineLoad(GeometricalLineLoad bhLoad, int loadCase)
        {
            RFEM6GeometricalLineLoadTypes typeFragment = bhLoad.FindFragment<RFEM6GeometricalLineLoadTypes>();

            if (typeFragment?.geometrialLineLoadType == GeometricalLineLoadTypesEnum.NonFreeLineLoad)
            {
                int lineNo = FindLineNumber(bhLoad.Location);
                if (lineNo <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"The location of line load '{bhLoad.Name}' does not match any straight line in RFEM6 or in this push. The location must match a line's start and end points, in the same direction.");
                    return 0;
                }

                return QueueLoads(loadCase, bhLoad.ToRFEM6LineLoad(lineNo));
            }

            if (typeFragment == null)
                BH.Engine.Base.Compute.RecordWarning($"Line load '{bhLoad.Name}' has no RFEM6GeometricalLineLoadTypes fragment and is pushed as a free line load. Add the fragment with NonFreeLineLoad to load an existing line instead.");

            if ((bhLoad.MomentA?.Length() ?? 0) > 0 || (bhLoad.MomentB?.Length() ?? 0) > 0)
            {
                BH.Engine.Base.Compute.RecordError($"Free line loads cannot carry moments. Remove the moments from line load '{bhLoad.Name}'.");
                return 0;
            }

            List<int> surfaceNos = ElementNumbers(bhLoad.Objects?.Elements).ToList();
            if (surfaceNos.Count == 0)
            {
                surfaceNos = GetObjectList(new StructureCore.Surface()).Select(s => s.No)
                    .Concat(m_createQueue.OfType<StructureCore.Surface>().Select(s => s.No))
                    .Where(n => n > 0).Distinct().ToList();
            }

            return QueueLoads(loadCase, bhLoad.ToRFEM6FreeLineLoad(surfaceNos));
        }

        /// <summary>
        /// Numbers the loads within their load case and queues them for create_object_list. Returns the first number, or 0 if nothing was queued.
        /// </summary>
        private int QueueLoads<T>(int loadCase, IEnumerable<T> loads) where T : class, new()
        {
            HashSet<int> usedNumbers = UsedLoadNumbers<T>(loadCase);
            PropertyInfo numberProperty = typeof(T).GetProperty("No");
            int firstNo = 0;

            foreach (T load in loads)
            {
                int loadNo = ClaimNumber(0, usedNumbers);
                numberProperty.SetValue(load, loadNo);
                CreateObject((IMessage)load);

                if (firstNo == 0)
                    firstNo = loadNo;
            }

            return firstNo;
        }

        private static IEnumerable<int> ElementNumbers(IEnumerable<IBHoMObject> elements)
        {
            return elements?.Where(e => e != null).Select(e => e.GetRFEM6ID()) ?? Enumerable.Empty<int>();
        }

        /// <summary>
        /// Number of the straight line, in RFEM or queued in this push, that runs from the start to the end of the location.
        /// </summary>
        private int FindLineNumber(Line location, double tolerance = 1e-4)
        {
            if (location == null)
                return 0;

            Dictionary<int, Point> nodePositions = new Dictionary<int, Point>();
            foreach (StructureCore.Node node in GetObjectList(new StructureCore.Node()).Concat(m_createQueue.OfType<StructureCore.Node>()))
                nodePositions[node.No] = new Point { X = node.Coordinate1, Y = node.Coordinate2, Z = node.Coordinate3 };

            bool Matches(int nodeNo, Point point) =>
                nodePositions.TryGetValue(nodeNo, out Point position) && position.Distance(point) < tolerance;

            StructureCore.Line match = GetObjectList(new StructureCore.Line())
                .Concat(m_createQueue.OfType<StructureCore.Line>())
                .FirstOrDefault(l => l.Type == StructureCore.Line.Types.Type.Polyline
                    && l.DefinitionNodes.Count == 2
                    && Matches(l.DefinitionNodes[0], location.Start)
                    && Matches(l.DefinitionNodes[1], location.End));

            return match?.No ?? 0;
        }
    }
}
