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
using System.Linq;
using System.Text.RegularExpressions;

using BH.oM.Base;
using BH.oM.Structure.SectionProperties;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        private static readonly Regex CompactRolledSteelName = new Regex(
            @"^(?<prefix>HE|IPE|IPN|UPE|UPN|UB|UC|UBP|PFC)(?<size>\d+(?:\.\d+)?)(?<suffix>[A-Z]{1,2})?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static StructureCore.CrossSection ToRFEM6(this ISectionProperty bhSection, int matNo, string materialType)
        {
            string sectionName = bhSection.ToRFEM6SectionName();

            StructureCore.CrossSection rfSection = new StructureCore.CrossSection
            {
                Material = matNo,
                Name = sectionName,
            };

            // C# cantilever omits No for the first library section. Keep it when the
            // predicted number is not 1 so members still reference the correct section.
            int sectionNo = bhSection.GetRFEM6ID();
            if (sectionNo > 0)
                rfSection.No = sectionNo;

            if (!(materialType.Equals("Steel") && IsStandardSteelSection(bhSection))
                && !sectionName.StartsWith("R_M1 ", StringComparison.Ordinal)
                && !sectionName.StartsWith("CIRCLE_M1 ", StringComparison.Ordinal))
                BH.Engine.Base.Compute.RecordWarning($"Section {sectionName} uses a simplified RFEM6 export in v1. Parametric sections are not fully migrated yet.");

            if (bhSection is IBHoMObject bhomSection)
            {
                string comment = bhomSection.ToRFEM6Comment();
                if (!string.IsNullOrWhiteSpace(comment))
                    rfSection.Comment = comment;
            }

            return rfSection;
        }

        public static string ToRFEM6SectionName(this ISectionProperty bhSection)
        {
            if (bhSection == null)
                return string.Empty;

            AlterSectionName(bhSection);

            string parametricName = ParametricRfemSectionName(bhSection);
            if (!string.IsNullOrWhiteSpace(parametricName))
            {
                bhSection.Name = parametricName;
                return parametricName;
            }

            string name = bhSection.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = FallbackSectionName(bhSection);
                if (!string.IsNullOrWhiteSpace(name))
                    bhSection.Name = name;
                return name ?? string.Empty;
            }

            string compactName = Regex.Replace(name, @"\s+", "");
            Match rolledSteel = CompactRolledSteelName.Match(compactName);
            if (rolledSteel.Success)
            {
                string prefix = rolledSteel.Groups["prefix"].Value.ToUpperInvariant();
                string size = rolledSteel.Groups["size"].Value;
                string suffix = rolledSteel.Groups["suffix"].Value.ToUpperInvariant();
                name = string.IsNullOrEmpty(suffix) ? $"{prefix} {size}" : $"{prefix} {size} {suffix}";
                bhSection.Name = name;
            }

            return bhSection.Name;
        }

        public static String AlterSectionName(ISectionProperty bhSection)
        {
            string materialType = bhSection.Material.GetType().Name;

            if (materialType.Equals("Steel") && IsStandardSteelSection(bhSection) && !string.IsNullOrWhiteSpace(bhSection.Name) && bhSection.Name.Length >= 3)
            {
                if (bhSection.Name.Split(' ')[0].Equals("L"))
                {
                    double v0 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[0]);
                    double v1 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[1]);
                    double thickness = Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray().Last());
                    bhSection.Name = $"L {v0}x{v1}x{thickness}";
                }
                else if (bhSection.Name.Length > 3 && !bhSection.Name.Substring(3, 1).Equals(" ") && ((bhSection.Name.Substring(0, 3).Equals("RHS")) || (bhSection.Name.Substring(0, 3).Equals("SHS"))))
                {
                    bhSection.Name = bhSection.Name.Insert(3, " ");
                }
                else if (bhSection.Name.Substring(0, 3).Equals("TUB") || bhSection.Name.Substring(0, 3).Equals("TUC"))
                {
                    bhSection.Name = bhSection.Name.Remove(0, 1);
                    bhSection.Name = bhSection.Name.Insert(0, "1/2 ");
                }
            }

            return bhSection.Name;
        }

        private static bool IsStandardSteelSection(ISectionProperty bhSection)
        {
            if (string.IsNullOrWhiteSpace(bhSection?.Name))
                return false;

            string compactName = Regex.Replace(bhSection.Name, @"\s+", "");
            if (CompactRolledSteelName.IsMatch(compactName))
                return true;

            String bhSecNameAbrev = bhSection.Name.Split(' ')[0].Length > 3 ? bhSection.Name.TrimStart().Substring(3).ToUpper() : bhSection.Name.Split(' ')[0];
            HashSet<String> crossSectionNameSet = new HashSet<String>() { "HE", "CHS", "IPE", "L", "RHS", "SHS", "UPE", "PFC", "TUB", "TUC", "UB", "UBP", "UC", "1/2", "IPN", "UPN" };
            return crossSectionNameSet.Contains(bhSecNameAbrev);
        }

        /// <summary>
        /// RFEM parametric rectangle and circle names (M1 = material group 1 in RFEM). X and Y are in millimetres; BHoM lengths are in metres.
        /// Rectangle: R_M1 breadth/height (breadth = Width). Circle: CIRCLE_M1 diameter.
        /// </summary>
        private static string ParametricRfemSectionName(ISectionProperty bhSection)
        {
            if (bhSection == null || (IsStandardSteelSection(bhSection) && !string.IsNullOrWhiteSpace(bhSection.Name)))
                return null;

            SectionDimensions(bhSection, out double? width, out double? height, out double? diameter);

            string profileType = SectionProfileTypeName(bhSection);
            string typeName = bhSection.GetType().Name;
            bool treatAsCircle = diameter > 0
                && (profileType.IndexOf("Circle", StringComparison.OrdinalIgnoreCase) >= 0
                    || typeName.IndexOf("Circle", StringComparison.OrdinalIgnoreCase) >= 0
                    || typeName.IndexOf("Circular", StringComparison.OrdinalIgnoreCase) >= 0
                    || !(width > 0 && height > 0));

            if (treatAsCircle)
                return $"CIRCLE_M1 {LengthMillimetres(diameter.Value)}";

            if (width > 0 && height > 0)
                return $"R_M1 {LengthMillimetres(width.Value)}/{LengthMillimetres(height.Value)}";

            return null;
        }

        private static string FallbackSectionName(ISectionProperty bhSection)
        {
            string parametric = ParametricRfemSectionName(bhSection);
            if (!string.IsNullOrWhiteSpace(parametric))
                return parametric;

            object description = BH.Engine.Base.Query.PropertyValue(bhSection, "Description");
            if (description != null && !string.IsNullOrWhiteSpace(description.ToString()))
                return description.ToString().Trim();

            return null;
        }

        private static int LengthMillimetres(double lengthMetres)
        {
            return (int)Math.Round(lengthMetres * 1000);
        }

        private static void SectionDimensions(ISectionProperty section, out double? width, out double? height, out double? diameter)
        {
            width = PropertyLength(section, "Width");
            height = PropertyLength(section, "Height") ?? PropertyLength(section, "Depth");
            diameter = PropertyLength(section, "Diameter");

            if (section is IBHoMObject sectionObject)
            {
                foreach (IBHoMObject profileObject in SectionProfiles(sectionObject))
                {
                    width ??= PropertyLength(profileObject, "Width");
                    height ??= PropertyLength(profileObject, "Height") ?? PropertyLength(profileObject, "Depth");
                    diameter ??= PropertyLength(profileObject, "Diameter");
                }
            }
        }

        /// <summary>
        /// BHoM <see cref="ConcreteSection"/> and similar types store dimensions on <c>SectionProfile</c>; <see cref="GenericSection"/> uses <c>Profile</c>.
        /// </summary>
        private static IEnumerable<IBHoMObject> SectionProfiles(IBHoMObject section)
        {
            object sectionProfile = BH.Engine.Base.Query.PropertyValue(section, "SectionProfile");
            if (sectionProfile is IBHoMObject concreteProfile)
                yield return concreteProfile;

            object profile = BH.Engine.Base.Query.PropertyValue(section, "Profile");
            if (profile is IBHoMObject genericProfile && genericProfile != sectionProfile)
                yield return genericProfile;
        }

        private static string SectionProfileTypeName(ISectionProperty section)
        {
            if (section is not IBHoMObject sectionObject)
                return string.Empty;

            IBHoMObject profile = SectionProfiles(sectionObject).FirstOrDefault();
            return profile?.GetType().Name ?? string.Empty;
        }

        private static double? PropertyLength(IBHoMObject obj, string propertyName)
        {
            object value = BH.Engine.Base.Query.PropertyValue(obj, propertyName);
            if (value is double d)
                return d;
            if (value is int i)
                return i;
            return null;
        }
    }
}
