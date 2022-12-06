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

        public static rfModel.nodal_support ToRFEM6(this Node bhNode, int nodalSupportNo, int constraintSupportNo)
        {
            rfModel.nodal_support rfNodelSupport = new rfModel.nodal_support()
            {
                no = nodalSupportNo,
                nodes = new int[] { constraintSupportNo },
                spring = new rfModel.vector_3d() { x = stiffnessTranslationBHToRF("" + bhNode.Support.TranslationX), y = stiffnessTranslationBHToRF("" + bhNode.Support.TranslationY), z = stiffnessTranslationBHToRF("" + bhNode.Support.TranslationZ) },
                rotational_restraint = new rfModel.vector_3d() { x = stiffnessTranslationBHToRF("" + bhNode.Support.RotationX), y = stiffnessTranslationBHToRF("" + bhNode.Support.RotationY), z = stiffnessTranslationBHToRF("" + bhNode.Support.RotationZ) },
            };

            return rfNodelSupport;

        }


        public static Double stiffnessTranslationBHToRF(String stiffness)
        {

            Double result = stiffness == "Free" ? 0.0 : Double.PositiveInfinity;

            return result;
        }
    }
}
