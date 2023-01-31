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

            if (section.name.Split(' ')[0].Equals("L")) {
                rfSecName_simplified += ".0";
            } else if (section.name.Split(' ')[0].Equals("RHSU")) {

                string[] signature = section.name.Split(' ')[1].Split('/');

                int height = (int)(Double.Parse(signature[0]) * 1000);
                int width = (int)(Double.Parse(signature[1]) * 1000);
                double thickness = (Double.Parse(signature[2]) * 1000);

                if (height.Equals(width)) {

                    rfSecName_simplified = "SHS" + height + "X" + width + "X" + thickness;
                }
                else {

                    rfSecName_simplified = "RHS" + height + "X" + width + "X" + thickness;
                }

            } else if (section.name.Split(' ')[0].Equals("1/2")) {

                string[] signature = section.name.Split(' ')[2].Split('x');

                int height = (int)(Double.Parse(signature[0]));
                int width = (int)(Double.Parse(signature[1]));
                double thickness = (Double.Parse(signature[2]));


                rfSecName_simplified = section.name.Split(' ')[1].Equals("UB")?"TUB":"TUC";

                rfSecName_simplified+=height + "X" + width + "X" + thickness;

            }

            var cs1 = BH.Engine.Library.Query.Library("StructureSectionProperties");
            var bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty) BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true);

            return  bhSec; 
        }


        }
}
