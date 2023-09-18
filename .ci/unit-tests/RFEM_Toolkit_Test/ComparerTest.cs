using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFEM_Toolkit_Test
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
        RFEMLine line0Mod;

        RFEMLine line1;
        RFEMLine line2;
        RFEMLine line3;
        RFEMLine line0Supported;
        RFEMLine line1Supported;
        RFEMLine line2Supported;
        RFEMLine line3Supported;


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
            line0Mod = new RFEMLine() { Curve = new Line(){ Start = l0.Start, End=new Point() {X=l0.End.X,Y= l0.End.Y, Z= l0.End.Y + 0.1} } };
            line1 = new RFEMLine() { Curve = l1 };
            line2 = new RFEMLine() { Curve = l2 };
            line3 = new RFEMLine() { Curve = l3 };


            line0 = new RFEMLine() { Curve = l0 };
            line1 = new RFEMLine() { Curve = l1 };
            line2 = new RFEMLine() { Curve = l2 };


            line0Supported = new RFEMLine() { Curve = l0, Support = constraint0 };
            line1Supported = new RFEMLine() { Curve = l1, Support = constraint1 };
            line2Supported = new RFEMLine() { Curve = l2, Support = constraint2 };


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
            //Test equal Elements for equality
            Assert.IsTrue(lineComparer.Equals(line0, line0));
            Assert.IsTrue(lineComparer.Equals(line0, line0.DeepClone()));
            Assert.IsTrue(lineComparer.Equals(line0, line0Supported));

            //Test unequal Elements for inequality
            Assert.IsFalse(lineComparer.Equals(line0, line0Mod));
            Assert.IsFalse(lineComparer.Equals(line0, line1));




        }

        [Test]
        public void LineSupportComparer_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

        }

        [Test]
        public void NodalSupportComparer_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

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
        public void SirfaceProperty_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

        }

        [Test]
        public void SectionComparer_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

        }

        [Test]
        public void SurfaceComparer_Test()
        {

            //Define two equal objects e0 and e1

            //Compare e0 and e1

            //Define third object e3, unequal to e0, and compare to e0 

            //Compare e0 to e3

        }
    }
}
