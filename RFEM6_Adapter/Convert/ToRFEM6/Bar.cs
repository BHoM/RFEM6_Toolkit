using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Base;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static rfModel.member ToRFEM6(this Bar bar)
        {
            rfModel.member rfMember = new rfModel.member()
            {
                no = bar.GetRFEM6ID(),
                line = bar.FindFragment<RFEMLine>().GetRFEM6ID(),
                lineSpecified = true,
                section_start = bar.SectionProperty.GetRFEM6ID(),
                section_startSpecified = true,
                section_endSpecified = true,
                comment = "",
            };


            return rfMember;

        }

    }
}
