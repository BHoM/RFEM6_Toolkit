///*
// * This file is part of the Buildings and Habitats object Model (BHoM)
// * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
// *
// * Each contributor holds copyright over their respective contributions.
// * The project versioning (Git) records all such contribution source information.
// *                                           
// *                                                                              
// * The BHoM is free software: you can redistribute it and/or modify         
// * it under the terms of the GNU Lesser General Public License as published by  
// * the Free Software Foundation, either version 3.0 of the License, or          
// * (at your option) any later version.                                          
// *                                                                              
// * The BHoM is distributed in the hope that it will be useful,              
// * but WITHOUT ANY WARRANTY; without even the implied warranty of               
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
// * GNU Lesser General Public License for more details.                          
// *                                                                            
// * You should have received a copy of the GNU Lesser General Public License     
// * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
// */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Numerics;

//using BH.oM.Adapter;
//using BH.oM.Structure.Elements;
//using BH.oM.Structure.Constraints;

//using rfModel = Dlubal.WS.Rfem6.Model;
//using BH.Engine.Structure;
//using BH.oM.Structure.Loads;
//using BH.oM.Adapter.Commands;
//using Dlubal.WS.Rfem6.Model;

//namespace BH.Adapter.RFEM6
//{
//    public partial class RFEM6Adapter
//    {
//        private bool CreateCollection(IEnumerable<LoadCombination> bhLoadCombinations)
//        {

//            // Taking Care of Analysis Settings

//            //Check if Analysis Setting does exist already
//            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_DESIGN_SITUATION);
//            List<rfModel.design_situation> foundDesingSituations = numbers.ToList().Select(n => m_Model.get_design_situation(n.no)).ToList();

//			Dictionary<int, Loadcase> LoadCaseDict = this.GetCachedOrReadAsDictionary<int, Loadcase>();

//			rfModel.design_situation analysis;

//            if (foundDesingSituations.Count == 0)
//            {
//                int no=m_Model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_DESIGN_SITUATION, 0);


//				analysis = new design_situation()
//                {
//                    no = no,
//					//name = $"DS {no}",
//					//user_defined_name_enabled = true,
//					//user_defined_name_enabledSpecified = true,
//					design_situation_type = "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT",
//					is_generated = false,
//					is_generatedSpecified = true,
//					consider_inclusive_exclusive_load_cases = false,
//					consider_inclusive_exclusive_load_casesSpecified = false,
//				};

//                m_Model.set_design_situation(analysis);
//			}
//            else
//            {

//                analysis = foundDesingSituations.Find(s => s.design_situation_type == "DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT");

//            }

//			foreach (LoadCombination loadCombination in bhLoadCombinations)
//			{
//				//Convert Load Case to RFEM6
//				rfModel.load_combination rfLoadCombination = loadCombination.ToRFEM6(analysis.no);
//				//Set Load Case
//				m_Model.set_load_combination(rfLoadCombination);
//			}

//			return true;
//        }

//    }
//}


