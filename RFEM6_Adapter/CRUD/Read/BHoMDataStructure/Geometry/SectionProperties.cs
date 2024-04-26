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
using BH.oM.Spatial.ShapeProfiles;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;
using BH.oM.Base;
using System.Text.RegularExpressions;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        //private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        //{

        //    List<ISectionProperty> sectionList = new List<ISectionProperty>();

        //    var sectionNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
        //    var allSections = sectionNumbers.ToList().Select(n => m_Model.get_section(n.no));

        //    Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();
        //    IMaterialFragment material;
        //    var eu_Steel_Section = BH.Engine.Library.Query.Library("EU_SteelSections");
        //    var uk_Steel_Section = BH.Engine.Library.Query.Library("UK_SteelSections");
        //    var us_Steel_Section = BH.Engine.Library.Query.Library("US_SteelSections");


        //    foreach (var section in allSections)
        //    {

        //        //String sectionName = (new string(section.name.Where(c => !char.IsWhiteSpace(c)).ToArray())).Split()[0];
        //        String sectionName = new String((section.name.Where(c => !char.IsWhiteSpace(c)).ToArray())).Split()[0];
        //        sectionName = sectionName.Split('|')[0];
        //        ISectionProperty bhSetion;
        //        //Standard steel sections
        //        if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_STEEL))
        //        {

        //            //Checking differnt data sets for for sections

        //            bhSetion = BH.Engine.Library.Query.Match("EU_SteelSections", sectionName, true, true).DeepClone() as SteelSection;
        //            if (bhSetion is null) {
        //                bhSetion = BH.Engine.Library.Query.Match("UK_SteelSections", sectionName, true, true).DeepClone() as SteelSection;
        //            }
        //            if (bhSetion is null) {
        //                bhSetion = BH.Engine.Library.Query.Match("US_SteelSections", sectionName, true, true).DeepClone() as SteelSection;
        //            }

        //            bhSetion.SetRFEM6ID(section.no);
        //            sectionList.Add(bhSetion);

        //        }
        //        //Concrete Section Parametric Massive I
        //        else if (section.type.Equals(rfModel.section_type.TYPE_PARAMETRIC_MASSIVE_I))
        //        {

        //            if (!materials.TryGetValue(section.material, out material))
        //            {
        //                material = m_Model.get_material(section.material).FromRFEM();
        //                materials[section.material] = material;
        //            }


        //            if (material != null)
        //            {

        //                bhSetion = section.FromRFEM(material);
        //                bhSetion.SetRFEM6ID(section.no);
        //                sectionList.Add(bhSetion);

        //            }

        //        }
        //        else if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_TIMBER))
        //        {
        //            if (!materials.TryGetValue(section.material, out material))
        //            {
        //                material = m_Model.get_material(section.material).FromRFEM();
        //                materials[section.material] = material;
        //            }


        //            if (material != null)
        //            {

        //                bhSetion = section.FromRFEM(material);
        //                bhSetion.SetRFEM6ID(section.no);
        //                sectionList.Add(bhSetion);

        //            }


        //        }
        //        else {


        //        }


        //        //IMaterialFragment material;

        //    }

        //    return sectionList;
        //}

        public static ISectionProperty ReadStandardSteelSections(string sectionName, List<IBHoMObject> bhSections)
        {

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


            return (ISectionProperty)result;

            //var scorsDictSorted = scorsDict.OrderByDescending(z => z.Value).ToDictionary(z => z.Key, z => z.Value);
            //var sortedSectNames = scorsDictSorted.Keys.ToList();

            //var matchedSection = sortedSectNames[0];

            //return matchedSection;

        }



        private List<ISectionProperty> ReadSectionProperties_refactor(List<string> ids = null)
        {
            List<ISectionProperty> sectionList = new List<ISectionProperty>();
            //Read Standard BHoM Steel Libraries
            List<IBHoMObject> sectionListLib = new List<IBHoMObject>();
            var eu_Steel_Section = BH.Engine.Library.Query.Library("EU_SteelSections");
            var uk_Steel_Section = BH.Engine.Library.Query.Library("UK_SteelSections");
            var us_Steel_Section = BH.Engine.Library.Query.Library("US_SteelSections");
            sectionListLib.AddRange(eu_Steel_Section);
            sectionListLib.AddRange(uk_Steel_Section);
            sectionListLib.AddRange(us_Steel_Section);

            // Read RFEM Material Fragments From Caching System
            Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();
            IMaterialFragment material;

            // Read RFEM Sections from Model
            var sectionNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
            var allSections = sectionNumbers.ToList().Select(n => m_Model.get_section(n.no));



            foreach (var section in allSections)
            {

                // Preprocessing RFEM6 Section Name to match with BHoM Library
                String sectionName = new String((section.name.Where(c => !char.IsWhiteSpace(c)).ToArray())).Split()[0];
                sectionName = sectionName.Split('|')[0];
                ISectionProperty bhSetion;
                //Standard steel sections
                if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_STEEL))
                {

                    // Brows Through the BHoM Library and find the best match
                    bhSetion = ReadStandardSteelSections(sectionName, sectionListLib);

                    bhSetion.SetRFEM6ID(section.no);
                    sectionList.Add(bhSetion);

                }
                // Concrete Section Parametric Massive I
                else if (section.type.Equals(rfModel.section_type.TYPE_PARAMETRIC_MASSIVE_I))
                {

                    if (!materials.TryGetValue(section.material, out material))
                    {
                        material = m_Model.get_material(section.material).FromRFEM();
                        materials[section.material] = material;
                    }


                    if (material != null)
                    {

                        bhSetion = section.FromRFEM(material);
                        bhSetion.SetRFEM6ID(section.no);
                        sectionList.Add(bhSetion);

                    }

                }
                // Standardized Timber Section
                else if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_TIMBER))
                {
                    if (!materials.TryGetValue(section.material, out material))
                    {
                        material = m_Model.get_material(section.material).FromRFEM();
                        materials[section.material] = material;
                    }


                    if (material != null)
                    {

                        bhSetion = section.FromRFEM(material);
                        bhSetion.SetRFEM6ID(section.no);
                        sectionList.Add(bhSetion);

                    }


                }
                else
                {



                }



                //IMaterialFragment material;

            }

            return sectionList;
        }

        private static bool IsAnagramUsingSort(string str1, string str2)
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

