/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */
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
using BH.Engine.Base;

using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Physical.Materials;
using static System.Collections.Specialized.BitVector32;
using System.Security.Permissions;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static ISectionProperty FromRFEM(this rfModel.section section, IMaterialFragment bhMaterial)
        {
            ISectionProperty bhSection = null;

            if (bhMaterial is Steel)
            {

                if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_STEEL))
                {

                    bhSection = TranslateStandardizedSteelSectionToBHOM(section);

                }
                else
                {

                    bhSection = TranslateThinWalledSectionToBHoM(section, bhMaterial);
                }

            }

            else if (bhMaterial is Concrete || bhMaterial is Glulam || bhMaterial is SawnTimber)
            {

                bhSection = TranslateMassiveISectionToBHoM(section, bhMaterial);

            }
            //else if (bhMaterial is Glulam || bhMaterial is SawnTimber)
            //{
            //    var profile = Engine.Spatial.Create.RectangleProfile(section.h, section.b, 0);
            //    bhSection = BH.Engine.Structure.Create.GenericSectionFromProfile(profile, bhMaterial, section.name);
            //    bhSection = TranslateMassiveISectionToBHoM(section, bhMaterial);


            //}

            // if not section has been found, create a default section
            if (bhSection == null)

            {
                BH.Engine.Base.Compute.RecordWarning($"Section {section.name} could not be read and will be set to Explicite parameters set to 0!");
                bhSection = new BH.oM.Structure.SectionProperties.ExplicitSection { Name = section.name };

            }

            bhSection.SetRFEM6ID(section.no);

            if (section.comment.Contains("BHComment"))
            {

                string sectionComment = "";

                sectionComment = section.comment.Split(';').ToList().Last();

                BH.Engine.Base.Modify.SetPropertyValue(bhSection, "Comment", sectionComment);

            }



            return bhSection;
        }

        //Translates Standard Steel Sections From RFEM int BHoM
        private static ISectionProperty TranslateStandardizedSteelSectionToBHOM(rfModel.section section)
        {

            BH.oM.Structure.SectionProperties.ISectionProperty bhSec = null;
            string rfSecName_simplified = section.name.Split('|')[0].Replace(" ", "").ToUpper();

            if (section.name.Split(' ')[0].Equals("L"))
            {

                bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty)BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true).DeepClone();

                if (bhSec is null)
                {
                    rfSecName_simplified += ".0";
                    bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty)BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true).DeepClone();

                }

                return bhSec.DeepClone();
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


                rfSecName_simplified = section.name.Split(' ')[1].Equals("UB") ? "TUB " : "TUC ";

                rfSecName_simplified += height + "X" + width + "X" + thickness;

                bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty)BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true).DeepClone();
                if (bhSec is null)
                {

                    rfSecName_simplified = section.name.Split(' ')[1].Equals("UB") ? "1/2 UB " : "1/2 UC ";

                    rfSecName_simplified += height + "X" + width + "X" + thickness;

                    bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty)BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true).DeepClone();
                }

            }
            else
            {
                bhSec = (BH.oM.Structure.SectionProperties.ISectionProperty)BH.Engine.Library.Query.Match("SectionProperties", rfSecName_simplified, true, true).DeepClone();
            }
            return bhSec;
        }


        //Translates Massive I Section to BHoM. Massive I is usually used for Concrete of Glulam or Sawn Timber
        private static ISectionProperty TranslateMassiveISectionToBHoM(section rfSection, IMaterialFragment bhMaterial)
        {
            string[] sectionSignature = rfSection.name.Split(' ');
            string sectionCatName = rfSection.name.Split(' ')[0].ToString();
            String[] secParameters = new string[1];
            Concrete bhConcrete = bhMaterial as Concrete;
            String sectionName = bhMaterial is Concrete ? sectionCatName : rfSection.comment.Split(':')[1];

            double width, height, diameter, thickness0, thickness1, thickness2, thickness3, radiusToe, radiusRoot;

            ISectionProperty bhSection = null;


            switch (sectionCatName)
            {

                case "CIRCLE_M1":

                    diameter = Double.Parse(sectionSignature[1]);
                    var d = rfSection.d;

                    bhSection = bhMaterial is Concrete ?
                    BH.Engine.Structure.Create.ConcreteCircularSection(diameter, bhConcrete, sectionName, null) as ISectionProperty :
                    BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.CircleProfile(diameter), bhMaterial, sectionName);

                    break;

                case "R_M1":

                    secParameters = sectionSignature[1].Split('/');
                    width = Double.Parse(secParameters[0]);
                    height = Double.Parse(secParameters[1]);


                    bhSection = bhMaterial is Concrete ?
                    BH.Engine.Structure.Create.ConcreteRectangleSection(height, width, bhConcrete, sectionName, null) as ISectionProperty :
                    BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.RectangleProfile(height, width), bhMaterial, sectionName);

                    break;

                case "HCIRCLE_M1":

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);
                    thickness0 = Double.Parse(secParameters[1]);

                    var bhProfile0 = BH.Engine.Spatial.Create.TubeProfile(diameter, thickness0);

                    bhSection = bhMaterial is Concrete ?
                    BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile0, bhConcrete, null) as ISectionProperty :
                    BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.TubeProfile(diameter, thickness0), bhMaterial, sectionName);

                    break;

                case "PRO_M1":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);

                    BH.oM.Spatial.ShapeProfiles.BoxProfile bhProfile1 = BH.Engine.Spatial.Create.BoxProfile(height, width, thickness0);

                    bhSection = bhMaterial is Concrete ?
                    BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile1, bhConcrete, null) as ISectionProperty :
                    BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.BoxProfile(height, width, thickness0), bhMaterial, sectionName);

                    break;

                case "ID_M1":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//Flange
                    thickness1 = Double.Parse(secParameters[3]);//Web

                    BH.oM.Spatial.ShapeProfiles.ISectionProfile bhProfile2 = BH.Engine.Spatial.Create.ISectionProfile(height, width, thickness1, thickness0, 0, 0);

                    bhSection = bhMaterial is Concrete ?
                    BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile2, bhConcrete, null) as ISectionProperty :
                    BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.ISectionProfile(height, width, thickness1, thickness0, 0, 0), bhMaterial, sectionName);


                    break;

                case "T_M1":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//Flange
                    thickness1 = Double.Parse(secParameters[3]);//Web

                    BH.oM.Spatial.ShapeProfiles.TSectionProfile bhProfile3 = BH.Engine.Spatial.Create.TSectionProfile(height, width, thickness1, thickness0, 0, 0);

                    bhSection = bhMaterial is Concrete ?
                    BH.Engine.Structure.Create.ConcreteSectionFromProfile(bhProfile3, bhConcrete, null) as ISectionProperty :
                    BH.Engine.Structure.Create.GenericSectionFromProfile(BH.Engine.Spatial.Create.TSectionProfile(height, width, thickness1, thickness0, 0, 0), bhMaterial, sectionName);


                    break;

                default:

                    bhSection = BH.Engine.Structure.Create.ConcreteCircularSection(1, bhConcrete, null);

                    break;

            }

            return bhSection;
        }

        // Translates Thin Walled Section to BHoM. Thin Walled Sections are usually used for Steel
        private static ISectionProperty TranslateThinWalledSectionToBHoM(section rfSection, IMaterialFragment bhMaterial)
        {
            string[] sectionSignature = rfSection.name.Split(' ');
            string sectionCatName = sectionSignature[0];
            String[] secParameters = new string[1];
            Steel bhSteel = bhMaterial as Steel;

            double width, height, diameter, thickness0, thickness1, thickness2, thickness3, radiusToe, radiusRoot0, radiusRoot1, flangeWidthTop, flangeWidthBot;

            ISectionProperty bhSection = null;

            switch (sectionCatName)
            {

                case "ROUND":

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);
                    bhSection = BH.Engine.Structure.Create.SteelCircularSection(diameter, bhSteel, rfSection.name);

                    break;

                case "RHSU":

                    secParameters = sectionSignature[1].Split('/');
                    width = Double.Parse(secParameters[1]);
                    height = Double.Parse(secParameters[0]);
                    thickness0 = Double.Parse(secParameters[2]);//web1
                    thickness1 = Double.Parse(secParameters[3]);//web2
                    thickness2 = Double.Parse(secParameters[4]);//flange1
                    thickness3 = Double.Parse(secParameters[5]);//flange2

                    bhSection = BH.Engine.Structure.Create.FabricatedSteelBoxSection(height, width, thickness0, thickness2, 0, bhSteel, rfSection.name);

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

                    bhSection = BH.Engine.Structure.Create.SteelFabricatedISection(height, thickness0, flangeWidthTop, thickness1, flangeWidthBot, thickness2, 0, bhSteel, rfSection.name);

                    break;

                case "I":

                    secParameters = sectionSignature[1].Split('/');

                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//thickWeb
                    thickness1 = Double.Parse(secParameters[3]);//thickFlange
                    radiusRoot0 = Double.Parse(secParameters[4]);//RootRadTop
                    radiusToe = Double.Parse(secParameters[5]);//ToeRad

                    bhSection = BH.Engine.Structure.Create.SteelISection(height, thickness0, width, thickness1, radiusRoot0, radiusToe, bhSteel, rfSection.name);

                    break;

                case "FLAT":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);

                    bhSection = BH.Engine.Structure.Create.SteelRectangleSection(height, width, 0, bhSteel, rfSection.name);

                    break;

                case "T":

                    secParameters = sectionSignature[1].Split('/');
                    height = Double.Parse(secParameters[0]);
                    width = Double.Parse(secParameters[1]);
                    thickness0 = Double.Parse(secParameters[2]);//thickWeb
                    thickness1 = Double.Parse(secParameters[3]);//thickFlange
                    radiusRoot0 = Double.Parse(secParameters[4]);//RootRadTop
                    radiusToe = Double.Parse(secParameters[5]);//ToeRad

                    bhSection = BH.Engine.Structure.Create.SteelTSection(height, thickness0, width, thickness1, radiusRoot0, radiusToe, bhSteel, rfSection.name);

                    break;

                case "CHS":

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);
                    thickness0 = Double.Parse(secParameters[1]);
                    bhSection = BH.Engine.Structure.Create.SteelTubeSection(diameter, thickness0, bhSteel, rfSection.name);

                    break;

                default:

                    secParameters = sectionSignature[1].Split('/');
                    diameter = Double.Parse(secParameters[0]);
                    bhSection = BH.Engine.Structure.Create.SteelCircularSection(1, bhSteel, "Default Round Beam D=1m");

                    break;

            }

            bhSection.Name = rfSection.name;

            return bhSection;
        }


        public static ISectionProperty FromRFEM_MassivI(this rfModel.section rfSection, IMaterialFragment sectionMaterials)
        {

            section_parametrization_type parametrization_type = rfSection.parametrization_type;

            ISectionProperty resultSection = new ExplicitSection() { };

            switch (parametrization_type)
            {
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1:
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_SQUARE__SQ_M1:
                    resultSection = BH.Engine.Structure.Create.ConcreteRectangleSection(rfSection.h, rfSection.b, sectionMaterials as Concrete, rfSection.name, null);
                    break;
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_ROUND_CORNER_RECTANGLE__RR_M1:
                    resultSection = BH.Engine.Structure.Create.SectionPropertyFromProfile(BH.Engine.Spatial.Create.RectangleProfile(rfSection.d, rfSection.t, rfSection.r_o), sectionMaterials, rfSection.name);
                    break;
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_T_SECTION__T_M1:
                    resultSection = BH.Engine.Structure.Create.ConcreteTSection(rfSection.h, rfSection.b_w_M, rfSection.b, rfSection.h_f_M, sectionMaterials as Concrete, rfSection.name);
                    break;
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_HOLLOW_CIRCLE__HCIRCLE_M1:
                    resultSection = BH.Engine.Structure.Create.SectionPropertyFromProfile(BH.Engine.Spatial.Create.TubeProfile(rfSection.d, rfSection.t), sectionMaterials, rfSection.name);
                    break;
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_CIRCLE__CIRCLE_M1:
                    resultSection = BH.Engine.Structure.Create.ConcreteCircularSection(rfSection.d, sectionMaterials as Concrete, rfSection.name, null);
                    break;
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE_WITH_RECTANGULAR_OPENING__RRO_M1
:
                    var h0 = rfSection.h_f_b_M;
                    var h1 = rfSection.h_f_t_M;
                    var b0 = rfSection.b_w_i_M;
                    var b1 = rfSection.b_w_r_M;

                    if (!(h0 == h1 && b0 == b1 && h0 == b1))
                    {
                        BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} can't be read as wall thickness of the Box need to constant. A Explicit section will be read instead!");
                        break;
                    }

                    resultSection = BH.Engine.Structure.Create.SectionPropertyFromProfile(BH.Engine.Spatial.Create.BoxProfile(rfSection.h, rfSection.b, rfSection.h_f_b_M), sectionMaterials, rfSection.name);

                    break;

                default:

                    BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} could not be read and will be set to Explicite parameters set to 0!");
                    resultSection = new ExplicitSection() { Name = rfSection.name };
                    break;
            }

            return resultSection;

        }


        public static ISectionProperty FromRFEM_Standardized(this rfModel.section rfSection, IMaterialFragment sectionMaterials)
        {


            return null;

        }

        public static ISectionProperty FromRFEM_ThinWalled(this rfModel.section rfSection, IMaterialFragment sectionMaterials)
        {
            section_parametrization_type parametrization_type = rfSection.parametrization_type;

            var s = parametrization_type;

            switch (parametrization_type)
            {
                case section_parametrization_type.PARAMETRIC_THIN_WALLED__I_SECTION__I:

                    //BHoM ISection

                    return null;
                default:
                    break;
            }



            return null;



        }

    }
}

