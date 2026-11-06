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
        /**** Private Fields                            ****/
        /***************************************************/

        private static readonly Regex EuropeanSteelGrade = new Regex(
            @"^S(?<yield>\d{3})(?<suffix>[A-Z]*)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts the European steel grade token from an RFEM material name (for example S460N).")]
        [Input("rfemMaterialName", "RFEM material name, including optional catalogue suffix after '|'.")]
        [Output("gradeToken", "Alphanumeric grade token, or empty when no European steel grade is recognised.")]
        public static string EuropeanSteelGradeToken(string rfemMaterialName)
        {
            string lookupKey = RfemMaterialLibraryLookupKey(rfemMaterialName);
            if (string.IsNullOrEmpty(lookupKey))
                return string.Empty;

            Match grade = EuropeanSteelGrade.Match(lookupKey);
            if (!grade.Success)
                return string.Empty;

            string suffix = grade.Groups["suffix"].Value.ToUpperInvariant();
            return $"S{grade.Groups["yield"].Value}{suffix}";
        }

        /***************************************************/
    }
}
