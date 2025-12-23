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

		public IEnumerable<IResult> ReadResults(NodeResultRequest request, ActionConfig actionConfig)
		{



			List<int> nodeIds = request.ObjectIds.Select(s => Int32.Parse(s.ToString())).ToList();
			List<int> loadCaseIds = request.Cases.Select(s => Int32.Parse(s.ToString())).ToList();

			switch (request.ResultType)
			{

				case NodeResultType.NodeReaction:

					//m_Model.calculate_all(true);
					var result = ExtractNodeReaction(nodeIds, loadCaseIds);
					return result;

				default:

					BH.Engine.Base.Compute.RecordWarning("WOOOOPS....Seems like Extraction has only been implemented for Node Reactions!");

					break;

			}

			return null;
		}

		private IEnumerable<IResult> ExtractNodeReaction(List<int> nodeIds, List<int> loadCaseIds)
		{

			List<IResult> resultList = new List<IResult>();
			object_location[] filter = null;

			rfModel.object_with_children[] nodalSupportObjWitheChildern = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
			nodalSupportObjWitheChildern = nodalSupportObjWitheChildern.ToList().Where(n => n.no != 0).ToArray();
			IEnumerable<rfModel.nodal_support> nodalSupport = nodalSupportObjWitheChildern.Length >= 1 ? nodalSupportObjWitheChildern.ToList().Select(n => m_Model.get_nodal_support(n.no)) : new List<rfModel.nodal_support>();

			List<int> nodalSupportNo = nodalSupport.ToList().Select(n => n.no).ToList();

			if (nodalSupportNo.Count != 0)
			{

				filter = nodalSupportNo.Select(n => new object_location() { type = object_types.E_OBJECT_TYPE_NODAL_SUPPORT, no = n, parent_no = 0 }).ToArray();
			}
			else
			{
				BH.Engine.Base.Compute.RecordWarning("There no nodal Support that have been defined in RFEM6");
				return resultList;
			}


			m_Model.calculate_all(true);

			foreach (int lc in loadCaseIds)
			{

				nodes_support_forces_row[] res_all = m_Model.get_results_for_nodes_support_forces(
					case_object_types.E_OBJECT_TYPE_LOAD_CASE,
					lc,
					filter
					);


				//Gather all ids of Nodes that are linked to a Nodal support
				HashSet<int> idsOfAllNodesLikedToNodalSupport = res_all.Select(z => z.row.node_no).ToHashSet();

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

					NodeReaction nodeReaction = new NodeReaction(r.row.node_no, lc, 0, 0, oM.Geometry.Basis.XY, fxValue, fyValue, fzValue, mxValue, myValue, mzValue);
					resultList.Add(nodeReaction);
				}


			}

			return resultList;

		}

	}
}



