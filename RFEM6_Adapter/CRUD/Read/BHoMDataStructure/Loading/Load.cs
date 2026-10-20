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

using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private List<ILoad> ReadLoads(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();
            loads.AddRange(ReadPointLoads());
            loads.AddRange(ReadBarUniformlyDistributedLoads());
            loads.AddRange(ReadAreaUniformlyDistributedLoads());
            loads.AddRange(ReadGeometricalLineLoads());
            return loads;
        }

        private List<PointLoad> ReadPointLoads(List<string> ids = null)
        {
            Dictionary<int, Loadcase> loadcases = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();

            List<PointLoad> loads = GetObjectList(new RfLoads.NodalLoad())
                .Where(l => l.No > 0)
                .Select(l => l.FromRFEM(Lookup(nodes, l.Nodes), Lookup(loadcases, l.LoadCase)))
                .Where(l => l != null)
                .ToList();

            return MergeLoads(loads, l => Key(l, l.Objects), (a, b) =>
            {
                a.Force = Add(a.Force, b.Force);
                a.Moment = Add(a.Moment, b.Moment);
            });
        }

        private List<BarUniformlyDistributedLoad> ReadBarUniformlyDistributedLoads(List<string> ids = null)
        {
            Dictionary<int, Loadcase> loadcases = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Bar> bars = this.GetCachedOrReadAsDictionary<int, Bar>();

            List<BarUniformlyDistributedLoad> loads = GetObjectList(new RfLoads.MemberLoad())
                .Where(l => l.No > 0)
                .Select(l => l.FromRFEM(Lookup(bars, l.Members), Lookup(loadcases, l.LoadCase)))
                .Where(l => l != null)
                .ToList();

            return MergeLoads(loads, l => $"{Key(l, l.Objects)}|{l.Axis}|{l.Projected}", (a, b) =>
            {
                a.Force = Add(a.Force, b.Force);
                a.Moment = Add(a.Moment, b.Moment);
            });
        }

        private List<AreaUniformlyDistributedLoad> ReadAreaUniformlyDistributedLoads(List<string> ids = null)
        {
            Dictionary<int, Loadcase> loadcases = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Panel> panels = this.GetCachedOrReadAsDictionary<int, Panel>();

            List<AreaUniformlyDistributedLoad> loads = GetObjectList(new RfLoads.SurfaceLoad())
                .Where(l => l.No > 0)
                .Select(l => l.FromRFEM(Lookup(panels, l.Surfaces), Lookup(loadcases, l.LoadCase)))
                .Concat(GetObjectList(new RfLoads.FreePolygonLoad())
                    .Where(l => l.No > 0)
                    .Select(l => l.FromRFEM(Lookup(panels, l.Surfaces), Lookup(loadcases, l.LoadCase))))
                .Where(l => l != null)
                .ToList();

            return MergeLoads(loads, l => $"{Key(l, l.Objects)}|{l.Axis}|{l.Projected}|{PolygonKey(l)}", (a, b) => a.Pressure = Add(a.Pressure, b.Pressure));
        }

        private List<GeometricalLineLoad> ReadGeometricalLineLoads(List<string> ids = null)
        {
            Dictionary<int, Loadcase> loadcases = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Panel> panels = this.GetCachedOrReadAsDictionary<int, Panel>();

            List<GeometricalLineLoad> loads = GetObjectList(new RfLoads.FreeLineLoad())
                .Where(l => l.No > 0)
                .Select(l => l.FromRFEM(Lookup(panels, l.Surfaces), Lookup(loadcases, l.LoadCase)))
                .ToList();

            List<RfLoads.LineLoad> rfLineLoads = GetObjectList(new RfLoads.LineLoad()).Where(l => l.No > 0).ToList();
            if (rfLineLoads.Count > 0)
            {
                Dictionary<int, RFEMLine> lines = this.GetCachedOrReadAsDictionary<int, RFEMLine>();
                foreach (RfLoads.LineLoad rfLoad in rfLineLoads)
                {
                    foreach (int lineNo in rfLoad.Lines)
                    {
                        Line location = lines.TryGetValue(lineNo, out RFEMLine line) && line.Nodes.Count == 2 && line.Nodes.All(n => n != null)
                            ? new Line { Start = line.Nodes[0].Position, End = line.Nodes[1].Position }
                            : null;

                        if (location == null)
                        {
                            BH.Engine.Base.Compute.RecordWarning($"Line load {rfLoad.No} in load case {rfLoad.LoadCase} is on line {lineNo}, which is not a straight line between two nodes. It is not read.");
                            continue;
                        }

                        loads.Add(rfLoad.FromRFEM(location, Lookup(loadcases, rfLoad.LoadCase)));
                    }
                }
            }

            return MergeLoads(loads.Where(l => l != null).ToList(), l => $"{Key(l, l.Objects)}|{l.Axis}|{l.Projected}|{l.FindFragment<RFEM6GeometricalLineLoadTypes>()?.geometrialLineLoadType}|{PointKey(l.Location?.Start)}|{PointKey(l.Location?.End)}", (a, b) =>
            {
                a.ForceA = Add(a.ForceA, b.ForceA);
                a.ForceB = Add(a.ForceB, b.ForceB);
                a.MomentA = Add(a.MomentA, b.MomentA);
                a.MomentB = Add(a.MomentB, b.MomentB);
            });
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<T> Lookup<T>(Dictionary<int, T> objects, IEnumerable<int> numbers)
        {
            return numbers.Select(n => objects.TryGetValue(n, out T obj) ? obj : default(T)).Where(o => o != null).ToList();
        }

        private static Loadcase Lookup(Dictionary<int, Loadcase> loadcases, int loadCaseNo)
        {
            return loadcases.TryGetValue(loadCaseNo, out Loadcase loadcase) ? loadcase : null;
        }

        /// <summary>
        /// Loads that were pushed as one BHoM load are split into one RFEM load per axis. Loads with the same key are summed back into one.
        /// </summary>
        private static List<T> MergeLoads<T>(List<T> loads, Func<T, string> key, Action<T, T> add) where T : ILoad
        {
            List<T> merged = new List<T>();
            foreach (IGrouping<string, T> group in loads.GroupBy(key))
            {
                T first = group.First();
                foreach (T other in group.Skip(1))
                    add(first, other);
                merged.Add(first);
            }
            return merged;
        }

        private static string Key<T>(ILoad load, BHoMGroup<T> objects) where T : IBHoMObject
        {
            IEnumerable<object> ids = objects?.Elements?.Select(e => e?.GetRFEM6ID()).Cast<object>() ?? Enumerable.Empty<object>();
            return $"{load.Loadcase?.Number}|{load.Name}|{string.Join(",", ids)}";
        }

        private static string PolygonKey(IBHoMObject load)
        {
            if (!(load.PropertyValue("Polygon") is Polygon polygon))
                return "";

            return string.Join(";", polygon.Vertices.Select(PointKey));
        }

        private static string PointKey(Point point)
        {
            return point == null ? "" : $"{Math.Round(point.X, 6)},{Math.Round(point.Y, 6)},{Math.Round(point.Z, 6)}";
        }

        private static Vector Add(Vector a, Vector b)
        {
            if (a == null || b == null)
                return a ?? b;

            return new Vector { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
        }
    }
}
