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
using BH.oM.Analytical.Elements;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Loads;


namespace RFEM_Toolkit_Test.Elements
{


    public class Panel_Test
    {
        RFEM6Adapter adapter;
        RFEMPanelComparer comparer;
        Opening opening1;
        Opening opening2;
        Panel panel1;
        Panel panel2;
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

        AreaUniformlyDistributedLoad areaLoad1;
        AreaUniformlyDistributedLoad areaLoad2;
        AreaUniformlyDistributedLoad areaLoad3;
        AreaUniformlyDistributedLoad areaLoad4;



        [OneTimeSetUp]
        public void InitializeOpenings()
        {

            //Arange



            adapter = new RFEM6Adapter(true);

            comparer = new RFEMPanelComparer();

            // Panel/Opening definition
            edge1 = new Edge() { Curve = new Line() { Start = new Point() { X = 10, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 20, Z = 0 } } };
            edge2 = new Edge() { Curve = new Line() { Start = new Point() { X = 5, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 10, Z = 0 } } };
            edge3 = new Edge() { Curve = new Line() { Start = new Point() { X = 10, Y = 20, Z = 0 }, End = new Point() { X = 5, Y = 10, Z = 0 } } };
            opening1 = new Opening() { Edges = new List<Edge>() { edge1, edge2, edge3 } };
            edge4 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 5, Z = 0 }, End = new Point() { X = 12, Y = 5, Z = 0 } } };
            edge5 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 5, Z = 0 }, End = new Point() { X = 12, Y = 25, Z = 0 } } };
            edge6 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 25, Z = 0 }, End = new Point() { X = 2, Y = 25, Z = 0 } } };
            edge7 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 25, Z = 0 }, End = new Point() { X = 2, Y = 5, Z = 0 } } };
            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;
            var steel = BH.Engine.Library.Query.Match("Steel", "S235", true, true) as IMaterialFragment;
            panel1 = new Panel() { ExternalEdges = new List<Edge>() { edge4, edge5, edge6, edge7 }, Openings = new List<Opening>() { opening1 }, Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };

            panelGroup1 = new List<IAreaElement>() { panel1 };



            // Definition of UniformlyDistributedAreaLoad
            areaLoad1 = BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, Vector.XAxis * 100, panelGroup1, LoadAxis.Global, false);



        }

    

        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [Test]
        public void SinglePushPullOfOpening()
        {



            // Pull it
            FilterRequest panelFilter = new FilterRequest() { Type = typeof(Panel) };
            var panelPulled = adapter.Pull(panelFilter).ToList();
            Panel pp = (Panel)panelPulled[0];

            // Check
            Assert.IsNotNull(pp);
            Assert.IsTrue(comparer.Equals(pp, panel1));
        }

        [Test]
        public void DoublePushPullOfOpenings()
        {
            comparer = new RFEMPanelComparer();
            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;
            var steel = BH.Engine.Library.Query.Match("Steel", "S235", true, true) as IMaterialFragment;
            // Create Panel 1
            edge1 = new Edge() { Curve = new Line() { Start = new Point() { X = 10, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 20, Z = 0 } } };
            edge2 = new Edge() { Curve = new Line() { Start = new Point() { X = 5, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 10, Z = 0 } } };
            edge3 = new Edge() { Curve = new Line() { Start = new Point() { X = 10, Y = 20, Z = 0 }, End = new Point() { X = 5, Y = 10, Z = 0 } } };
            opening1 = new Opening() { Edges = new List<Edge>() { edge1, edge2, edge3 } };
            edge4 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 5, Z = 0 }, End = new Point() { X = 12, Y = 5, Z = 0 } } };
            edge5 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 5, Z = 0 }, End = new Point() { X = 12, Y = 25, Z = 0 } } };
            edge6 = new Edge() { Curve = new Line() { Start = new Point() { X = 12, Y = 25, Z = 0 }, End = new Point() { X = 2, Y = 25, Z = 0 } } };
            edge7 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 25, Z = 0 }, End = new Point() { X = 2, Y = 5, Z = 0 } } };
            panel1 = new Panel() { ExternalEdges = new List<Edge>() { edge4, edge5, edge6, edge7 }, Openings = new List<Opening>() { opening1 }, Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete } };
            // Create Panel 2
            edge8 = new Edge() { Curve = new Circle() { Centre = new Point() { X = 0, Y = 0, Z = 5 }, Radius = 1 } };
            opening2 = new Opening() { Edges = new List<Edge>() { edge4 } };
            edge9 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = 2, Z = 5 }, End = new Point() { X = -2, Y = 2, Z = 5 } } };
            edge10 = new Edge() { Curve = new Line() { Start = new Point() { X = -2, Y = 2, Z = 5 }, End = new Point() { X = -2, Y = -2, Z = 5 } } };
            edge11 = new Edge() { Curve = new Line() { Start = new Point() { X = -2, Y = -2, Z = 5 }, End = new Point() { X = 2, Y = -2, Z = 5 } } };
            edge12 = new Edge() { Curve = new Line() { Start = new Point() { X = 2, Y = -2, Z = 5 }, End = new Point() { X = 2, Y = 2, Z = 5 } } };
            panel2 = new Panel() { ExternalEdges = new List<Edge>() { edge9, edge10, edge11, edge12 }, Openings = new List<Opening>() { opening2 }, Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = steel } };
            adapter.Push(new List<Panel>() { panel1 });

            adapter.Push(new List<Panel>() { panel1, panel2 });


            // Pull it
            FilterRequest panelFilter = new FilterRequest() { Type = typeof(Panel) };
            var panelsPulled = adapter.Pull(panelFilter).ToList();
            Panel pp1 = (Panel)panelsPulled[0];
            Panel pp2 = (Panel)panelsPulled[1];

            // Check
            Assert.IsNotNull(pp1);
            Assert.IsNotNull(pp2);
            Assert.IsTrue(comparer.Equals(pp1, panel1));
            Assert.IsTrue(comparer.Equals(pp2, panel2));
            Assert.AreEqual(panelsPulled.Count, 2);
        }
    }


}


