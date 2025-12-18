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
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using Dlubal.WS.Rfem6.Model;

namespace RFEM_Toolkit_Test.Elements
{


    public class AreaUniformlyDistributedLoad_test
    {
        RFEM6Adapter adapter;
        RFEMPanelComparer comparer;
        Opening opening1;
        Opening opening2;
        Panel panel1;
        Panel panel2;
        Panel panel3;

        Edge edge1;
        Edge edge2;
        Edge edge3;
        Edge edge4;
        Edge edge5;
        Edge edge6;
        Edge edge7;
        Edge edge8;
        Edge edge9;
        Edge edge10;
        Edge edge11;
        Edge edge12;


        Loadcase loadcase;

        List<IAreaElement> panelGroup1;
        List<IAreaElement> panelGroup2;
        List<IAreaElement> panelGroup3;


        List<AreaUniformlyDistributedLoad> areaLoadList;
        List<AreaUniformlyDistributedLoad> reverseAreaLoadList;


        [OneTimeSetUp]
        public void InitializeOpenings()
        {
            adapter = new RFEM6Adapter(active:true);

            comparer = new RFEMPanelComparer();

            // Create Panel 1
            edge1 = new Edge() { Curve = new Line() { Start = new Point() { X = 10, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 20, Z = 0 } } };
            edge2 = new Edge() { Curve = new Line() { Start = new Point() { X = 5, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 10, Z = 0 } } };
            edge3 = new Edge() { Curve = new Line() { Start = new Point() { X = 10, Y = 20, Z = 0 }, End = new Point() { X = 5, Y = 10, Z = 0 } } };
            //opening1 = new Opening() { Edges = new List<Edge>() { edge1, edge2, edge3 } };
            //var opening2 = opening1.DeepClone().Translate(Vector.XAxis * 20);
            //var opening3 = opening2.DeepClone().Translate(Vector.XAxis * 20);

            edge4 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 5, Z = 0 }, End = new Point() { X = 12, Y = 5, Z = 5 } } };
            edge5 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 5, Z = 5 }, End = new Point() { X = 12, Y = 25, Z = 5 } } };
            edge6 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 25, Z = 5 }, End = new Point() { X = 2, Y = 25, Z = 0 } } };
            edge7 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 25, Z = 0 }, End = new Point() { X = 2, Y = 5, Z = 0 } } };

            var outlineEdges1 = new List<Edge>() { edge4, edge5, edge6, edge7 };
            var outlineEdges2 = (outlineEdges1.Select(x => x.DeepClone().Curve.Translate(Vector.XAxis * 20)).ToList()).Select(s => new Edge() { Curve = (Line)s }).ToList();
            var outlineEdges3 = (outlineEdges2.Select(x => x.DeepClone().Curve.Translate(Vector.XAxis * 20)).ToList()).Select(s => new Edge() { Curve = (Line)s }).ToList();



            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;

            panel1 = new Panel() { ExternalEdges = outlineEdges1, /*Openings = new List<Opening>() { opening1 }*//*,*/ Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };
            panel2 = new Panel() { ExternalEdges = outlineEdges2, /*Openings = new List<Opening>() { opening2 },*/ Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };
            panel3 = new Panel() { ExternalEdges = outlineEdges3, /*Openings = new List<Opening>() { opening2 },*/ Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };

            loadcase = new BH.oM.Structure.Loads.Loadcase() { Name = "Loadcase", Nature = LoadNature.Dead, Number=1 };


            panelGroup1 = new List<IAreaElement>() { panel1 };
            panelGroup2 = new List<IAreaElement>() { panel2 };
            panelGroup3 = new List<IAreaElement>() { panel3 };



            // Definition of UniformlyDistributedAreaLoad
            areaLoadList = new List<AreaUniformlyDistributedLoad>() { };
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis * 100, panelGroup1, LoadAxis.Global, false));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis * 100, panelGroup1, LoadAxis.Global, true));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis * 100, panelGroup1, LoadAxis.Local, false));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.YAxis * 100, panelGroup2, LoadAxis.Global, false));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.YAxis * 100, panelGroup2, LoadAxis.Global, true));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.YAxis * 100, panelGroup2, LoadAxis.Local, false));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.ZAxis * 100, panelGroup3, LoadAxis.Global, false));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.ZAxis * 100, panelGroup3, LoadAxis.Global, true));
            areaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.ZAxis * 100, panelGroup3, LoadAxis.Local, false));


            reverseAreaLoadList = new List<AreaUniformlyDistributedLoad>() { };
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis.Reverse() * 100, panelGroup1, LoadAxis.Global, false));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis.Reverse() * 100, panelGroup1, LoadAxis.Global, true));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis.Reverse() * 100, panelGroup1, LoadAxis.Local, false));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.YAxis.Reverse() * 100, panelGroup2, LoadAxis.Global, false));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.YAxis.Reverse() * 100, panelGroup2, LoadAxis.Global, true));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.YAxis.Reverse() * 100, panelGroup2, LoadAxis.Local, false));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.ZAxis.Reverse() * 100, panelGroup3, LoadAxis.Global, false));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.ZAxis.Reverse() * 100, panelGroup3, LoadAxis.Global, true));
            reverseAreaLoadList.Add(BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.ZAxis.Reverse() * 100, panelGroup3, LoadAxis.Local, false));

           
        }

        [SetUp]
        public void SetUp()
        {
            //adapter.Push(new List<Panel>() { panel1, panel2, panel3 });
        }


        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [Test]
        public void PushOfAreaUniformlyDistributedLoad()
        {

            //Act

            // Push
            adapter.Push(this.areaLoadList);

            // Pull
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(AreaUniformlyDistributedLoad) };
            List<AreaUniformlyDistributedLoad> areaLoadList = adapter.Pull(loadFilter).ToList().Select(p => (AreaUniformlyDistributedLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.areaLoadList[i].Projected.Equals(areaLoadList[i].Projected));
            
            i = 1;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.areaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 2;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));

            i = 3;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.areaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 4;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.areaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 5;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));

            i = 6;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.areaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 7;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.areaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 8;
            Assert.True(this.areaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.areaLoadList[i].Axis.Equals(areaLoadList[i].Axis));


        }

        [Test]
        public void PushOfReversedAreaUniformlyDistributedLoad()
        {

            //Act

            // Push
            adapter.Push(this.reverseAreaLoadList);

            // Pull
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(AreaUniformlyDistributedLoad) };
            List<AreaUniformlyDistributedLoad> areaLoadList = adapter.Pull(loadFilter).ToList().Select(p => (AreaUniformlyDistributedLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.reverseAreaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 1;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.reverseAreaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 2;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));

            i = 3;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.reverseAreaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 4;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.reverseAreaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 5;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));

            i = 6;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.reverseAreaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 7;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
            Assert.True(this.reverseAreaLoadList[i].Projected.Equals(areaLoadList[i].Projected));

            i = 8;
            Assert.True(this.reverseAreaLoadList[i].Pressure.Equals(areaLoadList[i].Pressure));
            Assert.True(this.reverseAreaLoadList[i].Axis.Equals(areaLoadList[i].Axis));
        }

        [Test]
        public void PullUniformlyDistributedLoad()
        {

            //Act

            // Push

            // Pull
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(AreaUniformlyDistributedLoad) };
            List<AreaUniformlyDistributedLoad> areaLoadList = adapter.Pull(loadFilter).ToList().Select(p => (AreaUniformlyDistributedLoad)p).ToList();
            var a = areaLoadList.First();
        }
    }


}

