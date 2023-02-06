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

        public static Line FromRFEM(rfModel.node startNode,rfModel.node endNode)
        {
            //Node bhNode = new Node { Position = new oM.Geometry.Point() { X = node.coordinate_1, Y = node.coordinate_2, Z = node.coordinate_3 } };
            // bhNode.SetAdapterId(typeof(RFEMId), node.no);

            Node bhNodeStart= startNode.FromRFEM();
            Node bhNodeEnd = endNode.FromRFEM();

            Line bhLine =BH.Engine.Geometry.Create.Line(bhNodeStart.Position, bhNodeEnd.Position);


           

            return bhLine;
        }

    }
}
