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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;
using BH.oM.Structure.Loads;
using BH.oM.Adapter.Commands;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.load_case ToRFEM6(this Loadcase bhLoadcase, int analysisNo)
        {


            load_case rfLoadCase = new rfModel.load_case()
            {
                no = bhLoadcase.GetRFEM6ID(),
                name = bhLoadcase.Name,
                static_analysis_settings = analysisNo,
                analysis_type = load_case_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                static_analysis_settingsSpecified = true,
                action_category = bhLoadcase.Nature.ToRFEM(),
                calculate_critical_load = true,
                calculate_critical_loadSpecified = true,
                stability_analysis_settings = analysisNo,
                stability_analysis_settingsSpecified = true,
            };

            
            return rfLoadCase;

        }

        private static String ToRFEM(this LoadNature loadNature)
        {



            switch (loadNature)
            {
                case LoadNature.Dead:
                    return "ACTION_CATEGORY_PERMANENT_G";

                case LoadNature.SuperDead:
                    return "ACTION_CATEGORY_PERMANENT_IMPOSED_GQ";

                case LoadNature.Snow:
                    return "ACTION_CATEGORY_SNOW_ICE_LOADS_FINLAND_ICELAND_QS";

                case LoadNature.Live:
                    return "ACTION_CATEGORY_IMPOSED_LOADS_CATEGORY_A_DOMESTIC_RESIDENTIAL_AREAS_QI_A";

                case LoadNature.Temperature:
                    return "ACTION_CATEGORY_TEMPERATURE_NON_FIRE_QT";

                case LoadNature.Accidental:
                    return "ACTION_CATEGORY_ACCIDENTAL_ACTIONS_A";

                case LoadNature.Prestress:
                    return "ACTION_CATEGORY_PRESTRESS_P";

                case LoadNature.Seismic:
                    return "ACTION_CATEGORY_SEISMIC_ACTIONS_AE";

                case LoadNature.Wind:
                    return "ACTION_CATEGORY_WIND_QW";

                default:
                    BH.Engine.Base.Compute.RecordWarning($"Load cases of Nature Type {loadNature} will be set as Dead Load!");
                    return "ACTION_CATEGORY_PERMANENT_G";
            }


            //return "";
        }

    }
}

