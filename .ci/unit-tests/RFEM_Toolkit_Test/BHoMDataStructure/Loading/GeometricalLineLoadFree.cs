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
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using Dlubal.WS.Rfem6.Model;

namespace RFEM_Toolkit_Test.Elements
{


    public class GeometricalLineLoadFree_test
    {
        RFEM6Adapter adapter;
        RFEMPanelComparer comparer;
        Panel panel1;
        Panel panel2;
        Panel panel3;

        Line impactLine;

        Edge edge1;
        Edge edge2;
        Edge edge3;
        Edge edge4;
        Edge edge5;
        Edge edge6;
        Edge edge7;
        Edge edge8;
        Edge edge9;


        Loadcase loadcase;

        BHoMGroup<IAreaElement> panelGroup1;
        BHoMGroup<IAreaElement> panelGroup2;
        BHoMGroup<IAreaElement> panelGroup3;


        List<GeometricalLineLoad> forceList1;
        List<GeometricalLineLoad> momentList1;

        GeometricalLineLoad load_force_0_h;
        GeometricalLineLoad load_force_0_i;
        GeometricalLineLoad load_force_0_h_proj;
        GeometricalLineLoad load_force_0_i_proj;
        GeometricalLineLoad load_force_0_h_loc;
        GeometricalLineLoad load_force_0_i_loc;

        GeometricalLineLoad load_moment_0_h;
        GeometricalLineLoad load_moment_0_i;
        GeometricalLineLoad load_moment_0_h_proj;
        GeometricalLineLoad load_moment_0_i_proj;
        GeometricalLineLoad load_moment_0_h_loc;
        GeometricalLineLoad load_moment_0_i_loc;




        [OneTimeSetUp]
        public void InitializeOpenings()
        {
            adapter = new RFEM6Adapter(true);

            comparer = new RFEMPanelComparer();

            // Create Panels
            edge1 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 5, Z = 0 }, End = new Point() { X = 12, Y = 5, Z = 5 } } };
            edge2 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 5, Z = 5 }, End = new Point() { X = 12, Y = 25, Z = 5 } } };
            edge3 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 25, Z = 5 }, End = new Point() { X = 2, Y = 25, Z = 0 } } };
            edge4 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 25, Z = 0 }, End = new Point() { X = 2, Y = 5, Z = 0 } } };

            var outlineEdges1 = new List<Edge>() { edge1, edge2, edge3, edge4 };
            var outlineEdges2 = (outlineEdges1.Select(x => x.DeepClone().Curve.Translate(Vector.XAxis * 20)).ToList()).Select(s => new Edge() { Curve = (Line)s }).ToList();
            var outlineEdges3 = (outlineEdges2.Select(x => x.DeepClone().Curve.Translate(Vector.XAxis * 20)).ToList()).Select(s => new Edge() { Curve = (Line)s }).ToList();

            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;

            panel1 = new Panel() { ExternalEdges = outlineEdges1, /*Openings = new List<Opening>() { opening1 }*//*,*/ Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };
            panel2 = new Panel() { ExternalEdges = outlineEdges2, /*Openings = new List<Opening>() { opening2 },*/ Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };
            panel3 = new Panel() { ExternalEdges = outlineEdges3, /*Openings = new List<Opening>() { opening2 },*/ Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };

            // Load Definition
            panelGroup1 = new BHoMGroup<IAreaElement>() { Elements = new List<IAreaElement>() { panel1 } };
            panelGroup2 = new BHoMGroup<IAreaElement>() { Elements = new List<IAreaElement>() { panel2 } };
            panelGroup3 = new BHoMGroup<IAreaElement>() { Elements = new List<IAreaElement>() { panel3 } };

            loadcase = new BH.oM.Structure.Loads.Loadcase() { Name = "Loadcase", Nature = LoadNature.Dead, Number=1 };




            // Free Line Loads

            impactLine = new Line() { Start = new Point() { X = 0, Y = 10, Z = 0 }, End = new Point() { X = 80, Y = 20, Z = 0 } };

            // Forces
            //Global Non Projected
            load_force_0_h = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * 100, Objects = panelGroup1 };

            load_force_0_i = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * -100, Objects = panelGroup1 };

            //Local Non Projected
            load_force_0_h_loc = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * 100, Axis = LoadAxis.Local, Objects = panelGroup2 };

            load_force_0_i_loc = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * -100, Axis = LoadAxis.Local, Objects = panelGroup2 };


            // Global Projected
            load_force_0_h_proj = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * 100, Projected = true, Objects = panelGroup3 };

            load_force_0_i_proj = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * -100, Projected = true, Objects = panelGroup3 };

            forceList1 = new List<GeometricalLineLoad>() { load_force_0_h, load_force_0_i, load_force_0_h_loc, load_force_0_i_loc, load_force_0_h_proj, load_force_0_i_proj };



            // Moments
            //Global Non Projected
            load_moment_0_h = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * 100 };

            load_moment_0_i = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * -100 };

            //Local Non Projected
            load_moment_0_h_loc = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * 100, Axis = LoadAxis.Local };

            load_moment_0_i_loc = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * -100, Axis = LoadAxis.Local };


            // Global Projected
            load_moment_0_h_proj = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, MomentA = Vector.XAxis * 2000, MomentB = Vector.XAxis * 2000, Projected = true };

            load_moment_0_i_proj = new GeometricalLineLoad() { Location = impactLine, Loadcase = loadcase, MomentA = Vector.XAxis * 2000, MomentB = Vector.XAxis * -2000, Projected = true };

            momentList1 = new List<GeometricalLineLoad>() { load_moment_0_h, load_moment_0_i, load_moment_0_h_loc, load_moment_0_i_loc, load_moment_0_h_proj, load_moment_0_i_proj };

        }

        //[SetUp]
        //public void SetUp()
        //{
        //    adapter.Push(new List<Panel>() { panel1, panel2, panel3 });
        //}


        [TearDown]
        public void TearDown()
        {
            //adapter.Wipeout();
        }

        [Test]
        public void PushOrNonFreeGeometricalLineLoad_Forces()
        {
            adapter.Push(new List<Panel>() { panel1,panel2,panel3 });

            //Act

            // Push
            adapter.Push(this.forceList1);

            // Pull
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(GeometricalLineLoad) };
            List<GeometricalLineLoad> lineLoadList = adapter.Pull(loadFilter).ToList().Select(p => (GeometricalLineLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(this.forceList1[i].ForceA.Equals(lineLoadList[i].ForceA));
            Assert.True(this.forceList1[i].ForceB.Equals(lineLoadList[i].ForceB));
            Assert.True(this.forceList1[i].Axis.Equals(lineLoadList[i].Axis));
            Assert.True(this.forceList1[i].Projected.Equals(lineLoadList[i].Projected));

            i = 1;
            Assert.True(this.forceList1[i].ForceA.Equals(lineLoadList[i].ForceA));
            Assert.True(this.forceList1[i].ForceB.Equals(lineLoadList[i].ForceB));
            Assert.True(this.forceList1[i].Axis.Equals(lineLoadList[i].Axis));
            Assert.True(this.forceList1[i].Projected.Equals(lineLoadList[i].Projected));

            i = 2;
            Assert.True(this.forceList1[i].ForceA.Equals(lineLoadList[i].ForceA));
            Assert.True(this.forceList1[i].ForceB.Equals(lineLoadList[i].ForceB));
            Assert.True(this.forceList1[i].Axis.Equals(lineLoadList[i].Axis));
            Assert.True(this.forceList1[i].Projected.Equals(lineLoadList[i].Projected));

            i = 3;
            Assert.True(this.forceList1[i].ForceA.Equals(lineLoadList[i].ForceA));
            Assert.True(this.forceList1[i].ForceB.Equals(lineLoadList[i].ForceB));
            Assert.True(this.forceList1[i].Axis.Equals(lineLoadList[i].Axis));
            Assert.True(this.forceList1[i].Projected.Equals(lineLoadList[i].Projected));

            i = 4;
            Assert.True(this.forceList1[i].ForceA.Equals(lineLoadList[i].ForceA));
            Assert.True(this.forceList1[i].ForceB.Equals(lineLoadList[i].ForceB));
            Assert.True(this.forceList1[i].Axis.Equals(lineLoadList[i].Axis));
            Assert.True(this.forceList1[i].Projected.Equals(lineLoadList[i].Projected));

            i = 5;
            Assert.True(this.forceList1[i].ForceA.Equals(lineLoadList[i].ForceA));
            Assert.True(this.forceList1[i].ForceB.Equals(lineLoadList[i].ForceB));
            Assert.True(this.forceList1[i].Axis.Equals(lineLoadList[i].Axis));
            Assert.True(this.forceList1[i].Projected.Equals(lineLoadList[i].Projected));


        }


    }


}

