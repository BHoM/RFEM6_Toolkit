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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.RFEM6
{
    public static partial class Query
    {
        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static readonly Regex LibraryEuropeanSteelGrade = new Regex(
            @"^S(?<yield>\d{3})(?<suffix>[A-Z]*?)(?=EN|\d|\(|$)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Matches an RFEM European steel grade name to a BHoM library Steel material (for example S460N to S460 N/NL).")]
        [Input("rfemMaterialName", "RFEM material name.")]
        [Input("materials", "BHoM structure material library entries.")]
        [Output("material", "Matching library Steel material, or null when no grade match is found.")]
        public static IBHoMObject MatchEuropeanSteelMaterial(string rfemMaterialName, IEnumerable<IBHoMObject> materials)
        {
            if (string.IsNullOrWhiteSpace(rfemMaterialName) || materials == null)
                return null;

            string rfemGradeToken = EuropeanSteelGradeToken(rfemMaterialName);
            if (string.IsNullOrEmpty(rfemGradeToken))
                return null;

            List<IBHoMObject> matches = materials
                .Where(material => material != null
                    && !string.IsNullOrWhiteSpace(material.Name)
                    && material.GetType().Name.Equals("Steel", StringComparison.Ordinal)
                    && EuropeanSteelLibraryNameMatchesGrade(rfemGradeToken, material.Name))
                .ToList();

            if (matches.Count == 0)
                return null;

            if (matches.Count == 1)
                return matches[0];

            return matches.OrderBy(material => material.Name.Length).ThenBy(material => material.Name, StringComparer.OrdinalIgnoreCase).First();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool EuropeanSteelLibraryNameMatchesGrade(string rfemGradeToken, string libraryMaterialName)
        {
            Match rfemGrade = EuropeanSteelGrade.Match(rfemGradeToken.ToUpperInvariant());
            if (!rfemGrade.Success)
                return false;

            string libraryKey = CompactAlphanumeric.Replace(RfemNameStem(libraryMaterialName), string.Empty).ToUpperInvariant();
            Match libraryGrade = LibraryEuropeanSteelGrade.Match(libraryKey);
            if (!libraryGrade.Success)
                return false;

            if (!rfemGrade.Groups["yield"].Value.Equals(libraryGrade.Groups["yield"].Value, StringComparison.Ordinal))
                return false;

            string rfemSuffix = rfemGrade.Groups["suffix"].Value.ToUpperInvariant();
            string librarySuffix = libraryGrade.Groups["suffix"].Value.ToUpperInvariant();

            if (string.IsNullOrEmpty(rfemSuffix))
                return true;

            if (librarySuffix.StartsWith(rfemSuffix, StringComparison.Ordinal))
                return true;

            if (rfemSuffix.Length == 1 && librarySuffix.StartsWith(rfemSuffix + rfemSuffix, StringComparison.Ordinal))
                return true;

            return false;
        }
    }
}
