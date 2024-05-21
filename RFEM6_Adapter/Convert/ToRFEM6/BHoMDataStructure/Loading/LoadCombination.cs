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

        public static rfModel.load_combination ToRFEM6(this LoadCombination bhLoadcombination, int analysisNo)
        {


            load_combination rfLoadCombination = new rfModel.load_combination()
            {
                no = bhLoadcombination.Number,
                name = bhLoadcombination.Name,
                user_defined_name_enabled = true,
                user_defined_name_enabledSpecified = true,
                to_solve = true,
                to_solveSpecified = true,
                analysis_type = load_combination_analysis_type.ANALYSIS_TYPE_STATIC,
                analysis_typeSpecified = true,
                items = loadCombinationItems(bhLoadcombination),
                design_situation = 1,
                design_situationSpecified = true,
            };


            return rfLoadCombination;

        }

        private static load_combination_items_row[] loadCombinationItems(this LoadCombination bhLoadCombination)
        {
            load_combination_items_row[] loadCombinationItems = new load_combination_items_row[bhLoadCombination.LoadCases.Count];
            for (int c = 0; c < bhLoadCombination.LoadCases.Count; c++)
            {


                load_combination_items_row lc = new load_combination_items_row()
                {
                    no = c+1,
                    row = new load_combination_items()
                    {
                        load_case = bhLoadCombination.LoadCases[c].Item2.Number,
                        load_caseSpecified = true,
                        factor = bhLoadCombination.LoadCases[c].Item1,
                        factorSpecified = true,
                    }
                };
                loadCombinationItems[c] = lc;

            }

            //foreach (var item in bhLoadCombination.LoadCases)
            //{

            //    load_combination_items_row lc = new load_combination_items_row()
            //    {
            //        no = i,
            //        row = new load_combination_items()
            //        {
            //            load_case = item.Item2.Number,
            //            load_caseSpecified = true,
            //            factor = item.Item1,
            //            factorSpecified = true,
            //        }
            //    };
            //    loadCombinationItems.Append(lc);
            //    i++;

            //}

            return loadCombinationItems;
        }

    }
}

