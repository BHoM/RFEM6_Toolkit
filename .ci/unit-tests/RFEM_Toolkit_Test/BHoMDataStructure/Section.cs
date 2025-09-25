/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.Adapter.RFEM6;
using BH.oM.Data.Requests;
using BH.oM.Structure.SectionProperties;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullSections
    {
        RFEM6Adapter adapter;
        ISectionProperty section1;
        ISectionProperty section2;
        RFEMSectionComparer comparer;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(active:true);
        }

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }

        [Test]
        public void SinglePushPullOfSection()
        {
            comparer = new RFEMSectionComparer();

            // Define Sections
            section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;

            // Push it once
            adapter.Push(new List<ISectionProperty>() { section1 });

            // Pull it
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionPulled = adapter.Pull(sectionFilter).ToList();
            ISectionProperty sp = (ISectionProperty)sectionPulled[0];

            // Check
            Assert.IsNotNull(sp);
            Assert.IsTrue(comparer.Equals(section1, sp));
        }

        [Test]
        public void DoublePushPullOfSections()
        {
            comparer = new RFEMSectionComparer();

            // Define Sections
            section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            // Push them twice
            adapter.Push(new List<ISectionProperty>() { section1 });
            adapter.Push(new List<ISectionProperty>() { section1, section2 });

            // Pull it
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).ToList();
            ISectionProperty sp1 = (ISectionProperty)sectionsPulled[0];
            ISectionProperty sp2 = (ISectionProperty)sectionsPulled[1];

            // Check
            Assert.IsNotNull(sp1);
            Assert.IsNotNull(sp2);
            Assert.IsTrue(comparer.Equals(section1, sp1));
            Assert.IsTrue(comparer.Equals(section2, sp2));
            Assert.IsFalse(comparer.Equals(sp1, sp2));
            Assert.AreEqual(sectionsPulled.Count, 2);
        }
    }

}

