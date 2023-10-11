/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Adapters.RFEM6;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Data.Library;
using BH.Engine.Library;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base;
using BH.Engine.Structure;
using BH.oM.Structure.Loads;

namespace RFEM_Toolkit_Test.Loading
{


    public class PushPullLoadCases

    {

        RFEM6Adapter adapter;
        Node n1;
        Node n2;
        Node n3;
        Node n4;
        Node n5;
        Node n6;
        Node n7;
        Node n8;
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

        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [Test]
        public void PushPullLoadCase()
        {

            //LoadCases
            Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
            Loadcase loadcaseSDL = new Loadcase() { Name = "SuperDeadLoad", Nature = LoadNature.SuperDead, Number = 1 };
            Loadcase loadcaseLL = new Loadcase() { Name = "LiveLoad", Nature = LoadNature.Live, Number = 1 };
            Loadcase loadcaseTemp = new Loadcase() { Name = "TemperatureLoad", Nature = LoadNature.Temperature, Number = 1 };
            Loadcase loadcaseWind = new Loadcase() { Name = "WindLoad", Nature = LoadNature.Wind, Number = 1 };
            Loadcase loadcaseAccident = new Loadcase() { Name = "AccidentalLoad", Nature = LoadNature.Accidental, Number = 1 };
            Loadcase loadcaseCase = new Loadcase() { Name = "SnowLoad", Nature = LoadNature.Snow, Number = 1 };

            HashSet<Loadcase> loadcaseSet = new HashSet<Loadcase>(new LoadCaseComparer()) { loadcaseDL, loadcaseSDL, loadcaseLL, loadcaseTemp, loadcaseWind, loadcaseAccident, loadcaseCase };

            adapter.Push(loadcaseSet.ToList());

            FilterRequest loadCaseFilter = new FilterRequest() { Type = typeof(Loadcase) };
            var loadCases = adapter.Pull(loadCaseFilter).ToList();
            Loadcase lc0 = (Loadcase)loadCases[0];
            Loadcase lc1 = (Loadcase)loadCases[1];
            Loadcase lc2 = (Loadcase)loadCases[2];
            Loadcase lc3 = (Loadcase)loadCases[3];
            Loadcase lc4 = (Loadcase)loadCases[4];
            Loadcase lc5 = (Loadcase)loadCases[5];
            Loadcase lc6 = (Loadcase)loadCases[6];


            Assert.AreEqual(loadCases.Count(), loadcaseSet.Count());
            Assert.True(loadcaseSet.Contains(lc0));
            Assert.True(loadcaseSet.Contains(lc1));
            Assert.True(loadcaseSet.Contains(lc2));
            Assert.True(loadcaseSet.Contains(lc3));
            Assert.True(loadcaseSet.Contains(lc4));
            Assert.True(loadcaseSet.Contains(lc5));
            Assert.True(loadcaseSet.Contains(lc6));

        }

        //[Test]
        //public void PushPullLoad()
        //{

        //    //Bar Setup
        //    n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 }, Support = BH.Engine.Structure.Create.PinConstraint6DOF() };
        //    n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 }, Support = BH.Engine.Structure.Create.PinConstraint6DOF() };
        //    n3 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 }, Support = BH.Engine.Structure.Create.PinConstraint6DOF() };
        //    n4 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 }, Support = BH.Engine.Structure.Create.PinConstraint6DOF() };

        //    ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
        //    ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

        //    bar1In = new Bar() { StartNode = n1, EndNode = n2, SectionProperty = section1 };
        //    bar2In = new Bar() { StartNode = n3, EndNode = n4, SectionProperty = section2 };

        //    //adapter.Push(new List<IBHoMObject>() { bar1In, bar2In });

        //    Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
        //    Loadcase loadcaseSDL = new Loadcase() { Name = "SuperDeadLoad", Nature = LoadNature.SuperDead, Number = 1 };
        //    Loadcase loadcaseLL = new Loadcase() { Name = "LiveLoad", Nature = LoadNature.Live, Number = 1 };
        //    Loadcase loadcaseTemp = new Loadcase() { Name = "TemperatureLoad", Nature = LoadNature.Temperature, Number = 1 };
        //    Loadcase loadcaseWind = new Loadcase() { Name = "WindLoad", Nature = LoadNature.Wind, Number = 1 };
        //    Loadcase loadcaseAccident = new Loadcase() { Name = "AccidentalLoad", Nature = LoadNature.Accidental, Number = 1 };
        //    Loadcase loadcaseCase = new Loadcase() { Name = "SnowLoad", Nature = LoadNature.Snow, Number = 1 };

        //    //adapter.Push(new List<Loadcase> { loadcaseDL, loadcaseSDL, loadcaseLL, loadcaseTemp, loadcaseWind, loadcaseAccident, loadcaseCase });
        //    var group0 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar1In, bar2In } };
        //    var group1 = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node> { n1, n2, n3, n4 } };


        //    var barLoad0 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = group0, Loadcase = loadcaseDL };
        //    var barLoad1 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = group0, Loadcase = loadcaseSDL };
        //    var pointLoad0 = new BH.oM.Structure.Loads.PointLoad { Objects = group1, Loadcase = loadcaseSDL };


        //    BH.oM.Structure.Loads.AreaUniformlyDistributedLoad areaLoad = new AreaUniformlyDistributedLoad();
        //    BH.oM.Structure.Loads.PointLoad pointLoad = new PointLoad();
        //    BH.oM.Structure.Loads.GeometricalLineLoad lineLoad = new GeometricalLineLoad();


        //    adapter.Push(new List<IBHoMObject> { bar1In, bar2In, barLoad0, barLoad1, pointLoad0 });


        //    //FilterRequest loadCaseFilter = new FilterRequest() { Type = typeof(Loadcase) };
        //    //var loadCases = adapter.Pull(loadCaseFilter).ToList();
        //    //Loadcase lc0 = (Loadcase)loadCases[0];
        //    //Loadcase lc1 = (Loadcase)loadCases[1];
        //    //Loadcase lc2 = (Loadcase)loadCases[2];
        //    //Loadcase lc3 = (Loadcase)loadCases[3];
        //    //Loadcase lc4 = (Loadcase)loadCases[4];
        //    //Loadcase lc5 = (Loadcase)loadCases[5];
        //    //Loadcase lc6 = (Loadcase)loadCases[6];

        //}


    }
}