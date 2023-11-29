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
using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullPanels0
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


        [OneTimeSetUp]
        public void InitializeOpenings()
        {
            adapter = new RFEM6Adapter(true);
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [Test]
        public void pushTwoAdjecentPanels()
        {
      
            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true) as IMaterialFragment;
            var steel = BH.Engine.Library.Query.Match("Steel", "S235", true, true) as IMaterialFragment;

            panel1 = new Panel() { ExternalEdges = new List<Edge>() { edge4, edge5, edge6, edge7 }, Openings = new List<Opening>() { opening1 }, Property = new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.5, Material = concrete } };

            Polyline outline0 = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 0, Y = 0, Z = 0 }, new Point() { X = 10, Y = 0, Z = 0 }, new Point() { X = 10, Y = 10, Z = 0 }, new Point() { X = 0, Y = 10, Z = 0 } } };
            panel1 = BH.Engine.Structure.Create.Panel((ICurve)outline0.Close(), null, new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete }, "");

            Polyline outline1 = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 10, Y = 0, Z = 0 }, new Point() { X = 20, Y = 0, Z = 0 }, new Point() { X = 20, Y = 10, Z = 0 }, new Point() { X = 10, Y = 10, Z = 0 } } };
            panel2 = BH.Engine.Structure.Create.Panel((ICurve)outline1.Close().Flip(), null, new BH.oM.Structure.SurfaceProperties.ConstantThickness() { Thickness = 0.1, Material = concrete }, "");


            // Push panel 
            adapter.Push(new List<Panel>() { panel1, panel2 });

            //// Pull it
            //FilterRequest panelFilter = new FilterRequest() { Type = typeof(Panel) };
            //var panelPulled = adapter.Pull(panelFilter).ToList();
            //Panel pp = (Panel)panelPulled[0];

            //// Check
            //Assert.IsNotNull(pp);
            //Assert.IsTrue(comparer.Equals(pp, panel1));
        }


    }
}


