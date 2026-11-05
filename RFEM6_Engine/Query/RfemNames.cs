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
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.RFEM6
{
    public static partial class Query
    {
        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static readonly Regex CompactAlphanumeric = new Regex(
            @"[^a-zA-Z0-9]",
            RegexOptions.CultureInvariant);

        private static readonly Regex RfemHeCatalogueName = new Regex(
            @"^HE(?<variant>AA|BB|A|B|M|C)(?<size>\d+(?:\.\d+)?)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex BhomHeCompactName = new Regex(
            @"^HE(?<size>\d+(?:\.\d+)?)(?<variant>[A-Z]{1,2})$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex EuropeanSteelGrade = new Regex(
            @"^S(?<yield>\d{3})(?<suffix>[A-Z]*)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex ParametricRectangleName = new Regex(
            @"^(?:R_M1|RM_1)\s*(?<breadth>\d+)\s*/\s*(?<height>\d+)\s*",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex ParametricCircleName = new Regex(
            @"^CIRCLE_M1\s*(?<diameter>\d+)\s*",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an alphanumeric key used to match an RFEM cross-section name to BHoM steel library entries.")]
        [Input("rfemSectionName", "RFEM cross-section name, including optional catalogue suffix after '|'.")]
        [Output("lookupKey", "Compact name aligned with BHoM EU steel naming (for example HEB 800 becomes HE800B).")]
        public static string RfemSectionLibraryLookupKey(string rfemSectionName)
        {
            if (string.IsNullOrWhiteSpace(rfemSectionName))
                return string.Empty;

            string compact = CompactAlphanumeric.Replace(RfemNameStem(rfemSectionName), string.Empty);
            if (string.IsNullOrEmpty(compact))
                return string.Empty;

            Match rfemHe = RfemHeCatalogueName.Match(compact);
            if (rfemHe.Success)
            {
                string variant = rfemHe.Groups["variant"].Value.ToUpperInvariant();
                string size = rfemHe.Groups["size"].Value;
                return $"HE{size}{variant}";
            }

            Match bhomHe = BhomHeCompactName.Match(compact);
            if (bhomHe.Success)
                return compact.ToUpperInvariant();

            return compact.ToUpperInvariant();
        }

        [Description("Returns an alphanumeric key used to match an RFEM material name to BHoM material library entries.")]
        [Input("rfemMaterialName", "RFEM material name, including optional catalogue suffix after '|'.")]
        [Output("lookupKey", "Compact name used for fuzzy library matching.")]
        public static string RfemMaterialLibraryLookupKey(string rfemMaterialName)
        {
            if (string.IsNullOrWhiteSpace(rfemMaterialName))
                return string.Empty;

            string compact = CompactAlphanumeric.Replace(RfemNameStem(rfemMaterialName), string.Empty);
            if (string.IsNullOrEmpty(compact))
                return string.Empty;

            return compact.ToUpperInvariant();
        }

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

        [Description("Parses RFEM parametric rectangle or circle cross-section names into BHoM lengths in metres.")]
        [Input("rfemSectionName", "RFEM cross-section name (R_M1, RM_1, or CIRCLE_M1).")]
        [Output("parsed", "True when the name is recognised as a parametric RFEM section.")]
        public static bool TryParseParametricRfemSectionName(
            string rfemSectionName,
            out double widthMetres,
            out double heightMetres,
            out double diameterMetres)
        {
            widthMetres = 0;
            heightMetres = 0;
            diameterMetres = 0;

            if (string.IsNullOrWhiteSpace(rfemSectionName))
                return false;

            string stem = RfemNameStem(rfemSectionName);

            Match rectangle = ParametricRectangleName.Match(stem);
            if (rectangle.Success)
            {
                // RFEM name order is breadth (b) / height (h).
                if (!TryParseMillimetres(rectangle.Groups["breadth"].Value, out widthMetres))
                    return false;
                if (!TryParseMillimetres(rectangle.Groups["height"].Value, out heightMetres))
                    return false;

                return true;
            }

            Match circle = ParametricCircleName.Match(stem);
            if (circle.Success)
            {
                if (!TryParseMillimetres(circle.Groups["diameter"].Value, out diameterMetres))
                    return false;

                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string RfemNameStem(string rfemName)
        {
            return rfemName.Split('|')[0].Trim();
        }

        private static bool TryParseMillimetres(string millimetresText, out double lengthMetres)
        {
            lengthMetres = 0;
            if (!int.TryParse(millimetresText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int millimetres))
                return false;

            lengthMetres = millimetres / 1000.0;
            return true;
        }
    }
}
