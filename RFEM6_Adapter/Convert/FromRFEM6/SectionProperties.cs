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
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static ISectionProperty FromRFEM(this rfModel.section section, rfModel.material rfMaterial)
        {
            ISectionProperty bhSection = null;

            String materialType = rfMaterial.application_context.ToString();


            //Chekc for material
            if (materialType.Equals("STEEL_DESIGN"))
            {

                if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_STEEL))
                {

                    bhSection = bhSteelSectionFromRfSection_Standard(section);

                }
                else
                {

                    bhSection = bhSteelSectionFromRfSection_NonStandard(section, rfMaterial);
                }

            }
            else if (materialType.Equals("CONCRETE_DESIGN"))
            {

                bhSection = bhConcreteFromRfSection_NonStandard(section, rfMaterial);

            }

            return bhSection;
        }


        private static ISectionProperty bhSteelSectionFromRfSection_Standard(rfModel.section section)
        {
            string rfSecName_simplified = section.name.Split('|')[0].Replace(" ", "").ToUpper();

            if (section.name.Split(' ')[0].Equals("L"))
            {
                rfSecName_simplified += ".0";
            }
            else if (section.name.Split(' ')[0].Equals("RHSU"))
            {

                string[] signature = section.name.Split(' ')[1].Split('/');

                int height = (int)(Double.Parse(signature[0]) * 1000);
                int width = (int)(Double.Parse(signature[1]) * 1000);
                double thickness = (Double.Parse(signature[2]) * 1000);

                if (height.Equals(width))
                {

                    rfSecName_simplified = "SHS" + height + "X" + width + "X" + thickness;
                }
                else
                {

                    rfSecName_simplified = "RHS" + height + "X" + width + "X" + thickness;
                }

            }
            else if (section.name.Split(' ')[0].Equals("1/2"))
            {

                string[] signature = section.name.Split(' ')[2].Split('x');

                int height = (int)(Double.Parse(signature[0]));
                int width = (int)(Double.Parse(signature[1]));
                double thickness = (Double.Parse(signature[2]));


                rfSecName_simplified = section.name.Split(' ')[1].Equals("UB") ? "TUB" : "TUC";

                rfSecName_simplified += height + "X" + width + "X" + thickness;

            }

            var cs1 = BH.Engine.Library.Query.Library("StructureSectionProperties");
            var bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty)BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true);

            return bhSec;
        }


        private static ISectionProperty bhConcreteFromRfSection_NonStandard(section rfSection, material rfmaterial)
        {
            string[] sectionSignature = rfSection.name.Split(' ');
            string sectionCatName = rfSection.name;
            var bhMaterial = rfmaterial.FromRFEM() as Concrete;
            String[] secParameters = new string[1];

            double width, height, diameter, thickness0, thickness1, thickness2, thickness3, radiusToe, radiusRoot;

            ISectionProperty bhSection = null;

            switch (sectionCatName)
            {

                case "CIRCLE_M1":

                    diameter = Double.Parse(sectionSignature[1]);

                    bhSection = BH.Engine.Structure.Create.ConcreteCircularSection(diameter, bhMaterial, sectionCatName, null);

                    break;

                case "R_M1":

                    secParameters = sectionSignature[1].Split('/');
                    width = Double.Parse(secParameters[0]);
                    height = Double.Parse(secParameters[1]);

                    bhSection = BH.Engine.Structure.Create.ConcreteRectangleSection(height, width, bhMaterial, sectionCatName, null);

                    break;

                case "HCIRCLE_M1":

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);
                    thickness0 = Double.Parse(secParameters[1]);

                    var bhProfile0 = BH.Engine.Spatial.Create.TubeProfile(diameter, thickness0);

                    bhSection = BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile0, bhMaterial, sectionCatName, null);

                    break;

                case "PRO_M1":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);
                    //thickness1 = Double.Parse(secParameters[3]);
                    //thickness2 = Double.Parse(secParameters[4]);
                    //thickness3 = Double.Parse(secParameters[5]);


                    BH.oM.Spatial.ShapeProfiles.BoxProfile bhProfile1 = BH.Engine.Spatial.Create.BoxProfile(height, width, thickness0);

                    bhSection = BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile1, bhMaterial, sectionCatName, null);

                    break;

                case "ID_M1":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//Flange
                    thickness1 = Double.Parse(secParameters[3]);//Web
                    //thickness2 = Double.Parse(secParameters[4]);
                    //thickness3 = Double.Parse(secParameters[5]);


                    BH.oM.Spatial.ShapeProfiles.ISectionProfile bhProfile2 = BH.Engine.Spatial.Create.ISectionProfile(height, width, thickness1, thickness0, 0, 0);

                    bhSection = BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile2, bhMaterial, sectionCatName, null);

                    break;

                case "T_M1":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//Flange
                    thickness1 = Double.Parse(secParameters[3]);//Web
                    //thickness2 = Double.Parse(secParameters[4]);
                    //thickness3 = Double.Parse(secParameters[5]);


                    BH.oM.Spatial.ShapeProfiles.TSectionProfile bhProfile3 = BH.Engine.Spatial.Create.TSectionProfile(height, width, thickness1, thickness0, 0, 0);

                    bhSection = BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile3, bhMaterial, sectionCatName, null);

                    break;

                default:

                    bhSection = BH.Engine.Structure.Create.ConcreteCircularSection(1, bhMaterial, sectionCatName, null);

                    break;

            }


            return bhSection;
        }


        private static ISectionProperty bhSteelSectionFromRfSection_NonStandard(section rfSection, material rfmaterial)
        {
            string[] sectionSignature = rfSection.name.Split(' ');
            string sectionCatName = sectionSignature[0];
            var bhMaterial = rfmaterial.FromRFEM() as Steel;
            String[] secParameters = new string[1];

            double width, height, diameter, thickness0, thickness1, thickness2, thickness3, radiusToe, radiusRoot0, radiusRoot1, flangeWidthTop, flangeWidthBot;

            ISectionProperty bhSection = null;

            switch (sectionCatName)
            {


                case "ROUND":

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);

                    bhSection = BH.Engine.Structure.Create.SteelCircularSection(diameter, bhMaterial, rfSection.name);

                    break;

                case "RHSU":

                    secParameters = sectionSignature[1].Split('/');
                    width = Double.Parse(secParameters[0]);
                    height = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//web1
                    thickness1 = Double.Parse(secParameters[3]);//web2
                    thickness2 = Double.Parse(secParameters[4]);//flange1
                    thickness3 = Double.Parse(secParameters[5]);//flange2

                    bhSection = BH.Engine.Structure.Create.FabricatedSteelBoxSection(height, width, thickness0, thickness2, 0, bhMaterial, rfSection.name);

                    break;

                case "IS":

                    secParameters = sectionSignature[1].Split('/');

                    height = Double.Parse(secParameters[0]);
                    flangeWidthTop = Double.Parse(secParameters[1]);
                    flangeWidthBot = Double.Parse(secParameters[2]);
                    thickness0 = Double.Parse(secParameters[3]);//web
                    thickness1 = Double.Parse(secParameters[4]);//FlangeTop
                    thickness2 = Double.Parse(secParameters[5]);//FlangeBot
                    radiusRoot0 = Double.Parse(secParameters[6]);//RootRadTop
                    radiusRoot1 = Double.Parse(secParameters[7]);//RootRadBot

                    bhSection = BH.Engine.Structure.Create.SteelFabricatedISection(height, thickness0, flangeWidthTop, thickness1, flangeWidthBot, thickness2, 0, bhMaterial, rfSection.name);

                    break;

                case "I":

                    secParameters = sectionSignature[1].Split('/');

                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//thickWeb
                    thickness1 = Double.Parse(secParameters[3]);//thickFlange
                    radiusRoot0 = Double.Parse(secParameters[4]);//RootRadTop
                    radiusToe = Double.Parse(secParameters[5]);//ToeRad

                    bhSection = BH.Engine.Structure.Create.SteelISection(height, thickness0, width, thickness1, radiusRoot0, radiusToe, bhMaterial, rfSection.name);

                    break;

                case "FLAT":

                    secParameters = sectionSignature[1].Split('/');

                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);

                    bhSection = BH.Engine.Structure.Create.SteelRectangleSection(height, width, 0, bhMaterial, rfSection.name);

                    break;

                case "T":

                    secParameters = sectionSignature[1].Split('/');

                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//thickWeb
                    thickness1 = Double.Parse(secParameters[3]);//thickFlange
                    radiusRoot0 = Double.Parse(secParameters[4]);//RootRadTop
                    radiusToe = Double.Parse(secParameters[5]);//ToeRad

                    bhSection = BH.Engine.Structure.Create.SteelTSection(height, thickness0, width, thickness1, radiusRoot0, radiusToe, bhMaterial, rfSection.name);

                    break;

                case "CHS":

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);
                    thickness0 = Double.Parse(secParameters[1]);

                    bhSection = BH.Engine.Structure.Create.SteelTubeSection(diameter, thickness0, bhMaterial, rfSection.name);

                    break;

                default:

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);

                    bhSection = BH.Engine.Structure.Create.SteelCircularSection(1, bhMaterial, "Default Round Beam D=1m");

                    break;

            }


            return bhSection;
        }

    }
}
