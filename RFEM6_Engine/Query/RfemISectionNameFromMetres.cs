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

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string LengthMillimetresText(double lengthMetres)
        {
            return ((int)Math.Round(lengthMetres * 1000)).ToString(CultureInfo.InvariantCulture);
        }

        /***************************************************/
    }
}
