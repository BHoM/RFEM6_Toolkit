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
        ISectionProperty steelSection1;
        ISectionProperty steelSection2;
        ISectionProperty concreteSection0;
        ISectionProperty concreteSection1;
        ISectionProperty genericSectionGLTimber;
        ISectionProperty genericSectionSawnTimber;
        IProfile rectProfileGLTimber;
        IProfile circleProfileSawnTimber;
        IProfile concreteProfile1;
        IProfile concreteProfile2;

        IMaterialFragment glulam;
        IMaterialFragment timberC;
        Concrete concrete0;
        Concrete concrete1;

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
            //adapter.Wipeout();
        }

        [Test]
        public void PushPullOfSteelSection()
        {
            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            steelSection1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            steelSection2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            //Push it once
            adapter.Push(new List<ISectionProperty>() { steelSection1 });
            adapter.Push(new List<ISectionProperty>() { steelSection1 });
            adapter.Push(new List<ISectionProperty>() { steelSection2 });
            adapter.Push(new List<ISectionProperty>() { steelSection2 });

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
            Assert.IsTrue(sectionPulledSet.Contains(steelSection1));
            Assert.IsTrue(sectionPulledSet.Contains(steelSection2));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(2, sectionsPulled.Count);


        }

        [Test]
        public void PushPullOfConcreteSection()
        {

            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            concrete0 = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as Concrete;
            concrete1 = BH.Engine.Library.Query.Match("Concrete", "C45/55", true, true).DeepClone() as Concrete;

            concreteSection0 = BH.Engine.Structure.Create.ConcreteCircularSection(0.5, concrete0 as Concrete, "ConcreteSection0", null);
            concreteSection1 = BH.Engine.Structure.Create.ConcreteRectangleSection(1, 0.5, concrete1 as Concrete, "ConcreteSection1", null);

            //Push it once
            adapter.Push(new List<ISectionProperty>() { concreteSection0 });
            adapter.Push(new List<ISectionProperty>() { concreteSection0 });
            adapter.Push(new List<ISectionProperty>() { concreteSection1 });
            adapter.Push(new List<ISectionProperty>() { concreteSection1 });

            ////Pull it   
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ISectionProperty)s).ToList();
            HashSet<ISectionProperty> sectionPulledSet = new HashSet<ISectionProperty>(comparer);
            sectionPulledSet.UnionWith(sectionsPulled.ToHashSet());

            //sectionPulledSet.
            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            ////Null Check
            Assert.IsNotNull(sectionsPulled);

            //Compares pushed to pulled material
            Assert.IsTrue(sectionPulledSet.Contains(concreteSection0));
            Assert.IsTrue(sectionPulledSet.Contains(concreteSection1));


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
            glulam = BH.Engine.Library.Query.Match("Glulam", "GL 20C", true, true).DeepClone() as IMaterialFragment;
            timberC = BH.Engine.Library.Query.Match("SawnTimber", "C24", true, true).DeepClone() as IMaterialFragment;


            rectProfileGLTimber = BH.Engine.Spatial.Create.RectangleProfile(0.5, 0.2);
            genericSectionGLTimber = BH.Engine.Structure.Create.GenericSectionFromProfile(rectProfileGLTimber, glulam, "GLSec");

            circleProfileSawnTimber = BH.Engine.Spatial.Create.CircleProfile(0.5);
            genericSectionSawnTimber = BH.Engine.Structure.Create.GenericSectionFromProfile(circleProfileSawnTimber, timberC, "SawTimberSec");


            //Push it once

            adapter.Push(new List<ISectionProperty>() { genericSectionGLTimber });
            adapter.Push(new List<ISectionProperty>() { genericSectionGLTimber });
            adapter.Push(new List<ISectionProperty>() { genericSectionSawnTimber });
            adapter.Push(new List<ISectionProperty>() { genericSectionSawnTimber });

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
            Assert.IsTrue(sectionPulledSet.Contains(genericSectionGLTimber));
            Assert.IsTrue(sectionPulledSet.Contains(genericSectionSawnTimber));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(2, sectionsPulled.Count);


        }

        [Test]
        public void PushPullOfGenericConcreteSections()
        {
            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            //TODO: Add a test for Glulam
            concrete0 = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as Concrete;
            concrete1 = BH.Engine.Library.Query.Match("Concrete", "C45/55", true, true).DeepClone() as Concrete;


            concreteProfile1 = BH.Engine.Spatial.Create.CircleProfile( 0.2);
            concreteSection0 = BH.Engine.Structure.Create.GenericSectionFromProfile(concreteProfile1, concrete0, "ConcreteSection1");

            concreteProfile2 = BH.Engine.Spatial.Create.CircleProfile(0.5);
            concreteSection1 = BH.Engine.Structure.Create.GenericSectionFromProfile(concreteProfile2, concrete1, "ConcreteSection2");


            //Push it once

            adapter.Push(new List<ISectionProperty>() { concreteSection0 });
            adapter.Push(new List<ISectionProperty>() { concreteSection0 });
            adapter.Push(new List<ISectionProperty>() { concreteSection1 });
            adapter.Push(new List<ISectionProperty>() { concreteSection1 });

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
            Assert.IsTrue(sectionPulledSet.Contains(concreteSection0));
            Assert.IsTrue(sectionPulledSet.Contains(concreteSection1));


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


