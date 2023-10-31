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
using BH.oM.Structure.Elements;
using BH.oM.Base;
using BH.Engine.Structure;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullNodes

    {

        RFEM6Adapter adapter;
        Node n1;
        Node n2;
        NodeDistanceComparer comparer;

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
        public void SinglePushPullOfNode()
        {
            comparer = new NodeDistanceComparer(3);

            //Define Nodes
            n1 = new Node() { Position = new Point() { X = 1, Y = 1, Z = 0} };
            
            //Push them once
            adapter.Push(new List<Node>() { n1 });

            //Pull it
            FilterRequest nodeFilter = new FilterRequest() { Type = typeof(Node) };
            //TODO : Fix Pull issue - pull works if there are at least 2 elements to pull
            // or pull doesn't work properly
            var nodePulled = adapter.Pull(nodeFilter).ToList();
            Node np = (Node)nodePulled[0];

            //Check
            Assert.IsNotNull(np);
            Assert.IsTrue(comparer.Equals(n1, np));            
        }

        [Test]
        public void DoublePushPullOfNodes()
        {

            comparer = new NodeDistanceComparer(3);

            //Define Nodes
            n1 = new Node() { Position = new Point() { X = 1, Y = 1, Z = 0 } };
            n2 = new Node() { Position = new Point() { X = 5, Y = 5, Z = 0 } };

            //Push them twice
            adapter.Push(new List<IBHoMObject>() { n1 });
            adapter.Push(new List<IBHoMObject>() { n1,n2 });
            // TO DO: Fix the push - Currently pushing n1 twice.
            // Hence, last 2 assertions will fail

            //Pull it
            FilterRequest nodeFilter = new FilterRequest() { Type = typeof(Node) };
            var nodesPulled = adapter.Pull(nodeFilter).ToList();
            Node np1 = (Node)nodesPulled[0];
            Node np2 = (Node)nodesPulled[1];

            //Check
            Assert.IsNotNull(np1);
            Assert.IsNotNull(np2);
            Assert.IsTrue(comparer.Equals(n1, np1));
            Assert.IsTrue(comparer.Equals(n2, np2));
            Assert.AreEqual(nodesPulled.Count, 2);
        }

    }
}