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
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using System.Security.Policy;

namespace RFEM_Toolkit_Test.Elements
{


    public class PointLoad_test
    {
        RFEM6Adapter adapter;
        Node node0;
        Node node1;
        Node node2;
        Node node3;
        Node node4;
        Node node5;

        BH.oM.Structure.Loads.Loadcase loadcase;

        BH.oM.Base.BHoMGroup<Node> nodeGroup0;
        BH.oM.Base.BHoMGroup<Node> nodeGroup1;
        BH.oM.Base.BHoMGroup<Node> nodeGroup2;
        BH.oM.Base.BHoMGroup<Node> nodeGroup3;
        BH.oM.Base.BHoMGroup<Node> nodeGroup4;
        BH.oM.Base.BHoMGroup<Node> nodeGroup5;

        PointLoad pointLoadInclined0;


        PointLoad pointLoad0;
        PointLoad pointLoad1;
        PointLoad pointLoad2;
        PointLoad pointLoad3;
        PointLoad pointLoad4;
        PointLoad pointLoad5;

        PointLoad pointLoad6;
        PointLoad pointLoad7;
        PointLoad pointLoad8;
        PointLoad pointLoad9;
        PointLoad pointLoad10;
        PointLoad pointLoad11;



        [OneTimeSetUp]
        public void Initialize()
        {
            adapter = new RFEM6Adapter(true);
            node0 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 } };
            node1 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
            node2 = new Node() { Position = new Point() { X = 20, Y = 0, Z = 0 } };
            node3 = new Node() { Position = new Point() { X = 30, Y = 0, Z = 0 } };
            node4 = new Node() { Position = new Point() { X = 40, Y = 0, Z = 0 } };
            node5 = new Node() { Position = new Point() { X = 50, Y = 0, Z = 0 } };

            loadcase = new BH.oM.Structure.Loads.Loadcase() { Name = "Loadcase", Nature = LoadNature.Dead,Number=1 };

            var nodeGroup0 = new BH.oM.Base.BHoMGroup<Node>() { Elements = new List<Node> { node0 } };

            pointLoadInclined0 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, new Vector() { X = 1, Y = 1, Z = 0 },null,LoadAxis.Global,"");

            pointLoad0 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, Vector.XAxis, null);
            pointLoad1 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, Vector.YAxis, null);
            pointLoad2 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, Vector.ZAxis, null);
            pointLoad3 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, Vector.XAxis);
            pointLoad4 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, Vector.YAxis);
            pointLoad5 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, Vector.ZAxis);

            //pointLoad6 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, BH.Engine.Geometry.Modify.Reverse(Vector.XAxis), null);
            //pointLoad7 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, BH.Engine.Geometry.Modify.Reverse(Vector.YAxis), null);
            //pointLoad8 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, BH.Engine.Geometry.Modify.Reverse(Vector.ZAxis), null);
            //pointLoad9 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, BH.Engine.Geometry.Modify.Reverse(Vector.XAxis));
            //pointLoad10 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, BH.Engine.Geometry.Modify.Reverse(Vector.YAxis));
            //pointLoad11 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, BH.Engine.Geometry.Modify.Reverse(Vector.ZAxis));

            pointLoad6 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, new Vector() { X = -1, Y = 0, Z = 0 }, null);
            pointLoad7 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, new Vector() { X = 0, Y = -1, Z = 0 }, null);
            pointLoad8 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, new Vector() { X = 0, Y = 0, Z = -1 }, null);
            pointLoad9 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, new Vector() { X = -1, Y = 0, Z = 0 });
            pointLoad10 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, new Vector() { X = 0, Y = -1, Z = 0 });
            pointLoad11 = BH.Engine.Structure.Create.PointLoad(loadcase, nodeGroup0, null, new Vector() { X = 0, Y = 0, Z = -1 });

            BH.Engine.Geometry.Modify.Reverse(Vector.XAxis);

        }

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }


        [Test]
        public void PushPullOrientationTest_AxisParallel()
        {
            adapter.Push(new List<IBHoMObject>() { pointLoad0, pointLoad1, pointLoad2, pointLoad3, pointLoad4, pointLoad5 });
            FilterRequest pointLoadFilter = new FilterRequest() { Type = typeof(PointLoad) };
            List<PointLoad> pointLoad = adapter.Pull(pointLoadFilter).ToList().Select(p => (PointLoad)p).ToList();

            Assert.IsTrue((pointLoad[0]).Force == (pointLoad0.Force));
            Assert.IsTrue((pointLoad[0]).Moment == (pointLoad0.Moment));

            Assert.IsTrue((pointLoad[1]).Force == (pointLoad1.Force));
            Assert.IsTrue((pointLoad[1]).Moment == (pointLoad1.Moment));

            Assert.IsTrue((pointLoad[2]).Force == (pointLoad2.Force));
            Assert.IsTrue((pointLoad[2]).Moment == (pointLoad2.Moment));

            Assert.IsTrue((pointLoad[3]).Force == (pointLoad3.Force));
            Assert.IsTrue((pointLoad[3]).Moment == (pointLoad3.Moment));

            Assert.IsTrue((pointLoad[4]).Force == (pointLoad4.Force));
            Assert.IsTrue((pointLoad[4]).Moment == (pointLoad4.Moment));

            Assert.IsTrue((pointLoad[5]).Force == (pointLoad5.Force));
            Assert.IsTrue((pointLoad[5]).Moment == (pointLoad5.Moment));


        }


        [Test]
        public void PushPullOrientationTest_AntiAxisParallel()
        {
            adapter.Push(new List<IBHoMObject>() { pointLoad6, pointLoad7, pointLoad8, pointLoad9, pointLoad10, pointLoad11 });
            FilterRequest pointLoadFilter = new FilterRequest() { Type = typeof(PointLoad) };
            List<PointLoad> pointLoad = adapter.Pull(pointLoadFilter).ToList().Select(p => (PointLoad)p).ToList();

            Assert.IsTrue((pointLoad[0]).Force == (pointLoad6.Force));
            Assert.IsTrue((pointLoad[0]).Moment == (pointLoad6.Moment));


            Assert.IsTrue((pointLoad[1]).Force == (pointLoad7.Force));
            Assert.IsTrue((pointLoad[1]).Moment == (pointLoad7.Moment));


            Assert.IsTrue((pointLoad[2]).Force == (pointLoad8.Force));
            Assert.IsTrue((pointLoad[2]).Moment == (pointLoad8.Moment));

            Assert.IsTrue((pointLoad[3]).Force == (pointLoad9.Force));
            Assert.IsTrue((pointLoad[3]).Moment == (pointLoad9.Moment));

            Assert.IsTrue((pointLoad[4]).Force == (pointLoad10.Force));
            Assert.IsTrue((pointLoad[4]).Moment == (pointLoad10.Moment));

            Assert.IsTrue((pointLoad[5]).Force == (pointLoad11.Force));
            Assert.IsTrue((pointLoad[5]).Moment == (pointLoad11.Moment));


        }

    }


}

