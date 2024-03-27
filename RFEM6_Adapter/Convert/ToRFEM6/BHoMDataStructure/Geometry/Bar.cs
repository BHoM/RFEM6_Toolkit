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
using BH.Engine.Base;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static rfModel.member ToRFEM6(this Bar bar)
        {
            Object bhComment="";

            if (bar.CustomData.Count != 0)
            {
                bar.CustomData.TryGetValue("Comment", out bhComment);
            }




            rfModel.member rfMember = new rfModel.member()
            {
                no = bar.GetRFEM6ID(),
                type = bar.FEAType.ToRFEM6(),
                typeSpecified = true,
                line = bar.FindFragment<RFEMLine>().GetRFEM6ID(),
                lineSpecified = true,
                section_start = bar.SectionProperty.GetRFEM6ID(),
                section_startSpecified = true,
                section_endSpecified = true,
                comment = (String)(bhComment == null || bhComment.Equals("") ? "" : $"BHComment:{bhComment}"),

            };

            return rfMember;

        }

    }
}

