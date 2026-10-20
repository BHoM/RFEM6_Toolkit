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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        private const double LoadTolerance = 1e-8;

        // Load numbers are assigned per load case by the Create methods, so converted loads have No=0.

        public static RfLoads.NodalLoad ToRFEM6(this PointLoad bhLoad, IEnumerable<int> nodeNos)
        {
            if (bhLoad.Axis == LoadAxis.Local)
                BH.Engine.Base.Compute.RecordWarning($"PointLoad '{bhLoad.Name}' uses a local axis, which the RFEM6 adapter does not support. It is pushed in global axes.");

            RfLoads.NodalLoad rfLoad = new RfLoads.NodalLoad
            {
                LoadCase = bhLoad.Loadcase.ToRFEM6Number(),
                LoadType = RfLoads.NodalLoad.Types.LoadType.Components,
                ComponentsForceX = bhLoad.Force?.X ?? 0,
                ComponentsForceY = bhLoad.Force?.Y ?? 0,
                ComponentsForceZ = bhLoad.Force?.Z ?? 0,
                ComponentsMomentX = bhLoad.Moment?.X ?? 0,
                ComponentsMomentY = bhLoad.Moment?.Y ?? 0,
                ComponentsMomentZ = bhLoad.Moment?.Z ?? 0,
            };
            rfLoad.Nodes.AddRange(nodeNos);
            SetLoadComment(bhLoad, c => rfLoad.Comment = c);

            return rfLoad;
        }

        /// <summary>
        /// RFEM member loads act in one direction only, so each non-zero force and moment component becomes its own load.
        /// </summary>
        public static List<RfLoads.MemberLoad> ToRFEM6(this BarUniformlyDistributedLoad bhLoad, IEnumerable<int> memberNos)
        {
            WarnProjectedLocal(bhLoad.Name, bhLoad.Axis, bhLoad.Projected);

            List<RfLoads.MemberLoad> loads = new List<RfLoads.MemberLoad>();

            foreach ((int axis, double value, _) in NonZeroComponents(bhLoad.Force))
                loads.Add(MemberLoad(bhLoad, memberNos, RfLoads.MemberLoad.Types.LoadType.Force, LoadDirection<RfLoads.MemberLoad.Types.LoadDirection>(axis, bhLoad.Axis, bhLoad.Projected), value));

            foreach ((int axis, double value, _) in NonZeroComponents(bhLoad.Moment))
                loads.Add(MemberLoad(bhLoad, memberNos, RfLoads.MemberLoad.Types.LoadType.Moment, LoadDirection<RfLoads.MemberLoad.Types.LoadDirection>(axis, bhLoad.Axis, false), value));

            if (loads.Count == 0)
                BH.Engine.Base.Compute.RecordWarning($"BarUniformlyDistributedLoad '{bhLoad.Name}' has no force or moment and was not pushed.");

            return loads;
        }

        public static List<RfLoads.SurfaceLoad> ToRFEM6(this AreaUniformlyDistributedLoad bhLoad, IEnumerable<int> surfaceNos)
        {
            WarnProjectedLocal(bhLoad.Name, bhLoad.Axis, bhLoad.Projected);

            List<RfLoads.SurfaceLoad> loads = new List<RfLoads.SurfaceLoad>();
            foreach ((int axis, double value, _) in NonZeroComponents(bhLoad.Pressure))
            {
                RfLoads.SurfaceLoad rfLoad = new RfLoads.SurfaceLoad
                {
                    LoadCase = bhLoad.Loadcase.ToRFEM6Number(),
                    LoadType = RfLoads.SurfaceLoad.Types.LoadType.Force,
                    LoadDistribution = RfLoads.SurfaceLoad.Types.LoadDistribution.Uniform,
                    LoadDirection = LoadDirection<RfLoads.SurfaceLoad.Types.LoadDirection>(axis, bhLoad.Axis, bhLoad.Projected),
                    UniformMagnitude = value,
                };
                rfLoad.Surfaces.AddRange(surfaceNos);
                SetLoadComment(bhLoad, c => rfLoad.Comment = c);
                loads.Add(rfLoad);
            }

            return loads;
        }

        /// <summary>
        /// Area load limited to a polygon. The polygon must lie in a plane parallel to XY, XZ or YZ.
        /// </summary>
        public static List<RfLoads.FreePolygonLoad> ToRFEM6(this AreaUniformlyDistributedLoad bhLoad, Polygon polygon, IEnumerable<int> surfaceNos)
        {
            WarnProjectedLocal(bhLoad.Name, bhLoad.Axis, bhLoad.Projected);

            Vector normal = polygon.IFitPlane().Normal;
            RfLoads.FreePolygonLoad.Types.LoadProjection projection;
            Func<Point, double[]> toPlane;

            if (normal.CrossProduct(Plane.XY.Normal).Length() < 0.01)
            {
                projection = RfLoads.FreePolygonLoad.Types.LoadProjection.XyOrUv;
                toPlane = p => new[] { p.X, p.Y };
            }
            else if (normal.CrossProduct(Plane.XZ.Normal).Length() < 0.01)
            {
                projection = RfLoads.FreePolygonLoad.Types.LoadProjection.XzOrUw;
                toPlane = p => new[] { p.X, p.Z };
            }
            else
            {
                if (normal.CrossProduct(Plane.YZ.Normal).Length() >= 0.01)
                    BH.Engine.Base.Compute.RecordWarning($"The polygon of area load '{bhLoad.Name}' is not parallel to XY, XZ or YZ. It is projected onto YZ.");

                projection = RfLoads.FreePolygonLoad.Types.LoadProjection.YzOrVw;
                toPlane = p => new[] { p.Y, p.Z };
            }

            List<RfLoads.FreePolygonLoad> loads = new List<RfLoads.FreePolygonLoad>();
            foreach ((int axis, double value, _) in NonZeroComponents(bhLoad.Pressure))
            {
                RfLoads.FreePolygonLoad rfLoad = new RfLoads.FreePolygonLoad
                {
                    LoadCase = bhLoad.Loadcase.ToRFEM6Number(),
                    LoadProjection = projection,
                    LoadDistribution = RfLoads.FreePolygonLoad.Types.LoadDistribution.Uniform,
                    LoadDirection = LoadDirection<RfLoads.FreePolygonLoad.Types.LoadDirection>(axis, bhLoad.Axis, bhLoad.Projected),
                    MagnitudeUniform = value,
                    LoadLocation = new RfLoads.FreePolygonLoad.Types.LoadLocationTable(),
                };

                int row = 1;
                foreach (Point vertex in polygon.Vertices)
                {
                    double[] uv = toPlane(vertex);
                    rfLoad.LoadLocation.Rows.Add(new RfLoads.FreePolygonLoad.Types.LoadLocationRow { No = row++, FirstCoordinate = uv[0], SecondCoordinate = uv[1] });
                }

                rfLoad.Surfaces.AddRange(surfaceNos);
                SetLoadComment(bhLoad, c => rfLoad.Comment = c);
                loads.Add(rfLoad);
            }

            return loads;
        }

        /// <summary>
        /// Free line loads carry forces only and are placed by their XY coordinates on the given surfaces.
        /// </summary>
        public static List<RfLoads.FreeLineLoad> ToRFEM6FreeLineLoad(this GeometricalLineLoad bhLoad, IEnumerable<int> surfaceNos)
        {
            WarnProjectedLocal(bhLoad.Name, bhLoad.Axis, bhLoad.Projected);

            List<RfLoads.FreeLineLoad> loads = new List<RfLoads.FreeLineLoad>();
            foreach ((int axis, double first, double second) in NonZeroComponents(bhLoad.ForceA, bhLoad.ForceB))
            {
                RfLoads.FreeLineLoad rfLoad = new RfLoads.FreeLineLoad
                {
                    LoadCase = bhLoad.Loadcase.ToRFEM6Number(),
                    LoadProjection = RfLoads.FreeLineLoad.Types.LoadProjection.XyOrUv,
                    LoadDistribution = RfLoads.FreeLineLoad.Types.LoadDistribution.Linear,
                    LoadDirection = LoadDirection<RfLoads.FreeLineLoad.Types.LoadDirection>(axis, bhLoad.Axis, bhLoad.Projected),
                    MagnitudeFirst = first,
                    MagnitudeSecond = second,
                    LoadLocationFirstPointCoordinate1 = bhLoad.Location.Start.X,
                    LoadLocationFirstPointCoordinate2 = bhLoad.Location.Start.Y,
                    LoadLocationSecondPointCoordinate1 = bhLoad.Location.End.X,
                    LoadLocationSecondPointCoordinate2 = bhLoad.Location.End.Y,
                };
                rfLoad.Surfaces.AddRange(surfaceNos);
                SetLoadComment(bhLoad, c => rfLoad.Comment = c);
                loads.Add(rfLoad);
            }

            return loads;
        }

        public static List<RfLoads.LineLoad> ToRFEM6LineLoad(this GeometricalLineLoad bhLoad, int lineNo)
        {
            WarnProjectedLocal(bhLoad.Name, bhLoad.Axis, bhLoad.Projected);

            List<RfLoads.LineLoad> loads = new List<RfLoads.LineLoad>();

            foreach ((int axis, double first, double second) in NonZeroComponents(bhLoad.ForceA, bhLoad.ForceB))
                loads.Add(LineLoad(bhLoad, lineNo, RfLoads.LineLoad.Types.LoadType.Force, LoadDirection<RfLoads.LineLoad.Types.LoadDirection>(axis, bhLoad.Axis, bhLoad.Projected), first, second));

            foreach ((int axis, double first, double second) in NonZeroComponents(bhLoad.MomentA, bhLoad.MomentB))
                loads.Add(LineLoad(bhLoad, lineNo, RfLoads.LineLoad.Types.LoadType.Moment, LoadDirection<RfLoads.LineLoad.Types.LoadDirection>(axis, bhLoad.Axis, false), first, second));

            return loads;
        }

        public static int ToRFEM6Number(this Loadcase loadcase)
        {
            if (loadcase == null)
                return 0;

            int id = loadcase.GetRFEM6ID();
            return id > 0 ? id : loadcase.Number;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static RfLoads.MemberLoad MemberLoad(BarUniformlyDistributedLoad bhLoad, IEnumerable<int> memberNos, RfLoads.MemberLoad.Types.LoadType loadType, RfLoads.MemberLoad.Types.LoadDirection direction, double magnitude)
        {
            RfLoads.MemberLoad rfLoad = new RfLoads.MemberLoad
            {
                LoadCase = bhLoad.Loadcase.ToRFEM6Number(),
                LoadType = loadType,
                LoadDistribution = RfLoads.MemberLoad.Types.LoadDistribution.Uniform,
                LoadDirection = direction,
                Magnitude = magnitude,
                LoadIsOverTotalLength = true,
            };
            rfLoad.Members.AddRange(memberNos);
            SetLoadComment(bhLoad, c => rfLoad.Comment = c);
            return rfLoad;
        }

        private static RfLoads.LineLoad LineLoad(GeometricalLineLoad bhLoad, int lineNo, RfLoads.LineLoad.Types.LoadType loadType, RfLoads.LineLoad.Types.LoadDirection direction, double first, double second)
        {
            RfLoads.LineLoad rfLoad = new RfLoads.LineLoad
            {
                LoadCase = bhLoad.Loadcase.ToRFEM6Number(),
                LoadType = loadType,
                LoadDistribution = RfLoads.LineLoad.Types.LoadDistribution.Trapezoidal,
                LoadDirection = direction,
                Magnitude1 = first,
                Magnitude2 = second,
                LoadIsOverTotalLength = true,
            };
            rfLoad.Lines.Add(lineNo);
            SetLoadComment(bhLoad, c => rfLoad.Comment = c);
            return rfLoad;
        }

        // Every RFEM load type has the same direction names: LocalX..Z, GlobalXOrUserDefinedU{True|Projected}Length..Z.
        private static T LoadDirection<T>(int axis, LoadAxis loadAxis, bool projected) where T : struct
        {
            string xyz = "XYZ".Substring(axis, 1);
            string name = loadAxis == LoadAxis.Local
                ? "Local" + xyz
                : $"Global{xyz}OrUserDefined{"UVW".Substring(axis, 1)}{(projected ? "ProjectedLength" : "TrueLength")}";

            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// Axis index (0=X, 1=Y, 2=Z) and values of each component that is non-zero in either vector.
        /// </summary>
        private static List<(int Axis, double A, double B)> NonZeroComponents(Vector a, Vector b = null)
        {
            double[] va = a == null ? new double[3] : new[] { a.X, a.Y, a.Z };
            double[] vb = b == null ? new double[3] : new[] { b.X, b.Y, b.Z };

            List<(int, double, double)> components = new List<(int, double, double)>();
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(va[i]) > LoadTolerance || Math.Abs(vb[i]) > LoadTolerance)
                    components.Add((i, va[i], vb[i]));
            }

            return components;
        }

        private static void WarnProjectedLocal(string loadName, LoadAxis axis, bool projected)
        {
            if (projected && axis == LoadAxis.Local)
                BH.Engine.Base.Compute.RecordWarning($"Load '{loadName}' is projected in local axes, which RFEM6 does not support. The projection is ignored.");
        }

        private static void SetLoadComment(ILoad bhLoad, Action<string> setComment)
        {
            if (!string.IsNullOrWhiteSpace(bhLoad.Name))
                setComment(bhLoad.Name);
        }
    }
}
