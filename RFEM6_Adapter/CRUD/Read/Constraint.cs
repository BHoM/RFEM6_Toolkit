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

        private List<Node> ReadConstraints(List<string> ids = null)
        {

            List<Node> constraintList = new List<Node>();

            rfModel.object_with_children[] numbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
            IEnumerable<rfModel.nodal_support> foundSupports = numbers.ToList().Select(n => model.get_nodal_support(n.no));

            foreach (rfModel.nodal_support s in foundSupports)
            {
                List<int> nodeNo = s.nodes.ToList();
                BH.oM.Structure.Constraints.Constraint6DOF rfConstraint = Convert.FromRFEM(s);

                foreach (int i in nodeNo)
                {

                    model.get_node(i);
                    Node n=Convert.FromRFEM(model.get_node(i));
                    n.Support=rfConstraint;
                    constraintList.Add(n);
                }

            }

            return constraintList;
        }

    }
}
