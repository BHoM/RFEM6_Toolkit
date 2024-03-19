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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.node ToRFEM6(this Node node)
        {
            Object bhComment = "";
            if (node.CustomData.Count != 0)
            {
                node.CustomData.TryGetValue("Comment", out bhComment);
            }


            rfModel.node rfNode = new rfModel.node()
            {
                no = node.GetRFEM6ID(),
                coordinates = new rfModel.vector_3d() { x = node.Position.X, y = node.Position.Y, z = node.Position.Z },
                coordinate_system_type = rfModel.node_coordinate_system_type.COORDINATE_SYSTEM_CARTESIAN,
                coordinate_system_typeSpecified = true,
                comment = (String)(bhComment == null || bhComment.Equals("") ? "" : $"BHComment:{bhComment}"),
            };

            return rfNode;

        }

    }
}

