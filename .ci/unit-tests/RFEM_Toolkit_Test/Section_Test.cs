//using BH.Adapter.RFEM6;
//using BH.Engine.Base;
//using BH.oM.Adapters.RFEM6;
//using BH.oM.Data.Requests;
//using BH.oM.Geometry;
//using BH.oM.Structure.Constraints;
//using BH.oM.Structure.Elements;
//using BH.oM.Data.Library;
//using Dlubal.WS.Rfem6.Model;
//using BH.oM.Physical.Materials;
//using BH.oM.Structure.MaterialFragments;
//using BH.Engine.Structure;
//using BH.oM.Structure;
//using BH.oM.Structure.SectionProperties;
//using BH.oM.Base;
//using BH.oM.Spatial.ShapeProfiles;

//namespace RFEM_Toolkit_Test
//{


//    public class Section_Tests
//    {

//        BH.Adapter.RFEM6.RFEM6Adapter adapter;

//        [SetUp]
//        public void Setup()
//        {
//            //adapter.Wipeout();
//        }

//        [OneTimeSetUp]
//        public void InitializeRFEM6Adapter()
//        {
//            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
//        }

//        [Test]
//        public void PushSection()
//        {


//            //var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as IMaterialFragment;
//            //var steel = BH.Engine.Library.Query.Match("Steel", "S235", true, true).DeepClone() as IMaterialFragment;
//            //SteelSection section =(SteelSection)BH.Engine.Library.Query.Match("EU_SteelSections", "HE1000M", true, true).DeepClone();
//            //adapter.Push(new List<SteelSection>() { section });

        
//            BH.oM.Data.Requests.FilterRequest filter = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(ISectionProperty) };
//            var pulledSections = adapter.Pull(filter);
//            var pulledSection = pulledSections.First();


//        }





//    }
//}