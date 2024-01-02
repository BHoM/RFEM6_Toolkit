/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.Adapter;
using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.Engine.Spatial;
using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RFEM_Toolkit_Test.Comparer_Tests
{
    internal class ComparerTests
    {

        EdgeComparer edgeComparer;
        RFEMHingeComparer hingeComparer;
        RFEMLineComparer lineComparer;
        RFEMLineSupportComparer lineSupportComparer;
        RFEMNodalSupportComparer nodalSupportComparer;
        RFEMPanelComparer panelComparer;
        RFEMSectionComparer sectionComparer;
        RFEMSurfacePropertyComparer surfacePropertyComparer;

        Node node0;
        Node node1;
        Node node2;
        Node node3;
        Node node4;
        Node node5;
        Node node6;
        Node node7;

        Line l0;
        Line l1;
        Line l2;
        Line l3;
        Circle c0;

        Polyline l0PolyLine;

        Edge edge0;
        Edge edge1;
        Edge edge2;
        Edge edge3;

        Constraint6DOF constraint0;
        Constraint6DOF constraint1;
        Constraint6DOF constraint2;
        Constraint6DOF constraint3;
        Constraint6DOF constraint4;
        Constraint6DOF constraint5;
        Constraint6DOF constraint6;
        Constraint6DOF constraint7;

        RFEMHinge hinge0;
        RFEMHinge hinge1;
        RFEMHinge hinge2;

        RFEMLine line0;
        RFEMLine line1;
        RFEMLine line2;
        RFEMLine line3;
        RFEMLine circle0;
        RFEMLine line0Mod;
        RFEMLine line0Polyline;

        RFEMLine line0Supported;
        RFEMLine line1Supported;
        RFEMLine line2Supported;
        RFEMLine line3Supported;

        RFEMLineSupport lineSupport0;
        RFEMLineSupport lineSupport1;

        RFEMNodalSupport nodalSupport0;
        RFEMNodalSupport nodalSupport1;

        ISectionProperty steelSection0;
        ISectionProperty steelSection1;
        ISectionProperty concreteCircleSec0;
        ISectionProperty concreteCircleSec1;
        ISectionProperty concreteRecSec0;
        ISectionProperty concreteRecSec1;

        ISectionProperty random;
        IProfile concreteCirculareProfile;
        IProfile concreteRectangleProfile;

        [SetUp]
        public void Setup()
        {

            node0 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 } };
            node1 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
            node2 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            node3 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };

            node4 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 10 } };
            node5 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 10 } };
            node6 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 10 } };
            node7 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 10 } };

            l0 = new Line() { Start = node0.Position, End = node1.Position };
            l1 = new Line() { Start = node1.Position, End = node3.Position };
            l2 = new Line() { Start = node3.Position, End = node2.Position };
            l3 = new Line() { Start = node2.Position, End = node0.Position };

            c0 = new Circle() { Centre = node0.Position, Normal = new Vector() { X = 0, Y = 0, Z = 1 }, Radius = 10 };



            l0PolyLine = new Polyline() { ControlPoints = new List<Point>() { node0.Position, node1.Position } };

            edge0 = new Edge() { Curve = l0 };
            edge1 = new Edge() { Curve = l1 };
            edge2 = new Edge() { Curve = l2 };
            edge3 = new Edge() { Curve = l3 };

            constraint0 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            constraint1 = BH.Engine.Structure.Create.PinConstraint6DOF("");
            constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true, true, true, true, true, true);
            constraint3 = BH.Engine.Structure.Create.Constraint6DOF(false, true, true, true, true, true);
            constraint4 = BH.Engine.Structure.Create.Constraint6DOF(false, false, true, true, true, true);
            constraint5 = BH.Engine.Structure.Create.Constraint6DOF(false, false, false, true, true, true);
            constraint6 = BH.Engine.Structure.Create.Constraint6DOF(false, false, false, false, true, true);
            constraint7 = BH.Engine.Structure.Create.Constraint6DOF(false, false, false, false, false, true);

            hinge0 = new RFEMHinge() { Constraint = constraint0 };
            hinge1 = new RFEMHinge() { Constraint = constraint1 };

            line0 = new RFEMLine() { Curve = l0 };
            line1 = new RFEMLine() { Curve = l1 };
            line2 = new RFEMLine() { Curve = l2 };
            line3 = new RFEMLine() { Curve = l3 };
            circle0 = new RFEMLine() { Curve = c0 };

            line0Mod = new RFEMLine() { Curve = new Line() { Start = l0.Start, End = new Point() { X = l0.End.X, Y = l0.End.Y, Z = l0.End.Y + 0.1 } } };
            line0Polyline = new RFEMLine() { Curve = l0PolyLine };

            line0 = new RFEMLine() { Curve = l0 };
            line1 = new RFEMLine() { Curve = l1 };
            line2 = new RFEMLine() { Curve = l2 };


            line0Supported = new RFEMLine() { Curve = l0, Support = constraint0 };
            line1Supported = new RFEMLine() { Curve = l1, Support = constraint1 };
            line2Supported = new RFEMLine() { Curve = l2, Support = constraint2 };

            lineSupport0 = new RFEMLineSupport() { Constraint = constraint0 };
            lineSupport1 = new RFEMLineSupport() { Constraint = constraint1 };

            nodalSupport0 = new RFEMNodalSupport() { Constraint = constraint0 };
            nodalSupport1 = new RFEMNodalSupport() { Constraint = constraint1 };

            steelSection0 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE1000M", true, true).DeepClone() as ISectionProperty;
            steelSection1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE600", true, true).DeepClone() as ISectionProperty;

            IMaterialFragment concrete0 = BH.Engine.Library.Query.Match("Concrete", "C30/37", true, true).DeepClone() as IMaterialFragment;
            IMaterialFragment concrete1 = BH.Engine.Library.Query.Match("Concrete", "C16/20", true, true).DeepClone() as IMaterialFragment;
            IMaterialFragment concrete2 = BH.Engine.Library.Query.Match("Concrete", "C20/25", true, true).DeepClone() as IMaterialFragment;

            //KCcirculareProfile=BH.Engine.Spatial.Create.CircleProfile(0.5) as IProfile;
            //rectangleProfile = BH.Engine.Spatial.Create.RectangleProfile(1,2) as IProfile;




            BH.Engine.Structure.Create.ConcreteRectangleSection(1, 2);


            concreteCirculareProfile = BH.Engine.Structure.Create.ConcreteCircularSection(0.5) as IProfile;
            concreteCircleSec0 = BH.Engine.Structure.Create.SectionPropertyFromProfile(concreteCirculareProfile, concrete0);
            concreteCircleSec1 = BH.Engine.Structure.Create.SectionPropertyFromProfile(concreteCirculareProfile, concrete1);

            concreteRectangleProfile = BH.Engine.Structure.Create.ConcreteRectangleSection(2.0,1.0) as IProfile;
            concreteRecSec0 = BH.Engine.Structure.Create.SectionPropertyFromProfile(concreteRectangleProfile, concrete0);
            concreteRecSec1 = BH.Engine.Structure.Create.SectionPropertyFromProfile(concreteRectangleProfile, concrete1);



        }

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            edgeComparer = new EdgeComparer();
            hingeComparer = new RFEMHingeComparer();
            lineComparer = new RFEMLineComparer();
            lineSupportComparer = new RFEMLineSupportComparer();
            nodalSupportComparer = new RFEMNodalSupportComparer();
            panelComparer = new RFEMPanelComparer();
            sectionComparer = new RFEMSectionComparer();
            surfacePropertyComparer = new RFEMSurfacePropertyComparer();



        }

        [Test]
        public void EdgeComparer_Test()
        {
            //Test equal Elements for equality
            Assert.IsTrue(edgeComparer.Equals(edge0, edge0));
            Assert.IsTrue(edgeComparer.Equals(edge0, edge0.DeepClone()));

            //Test unequal Elements for inequality
            Assert.IsFalse(edgeComparer.Equals(edge0, edge1));

        }

        [Test]
        public void HingeComparer_Test()
        {
            //Test equal Elements for equality
            Assert.IsTrue(hingeComparer.Equals(hinge0, hinge0));
            Assert.IsTrue(hingeComparer.Equals(hinge0, hinge0.DeepClone()));

            //Test unequal Elements for inequality
            Assert.IsFalse(hingeComparer.Equals(hinge0, hinge1));


        }

        [Test]
        public void LineComparer_Test()
        {
            ////Test equal Elements for equality////

            //Test Two Equal lines
            Assert.IsTrue(lineComparer.Equals(line0, line0));

            //Test Trow identical but not equal lines
            Assert.IsTrue(lineComparer.Equals(line0, line0.DeepClone()));

            //Test two identical linese, one supported one not
            Assert.IsTrue(lineComparer.Equals(line0, line0Supported));

            //Test two identical polylines
            Assert.IsTrue(lineComparer.Equals(line0Polyline, line0Polyline.DeepClone()));

            //test two Line and Polyline with identical geometry
            Assert.IsTrue(lineComparer.Equals(line0, line0Polyline));

            ////Test unequal Elements for inequality////

            // Two two lines with a slightly differnt geometry, endpoint differs by 0.1 in Z
            Assert.IsFalse(lineComparer.Equals(line0, line0Mod));

            //Two differnt lines with differnt geometries
            Assert.IsFalse(lineComparer.Equals(line0, line1));

            //Two completely differnt curves
            Assert.IsFalse(lineComparer.Equals(circle0, line0));



        }

        [Test]
        public void LineSupportComparer_Test()
        {



            //Compare identical line supports
            Assert.IsTrue(lineSupportComparer.Equals(lineSupport0, lineSupport0));
            Assert.IsTrue(lineSupportComparer.Equals(lineSupport0, lineSupport0.DeepClone()));

            //Compare unidentical line supports 
            Assert.IsFalse(lineSupportComparer.Equals(lineSupport0, lineSupport1));


        }

        [Test]
        public void NodalSupportComparer_Test()
        {


            //Compare identical line supports
            Assert.IsTrue(nodalSupportComparer.Equals(nodalSupport0, nodalSupport0));
            Assert.IsTrue(nodalSupportComparer.Equals(nodalSupport0, nodalSupport0.DeepClone()));

            //Compare unidentical line supports 
            Assert.IsFalse(nodalSupportComparer.Equals(nodalSupport0, nodalSupport1));


        }

        [Test]
        public void SectionComparer_Test()
        {

            //Steel
            Assert.IsTrue(sectionComparer.Equals(steelSection0, steelSection0));
            Assert.IsTrue(sectionComparer.Equals(steelSection0, steelSection0.DeepClone()));

            Assert.IsFalse(sectionComparer.Equals(steelSection0, steelSection1));

            //Concrete
            Assert.IsTrue(sectionComparer.Equals(concreteCircleSec0, concreteCircleSec0));
            Assert.IsTrue(sectionComparer.Equals(concreteCircleSec0, concreteCircleSec0.DeepClone()));

            //Assert.IsFalse(sectionComparer.Equals(concreteCircleSec0, concreteCircleSec1));
            //Assert.IsFalse(sectionComparer.Equals(concreteRecSec0, concreteRecSec1));
            
            //Concrete vs Steel
            Assert.IsFalse(sectionComparer.Equals(concreteRecSec0, concreteCircleSec0));



        }


        [Test]
        public void PanelComparer_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

        }

        [Test]
        public void SurfaceProperty_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

        }



    }
}

