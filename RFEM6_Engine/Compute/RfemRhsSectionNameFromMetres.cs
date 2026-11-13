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
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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

        /***************************************************/
    }
}
