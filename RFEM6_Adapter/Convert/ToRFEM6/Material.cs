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

        public static rfModel.material ToRFEM6(this IMaterialFragment material)
        {
            //Object[] nameAndType = materialTypeAndNameTranslater(bhMateraial);

            rfModel.material rfMaterial = new rfModel.material
            {
                no = material.GetRFEM6ID(),
                name = material.Name,
                //name = "S235 (DIN EN 1993-1-1:2010-12)",
                //material_type = rfModel.material_material_type.TYPE_STEEL,
                comment = "",
                material_type = material.GetType().Name.Equals("Steel") ? rfModel.material_material_type.TYPE_STEEL : rfModel.material_material_type.TYPE_CONCRETE
            };

            return rfMaterial;

        }



    }
}
