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
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Structure;
using BH.oM.Structure.Loads;
using BH.oM.Adapter.Commands;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Loadcase> bhLoadCase)
        {

            //Check if Analysis Setting does exist already
            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_STATIC_ANALYSIS_SETTINGS);
            List<rfModel.static_analysis_settings> foundAnalysisSettings = numbers.ToList().Select(n => m_Model.get_static_analysis_settings(n.no)).ToList();


            List<rfModel.static_analysis_settings> analysisSettingList = foundAnalysisSettings.FindAll(s => s.analysis_type == static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR);

            rfModel.static_analysis_settings analysis;

            if (analysisSettingList.Count == 0)
            {
                analysis = new static_analysis_settings()
                {
                    no = m_Model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_STATIC_ANALYSIS_SETTINGS, 0),
                    analysis_type = static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR,
                    analysis_typeSpecified = true,
                };
            }
            else
            {

                analysis = foundAnalysisSettings.FindAll(s => s.analysis_type == static_analysis_settings_analysis_type.GEOMETRICALLY_LINEAR).First();

            }
            m_Model.set_static_analysis_settings(analysis);


            //Create Load Cases
            foreach (Loadcase loadCase in bhLoadCase)
            {

                load_case selfWeightLC = loadCase.ToRFEM6(analysis.no);

                m_Model.set_load_case(selfWeightLC);
            }



            return true;
        }

    }
}

