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

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<ISectionProperty> sectionProperties)
        {

            foreach (ISectionProperty section in sectionProperties)
            {


                // TODO: Check if this could be made more efficiently. By chance instead of searcihng for id use the one that has been assigned

                //Loading material numbers from RFEM
                rfModel.object_with_children[] materials = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
                IEnumerable<int> materialNumbers = materials.ToList().Select(m => m.no);
                int matNo = m_Model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_MATERIAL, 0);

                //Find material number for section
                foreach (int n in materialNumbers)
                {

                    var rfMatrial = m_Model.get_material(n);
                    string rfMatName = rfMatrial.name.Split('|')[0].Trim(new Char[] { ' ' });

                    if (rfMatName.Equals(section.Material.Name))
                    {
                        matNo = n;

                        break;
                    }
                }

                rfModel.section rfSection;

                //creation of Section Split up into different if statements for different section types

                if (section is GenericSection && (section.Material is Glulam|| section.Material is SawnTimber||section.Material is Concrete))
                {
                    rfSection = section.ToRFEM6_TimberSections( section.Material.GetType().Name );
                }
                else
                {
                    rfSection = section.ToRFEM6(matNo, section.Material.GetType().Name);
                }


                m_Model.set_section(rfSection);

            }

            return true;

        }


    }
}

