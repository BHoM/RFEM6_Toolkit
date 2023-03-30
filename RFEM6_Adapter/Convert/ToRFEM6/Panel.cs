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
using BH.oM.Geometry;

using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.surface ToRFEM6(this Panel bhPanel)
        {
            
            List<int> edgeIdList=new List<int>();
            bhPanel.ExternalEdges.ForEach(e=>edgeIdList.Add(e.GetRFEM6ID()));


            rfModel.surface rfSurface = new rfModel.surface
            {

                no = bhPanel.GetRFEM6ID(),
                material = bhPanel.Property.Material.GetRFEM6ID(),
                materialSpecified = true,
                thickness = bhPanel.Property.GetRFEM6ID(),
                boundary_lines = edgeIdList.ToArray(),
                //type = surface_type.TYPE_STANDARD,
                //typeSpecified = true,
                //geometry = surface_geometry.GEOMETRY_PLANE,
                //geometrySpecified = true,
                //concrete_durability_top = concrete_Durability.no,
                //concrete_durability_topSpecified = true,
                //concrete_durability_bottom = concrete_Durability.no,
                //concrete_durability_bottomSpecified = true,
                //reinforcement_direction_top = Reinforcement_Direction.no,
                //reinforcement_direction_topSpecified = true,
                //reinforcement_direction_bottom = Reinforcement_Direction.no,
                //reinforcement_direction_bottomSpecified = true,
                //surface_reinforcements = new int[] { Surface_Reinforcement.no, Surface_ReinforcementMesh.no },
                //surface_concrete_design_sls_configuration = 1,
                //surface_concrete_design_sls_configurationSpecified = true,
                //surface_concrete_design_uls_configuration = 1,
                //surface_concrete_design_uls_configurationSpecified = true,
                //deflection_check_surface_type = surface_deflection_check_surface_type.DEFLECTION_CHECK_SURFACE_TYPE_DOUBLE_SUPPORTED,
                //deflection_check_surface_typeSpecified = true,
                //deflection_check_displacement_reference = surface_deflection_check_displacement_reference.DEFLECTION_CHECK_DISPLACEMENT_REFERENCE_DEFORMED_USER_DEFINED_REFERENCE_PLANE,
                //deflection_check_reference_length_z_definition_type = surface_deflection_check_reference_length_z_definition_type.DEFLECTION_CHECK_REFERENCE_LENGTH_DEFINITION_TYPE_BY_MAXIMUM_BOUNDARY_LINE,
                //deflection_check_reference_length_z_definition_typeSpecified = true,
            };


                 //surface concreteSlab = new()
                 //{
                 //    no = 1,
                 //    material = materialConcrete.no,
                 //    materialSpecified = true,
                 //    thickness = slabThickness.no,
                 //    boundary_lines = new int[] { slabLine.no },
                 //    type = surface_type.TYPE_STANDARD,
                 //    typeSpecified = true,
                 //    geometry = surface_geometry.GEOMETRY_PLANE,
                 //    geometrySpecified = true,
                 //    concrete_durability_top = concrete_Durability.no,
                 //    concrete_durability_topSpecified = true,
                 //    concrete_durability_bottom = concrete_Durability.no,
                 //    concrete_durability_bottomSpecified = true,
                 //    reinforcement_direction_top = Reinforcement_Direction.no,
                 //    reinforcement_direction_topSpecified = true,
                 //    reinforcement_direction_bottom = Reinforcement_Direction.no,
                 //    reinforcement_direction_bottomSpecified = true,
                 //    surface_reinforcements = new int[] { Surface_Reinforcement.no, Surface_ReinforcementMesh.no },
                 //    surface_concrete_design_sls_configuration = 1,
                 //    surface_concrete_design_sls_configurationSpecified = true,
                 //    surface_concrete_design_uls_configuration = 1,
                 //    surface_concrete_design_uls_configurationSpecified = true,
                 //    deflection_check_surface_type = surface_deflection_check_surface_type.DEFLECTION_CHECK_SURFACE_TYPE_DOUBLE_SUPPORTED,
                 //    deflection_check_surface_typeSpecified = true,
                 //    deflection_check_displacement_reference = surface_deflection_check_displacement_reference.DEFLECTION_CHECK_DISPLACEMENT_REFERENCE_DEFORMED_USER_DEFINED_REFERENCE_PLANE,
                 //    deflection_check_reference_length_z_definition_type = surface_deflection_check_reference_length_z_definition_type.DEFLECTION_CHECK_REFERENCE_LENGTH_DEFINITION_TYPE_BY_MAXIMUM_BOUNDARY_LINE,
                 //    deflection_check_reference_length_z_definition_typeSpecified = true,
                 //};



            return rfSurface;

        }

    }
}
