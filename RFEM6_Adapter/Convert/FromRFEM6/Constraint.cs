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

        public static BH.oM.Structure.Constraints.Constraint6DOF FromRFEM(this rfModel.nodal_support node)
        {

            BH.oM.Structure.Constraints.Constraint6DOF constraint = new BH.oM.Structure.Constraints.Constraint6DOF();
            constraint.TranslationX = (node.spring.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed: oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationY = (node.spring.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationZ = (node.spring.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationX = (node.rotational_restraint.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationY = (node.rotational_restraint.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationZ = (node.rotational_restraint.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);

            return constraint; ;
        }

    }
}
