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
using BH.oM.Structure.Loads;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static Type FromRFEM(rfModel.object_types rfType)
        {

            if (rfType == rfModel.object_types.E_OBJECT_TYPE_NODE)
            {
                return typeof(Node);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT)
            {
                return typeof(RFEMNodalSupport);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_LINE_SUPPORT)
            {
                return typeof(RFEMLineSupport);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_MATERIAL)
            {
                return typeof(IMaterialFragment);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_SECTION)
            {
                return typeof(ISectionProperty);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_THICKNESS)
            {
                return typeof(ISurfaceProperty);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_SURFACE)
            {
                return typeof(Panel);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_MEMBER_HINGE)
            {
                return typeof(RFEMHinge);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_OPENING)
            {
                return typeof(RFEMOpening);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_LOAD_CASE)
            {
                return typeof(Loadcase);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_MEMBER_LOAD)
            {
                return typeof(BarUniformlyDistributedLoad);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_NODAL_LOAD)
            {
                return typeof(PointLoad);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_LINE_LOAD)
            {
                return typeof(GeometricalLineLoad);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_SURFACE_LOAD)
            {
                return typeof(AreaUniformlyDistributedLoad);
            }

            return null;
        }

    }
}
