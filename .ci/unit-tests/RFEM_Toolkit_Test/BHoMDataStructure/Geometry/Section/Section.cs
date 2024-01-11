/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

namespace RFEM_Toolkit_Test.Elements
{


    public class SectionTestClass
    {
        RFEM6Adapter adapter;
        ISectionProperty SteelSection1;
        ISectionProperty SteelSection2;
        ISectionProperty ConcreteSection0;
        ISectionProperty ConcreteSection1;
        ISectionProperty GenericSectionGLTimber;
        ISectionProperty GenericSectionSawnTimber;
        IProfile RectProfileGLTimber;
        IProfile CircleProfileSawnTimber;

        IMaterialFragment Glulam;
        IMaterialFragment TimberC;
        Concrete Concrete0;
        Concrete Concrete1;

        RFEMSectionComparer comparer;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);
            comparer = new RFEMSectionComparer();
        }

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }

        [Test]
        public void PushPullOfSteelSection()
        {
            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            SteelSection1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            SteelSection2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            //Push it once
            adapter.Push(new List<ISectionProperty>() { SteelSection1 });
            adapter.Push(new List<ISectionProperty>() { SteelSection1 });
            adapter.Push(new List<ISectionProperty>() { SteelSection2 });
            adapter.Push(new List<ISectionProperty>() { SteelSection2 });

            ////Pull it   
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ISectionProperty)s).ToList();
            HashSet<ISectionProperty> sectionPulledSet = new HashSet<ISectionProperty>(comparer);
            sectionPulledSet.UnionWith(sectionsPulled.ToHashSet());

            //sectionPulledSet.
            ///***************************************************/
            ///**** Assertions                                ****/
            ///***************************************************/

            ////Null Check
            Assert.IsNotNull(sectionsPulled);

            //Compares pushed to pulled material
            Assert.IsTrue(sectionPulledSet.Contains(SteelSection1));
            Assert.IsTrue(sectionPulledSet.Contains(SteelSection2));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(2, sectionsPulled.Count);


        }

        [Test]
        public void PushPullOfConcreteSection()
        {

            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            Concrete0 = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as Concrete;
            Concrete1 = BH.Engine.Library.Query.Match("Concrete", "C45/55", true, true).DeepClone() as Concrete;

            ConcreteSection0 = BH.Engine.Structure.Create.ConcreteCircularSection(0.5, Concrete0 as Concrete, "ConcreteSection0", null);
            ConcreteSection1 = BH.Engine.Structure.Create.ConcreteRectangleSection(1, 0.5, Concrete1 as Concrete, "ConcreteSection1", null);

            //Push it once
            adapter.Push(new List<ISectionProperty>() { ConcreteSection0 });
            adapter.Push(new List<ISectionProperty>() { ConcreteSection0 });
            adapter.Push(new List<ISectionProperty>() { ConcreteSection1 });
            adapter.Push(new List<ISectionProperty>() { ConcreteSection1 });

            ////Pull it   
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ISectionProperty)s).ToList();
            HashSet<ISectionProperty> sectionPulledSet = new HashSet<ISectionProperty>(comparer);
            sectionPulledSet.UnionWith(sectionsPulled.ToHashSet());

            //sectionPulledSet.
            ///***************************************************/
            ///**** Assertions                                ****/
            ///***************************************************/

            ////Null Check
            Assert.IsNotNull(sectionsPulled);

            //Compares pushed to pulled material
            Assert.IsTrue(sectionPulledSet.Contains(ConcreteSection0));
            Assert.IsTrue(sectionPulledSet.Contains(ConcreteSection1));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(2, sectionsPulled.Count);


        }

        [Test]
        public void PushPullOfGenericTimberSections()
        {
            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            //TODO: Add a test for Glulam
            Glulam = BH.Engine.Library.Query.Match("Glulam", "GL 20C", true, true).DeepClone() as IMaterialFragment;
            TimberC = BH.Engine.Library.Query.Match("SawnTimber", "C24", true, true).DeepClone() as IMaterialFragment;


            RectProfileGLTimber = BH.Engine.Spatial.Create.RectangleProfile(0.5, 0.2);
            GenericSectionGLTimber = BH.Engine.Structure.Create.GenericSectionFromProfile(RectProfileGLTimber, Glulam, "GLSec");

            CircleProfileSawnTimber = BH.Engine.Spatial.Create.CircleProfile(0.5);
            GenericSectionSawnTimber = BH.Engine.Structure.Create.GenericSectionFromProfile(CircleProfileSawnTimber, TimberC, "SawTimberSec");


            //Push it once

            adapter.Push(new List<ISectionProperty>() { GenericSectionGLTimber });
            adapter.Push(new List<ISectionProperty>() { GenericSectionGLTimber });
            adapter.Push(new List<ISectionProperty>() { GenericSectionSawnTimber });
            adapter.Push(new List<ISectionProperty>() { GenericSectionSawnTimber });

            ////Pull it   
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ISectionProperty)s).ToList();
            ISectionProperty mp = (ISectionProperty)sectionsPulled[0];
            HashSet<ISectionProperty> sectionPulledSet = new HashSet<ISectionProperty>(comparer);
            sectionPulledSet.UnionWith(sectionsPulled.ToHashSet());

            //sectionPulledSet.
            ///***************************************************/
            ///**** Assertions                                ****/
            ///***************************************************/

            ////Null Check
            Assert.IsNotNull(sectionsPulled);

            //Compares pushed to pulled material
            Assert.IsTrue(sectionPulledSet.Contains(GenericSectionGLTimber));
            Assert.IsTrue(sectionPulledSet.Contains(GenericSectionSawnTimber));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(2, sectionsPulled.Count);


        }

        [Test]
        public void PushPullOfGenericSections()
        {
            // TODO: Add a test for Generic Section                                                                                            
        }

    }

}
