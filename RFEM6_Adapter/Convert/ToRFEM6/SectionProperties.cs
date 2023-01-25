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

                //if (bhSection.Name.Split(' ')[0].Equals("L")) {

                //    int v0 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[0]);
                //    int v1 = (int)Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray()[1]);
                //    double thickness = Double.Parse(bhSection.Name.Split(' ').ToArray().Last().Split('x').ToArray().Last());

                //    bhSection.Name = $"L {v0}x{v1}x{thickness}";

                //} else if (!bhSection.Name.Substring(3, 1).Equals(" ") && ((bhSection.Name.Substring(0, 3).Equals("RHS")) || (bhSection.Name.Substring(0, 3).Equals("SHS"))))
                //{
                //    bhSection.Name = bhSection.Name.Insert(3, " ");

                //} else if (bhSection.Name.Substring(0, 3).Equals("TUB") || bhSection.Name.Substring(0, 3).Equals("TUC"))
                //{

                //    bhSection.Name = bhSection.Name.Remove(0, 1);
                //    bhSection.Name = bhSection.Name.Insert(0, "1/2 ");

                //}

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


            } else if (materialType.Equals("Concrete")) {


           

                rfSection = new rfModel.section
                {
                    no = secNo,
                    material = matNo,
                    materialSpecified = true,
                    type = rfModel.section_type.TYPE_PARAMETRIC_MASSIVE_I,
                    typeSpecified = true,
                    parametrization_type = getParametrizationType(bhSection),
                    parametrization_typeSpecified = true,
                    name = bhSection.Name, // width/height as in RFEM, SI units
                };

            }

            return rfSection;
        }


        private static void alterSectionName(ISectionProperty bhSection, string materialType) {

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

                    }

                }
                else {


                    
                    //TODO: 
                    //Needs Testing, havent tested this ad all....should handle all req parametric steel sections
                    //Next would be the crateion of the Cross sextion....Inspired by concrete section create parametr. Type function!!!


                    //double v0, v1, v2, v3, v4, v5;

                    String bhSectionTypeName = (bhSection as ConcreteSection).SectionProfile.Shape.ToString();
                    String rfSectionTypeName = "";

                    switch ((bhSection as ConcreteSection).SectionProfile.Shape.ToString()) {

                        case "Box":
                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Height;
                            v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Width;
                            v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).Thickness;
                            v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).OuterRadius;
                            v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.BoxProfile).InnerRadius;

                            rfSectionTypeName = v0.Equals(v1) ? ("SHS "+"/"+v0+"/"+v2+"/"+v3+"/"+v4+"/H"): ("RHS " + "/" + v0 + "/"+ v1+ "/" + v2 + "/" + v3 + "/" + v4 + "/H");

                            break;
                        case "Circle":
                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.CircleProfile).Diameter;

                            rfSectionTypeName = "ROUND " + v0 + "/H";

                            break;
                        case "ISection":
                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).Height;
                            v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).Width;
                            v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).WebThickness;
                            v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).FlangeThickness;
                            v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).RootRadius;
                            v5 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.ISectionProfile).ToeRadius;

                            rfSectionTypeName = "I " + "/" + v0 + "/" + v1 + "/" + v2 + "/" + v3 + "/" + v4 + "/" + v5 + "/H";

                            break;
                        case "Rectangle":



                            break;
                        case "Tee":

                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).Height;
                            v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).Width;
                            v2 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).WebThickness;
                            v3 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).FlangeThickness;
                            v4 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).RootRadius;
                            v5 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TSectionProfile).ToeRadius;

                            rfSectionTypeName = "T " + "/" + v0 + "/" + v1 + "/" + v2 + "/" + v3 + "/" + v4 + "/" + v5 + "/H";


                            break;
                        case "Tube":
                            v0 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TubeProfile).Diameter;
                            v1 = ((bhSection as SteelSection).SectionProfile as BH.oM.Spatial.ShapeProfiles.TubeProfile).Thickness;

                            rfSectionTypeName = "CHS " + "/" + v0 + "/" + v1 + "/H";

                            break;
                        default:

                           

                            rfSectionTypeName = "ROUND " + 1 + "/H";

                            break;

                    }




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

                            rfSectionTypeName = isSqrt ? "SQ_M1 " + width +"/"+ CornerRadius : "R_M1 " + width + "/" + height+"/"+ CornerRadius;

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
                     
                        rfSectionTypeName = "T_M1 " + v0 + "/" + v1 + "/" + v2 + "/" + v3 + "/";

                        break;
                    default:

                        rfSectionTypeName = "CIRCLE_M1 " + 1;
                        Engine.Base.Compute.RecordWarning("Cross Section Could not be generated Round solid Section (D=1m) was generated instead");

                        break; 

                }

                bhSection.Name = rfSectionTypeName;

            }
    
        }

        private static rfModel.section_parametrization_type getParametrizationType(this ISectionProperty bhSection)
        {

            string bhSectionTypeName=(bhSection as ConcreteSection).SectionProfile.Shape.ToString();


            switch(bhSectionTypeName) {

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
                        paramType = isSqrt ? rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_SQUARE__SQ_M1: rfModel.section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1;
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

        private static bool isStandardSteelSection(ISectionProperty bhSection) {

            String bhSecNameAbrev = bhSection.Name.Length>3 ? bhSection.Name.TrimStart().Substring(3).ToUpper(): bhSection.Name;
            HashSet<String> crossSectionNameSet = new HashSet<String>() {"HE", "CHS", "IPE", "L", "RHS", "SHS", "UPE" , "PFC", "TUB", "TUC", "UB", "UBP", "UC"};
            return crossSectionNameSet.Contains(bhSecNameAbrev);
        
        }

    }
}
