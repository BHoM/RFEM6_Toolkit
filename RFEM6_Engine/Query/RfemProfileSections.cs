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

        /// <summary>
        /// RFEM hot-rolled flag when BHoM does not specify hot or cold forming.
        /// </summary>
        private const string DefaultHotColdFlag = "H";

        private static readonly Regex RfemISectionName = new Regex(
            @"^I\s+(?<height>\d+)\s*/\s*(?<width>\d+)\s*/\s*(?<web>\d+)\s*/\s*(?<flange>\d+)\s*/\s*(?<root>\d+)\s*/\s*(?<toe>\d+)\s*/\s*(?<hotCold>[HC])\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex RfemRhsSectionName = new Regex(
            @"^RHS\s+(?<height>\d+)\s*/\s*(?<width>\d+)\s*/\s*(?<thickness>\d+)\s*/\s*(?<outer>\d+)\s*/\s*(?<inner>\d+)\s*/\s*(?<hotCold>[HC])\s*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Builds an RFEM parametric I-section name from BHoM lengths in metres.")]
        [Input("heightMetres", "Overall depth h between flange fibres.")]
        [Input("widthMetres", "Flange width b.")]
        [Input("webThicknessMetres", "Web thickness tw.")]
        [Input("flangeThicknessMetres", "Flange thickness tf.")]
        [Input("rootRadiusMetres", "Root fillet radius r1.")]
        [Input("toeRadiusMetres", "Toe fillet radius r2.")]
        [Output("rfemName", "RFEM cross-section name, for example I 300/200/10/14/12/8/H.")]
        public static string RfemISectionNameFromMetres(
            double heightMetres,
            double widthMetres,
            double webThicknessMetres,
            double flangeThicknessMetres,
            double rootRadiusMetres,
            double toeRadiusMetres)
        {
            return $"I {LengthMillimetresText(heightMetres)}/{LengthMillimetresText(widthMetres)}/{LengthMillimetresText(webThicknessMetres)}/{LengthMillimetresText(flangeThicknessMetres)}/{LengthMillimetresText(rootRadiusMetres)}/{LengthMillimetresText(toeRadiusMetres)}/{DefaultHotColdFlag}";
        }

        [Description("Builds an RFEM parametric RHS (box) name from BHoM lengths in metres.")]
        [Input("heightMetres", "Overall height h.")]
        [Input("widthMetres", "Overall width b.")]
        [Input("thicknessMetres", "Wall thickness t.")]
        [Input("outerRadiusMetres", "Outer corner radius ro.")]
        [Input("innerRadiusMetres", "Inner corner radius ri.")]
        [Output("rfemName", "RFEM cross-section name, for example RHS 750/500/15/15/12/H.")]
        public static string RfemRhsSectionNameFromMetres(
            double heightMetres,
            double widthMetres,
            double thicknessMetres,
            double outerRadiusMetres,
            double innerRadiusMetres)
        {
            return $"RHS {LengthMillimetresText(heightMetres)}/{LengthMillimetresText(widthMetres)}/{LengthMillimetresText(thicknessMetres)}/{LengthMillimetresText(outerRadiusMetres)}/{LengthMillimetresText(innerRadiusMetres)}/{DefaultHotColdFlag}";
        }

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
        /**** Private Methods                           ****/
        /***************************************************/

        private static string LengthMillimetresText(double lengthMetres)
        {
            return ((int)Math.Round(lengthMetres * 1000)).ToString(CultureInfo.InvariantCulture);
        }
    }
}
