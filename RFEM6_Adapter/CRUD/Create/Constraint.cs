using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<Constraint6DOF> supports)
        {
           
            foreach (Constraint6DOF supprort in supports)
            {
                rfModel.nodal_support rfNodelSuport = supprort.ToRFEM6();
                model.set_nodal_support(rfNodelSuport);
            }

            return true;

            //Has been implemented inside of Nodes.cs


        }

    }
}
