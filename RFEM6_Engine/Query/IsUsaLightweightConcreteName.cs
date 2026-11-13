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
using System.Text.RegularExpressions;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.RFEM6
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks whether a BHoM material name is a MaterialsUSA lightweight concrete (for example 6000psiLW), which has no RFEM catalogue equivalent.")]
        [Input("bhomMaterialName", "Name of the BHoM material.")]
        [Output("isLightweight", "True when the name is a MaterialsUSA lightweight concrete.")]
        public static bool IsUsaLightweightConcreteName(this string bhomMaterialName)
        {
            Match match = MatchUsaConcreteLibraryName(bhomMaterialName);
            return match != null && match.Groups["weight"].Value.Equals("LW", System.StringComparison.OrdinalIgnoreCase);
        }

        /***************************************************/
    }
}
