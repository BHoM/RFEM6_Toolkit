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
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using EngineQuery = BH.Engine.Adapters.RFEM6.Query;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private const int MinimumSectionMatchScore = 85;
        private const int MinimumMaterialMatchScore = 80;

        private static readonly Regex CompactAlphanumeric = new Regex(
            @"[^a-zA-Z0-9]",
            RegexOptions.CultureInvariant);

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string LibraryNameKey(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return CompactAlphanumeric.Replace(name, string.Empty).ToUpperInvariant();
        }

        private static IBHoMObject MatchLibraryObject(
            string rfemName,
            IEnumerable<IBHoMObject> library,
            Func<string, string> lookupKeyFactory,
            int minimumScore,
            out int bestScore)
        {
            bestScore = 0;
            if (string.IsNullOrWhiteSpace(rfemName) || library == null)
                return null;

            string lookupKey = lookupKeyFactory(rfemName);
            if (string.IsNullOrEmpty(lookupKey))
                return null;

            Dictionary<int, HashSet<IBHoMObject>> matchScores = new Dictionary<int, HashSet<IBHoMObject>>();
            foreach (IBHoMObject libraryObject in library)
            {
                if (libraryObject == null || string.IsNullOrWhiteSpace(libraryObject.Name))
                    continue;

                string libraryKey = LibraryNameKey(libraryObject.Name);
                int score = BH.Engine.Search.Compute.MatchScore(lookupKey, libraryKey);
                if (matchScores.TryGetValue(score, out HashSet<IBHoMObject> objectsAtScore))
                    objectsAtScore.Add(libraryObject);
                else
                    matchScores[score] = new HashSet<IBHoMObject>() { libraryObject };
            }

            if (matchScores.Count == 0)
                return null;

            KeyValuePair<int, HashSet<IBHoMObject>> bestBucket = matchScores.OrderByDescending(pair => pair.Key).First();
            bestScore = bestBucket.Key;
            if (bestScore < minimumScore)
                return null;

            IBHoMObject result = bestBucket.Value.First();
            if (bestBucket.Value.Count <= 1)
                return result;

            foreach (IBHoMObject candidate in bestBucket.Value)
            {
                string candidateKey = LibraryNameKey(candidate.Name);
                if (IsAnagramUsingSort(candidateKey, lookupKey))
                {
                    result = candidate;
                    break;
                }
            }

            return result;
        }

        private static IMaterialFragment MatchMaterialLibraryObject(
            string rfemMaterialName,
            List<IMaterialFragment> matLibrary,
            out int bestScore)
        {
            bestScore = 0;
            if (string.IsNullOrWhiteSpace(rfemMaterialName) || matLibrary == null || matLibrary.Count == 0)
                return null;

            IMaterialFragment europeanSteel = EngineQuery.MatchEuropeanSteelMaterial(rfemMaterialName, matLibrary.Cast<IBHoMObject>()) as IMaterialFragment;
            if (europeanSteel != null)
            {
                bestScore = 100;
                return europeanSteel;
            }

            string gradeToken = EngineQuery.EuropeanSteelGradeToken(rfemMaterialName);
            if (!string.IsNullOrEmpty(gradeToken))
            {
                List<IMaterialFragment> gradeCandidates = matLibrary
                    .Where(material => material != null
                        && LibraryNameKey(material.Name).IndexOf(gradeToken, StringComparison.Ordinal) >= 0)
                    .ToList();

                if (gradeCandidates.Count > 0)
                {
                    IMaterialFragment gradeMatch = MatchMaterialFromCandidates(
                        rfemMaterialName,
                        gradeCandidates,
                        out bestScore);

                    if (gradeMatch != null)
                        return gradeMatch;
                }
            }

            return MatchMaterialFromCandidates(rfemMaterialName, matLibrary, out bestScore);
        }

        private static IMaterialFragment MatchMaterialFromCandidates(
            string rfemMaterialName,
            List<IMaterialFragment> candidates,
            out int bestScore)
        {
            IEnumerable<IBHoMObject> libraryObjects = candidates.Cast<IBHoMObject>();
            IBHoMObject match = MatchLibraryObject(
                rfemMaterialName,
                libraryObjects,
                EngineQuery.RfemMaterialLibraryLookupKey,
                MinimumMaterialMatchScore,
                out bestScore);

            return match as IMaterialFragment;
        }

        public static ISectionProperty TryCreateSectionFromParametricRfemName(
            string rfemSectionName,
            IMaterialFragment sectionMaterial)
        {
            if (string.IsNullOrWhiteSpace(rfemSectionName) || sectionMaterial == null)
                return null;

            string displayName = rfemSectionName.Split('|')[0].Trim();

            if (EngineQuery.TryParseRfemISectionName(
                    rfemSectionName,
                    out double heightMetres,
                    out double widthMetres,
                    out double webThicknessMetres,
                    out double flangeThicknessMetres,
                    out double rootRadiusMetres,
                    out double toeRadiusMetres))
            {
                ISectionProfile profile = BH.Engine.Spatial.Create.ISectionProfile(
                    heightMetres,
                    widthMetres,
                    webThicknessMetres,
                    flangeThicknessMetres,
                    rootRadiusMetres,
                    toeRadiusMetres);

                if (profile == null)
                    return null;

                return BH.Engine.Structure.Create.GenericSectionFromProfile(profile, sectionMaterial, displayName);
            }

            if (EngineQuery.TryParseRfemRhsSectionName(
                    rfemSectionName,
                    out heightMetres,
                    out widthMetres,
                    out double thicknessMetres,
                    out double outerRadiusMetres,
                    out double innerRadiusMetres))
            {
                BoxProfile profile = BH.Engine.Spatial.Create.BoxProfile(
                    heightMetres,
                    widthMetres,
                    thicknessMetres,
                    outerRadiusMetres,
                    innerRadiusMetres);

                if (profile == null)
                    return null;

                return BH.Engine.Structure.Create.GenericSectionFromProfile(profile, sectionMaterial, displayName);
            }

            if (!EngineQuery.TryParseParametricRfemSectionName(
                    rfemSectionName,
                    out double rectWidthMetres,
                    out double rectHeightMetres,
                    out double diameterMetres))
                return null;

            ISectionProperty section;

            if (diameterMetres > 0)
            {
                IProfile profile = BH.Engine.Spatial.Create.CircleProfile(diameterMetres);
                section = BH.Engine.Structure.Create.GenericSectionFromProfile(profile, sectionMaterial, displayName);
            }
            else
            {
                // RFEM R_M1 is breadth/height (b/h). BHoM RectangleProfile is (height, width).
                IProfile profile = BH.Engine.Spatial.Create.RectangleProfile(rectHeightMetres, rectWidthMetres);
                section = BH.Engine.Structure.Create.GenericSectionFromProfile(profile, sectionMaterial, displayName);
            }

            return section;
        }
    }
}
