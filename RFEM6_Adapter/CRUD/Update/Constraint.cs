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
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Update Node                               ****/
        /***************************************************/

        private bool UpdateObjects(IEnumerable<Constraint6DOF> constraints)
        {
            bool success = true;

            foreach (Constraint6DOF constraint in constraints)
            {
                m_Model.set_nodal_support(constraint.ToRFEM6());
               
            }

            return success;
        }

    }
}
