using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Data.Library;
using BH.Engine.Library;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

namespace RFEM_Toolkit_Test
{


    public class BarRelease_Test
    {

        BH.Adapter.RFEM6.RFEM6Adapter adapter;

        [SetUp]
        public void Setup()
        {
            //adapter.Wipeout();
        }

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
        }

        [Test]
        public void PushPanel_Test()
        {

            Node n0 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 } };
            Node n1 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
            ISteelSection steelSection = BH.Engine.Library.Query.Match("EU_SteelSections", "HE1000M", true, true).DeepClone() as ISteelSection;
            Constraint6DOF constraint0 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            Constraint6DOF constraint1 = BH.Engine.Structure.Create.PinConstraint6DOF("");

            Node n3 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            Node n4 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
            //ISteelSection steelSection = BH.Engine.Library.Query.Match("EU_SteelSections", "HE1000M", true, true).DeepClone() as ISteelSection;
            //Constraint6DOF constraint0 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            Constraint6DOF constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true,true,false,true,true,true);


            BarRelease release0 = new BarRelease() { StartRelease=constraint0, EndRelease=constraint1};
            BarRelease release1 = new BarRelease() { StartRelease = constraint0, EndRelease = constraint2 };


            Bar bar0 = new Bar() {StartNode=n0, EndNode=n1,SectionProperty= steelSection,Release=release0};
            Bar bar1 = new Bar() { StartNode = n3, EndNode = n4, SectionProperty = steelSection, Release = release1 };


            adapter.Push(new List<Bar>() { bar0,bar1 });

            //BH.oM.Data.Requests.FilterRequest filterRequest1 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(RFEMHinge) };
            //var hinges = adapter.Pull(filterRequest1);
            //var hinge=hinges.First();

            //BH.oM.Data.Requests.FilterRequest filterRequest2 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Bar) };
            //var bars = adapter.Pull(filterRequest2);
            //var bar = hinges.First();

        }





    }
}