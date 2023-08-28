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

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.object_types? ToRFEM6(this Type bhType)
        {
            if (bhType == typeof(Node))
            {
                return rfModel.object_types.E_OBJECT_TYPE_NODE;
            }
            else if (bhType == typeof(RFEMNodalSupport))
            {
                return rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT;
            }
            else if (bhType == typeof(RFEMLineSupport))
            {
                return rfModel.object_types.E_OBJECT_TYPE_LINE_SUPPORT;
            }
            else if (bhType == typeof(IMaterialFragment) || bhType.GetInterfaces().Contains(typeof(IMaterialFragment)))
            {
                return rfModel.object_types.E_OBJECT_TYPE_MATERIAL;
            }
            else if (bhType == typeof(ISectionProperty))
            {
                return rfModel.object_types.E_OBJECT_TYPE_SECTION;
            }
            else if (bhType == typeof(RFEMLine))
            {
                return rfModel.object_types.E_OBJECT_TYPE_LINE;
            }
            else if (bhType == typeof(Bar))
            {
                return rfModel.object_types.E_OBJECT_TYPE_MEMBER;
            }
            else if (bhType == typeof(ISurfaceProperty))
            {
                return rfModel.object_types.E_OBJECT_TYPE_THICKNESS;
            }
            else if (bhType == typeof(Panel))
            {
                return rfModel.object_types.E_OBJECT_TYPE_SURFACE;
            }
            else if (bhType == typeof(RFEMLineType))
            {
                return rfModel.object_types.E_OBJECT_TYPE_LINE;
            }
            else if (bhType == typeof(RFEMOpening))
            {
                return rfModel.object_types.E_OBJECT_TYPE_OPENING;
            }
            else if (bhType == typeof(Opening))
            {
                return rfModel.object_types.E_OBJECT_TYPE_OPENING;
            }


            return null;

        }
    }
}
