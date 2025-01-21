/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Loads;
using Dlubal.WS.Rfem6.Model;
using System.Xml.Linq;
using BH.Engine.Base;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Structure.Results;
using System.Configuration;

namespace BH.Adapter.RFEM6
{
	public partial class RFEM6Adapter
	{

		public IEnumerable<IResult> ReadResults(BarResultRequest request, ActionConfig actionConfig)
		{

			m_Model.use_detailed_member_results(true);
			// Loading of Member And LoadCase Ids
			List<int> memberIds = request.ObjectIds.Select(s => Int32.Parse(s.ToString())).ToList();
			List<int> loadCaseIds = request.Cases.Select(s => Int32.Parse(s.ToString())).ToList();
			
			// Definition of Object Locations for Members
			object_location[] filters = memberIds.Select(n => new object_location() { type = object_types.E_OBJECT_TYPE_MEMBER, no = n, parent_no = 0 }).ToArray();


			List<IResult> allInternalForces = new List<IResult>();
			foreach (int c in loadCaseIds)
			{
				//Get Results bar forces for specific LC
				members_internal_forces_row[] resultForMemberInternalForces = m_Model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters, axes_type.MEMBER_AXES);
				


				//Grouping of Internal Forces grouped by Member No
				var memberInternalForceGroup = resultForMemberInternalForces.GroupBy(r => r.row.member_no);

				foreach (IGrouping<int, members_internal_forces_row> group in memberInternalForceGroup)
				{
					if (group.Key == 0) continue;
					//If we are looing for extreme values
					if (request.DivisionType == DivisionType.ExtremeValues)
					{
						var extremeRow = group.Where(g=>g.description.ToUpper().Contains("EXTREMES")).First();
						allInternalForces.Add(extremeRow.FromRFEM(c, request.DivisionType));
						continue;
					}


					foreach (var g in group) {

						

						//Likely unnecessary
						if (g.description.ToUpper().Contains("EXTREMES")) break ;

						allInternalForces.Add(g.FromRFEM(c, request.DivisionType));
						
					}

				}

			}

			return allInternalForces;

		}

	}
}


