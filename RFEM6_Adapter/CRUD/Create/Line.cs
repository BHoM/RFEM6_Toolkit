using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<RFEMLine> bhLines)
        {

            foreach (RFEMLine rfemLine in bhLines)
            {

                Node n0 = rfemLine.EndNode;
                Node n1 = rfemLine.StartNode;

                rfModel.line rfLine = new rfModel.line()
                {
                    no = rfemLine.GetRFEM6ID(),
                    definition_nodes = new int[] { n0.GetRFEM6ID(), n1.GetRFEM6ID() },
                    type = rfModel.line_type.TYPE_POLYLINE,
                };

                model.set_line(rfLine);
            }

            return true;
        }

    }
}
