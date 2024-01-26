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
using BH.oM.Data.Requests;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace RFEM_Toolkit_Test.Elements
{


    public class BarTestClass

    {

        RFEM6Adapter adapter;

        Line line0;
        Line line1;
        Line line2;
        Line line3;



        IMaterialFragment glulam;
        IMaterialFragment timberC;
        //IMaterialFragment timberT;
        //IMaterialFragment timberD;

        Concrete concrete;
        //IMaterialFragment steel;

        ISectionProperty steelSection;
        ISectionProperty concreteSection;
        ISectionProperty genericSectionGLTimber;
        ISectionProperty genericSectionSawnTimber;

        Bar barSteelSection;
        Bar barConcreteSection;
        Bar barGLTimber;
        Bar barSawTimber;

        BarEndNodesDistanceComparer comparer = new BarEndNodesDistanceComparer(3);

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);


            //Set Up Materials
            glulam = BH.Engine.Library.Query.Match("Glulam", "GL 20C", true, true).DeepClone() as IMaterialFragment;
            timberC = BH.Engine.Library.Query.Match("SawnTimber", "C14", true, true).DeepClone() as IMaterialFragment;
            concrete = BH.Engine.Library.Query.Match("Concrete", "C30/37", true, true).DeepClone() as Concrete;

            //Set Up Sections
            steelSection = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            concreteSection = BH.Engine.Structure.Create.ConcreteRectangleSection(0.5, 0.2, concrete, "");
            genericSectionGLTimber = BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.RectangleProfile(0.5, 0.2), glulam, "GLSec");
            genericSectionSawnTimber = BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.CircleProfile(0.5), timberC, "SawTimberSec");

            //Line
            line0 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 0 }, new Point() { X = 10, Y = 0, Z = 0 });
            line1 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 0 }, new Point() { X = 0, Y = 10, Z = 0 });
            line2 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 0 }, new Point() { X = -10, Y = 0, Z = 0 });
            line3 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 0 }, new Point() { X = 0, Y = -10, Z = 0 });

            // Bar
            barSteelSection = BH.Engine.Structure.Create.Bar(line0, steelSection, 0);
            barConcreteSection = BH.Engine.Structure.Create.Bar(line1, concreteSection, 0);
            barGLTimber = BH.Engine.Structure.Create.Bar(line2, genericSectionGLTimber, 0);
            barSawTimber = BH.Engine.Structure.Create.Bar(line3, genericSectionSawnTimber, 0);

        }


        [Test]
        public void PushPullBarUnsupported()
        {

            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

            adapter.Push(new List<Bar>() { barSteelSection, barConcreteSection, barGLTimber, barSawTimber });
            adapter.Push(new List<Bar>() { barSteelSection, barConcreteSection, barGLTimber, barSawTimber });


            //Pull it   
            FilterRequest barFilter = new FilterRequest() { Type = typeof(Bar) };
            var barsPulled = adapter.Pull(barFilter).Select(s=>(Bar)s).ToList();
            HashSet<Bar> bars = new HashSet<Bar>(comparer);
            bars.UnionWith(barsPulled.ToHashSet());

            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            //Null Check
            Assert.IsNotNull(barsPulled);

            //Compares pushed to pulled material
            Assert.IsTrue(bars.Contains(barSteelSection));
            Assert.IsTrue(bars.Contains(barConcreteSection));
            Assert.IsTrue(bars.Contains(barGLTimber));
            Assert.IsTrue(bars.Contains(barSawTimber));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(4, barsPulled.Count);


        }

        [Test]
        public void PushPullBarSupported()
        {
            //TODO: Add tests for supported bars
        }

        [Test]
        public void PushPullCombinedBars()
        {
            //TODO: Add tests for combined bars
        }


    }
}
