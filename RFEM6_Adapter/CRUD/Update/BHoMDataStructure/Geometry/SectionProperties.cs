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
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;
using System.Text.RegularExpressions;

namespace BH.Adapter.RFEM6
{
#if RFEM6_8_2
    public partial class RFEM6AdapterV8_2 : BHoMAdapter
#elif RFEM6_12_11
    public partial class RFEM6AdapterV12_11 : BHoMAdapter
#else
    public partial class RFEM6Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Update Node                               ****/
        /***************************************************/

        private bool UpdateObjects(IEnumerable<ISectionProperty> sections)
        {
            bool success = true;

            foreach (ISectionProperty section in sections)
            {
                Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();
                int materialID=section.Material.GetRFEM6ID();
                IMaterialFragment material;
                materials.TryGetValue(materialID, out material);
                String materialName=material.GetType().Name;


#if RFEM6_8_2
                rfModel.section rfSection = null;
#else
                rfModel.cross_section rfSection = null;
#endif
                if (section is GenericSection && (section.Material is Glulam || section.Material is SawnTimber))
                {
                    rfSection = section.ToRFEM6_TimberSections(section.Material.GetType().Name);
                }
                else
                {
                    rfSection = section.ToRFEM6(section.Material.GetRFEM6ID(), section.Material.GetType().Name);
                }

#if RFEM6_8_2
                m_Model.set_section(rfSection);
#else
                m_Model.set_cross_section(rfSection);
#endif
                //m_Model.set_section(section.ToRFEM6(section.Material.GetRFEM6ID(), materialName));

            }

            return success;
        }

    }
}



