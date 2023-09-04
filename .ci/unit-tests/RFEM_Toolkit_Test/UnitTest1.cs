using BH.Adapter.RFEM6;
using BH.oM.Adapters.RFEM6;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using Dlubal.WS.Rfem6.Model;

namespace RFEM_Toolkit_Test
{


    public class Tests
    {

        BH.Adapter.RFEM6.RFEM6Adapter adapter;

        [SetUp]
        public void Setup()
        {
        }

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {

            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
        }
      
        [Test]
        public void SequentialEdgePush()
        {
            adapter.WipeOut();

            Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
            Point point1 = new Point() { X = 1, Y = 0, Z = 0 };
            Point point2 = new Point() { X = 2, Y = 0, Z = 0 };

            Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
            Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            Constraint6DOF constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true, true, false, false, false, false);

            Node node0 = new Node() { Position = point0, Support = constraint0 };
            Node node1 = new Node() { Position = point1, Support = constraint1 };
            Node node2 = new Node() { Position = point2, Support = constraint2 };

            Arc arc0 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point0, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };
            Arc arc1 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point1, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };

            Edge edge0 = new Edge() { Curve = arc0, Support = constraint0 };
            Edge edge1 = new Edge() { Curve = arc1, Support = constraint1 };



            adapter.Push(new List<object> { edge0 });
            //BH.oM.Data.Requests.FilterRequest filterRequest1 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Edge) };
            //var edges = adapter.Pull(filterRequest1);
            //BH.oM.Data.Requests.FilterRequest filterRequest2 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(RFEMLine) };
            //var rfemLines = adapter.Pull(filterRequest2);

            //var edgePulled = ((Edge)edges.First());
            ////var rfemLinePulled = ((RFEMLine)rfemLines.First());

            //EdgeComparer edgeComparer = new EdgeComparer();
            //Assert.IsTrue(edgeComparer.Equals(edge0, edgePulled));

            //RFEMLineComparer rfemLineComparer = new RFEMLineComparer(3);
            //Assert.IsTrue(rfemLineComparer.Equals(rfemLinePulled, EdgeComparer.RFEMLineFromEdge(edge0)));

        }


        //[Test]
        //public void PushPUllLineSupport()
        //{


        //    //Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
        //    //Point point1 = new Point() { X = 1, Y = 0, Z = 0 };
        //    //Point point2 = new Point() { X = 2, Y = 0, Z = 0 };

        //    //Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
        //    //Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");
        //    //Constraint6DOF constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true, true,false,false,false,false);

        //    //Node node0 = new Node() { Position = point0, Support = constraint0 };
        //    //Node node1 = new Node() { Position = point1, Support = constraint1 };
        //    //Node node2 = new Node() { Position = point2, Support = constraint2 };




        //    //adapter.Push(new List<object> { edge });

        //    Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
        //    Point point1 = new Point() { X = 0, Y = 0, Z = 0 };

        //    Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
        //    Constraint6DOF constraint1 = BH.Engine.Structure.Create.PinConstraint6DOF("");

        //    Arc arc0 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point0, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };
        //    Arc arc1 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point1, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };

        //    Edge edge0 = new Edge() { Curve = arc0, Support = constraint0 };
        //    Edge edge1 = new Edge() { Curve = arc1, Support = constraint1 };

        //    RFEMLineComparer rfemLineComparer = new RFEMLineComparer(3);
        //    RFEMLine rfLine0 = EdgeComparer.RFEMLineFromEdge(edge0);
        //    RFEMLine rfLine1 = EdgeComparer.RFEMLineFromEdge(edge1);

        //    EdgeComparer edgeComparar = new EdgeComparer();

        //    Assert.IsTrue(rfemLineComparer.Equals(rfLine0, rfLine1));
        //    Assert.IsTrue(edgeComparar.Equals(edge0, edge1));




        //}

    }
}