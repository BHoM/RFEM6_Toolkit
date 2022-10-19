using System;
using System.Collections.Generic;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool Create(Node bhNode)
        {
            rfModel.node n1 = new rfModel.node()
            {
                no = 1,
                coordinates = new rfModel.vector_3d() { x = bhNode.Position.X, y = bhNode.Position.Y, z = bhNode.Position.Z},
                coordinate_system_type = rfModel.node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                coordinate_system_typeSpecified = true,
                comment = "concrete part"
            };

            return true;
        }
    }
}
