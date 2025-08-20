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
using BH.Engine.Base;
using BH.oM.Data.Requests;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using Dlubal.WS.Rfem6.Model;
using BH.Engine.Geometry;
using System.Security.Permissions;

namespace RFEM_Toolkit_Test.Elements
{


    public class BarLoad_Test

    {

        RFEM6Adapter adapter;

        Line line0;
        Line line1;
        Line line2;
        Line line3;

        ISectionProperty steelSection;

        Loadcase loadCase;

        Bar barSteelSection0;
        Bar barSteelSection1;
        Bar barSteelSection2;
        Bar barSteelSection3;

        BH.oM.Base.BHoMGroup<Bar> barGroup0;
        BH.oM.Base.BHoMGroup<Bar> barGroup1;
        BH.oM.Base.BHoMGroup<Bar> barGroup2;
        BH.oM.Base.BHoMGroup<Bar> barGroup3;
        BH.oM.Base.BHoMGroup<Bar> barGroup4;
        BH.oM.Base.BHoMGroup<Bar> barGroup5;

        BarUniformlyDistributedLoad barLoadInclined0;
        BarUniformlyDistributedLoad barLoadInclined1;
        BarUniformlyDistributedLoad barLoadInclined2;

        BarUniformlyDistributedLoad barLoad0;
        BarUniformlyDistributedLoad barLoad1;
        BarUniformlyDistributedLoad barLoad2;
        BarUniformlyDistributedLoad barLoad3;
        BarUniformlyDistributedLoad barLoad4;
        BarUniformlyDistributedLoad barLoad5;
        BarUniformlyDistributedLoad barLoad6;
        BarUniformlyDistributedLoad barLoad7;
        BarUniformlyDistributedLoad barLoad8;

        BarUniformlyDistributedLoad barLoad9;
        BarUniformlyDistributedLoad barLoad10;
        BarUniformlyDistributedLoad barLoad11;
        BarUniformlyDistributedLoad barLoad12;
        BarUniformlyDistributedLoad barLoad13;
        BarUniformlyDistributedLoad barLoad14;
        BarUniformlyDistributedLoad barLoad15;
        BarUniformlyDistributedLoad barLoad16;
        BarUniformlyDistributedLoad barLoad17;


        BarUniformlyDistributedLoad barLoad18;
        BarUniformlyDistributedLoad barLoad19;
        BarUniformlyDistributedLoad barLoad20;
        BarUniformlyDistributedLoad barLoad21;
        BarUniformlyDistributedLoad barLoad22;
        BarUniformlyDistributedLoad barLoad23;
        BarUniformlyDistributedLoad barLoad24;
        BarUniformlyDistributedLoad barLoad25;
        BarUniformlyDistributedLoad barLoad26;

        BarUniformlyDistributedLoad barLoad27;
        BarUniformlyDistributedLoad barLoad28;
        BarUniformlyDistributedLoad barLoad29;
        BarUniformlyDistributedLoad barLoad30;
        BarUniformlyDistributedLoad barLoad31;
        BarUniformlyDistributedLoad barLoad32;
        BarUniformlyDistributedLoad barLoad33;
        BarUniformlyDistributedLoad barLoad34;
        BarUniformlyDistributedLoad barLoad35;

        List<BarUniformlyDistributedLoad> inclinedForces = new List<BarUniformlyDistributedLoad>();
        List<BarUniformlyDistributedLoad> axisAlForces = new List<BarUniformlyDistributedLoad>();
        List<BarUniformlyDistributedLoad> axisAlMoments = new List<BarUniformlyDistributedLoad>();
        List<BarUniformlyDistributedLoad> revAxisAlForces = new List<BarUniformlyDistributedLoad>();
        List<BarUniformlyDistributedLoad> revAxisAlMoments = new List<BarUniformlyDistributedLoad>();



        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [SetUp]
        public void EveryTimeSetUp()
        {
            //adapter = new RFEM6Adapter(true);
            adapter.Push(new List<Bar>() { barSteelSection0, barSteelSection1, barSteelSection2 });
        }

        [OneTimeSetUp]
        public void SetUpScenario()
        {
            adapter = new RFEM6Adapter(true);

            //Set Up Sections
            steelSection = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;

            //Line
            line0 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 5 }, new Point() { X = 10, Y = 0, Z = 0 });
            line1 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 5 }, new Point() { X = 0, Y = 10, Z = 0 });
            line2 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 5 }, new Point() { X = -10, Y = 0, Z = 0 });
            line3 = BH.Engine.Geometry.Create.Line(new Point() { X = 0, Y = 0, Z = 5 }, new Point() { X = 0, Y = -10, Z = 0 });

            // Bar
            barSteelSection0 = BH.Engine.Structure.Create.Bar(line0, steelSection, 0);
            barSteelSection1 = BH.Engine.Structure.Create.Bar(line1, steelSection, 0);
            barSteelSection2 = BH.Engine.Structure.Create.Bar(line2, steelSection, 0);
            barSteelSection3 = BH.Engine.Structure.Create.Bar(line3, steelSection, 0);

            // Push Bars to RFEM
            //adapter.Push(new List<Bar>() { barSteelSection0, barSteelSection1, barSteelSection2, barSteelSection3 });

            // Defining Loadcase
            loadCase = new BH.oM.Structure.Loads.Loadcase() { Name = "Loadcase", Nature = LoadNature.Dead, Number = 1 };

            // Defining group
            barGroup0 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { barSteelSection0 } };
            barGroup1 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { barSteelSection1 } };
            barGroup2 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { barSteelSection2 } };
            barGroup3 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { barSteelSection3 } };

            //Defining Loads

            // Inclined Forces
            barLoadInclined0 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, BH.Engine.Geometry.Create.Vector(100, 100, 0), null, LoadAxis.Global, false);
            barLoadInclined1 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, BH.Engine.Geometry.Create.Vector(100, 100, 0), LoadAxis.Global, false);
            barLoadInclined2 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, BH.Engine.Geometry.Create.Vector(100, 100, 0), BH.Engine.Geometry.Create.Vector(100, 100, 0), LoadAxis.Global, false);
            inclinedForces.AddRange(new List<BarUniformlyDistributedLoad>() { barLoadInclined0, barLoadInclined1, barLoadInclined2 });
            //inclinedForces.AddRange(new List<BarUniformlyDistributedLoad>() { barLoadInclined2 });

            // Forces
            barLoad0 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, Vector.XAxis * 100, null, LoadAxis.Global, false);
            barLoad1 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, Vector.XAxis * 100, null, LoadAxis.Global, true);
            barLoad2 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, Vector.XAxis * 100, null, LoadAxis.Local, false);
            barLoad3 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, Vector.YAxis * 100, null, LoadAxis.Global, false);
            barLoad4 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, Vector.YAxis * 100, null, LoadAxis.Global, true);
            barLoad5 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, Vector.YAxis * 100, null, LoadAxis.Local, false);
            barLoad6 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, Vector.ZAxis * 100, null, LoadAxis.Global, false);
            barLoad7 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, Vector.ZAxis * 100, null, LoadAxis.Global, true);
            barLoad8 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, Vector.ZAxis * 100, null, LoadAxis.Local, false);

            axisAlForces = new List<BarUniformlyDistributedLoad>() { barLoad0, barLoad1, barLoad2, barLoad3, barLoad4, barLoad5, barLoad6, barLoad7, barLoad8 };

            // Moments
            barLoad9 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, Vector.XAxis * 100, LoadAxis.Global, false);
            barLoad10 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, Vector.XAxis * 100, LoadAxis.Global, true);
            barLoad11 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, Vector.XAxis * 100, LoadAxis.Local, false);
            barLoad12 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, null, Vector.YAxis * 100, LoadAxis.Global, false);
            barLoad13 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, null, Vector.YAxis * 100, LoadAxis.Global, true);
            barLoad14 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, null, Vector.YAxis * 100, LoadAxis.Local, false);
            barLoad15 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, null, Vector.ZAxis * 100, LoadAxis.Global, false);
            barLoad16 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, null, Vector.ZAxis * 100, LoadAxis.Global, true);
            barLoad17 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, null, Vector.ZAxis * 100, LoadAxis.Local, false);
            axisAlMoments = new List<BarUniformlyDistributedLoad>() { barLoad9, barLoad10, barLoad11, barLoad12, barLoad13, barLoad14, barLoad15, barLoad16, barLoad17 };

            //// Reversed - Forces
            // Forces
            barLoad18 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, Vector.XAxis.Reverse() * 100, null, LoadAxis.Global, false);
            barLoad19 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, Vector.XAxis.Reverse() * 100, null, LoadAxis.Global, true);
            barLoad20 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, Vector.XAxis.Reverse() * 100, null, LoadAxis.Local, false);
            barLoad21 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, Vector.YAxis.Reverse() * 100, null, LoadAxis.Global, false);
            barLoad22 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, Vector.YAxis.Reverse() * 100, null, LoadAxis.Global, true);
            barLoad23 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, Vector.YAxis.Reverse() * 100, null, LoadAxis.Local, false);
            barLoad24 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, Vector.ZAxis.Reverse() * 100, null, LoadAxis.Global, false);
            barLoad25 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, Vector.ZAxis.Reverse() * 100, null, LoadAxis.Global, true);
            barLoad26 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, Vector.ZAxis.Reverse() * 100, null, LoadAxis.Local, false);

            revAxisAlForces = new List<BarUniformlyDistributedLoad>() { barLoad18, barLoad19, barLoad20, barLoad21, barLoad22, barLoad23, barLoad24, barLoad25, barLoad26 };

            //// Reversed - Moments
            barLoad27 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, Vector.XAxis.Reverse() * 100, LoadAxis.Global, false);
            barLoad28 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, Vector.XAxis.Reverse() * 100, LoadAxis.Global, true);
            barLoad29 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup0, null, Vector.XAxis.Reverse() * 100, LoadAxis.Local, false);
            barLoad30 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, null, Vector.YAxis.Reverse() * 100, LoadAxis.Global, false);
            barLoad31 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, null, Vector.YAxis.Reverse() * 100, LoadAxis.Global, true);
            barLoad32 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup1, null, Vector.YAxis.Reverse() * 100, LoadAxis.Local, false);
            barLoad33 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, null, Vector.ZAxis.Reverse() * 100, LoadAxis.Global, false);
            barLoad34 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, null, Vector.ZAxis.Reverse() * 100, LoadAxis.Global, true);
            barLoad35 = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(loadCase, barGroup2, null, Vector.ZAxis.Reverse() * 100, LoadAxis.Local, false);
            revAxisAlMoments = new List<BarUniformlyDistributedLoad>() { barLoad27, barLoad28, barLoad29, barLoad30, barLoad31, barLoad32, barLoad33, barLoad34, barLoad35 };
        }

        [Test]
        public void PushPullInclinedForces()
        {
            //Act
            adapter.Push(inclinedForces);

            FilterRequest barLoadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            List<BarUniformlyDistributedLoad> barLoads = adapter.Pull(barLoadFilter).ToList().Select(p => (BarUniformlyDistributedLoad)p).ToList();

            barLoads.Count();

            ////Assert
            //Check for amount of loads push + pulled
            Assert.AreEqual(barLoads.Count(), inclinedForces.Count());

            //Check if loads have forces/moment
            Assert.IsTrue(barLoads[0].Force == inclinedForces[0].Force);
            Assert.IsTrue(barLoads[0].Moment == inclinedForces[0].Moment);
            Assert.IsTrue(barLoads[1].Force == inclinedForces[1].Force);
            Assert.IsTrue(barLoads[1].Moment == inclinedForces[1].Moment);
            Assert.IsTrue(barLoads[2].Force == inclinedForces[2].Force);
            Assert.IsTrue(barLoads[2].Moment == inclinedForces[2].Moment);


        }

        [Test]
        public void PushPullOrientationTestAxisParallelForces()
        {
            //Act
            adapter.Push(axisAlForces);

            FilterRequest pointLoadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            List<BarUniformlyDistributedLoad> pointLoad = adapter.Pull(pointLoadFilter).ToList().Select(p => (BarUniformlyDistributedLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(axisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 1;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(axisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 2;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));

            i = 3;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(axisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 4;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(axisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 5;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));

            i = 6;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(axisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 7;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(axisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 8;
            Assert.True(axisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlForces[i].Axis.Equals(pointLoad[i].Axis));



        }

        [Test]
        public void PushPullOrientationTestAxisParallelMoments()
        {
            //Act
            adapter.Push(axisAlMoments);

            FilterRequest pointLoadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            List<BarUniformlyDistributedLoad> pointLoad = adapter.Pull(pointLoadFilter).ToList().Select(p => (BarUniformlyDistributedLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(axisAlMoments[i].Moment.Equals(pointLoad[i].Moment));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 1;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 2;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 3;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 4;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 5;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 6;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 7;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 8;
            Assert.True(axisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(axisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

        }


        [Test]
        public void PushPullOrientationTestAntiAxisParallelForces()
        {

            //Act
            adapter.Push(revAxisAlForces);

            FilterRequest pointLoadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            List<BarUniformlyDistributedLoad> pointLoad = adapter.Pull(pointLoadFilter).ToList().Select(p => (BarUniformlyDistributedLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(revAxisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 1;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(revAxisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 2;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));

            i = 3;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(revAxisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 4;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(revAxisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 5;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));

            i = 6;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(revAxisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 7;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));
            Assert.True(revAxisAlForces[i].Projected.Equals(pointLoad[i].Projected));

            i = 8;
            Assert.True(revAxisAlForces[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlForces[i].Axis.Equals(pointLoad[i].Axis));


        }

        [Test]
        public void PushPullOrientationTestAntiAxisParallelMoments()
        {

            //Act
            adapter.Push(revAxisAlMoments);

            FilterRequest pointLoadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            List<BarUniformlyDistributedLoad> pointLoad = adapter.Pull(pointLoadFilter).ToList().Select(p => (BarUniformlyDistributedLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(revAxisAlMoments[i].Moment.Equals(pointLoad[i].Moment));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 1;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 2;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 3;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 4;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 5;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 6;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 7;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));

            i = 8;
            Assert.True(revAxisAlMoments[i].Force.Equals(pointLoad[i].Force));
            Assert.True(revAxisAlMoments[i].Axis.Equals(pointLoad[i].Axis));


        }


    }
}

