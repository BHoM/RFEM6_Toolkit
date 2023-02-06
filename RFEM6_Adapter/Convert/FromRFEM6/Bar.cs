using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static Bar FromRFEM(this rfModel.member member, rfModel.node node0, rfModel.node node1, rfModel.material rfMaterial, rfModel.section rfSection)
        {

   

            Node bhMode0 = node0.FromRFEM();
            Node bhMode1 = node1.FromRFEM();

            BH.oM.Geometry.Line bhLine = BH.Engine.Geometry.Create.Line(bhMode0.Position, bhMode1.Position);
            //BH.Engine.Structure.Create.Bar()
            BH.oM.Structure.SectionProperties.ISectionProperty bhSection = rfSection.FromRFEM(rfMaterial);

           return BH.Engine.Structure.Create.Bar(bhLine, bhSection,0,null,BarFEAType.Flexural,"member nr."+member.no);

        }

    }
}
