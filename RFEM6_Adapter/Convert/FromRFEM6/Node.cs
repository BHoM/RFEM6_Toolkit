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

        public static Node FromRFEM(this rfModel.node node)
        {
            Node bhNode = new Node { Position = new oM.Geometry.Point() { X = node.coordinate_1, Y = node.coordinate_2, Z = node.coordinate_3 } };
           // bhNode.SetAdapterId(typeof(RFEMId), node.no);


            bhNode.Name = "Node Nr. " + node.no;

            return bhNode;
        }

    }
}
