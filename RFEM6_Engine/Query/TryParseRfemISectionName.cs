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

        private static readonly Regex RfemISectionName = new Regex(
            @"^I\s+(?<height>\d+)\s*/\s*(?<width>\d+)\s*/\s*(?<web>\d+)\s*/\s*(?<flange>\d+)\s*/\s*(?<root>\d+)\s*/\s*(?<toe>\d+)\s*/\s*(?<hotCold>[HC])\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Parses an RFEM parametric I-section name into BHoM lengths in metres.")]
        [Input("rfemSectionName", "RFEM cross-section name.")]
        [Output("parsed", "True when the name matches the RFEM I-section pattern.")]
        public static bool TryParseRfemISectionName(
            string rfemSectionName,
            out double heightMetres,
            out double widthMetres,
            out double webThicknessMetres,
            out double flangeThicknessMetres,
            out double rootRadiusMetres,
            out double toeRadiusMetres)
        {
            heightMetres = 0;
            widthMetres = 0;
            webThicknessMetres = 0;
            flangeThicknessMetres = 0;
            rootRadiusMetres = 0;
            toeRadiusMetres = 0;

            if (string.IsNullOrWhiteSpace(rfemSectionName))
                return false;

            Match match = RfemISectionName.Match(RfemNameStem(rfemSectionName));
            if (!match.Success)
                return false;

            return TryParseMillimetres(match.Groups["height"].Value, out heightMetres)
                && TryParseMillimetres(match.Groups["width"].Value, out widthMetres)
                && TryParseMillimetres(match.Groups["web"].Value, out webThicknessMetres)
                && TryParseMillimetres(match.Groups["flange"].Value, out flangeThicknessMetres)
                && TryParseMillimetres(match.Groups["root"].Value, out rootRadiusMetres)
                && TryParseMillimetres(match.Groups["toe"].Value, out toeRadiusMetres);
        }

        /***************************************************/
    }
}
