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

            var dataset = BH.Engine.Library.Query.Datasets("Structure.Materials.MaterialsEurope.Concrete");


            var bhMaterial = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as IMaterialFragment;


            BH.oM.Data.Requests.FilterRequest filterRequest1 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Bar) };
            var hinge = adapter.Pull(filterRequest1);


        }





    }
}