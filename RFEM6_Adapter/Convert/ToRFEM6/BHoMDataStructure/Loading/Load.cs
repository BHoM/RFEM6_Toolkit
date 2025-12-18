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
using BH.oM.Structure.Loads;
using BH.oM.Adapter.Commands;
using Dlubal.WS.Rfem6.Model;
using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.Engine.Spatial;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.member_load ToRFEM6(this BarUniformlyDistributedLoad bhBarLoad, member_load_load_type loadType, int id)
        {

            //var i = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToList();

            double loadMagnitude;
            member_load_load_direction loadDirecteion;
            Vector orientationVector = loadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force : bhBarLoad.Moment;

            if (bhBarLoad.Projected && bhBarLoad.Axis == LoadAxis.Local)
            {
                BH.Engine.Base.Compute.RecordWarning("Projected BarUniformlyDistributedLoad is not supported for Local Axis. The Load will be projected to the Global Axis");
            }

            // Checkinf if Load is Orientied in X directin
            if (orientationVector.X != 0)
            {
                // If the load Axis in slocal the load direction is local x
                if (bhBarLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                }
                else if (bhBarLoad.Projected && loadType.Equals(member_load_load_type.LOAD_TYPE_FORCE))
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED;
                }
                else
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                }
                loadMagnitude = loadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force.X : bhBarLoad.Moment.X;


            }
            else if (orientationVector.Y != 0)
            {
                if (bhBarLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                }
                else if (bhBarLoad.Projected && loadType.Equals(member_load_load_type.LOAD_TYPE_FORCE))
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED;
                }
                else
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                }
                loadMagnitude = loadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force.Y : bhBarLoad.Moment.Y;

            }
            else
            {
                if (bhBarLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                }
                else if (bhBarLoad.Projected && loadType.Equals(member_load_load_type.LOAD_TYPE_FORCE))
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED;
                }
                else
                {
                    loadDirecteion = member_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                }
                loadMagnitude = loadType == member_load_load_type.LOAD_TYPE_FORCE ? bhBarLoad.Force.Z : bhBarLoad.Moment.Z;
            }

            member_load rfLoadCase = new rfModel.member_load()
            {
                no = id,
                comment = bhBarLoad.Name,
                members = bhBarLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
                load_distribution = member_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                load_type = loadType,
                load_typeSpecified = true,
                load_direction = loadDirecteion,
                load_directionSpecified = true,
                magnitude = loadMagnitude,
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

            BH.Engine.Base.Compute.RecordWarning("PointLoad push has at the moment only been implemented for Axis=LoadAxis.Global. Axis will be set to LoadAxis.Global!");

            nodal_load rfLoadCase = new rfModel.nodal_load()
            {

                no = id,
                comment = bhPointLoad.Name,
                nodes = bhPointLoad.Objects.Elements.ToList().Select(x => x.GetRFEM6ID()).ToArray(),
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


            double loadMagnitude;
            surface_load_load_direction loadDirection;
            Vector orientationVector = bhAreaLoad.Pressure;

            if (bhAreaLoad.Projected && bhAreaLoad.Axis == LoadAxis.Local)
            {
                BH.Engine.Base.Compute.RecordWarning("Projected Area Load is not supported for Local Axis. The Load will be projected to the Global Axis");
            }

            // Checkinf if Load is Orientied in X directin
            if (orientationVector.X != 0)
            {
                // If the load Axis in slocal the load direction is local x
                if (bhAreaLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                }
                else if (bhAreaLoad.Projected)
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED;
                }
                else
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                }
                loadMagnitude = orientationVector.X;

            }
            else if (orientationVector.Y != 0)
            {
                if (bhAreaLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                }
                else if (bhAreaLoad.Projected)
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED;
                }
                else
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                }
                loadMagnitude = orientationVector.Y;

            }
            else
            {
                if (bhAreaLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                }
                else if (bhAreaLoad.Projected)
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED;
                }
                else
                {
                    loadDirection = surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                }
                loadMagnitude = orientationVector.Z;
            }


            surface_load rfSurfaceLoad = new rfModel.surface_load()
            {
                no = loadCaseSpecificLoadId,
                comment = bhAreaLoad.Name,
                surfaces = bhAreaLoad.Objects.Elements.ToList().Select(x => (x as Panel).GetRFEM6ID()).ToArray(),
                load_case = bhAreaLoad.Loadcase.Number,
                load_caseSpecified = true,
                load_distribution = surface_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                magnitude_force_u = loadMagnitude,
                magnitude_force_uSpecified = true,
                uniform_magnitude = loadMagnitude,
                uniform_magnitudeSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,
                load_direction = loadDirection,
                load_directionSpecified = true,
                load_type = surface_load_load_type.LOAD_TYPE_FORCE,
                load_typeSpecified = true,
            };


            return rfSurfaceLoad;

        }

        public static rfModel.free_polygon_load ToRFEM6_Polygon(this AreaUniformlyDistributedLoad bhAreaLoad, int loadCaseSpecificLoadId)
        {
            free_polygon_load_load_projection loadProjection;

            if (bhAreaLoad.Pressure.IsParallel(BH.oM.Geometry.Vector.XAxis)==0 
                && bhAreaLoad.Pressure.IsParallel(BH.oM.Geometry.Vector.YAxis) == 0
                && bhAreaLoad.Pressure.IsParallel(BH.oM.Geometry.Vector.ZAxis) == 0)
            {
                BH.Engine.Base.Compute.RecordWarning($"Please make sure that the direction of {bhAreaLoad} is axis-aligned.");
            }
            
            Polygon polygon = (Polygon)bhAreaLoad.CustomData.Values.First(p => p is Polygon);
            List<double[]> polygonValues;
            var fitPlane=polygon.IFitPlane();
            if (fitPlane.Normal.CrossProduct(Plane.XY.Normal).Length() < 0.01)
            {
                polygonValues= polygon.Vertices.Select(v => new double[] { v.X, v.Y }).ToList();
                loadProjection = free_polygon_load_load_projection.LOAD_PROJECTION_XY_OR_UV;
            }
            else if (fitPlane.Normal.CrossProduct(Plane.XZ.Normal).Length() < 0.01)
            {
                polygonValues= polygon.Vertices.Select(v => new double[] { v.X, v.Z }).ToList();
                loadProjection = free_polygon_load_load_projection.LOAD_PROJECTION_XZ_OR_UW;
            }
            else {
                polygonValues= polygon.Vertices.Select(v => new double[] { v.Y, v.Z }).ToList();
                loadProjection = free_polygon_load_load_projection.LOAD_PROJECTION_YZ_OR_VW;

            }

            free_polygon_load_load_location_row[] locationPolygons = polygonValues.Select((v, i) => new free_polygon_load_load_location_row()
            {
                no = (i+1),
                description=(i+1)+"",
                row = new free_polygon_load_load_location()
                {
                    magnitude=0,
                    magnitudeSpecified=false,
                    first_coordinate = v[0],
                    first_coordinateSpecified = true,
                    second_coordinate = v[1],
                    second_coordinateSpecified = true
                    
                }
            }

            ).ToArray();

            double loadMagnitude;
            free_polygon_load_load_direction loadDirection;
            Vector orientationVector = bhAreaLoad.Pressure;

            if (bhAreaLoad.Projected && bhAreaLoad.Axis == LoadAxis.Local)
            {
                BH.Engine.Base.Compute.RecordWarning("Projected Area Load is not supported for Local Axis. The Load will be projected to the Global Axis");
            }

            // Checkinf if Load is Orientied in X directin
            if (orientationVector.X != 0)
            {
                // If the load Axis in slocal the load direction is local x
                if (bhAreaLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                    if (bhAreaLoad.Projected) BH.Engine.Base.Compute.RecordWarning($"The 'Projected' option for {bhAreaLoad} is not supported when the Axis is set to Local and will therefore be ignored.");

                }
                else if (bhAreaLoad.Projected)
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_GLOBAL_X_PROJECTED;
                }
                else
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_GLOBAL_X_TRUE;
                }
                loadMagnitude = orientationVector.X;

            }
            else if (orientationVector.Y != 0)
            {
                if (bhAreaLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                    if (bhAreaLoad.Projected) BH.Engine.Base.Compute.RecordWarning($"The 'Projected' option for {bhAreaLoad} is not supported when the Axis is set to Local and will therefore be ignored.");
                }
                else if (bhAreaLoad.Projected)
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_PROJECTED;
                }
                else
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_TRUE;
                }
                loadMagnitude = orientationVector.Y;

            }
            else
            {
                if (bhAreaLoad.Axis.Equals(LoadAxis.Local))
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                    if (bhAreaLoad.Projected) BH.Engine.Base.Compute.RecordWarning($"The 'Projected' option for {bhAreaLoad} is not supported when the Axis is set to Local and will therefore be ignored.");
                }
                else if (bhAreaLoad.Projected)
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_PROJECTED;
                }
                else
                {
                    loadDirection = free_polygon_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_TRUE;
                }
                loadMagnitude = orientationVector.Z;
            }



            free_polygon_load rfSurfaceLoad = new rfModel.free_polygon_load()
            {
                no = loadCaseSpecificLoadId,
                comment = bhAreaLoad.Name,
                surfaces = bhAreaLoad.Objects.Elements.ToList().Select(x => (x as Panel).GetRFEM6ID()).ToArray(),
                load_case = bhAreaLoad.Loadcase.Number,
                load_caseSpecified = true,
                load_distribution = free_polygon_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM,
                load_distributionSpecified = true,
                magnitude_linear_1 = loadMagnitude,
                magnitude_linear_1Specified = true,
                magnitude_uniform = loadMagnitude,
                magnitude_uniformSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,
                load_direction = loadDirection,
                load_directionSpecified = true,
                load_location = locationPolygons,
                load_projection = loadProjection,
                load_projectionSpecified = true
            };


            return rfSurfaceLoad;

        }


        public static rfModel.free_line_load ToRFEM6(this GeometricalLineLoad bhLineLoad, int loadCaseSpecificID, int[] surfaceIds)
        {
            free_line_load_load_direction rfLineLoadDirection;
            Vector orientationVector = bhLineLoad.ForceA;
            double magnitudeA;
            double magnitudeB;

            if (bhLineLoad.Projected && bhLineLoad.Axis == LoadAxis.Local)
            {
                BH.Engine.Base.Compute.RecordWarning("Projected Free Line Load is not supported for Local Axis. The Load will be projected to the Global Axis");
            }

            if (orientationVector.X != 0)
            {
                rfLineLoadDirection = bhLineLoad.Projected ? free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_PROJECTED : free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_TRUE;

                if (bhLineLoad.Projected)
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_PROJECTED;
                }
                else if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_TRUE;
                }
                else
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                }

                magnitudeA = bhLineLoad.ForceA.X;
                magnitudeB = bhLineLoad.ForceB.X;


            }
            else if (orientationVector.Y != 0)
            {

                if (bhLineLoad.Projected)
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_PROJECTED;
                }
                else if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_TRUE;
                }
                else
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                }

                magnitudeA = bhLineLoad.ForceA.Y;
                magnitudeB = bhLineLoad.ForceB.Y;
            }
            else
            {
                if (bhLineLoad.Projected)
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_PROJECTED;
                }
                else if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_TRUE;
                }
                else
                {
                    rfLineLoadDirection = free_line_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                }

                magnitudeA = bhLineLoad.ForceA.Z;
                magnitudeB = bhLineLoad.ForceB.Z;
            }

            free_line_load rfLineLoad = new rfModel.free_line_load()
            {
                surfaces = surfaceIds,
                surfaces_string = string.Join(" ", surfaceIds.Select(n => n.ToString())),

                no = loadCaseSpecificID,
                load_case = bhLineLoad.Loadcase.Number,
                load_caseSpecified = true,
                load_distribution = free_line_load_load_distribution.LOAD_DISTRIBUTION_LINEAR,
                load_distributionSpecified = true,
                magnitude_first = magnitudeA,
                magnitude_firstSpecified = true,
                magnitude_second = magnitudeB,
                magnitude_secondSpecified = true,
                is_generated = false,
                is_generatedSpecified = true,
                load_direction = rfLineLoadDirection,
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
                comment = bhLineLoad.Name,

            };


            return rfLineLoad;

        }

        //Non Free Line Load
        public static rfModel.line_load ToRFEM6(this GeometricalLineLoad bhLineLoad, int id, int foundEdgeId, line_load_load_type rfLineLoadType)
        {


            //Checking for direction of the GeometricalLineLoad. First distinguishing by Moment or force. 
            //Additionally setting both magnitudes.
            line_load_load_direction rfLineLoadDirection;
            double loadMagnitude1;
            double loadMagnitude2;

            if (bhLineLoad.Projected && bhLineLoad.Axis == LoadAxis.Local)
            {
                BH.Engine.Base.Compute.RecordWarning("Projected Non-Free Line Load is not supported for Local Axis. The Load will be projected to the Global Axis");
            }

            //Lineload is a Moment
            if (rfLineLoadType.Equals(rfModel.line_load_load_type.LOAD_TYPE_MOMENT))
            {
                if (bhLineLoad.MomentA.X != 0 || bhLineLoad.MomentB.X != 0)
                {

                    if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                    }
                    else
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                    }

                    loadMagnitude1 = bhLineLoad.MomentA.X;
                    loadMagnitude2 = bhLineLoad.MomentB.X;
                }
                else if (bhLineLoad.MomentA.Y != 0 || bhLineLoad.MomentB.Y != 0)
                {

                    if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                    }
                    else
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                    }

                    loadMagnitude1 = bhLineLoad.MomentA.Y;
                    loadMagnitude2 = bhLineLoad.MomentB.Y;
                }
                else
                {


                    if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                    }
                    else
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                    }

                    loadMagnitude1 = bhLineLoad.MomentA.Z;
                    loadMagnitude2 = bhLineLoad.MomentB.Z;
                }
            }
            // Lineload is a Force
            else
            {
                if (bhLineLoad.ForceA.X != 0 || bhLineLoad.ForceB.X != 0)
                {

                    if (bhLineLoad.Projected)
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED;
                    }
                    else if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
                    }
                    else
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_LOCAL_X;
                    }

                    loadMagnitude1 = bhLineLoad.ForceA.X;
                    loadMagnitude2 = bhLineLoad.ForceB.X;
                }
                else if (bhLineLoad.ForceA.Y != 0 || bhLineLoad.ForceB.Y != 0)
                {


                    if (bhLineLoad.Projected)
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED;
                    }
                    else if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
                    }
                    else
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_LOCAL_Y;
                    }

                    loadMagnitude1 = bhLineLoad.ForceA.Y;
                    loadMagnitude2 = bhLineLoad.ForceB.Y;
                }
                else
                {

                    if (bhLineLoad.Projected)
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED;
                    }
                    else if (bhLineLoad.Axis.Equals(LoadAxis.Global))
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
                    }
                    else
                    {
                        rfLineLoadDirection = line_load_load_direction.LOAD_DIRECTION_LOCAL_Z;
                    }

                    loadMagnitude1 = bhLineLoad.ForceA.Z;
                    loadMagnitude2 = bhLineLoad.ForceB.Z;
                }

            }

            line_load rfLineLoad = new rfModel.line_load()
            {
                no = id,
                lines = new int[] { foundEdgeId },
                load_case = bhLineLoad.Loadcase.Number,
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


