/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        private bool CreateCollection(IEnumerable<Node> bhNodes)
        {
            //NOTE:A geometric object has, in general, a parent_no = 0. The parent_no parameter becomes significant for example with loads.
            foreach (Node bhNode in bhNodes)
            {
                rfModel.node rfNode = bhNode.ToRFEM6();

                if (bhNode.Support != null)
                {

                    //TODO: 
                    //API Call could possibly be reduced by combining the Node and Constraing6DOF Push...Problem, RFEM6 Needs Refernces in both Directions Nodes<->Nodal Support but BHoM has only a reference in o
                    var rfSupport = model.get_nodal_support(rfNode.support);
                    HashSet<int> collectionOFSupporNo = rfSupport.nodes.ToHashSet();
                    collectionOFSupporNo.Add(rfNode.no);
                    rfSupport.nodes= collectionOFSupporNo.ToArray();
                    model.set_nodal_support(rfSupport);

                }

                model.set_node(rfNode);
            }
            return true;
        }


    }
}
