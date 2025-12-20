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
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.Engine.Geometry;

namespace RFEM_Toolkit_Test.Loading
{


    public class PushPullLoad

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
        Bar bar;
        Bar bar0;
        Bar bar1;
        Bar bar2;
        Bar bar3;
        Bar bar4;
        Bar bar5;





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

            bar = new Bar() { Start = n1, End = n2, SectionProperty = section1 };

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

            Panel panel = new Panel() { ExternalEdges = new List<Edge>() { e0, e1, e2, e3 }, Property = surfaceProp };

            Loadcase loadcaseWind = new Loadcase() { Name = "Windload", Nature = LoadNature.Wind, Number = 1 };


            BHoMGroup<Node> nodeGroup = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node>() { n1 } };
            PointLoad pointLoad = new PointLoad() { Force = BH.Engine.Geometry.Create.Vector(100000000000, 0, 0), Loadcase = loadcaseWind, Objects = nodeGroup };


            //adapter.Push(new List<IObject>() { pointLoad });
            //adapter.Push(new List<IObject>() { panel, areaLoad0 });
            adapter.Push(new List<IObject>() { panel });


            FilterRequest panelFilter = new FilterRequest() { Type = typeof(Panel) };
            var panelReader = adapter.Pull(panelFilter).ToList();
            Panel p0 = (Panel)panelReader[0];

            AreaUniformlyDistributedLoad areaLoad0 = BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcaseWind, BH.Engine.Geometry.Create.Vector(100000000000, 0, 0), new List<Panel>() { p0 });



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

        [Test]
        public void PullBarload()
        {
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            var loads = adapter.Pull(loadFilter).ToList();
            var l0 = (BarUniformlyDistributedLoad)loads[0];






        }

        [Test]
        public void PushFreeLineLoadload()
        {
            Point p0 = new Point() { X = -10, Y = -10, Z = 0 };
            Point p1 = new Point() { X = -10, Y = 10, Z = 0 };
            Point p2 = new Point() { X = 10, Y = 10, Z = 0 };
            Point p3 = new Point() { X = 10, Y = -10, Z = 0 };




            // Create Panel 1
            Edge edge1 = new Edge() { Curve = new Line() { Start = p0, End = p1 } };
            Edge edge2 = new Edge() { Curve = new Line() { Start = p1, End = p2 } };
            Edge edge3 = new Edge() { Curve = new Line() { Start = p2, End = p3 } };
            Edge edge4 = new Edge() { Curve = new Line() { Start = p3, End = p0 } };
            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;
            //var steel = BH.Engine.Library.Query.Match("Steel", "S235", true, true) as IMaterialFragment;
            Panel panel0 = new Panel() { ExternalEdges = new List<Edge>() { edge1, edge2, edge3, edge4 }, Openings = new List<Opening>() { }, Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };


          
            Point pLine0 = new Point() { X = 0, Y = -10, Z = 0 };
            Point pLine1 = new Point() { X = 0, Y = 10, Z = 0 };

            Point pLine2 = panel0.ExternalEdges.First().Curve.ControlPoints().First();
            Point pline3 = panel0.ExternalEdges.First().Curve.ControlPoints().Last();

            //GeometricalLineLoad freeGeometricalLineLoad = new GeometricalLineLoad() { Name = "name1", Location = BH.Engine.Geometry.Create.Line(p0, p1), ForceA = BH.Engine.Geometry.Create.Vector(0, 0, 100000000000), ForceB = BH.Engine.Geometry.Create.Vector(0, 0, 100000), Loadcase = new Loadcase() { Nature = LoadNature.Wind } };
            GeometricalLineLoad freeGeometricalLineLoad = BH.Engine.Structure.Create.GeometricalLineLoad(new Line() { Start = pLine0, End = pLine1 }, new Loadcase() { Nature = LoadNature.Wind }, new Vector() { X = 0, Y = 0, Z = 100000 }, null, new List<Panel>() { panel0.ShallowClone() }, "LineloadNumber1");

            //GeometricalLineLoad nonFreeGeometricalLineLoad = new GeometricalLineLoad() { Name = "name2", Location = BH.Engine.Geometry.Create.Line(p3, p4), MomentA = BH.Engine.Geometry.Create.Vector(0, 0, 0), MomentB = BH.Engine.Geometry.Create.Vector(0, 0, 1000), Loadcase = new Loadcase() { Nature = LoadNature.Wind } };

            GeometricalLineLoad nonFreeGeometricalLineLoad = new GeometricalLineLoad() { Name = "name2", Location = BH.Engine.Geometry.Create.Line(pLine2, pline3), ForceA = BH.Engine.Geometry.Create.Vector(0, 0, 0), ForceB = BH.Engine.Geometry.Create.Vector(0, 0, 1000), Loadcase = new Loadcase() { Nature = LoadNature.Wind } };

            freeGeometricalLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(freeGeometricalLineLoad, new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.FreeLineLoad }, false);

            nonFreeGeometricalLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(nonFreeGeometricalLineLoad, new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }, false);

            //freeGeometricalLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.SetPropertyValue(freeGeometricalLineLoad, "Panels"/*, new List<Panel>() { panel0, panel1 }*/);


            //((List<BH.oM.Structure.Elements.Panel>)geometricalLineLoad.CustomData.ToList()[0].Value).Count();

            adapter.Push(new List<IObject>() { panel0.DeepClone()});
            //adapter.Push(new List<IObject>() { freeGeometricalLineLoad });
            adapter.Push(new List<IObject>() { nonFreeGeometricalLineLoad });

            //adapter.Push(new List<IObject>() { nonFreeGeometricalLineLoad });

            //FilterRequest linload = new FilterRequest() { Type = typeof(GeometricalLineLoad) };
            //var lineloads = adapter.Pull(linload).ToList();
            //var lineload0 = (GeometricalLineLoad)lineloads[0];




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

        [Test]
        public void PushPullBarLoad()
        {
            n1= new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 }};
            n2 = new Node() { Position = new Point() { X = -10, Y = 0 , Z = 0 }};
            n3 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
            n4 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 10 } };

            n5 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            n6 = new Node() { Position = new Point() { X = -10, Y = 10, Z = 0 } };
            n7 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
            n8 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 } };


            ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            bar0 = new Bar() { Start = n1, End = n2, SectionProperty = section1 };
            bar1 = new Bar() { Start = n1, End = n3, SectionProperty = section1 };
            bar2 = new Bar() { Start = n1, End = n4, SectionProperty = section1 };

            bar3 = new Bar() { Start = n5, End = n6, SectionProperty = section1 };
            bar4 = new Bar() { Start = n5, End = n7, SectionProperty = section1 };
            bar5 = new Bar() { Start = n5, End = n8, SectionProperty = section1 };



            //LoadCases
            Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };

            var barGroup = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar0 } };
            var barGroup0 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar1 } };
            var barGroup1 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar2 } };
            var barGroup2 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar3 } };
            var barGroup3 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar4 } };
            var barGroup4 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar5 } };

            var barLoad0 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup, Loadcase = loadcaseDL, Force = new Vector() { X = 1000, Y = 0, Z = 0 } };
            var barLoad1 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup0, Loadcase = loadcaseDL, Force = new Vector() { X = 0, Y = 1000, Z = 0 } };
            var barLoad2 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup1, Loadcase = loadcaseDL, Force = new Vector() { X = 0, Y = 0, Z = 1000 } };

            var barLoad3 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup2, Loadcase = loadcaseDL, Force = new Vector() { X = -1000, Y = 0, Z = 0 } };
            var barLoad4 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup3, Loadcase = loadcaseDL, Force = new Vector() { X = 0, Y = -1000, Z = 0 } };
            var barLoad5 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = barGroup4, Loadcase = loadcaseDL, Force = new Vector() { X = 0, Y = 0, Z = -1000 } };

            HashSet<ILoad> barloads = new HashSet<ILoad>() { barLoad0,barLoad1,barLoad2,barLoad3,barLoad4,barLoad5};

            adapter.Push(barloads.ToList());
            
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(BarUniformlyDistributedLoad) };
            var readBarLoad = adapter.Pull(loadFilter).ToList();

            Assert.AreEqual(barloads.Count(), readBarLoad.Count());

            BarUniformlyDistributedLoad readBarLoad0 = (BarUniformlyDistributedLoad)readBarLoad[0];
            Assert.AreEqual(barLoad0.Force.X, readBarLoad0.Force.X);
            Assert.AreEqual(barLoad0.Force.Y, readBarLoad0.Force.Y);
            Assert.AreEqual(barLoad0.Force.Z, readBarLoad0.Force.Z);


            BarUniformlyDistributedLoad readBarLoad1 = (BarUniformlyDistributedLoad)readBarLoad[1];
            Assert.AreEqual(barLoad1.Force.X, readBarLoad1.Force.X);
            Assert.AreEqual(barLoad1.Force.Y, readBarLoad1.Force.Y);
            Assert.AreEqual(barLoad1.Force.Z, readBarLoad1.Force.Z);

            BarUniformlyDistributedLoad readBarLoad2 = (BarUniformlyDistributedLoad)readBarLoad[2];
            Assert.AreEqual(barLoad2.Force.X, readBarLoad2.Force.X);
            Assert.AreEqual(barLoad2.Force.Y, readBarLoad2.Force.Y);
            Assert.AreEqual(barLoad2.Force.Z, readBarLoad2.Force.Z);

            BarUniformlyDistributedLoad readBarLoad3 = (BarUniformlyDistributedLoad)readBarLoad[3];
            Assert.AreEqual(barLoad3.Force.X, readBarLoad3.Force.X);
            Assert.AreEqual(barLoad3.Force.Y, readBarLoad3.Force.Y);
            Assert.AreEqual(barLoad3.Force.Z, readBarLoad3.Force.Z);

            BarUniformlyDistributedLoad readBarLoad4 = (BarUniformlyDistributedLoad)readBarLoad[4];
            Assert.AreEqual(barLoad4.Force.X, readBarLoad4.Force.X);
            Assert.AreEqual(barLoad4.Force.Y, readBarLoad4.Force.Y);
            Assert.AreEqual(barLoad4.Force.Z, readBarLoad4.Force.Z);

            BarUniformlyDistributedLoad readBarLoad5 = (BarUniformlyDistributedLoad)readBarLoad[5];
            Assert.AreEqual(barLoad5.Force.X, readBarLoad5.Force.X);
            Assert.AreEqual(barLoad5.Force.Y, readBarLoad5.Force.Y);
            Assert.AreEqual(barLoad5.Force.Z, readBarLoad5.Force.Z);


        }
    }
}


