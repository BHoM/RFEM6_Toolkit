using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<RFEMLine> ReadLines(List<string> ids = null)
        {

            List<RFEMLine> lineList = new List<RFEMLine>();

            var lineNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE);
            var allRfLInes = lineNumbers.ToList().Select(n => model.get_line(n.no));

            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();

            if (ids == null)
            {
                foreach (rfModel.line rfLine in allRfLInes)
                {
                    Node node0;
                    nodes.TryGetValue(rfLine.definition_nodes[0], out node0);

                    Node node1;
                    nodes.TryGetValue(rfLine.definition_nodes[1], out node1);

                    RFEMLine l = new RFEMLine();
                    l.StartNode = node0;
                    l.EndNode = node1;

                    lineList.Add(l);
                }
            }

            return lineList;
        }

    }
}
