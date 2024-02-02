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

            double loadMagintude;
            member_load_load_direction loadDirecteion;
            Vector orientationVector = nodalLoadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force : bhBarLoad.Moment;

            if (orientationVector.X != 0)
            {
                loadDirecteion = bhBarLoad.Projected ? member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED : member_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                loadMagintude = nodalLoadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force.Length() : bhBarLoad.Moment.Length();
            }
            else if (orientationVector.Y != 0)
            {
                loadDirecteion = bhBarLoad.Projected ? member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED : member_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                loadMagintude = nodalLoadType == member_load_load_type.LOAD_TYPE_FORCE ? -1 * bhBarLoad.Force.Length() : -1 * bhBarLoad.Moment.Length();

            }
            else
            {
                loadDirecteion = bhBarLoad.Projected ? member_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED : member_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                loadMagintude = nodalLoadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force.Length() : bhBarLoad.Moment.Length();
            }

            member_load rfLoadCase = new rfModel.member_load()
            {
                no = id,
                comment=bhBarLoad.Name,
                members = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                load_type = nodalLoadType,
                load_typeSpecified = true,
                load_direction = loadDirecteion,
                load_directionSpecified = true,
                magnitude = loadMagintude,
                magnitudeSpecified = true,
                load_is_over_total_length = true,
                load_is_over_total_lengthSpecified = true,
            };


            return rfLoadCase;

        }

        public static rfModel.nodal_load ToRFEM6(this PointLoad bhPointLoad, nodal_load_load_type nodalLoadType, int id)
        {

            nodal_load_load_direction loadDirecteion;
            Vector orientationVector = nodalLoadType == nodal_load_load_type.LOAD_TYPE_FORCE ? bhPointLoad.Force : bhPointLoad.Moment;
            if (orientationVector.X != 0) { loadDirecteion = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U; }
            else if (orientationVector.Y != 0) { loadDirecteion = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V; }
            else { loadDirecteion = nodal_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W; }


            nodal_load rfLoadCase = new rfModel.nodal_load()
            {

                no = id,
                comment=bhPointLoad.Name,
                nodes = bhPointLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                //load_direction = loadDirecteion,
                //load_directionSpecified = true,
                //force_magnitude = bhPointLoad.Force.Length(),
                //force_magnitudeSpecified = true,
                //moment_magnitude = bhPointLoad.Moment.Length(),
                //moment_magnitudeSpecified = true,
                //load_type = nodalLoadType,
                components_force_x = bhPointLoad.Force.X,
                components_force_xSpecified = true,
                components_force_y = bhPointLoad.Force.Y,
                components_force_ySpecified = true,
                components_force_z = bhPointLoad.Force.Z,
                components_force_zSpecified = true,
                components_moment_x = bhPointLoad.Moment.X,
                components_moment_xSpecified = true,
                components_moment_y = bhPointLoad.Moment.Y,
                components_moment_ySpecified = true,
                components_moment_z = bhPointLoad.Moment.Z,
                components_moment_zSpecified = true,
                load_type = nodal_load_load_type.LOAD_TYPE_COMPONENTS,
                load_typeSpecified = true,
            };


            return rfLoadCase;

        }

        public static rfModel.surface_load ToRFEM6(this AreaUniformlyDistributedLoad bhAreaLoad, int loadCaseSpecificLoadId)
        {




            surface_load_load_direction loadDirecteion;
            Vector orientationVector = bhAreaLoad.Pressure;
            double magnitude;
            if (orientationVector.X != 0)
            {
                loadDirecteion = bhAreaLoad.Projected ? surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED : surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                magnitude = bhAreaLoad.Pressure.X;
            }
            else if (orientationVector.Y != 0)
            {
                loadDirecteion = bhAreaLoad.Projected ? surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED : surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                    magnitude = bhAreaLoad.Pressure.Y;
            }
            else
            {
                loadDirecteion = bhAreaLoad.Projected ? surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED : surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                    magnitude = bhAreaLoad.Pressure.Z;
            }


            surface_load rfSurfaceLoad = new rfModel.surface_load()
            {
                no = loadCaseSpecificLoadId,
                comment=bhAreaLoad.Name,
                surfaces = bhAreaLoad.Objects.Elements.ToList().Select(x => (x as Panel).GetRFEM6ID()).ToArray(),
                load_case = bhAreaLoad.Loadcase.GetRFEM6ID(),
                load_caseSpecified = true,
                load_distribution = surface_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                magnitude_force_u = magnitude,
                magnitude_force_uSpecified = true,
                uniform_magnitude = magnitude,
                uniform_magnitudeSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,
                load_direction = loadDirecteion,
                load_directionSpecified = true,
                load_type = surface_load_load_type.LOAD_TYPE_FORCE,
                load_typeSpecified = true,
            };


            return rfSurfaceLoad;

        }


        public static rfModel.free_line_load ToRFEM6(this GeometricalLineLoad bhLineLoad, int loadCaseSpecificID, int[] surfaceIds)
        {
            free_line_load_load_direction loadDirecteion;
            Vector orientationVector = bhLineLoad.ForceA;
            double magnitudeA;
            double magnitudeB;

            if (orientationVector.X != 0)
            {
                loadDirecteion = bhLineLoad.Projected ? free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_PROJECTED : free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_TRUE;
                magnitudeA = bhLineLoad.ForceA.X;
                magnitudeB = bhLineLoad.ForceB.X;

            }
            else if (orientationVector.Y != 0)
            {
                loadDirecteion = bhLineLoad.Projected ? free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_PROJECTED : free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_TRUE;
                magnitudeA = bhLineLoad.ForceA.Y;
                magnitudeB = bhLineLoad.ForceB.Y;
            }
            else
            {
                loadDirecteion = bhLineLoad.Projected ? free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_PROJECTED : free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_TRUE;
                magnitudeA = bhLineLoad.ForceA.Z;
                magnitudeB = bhLineLoad.ForceB.Z;
            }



            free_line_load rfLineLoad = new rfModel.free_line_load()
            {
                surfaces = surfaceIds,
                surfaces_string = string.Join(" ", surfaceIds.Select(n => n.ToString())),

                no = loadCaseSpecificID,
                load_case = bhLineLoad.Loadcase.GetRFEM6ID(),
                load_caseSpecified = true,
                load_distribution = free_line_load_load_distribution.LOAD_DISTRIBUTION_LINEAR,
                load_distributionSpecified = true,
                magnitude_first = magnitudeA,
                magnitude_firstSpecified = true,
                magnitude_second = magnitudeB,
                magnitude_secondSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,
                load_direction = loadDirecteion,
                //load_direction = bhLineLoad.Projected ? free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_PROJECTED : free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_TRUE,
                load_directionSpecified = true,
                load_location_first_x = bhLineLoad.Location.Start.X,
                load_location_first_xSpecified = true,
                load_location_first_y = bhLineLoad.Location.Start.Y,
                load_location_first_ySpecified = true,
                load_location_second_x = bhLineLoad.Location.End.X,
                load_location_second_xSpecified = true,
                load_location_second_y = bhLineLoad.Location.End.Y,
                load_location_second_ySpecified = true,
                comment= bhLineLoad.Name,

            };


            return rfLineLoad;

        }

        public static rfModel.line_load ToRFEM6(this GeometricalLineLoad bhLineLoad, int id, int foundEdgeId, line_load_load_type rfLineLoadType)
        {


            //Checking for diretction of the GeometricalLineLoad. First discitguishing by Moment or force. 
            //Additionally setting both Maginigutes.
            line_load_load_direction rfLineLoadDirection;
            double loadMagnitude1;
            double loadMagnitude2;
            if (rfLineLoadType.Equals(rfModel.line_load_load_type.LOAD_TYPE_MOMENT))
            {
                if (bhLineLoad.MomentA.X != 0||bhLineLoad.MomentB.X!=0)
                {
                    rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                    loadMagnitude1 = bhLineLoad.MomentA.X;
                    loadMagnitude2 = bhLineLoad.MomentB.X;
                }
                else if (bhLineLoad.MomentA.Y != 0 || bhLineLoad.MomentB.Y != 0)
                {
                    rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                    loadMagnitude1 = bhLineLoad.MomentA.Y;
                    loadMagnitude2 = bhLineLoad.MomentB.Y;
                }
                else
                {
                    rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                    loadMagnitude1 = bhLineLoad.MomentA.Z;
                    loadMagnitude2 = bhLineLoad.MomentB.Z;
                }
            }
            else
            {
                if (bhLineLoad.ForceA.X != 0|| bhLineLoad.ForceB.X != 0)
                {
                    rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                    loadMagnitude1 = bhLineLoad.ForceA.X;
                    loadMagnitude2 = bhLineLoad.ForceB.X;
                }
                else if (bhLineLoad.ForceA.Y != 0 || bhLineLoad.ForceB.Y != 0)
                {
                    rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                    loadMagnitude1 = bhLineLoad.ForceA.Y;
                    loadMagnitude2 = bhLineLoad.ForceB.Y;
                }
                else
                {
                    rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                    loadMagnitude1 = bhLineLoad.ForceA.Z;
                    loadMagnitude2 = bhLineLoad.ForceB.Z;
                }

            }

            line_load rfLineLoad = new rfModel.line_load()
            {
                no = id,
                lines = new int[] { foundEdgeId },
                load_case = bhLineLoad.Loadcase.GetRFEM6ID(),
                load_caseSpecified = true,
                load_distribution = line_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL,
                load_distributionSpecified = true,
                magnitude = bhLineLoad.ForceA.Length(),
                magnitudeSpecified = true,
                magnitude_1 = loadMagnitude1,
                magnitude_1Specified = true,
                magnitude_2 = loadMagnitude2,
                magnitude_2Specified = true,
                reference_to_list_of_lines = false,
                reference_to_list_of_linesSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,
                load_direction = rfLineLoadDirection,
                load_directionSpecified = true,
                load_is_over_total_length = true,
                load_is_over_total_lengthSpecified = true,
                load_type = rfLineLoadType,
                load_typeSpecified = true,
                comment = bhLineLoad.Name,

            };


            return rfLineLoad;

        }



    }
}

