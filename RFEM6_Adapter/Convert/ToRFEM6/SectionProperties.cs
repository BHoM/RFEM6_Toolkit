using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static rfModel.section ToRFEM6(this ISectionProperty bhSection, int secNo, int matNo )
        {

            // create section
            rfModel.section rfSection = new rfModel.section
            {
                no = secNo,
                material = matNo,
                materialSpecified = true,
                name = bhSection.Name,
                typeSpecified = true,
                type = rfModel.section_type.TYPE_STANDARDIZED_STEEL,
                manufacturing_type = rfModel.section_manufacturing_type.MANUFACTURING_TYPE_HOT_ROLLED,
                manufacturing_typeSpecified = true,
                thin_walled_model = true,
                thin_walled_modelSpecified = true,
            };


            return rfSection;

        }

    }
}
