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

        public static BH.oM.Structure.Constraints.Constraint6DOF FromRFEM(this rfModel.nodal_support support)
        {

            BH.oM.Structure.Constraints.Constraint6DOF constraint = new BH.oM.Structure.Constraints.Constraint6DOF();
            constraint.TranslationX = (support.spring.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationY = (support.spring.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationZ = (support.spring.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationX = (support.rotational_restraint.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationY = (support.rotational_restraint.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationZ = (support.rotational_restraint.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);

            constraint.SetRFEM6ID(support.no);
            constraint.Name = support.name;
            return constraint;
        }

    }
}
