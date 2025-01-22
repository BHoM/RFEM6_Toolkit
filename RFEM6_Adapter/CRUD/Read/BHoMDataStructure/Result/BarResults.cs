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
using System.Globalization;

namespace BH.Adapter.RFEM6
{
	public partial class RFEM6Adapter
	{

		public IEnumerable<IResult> ReadResults(BarResultRequest request, ActionConfig actionConfig)
		{

			BH.Engine.Base.Compute.RecordWarning($"Divisions are set to {request.Divisions}. Division functionality has not been implemented yet in RFEM6_Toolkit. Currently, the number of divisions depends solely on what RFEM6 provides and can vary significantly based on the load type on the corresponding Bar/Member.");

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

				foreach (IGrouping<int, members_internal_forces_row> member in memberInternalForceGroup)
				{
					//Ignore Results that do now have a valied ID
					if (member.Key == 0) continue;

					//Determine Member length
					String lengthAsString = member.First().row.specification.Split(new[] { "L : ", " m" }, StringSplitOptions.None)[1];
					double memberLength = double.Parse(lengthAsString, CultureInfo.InvariantCulture);// Member Length in SI unit m;

					if (request.DivisionType == DivisionType.ExtremeValues)
					{


						var extremes = new Dictionary<string, (double value, members_internal_forces_row row)>();

						foreach (var memberSegment in member)
						{

							if (memberSegment.description.Contains("Extremes")) break;

						//	//local method updating dictinary
						//	void UpdateExtreme(string key, double value, members_internal_forces_row row, Boolean isNegative)
						//	{

						//		//bool valueGetsUpdated = (!extremes.ContainsKey(key) || isNegative )? value < extremes[key].value : value > extremes[key].value;
						//		bool valueGetsUpdated = !extremes.ContainsKey(key) || (isNegative ? value < extremes[key].value : value > extremes[key].value);

						//		if ((!extremes.ContainsKey(key) || valueGetsUpdated) & !isNegative)
						//		{
						//			extremes[key] = (value, row);
						//		}

						//	}

						//	UpdateExtreme("FX_Pos", memberSegment.row.internal_force_n, memberSegment, false);
						//	UpdateExtreme("FY_Pos", memberSegment.row.internal_force_vy, memberSegment, false);
						//	UpdateExtreme("FZ_Pos", memberSegment.row.internal_force_vz, memberSegment, false);
						//	UpdateExtreme("MX_Pos", memberSegment.row.internal_force_mt, memberSegment, false);
						//	UpdateExtreme("MY_Pos", memberSegment.row.internal_force_my, memberSegment, false);
						//	UpdateExtreme("MZ_Pos", memberSegment.row.internal_force_mz, memberSegment, false);
						//	UpdateExtreme("FX_Neg", memberSegment.row.internal_force_n, memberSegment, false);
						//	UpdateExtreme("FY_Neg", memberSegment.row.internal_force_vy, memberSegment, false);
						//	UpdateExtreme("FZ_Neg", memberSegment.row.internal_force_vz, memberSegment, false);
						//	UpdateExtreme("MX_Neg", memberSegment.row.internal_force_mt, memberSegment, false);
						//	UpdateExtreme("MY_Neg", memberSegment.row.internal_force_my, memberSegment, false);
						//	UpdateExtreme("MZ_Neg", memberSegment.row.internal_force_mz, memberSegment, false);
						//}


						//var extremeValues = new List<members_internal_forces_row>
						//						{
						//							extremes["FX_Pos"].row,
						//							extremes["FX_Pos"].row,
						//							extremes["FZ_Pos"].row,
						//							extremes["MX_Pos"].row,
						//							extremes["MY_Pos"].row,
						//							extremes["MZ_Pos"].row,
						//							extremes["FX_Neg"].row,
						//							extremes["FX_Neg"].row,
						//							extremes["FZ_Neg"].row,
						//							extremes["MX_Neg"].row,
						//							extremes["MY_Neg"].row,
						//							extremes["MZ_Neg"].row
						//						};

						//foreach (var e in extremeValues)
						//{
						//	//e.FromRFEM(c,memberLength).
						//	allInternalForces.Add(e.FromRFEM(c, memberLength));
						//}


						var extremes_ = member.ToList().TakeWhile(v=>v.description.Contains("Extreme")).Aggregate(
			   new
			   {
				   NMax = double.MinValue,
				   NMaxSection = member.ToList()[0],
				   NMin = double.MaxValue,
				   NMinSection = member.ToList()[0],
				   ZMax = double.MinValue,
				   ZMaxSection = member.ToList()[0],
				   ZMin = double.MaxValue,
				   ZMinSection = member.ToList()[0],
				   YMax = double.MinValue,
				   YMaxSection = member.ToList()[0],
				   YMin = double.MaxValue,
				   YMinSection = member.ToList()[0],
				   MXMax = double.MinValue,
				   MXMaxSection = member.ToList()[0],
				   MXMin = double.MaxValue,
				   MXMinSection = member.ToList()[0],
				   MYMax = double.MinValue,
				   MYMaxSection = member.ToList()[0],
				   MYMin = double.MaxValue,
				   MYMinSection = member.ToList()[0],
				   MZMax = double.MinValue,
				   MZMaxSection = member.ToList()[0],
				   MZMin = double.MaxValue,
				   MZMinSection = member.ToList()[0]
			   },
		(acc, m) => new
		{
			NMax = Math.Max(acc.NMax, m.row.internal_force_n),
			NMaxSection = m.row.internal_force_n > acc.NMax ? m : acc.NMaxSection,
			NMin = Math.Min(acc.NMin, m.row.internal_force_n),
			NMinSection = m.row.internal_force_n < acc.NMin ? m : acc.NMinSection,
			ZMax = Math.Max(acc.ZMax, m.row.internal_force_vz),
			ZMaxSection = m.row.internal_force_vz > acc.ZMax ? m : acc.ZMaxSection,
			ZMin = Math.Min(acc.ZMin, m.row.internal_force_vz),
			ZMinSection = m.row.internal_force_vz < acc.ZMin ? m : acc.ZMinSection,
			YMax = Math.Max(acc.YMax, m.row.internal_force_vy),
			YMaxSection = m.row.internal_force_vy > acc.YMax ? m : acc.YMaxSection,
			YMin = Math.Min(acc.YMin, m.row.internal_force_vy),
			YMinSection = m.row.internal_force_vy < acc.YMin ? m : acc.YMinSection,
			MXMax = Math.Max(acc.MXMax, m.row.internal_force_mt),
			MXMaxSection = m.row.internal_force_mt > acc.MXMax ? m : acc.MXMaxSection,
			MXMin = Math.Min(acc.MXMin, m.row.internal_force_mt),
			MXMinSection = m.row.internal_force_mt < acc.MXMin ? m : acc.MXMinSection,
			MYMax = Math.Max(acc.MYMax, m.row.internal_force_my),
			MYMaxSection = m.row.internal_force_my > acc.MYMax ? m : acc.MYMaxSection,
			MYMin = Math.Min(acc.MYMin, m.row.internal_force_my),
			MYMinSection = m.row.internal_force_my < acc.MYMin ? m : acc.MYMinSection,
			MZMax = Math.Max(acc.MZMax, m.row.internal_force_mz),
			MZMaxSection = m.row.internal_force_mz > acc.MZMax ? m : acc.MZMaxSection,
			MZMin = Math.Min(acc.MZMin, m.row.internal_force_mz),
			MZMinSection = m.row.internal_force_mz < acc.MZMin ? m : acc.MZMinSection
		}
			);

						extremeValues = new List<members_internal_forces_row>() {
   extremes_.MXMaxSection, extremes_.NMinSection,
   extremes_.ZMaxSection, extremes_.ZMinSection,
   extremes_.YMaxSection, extremes_.YMinSection,
   extremes_.MXMaxSection, extremes_.MXMinSection,
   extremes_.MYMaxSection, extremes_.MYMinSection,
   extremes_.MZMaxSection, extremes_.MZMinSection
};
						foreach (var e in extremeValues)
						{
							//e.FromRFEM(c,memberLength).
							allInternalForces.Add(e.FromRFEM(c, memberLength));
						}


						continue;
					}

					else
					{

						foreach (var memberSegment in member)
						{
							//Ignoring Rows after Extremes
							if (memberSegment.description.Contains("Extremes")) { break; }

							allInternalForces.Add(memberSegment.FromRFEM(c, memberLength));

						}
					}
				}

			}

			return allInternalForces;

		}

	}
}


