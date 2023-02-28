using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Constraint6DOF> ReadConstraints(List<string> ids = null)
        {

            List<Constraint6DOF> constraintList = new List<Constraint6DOF>();

            rfModel.object_with_children[] numbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
            IEnumerable<rfModel.nodal_support> foundSupports = numbers.ToList().Select(n => model.get_nodal_support(n.no));

            foreach (rfModel.nodal_support s in foundSupports)
            {
                Constraint6DOF rfConstraint = Convert.FromRFEM(s);
                constraintList.Add(rfConstraint);
            }

            return constraintList;
        }

   

    }
}
