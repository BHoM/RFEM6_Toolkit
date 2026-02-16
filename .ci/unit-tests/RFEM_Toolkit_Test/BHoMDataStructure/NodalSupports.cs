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
using BH.oM.Structure.Elements;
using BH.oM.Base;
using BH.Engine.Structure;
using BH.oM.Structure.Constraints;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullNodalSupports

    {

        RFEM6Adapter adapter;
        Node n1;
        Node n2;
        NodeDistanceComparer comparer;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(active:true);
        }

        [TearDown]
        public void TearDown()
        {
            //adapter.Wipeout();
        }

        [Test]
        public void PullConstraint6DO()
        {
            //comparer = new NodeDistanceComparer(3);

            ////Define Nodes
            //n1 = new Node() { Position = new Point() { X = 1, Y = 1, Z = 0} };
            
            ////Push them once
            //adapter.Push(new List<Node>() { n1 });

            //Pull it
            FilterRequest constraint6DOFFilter = new FilterRequest() { Type = typeof(Constraint6DOF) };
          

            var nodePulled = adapter.Pull(constraint6DOFFilter).ToList();
            Constraint6DOF np = (Constraint6DOF)nodePulled[0];

            //Check
            Assert.IsNotNull(np);
            //Assert.IsTrue(comparer.Equals(n1, np));            
        }
        [Test]
        public void PullNodeWithConstraint6DOF()
        {
            //comparer = new NodeDistanceComparer(3);

            ////Define Nodes
            //n1 = new Node() { Position = new Point() { X = 1, Y = 1, Z = 0} };

            ////Push them once
            //adapter.Push(new List<Node>() { n1 });

            //Pull it
            FilterRequest constraint6DOFFilter = new FilterRequest() { Type = typeof(Node) };


            var nodePulled = adapter.Pull(constraint6DOFFilter).ToList();
            //Constraint6DOF np = (Constraint6DOF)nodePulled[0];

            //Check
            Assert.IsNotNull(nodePulled);
            //Assert.IsTrue(comparer.Equals(n1, np));            
        }


    }
}


