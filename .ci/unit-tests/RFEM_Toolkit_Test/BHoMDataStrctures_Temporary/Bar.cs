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
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base;
using BH.Engine.Structure;

namespace RFEM_Toolkit_Test.BHoMDataStrctures_Temporary
{


    public class PushPullBar

    {

        RFEM6Adapter adapter;
        Node n1;
        Node n2;
        Node n3;
        Node n4;
        IMaterialFragment inputSection2;
        IMaterialFragment inputSection1;
        BarEndNodesDistanceComparer comparer;
        BarRelease release1;
        BarRelease release2;
        BarRelease release3;
        BarRelease release4;
        Bar bar1In;
        Bar bar2In;
        Bar bar3In;
        Bar bar4In;


        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);
        }

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }

        [Test]
        public void PushPullOfBars()
        {

            comparer = new BarEndNodesDistanceComparer(3);

            n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
            n3 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 } };
            n4 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };

            ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            bar1In = new Bar() { Start = n1, End = n2, SectionProperty = section1 };
            bar2In = new Bar() { Start = n3, End = n4, SectionProperty = section2 };

            adapter.Push(new List<IBHoMObject>() { bar1In, bar2In });
            adapter.Push(new List<IBHoMObject>() { bar1In, bar2In });

            FilterRequest barFilter = new FilterRequest() { Type = typeof(Bar) };
            var bars = adapter.Pull(barFilter).ToList();

            Bar bar1Out = (Bar)bars[0];
            Bar bar2Out = (Bar)bars[1];

            Assert.True(comparer.Equals(bar1Out, bar1In) || comparer.Equals(bar1Out, bar2In));
            Assert.True(comparer.Equals(bar2Out, bar1In) || comparer.Equals(bar2Out, bar2In));
            Assert.True(bars.Count == 2);



        }

        [Test]
        public void PushPullOfBarsAndHinges()
        {

            comparer = new BarEndNodesDistanceComparer(3);



            ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;


            release1 = Create.BarReleaseFixFix("");
            release2 = Create.BarReleaseFixFix("");
            release3 = Create.BarReleasePinPin("");
            release4 = Create.BarReleasePinSlip("");

            bar1In = new Bar() { Start = n1, End = n2, SectionProperty = section1, Release = release1 };
            bar2In = new Bar() { Start = n3, End = n4, SectionProperty = section2, Release = release2 };
            bar3In = new Bar() { Start = n1, End = n2, SectionProperty = section1, Release = release3 };
            bar4In = new Bar() { Start = n3, End = n4, SectionProperty = section2, Release = release4 };



            adapter.Push(new List<IBHoMObject>() { bar1In, bar2In, bar2In, bar4In });
            adapter.Push(new List<IBHoMObject>() { bar1In, bar2In, bar2In, bar4In });


            FilterRequest barFilter = new FilterRequest() { Type = typeof(Bar) };
            var bars = adapter.Pull(barFilter).ToList();

            Bar bar1Out = (Bar)bars[0];
            Bar bar2Out = (Bar)bars[1];



            Assert.True(comparer.Equals(bar1Out, bar1In) || comparer.Equals(bar1Out, bar2In));
            Assert.True(comparer.Equals(bar2Out, bar1In) || comparer.Equals(bar2Out, bar2In));
            Assert.True(bars.Count == 2);



        }

        [Test]
        public void PushAndPullOfFEAType()
        {

            comparer = new BarEndNodesDistanceComparer(3);

            n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
            n3 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 } };
            n4 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
            //n5 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 20 } };
            //n6 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 20 } };
            //n7 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 30 } };
            //n8 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 30 } };

            ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            release1 = Create.BarReleaseFixFix("");
            release2 = Create.BarReleaseFixFix("");
            release3 = Create.BarReleasePinPin("");
            release4 = Create.BarReleasePinSlip("");

            bar1In = new Bar() { Start = n3, End = n4, SectionProperty = section2, Release = release2, FEAType = BarFEAType.Axial };
            bar2In = new Bar() { Start = n1, End = n2, SectionProperty = section1, Release = release3, FEAType = BarFEAType.TensionOnly };
            adapter.Push(new List<IBHoMObject>() { bar1In, bar2In });

            FilterRequest barFilter = new FilterRequest() { Type = typeof(Bar) };
            var bars = adapter.Pull(barFilter).ToList();

            Bar bar1Out = (Bar)bars[0];
            Bar bar2Out = (Bar)bars[1];



            Assert.True(bar1In.FEAType.ToString().Equals(bar1Out.FEAType.ToString()));
            Assert.True(bar2In.FEAType.ToString().Equals(bar2Out.FEAType.ToString()));
            //Assert.True(bars.Count == 2);



        }



    }
}

