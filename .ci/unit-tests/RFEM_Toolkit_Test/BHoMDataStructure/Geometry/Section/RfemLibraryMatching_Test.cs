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
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using BH.oM.Base;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Spatial.ShapeProfiles;
using BH.Adapter.RFEM6;

namespace RFEM_Toolkit_Test.Elements
{
    [TestFixture]
    public class RfemLibraryMatching_Test
    {
        [TestCase("HEB 800", "HE800B")]
        [TestCase("HEM 280", "HE280M")]
        [TestCase("HEA 200", "HE200A")]
        [TestCase("HE 800 B", "HE800B")]
        public void RfemSectionLibraryLookupKey_MapsEuropeanProfiles(string rfemName, string expectedKey)
        {
            string key = BH.Engine.Adapters.RFEM6.Query.RfemSectionLibraryLookupKey(rfemName);
            Assert.AreEqual(expectedKey, key);
        }

        [Test]
        public void RfemSectionLibraryLookupKey_MatchesBhomDatasetName()
        {
            ISectionProperty section = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 800 B", true, true) as ISectionProperty;
            Assert.IsNotNull(section);

            string libraryKey = Regex.Replace(section.Name, @"[^a-zA-Z0-9]", "");
            string rfemKey = BH.Engine.Adapters.RFEM6.Query.RfemSectionLibraryLookupKey("HEB 800");

            Assert.AreEqual(libraryKey.ToUpperInvariant(), rfemKey);
            Assert.GreaterOrEqual(BH.Engine.Search.Compute.MatchScore(rfemKey, libraryKey), 85);
        }

        [Test]
        public void EuropeanSteelGradeToken_ExtractsRfemGrade()
        {
            string token = BH.Engine.Adapters.RFEM6.Query.EuropeanSteelGradeToken("S460N");
            Assert.AreEqual("S460N", token);
        }

        [Test]
        public void MatchEuropeanSteelMaterial_MapsS460NToLibraryGrade()
        {
            IMaterialFragment match = BH.Engine.Adapters.RFEM6.Query.MatchEuropeanSteelMaterial(
                "S460N",
                BH.Engine.Library.Query.Library("Structure\\Materials").Cast<IBHoMObject>()) as IMaterialFragment;

            Assert.IsNotNull(match);
            Assert.AreEqual("Steel", match.GetType().Name);
            Assert.IsTrue(match.Name.IndexOf("S460", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        [Test]
        public void RfemMaterialLookup_MatchesLibrarySteelGrade()
        {
            IMaterialFragment libraryMaterial = BH.Engine.Library.Query.Library("Structure\\Materials")
                .Cast<IMaterialFragment>()
                .FirstOrDefault(material => material.Name != null && material.Name.StartsWith("S460 N/NL"));

            Assert.IsNotNull(libraryMaterial);

            string libraryKey = Regex.Replace(libraryMaterial.Name, @"[^a-zA-Z0-9]", "");
            Assert.IsTrue(libraryKey.IndexOf(BH.Engine.Adapters.RFEM6.Query.EuropeanSteelGradeToken("S460N")) >= 0);
        }

        [TestCase("R_M1 300/400", 0.3, 0.4, 0)]
        [TestCase("RM_1 500/250", 0.5, 0.25, 0)]
        [TestCase("CIRCLE_M1 200", 0, 0, 0.2)]
        public void TryParseParametricRfemSectionName_ParsesDimensions(
            string rfemName,
            double expectedWidth,
            double expectedHeight,
            double expectedDiameter)
        {
            bool parsed = BH.Engine.Adapters.RFEM6.Query.TryParseParametricRfemSectionName(
                rfemName,
                out double widthMetres,
                out double heightMetres,
                out double diameterMetres);

            Assert.IsTrue(parsed);
            Assert.AreEqual(expectedWidth, widthMetres, 1e-9);
            Assert.AreEqual(expectedHeight, heightMetres, 1e-9);
            Assert.AreEqual(expectedDiameter, diameterMetres, 1e-9);
        }

        [Test]
        public void ParametricRm1Pull_MapsBreadthToWidthAndHeightToHeight()
        {
            Concrete material = new Concrete { Name = "C30/37" };
            ISectionProperty section = BH.Adapter.RFEM6.Convert.TryCreateSectionFromParametricRfemName("R_M1 300/400", material);

            Assert.IsNotNull(section);
            IGeometricalSection geometricalSection = section as IGeometricalSection;
            Assert.IsNotNull(geometricalSection);
            RectangleProfile profile = geometricalSection.SectionProfile as RectangleProfile;
            Assert.IsNotNull(profile);
            Assert.AreEqual(0.3, profile.Width, 1e-9);
            Assert.AreEqual(0.4, profile.Height, 1e-9);
        }

        [Test]
        public void RfemISectionName_RoundTripMetres()
        {
            string rfemName = BH.Engine.Adapters.RFEM6.Query.RfemISectionNameFromMetres(0.3, 0.2, 0.01, 0.014, 0.012, 0.008);
            Assert.AreEqual("I 300/200/10/14/12/8/H", rfemName);

            Assert.IsTrue(BH.Engine.Adapters.RFEM6.Query.TryParseRfemISectionName(
                rfemName,
                out double heightMetres,
                out double widthMetres,
                out double webMetres,
                out double flangeMetres,
                out double rootMetres,
                out double toeMetres));

            Assert.AreEqual(0.3, heightMetres, 1e-9);
            Assert.AreEqual(0.2, widthMetres, 1e-9);
            Assert.AreEqual(0.01, webMetres, 1e-9);
            Assert.AreEqual(0.014, flangeMetres, 1e-9);
            Assert.AreEqual(0.012, rootMetres, 1e-9);
            Assert.AreEqual(0.008, toeMetres, 1e-9);
        }

        [Test]
        public void RfemRhsSectionName_RoundTripMetres()
        {
            string rfemName = BH.Engine.Adapters.RFEM6.Query.RfemRhsSectionNameFromMetres(0.75, 0.5, 0.015, 0.015, 0.012);
            Assert.AreEqual("RHS 750/500/15/15/12/H", rfemName);

            Assert.IsTrue(BH.Engine.Adapters.RFEM6.Query.TryParseRfemRhsSectionName(
                rfemName,
                out double heightMetres,
                out double widthMetres,
                out double thicknessMetres,
                out double outerMetres,
                out double innerMetres));

            Assert.AreEqual(0.75, heightMetres, 1e-9);
            Assert.AreEqual(0.5, widthMetres, 1e-9);
            Assert.AreEqual(0.015, thicknessMetres, 1e-9);
            Assert.AreEqual(0.015, outerMetres, 1e-9);
            Assert.AreEqual(0.012, innerMetres, 1e-9);
        }
    }
}
