using BH.Adapter.RFEM6;
using BH.Engine.Base;
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
            adapter.Wipeout();
        }

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
        }

        [Test]
        public void SequentialEdgePush()
        {
           

            Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
            Point point1 = new Point() { X = 1, Y = 0, Z = 0 };
            Point point2 = new Point() { X = 2, Y = 0, Z = 0 };

            Constraint6DOF constraint0 = BH.Engine.Structure.Create.Constraint6DOF(true, true, false, false, false, false);
            Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            Constraint6DOF constraint2 = BH.Engine.Structure.Create.PinConstraint6DOF("");

            Node node0 = new Node() { Position = point0, Support = constraint0 };
            Node node1 = new Node() { Position = point1, Support = constraint1 };
            Node node2 = new Node() { Position = point2, Support = constraint2 };

            Arc arc0 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point0, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };
            Arc arc1 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point1, Vector.XAxis, Vector.YAxis), Radius =10, StartAngle = 0, EndAngle = 180 };

            //Edge edge0_push = new Edge() { Curve = arc0, Support = constraint0 };
            Edge edge1 = new Edge() { Curve = arc1, Support = constraint0 };

            //First Pushs

            //adapter.Push(new List<object> { node0 });
            adapter.Push(new List<object> { edge1 });
            //adapter.Push(new List<object> { edge1 });


            BH.oM.Data.Requests.FilterRequest filterRequest1 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Node) };
            IEnumerable<Object> nodes = adapter.Pull(filterRequest1);
            Assert.IsTrue(nodes.Count() == 3);
            
            //Edge edge0_pull = (Edge)edges.First();


            //Seciond Pushs
            //adapter.Push(new List<object> { edge0_pull });
            //adapter.Push(new List<object> { edge1 });


            //EdgeComparer edgeComparer = new EdgeComparer();
            //bool bo = edgeComparer.Equals(edge0_push, edge0_pull);
            //Assert.IsTrue(edgeComparer.Equals(edge0_push, edge0_pull));



            //adapter.Push(new List<object> { edge0 });
            //adapter.Push(new List<object> { edge1 });



            //Assert.IsTrue((new EdgeComparer()).Equals(edge0,edge));

            //adapter.Push(new List<object> { edge });

            //adapter.Push(new List<object> { edge0 });


            //BH.oM.Data.Requests.FilterRequest filterRequest2 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(RFEMLine) };
            //var rfemLines = adapter.Pull(filterRequest2);

            //var edgePulled = ((Edge)edges.First());
            ////var rfemLinePulled = ((RFEMLine)rfemLines.First());

            //EdgeComparer edgeComparer = new EdgeComparer();
            //Assert.IsTrue(edgeComparer.Equals(edge0, edgePulled));

            //RFEMLineComparer rfemLineComparer = new RFEMLineComparer(3);
            //Assert.IsTrue(rfemLineComparer.Equals(rfemLinePulled, EdgeComparer.RFEMLineFromEdge(edge0)));

        }

        // Keep for reference when debugingg the stackoverflow exception form the bASE FullCRUD.
        // This requires to set the m_AdapterSettings.OnlyUpdateChangedObjects to True, and serialize the object throwing the Stackoverflow exception..
        // The exception is generally thowing from HashString or Hash methods, both called from the FullCRUD.
        public void TestHashStringWithSerializer(){
            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
            var jsonWriter = new Newtonsoft.Json.JsonTextReader(new System.IO.StreamReader(@"C:\BHoMGit\RFEM6_Toolkit\RFEMNodalSupports.json"));
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);
            var objs = serializer.Deserialize<IEnumerable<RFEMLineSupport>>(jsonWriter);

            foreach (var OBJ in objs)
            {
                OBJ.Hash();
            }
        }
            

        //[Test]
        //[Description("This test requires to set m_AdapterSettings.OnlyUpdateChangedObjects to True." +
        //    "It will throw a StackOverflow exception in the base FullCRUD when using the HashComparer.")] 
        //public void testHashString()
        //{

        //    //adapter.WipeOut();

        //    Point point0 = new Point() { X = 0, Y = 0, Z = 0 };
        //    Point point1 = new Point() { X = 1, Y = 0, Z = 0 };
        //    Point point2 = new Point() { X = 2, Y = 0, Z = 0 };

        //    Constraint6DOF constraint0 = BH.Engine.Structure.Create.Constraint6DOF(true, true, false, false, false, false);
        //    Constraint6DOF constraint1 = BH.Engine.Structure.Create.FixConstraint6DOF("");
        //    Constraint6DOF constraint2 = BH.Engine.Structure.Create.PinConstraint6DOF("");

        //    Node node0 = new Node() { Position = point0, Support = constraint0 };
        //    Node node1 = new Node() { Position = point1, Support = constraint1 };
        //    Node node2 = new Node() { Position = point2, Support = constraint2 };

        //    Arc arc0 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point0, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };
        //    Arc arc1 = new Arc() { CoordinateSystem = BH.Engine.Geometry.Create.CartesianCoordinateSystem(point1, Vector.XAxis, Vector.YAxis), Radius = 5, StartAngle = 0, EndAngle = 180 };

        //    Edge edge0 = new Edge() { Curve = arc0, Support = constraint0 };
        //    Edge edge1 = new Edge() { Curve = arc1, Support = constraint1 };


        //    adapter.Push(new List<object> { edge0 });

        //    BH.oM.Data.Requests.FilterRequest filterRequest1 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Edge) };
        //    var edges = adapter.Pull(filterRequest1);
        //    var edge = edges.First();

        //    adapter.Push(new List<object> { node0 });
        //    adapter.Push(new List<object> { node0 });

        //    //adapter.Push(new List<object> { edge0 });


        //    //BH.oM.Data.Requests.FilterRequest filterRequest2 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(RFEMLine) };
        //    //var rfemLines = adapter.Pull(filterRequest2);

        //    //var edgePulled = ((Edge)edges.First());
        //    ////var rfemLinePulled = ((RFEMLine)rfemLines.First());

        //    //EdgeComparer edgeComparer = new EdgeComparer();
        //    //Assert.IsTrue(edgeComparer.Equals(edge0, edgePulled));

        //    //RFEMLineComparer rfemLineComparer = new RFEMLineComparer(3);
        //    //Assert.IsTrue(rfemLineComparer.Equals(rfemLinePulled, EdgeComparer.RFEMLineFromEdge(edge0)));

        //}


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