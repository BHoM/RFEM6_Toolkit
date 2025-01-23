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
using System.ComponentModel;
using System.Linq.Expressions;

namespace BH.Adapter.RFEM6
{
	public partial class RFEM6Adapter
	{

		public IEnumerable<IResult> ReadResults(BarResultRequest request, ActionConfig actionConfig)
		{

			BH.Engine.Base.Compute.RecordWarning($"Divisions are set to {request.Divisions}. Division functionality has not been implemented yet in RFEM6_Toolkit. Currently, the number of divisions depends solely on what RFEM6 provides and can vary significantly based on the load type on the corresponding Bar/Member and the DivisionType.");

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

				///////////////////////////////////////////playground

				INotifyPropertyChanged[] barResults = m_Model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters, axes_type.MEMBER_AXES);

				switch (request.ResultType)
				{

					case BarResultType.BarForce:
						barResults = m_Model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters, axes_type.MEMBER_AXES);
						break;
					case BarResultType.BarDisplacement:
						barResults = m_Model.get_results_for_members_local_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters, axes_type.MEMBER_AXES);
						break;
					case BarResultType.BarDeformation:
						barResults = m_Model.get_results_for_members_global_deformations(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters);
						break;
					case BarResultType.BarStrain:
						barResults = m_Model.get_results_for_members_strains(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters, axes_type.MEMBER_AXES);
						break;
					case BarResultType.BarStress:

						BH.Engine.Base.Compute.RecordError("The Pull of Bar Stresses has not been developed Yet");
						return null;
						break;

					default:
						barResults = m_Model.get_results_for_members_internal_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, c, filters, axes_type.MEMBER_AXES);
						break;

				}

				//Grouping of Internal Forces grouped by Member No
				var memberInternalForceGroup = resultForMemberInternalForces.GroupBy(r => r.row.member_no);

				foreach (IGrouping<int, members_internal_forces_row> member in memberInternalForceGroup)
				{
					//Ignore Results that do now have a valied ID
					if (member.Key == 0) continue;

					//Determine Member length
					String lengthAsString = member.First().row.specification.Split(new[] { "L : ", " m" }, StringSplitOptions.None)[1];
					double memberLength = double.Parse(lengthAsString, CultureInfo.InvariantCulture);// Member Length in SI unit m;


					//If 
					if (request.DivisionType == DivisionType.ExtremeValues)
					{

						List<members_internal_forces_row> extremeValues = getExtremesForBarForces(member.ToList());

						foreach (var e in extremeValues)
						{
							//e.FromRFEM(c,memberLength).
							allInternalForces.Add(e.FromRFEM(c, memberLength));
						}


						continue;
					}
					//}

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


		private List<members_internal_forces_row> getExtremesForBarForces(List<members_internal_forces_row> membersInternalForces)
		{

			var extremes_ = membersInternalForces.ToList().TakeWhile(v => !v.description.Contains("Extreme")).Aggregate(
			   new
			   {
				   NMaxSection = membersInternalForces.First(),
				   NMinSection = membersInternalForces.First(),
				   ZMaxSection = membersInternalForces.First(),
				   ZMinSection = membersInternalForces.First(),
				   YMaxSection = membersInternalForces.First(),
				   YMinSection = membersInternalForces.First(),
				   MXMaxSection = membersInternalForces.First(),
				   MXMinSection = membersInternalForces.First(),
				   MYMaxSection = membersInternalForces.First(),
				   MYMinSection = membersInternalForces.First(),
				   MZMaxSection = membersInternalForces.First(),
				   MZMinSection = membersInternalForces.First()
			   },
		(acc, m) => new
		{
			NMaxSection = m.row.internal_force_n > acc.NMaxSection.row.internal_force_n ? m : acc.NMaxSection,
			NMinSection = m.row.internal_force_n < acc.NMinSection.row.internal_force_n ? m : acc.NMinSection,
			ZMaxSection = m.row.internal_force_vz > acc.ZMaxSection.row.internal_force_vz ? m : acc.ZMaxSection,
			ZMinSection = m.row.internal_force_vz < acc.ZMinSection.row.internal_force_vz ? m : acc.ZMinSection,
			YMaxSection = m.row.internal_force_vy > acc.YMaxSection.row.internal_force_vy ? m : acc.YMaxSection,
			YMinSection = m.row.internal_force_vy < acc.YMinSection.row.internal_force_vy ? m : acc.YMinSection,
			MXMaxSection = m.row.internal_force_mt > acc.MXMaxSection.row.internal_force_mt ? m : acc.MXMaxSection,
			MXMinSection = m.row.internal_force_mt < acc.MXMinSection.row.internal_force_mt ? m : acc.MXMinSection,
			MYMaxSection = m.row.internal_force_my > acc.MYMaxSection.row.internal_force_my ? m : acc.MYMaxSection,
			MYMinSection = m.row.internal_force_my < acc.MYMinSection.row.internal_force_my ? m : acc.MYMinSection,
			MZMaxSection = m.row.internal_force_mz > acc.MZMaxSection.row.internal_force_mz ? m : acc.MZMaxSection,
			MZMinSection = m.row.internal_force_mz < acc.MZMinSection.row.internal_force_mz ? m : acc.MZMinSection
		}
			);

			List<members_internal_forces_row> resultList = new List<members_internal_forces_row>() {
	extremes_.NMaxSection, extremes_.NMinSection,
	extremes_.YMaxSection, extremes_.YMinSection,
	extremes_.ZMaxSection, extremes_.ZMinSection,
	extremes_.MXMaxSection, extremes_.MXMinSection,
	extremes_.MYMaxSection, extremes_.MYMinSection,
	extremes_.MZMaxSection, extremes_.MZMinSection };

			return resultList;

		}
		private List<members_local_deformations_row> getExtremesForBarForces(List<members_local_deformations_row> membersLocalDeformation)
		{

			var extremes_ = membersLocalDeformation.ToList().TakeWhile(v => !v.description.Contains("Extreme")).Aggregate(
			   new
			   {
				   dXMax = membersLocalDeformation.First(),
				   dXMin = membersLocalDeformation.First(),
				   dyMax = membersLocalDeformation.First(),
				   dyMin = membersLocalDeformation.First(),
				   dzMax = membersLocalDeformation.First(),
				   dzMin = membersLocalDeformation.First(),
				   rotXMax = membersLocalDeformation.First(),
				   rotXMin = membersLocalDeformation.First(),
				   rotYMax = membersLocalDeformation.First(),
				   rotYMin = membersLocalDeformation.First(),
				   rotZMax = membersLocalDeformation.First(),
				   rotZMin = membersLocalDeformation.First(),

			   },
(acc, m) => new
{
	dXMax = m.row.displacement_x > acc.dXMax.row.displacement_x ? m : acc.dXMax,
	dXMin = m.row.displacement_x < acc.dXMin.row.displacement_x ? m : acc.dXMin,
	dyMax = m.row.displacement_y > acc.dyMax.row.displacement_y ? m : acc.dyMax,
	dyMin = m.row.displacement_y < acc.dyMin.row.displacement_y ? m : acc.dyMin,
	dzMax = m.row.displacement_z > acc.dzMax.row.displacement_z ? m : acc.dzMax,
	dzMin = m.row.displacement_z < acc.dzMin.row.displacement_z ? m : acc.dzMin,
	rotXMax = m.row.rotation_x > acc.rotXMax.row.rotation_x ? m : acc.rotXMax,
	rotXMin = m.row.rotation_x < acc.rotXMin.row.rotation_x ? m : acc.rotXMin,
	rotYMax = m.row.rotation_y > acc.rotYMax.row.rotation_y ? m : acc.rotYMax,
	rotYMin = m.row.rotation_y < acc.rotYMin.row.rotation_y ? m : acc.rotYMin,
	rotZMax = m.row.rotation_z > acc.rotZMax.row.rotation_z ? m : acc.rotZMax,
	rotZMin = m.row.rotation_z < acc.rotZMin.row.rotation_z ? m : acc.rotZMin
}
			);

			List<members_local_deformations_row> resultList = new List<members_local_deformations_row>() {
	extremes_.dXMax, extremes_.dXMin,
	extremes_.dyMax, extremes_.dyMin,
	extremes_.dzMax, extremes_.dzMin,
	extremes_.rotXMax, extremes_.rotXMin,
	extremes_.rotYMax, extremes_.rotYMin,
	extremes_.rotZMax, extremes_.rotZMin
};

			return resultList;

		}

	}
}


