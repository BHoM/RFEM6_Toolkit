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


			//Warnings 
			BH.Engine.Base.Compute.RecordWarning($"Divisions are set to {request.Divisions}. Division functionality has not been implemented yet in RFEM6_Toolkit. Currently, the number of divisions depends solely on what RFEM6 provides and can vary significantly based on the load type on the corresponding Bar/Member and the DivisionType.");

			if (request.Cases.Where(c=> c is string).ToList().Count>0) { BH.Engine.Base.Compute.RecordWarning("At least one instance of the Case input is neither of type LoadCase nor LoadCombination. The set CaseIds will be assumed to be LoadCase IDs for now."); };

			//RFEM Specific Stuff
			m_Model.use_detailed_member_results(true);

			// Loading of Member And LoadCase Ids
			List<int> memberIds = request.ObjectIds.Select(s => Int32.Parse(s.ToString())).ToList();

			// Definition of Object Locations for Members
			object_location[] objectLocations = memberIds.Select(n => new object_location() { type = object_types.E_OBJECT_TYPE_MEMBER, no = n, parent_no = 0 }).ToArray();

			//ResultList
			List<IResult> resultList = new List<IResult>();


			foreach (var c in request.Cases)
			{
				int cId = 0;

				if (c is Loadcase)
				{
					cId = (c as Loadcase).Number;

				}
				else if (c is LoadCombination)
				{

					cId = (c as LoadCombination).Number;

				}
				else
				{

					cId = Int32.Parse(c.ToString());
				}



				//Loading resulst from RFEM
				INotifyPropertyChanged[] barResults = new INotifyPropertyChanged[1];

				var loadingType = c is LoadCombination ? case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION : case_object_types.E_OBJECT_TYPE_LOAD_CASE;

				switch (request.ResultType)
				{

					case BarResultType.BarForce:
						barResults = m_Model.get_results_for_members_internal_forces(loadingType, cId, objectLocations, axes_type.MEMBER_AXES);
						break;
					case BarResultType.BarDisplacement:
						barResults = m_Model.get_results_for_members_global_deformations(loadingType, cId, objectLocations);
						break;
					case BarResultType.BarDeformation:
						barResults = m_Model.get_results_for_members_local_deformations(loadingType, cId, objectLocations, axes_type.MEMBER_AXES);
						break;
					case BarResultType.BarStrain:
						barResults = m_Model.get_results_for_members_strains(loadingType, cId, objectLocations, axes_type.MEMBER_AXES);
						break;
					default:
						BH.Engine.Base.Compute.RecordError($"The pull result types of type {request.ResultType} has not been developed Yet");
						return null;

				}

				//Grouping of Internal Forces grouped by Member No
				var memberInternalForceGroup = barResults.GroupBy(r => Int32.Parse(r.PropertyValue("row.member_no").ToString()));


				//Results processing memberwise
				foreach (IGrouping<int, INotifyPropertyChanged> member in memberInternalForceGroup)
				{
					//Ignore Results that do now have a valied ID
					if (member.Key == 0) continue;

					//Determine Member length
					String lengthAsString = member.ToList()[0].PropertyValue("row.specification").ToString().Split(new[] { "L : ", " m" }, StringSplitOptions.None)[1];
					double memberLength = double.Parse(lengthAsString, CultureInfo.InvariantCulture);// Member Length in SI unit m;

					var memberSegmentValues = member.ToList();

					//If we are looking for exterme Values
					if (request.DivisionType == DivisionType.ExtremeValues)
					{

						memberSegmentValues = member.SkipWhile(m => !m.PropertyValue("description").ToString().Contains("Extremes")).ToList();
						memberSegmentValues = memberSegmentValues.TakeWhile(m => !m.PropertyValue("description").ToString().Contains("Total")).ToList();

					}

					else
					{
						memberSegmentValues = memberSegmentValues.TakeWhile(m => !m.PropertyValue("description").ToString().Contains("Extremes")).ToList();

					}

					int corrective = (request.ResultType == BarResultType.BarDisplacement || request.ResultType == BarResultType.BarDeformation) ? 0 : 2;
					// important do to enable processing of Deformations due to the |u|
					if (memberSegmentValues?.First()?.PropertyValue("row.deformation_label")?.ToString()?.Contains("|u|") ?? false)
					{
						memberSegmentValues = memberSegmentValues.Skip(2).ToList();
						//corrective = 0;
					}

					//Conversion for every segment of member
					foreach (var e in memberSegmentValues)
					{

						var location = Double.Parse(e.PropertyValue("row.location").ToString());
						var memberNumber = Int32.Parse(e.PropertyValue("row.member_no").ToString());
						var props = e.PropertyValue("row").GetType().GetProperties();
						List<int> accesList = new List<int>() { 10 - corrective, 12 - corrective, 14 - corrective, 16 - corrective, 18 - corrective, 20 - corrective };
						var accessedlist = accesList.Select(a => props.ToList()[a]);
						Dictionary<string, double> val = new[] {
							(10-corrective, "x"), (12-corrective, "y"), (14-corrective, "z"),
							(16-corrective, "rx"), (18-corrective, "ry"), (20-corrective, "rz")
						}.ToDictionary(p => p.Item2, p => Double.Parse(
							e.PropertyValue($"row.{props[p.Item1].Name}").ToString()
						));

						var result = val.Values.ToList().FromRFEM(cId, memberLength, location, memberNumber, request.ResultType);
						resultList.Add(result);
					}

				}
			}

			return resultList;

		}

	}
}


