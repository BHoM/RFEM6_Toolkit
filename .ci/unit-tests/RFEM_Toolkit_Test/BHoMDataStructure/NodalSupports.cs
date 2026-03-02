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
using BH.Engine.Structure;
using BH.oM.Base;
using BH.Engine.Structure;
using BH.oM.Structure.Constraints;
using BH.Engine.Base;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullNodalSupports

    {

        RFEM6Adapter adapter;
        Node n0;
        Node n1;
        Node n2;
        Node n3;
        Node n4;
        Constraint6DOF constraint6dof0;
        Constraint6DOF constraint6dof1;
        Constraint6DOF constraint6dof2;
        NodeDistanceComparer comparer;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(active: true);
        }

        [TearDown]
        public void TearDown()
        {
            //adapter.Wipeout();
        }

        [Test]
        public void PushPullConstraintNodes()
        {

            // Arrange
            n0 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 15 } };

            constraint6dof0 = new Constraint6DOF()
            {
                Name = "",
                TranslationX = DOFType.Spring,
                TranslationY = DOFType.Spring,
                TranslationZ = DOFType.Spring,
                RotationX = DOFType.Spring,
                RotationY = DOFType.Spring,
                RotationZ = DOFType.Spring,
                TranslationalStiffnessX = 1000,
                TranslationalStiffnessY = 2000,
                TranslationalStiffnessZ = 3000,
                RotationalStiffnessX = 1000,
                RotationalStiffnessY = 2000,
                RotationalStiffnessZ = 3000,
            };
            n0.Support = constraint6dof0;

            //Define Nodes
            n1 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };

            constraint6dof0 = new Constraint6DOF()
            {
                Name = "",
                TranslationX = DOFType.Spring,
                TranslationY = DOFType.Spring,
                TranslationZ = DOFType.Spring,
                RotationX = DOFType.Spring,
                RotationY = DOFType.Spring,
                RotationZ = DOFType.Spring,
                TranslationalStiffnessX = 1000,
                TranslationalStiffnessY = 2000,
                TranslationalStiffnessZ = 3000,
                RotationalStiffnessX = 1000,
                RotationalStiffnessY = 2000,
                RotationalStiffnessZ = 3000,
            };

            n1.Support = constraint6dof0;

            n2 = new Node() { Position = new Point() { X = 15, Y = 15, Z = 0 } };

            constraint6dof2 = new Constraint6DOF()
            {
                Name = "",
                TranslationX = DOFType.Spring,
                TranslationY = DOFType.Spring,
                TranslationZ = DOFType.Spring,
                RotationX = DOFType.Spring,
                RotationY = DOFType.Spring,
                RotationZ = DOFType.Spring,
                TranslationalStiffnessX = 1000,
                TranslationalStiffnessY = 2000,
                TranslationalStiffnessZ = 3000,
                RotationalStiffnessX = 4000,
                RotationalStiffnessY = 5000,
                RotationalStiffnessZ = 6000,
            };

            n2.Support = constraint6dof2;

            n3 = new Node() { Position = new Point() { X = 15, Y = 15, Z = 15 } };

            constraint6dof2 = new Constraint6DOF()
            {
                Name = "",
                TranslationX = DOFType.Spring,
                TranslationY = DOFType.Spring,
                TranslationZ = DOFType.Spring,
                TranslationalStiffnessX = 1000,
                TranslationalStiffnessY = 2000,
                TranslationalStiffnessZ = 3000,
            };

            n3.Support = constraint6dof2;


            n4 = new Node() { Position = new Point() { X = 20, Y = 20, Z = 20 } };

            var pushNodeList = new List<Node>() { n0, n1, n2, n3, n4 };
            //Act
            adapter.Push(pushNodeList);
            FilterRequest nodesFilter = new FilterRequest() { Type = typeof(Node) };


            List<Node> constrainedNodes = (adapter.Pull(nodesFilter).Select(n => (Node)n).ToList());

            //Assert

            //Null Check
            Assert.IsNotNull(constrainedNodes);

            //Check for pulled node size
            Assert.That(constrainedNodes.Count(), Is.EqualTo(pushNodeList.Count()));

            //
            //Assert.
            var extractedConstraints = constrainedNodes.Select(c => c.Support).Where(s => s != null).ToHashSet(new BH.Engine.Structure.Constraint6DOFComparer());
            Assert.IsTrue(extractedConstraints.Contains(n0.Support));
            Assert.IsTrue(extractedConstraints.Contains(n1.Support));
            Assert.IsTrue(extractedConstraints.Contains(n2.Support));
            Assert.IsTrue(extractedConstraints.Contains(n3.Support));
            Assert.That(extractedConstraints.Count(), Is.EqualTo(3));

        }

        [Test]
        public void FirstPushWithContraintThenPushWithoutConstraint()
        {
            // Arrange
            n0 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
            constraint6dof0 = new Constraint6DOF()
            {
                //Name = "",
                TranslationX = DOFType.Spring,
                TranslationY = DOFType.Spring,
                TranslationZ = DOFType.Spring,
                RotationX = DOFType.Spring,
                RotationY = DOFType.Spring,
                RotationZ = DOFType.Spring,
                TranslationalStiffnessX = 1000,
                TranslationalStiffnessY = 2000,
                TranslationalStiffnessZ = 3000,
                RotationalStiffnessX = 1000,
                RotationalStiffnessY = 2000,
                RotationalStiffnessZ = 3000,
            };
            n0.Support = constraint6dof0;

            var pushNodeList0 = new List<Node>() { n0 };


            n1 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
            constraint6dof1 = new Constraint6DOF()
            {
                //Name = "",
                TranslationX = DOFType.Spring,
                TranslationY = DOFType.Spring,
                TranslationZ = DOFType.Spring,
                RotationX = DOFType.Spring,
                RotationY = DOFType.Spring,
                RotationZ = DOFType.Spring,
                TranslationalStiffnessX = 1000,
                TranslationalStiffnessY = 2000,
                TranslationalStiffnessZ = 3000,
                RotationalStiffnessX = 1000,
                RotationalStiffnessY = 2000,
                RotationalStiffnessZ = 3000,
            };
            n1.Support = constraint6dof0;

            var pushNodeList1 = new List<Node>() { n1 };

            //Act
            adapter.Push(pushNodeList0);
            adapter.Push(pushNodeList1);
            FilterRequest nodesFilter = new FilterRequest() { Type = typeof(Node) };

            HashSet<Node> constrainedNodes = (adapter.Pull(nodesFilter).Select(n => (Node)n).ToHashSet(new RFEMNodalComparer()));


            //Assert
            Assert.That(constrainedNodes.Count(), Is.EqualTo(1));
            Assert.IsTrue(constrainedNodes.Contains(n0));

        }

        //[Test]
        //public void FirstPushWithContraintThenPushWithoutConstraint()
        //{
        //    // Arrange
        //    n0 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
        //    n1 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
        //    constraint6dof0 = new Constraint6DOF()
        //    {
        //        Name = "",
        //        TranslationX = DOFType.Spring,
        //        TranslationY = DOFType.Spring,
        //        TranslationZ = DOFType.Spring,
        //        RotationX = DOFType.Spring,
        //        RotationY = DOFType.Spring,
        //        RotationZ = DOFType.Spring,
        //        TranslationalStiffnessX = 1000,
        //        TranslationalStiffnessY = 2000,
        //        TranslationalStiffnessZ = 3000,
        //        RotationalStiffnessX = 1000,
        //        RotationalStiffnessY = 2000,
        //        RotationalStiffnessZ = 3000,
        //    };

        //    n0.Support = constraint6dof0;

        //    var pushNodeList0 = new List<Node>() { n0 };
        //    var pushNodeList1 = new List<Node>() { n1 };
        //    //Act
        //    adapter.Push(pushNodeList0);
        //    adapter.Push(pushNodeList1);
        //    FilterRequest nodesFilter = new FilterRequest() { Type = typeof(Node) };

        //}

        //[Test]
        //public void FirstPushWithoutContraintThenPushConstraint()
        //{
        //    // Arrange
        //    n0 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
        //    n1 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };
        //    constraint6dof1 = new Constraint6DOF()
        //    {
        //        Name = "",
        //        TranslationX = DOFType.Spring,
        //        TranslationY = DOFType.Spring,
        //        TranslationZ = DOFType.Spring,
        //        RotationX = DOFType.Spring,
        //        RotationY = DOFType.Spring,
        //        RotationZ = DOFType.Spring,
        //        TranslationalStiffnessX = 1000,
        //        TranslationalStiffnessY = 2000,
        //        TranslationalStiffnessZ = 3000,
        //        RotationalStiffnessX = 1000,
        //        RotationalStiffnessY = 2000,
        //        RotationalStiffnessZ = 3000,
        //    };

        //    n1.Support = constraint6dof1;

        //    var pushNodeList0 = new List<Node>() { n0 };
        //    var pushNodeList1 = new List<Node>() { n1 };
        //    //Act
        //    adapter.Push(pushNodeList0);
        //    adapter.Push(pushNodeList1);
        //    FilterRequest nodesFilter = new FilterRequest() { Type = typeof(Node) };

        //}

    }
    }


