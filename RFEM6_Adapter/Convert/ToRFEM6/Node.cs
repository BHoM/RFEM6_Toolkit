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

        public static rfModel.node ToRFEM6(this Node node)
        {

            rfModel.node rfNode = new rfModel.node()
            {
                no = node.GetRFEM6ID(),
                coordinates = new rfModel.vector_3d() { x = node.Position.X, y = node.Position.Y, z = node.Position.Z },
                coordinate_system_type = rfModel.node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                coordinate_system_typeSpecified = true,
                comment = ""
            };

            if (node.Support != null)
            {
                rfNode.support = node.Support.GetRFEM6ID();
            }   

            return rfNode;

        }

    }
}
