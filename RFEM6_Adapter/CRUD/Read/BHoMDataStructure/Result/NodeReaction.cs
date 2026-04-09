/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Adapter;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Structure.Loads;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;
using BH.oM.Structure.Results;

namespace BH.Adapter.RFEM6
{
#if RFEM6_8_2
	public partial class RFEM6AdapterV8_2
#elif RFEM6_12_11
	public partial class RFEM6AdapterV12_11
#else
	public partial class RFEM6Adapter
#endif
	{
		/// <summary>
		/// Reads results from RFEM6 based on the provided <see cref="NodeResultRequest"/>.
		/// Resolves node IDs and load cases from the request, then dispatches to the appropriate extraction method.
		/// </summary>
		/// <param name="request">The request defining which nodes, load cases, and result type to extract.</param>
		/// <param name="actionConfig">Adapter action configuration.</param>
		/// <returns>A collection of <see cref="IResult"/> objects, or an empty list if the result type is not supported.</returns>
		public IEnumerable<IResult> ReadResults(NodeResultRequest request, ActionConfig actionConfig)
		{

			List<int> nodeIds = request.ObjectIds.Select(s => Int32.Parse(s.ToString())).ToList();
			Dictionary<int, case_object_types> caseIDToTypeMap = new Dictionary<int, case_object_types>();
			foreach (object c in request.Cases)
			{

				if (c is Loadcase loadcase)
				{

					caseIDToTypeMap.Add(Int32.Parse(loadcase.Number.ToString()), case_object_types.E_OBJECT_TYPE_LOAD_CASE);
				}
				else if (c is LoadCombination loadCombination)
				{

					caseIDToTypeMap.Add(Int32.Parse(loadCombination.Number.ToString()), case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION);

				}

			}

			switch (request.ResultType)
			{

				case NodeResultType.NodeReaction:

					var result = ExtractNodeReaction(nodeIds, caseIDToTypeMap);
					return result;

				default:

					BH.Engine.Base.Compute.RecordWarning("WOOOOPS....Seems like Extraction has only been implemented for Node Reactions!");

					break;

			}

			return new List<IResult>();
		}
		/// <summary>
		/// Extracts nodal support reaction forces and moments from RFEM6 for the given nodes and load cases.
		/// Triggers a model calculation, then retrieves support forces for all nodal supports.
		/// Only nodes linked to a nodal support will return results; others are skipped with a warning.
		/// </summary>
		/// <param name="nodeIds">List of node IDs to extract reactions for. If empty, all nodes are used.</param>
		/// <param name="loadCaseIds">Map of load case/combination IDs to their RFEM6 object type.</param>
		/// <returns>A list of <see cref="NodeReaction"/> results.</returns>
		private IEnumerable<IResult> ExtractNodeReaction(List<int> nodeIds, Dictionary<int, case_object_types> loadCaseIds)
		{

			List<IResult> resultList = new List<IResult>();


			//Get all nodal supports and filter out the ones with no number (no = 0)
			rfModel.object_with_children[] nodalSupportObjWithChildern = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
			nodalSupportObjWithChildern = nodalSupportObjWithChildern.Where(n => n.no != 0).ToArray();
			IEnumerable<rfModel.nodal_support> nodalSupport = nodalSupportObjWithChildern.Length >= 1 ? nodalSupportObjWithChildern.Select(n => m_Model.get_nodal_support(n.no)) : new List<rfModel.nodal_support>();

			List<int> nodalSupportNo = nodalSupport.ToList().Select(n => n.no).ToList();

			nodalSupport.First().nodes.ToList();

			//Filter to get only nodal supports that are linked to nodes. This is because only those will have support forces and moments results.
			object_location[] filter = null;
			if (nodalSupportNo.Count != 0)
			{

				filter = nodalSupportNo.Select(n => new object_location() { type = object_types.E_OBJECT_TYPE_NODAL_SUPPORT, no = n, parent_no = 0 }).ToArray();
			}
			else
			{
				BH.Engine.Base.Compute.RecordWarning("There no nodal Support that have been defined in RFEM6");
				return resultList;
			}

			//Calculate the model to make sure that results are up to date. This is important especially if the user has made changes to the model and has not calculated it yet.
			m_Model.calculate_all(true);


			foreach (var lc in loadCaseIds)
			{

				nodes_support_forces_row[] res_all = m_Model.get_results_for_nodes_support_forces(
					lc.Value,
					lc.Key,
					filter
					);


				//Gather all ids of Nodes that are linked to a Nodal support
				HashSet<int> idsOfAllNodesLikedToNodalSupport = res_all.Select(z => z.row.node_no).ToHashSet();

				//If no node ids have been provided in the request, we will extract results for all nodes that are linked to a nodal support. Otherwise, we will only extract results for the node ids provided in the request.
				nodeIds = nodeIds.Count == 0 ? nodalSupport.SelectMany(s => s.nodes).Distinct().ToList() : nodeIds;

				for (int i = 0; i < nodeIds.Count; i++)
				{
					//Is node id Linked to an id linked to Nodal Support
					if (!idsOfAllNodesLikedToNodalSupport.Contains(nodeIds[i]))
					{
						BH.Engine.Base.Compute.RecordWarning(String.Format("There is no node id {0} linked to an Nodal Support", nodeIds[i]));
						continue;
					}


					var r = res_all.First(k => k.row.node_no.Equals(nodeIds[i]));

					double fxValue = r.row.support_force_p_x;
					double fyValue = r.row.support_force_p_y;
					double fzValue = r.row.support_force_p_z;
					double mxValue = r.row.support_moment_m_x;
					double myValue = r.row.support_moment_m_y;
					double mzValue = r.row.support_moment_m_z;

					NodeReaction nodeReaction = new NodeReaction(r.row.node_no, lc.Key, 0, 0, oM.Geometry.Basis.XY, fxValue, fyValue, fzValue, mxValue, myValue, mzValue);
					resultList.Add(nodeReaction);
				}


			}

			return resultList;

		}

	}
}



