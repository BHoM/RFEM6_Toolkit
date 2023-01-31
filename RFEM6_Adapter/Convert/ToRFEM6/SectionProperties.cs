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

        public static rfModel.section ToRFEM6(this ISectionProperty bhSection, int secNo, int matNo, string materialType )
        {

            rfModel.section rfSection = null;

            alterSectionName( bhSection,  materialType);

            if (materialType.Equals("Steel")) {

                if (isStandardSteelSection(bhSection)) {

                    // create section
                    rfSection = new rfModel.section
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

                } else {

                    rfSection = new rfModel.section
                    {
                        //no = secNo,
                        //material = matNo,
                        //materialSpecified = true,
                        //type = rfModel.section_type.TYPE_PARAMETRIC_THIN_WALLED,
                        //typeSpecified = true,
                        //parametrization_type = rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__RECTANGULAR_HOLLOW_SECTION__RHS,
                        //parametrization_typeSpecified = true,
                        //name = bhSection.Name, // width/height as in RFEM, SI units

                        no = secNo,
                        material = matNo,
                        materialSpecified = true,
                        type = rfModel.section_type.TYPE_PARAMETRIC_THIN_WALLED,
                        typeSpecified = true,
                        parametrization_type = getParametrizationType(bhSection, materialType),
                        parametrization_typeSpecified = true,
                        manufacturing_type = rfModel.section_manufacturing_type.MANUFACTURING_TYPE_WELDED,
                        manufacturing_typeSpecified = true,
                        name = bhSection.Name, // width as in RFEM

                    };

                }

            } else if (materialType.Equals("Concrete")) {

                rfSection = new rfModel.section
                {
                    no = secNo,
                    material = matNo,
                    materialSpecified = true,
                    type = rfModel.section_type.TYPE_PARAMETRIC_MASSIVE_I,
                    typeSpecified = true,
                    parametrization_type = getParametrizationType(bhSection,materialType),
                    parametrization_typeSpecified = true,
                    name = bhSection.Name, // width/height as in RFEM, SI units
                };

            }

            return rfSection;
        }


        public static String alterSectionName(ISectionProperty bhSection, string materialType) {

            //Parametrs for dimensioning Cross Sections
            double v0, v1, v2, v3, v4, v5;

            if (materialType.Equals("Steel")) {

                if (isStandardSteelSection(bhSection))
                {

                    if (bhSection.Name.Split(' ')[0].Equals("L"))
                    {

                        v0 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[0]);
                        v1 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[1]);
                        double thickness = Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray().Last());

                        bhSection.Name = $"L {v0}x{v1}x{thickness}";

                    }
                    else if (!bhSection.Name.Substring(3, 1).Equals(" ") && ((bhSection.Name.Substring(0, 3).Equals("RHS")) || (bhSection.Name.Substring(0, 3).Equals("SHS"))))
                    {
                        bhSection.Name = bhSection.Name.Insert(3, " ");

                    }
                    else if (bhSection.Name.Substring(0, 3).Equals("TUB") || bhSection.Name.Substring(0, 3).Equals("TUC"))
                    {

                        bhSection.Name = bhSection.Name.Remove(0, 1);
                        bhSection.Name = bhSection.Name.Insert(0, "1/2 ");

                        return bhSection.Name;
                    }

                    //return bhSection.Name;

                }
                else {

                   
                    String bhSectionTypeName = (bhSection as SteelSection).SectionProfile.Shape.ToString();
                    String rfSectionTypeName = "";

                    switch ((bhSection as SteelSection).SectionProfile.Shape.ToString()) {

                        case "Box":

                            if ((bhSection as SteelSection).SectionProfile.GetType().Name.Equals("BoxProfile"))
                            {
                                v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Height;
                                v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Width;
                                v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Thickness;
                                rfSectionTypeName = ("RHSU " + v0 + "/" + v1 + "/" + v2 + "/" + v2 + "/" + v2 + "/" + v2);
                            }
                            else {
                                v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedBoxProfile).Height;
                                v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedBoxProfile).Width;
                                v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedBoxProfile).BotFlangeThickness;
                                v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedBoxProfile).TopFlangeThickness;
                                v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedBoxProfile).WebThickness;

                                rfSectionTypeName = ("RHSU " + v0 + "/" + v1 + "/" + v4 + "/" + v4 + "/" + v2 + "/" + v3);
                            }

                            break;
                        case "Circle":
                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.CircleProfile).Diameter;

                            rfSectionTypeName = "ROUND " + v0 + "/H";

                            break;
                        case "ISection":


                            if ((bhSection as SteelSection).SectionProfile.GetType().Name.Equals("ISectionProfile"))
                            {

                                v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).Height;
                                v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).Width;
                                v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).WebThickness;
                                v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).FlangeThickness;
                                v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).RootRadius;
                                v5 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).ToeRadius;


                                rfSectionTypeName = "I " + v0 + "/" + v1 + "/" + v2 + "/" + v3 + "/" + v4 + "/" + v5 + "/H";

                            }
                            else {

                                v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedISectionProfile).Height;
                                v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedISectionProfile).TopFlangeWidth;
                                v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedISectionProfile).BotFlangeWidth;
                                v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedISectionProfile).WebThickness;
                                v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedISectionProfile).TopFlangeThickness;
                                v5 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.FabricatedISectionProfile).BotFlangeThickness;


                                rfSectionTypeName = "IS "  + v0 + "/" + v1 + "/" + v2 + "/" + v3 + "/" + v4 + "/" + v5 + "/0/0/H";

                                Engine.Base.Compute.RecordWarning("Weld size for " + bhSection.Name + " has been set to 0!");

                            }


                            break;
                
                        case "Tee":

                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).Height;
                            v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).Width;
                            v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).WebThickness;
                            v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).FlangeThickness;
                            v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).RootRadius;
                            v5 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).ToeRadius;

                            rfSectionTypeName = "T "  + v0 + "/" + v1 + "/" + v2 + "/" + v3 + "/" + v4 + "/" + v5 + "/H";

                            break;
                        case "Tube":
                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TubeProfile).Diameter;
                            v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TubeProfile).Thickness;

                            rfSectionTypeName = "CHS " + v0 + "/" + v1 + "/H";

                            break;

                        case "Rectangle":

                            double CornerRadius = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).CornerRadius;
                            double height = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Height;
                            double width = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Width;
                            bool isSqrt = height.Equals(width);


                            if (CornerRadius > 0)
                            {

                                rfSectionTypeName = isSqrt  ? "SQUARER " + width + "/" + CornerRadius+"/H" : "FLAT " + width + "/" + height + "/H";

                            }
                            else
                            {

                                rfSectionTypeName = isSqrt ? "SQUARES " + width+"/H" : "FLAT " + width + "/" + height+"/H";

                            }


                            break;

                        default:

                            rfSectionTypeName = "ROUND " + 1 + "/H";

                            break;

                    }


                    bhSection.Name = rfSectionTypeName;
                    //return rfSectionTypeName;
                }


            } else if (materialType.Equals("Concrete")) {

                String bhSectionTypeName = (bhSection as ConcreteSection).SectionProfile.Shape.ToString();
                String rfSectionTypeName = "";

                //double v0, v1, v2, v3, v4, v5;

                switch(bhSectionTypeName)
                {

                    case "Circle":
                        rfSectionTypeName = "CIRCLE_M1 " + ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.CircleProfile).Diameter; 
                        break;
                    case "Rectangle":

                        double CornerRadius = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).CornerRadius;
                        double height = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Height;
                        double width = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Width;
                        bool isSqrt = height.Equals(width);


                        if (CornerRadius>0) {

                            rfSectionTypeName = isSqrt ? "SQR_M1 " + width +"/"+ CornerRadius : "RR_M1 " + width + "/" + height+"/"+ CornerRadius;

                        }
                        else {

                            rfSectionTypeName = isSqrt ?"SQ_M1 "+width:"R_M1 "+width+"/"+height;

                        }


                        break;
                    case "Tube":
                        v0 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TubeProfile).Diameter;
                        v1 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TubeProfile).Thickness;
                        rfSectionTypeName = "HCIRCLE_M1 " + v0+"/"+v1;
                        break;
                    case "Box":
                        v0 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Height;
                        v1 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Width;
                        v2 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Thickness;
                        v3 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Thickness;
                        v4 = 0;
                        v5 = 0;

                        rfSectionTypeName = "RRO_M1 " + v0 + "/" + v1 +"/" + v2 + "/" + v2 + "/" + v3 + "/" + v3 ;

                        break;
                    case "ISection":
                       
                        v0 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).Height;
                        v1 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).Width;
                        v2 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).FlangeThickness;
                        v3 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).WebThickness;

                        // rfSectionTypeName = "ID_M1 " + v0 + "/" + v2 + "/" + v1 + "/" + v2 +"/"+v1+"/"+v3;
                        rfSectionTypeName = "ID_M1 " + v0 + "/" + v1 + "/" + v2 + "/" + v3;

                        break;
                    case "Tee":

                        v0 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).Height;
                        v1 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).Width;
                        v2 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).FlangeThickness;
                        v3 = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).WebThickness;
                     
                        rfSectionTypeName = "T_M1 " + v0 + "/" + v1 + "/" + v2 + "/" + v3;

                        break;



                    default:

                        rfSectionTypeName = "CIRCLE_M1 " + 1;
                        Engine.Base.Compute.RecordWarning("Cross Section Could not be generated Round solid Section (D=1m) was generated instead");

                        break; 

                }

                bhSection.Name = rfSectionTypeName;

               // return rfSectionTypeName;

            }
            return bhSection.Name;

        }

        private static rfModel.section_parametrization_type getParametrizationType(this ISectionProperty bhSection,String materialType)
        {

            


            if (materialType.Equals("Concrete")) {

                string bhSectionTypeName = (bhSection as ConcreteSection).SectionProfile.Shape.ToString();

                switch (bhSectionTypeName)
                {

                    case "Circle":
                        return rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_CIRCLE__CIRCLE_M1;

                    case "Rectangle":

                        double CornerRadius = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).CornerRadius;
                        double height = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Height;
                        double width = ((bhSection as ConcreteSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Width;
                        bool isSqrt = height.Equals(width);
                        rfModel.section_parametrization_type paramType;

                        if (CornerRadius > 0)
                        {

                            paramType = isSqrt ? rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_ROUND_CORNER_SQUARE__SQR_M1 : rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_ROUND_CORNER_RECTANGLE__RR_M1;

                        }
                        else
                        {
                            paramType = isSqrt ? rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_SQUARE__SQ_M1 : rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1;
                        }
                        return paramType;

                    case "Tube":
                        return rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_HOLLOW_CIRCLE__HCIRCLE_M1;
                    case "Box":
                        return rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE_WITH_RECTANGULAR_OPENING__RRO_M1;
                    case "ISection":
                        return rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_DOUBLY_SYMMETRIC_I_SECTION__ID_M1;
                    case "Tee":
                        return rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_T_SECTION__T_M1;
                    default:
                        return rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_CIRCLE__CIRCLE_M1;
                }


            }
            else  {

                string bhSectionTypeName = (bhSection as SteelSection).SectionProfile.Shape.ToString();

                switch (bhSectionTypeName) {
                    case "Box":
                        return rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__UNSYMMETRIC_RECTANGULAR_HOLLOW_SECTION__RHSU;
                    case "Circle":
                        return rfModel.section_parametrization_type.PARAMETRIC_BARS__ROUND_BAR__ROUND;
                    case "ISection":

                        if ((bhSection as SteelSection).SectionProfile.GetType().Name.Equals("ISectionProfile")) {
                            return rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__I_SECTION__I;
                        }
                        else { return rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__SINGLY_SYMMETRIC_I_SECTION__IS; }


                    //case "Rectangle":
                    //    return rfModel.section_parametrization_type.PARAMETRIC_BARS__SHARP_CORNER_SQUARE_BAR__SQUARES;

                    case "Rectangle":


                        double CornerRadius = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).CornerRadius;
                        double height = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Height;
                        double width = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.RectangleProfile).Width;
                        bool isSqrt = height.Equals(width);

                        


                        if (CornerRadius > 0)
                        {

                            //rfSectionTypeName = isSqrt ? "SQUARER " + width + "/" + CornerRadius + "/H" : "FLAT " + width + "/" + height + "/H";
                            if (isSqrt) { return rfModel.section_parametrization_type.PARAMETRIC_BARS__ROUND_CORNER_SQUARE_BAR__SQUARER; }
                            else { 
                                Engine.Base.Compute.RecordWarning("Corner Radius of "+bhSection.Name+" has been set to 0!");
                                return rfModel.section_parametrization_type.PARAMETRIC_BARS__FLAT_BAR__FLAT;
                            }


                        }
                        else
                        {
                            if (isSqrt) { return rfModel.section_parametrization_type.PARAMETRIC_BARS__SHARP_CORNER_SQUARE_BAR__SQUARES; }
                            else { return rfModel.section_parametrization_type.PARAMETRIC_BARS__FLAT_BAR__FLAT; }
                            // rfSectionTypeName = isSqrt ? "SQUARES " + width + "/H" : "FLAT " + width + "/" + height + "/H";

                        }



                    case "Tee":

                        return rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__T_SECTION__T;

                    case "Tube":

                        return rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__CIRCULAR_HOLLOW_SECTION__CHS;


                    default: 
                        return rfModel.section_parametrization_type.PARAMETRIC_THIN_WALLED__UNSYMMETRIC_RECTANGULAR_HOLLOW_SECTION__RHSU;

                }
            
            }

           



        }

        private static bool isStandardSteelSection(ISectionProperty bhSection) {

            String bhSecNameAbrev = bhSection.Name.Split(' ')[0].Length>3 ? bhSection.Name.TrimStart().Substring(3).ToUpper(): bhSection.Name.Split(' ')[0];
            HashSet<String> crossSectionNameSet = new HashSet<String>() {"HE", "CHS", "IPE", "L", "RHS", "SHS", "UPE" , "PFC", "TUB", "TUC", "UB", "UBP", "UC", "1/2"};
            return crossSectionNameSet.Contains(bhSecNameAbrev);
        
        }

    }
}
