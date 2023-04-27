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
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SurfaceProperties;


using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.thickness ToRFEM6(this ISurfaceProperty bhSurfaceProperty, IMaterialFragment bhMaterial)
        {
            //Object[] nameAndType = materialTypeAndNameTranslater(bhMateraial);

            rfModel.thickness rfThickness = new rfModel.thickness()
            {
                no = bhSurfaceProperty.GetRFEM6ID(),
                material = bhMaterial.GetRFEM6ID(),
                name = bhSurfaceProperty.Name,
                materialSpecified = true,
                type = rfModel.thickness_type.TYPE_UNIFORM,
                typeSpecified = true,
                uniform_thickness = BH.Engine.Structure.Query.ITotalThickness(bhSurfaceProperty),
                uniform_thicknessSpecified = true,
            };

            //thickness slabThickness = new thickness
            //{
            //    no = 1,
            //    material = materialConcrete.no,
            //    materialSpecified = true,
            //    type = thickness_type.TYPE_UNIFORM,
            //    typeSpecified = true,
            //    uniform_thickness = 0.5,
            //    uniform_thicknessSpecified = true,
            //};



            //// opening
            //node openingNodeOne = new node()
            //{
            //    no = nodeID,
            //    coordinates = new vector_3d()
            //    {
            //        x = firstNode.FirstOrDefault().coordinate_1 + 1.0,
            //        y = firstNode.FirstOrDefault().coordinate_1 + 1.0,
            //        z = -hc,
            //    },
            //    comment = "opening node",

            //};



            return rfThickness;

        }



    }
}
