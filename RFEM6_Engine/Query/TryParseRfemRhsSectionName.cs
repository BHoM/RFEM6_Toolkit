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

        private static readonly Regex RfemRhsSectionName = new Regex(
            @"^RHS\s+(?<height>\d+)\s*/\s*(?<width>\d+)\s*/\s*(?<thickness>\d+)\s*/\s*(?<outer>\d+)\s*/\s*(?<inner>\d+)\s*/\s*(?<hotCold>[HC])\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Parses an RFEM parametric RHS name into BHoM lengths in metres.")]
        [Input("rfemSectionName", "RFEM cross-section name.")]
        [Output("parsed", "True when the name matches the RFEM RHS pattern.")]
        public static bool TryParseRfemRhsSectionName(
            string rfemSectionName,
            out double heightMetres,
            out double widthMetres,
            out double thicknessMetres,
            out double outerRadiusMetres,
            out double innerRadiusMetres)
        {
            heightMetres = 0;
            widthMetres = 0;
            thicknessMetres = 0;
            outerRadiusMetres = 0;
            innerRadiusMetres = 0;

            if (string.IsNullOrWhiteSpace(rfemSectionName))
                return false;

            Match match = RfemRhsSectionName.Match(RfemNameStem(rfemSectionName));
            if (!match.Success)
                return false;

            return TryParseMillimetres(match.Groups["height"].Value, out heightMetres)
                && TryParseMillimetres(match.Groups["width"].Value, out widthMetres)
                && TryParseMillimetres(match.Groups["thickness"].Value, out thicknessMetres)
                && TryParseMillimetres(match.Groups["outer"].Value, out outerRadiusMetres)
                && TryParseMillimetres(match.Groups["inner"].Value, out innerRadiusMetres);
        }

        /***************************************************/
    }
}
