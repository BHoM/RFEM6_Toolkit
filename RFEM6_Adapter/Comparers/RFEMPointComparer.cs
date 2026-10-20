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
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Constraints;
using BH.oM.Geometry;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public class RFEMPointComparer : IEqualityComparer<Point>
    {
        private const double Tolerance = 0.00001;

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMPointComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Point point1, Point point2)
        {
            if (point1 == null && point2 == null)
                return true;
            if (point1 == null || point2 == null)
                return false;

            if (Math.Abs(point1.X - point2.X) > Tolerance)
                return false;
            if (Math.Abs(point1.Y - point2.Y) > Tolerance)
                return false;
            if (Math.Abs(point1.Z - point2.Z) > Tolerance)
                return false;

            return true;
        }

        /***************************************************/

        public int GetHashCode(Point point)
        {
            if (point == null)
                return 0;

            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + Quantise(point.X);
                hash = (hash * 31) + Quantise(point.Y);
                hash = (hash * 31) + Quantise(point.Z);
                return hash;
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static int Quantise(double coordinate)
        {
            return (int)Math.Round(coordinate / Tolerance);
        }


        /***************************************************/
    }
}




