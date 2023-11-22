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
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Structure;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using System.Security.Policy;

namespace RFEM_Toolkit_Test.Loading
{


    public class PushPullLoad

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
        Bar bar;


        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);
        }

        [Test]
        public void PushPullLoadCase()
        {
            n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 }, Support = BH.Engine.Structure.Create.FixConstraint6DOF() };
            n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 }, Support = BH.Engine.Structure.Create.FixConstraint6DOF() };

            ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            bar = new Bar() { StartNode = n1, EndNode = n2, SectionProperty = section1 };

            //BH.oM.Base.BHoMGroup<Bar> group = new BH.oM.Base.BHoMGroup<Bar>() { Elements=new List<Bar> {bar } };

            //LoadCases
            Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
            Loadcase loadcaseSDL = new Loadcase() { Name = "SuperDeadLoad", Nature = LoadNature.SuperDead, Number = 1 };
            Loadcase loadcaseWind = new Loadcase() { Name = "WindLoads", Nature = LoadNature.Wind, Number = 1 };


            var barGroup = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar } };
            var barLoad0 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup, Loadcase = loadcaseDL, Force = new Vector() { X = 0, Y = 20000, Z = 1000 } };
            var barLoadW = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup, Loadcase = loadcaseWind, Force = new Vector() { X = 0, Y = 20000, Z = 1000 } };

            var pointGroup = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node> { n1, n2 } };
            var pointLoad = new BH.oM.Structure.Loads.PointLoad() { Objects = pointGroup, Loadcase = loadcaseSDL, Force = new Vector() { X = 0, Y = 0, Z = 100000 } };

            HashSet<ILoad> barloads0 = new HashSet<ILoad>() { barLoad0 };
            HashSet<ILoad> pointloads0 = new HashSet<ILoad>() { pointLoad };
            HashSet<ILoad> barloadW = new HashSet<ILoad>() { barLoadW };



            adapter.Push(barloads0.ToList());
            adapter.Push(pointloads0.ToList());
            adapter.Push(barloadW.ToList());


            //FilterRequest loadFilter = new FilterRequest() { Type = typeof(PointLoad) };
            //var loads = adapter.Pull(loadFilter).ToList();
            //PointLoad l0 = (PointLoad)loads[0];

            //Assert.AreEqual(loadCases.Count(), loadcaseSet.Count());
            //Assert.True(loadcaseSet.Contains(lc0));

        }

        //[Test]
        //public void PullLineLoad()
        //{
        //    n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 }, Support = BH.Engine.Structure.Create.FixConstraint6DOF() };
        //    n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 }, Support = BH.Engine.Structure.Create.FixConstraint6DOF() };

        //    //ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
        //    //ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

        //    //bar = new Bar() { StartNode = n1, EndNode = n2, SectionProperty = section1 };

        //    ////BH.oM.Base.BHoMGroup<Bar> group = new BH.oM.Base.BHoMGroup<Bar>() { Elements=new List<Bar> {bar } };

        //    ////LoadCases
        //    //Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
        //    Loadcase loadcaseSDL = new Loadcase() { Name = "SuperDeadLoad", Nature = LoadNature.SuperDead, Number = 1 };

        //    //var barGroup = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar } };
        //    //var barLoad0 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup, Loadcase = loadcaseDL, Force = new Vector() { X = 0, Y = 20000, Z = 1000 } };

        //    var pointGroup = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node> { n1, n2 } };
        //    var pointLoad = new BH.oM.Structure.Loads.PointLoad() { Objects = pointGroup, Loadcase = loadcaseSDL, Force = new Vector() { X = 0, Y = 0, Z = 100000 } };

        //    //HashSet<ILoad> loadcaseSet = new HashSet<ILoad>() { barLoad0, pointLoad };

        //    //adapter.Push(loadcaseSet.ToList());

        //    FilterRequest loadFilter = new FilterRequest() { Type = typeof(GeometricalLineLoad) };
        //    var loads = adapter.Pull(loadFilter).ToList();
        //    GeometricalLineLoad l0 = (GeometricalLineLoad)loads[0];

        //    //Assert.AreEqual(loadCases.Count(), loadcaseSet.Count());
        //    //Assert.True(loadcaseSet.Contains(lc0));

        //}


        [Test]
        public void PushPointLoad()
        {
            n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
            n3 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 } };
            n4 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };

            Loadcase loadcaseDL0 = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
            Loadcase loadcaseDL1 = new Loadcase() { Name = "Windload", Nature = LoadNature.Wind, Number = 1 };

            var pointGroup0 = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node> { n1, n2 } };
            var pointLoad0 = new BH.oM.Structure.Loads.PointLoad() { Objects = pointGroup0, Loadcase = loadcaseDL0, Moment = new Vector() { X = 100000, Y = 0, Z = 0 } };

            var pointGroup1 = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node> { n3, n4 } };
            var pointLoad1 = new BH.oM.Structure.Loads.PointLoad() { Objects = pointGroup1, Loadcase = loadcaseDL1, Moment = new Vector() { X = 0, Y = 100000, Z = 0 } };

            adapter.Push(new List<ILoad>() { pointLoad0 });
            adapter.Push(new List<ILoad>() { pointLoad1 });

        }


        [Test]
        public void PullSurfaceLoad()
        {
            //FilterRequest loadFilter = new FilterRequest() { Type = typeof(AreaUniformlyDistributedLoad) };
            //var areaLoadRead = adapter.Pull(loadFilter).ToList();
            //AreaUniformlyDistributedLoad l0 = (AreaUniformlyDistributedLoad) areaLoadRead[0];


            //FilterRequest panelFilter = new FilterRequest() { Type = typeof(Panel) };
            //var panelReader = adapter.Pull(panelFilter).ToList();
            //Panel p0 = (Panel)panelReader[0];
            //Panel p1 = (Panel)panelReader[1];
            ////Panel p2 = (Panel)panelReader[2];

            n1 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 10 } };
            n2 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 10 } };
            n3 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
            n4 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 } };

            Edge e0 = new Edge() { Curve = BH.Engine.Geometry.Create.Line(n1.Position, n2.Position) };
            Edge e1 = new Edge() { Curve = BH.Engine.Geometry.Create.Line(n2.Position, n3.Position) };
            Edge e2 = new Edge() { Curve = BH.Engine.Geometry.Create.Line(n3.Position, n4.Position) };
            Edge e3 = new Edge() { Curve = BH.Engine.Geometry.Create.Line(n4.Position, n1.Position) };

            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;

            BH.oM.Structure.SurfaceProperties.ConstantThickness surfaceProp = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.3, Material = concrete };

            Panel panel = new Panel() { ExternalEdges = new List<Edge>() {e0,e1,e2,e3}, Property = surfaceProp };

            Loadcase loadcaseWind = new Loadcase() { Name = "Windload", Nature = LoadNature.Wind, Number = 1 };


            BHoMGroup<Node> nodeGroup = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node>() { n1 } };    
            PointLoad pointLoad = new PointLoad() { Force= BH.Engine.Geometry.Create.Vector(100000000000, 0, 0),Loadcase=loadcaseWind,  Objects=nodeGroup};


            //adapter.Push(new List<IObject>() { pointLoad });
            //adapter.Push(new List<IObject>() { panel, areaLoad0 });
            adapter.Push(new List<IObject>() { panel });


            FilterRequest panelFilter = new FilterRequest() { Type = typeof(Panel) };
            var panelReader = adapter.Pull(panelFilter).ToList();
            Panel p0 = (Panel)panelReader[0];

            AreaUniformlyDistributedLoad areaLoad0 = BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcaseWind, BH.Engine.Geometry.Create.Vector(100000000000,0,0), new List<Panel>() { p0 });
            
            
            
            adapter.Push(new List<IObject>() { areaLoad0 });

        }

        [Test]
        public void PullPointLoad()
        {
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(PointLoad) };
            var loads = adapter.Pull(loadFilter).ToList();


        }

        [Test]
        public void PullGeometricalLineload()
        {
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(GeometricalLineLoad) };
            var loads = adapter.Pull(loadFilter).ToList();
            var l0 = (GeometricalLineLoad)loads[0];

        }

        //[Test]
        //public void PushLineLoad()
        //{
        //    n1 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 } };
        //    n2 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
        //    n3 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
        //    n4 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };

        //    Line l1 = new Line() { Start = n1.Position, End = n2.Position };
        //    Line l2 = new Line() { Start = n2.Position, End = n3.Position };
        //    Line l3 = new Line() { Start = n3.Position, End = n4.Position };
        //    Line l4 = new Line() { Start = n4.Position, End = n1.Position };

        //    List<Line> lines = new List<Line>() { l1, l2, l3, l4 };

        //    List<Edge> edges = lines.Select(l => new Edge() { Curve = l }).ToList();
        //    Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
        //    var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;

        //    BH.oM.Structure.SurfaceProperties.ConstantThickness surfaceProp = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.3, Material = concrete };

        //    Panel panel = new Panel() { ExternalEdges = edges, Property = surfaceProp };
        //    GeometricalLineLoad lineLoad = new GeometricalLineLoad() { Location = l1, ForceA = new Vector() { X = 0, Y = 0, Z = 100 }, Loadcase = loadcaseDL };

        //    adapter.Push(new List<Panel>() { panel });
        //    adapter.Push(new List<GeometricalLineLoad>() { lineLoad });

        //}

        //[Test]
        //public void PushPullAreaLoads()
        //{


        //    n1 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 } };
        //    n2 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
        //    n3 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
        //    n4 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };

        //    Line l1 = new Line() { Start = n1.Position, End = n2.Position };
        //    Line l2 = new Line() { Start = n2.Position, End = n3.Position };
        //    Line l3 = new Line() { Start = n3.Position, End = n4.Position };
        //    Line l4 = new Line() { Start = n4.Position, End = n1.Position };

        //    List<Line> lines = new List<Line>() { l1, l2, l3, l4 };

        //    List<Edge> edges = lines.Select(l => new Edge() { Curve = l }).ToList();
        //    Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
        //    var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;

        //    BH.oM.Structure.SurfaceProperties.ConstantThickness surfaceProp = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.3, Material = concrete };

        //    Panel panel = new Panel() { ExternalEdges = edges, Property = surfaceProp };


        //    AreaUniformlyDistributedLoad areaLoad = BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcaseDL, Vector.ZAxis, new List<Panel>() { panel });


        //    var k = new BH.oM.Base.BHoMGroup<IAreaElement>() { Elements = new List<IAreaElement>() { panel } };

        //    AreaUniformlyDistributedLoad areaLoaD = new AreaUniformlyDistributedLoad()
        //    {
        //        Loadcase = loadcaseDL,
        //        Pressure = BH.Engine.Geometry.Create.Vector(0, 0, 10000000),
        //        Objects = k
        //    };


        //    adapter.Push(new List<IObject>() { panel });
        //    adapter.Push(new List<IObject>() { areaLoaD });
        //    //adapter.Push(new List<IObject>() { panel, areaLoaD });


        //    //FilterRequest loadFilter = new FilterRequest() { Type = typeof(AreaUniformlyDistributedLoad) };
        //    //var loads = adapter.Pull(loadFilter).ToList();
        //    //AreaUniformlyDistributedLoad l0 = (AreaUniformlyDistributedLoad)loads[0];

        //    FilterRequest loadFilter = new FilterRequest() { Type = typeof(Panel) };
        //    var panelRead = adapter.Pull(loadFilter).ToList();
        //    Panel l0 = (Panel)panelRead[0];

        //    RFEMPanelComparer panelComparer = new RFEMPanelComparer();
        //    Assert.IsTrue(panelComparer.Equals(panel, l0));

        //}


    }
}