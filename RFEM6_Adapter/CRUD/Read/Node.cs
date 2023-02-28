using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Node> ReadNodes(List<string> ids = null)
        {

            List<Node> nodeList = new List<Node>();

            var nodeNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODE);
            var allRfNodes = nodeNumbers.ToList().Select(n => model.get_node(n.no));
           
           Dictionary<int, Constraint6DOF> supports = this.GetCachedOrReadAsDictionary<int, Constraint6DOF>();

            if (ids == null)
            {
                foreach (rfModel.node rfNode in allRfNodes)
                {
                    Node node = rfNode.FromRFEM();

                    int supportId = rfNode.support;

                    Constraint6DOF support;
                    if (supports.TryGetValue(supportId, out support))
                        node.Support = support;

                    nodeList.Add(node);
                }
            }

            return nodeList;
        }

    }
}
