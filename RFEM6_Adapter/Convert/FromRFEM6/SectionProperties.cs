using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static ISectionProperty FromRFEM(this rfModel.section section)
        {
            ISectionProperty bhSection = null;

            //Chekc for material
            
            bhSection = GetSectionProfile2(section);

            //generate section for steel


            //generate section for concrete



            return bhSection;
        }


        private static ISectionProperty GetSectionProfile2(rfModel.section section)
        {
            string rfSecName_simplified = section.name.Split('|')[0].Replace(" ", "").ToUpper();

            var cs1 = BH.Engine.Library.Query.Library("StructureSectionProperties");
            var bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty) BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true);

            return  bhSec; 
        }


        }
}
