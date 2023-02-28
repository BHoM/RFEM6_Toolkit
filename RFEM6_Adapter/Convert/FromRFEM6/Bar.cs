using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static Bar FromRFEM(this rfModel.member member, Node node0, Node node1, ISectionProperty section)
        {

            Bar bar = new Bar { StartNode = node0, EndNode = node1, SectionProperty = section, Name = "member nr." + member.no };
            bar.SetRFEM6ID(member.no);
            return bar;

        }

    }
}
