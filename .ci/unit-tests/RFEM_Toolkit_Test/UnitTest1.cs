using BH.Adapter.RFEM6;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;

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

        }

        [Test]
        public void Test1()
        {
            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);


            Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
            Point point1 = new Point() { X = 1, Y = 0, Z = 0 };

            Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
            Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");


            Node node0 = new Node() { Position = point0, Support = constraint0 };
            Node node1 = new Node() { Position = point1, Support = constraint1 };

            Edge edge = new Edge() { Curve = new Line() { Start = point0, End = point1 } };

            //adapter.Connect();

            //adapter.Push(new List<object> { edge });
            adapter.Push(new List<object> { edge, node0, node1 });


            //adapter.Disconnect();  

            BH.oM.Data.Requests.FilterRequest request = new FilterRequest { Type = typeof(Node) };

            //var nodes=adapter.Pull(request);




        }
    }
}