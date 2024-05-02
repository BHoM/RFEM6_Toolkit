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
using BH.oM.Base;
using System.Text.RegularExpressions;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        // Conversion of RFEM6 section of type Massiv I to BHoM Section
        public static ISectionProperty FromRFEM_MassivI(this rfModel.section rfSection, IMaterialFragment sectionMaterials)
        {

            section_parametrization_type parametrization_type = rfSection.parametrization_type;

            ISectionProperty resultSection = new ExplicitSection() { Name = rfSection.name, Material = sectionMaterials };

            switch (parametrization_type)
            {
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_RECTANGLE__R_M1:
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_SQUARE__SQ_M1:
                    resultSection = BH.Engine.Structure.Create.ConcreteRectangleSection(rfSection.h, rfSection.b, sectionMaterials as Concrete, rfSection.name, null);
                    break;
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_ROUND_CORNER_RECTANGLE__RR_M1:
                case section_parametrization_type.PARAMETRIC_MASSIVE_I__MASSIVE_ROUND_CORNER_SQUARE__SQR_M1:
                    resultSection = BH.Engine.Structure.Create.SectionPropertyFromProfile(BH.Engine.Spatial.Create.RectangleProfile(rfSection.h, rfSection.b, rfSection.r_o), sectionMaterials, rfSection.name);
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
                    var b0 = rfSection.b_w_l_M;
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

            if (resultSection == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} could not be read and will be set to Explicite parameters set to 0!");
                resultSection = new ExplicitSection() { Name = rfSection.name, Material = sectionMaterials };
            }

            return resultSection;

        }

        // Conversion of RFEM6 section of type Thin Walled to BHoM Section
        public static ISectionProperty FromRFEM_ThinWalled(this rfModel.section rfSection, IMaterialFragment sectionMaterials)
        {
            section_parametrization_type parametrization_type = rfSection.parametrization_type;

            ISectionProperty resultSection = new ExplicitSection() { };

            switch (parametrization_type)
            {
                case section_parametrization_type.PARAMETRIC_THIN_WALLED__SQUARE_HOLLOW_SECTION__SHS:

                    if (rfSection.manufacturing_type.Equals(section_manufacturing_type.MANUFACTURING_TYPE_WELDED))
                    {
                        //welded
                        resultSection = BH.Engine.Structure.Create.FabricatedSteelBoxSection(rfSection.h, rfSection.b, rfSection.t, rfSection.t, 0, sectionMaterials as Steel, rfSection.name);
                    }
                    else
                    {
                        //cold formed or hot rolled
                        resultSection = BH.Engine.Structure.Create.SteelBoxSection(rfSection.h, rfSection.b, rfSection.t, rfSection.r_i, rfSection.r_o, sectionMaterials as Steel, rfSection.name);
                    }

                    break;
                case section_parametrization_type.PARAMETRIC_THIN_WALLED__RECTANGULAR_HOLLOW_SECTION__RHS:

                    if (rfSection.manufacturing_type.Equals(section_manufacturing_type.MANUFACTURING_TYPE_WELDED))
                    {
                        //welded
                        resultSection = BH.Engine.Structure.Create.FabricatedSteelBoxSection(rfSection.h, rfSection.b, rfSection.t, rfSection.t, 0, sectionMaterials as Steel, rfSection.name);
                    }
                    else
                    {
                        //cold formed or hot rolled
                        resultSection = BH.Engine.Structure.Create.SteelBoxSection(rfSection.h, rfSection.b, rfSection.t, rfSection.r_i, rfSection.r_o, sectionMaterials as Steel, rfSection.name);
                    }

                    break;
                case section_parametrization_type.PARAMETRIC_THIN_WALLED__CIRCULAR_HOLLOW_SECTION__CHS:


                    //cold formed or hot rolled
                    resultSection = BH.Engine.Structure.Create.SteelTubeSection(rfSection.d, rfSection.t, sectionMaterials as Steel, rfSection.name);

                    break;
                case section_parametrization_type.PARAMETRIC_THIN_WALLED__I_SECTION__I:

                    if (rfSection.manufacturing_type.Equals(section_manufacturing_type.MANUFACTURING_TYPE_WELDED))
                    {
                        //welded 
                        resultSection = BH.Engine.Structure.Create.SteelFabricatedISection(rfSection.h, rfSection.t_w, rfSection.b, rfSection.t_f, rfSection.b, rfSection.t_f, rfSection.a_weld, sectionMaterials as Steel, rfSection.name);
                    }
                    else
                    {
                        //Hot rolled
                        resultSection = BH.Engine.Structure.Create.SteelISection(rfSection.h, rfSection.t_w, rfSection.b, rfSection.t_f, rfSection.r_1, rfSection.r_2, sectionMaterials as Steel, rfSection.name);
                    }

                    break;
                case section_parametrization_type.PARAMETRIC_THIN_WALLED__T_SECTION__T:

                    // welded
                    if (rfSection.manufacturing_type.Equals(section_manufacturing_type.MANUFACTURING_TYPE_WELDED))
                    {
                        BH.Engine.Base.Compute.RecordWarning($"BHoM does not support welded T section. {rfSection.name} will be read as Hot Rolled.");
                    }
                    //Hot rolled
                    resultSection = BH.Engine.Structure.Create.SteelTSection(rfSection.h, rfSection.t_w, rfSection.b, rfSection.t_f, rfSection.r_1, rfSection.r_2, sectionMaterials as Steel, rfSection.name);


                    break;
                default:

                    //If section has not been implemented yet
                    BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} could not be read and will be set to Explicite parameters set to 0!");
                    resultSection = new ExplicitSection() { Name = rfSection.name, Material = sectionMaterials };
                    break;
            }

            // creation of BHoM Section has failed
            if (resultSection == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} could not be read and will be set to Explicite parameters set to 0!");
                resultSection = new ExplicitSection() { Name = rfSection.name, Material = sectionMaterials };

            }


            return resultSection;

        }


        public static ISectionProperty FromRFEM_Standardized_Steel(this rfModel.section rfSection, List<IBHoMObject> bhSections, IMaterialFragment sectionMaterials)
        {


            String sectionName = new String((rfSection.name.Where(c => !char.IsWhiteSpace(c)).ToArray())).Split()[0];
            sectionName = sectionName.Split('|')[0];
            //sectionName = sectionName.Replace(" ", "");
            sectionName = Regex.Replace(sectionName, @"[^a-zA-Z0-9]", "");

            Dictionary<ISectionProperty, int> scorsDict = new Dictionary<ISectionProperty, int>();
            bhSections.ForEach(z => scorsDict.Add((ISectionProperty)z, BH.Engine.Search.Compute.MatchScore(sectionName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", ""))));

            Dictionary<int, HashSet<IBHoMObject>> scorsDict_test = new Dictionary<int, HashSet<IBHoMObject>>();
            //bhSections.ForEach(z => scorsDict_test[BH.Engine.Search.Compute.MatchScore(sectionName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", "")], )));


            foreach (var z in bhSections)
            {

                HashSet<IBHoMObject> value;
                int key = BH.Engine.Search.Compute.MatchScore(sectionName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", ""));
                if (scorsDict_test.TryGetValue(key, out value))
                {
                    scorsDict_test[key].Add((ISectionProperty)z);

                }
                else
                {

                    scorsDict_test[key] = new HashSet<IBHoMObject>() { z };
                }


            }
            var sortedSectNames_test = scorsDict_test.OrderByDescending(z => z.Key).ToDictionary(z => z.Key, z => z.Value);

            var result = sortedSectNames_test.Values.First().ToList()[0];

            if (sortedSectNames_test.Keys.First() < 85)
            {

                string noMatchSecName = rfSection.name.Split('|')[0];
                BH.Engine.Base.Compute.RecordWarning($"The Standard section {noMatchSecName} has no corresponding object within the BHoM dataset. It will be read as ExpliciteSection with an appropriate name and material");
                return new ExplicitSection() { Name = noMatchSecName, Material = sectionMaterials };
            }

            if (sortedSectNames_test.Values.First().Count > 1)
            {
                result = sortedSectNames_test.Values.First().ToList()[0];

                foreach (var i in sortedSectNames_test.Values.First())
                {

                    string mod_name = Regex.Replace(i.Name, @"[^a-zA-Z0-9]", "");
                    string mod_sectionName = Regex.Replace(sectionName, @"[^a-zA-Z0-9]", "");

                    if (IsAnagramUsingSort(mod_name, mod_sectionName))
                    {

                        result = i;

                        break;
                    }

                }

            }

            // Set Correct Materials
            ((ISectionProperty)result).Material = sectionMaterials;

            return (ISectionProperty)result;


        }

        // Conversion of RFEM6 section of type Standardized Timber to BHoM Section
        public static ISectionProperty FromRFEM_Standardized_Timber(this rfModel.section rfSection, IMaterialFragment sectionMaterials)
        {
            section_type parametrization_type = rfSection.type;

            ISectionProperty resultSection = new ExplicitSection() { };

            switch (parametrization_type)
            {
                case section_type.TYPE_STANDARDIZED_TIMBER:
                    resultSection = BH.Engine.Structure.Create.TimberRectangleSection(rfSection.h, rfSection.b, 0, sectionMaterials as ITimber, rfSection.name);
                    break;

                default:
                    BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} could not be read and will be set to Explicit parameters set to 0!");
                    resultSection = new ExplicitSection() { Name = rfSection.name, Material = sectionMaterials };
                    break;

            }

            // creation of BHoM Section has failed
            if (resultSection == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"Section {rfSection.name} could not be read and will be set to Explicit parameters set to 0!");
                resultSection = new ExplicitSection() { Name = rfSection.name, Material = sectionMaterials };

            }

            return resultSection;
        }

        // Function to check if two strings are anagrams
        public static bool IsAnagramUsingSort(string str1, string str2)
        {
            if (str1.Length != str2.Length)
                return false;

            char[] sorted1 = str1.ToCharArray();
            char[] sorted2 = str2.ToCharArray();

            Array.Sort(sorted1);
            Array.Sort(sorted2);

            return sorted1.SequenceEqual(sorted2);
        }

    }
}

