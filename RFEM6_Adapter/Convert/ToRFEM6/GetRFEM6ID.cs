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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Base;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        /// <summary>
        /// RFEM number of the object, or 0 if it has none yet. Objects being pushed for the first time have no number, so no warning is raised.
        /// </summary>
        public static int GetRFEM6ID(this IBHoMObject obj)
        {
            return obj.AdapterId<int>(typeof(RFEM6ID), false);
        }

        /// <summary>
        /// RFEM comment field: BHoM <see cref="IBHoMObject.Name"/> first, otherwise legacy CustomData "Comment".
        /// </summary>
        public static string ToRFEM6Comment(this IBHoMObject obj)
        {
            if (obj == null)
                return null;

            if (!string.IsNullOrWhiteSpace(obj.Name))
                return obj.Name.Trim();

            if (obj.CustomData != null && obj.CustomData.TryGetValue("Comment", out object comment) && comment != null && !string.IsNullOrWhiteSpace(comment.ToString()))
                return comment.ToString().Trim();

            return null;
        }
    }
}
