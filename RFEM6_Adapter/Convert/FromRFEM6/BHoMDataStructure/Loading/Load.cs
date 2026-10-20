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
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        // Each RFEM load acts along one axis. The Read methods merge loads that were split per axis on push.

        public static PointLoad FromRFEM(this RfLoads.NodalLoad rfLoad, List<Node> nodes, Loadcase loadcase)
        {
            Vector force = new Vector();
            Vector moment = new Vector();

            switch (rfLoad.LoadType)
            {
                case RfLoads.NodalLoad.Types.LoadType.Components:
                    force = new Vector { X = rfLoad.ComponentsForceX, Y = rfLoad.ComponentsForceY, Z = rfLoad.ComponentsForceZ };
                    moment = new Vector { X = rfLoad.ComponentsMomentX, Y = rfLoad.ComponentsMomentY, Z = rfLoad.ComponentsMomentZ };
                    break;
                case RfLoads.NodalLoad.Types.LoadType.Force:
                case RfLoads.NodalLoad.Types.LoadType.Moment:
                    if (!TryParseLoadDirection(rfLoad.LoadDirection.ToString(), out int axis, out LoadAxis loadAxis, out _))
                        return null;
                    if (loadAxis == LoadAxis.Local)
                        BH.Engine.Base.Compute.RecordWarning($"Nodal load {rfLoad.No} in load case {rfLoad.LoadCase} acts in a local direction and is read in global axes.");
                    if (rfLoad.LoadType == RfLoads.NodalLoad.Types.LoadType.Force)
                        force = AxisVector(axis, rfLoad.ForceMagnitude);
                    else
                        moment = AxisVector(axis, rfLoad.MomentMagnitude);
                    break;
                default:
                    BH.Engine.Base.Compute.RecordWarning($"Nodal load {rfLoad.No} in load case {rfLoad.LoadCase} is of type {rfLoad.LoadType}, which cannot be read. Only force, moment and component loads are supported.");
                    return null;
            }

            PointLoad bhLoad = new PointLoad
            {
                Name = rfLoad.Comment,
                Loadcase = loadcase,
                Objects = new BHoMGroup<Node> { Elements = nodes },
                Force = force,
                Moment = moment,
                Axis = LoadAxis.Global,
            };
            bhLoad.SetRFEM6ID(rfLoad.No);
            return bhLoad;
        }

        public static BarUniformlyDistributedLoad FromRFEM(this RfLoads.MemberLoad rfLoad, List<Bar> bars, Loadcase loadcase)
        {
            bool isForce = rfLoad.LoadType == RfLoads.MemberLoad.Types.LoadType.Force;
            if ((!isForce && rfLoad.LoadType != RfLoads.MemberLoad.Types.LoadType.Moment) || rfLoad.LoadDistribution != RfLoads.MemberLoad.Types.LoadDistribution.Uniform)
            {
                BH.Engine.Base.Compute.RecordWarning($"Member load {rfLoad.No} in load case {rfLoad.LoadCase} is a {rfLoad.LoadDistribution} {rfLoad.LoadType} load. Only uniform force and moment loads can be read.");
                return null;
            }

            if (!TryParseLoadDirection(rfLoad.LoadDirection.ToString(), out int axis, out LoadAxis loadAxis, out bool projected))
                return null;

            BarUniformlyDistributedLoad bhLoad = new BarUniformlyDistributedLoad
            {
                Name = rfLoad.Comment,
                Loadcase = loadcase,
                Objects = new BHoMGroup<Bar> { Elements = bars },
                Force = isForce ? AxisVector(axis, rfLoad.Magnitude) : new Vector(),
                Moment = isForce ? new Vector() : AxisVector(axis, rfLoad.Magnitude),
                Axis = loadAxis,
                Projected = projected,
            };
            bhLoad.SetRFEM6ID(rfLoad.No);
            return bhLoad;
        }

        public static AreaUniformlyDistributedLoad FromRFEM(this RfLoads.SurfaceLoad rfLoad, List<Panel> panels, Loadcase loadcase)
        {
            if (rfLoad.LoadType != RfLoads.SurfaceLoad.Types.LoadType.Force || rfLoad.LoadDistribution != RfLoads.SurfaceLoad.Types.LoadDistribution.Uniform)
            {
                BH.Engine.Base.Compute.RecordWarning($"Surface load {rfLoad.No} in load case {rfLoad.LoadCase} is a {rfLoad.LoadDistribution} {rfLoad.LoadType} load. Only uniform force loads can be read.");
                return null;
            }

            if (!TryParseLoadDirection(rfLoad.LoadDirection.ToString(), out int axis, out LoadAxis loadAxis, out bool projected))
                return null;

            AreaUniformlyDistributedLoad bhLoad = new AreaUniformlyDistributedLoad
            {
                Name = rfLoad.Comment,
                Loadcase = loadcase,
                Objects = new BHoMGroup<IAreaElement> { Elements = panels.Cast<IAreaElement>().ToList() },
                Pressure = AxisVector(axis, rfLoad.UniformMagnitude),
                Axis = loadAxis,
                Projected = projected,
            };
            bhLoad.SetRFEM6ID(rfLoad.No);
            return bhLoad;
        }

        public static AreaUniformlyDistributedLoad FromRFEM(this RfLoads.FreePolygonLoad rfLoad, List<Panel> panels, Loadcase loadcase)
        {
            if (rfLoad.LoadDistribution != RfLoads.FreePolygonLoad.Types.LoadDistribution.Uniform)
            {
                BH.Engine.Base.Compute.RecordWarning($"Free polygon load {rfLoad.No} in load case {rfLoad.LoadCase} has a {rfLoad.LoadDistribution} distribution. Only uniform polygon loads can be read.");
                return null;
            }

            if (!TryParseLoadDirection(rfLoad.LoadDirection.ToString(), out int axis, out LoadAxis loadAxis, out bool projected))
                return null;

            List<Point> vertices = (rfLoad.LoadLocation?.Rows ?? Enumerable.Empty<RfLoads.FreePolygonLoad.Types.LoadLocationRow>())
                .Select(r => FromProjectedCoordinates(rfLoad.LoadProjection.ToString(), r.FirstCoordinate, r.SecondCoordinate))
                .ToList();

            AreaUniformlyDistributedLoad bhLoad = new AreaUniformlyDistributedLoad
            {
                Name = rfLoad.Comment,
                Loadcase = loadcase,
                Objects = new BHoMGroup<IAreaElement> { Elements = panels.Cast<IAreaElement>().ToList() },
                Pressure = AxisVector(axis, rfLoad.MagnitudeUniform),
                Axis = loadAxis,
                Projected = projected,
            };

            if (vertices.Count >= 3)
                bhLoad.SetPropertyValue("Polygon", BH.Engine.Geometry.Create.Polygon(vertices));

            bhLoad.SetRFEM6ID(rfLoad.No);
            return bhLoad;
        }

        public static GeometricalLineLoad FromRFEM(this RfLoads.FreeLineLoad rfLoad, List<Panel> panels, Loadcase loadcase)
        {
            if (!TryParseLoadDirection(rfLoad.LoadDirection.ToString(), out int axis, out LoadAxis loadAxis, out bool projected))
                return null;

            double first = rfLoad.LoadDistribution == RfLoads.FreeLineLoad.Types.LoadDistribution.Uniform ? rfLoad.MagnitudeUniform : rfLoad.MagnitudeFirst;
            double second = rfLoad.LoadDistribution == RfLoads.FreeLineLoad.Types.LoadDistribution.Uniform ? rfLoad.MagnitudeUniform : rfLoad.MagnitudeSecond;
            string projection = rfLoad.LoadProjection.ToString();

            GeometricalLineLoad bhLoad = new GeometricalLineLoad
            {
                Name = rfLoad.Comment,
                Loadcase = loadcase,
                Objects = new BHoMGroup<IAreaElement> { Elements = panels.Cast<IAreaElement>().ToList() },
                Location = new Line
                {
                    Start = FromProjectedCoordinates(projection, rfLoad.LoadLocationFirstPointCoordinate1, rfLoad.LoadLocationFirstPointCoordinate2),
                    End = FromProjectedCoordinates(projection, rfLoad.LoadLocationSecondPointCoordinate1, rfLoad.LoadLocationSecondPointCoordinate2),
                },
                ForceA = AxisVector(axis, first),
                ForceB = AxisVector(axis, second),
                MomentA = new Vector(),
                MomentB = new Vector(),
                Axis = loadAxis,
                Projected = projected,
            };
            bhLoad = (GeometricalLineLoad)bhLoad.AddFragment(new RFEM6GeometricalLineLoadTypes { geometrialLineLoadType = GeometricalLineLoadTypesEnum.FreeLineLoad });
            bhLoad.SetRFEM6ID(rfLoad.No);
            return bhLoad;
        }

        public static GeometricalLineLoad FromRFEM(this RfLoads.LineLoad rfLoad, Line location, Loadcase loadcase)
        {
            bool isForce = rfLoad.LoadType == RfLoads.LineLoad.Types.LoadType.Force;
            if (!isForce && rfLoad.LoadType != RfLoads.LineLoad.Types.LoadType.Moment)
            {
                BH.Engine.Base.Compute.RecordWarning($"Line load {rfLoad.No} in load case {rfLoad.LoadCase} is of type {rfLoad.LoadType}. Only force and moment line loads can be read.");
                return null;
            }

            if (!TryParseLoadDirection(rfLoad.LoadDirection.ToString(), out int axis, out LoadAxis loadAxis, out bool projected))
                return null;

            bool uniform = rfLoad.LoadDistribution == RfLoads.LineLoad.Types.LoadDistribution.Uniform;
            Vector a = AxisVector(axis, uniform ? rfLoad.Magnitude : rfLoad.Magnitude1);
            Vector b = AxisVector(axis, uniform ? rfLoad.Magnitude : rfLoad.Magnitude2);

            GeometricalLineLoad bhLoad = new GeometricalLineLoad
            {
                Name = rfLoad.Comment,
                Loadcase = loadcase,
                Location = location,
                ForceA = isForce ? a : new Vector(),
                ForceB = isForce ? b : new Vector(),
                MomentA = isForce ? new Vector() : a,
                MomentB = isForce ? new Vector() : b,
                Axis = loadAxis,
                Projected = projected,
            };
            bhLoad = (GeometricalLineLoad)bhLoad.AddFragment(new RFEM6GeometricalLineLoadTypes { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad });
            bhLoad.SetRFEM6ID(rfLoad.No);
            return bhLoad;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Direction names are LocalX..Z or GlobalXOrUserDefinedU{True|Projected}Length..Z.
        private static bool TryParseLoadDirection(string direction, out int axis, out LoadAxis loadAxis, out bool projected)
        {
            axis = -1;
            loadAxis = LoadAxis.Global;
            projected = false;

            if (direction.StartsWith("Local") && direction.Length > 5)
            {
                axis = "XYZ".IndexOf(direction[5]);
                loadAxis = LoadAxis.Local;
            }
            else if (direction.StartsWith("Global") && direction.Length > 6)
            {
                axis = "XYZ".IndexOf(direction[6]);
                projected = direction.EndsWith("ProjectedLength");
            }

            if (axis < 0)
                BH.Engine.Base.Compute.RecordWarning($"RFEM load direction {direction} is not supported and the load is not read.");

            return axis >= 0;
        }

        private static Vector AxisVector(int axis, double value)
        {
            return new Vector { X = axis == 0 ? value : 0, Y = axis == 1 ? value : 0, Z = axis == 2 ? value : 0 };
        }

        private static Point FromProjectedCoordinates(string projection, double first, double second)
        {
            switch (projection)
            {
                case "XzOrUw":
                    return new Point { X = first, Z = second };
                case "YzOrVw":
                    return new Point { Y = first, Z = second };
                default:
                    return new Point { X = first, Y = second };
            }
        }
    }
}
