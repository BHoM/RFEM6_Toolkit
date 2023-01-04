using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Node> ReadNodes(List<string> ids = null)
        {

            List<Node> nodeList = new List<Node>();

            var nodeNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODE);
            var allNodes = nodeNumbers.ToList().Select(n => model.get_node(n.no));


            if (ids == null)
            {
                foreach (rfModel.node rfNode in allNodes)
                {
                    nodeList.Add(rfNode.FromRFEM());
                }
            }
            else
            {
                //foreach (string id in ids)
                //{
                //    nodeList.Add(modelData.GetNode(Int32.Parse(id), rf.ItemAt.AtNo).GetData().FromRFEM());
                //}
            }


            return nodeList;
        }

    }
}
