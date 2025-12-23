/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System.Security.RightsManagement;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {
            List<ISectionProperty> sectionList = new List<ISectionProperty>();

            //Read Standard BHoM Steel Libraries
            List<IBHoMObject> sectionListLib = new List<IBHoMObject>();
            sectionListLib = BH.Engine.Library.Query.Library("Structure\\SectionProperties");

            // Read RFEM Material Fragments From Caching System
            Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();
            IMaterialFragment sectionMaterials;

            // Read RFEM Sections from Model
            var sectionNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
            var allSections = sectionNumbers.ToList().Select(n => m_Model.get_section(n.no));

            foreach (var section in allSections)
            {

                // Preprocessing RFEM6 Section Name to match with BHoM Library
                //String sectionName = new String((section.name.Where(c => !char.IsWhiteSpace(c)).ToArray())).Split()[0];
                //sectionName = sectionName.Split('|')[0];
                ISectionProperty bhSection;
                //Standard steel sections
                if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_STEEL))
                {
                    if (!materials.TryGetValue(section.material, out sectionMaterials))
                    {
                        continue;
                    }

                    // Brows Through the BHoM Library and find the best match
                    bhSection = section.FromRFEM_Standardized_Steel(sectionListLib, sectionMaterials);
                    bhSection.SetRFEM6ID(section.no);
                    sectionList.Add(bhSection);

                }
                // Concrete Section Parametric Massive I
                else if (section.type.Equals(rfModel.section_type.TYPE_PARAMETRIC_MASSIVE_I))
                {

                    if (!materials.TryGetValue(section.material, out sectionMaterials))
                    {
                        continue;
                    }
                    bhSection = section.FromRFEM_MassivI(sectionMaterials);
                    bhSection.SetRFEM6ID(section.no);
                    sectionList.Add(bhSection);
                }
                // Standardized Timber Section
                else if (section.type.Equals(rfModel.section_type.TYPE_STANDARDIZED_TIMBER))
                {
                    if (!materials.TryGetValue(section.material, out sectionMaterials))
                    {

                        continue;
                    }

                    if (sectionMaterials != null)
                    {
                        //bhSection = Convert.FromRFEM_Standardized_Timber(section, sectionMaterials);
                        bhSection = section.FromRFEM_Standardized_Timber(sectionMaterials);

                        bhSection.SetRFEM6ID(section.no);
                        sectionList.Add(bhSection);
                    }
                }
                // Parametric Thin Walled Section
                else if (section.type.Equals(rfModel.section_type.TYPE_PARAMETRIC_THIN_WALLED))
                {
                    if (!materials.TryGetValue(section.material, out sectionMaterials))
                    {
                        continue;
                    }
                    bhSection = section.FromRFEM_ThinWalled(sectionMaterials);
                    bhSection.SetRFEM6ID(section.no);
                    sectionList.Add(bhSection);
                }
                else {

                    BH.Engine.Base.Compute.RecordWarning($"The section {section.name} is not supported by the RFEM6 Adapter and will be read as ExplicitSection.");

                    if (!materials.TryGetValue(section.material, out sectionMaterials))
                    {
                        continue;
                    }

                    bhSection =new ExplicitSection() { Name=section.name,Material= sectionMaterials };
                    bhSection.SetRFEM6ID(section.no);                   
                    sectionList.Add(bhSection);
                
                }
            }

            //Sort sections by RFEM6 ID
            sectionList = sectionList.OrderBy(x => x.GetRFEM6ID()).ToList();


            return sectionList;
        }
    }
}



