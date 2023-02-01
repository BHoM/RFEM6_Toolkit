using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;


using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static rfModel.material ToRFEM6(IMaterialFragment bhMateraial, int materialNo)
        {


            Object[] nameAndType = materialTypeAndNameTranslater(bhMateraial);

            rfModel.material rfMaterial = new rfModel.material
            {
                no = materialNo,
                name = nameAndType[0].ToString(),
                //name = "S235 (DIN EN 1993-1-1:2010-12)",
                //material_type = rfModel.material_material_type.TYPE_STEEL,
                comment = "" + nameAndType[2],
                material_type = (rfModel.material_material_type)nameAndType[1],
            };


            return rfMaterial;

        }

        private static Object[] materialTypeAndNameTranslater(IMaterialFragment bhMaterial)
        {


            Object[] result = new Object[3];

            if (bhMaterial.GetType().Equals(typeof(Steel)))
            {
                result[0] = bhMaterial.Name;
                result[1] = rfModel.material_material_type.TYPE_STEEL;
                result[2] = bhMaterial.Name + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "Density") + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "DampingRatio") + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "PoissonsRatio") + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "ThermalExpansionCoeff") + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "YoungsModulus") + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "YieldStress") + "|" +
                    BH.Engine.Base.Query.PropertyValue(bhMaterial, "UltimateStress");
            }
            else if (bhMaterial.GetType().Equals(typeof(Concrete)))
            {
                result[0] = bhMaterial.Name;
                result[1] = rfModel.material_material_type.TYPE_CONCRETE;
                result[2] = bhMaterial.Name + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "Density") + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "DampingRatio") + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "PoissonsRatio") + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "ThermalExpansionCoeff") + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "YoungsModulus") + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "CylinderStrength") + "|" +
                BH.Engine.Base.Query.PropertyValue(bhMaterial, "CubeStrength");
            }
            else
            {
                Engine.Base.Compute.RecordWarning("The Material " + bhMaterial + " has not been implemented yet. Steel S235 has been created insted.");
                result[0] = "S235";
                result[1] = rfModel.material_material_type.TYPE_STEEL;
                result[2] = bhMaterial.Name + "|" +
                7850 + "|" +
                0 + "|" +
                0.3 + "|" +
                0.000012 + "|" +
                2.1e+11 + "|" +
                2.35e+8 + "|" +
                3.6e+8;

            }

            return result;
        }

    }
}
