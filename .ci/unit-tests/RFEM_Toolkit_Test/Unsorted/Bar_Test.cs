///*
// * This file is part of the Buildings and Habitats object Model (BHoM)
// * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
// *
// * Each contributor holds copyright over their respective contributions.
// * The project versioning (Git) records all such contribution source information.
// *                                           
// *                                                                              
// * The BHoM is free software: you can redistribute it and/or modify         
// * it under the terms of the GNU Lesser General Public License as published by  
// * the Free Software Foundation, either version 3.0 of the License, or          
// * (at your option) any later version.                                          
// *                                                                              
// * The BHoM is distributed in the hope that it will be useful,              
// * but WITHOUT ANY WARRANTY; without even the implied warranty of               
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
// * GNU Lesser General Public License for more details.                          
// *                                                                            
// * You should have received a copy of the GNU Lesser General Public License     
// * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
// */
//using BH.Adapter.RFEM6;
//using BH.Engine.Base;
//using BH.oM.Adapters.RFEM6;
//using BH.oM.Data.Requests;
//using BH.oM.Geometry;
//using BH.oM.Structure.Constraints;
//using BH.oM.Structure.Elements;
//using BH.oM.Data.Library;
//using Dlubal.WS.Rfem6.Model;
//using BH.oM.Physical.Materials;
//using BH.oM.Structure.MaterialFragments;

//namespace RFEM_Toolkit_Test
//{


//    public class Panel_Tests
//    {

//        BH.Adapter.RFEM6.RFEM6Adapter adapter;

//        [SetUp]
//        public void Setup()
//        {
//            //adapter.Wipeout();
//        }

//        [OneTimeSetUp]
//        public void InitializeRFEM6Adapter()
//        {
//            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
//        }

//        [Test]
//        public void PushPanel()
//        {


//            Point p0 = new Point() { X = 0, Y = 0, Z = 0 };
//            Point p1 = new Point() { X = 10, Y = 0, Z = 0 };
//            Point p2 = new Point() { X = 5, Y = 5, Z = 0 };
//            Point p3 = new Point() { X = 2, Y = 1, Z = 0 };
//            Point p4 = new Point() { X = 7, Y = 1, Z = 0 };
//            Point p5 = new Point() { X = 5, Y = 3, Z = 0 };



//            Line l1 = new Line() { Start = p0, End = p1 };
//            Line l2 = new Line() { Start = p1, End = p2 };
//            Line l3 = new Line() { Start = p2, End = p0 };
//            Line l4 = new Line() { Start = p3, End = p4 };
//            Line l5 = new Line() { Start = p4, End = p5 };
//            Line l6 = new Line() { Start = p5, End = p3 };



//            Polyline lNew = BH.Engine.Geometry.Compute.Join(new List<Line>() { l1, l2, l3 }).First();
//            Edge eNew = new Edge() { Curve = lNew };
//            Panel pNew = new Panel() { ExternalEdges = new List<Edge>() { eNew }, };

//            Edge e1 = new Edge() { Curve = l1 };
//            Edge e2 = new Edge() { Curve = l2 };
//            Edge e3 = new Edge() { Curve = l3 };



//            Edge e4 = new Edge() { Curve = l4 };
//            Edge e5 = new Edge() { Curve = l5 };
//            Edge e6 = new Edge() { Curve = l6 };
//            Opening o = new Opening() { Edges = new List<Edge> { e4, e5, e6 } };

//            var bhMaterial = BH.Engine.Library.Query.Match("Steel", "IPE 600", true, true).DeepClone() as IMaterialFragment;

//            Panel panel1 = new Panel() { ExternalEdges = new List<Edge> { e1, e2, e3 } };

//            Panel panel2 = new Panel() { ExternalEdges = new List<Edge> { e1, e2, e3 } };
//            Panel panel3 = new Panel() { ExternalEdges = new List<Edge> { e2, e3, e1 }, Openings = new List<Opening> { o } };





//            //adapter.Push(new List<Panel>() { panel1 });
//            adapter.Push(new List<Panel>() { pNew });

//            //Assert.IsTrue(panelComparer.Equals(panel1, panel2));
//            //Assert.IsFalse(panelComparer.Equals(panel1, panel3));


//        }





//    }
//}