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

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.member_load ToRFEM6(this BarUniformlyDistributedLoad bhBarLoad)
        {

            var i = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToList();

            member_load rfLoadCase = new rfModel.member_load()
            {
                no = bhBarLoad.GetRFEM6ID(),
                //members_string = (i + 1).ToString(),
                members = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                magnitude = bhBarLoad.Force.Z,
                magnitudeSpecified = true,
                load_is_over_total_length = true,
                load_is_over_total_lengthSpecified = true,
            };


            return rfLoadCase;

        }

        public static rfModel.nodal_load ToRFEM6(this PointLoad bhPointLoad)
        {

            var i = bhPointLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToList();

            nodal_load rfLoadCase = new rfModel.nodal_load()
            {
                no = bhPointLoad.GetRFEM6ID(),
                //members_string = (i + 1).ToString(),
                nodes = bhPointLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                load_direction = nodal_load_load_direction.LOAD_DIRECTION_LOCAL_Z,
                load_directionSpecified = true,
                force_magnitude = bhPointLoad.Force.Length(),
                force_magnitudeSpecified = true,
                components_force_x = bhPointLoad.Force.X,
                components_force_xSpecified=true,
                components_force_y = bhPointLoad.Force.Y,
                components_force_ySpecified = true,
                components_force_z = bhPointLoad.Force.Z,
                components_force_zSpecified = true,
                moment_magnitude = bhPointLoad.Moment.Length(),
                moment_magnitudeSpecified = true,
                components_moment_x = bhPointLoad.Moment.X,
                components_moment_xSpecified = true,
                components_moment_y = bhPointLoad.Moment.Y,
                components_moment_ySpecified = true,
                components_moment_z = bhPointLoad.Moment.Z,
                components_moment_zSpecified = true,

            };


            return rfLoadCase;

        }

        public static rfModel.nodal_load ToRFEM6(this GeometricalLineLoad bhLineLoad)
        {
            

            nodal_load rfLoadCase = new rfModel.nodal_load()
            {
                no = bhLineLoad.GetRFEM6ID(),
                //members_string = (i + 1).ToString(),
                nodes = bhLineLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                load_direction = nodal_load_load_direction.LOAD_DIRECTION_LOCAL_Z,
                load_directionSpecified = true,
                force_magnitude = bhPointLoad.Force.Length(),
                force_magnitudeSpecified = true,
                components_force_x = bhPointLoad.Force.X,
                components_force_xSpecified = true,
                components_force_y = bhPointLoad.Force.Y,
                components_force_ySpecified = true,
                components_force_z = bhPointLoad.Force.Z,
                components_force_zSpecified = true,
                moment_magnitude = bhPointLoad.Moment.Length(),
                moment_magnitudeSpecified = true,
                components_moment_x = bhPointLoad.Moment.X,
                components_moment_xSpecified = true,
                components_moment_y = bhPointLoad.Moment.Y,
                components_moment_ySpecified = true,
                components_moment_z = bhPointLoad.Moment.Z,
                components_moment_zSpecified = true,

            };


            return rfLoadCase;

        }


    }
}
