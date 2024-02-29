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

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<RFEM6NodalSupportReaction> ReadNodeReaction(List<string> ids = null)
        {
            
            List<RFEM6NodalSupportReaction> reactions = new List<RFEM6NodalSupportReaction>();
           
            rfModel.calculation_result result = m_Model.calculate_all(false);

            //Checking if Calculation was successful
            if (!result.succeeded) { return null; }

            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT);
            IEnumerable<rfModel.nodal_support> foundSupports = numbers.ToList().Select(n => m_Model.get_nodal_support(n.no));

            Dictionary<int, Node> nodeMap = this.GetCachedOrReadAsDictionary<int, Node>();

            List<int> foundSupportListID=foundSupports.SelectMany(s => s.nodes).ToList();

            foreach (int s in foundSupportListID)
            {
                //if(s.nodes.Length==0) { continue; }
                

                nodes_support_forces_row[] nodes_support_forces_rows = m_Model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, 1, s);

                //nodes_support_forces_row[] nodes_support_forces_rowsd = m_Model.get_results_for_nodes_support_forces(case_object_types.E_OBJECT_TYPE_LOAD_CASE, 1, s);


                var supportResultReaction=nodes_support_forces_rows.First().row;
                double fxValue = Double.Parse(supportResultReaction.support_force_p_x.value);
                double fyValue = Double.Parse(supportResultReaction.support_force_p_y.value);
                double fzValue = Double.Parse(supportResultReaction.support_force_p_z.value);
                double mxValue = Double.Parse(supportResultReaction.support_moment_m_x.value);
                double myValue = supportResultReaction.support_moment_m_y;
                double mzValue = supportResultReaction.support_moment_m_z;

                RFEM6NodalSupportReaction reaction = new RFEM6NodalSupportReaction();
                reaction.Forces=new double[] { fxValue, fyValue, fzValue };
                reaction.Moments = new double[] { mxValue, myValue, mzValue };

                double[] forces = new double[] { fxValue, fyValue, fzValue };
                double[] moments = new double[] { mxValue, myValue, mzValue };
                reaction.Node = nodeMap[s];
                //s.nodes.ToList().ForEach(n =>
                //{
                //    Node node = supportMap[n];
                //    node.Reactions.Add(reaction);
                //});

                reactions.Add(reaction);

            }

            return reactions;
        }

    }
}

