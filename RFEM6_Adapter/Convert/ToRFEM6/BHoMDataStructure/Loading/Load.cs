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

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;
using BH.oM.Structure.Loads;
using BH.oM.Adapter.Commands;
using Dlubal.WS.Rfem6.Model;
using BH.Engine.Geometry;
using BH.oM.Geometry;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.member_load ToRFEM6(this BarUniformlyDistributedLoad bhBarLoad, member_load_load_type nodalLoadType, int id)
        {

            var i = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToList();
            
            member_load_load_direction loadDirecteion;
            Vector orientationVector = nodalLoadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force : bhBarLoad.Moment;
            if (orientationVector.X != 0) { loadDirecteion = member_load_load_direction.LOAD_DIRECTION_LOCAL_X; }
            else if (orientationVector.Y != 0) { loadDirecteion = member_load_load_direction.LOAD_DIRECTION_LOCAL_Y; }
            else { loadDirecteion = member_load_load_direction.LOAD_DIRECTION_LOCAL_Z ; }

            member_load rfLoadCase = new rfModel.member_load()
            {
                no = id,
                members = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                load_type=nodalLoadType,
                load_typeSpecified=true,
                load_direction= loadDirecteion,
                load_directionSpecified = true,
                magnitude = nodalLoadType==member_load_load_type.LOAD_TYPE_FORCE? bhBarLoad.Force.Length(): bhBarLoad.Moment.Length(),
                magnitudeSpecified = true,
                load_is_over_total_length = true,
                load_is_over_total_lengthSpecified = true,
            };


            return rfLoadCase;

        }

        public static rfModel.nodal_load ToRFEM6(this PointLoad bhPointLoad, nodal_load_load_type nodalLoadType, int id)
        {

            nodal_load_load_direction loadDirecteion;
            Vector orientationVector = nodalLoadType == nodal_load_load_type.LOAD_TYPE_FORCE? bhPointLoad.Force : bhPointLoad.Moment;
            if (orientationVector.X!=0) { loadDirecteion = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U; }
            else if (orientationVector.Y != 0) { loadDirecteion = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V; }
            else { loadDirecteion = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W; }


            nodal_load rfLoadCase = new rfModel.nodal_load()
            {
                no = id,
                nodes = bhPointLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                //load_direction = loadDirecteion,
                //load_directionSpecified = true,
                //force_magnitude = bhPointLoad.Force.Length(),
                //force_magnitudeSpecified = true,
                //moment_magnitude = bhPointLoad.Moment.Length(),
                //moment_magnitudeSpecified = true,
                //load_type = nodalLoadType,
                components_force_x= bhPointLoad.Force.X,
                components_force_xSpecified=true,
                components_force_y = -bhPointLoad.Force.Y,   
                components_force_ySpecified = true, 
                components_force_z = -bhPointLoad.Force.Z,   
                components_force_zSpecified = true, 
                components_moment_x = bhPointLoad.Moment.X,
                components_moment_xSpecified = true,
                components_moment_y = -bhPointLoad.Moment.Y,
                components_moment_ySpecified = true,
                components_moment_z = -bhPointLoad.Moment.Z,
                components_moment_zSpecified = true,
                load_type = nodal_load_load_type.LOAD_TYPE_COMPONENTS,
                load_typeSpecified = true,
            };


            return rfLoadCase;

        }

        //public static rfModel.line_load ToRFEM6(this GeometricalLineLoad bhLineLoad, List<int> lineNoList)
        //{

        //    line_load rfLineLoad = new rfModel.line_load()
        //    {
        //        no = bhLineLoad.GetRFEM6ID(),
        //        lines = lineNoList.ToArray(),
        //        load_case=bhLineLoad.Loadcase.GetRFEM6ID(),
        //        load_caseSpecified=true,
        //        load_distribution = line_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
        //        load_distributionSpecified = true,
        //        magnitude = bhLineLoad.ForceA.Length(),
        //        magnitudeSpecified = true,
        //        reference_to_list_of_lines=false,
        //        reference_to_list_of_linesSpecified=true,
        //        is_generated=false,
        //        is_generatedSpecified=true,
        //        load_direction = line_load_load_direction.LOAD_DIRECTION_LOCAL_Z,
        //        load_directionSpecified = true,
        //        load_is_over_total_length = true,
        //        load_is_over_total_lengthSpecified = true,
        //        load_type= line_load_load_type.LOAD_TYPE_FORCE,
        //        load_typeSpecified=true,
        //    };


        //    return rfLineLoad;

        //}

        //public static rfModel.surface_load ToRFEM6(this AreaUniformlyDistributedLoad bhAreaLoad)
        //{

        //    surface_load rfSurfaceLoad = new rfModel.surface_load()
        //    {
        //        no = bhAreaLoad.GetRFEM6ID(),
        //        surfaces = bhAreaLoad.Objects.Elements.ToList().Select(x => (x as Panel).GetRFEM6ID()).ToArray(),
        //        //surfaces = new int[] {1},
        //        load_case = bhAreaLoad.Loadcase.GetRFEM6ID(),
        //        load_caseSpecified = true,
        //        load_distribution = surface_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
        //        load_distributionSpecified = true,
        //        magnitude_force_u = bhAreaLoad.Pressure.Length(),
        //        magnitude_force_uSpecified = true,
        //        uniform_magnitude = bhAreaLoad.Pressure.Length(),
        //        uniform_magnitudeSpecified = true,
        //        is_generated = false,
        //        is_generatedSpecified = true,
        //        load_direction = surface_load_load_direction.LOAD_DIRECTION_LOCAL_Z,
        //        load_directionSpecified = true,
        //        load_type = surface_load_load_type.LOAD_TYPE_FORCE,
        //        load_typeSpecified = true,
        //    };


        //    return rfSurfaceLoad;

        //}


    }
}
