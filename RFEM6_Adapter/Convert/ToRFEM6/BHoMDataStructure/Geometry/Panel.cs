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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Geometry;

using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.surface ToRFEM6(this Panel bhPanel)
        {

            List<int> edgeIdList = new List<int>();
            bhPanel.ExternalEdges.ForEach(e => edgeIdList.Add(e.GetRFEM6ID()));
            Object bhComment = "";

            if (bhPanel.CustomData.Count != 0)
            {
                bhPanel.CustomData.TryGetValue("Comment", out bhComment);
            }



            rfModel.surface rfSurface = new rfModel.surface
            {

                no = bhPanel.GetRFEM6ID(),
                thickness = bhPanel.Property.GetRFEM6ID(),
                thicknessSpecified = true,
                boundary_lines = edgeIdList.ToArray(),
                type = surface_type.TYPE_STANDARD,
                typeSpecified = true,
                comment = (String)(bhComment == null || bhComment.Equals("") ? "" : $"BHComment:{bhComment}"),

            };

            return rfSurface;

        }

    }
}



