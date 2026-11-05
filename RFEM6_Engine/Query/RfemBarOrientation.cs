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

using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.RFEM6
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Maps a BHoM bar OrientationAngle to the RFEM member rotation_angle. Both are in radians (RFEM API; the GUI may show β in degrees).")]
        [Input("orientationAngleRadians", "BHoM bar OrientationAngle in radians.")]
        [Output("rotationAngleRadians", "RFEM member rotation_angle in radians.")]
        public static double RfemMemberRotationAngle(double orientationAngleRadians)
        {
            return orientationAngleRadians;
        }

        /***************************************************/

        [Description("Maps an RFEM member rotation_angle to a BHoM bar OrientationAngle. Both are in radians.")]
        [Input("rotationAngleRadians", "RFEM member rotation_angle in radians.")]
        [Output("orientationAngleRadians", "BHoM bar OrientationAngle in radians.")]
        public static double OrientationAngleFromRfemMemberRotation(double rotationAngleRadians)
        {
            return rotationAngleRadians;
        }
    }
}
