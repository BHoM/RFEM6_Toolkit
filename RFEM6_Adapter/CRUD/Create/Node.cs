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
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {

            //NOTE:A geometric object has, in general, a parent_no = 0. The parent_no parameter becomes significant for example with loads.
            int nodeId = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_NODE, 0);


            int counter = 0;


            for (int i = 0; i < bhNodes.Count(); i++)
            {
                Node bhNode = bhNodes.ToList()[i];
             
                if (modelDoesContainNode(bhNode)==null) {

                    rfModel.node rfNode = new rfModel.node()
                    {
                        no = nodeId + i -counter,
                        coordinates = new rfModel.vector_3d() { x = bhNode.Position.X, y = bhNode.Position.Y, z = bhNode.Position.Z },
                        coordinate_system_type = rfModel.node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                        coordinate_system_typeSpecified = true,
                        comment = "concrete part"
                    };
                    model.set_node(rfNode);
                }
                else
                {
                     counter = counter +1;

                }

            }


            return true;
        }

        private rfModel.node modelDoesContainNode(Node bhNode)
        {

            var numbers=model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODE);
            var foundNode=numbers.ToList().Select(n => model.get_node(n.no));

            var collectedNode = foundNode.Where(n => (n.coordinate_1.Equals(bhNode.Position.X) && n.coordinate_2.Equals(bhNode.Position.Y) && n.coordinate_3.Equals(bhNode.Position.Z)));

            if (collectedNode.ToList().Count>0)
            {
                return collectedNode.ToList().First(); 
            }


            return null;
        }

    }
}
