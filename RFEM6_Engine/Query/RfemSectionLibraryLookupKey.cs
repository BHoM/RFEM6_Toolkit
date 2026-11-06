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

        private static readonly Regex CompactAlphanumeric = new Regex(
            @"[^a-zA-Z0-9]",
            RegexOptions.CultureInvariant);

        private static readonly Regex RfemHeCatalogueName = new Regex(
            @"^HE(?<variant>AA|BB|A|B|M|C)(?<size>\d+(?:\.\d+)?)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex BhomHeCompactName = new Regex(
            @"^HE(?<size>\d+(?:\.\d+)?)(?<variant>[A-Z]{1,2})$",
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

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string RfemNameStem(string rfemName)
        {
            return rfemName.Split('|')[0].Trim();
        }

        /***************************************************/
    }
}
