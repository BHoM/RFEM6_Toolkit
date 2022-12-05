using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    internal partial class RFEM6Adapter
    {

        private List<Node> ReadNodes(List<string> ids = null)
        {

            List<Node> nodeList = new List<Node>();

            //if (ids == null)
            //{
            //    foreach (rfModel.node rfNode in rfModel.getno)
            //    {
            //        nodeList.Add(rfNode.FromRFEM());
            //    }
            //}
            //else
            //{
            //    foreach (string id in ids)
            //    {
            //        nodeList.Add(modelData.GetNode(Int32.Parse(id), rf.ItemAt.AtNo).GetData().FromRFEM());
            //    }
            //}


            return nodeList;
        }

    }
}
