using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static IProfile FromRFEM(this rfModel.section section)
        {
            IProfile bhSection = null;

            //Chekc for material
            bhSection = GetSectionProfile(section);

            //generate section for steel


            //generate section for concrete



            return bhSection;
        }


        private static IProfile GetSectionProfile(rfModel.section section) {


            string profileName = (section.name.Split('|')[0]).Split(' ')[0];
            IProfile profile = null;
            double v1=0, v2=0, v3=0, v4=0, v5=0, v6=0, v7=0, v8=0, v9=0, v10=0;


            switch (profileName) {


                case "I":
                case "IPE":
                case "HE":
                case "HEA":
                case "HEB":
                case "HEM":
                case "UB":
                case "UBP":
                case "UC":
                case "HD":
                    //v1 = section.file;
                    //v2 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_b).fValue;
                    //v3 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s).fValue;
                    //v4 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g).fValue;
                    //v5 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r).fValue;
                    //v6 = sectionDBProps.FirstOrDefault(x => x.ID == rf3.DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1).fValue;
                    profile = Engine.Spatial.Create.ISectionProfile(v1, v2, v3, v4, v5, v6);
                    break;
                default:
                    Engine.Base.Compute.RecordError("Don't know how to create profile: " + section.name.Split('|')[0]);
                    break;
            }



            return profile; 

        }

    }
}
