using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Geometry;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static rfModel.line ToRFEM6(this Line line, int lineNo, Node startNode, Node endNode)
        {


            rfModel.line rfLine = new rfModel.line()
            {
                no = lineNo,
                definition_nodes = new int[] { startNode.GetRFEM6ID(), endNode.GetRFEM6ID() },
                comment = "lines for beams",
                type = rfModel.line_type.TYPE_POLYLINE,
                typeSpecified = true,
            };

        
            return rfLine;

        }

    }
}
