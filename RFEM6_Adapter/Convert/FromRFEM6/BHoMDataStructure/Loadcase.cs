/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static Loadcase FromRFEM(this rfModel.load_case loadCase)
        {

            Loadcase bhLoadCase = new Loadcase() { Name = loadCase.name, Nature = loadCase.action_category.FromRFEM(), Number = loadCase.no };
            bhLoadCase.SetRFEM6ID(loadCase.no);


            return bhLoadCase;
        }


        private static LoadNature FromRFEM(this String actionCat)
        {

            switch (actionCat)
            {
                case "ACTION_CATEGORY_SEISMIC_ACTIONS_AE":
                    return LoadNature.Seismic;
                case "ACTION_CATEGORY_PERMANENT_G":
                    return LoadNature.Dead;
                case "ACTION_CATEGORY_PERMANENT_IMPOSED_GQ":
                    return LoadNature.SuperDead;
                case "ACTION_CATEGORY_PRESTRESS_P":
                    return LoadNature.Prestress;
                case "ACTION_CATEGORY_IMPOSED_LOADS_CATEGORY_A_DOMESTIC_RESIDENTIAL_AREAS_QI_A":
                    return LoadNature.Live;
                case "ACTION_CATEGORY_SNOW_ICE_LOADS_FINLAND_ICELAND_QS":
                    return LoadNature.Snow;
                case "ACTION_CATEGORY_WIND_QW":
                    return LoadNature.Wind;
                case "ACTION_CATEGORY_TEMPERATURE_NON_FIRE_QT":
                    return LoadNature.Temperature;
                case "ACTION_CATEGORY_ACCIDENTAL_ACTIONS_A":
                    return LoadNature.Accidental;
                default:
                    
                    return LoadNature.Other;
            }


        }
    }
}
