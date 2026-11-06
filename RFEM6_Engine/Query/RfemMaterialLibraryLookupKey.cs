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
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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

        /***************************************************/
    }
}
