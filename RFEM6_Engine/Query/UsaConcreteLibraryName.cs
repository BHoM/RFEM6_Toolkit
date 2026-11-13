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

        private const double PsiToPascal = 6894.757293168;

        private static readonly Regex PsiStrength = new Regex(
            @"(?<strength>\d{1,3}(?:,\d{3})+|\d+)\s*psi",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex LightweightConcrete = new Regex(
            @"(?<![a-z])(LW|light\s*-?\s*weight)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the name of the BHoM MaterialsUSA concrete matching an RFEM concrete name given in psi (for example \"Concrete f'c = 6000 psi\" to 6000psiNW). Normal weight is assumed unless the name states lightweight.")]
        [Input("rfemMaterialName", "RFEM material name, including optional catalogue suffix after '|'.")]
        [Output("libraryName", "BHoM MaterialsUSA concrete name, or empty when the name has no strength in psi.")]
        public static string UsaConcreteLibraryName(this string rfemMaterialName)
        {
            if (string.IsNullOrWhiteSpace(rfemMaterialName))
                return string.Empty;

            string stem = RfemNameStem(rfemMaterialName);
            Match strength = PsiStrength.Match(stem);
            if (!strength.Success)
                return string.Empty;

            string weight = LightweightConcrete.IsMatch(stem.Replace(strength.Value, " ")) ? "LW" : "NW";
            return $"{strength.Groups["strength"].Value.Replace(",", "")}psi{weight}";
        }

        /***************************************************/

        [Description("Returns the concrete strength stated in psi in a material name (for example \"Concrete f'c = 6000 psi\" or 6000psiNW), converted to Pascals.")]
        [Input("materialName", "RFEM or BHoM material name.")]
        [Output("strength", "Strength in Pascals, or 0 when the name has no strength in psi.")]
        public static double PsiStrengthFromName(this string materialName)
        {
            if (string.IsNullOrWhiteSpace(materialName))
                return 0;

            Match strength = PsiStrength.Match(materialName);
            if (!strength.Success || !double.TryParse(strength.Groups["strength"].Value.Replace(",", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out double psi))
                return 0;

            return psi * PsiToPascal;
        }

        /***************************************************/
    }
}
