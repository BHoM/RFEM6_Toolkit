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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;
using Newtonsoft.Json.Linq;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.member_type ToRFEM6(this BarFEAType feaType)
        {


            switch (feaType)
            {
                case BarFEAType.Axial:
                    return rfModel.member_type.TYPE_TRUSS;
                case BarFEAType.CompressionOnly:
                    return rfModel.member_type.TYPE_COMPRESSION;
                case BarFEAType.TensionOnly:
                    return rfModel.member_type.TYPE_TENSION;
                default:
                    return rfModel.member_type.TYPE_BEAM;
            }


        }

    }
}


