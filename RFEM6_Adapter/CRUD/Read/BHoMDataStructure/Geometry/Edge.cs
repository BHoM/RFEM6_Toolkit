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
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Edge> ReadEdges(List<string> ids = null)
        {

            List<Edge> edgeList = new List<Edge>();

            List<RFEMLine> rfemLineList = GetCachedOrRead<RFEMLine>();

            Dictionary<int, RFEMLineSupport> supportMap = this.GetCachedOrReadAsDictionary<int, RFEMLineSupport>();

            foreach (RFEMLine rfemLine in rfemLineList)
            {

                Edge edge = rfemLine.FromRFEMLineToEdge();

                RFEMLineSupport support;
                if (supportMap.TryGetValue(rfemLine.supportID, out support))
                    edge.Support = support.Constraint;


                edgeList.Add(edge);
            }

            return edgeList;

        }

    }
}


