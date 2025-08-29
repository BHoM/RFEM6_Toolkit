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
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using Dlubal.WS.Rfem6.Model;

namespace RFEM_Toolkit_Test.Elements
{


    public class GeometricalLineLoadNonFree_test
    {
        RFEM6Adapter adapter;
        RFEMPanelComparer comparer;
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


        Loadcase loadcase;

        List<IAreaElement> panelGroup1;
        List<IAreaElement> panelGroup2;
        List<IAreaElement> panelGroup3;


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
            panelGroup1 = new List<IAreaElement>() { panel1 };
            panelGroup2 = new List<IAreaElement>() { panel2 };
            panelGroup3 = new List<IAreaElement>() { panel3 };

            loadcase = new BH.oM.Structure.Loads.Loadcase() { Name = "Loadcase", Nature = LoadNature.Dead, Number = 1 };


            // NON-Free Line Loads

            // Forces
            //Global Non Projected
            load_force_0_h = new GeometricalLineLoad() { Location = (Line)panel1.ExternalEdges[0].Curve, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * 100 };
            load_force_0_h = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_force_0_h, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            load_force_0_i = new GeometricalLineLoad() { Location = (Line)panel1.ExternalEdges[2].Curve, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * -100 };
            load_force_0_i = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_force_0_i, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            //Local Non Projected
            load_force_0_h_loc = new GeometricalLineLoad() { Location = (Line)panel2.ExternalEdges[0].Curve, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * 100, Axis = LoadAxis.Local };
            load_force_0_h_loc = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_force_0_h_loc, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            load_force_0_i_loc = new GeometricalLineLoad() { Location = (Line)panel2.ExternalEdges[2].Curve, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * -100, Axis = LoadAxis.Local };
            load_force_0_i_loc = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_force_0_i_loc, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));


            // Global Projected
            load_force_0_h_proj = new GeometricalLineLoad() { Location = (Line)panel3.ExternalEdges[0].Curve, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * 100, Projected = true };
            load_force_0_h_proj = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_force_0_h_proj, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            load_force_0_i_proj = new GeometricalLineLoad() { Location = (Line)panel3.ExternalEdges[2].Curve, Loadcase = loadcase, ForceA = Vector.XAxis * 100, ForceB = Vector.XAxis * -100, Projected = true };
            load_force_0_i_proj = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_force_0_i_proj, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            forceList1 = new List<GeometricalLineLoad>() { load_force_0_h, load_force_0_i, load_force_0_h_loc, load_force_0_i_loc, load_force_0_h_proj, load_force_0_i_proj };



            // Moments
            //Global Non Projected
            load_moment_0_h = new GeometricalLineLoad() { Location = (Line)panel1.ExternalEdges[0].Curve, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * 100 };
            load_moment_0_h = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_moment_0_h, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            load_moment_0_i = new GeometricalLineLoad() { Location = (Line)panel1.ExternalEdges[2].Curve, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * -100 };
            load_moment_0_i = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_moment_0_i, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            //Local Non Projected
            load_moment_0_h_loc = new GeometricalLineLoad() { Location = (Line)panel2.ExternalEdges[0].Curve, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * 100, Axis = LoadAxis.Local };
            load_moment_0_h_loc = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_moment_0_h_loc, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            load_moment_0_i_loc = new GeometricalLineLoad() { Location = (Line)panel2.ExternalEdges[2].Curve, Loadcase = loadcase, MomentA = Vector.XAxis * 100, MomentB = Vector.XAxis * -100, Axis = LoadAxis.Local };
            load_moment_0_i_loc = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_moment_0_i_loc, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));


            // Global Projected
            load_moment_0_h_proj = new GeometricalLineLoad() { Location = (Line)panel3.ExternalEdges[0].Curve, Loadcase = loadcase, MomentA = Vector.XAxis * 2000, MomentB = Vector.XAxis * 2000, Projected = true };
            load_moment_0_h_proj = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_moment_0_h_proj, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            load_moment_0_i_proj = new GeometricalLineLoad() { Location = (Line)panel3.ExternalEdges[2].Curve, Loadcase = loadcase, MomentA = Vector.XAxis * 2000, MomentB = Vector.XAxis * -2000, Projected = true };
            load_moment_0_i_proj = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(load_moment_0_i_proj, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            momentList1 = new List<GeometricalLineLoad>() { load_moment_0_h, load_moment_0_i, load_moment_0_h_loc, load_moment_0_i_loc, load_moment_0_h_proj, load_moment_0_i_proj };

        }

        [SetUp]
        public void SetUp()
        {
            adapter.Push(new List<Panel>() { panel1, panel2, panel3 });
        }


        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [Test]
        public void PushOfNonFreeGeometricalLineLoad_Forces()
        {

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


        [Test]
        public void PushOFNonFreeGeometricalLineLoad_Moments()
        {

            // Act

            // Push
            adapter.Push(this.momentList1);

            // Pull
            FilterRequest loadFilter = new FilterRequest() { Type = typeof(GeometricalLineLoad) };
            List<GeometricalLineLoad> lineLoadList = adapter.Pull(loadFilter).ToList().Select(p => (GeometricalLineLoad)p).ToList();

            //Assert
            int i = 0;
            Assert.True(this.momentList1[i].MomentA.Equals(lineLoadList[i].MomentA));
            Assert.True(this.momentList1[i].MomentB.Equals(lineLoadList[i].MomentB));
            Assert.True(this.momentList1[i].Axis.Equals(lineLoadList[i].Axis));

            i = 1;
            Assert.True(this.momentList1[i].MomentA.Equals(lineLoadList[i].MomentA));
            Assert.True(this.momentList1[i].MomentB.Equals(lineLoadList[i].MomentB));
            Assert.True(this.momentList1[i].Axis.Equals(lineLoadList[i].Axis));

            i = 2;
            Assert.True(this.momentList1[i].MomentA.Equals(lineLoadList[i].MomentA));
            Assert.True(this.momentList1[i].MomentB.Equals(lineLoadList[i].MomentB));
            Assert.True(this.momentList1[i].Axis.Equals(lineLoadList[i].Axis));

            i = 3;
            Assert.True(this.momentList1[i].MomentA.Equals(lineLoadList[i].MomentA));
            Assert.True(this.momentList1[i].MomentB.Equals(lineLoadList[i].MomentB));
            Assert.True(this.momentList1[i].Axis.Equals(lineLoadList[i].Axis));

            i = 4;
            Assert.True(this.momentList1[i].MomentA.Equals(lineLoadList[i].MomentA));
            Assert.True(this.momentList1[i].MomentB.Equals(lineLoadList[i].MomentB));
            Assert.True(this.momentList1[i].Axis.Equals(lineLoadList[i].Axis));

            i = 5;
            Assert.True(this.momentList1[i].MomentA.Equals(lineLoadList[i].MomentA));
            Assert.True(this.momentList1[i].MomentB.Equals(lineLoadList[i].MomentB));
            Assert.True(this.momentList1[i].Axis.Equals(lineLoadList[i].Axis));

        }


    }


}

