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


            if (bhSection.Name.Split(' ')[0].Equals("L")) {

                int v0 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[0]);
                int v1 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[1]);
                double thickness = Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray().Last());

                bhSection.Name = $"L {v0}x{v1}x{thickness}";

            } else if (!bhSection.Name.Substring(3, 1).Equals(" ")&&((bhSection.Name.Substring(0, 3).Equals("RHS")) || (bhSection.Name.Substring(0, 3).Equals("SHS"))))
            {
                bhSection.Name = bhSection.Name.Insert(3, " ");

            } else if (bhSection.Name.Substring(0, 3).Equals("TUB") || bhSection.Name.Substring(0, 3).Equals("TUC"))
            {

                bhSection.Name = bhSection.Name.Remove(0,1);
                bhSection.Name = bhSection.Name.Insert(0, "1/2 ");

            }

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
