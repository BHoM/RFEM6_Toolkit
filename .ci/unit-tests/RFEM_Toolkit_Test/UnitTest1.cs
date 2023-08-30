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

            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
        }

        [Test]
        public void PushPullTestForNodalSupports()
        {


            Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
            Point point1 = new Point() { X = 1, Y = 0, Z = 0 };
            Point point2 = new Point() { X = 2, Y = 0, Z = 0 };

            Constraint6DOF constraint0 = BH.Engine.Structure.Create.PinConstraint6DOF("");
            Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            Constraint6DOF constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true, true,false,false,false,false);
            //Constraint4DOF constraint2 = BH.Engine.Structure.Create.BarReleaseFixFix("");



            Node node0 = new Node() { Position = point0, Support = constraint0 };
            Node node1 = new Node() { Position = point1, Support = constraint1 };
            Node node2 = new Node() { Position = point2, Support = constraint2 };


            //BH.Engine.Structure.Create.Node( (BH.Engine.Geometry.Create.CartesianCoordinateSystem(point0,Vector.XAxis,Vector.YAxis)), support=constraint0);

            ////Edge edge = new Edge() { Curve = new Line() { Start = point0, End = point1 }, Support = constraint1 };
            //Edge edge = new Edge() { Curve = new Line() { Start = point0, End = point1 }, Support=constraint0 };




            //adapter.Connect();

            //adapter.Push(new List<object> { edge });
            adapter.Push(new List<object> { node0, node1, node2 });
            //adapter.Push(new List<object> { node2 });

            //adapter.Push(new List<object> { node0, node1, node2 });
            //adapter.Disconnect();  

            //BH.oM.Data.Requests.FilterRequest request = new FilterRequest { Type = typeof(Node) };

            //var nodes=adapter.Pull(request);

        }


        //[Test]
        //public void PushPUllLineSupport()
        //{

        //    //adapter.Disconnect();  
        //    BH.oM.Data.Requests.FilterRequest request = new FilterRequest { Type = typeof(Edge) };


        //    var edges = adapter.Pull(request);



        //}

    }
}