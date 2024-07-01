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

            //If no node ids are provided, then we will extract the results for all nodes
            if (nodeIds.Count == 0)
            {
                HashSet<int> nodalSupportNodeIDs = new HashSet<int>();
                rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
                IEnumerable<rfModel.nodal_support> foundSupports = numbers.ToList().Select(n => m_Model.get_nodal_support(n.no));
                foundSupports.ToList().ForEach(s => s.nodes.ToList().ForEach(n => nodalSupportNodeIDs.Add(n)));
                nodeIds = nodalSupportNodeIDs.ToList();
                BH.Engine.Base.Compute.RecordWarning("No Node Ids were provided for the extraction of Node Reactions, all supports were chosen istead.");
            }



            m_Model.calculate_all(true);
           
            foreach (int lc in loadCaseIds)
            {

                foreach (int n in nodeIds)
                {

                    nodes_support_forces_row[] nodes_support_forces_rows = m_Model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, lc, n);

                    var supportResultReaction = nodes_support_forces_rows.First().row;
                    double fxValue = Double.Parse(supportResultReaction.support_force_p_x.value);
                    double fyValue = Double.Parse(supportResultReaction.support_force_p_y.value);
                    double fzValue = Double.Parse(supportResultReaction.support_force_p_z.value);
                    double mxValue = Double.Parse(supportResultReaction.support_moment_m_x.value);
                    double myValue = supportResultReaction.support_moment_m_y;
                    double mzValue = supportResultReaction.support_moment_m_z;

                    NodeReaction nodeReaction = new NodeReaction(n, lc, 0, 0, oM.Geometry.Basis.XY, fxValue, fyValue, fzValue, mxValue, myValue, mzValue);

                    resultList.Add(nodeReaction);



                }

            }

            return resultList;

        }

    }
}

