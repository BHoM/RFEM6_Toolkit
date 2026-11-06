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

        private static readonly Regex ParametricRectangleName = new Regex(
            @"^(?:R_M1|RM_1)\s*(?<breadth>\d+)\s*/\s*(?<height>\d+)\s*",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex ParametricCircleName = new Regex(
            @"^CIRCLE_M1\s*(?<diameter>\d+)\s*",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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

        private static bool TryParseMillimetres(string millimetresText, out double lengthMetres)
        {
            lengthMetres = 0;
            if (!int.TryParse(millimetresText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int millimetres))
                return false;

            lengthMetres = millimetres / 1000.0;
            return true;
        }

        /***************************************************/
    }
}
