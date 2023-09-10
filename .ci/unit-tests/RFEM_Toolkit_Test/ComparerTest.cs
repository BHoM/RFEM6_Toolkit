//using BH.Adapter.RFEM6;
//using BH.oM.Adapters.RFEM6;
//using BH.oM.Geometry;
//using BH.oM.Structure.Constraints;
//using BH.oM.Structure.Elements;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RFEM_Toolkit_Test
//{
//    internal class ComparerTests
//    {

//        EdgeComparer edgeComparer;
//        RFEMLineComparer lineComparer ;
//        RFEMLineSupportComparer lineSupportComparer;
//        RFEMNodalSupportComparer nodalSupportComparer;
//        RFEMPanelComparer panelComparer;
//        RFEMSectionComparer sectionComparer ;    
//        RFEMSurfacePropertyComparer surfacePropertyComparer ;

//        [SetUp]
//        public void Setup()
//        {
//        }

//        [OneTimeSetUp]
//        public void InitializeRFEM6Adapter()
//        {
//             edgeComparer = new EdgeComparer();
//             lineComparer = new RFEMLineComparer();
//             lineSupportComparer = new RFEMLineSupportComparer();
//             nodalSupportComparer = new RFEMNodalSupportComparer();
//             panelComparer = new RFEMPanelComparer();
//             sectionComparer = new RFEMSectionComparer();
//             surfacePropertyComparer = new RFEMSurfacePropertyComparer();

//        }

//        [Test]
//        public void TestEdgeComparer() 
//        {

//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3



//            //Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
//            //Point point1 = new Point() { X = 1, Y = 0, Z = 0 };
//            //Point point2 = new Point() { X = 2, Y = 0, Z = 0 };

//            //Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
//            //Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");
//            //Constraint6DOF constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true, true,false,false,false,false);

//            //Node node0 = new Node() { Position = point0, Support = constraint0 };
//            //Node node1 = new Node() { Position = point1, Support = constraint1 };
//            //Node node2 = new Node() { Position = point2, Support = constraint2 };




//            //adapter.Push(new List<object> { edge });

//            Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
//            Point point1 = new Point() { X = 0, Y = 0, Z = 0 };

//            Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
//            Constraint6DOF constraint1 = BH.Engine.Structure.Create.PinConstraint6DOF("");
//            //Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");


//            Arc arc0 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point0, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };
//            Arc arc1 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point1, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };

//            Edge edge0 = new Edge() { Curve = arc0, Support = constraint0 };
//            Edge edge1 = new Edge() { Curve = arc1, Support = constraint1 };

//            RFEMLineComparer rfemLineComparer = new RFEMLineComparer(3);
//            RFEMLine rfLine0 = EdgeComparer.RFEMLineFromEdge(edge0);
//            RFEMLine rfLine1 = EdgeComparer.RFEMLineFromEdge(edge1);

//            EdgeComparer edgeComparar = new EdgeComparer();

//            Assert.IsTrue(rfemLineComparer.Equals(rfLine0, rfLine1));
//            Assert.IsTrue(edgeComparar.Equals(edge0, edge1));



//        }

//        [Test]
//        public void TestLineComparer()
//        {

//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3

//        }

//        [Test]
//        public void TestLineSupportComparer()
//        {
//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3


//        }

//        [Test]
//        public void TestNodalSupportComparer()
//        {

//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3

//        }

//        [Test]
//        public void TestPanelComparer()
//        {

//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3

//        }

//        [Test]
//        public void TestSectionComparer()
//        {

//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3

//        }

//        [Test]
//        public void TestSurfaceProtertyComparer()
//        {

//            //Define two equal objects e0 and e1

//            //Compare e0 and e1

//            //Define third object e3, unequal to e0, and compare to e0 

//            //Compare e0 to e3

//        }

//    }
//}
