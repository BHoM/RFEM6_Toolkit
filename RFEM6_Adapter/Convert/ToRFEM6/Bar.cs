using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static rfModel.member ToRFEM6(this Bar bar,int memberNo,rfModel.line rfLine, rfModel.section rfSection)
        {
            rfModel.member rfMember = new rfModel.member()
            {
                no = memberNo,
                line = rfLine.no,
                lineSpecified = true,
                section_start = rfSection.no,
                section_startSpecified = true,
                //section_end = sectionSquare.no,
                section_endSpecified = true,
                comment = "",
            };


            return rfMember;

        }

    }
}
