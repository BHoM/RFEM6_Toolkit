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
using BH.oM.Structure.Elements;
using BH.oM.Base;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullEdge

    {

        RFEM6Adapter adapter;
        Circle circle1;
        Line line1;        
        EdgeComparer comparer;
        Edge edge1;
        Edge edge2;


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
        public void SinglePushPullOfEdges()
        {

            comparer = new EdgeComparer();

            //Define Edge
            line1 = new Line() { Start = new Point() { X = 10, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 20, Z = 0 } };
            edge1 = new Edge() {Curve = line1};

            //Push it once
            adapter.Push(new List<IBHoMObject>() { edge1 });

            //Pull it
            FilterRequest edgeFilter = new FilterRequest() { Type = typeof(Edge) };
            var edgePulled = adapter.Pull(edgeFilter).ToList();
            Edge ep = (Edge)edgePulled[0];

            //Check
            Assert.IsNotNull(ep);
            Assert.True(comparer.Equals(edge1, ep));
        }

        [Test]
        public void DoublePushPullOfEdges()
        {

            comparer = new EdgeComparer();

            //Define Edges
            line1 = new Line() { Start = new Point() { X = 10, Y = 10, Z = 0 }, End = new Point() { X = 10, Y = 20, Z = 0 } };
            edge1 = new Edge() { Curve = line1 };
            circle1 = new Circle() { Centre = new Point() { X = 0, Y = 0, Z = 0 }, Radius = 5 };
            edge2 = new Edge() { Curve = circle1 };

            //Push only one edge and then both edges
            adapter.Push(new List<IBHoMObject>() { edge1 });
            adapter.Push(new List<IBHoMObject>() { edge1, edge2 });

            //Pull edges
            FilterRequest edgeFilter = new FilterRequest() { Type = typeof(Edge) };
            var edgesPulled = adapter.Pull(edgeFilter).ToList();
            Edge ep1 = (Edge)edgesPulled[0];
            Edge ep2 = (Edge)edgesPulled[1];

            //Check
            Assert.IsNotNull(ep1);
            Assert.IsNotNull(ep2);
            Assert.True(comparer.Equals(edge1, ep1));
            Assert.True(comparer.Equals(edge2 , ep2));
            Assert.AreEqual(edgesPulled.Count, 2);
        }

    }
}

