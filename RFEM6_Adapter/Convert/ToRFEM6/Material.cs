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
                material_type = (rfModel.material_material_type) nameAndType[1],
            };


            return rfMaterial;

        }


        private static Object[] materialTypeAndNameTranslater(IMaterialFragment bhMaterial)
        {

            
            Object[] result = new Object[2];

            if (bhMaterial.GetType().Equals(typeof(Steel)))
            {
                result[0] = bhMaterial.Name;
                result[1] = rfModel.material_material_type.TYPE_STEEL;
            }else if (bhMaterial.GetType().Equals(typeof(Concrete)))
            {
                result[0] = bhMaterial.Name;
                result[1] = rfModel.material_material_type.TYPE_CONCRETE;
            }
            else
            {
                Engine.Base.Compute.RecordWarning("The Material "+bhMaterial+" has not been implemented yet. Steel S235 has been created insted.");
                result[0] = "S235";
                result[1] = rfModel.material_material_type.TYPE_STEEL;

            }



            return result;
        }

    }
}
