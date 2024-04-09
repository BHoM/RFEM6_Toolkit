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
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static BarUniformlyDistributedLoad FromRFEM(this rfModel.member_load rfMemberLoad, List<Bar> bhBars, Loadcase bhLoadCase)
        {
            Vector forceVector = new Vector() { X = 0, Y = 0, Z = 0 };
            Vector momentVector = new Vector() { X = 0, Y = 0, Z = 0 };

            if (rfMemberLoad.load_direction == member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED || rfMemberLoad.load_direction == member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE || rfMemberLoad.load_direction == member_load_load_direction.LOAD_DIRECTION_LOCAL_X)
            {
                momentVector = rfMemberLoad.load_type == member_load_load_type.LOAD_TYPE_MOMENT ? new Vector() { X = rfMemberLoad.magnitude, Y = 0, Z = 0 } : new Vector() { X = 0, Y = 0, Z = 0 };
                forceVector = rfMemberLoad.load_type == member_load_load_type.LOAD_TYPE_FORCE ? new Vector() { X = rfMemberLoad.magnitude, Y = 0, Z = 0 } : new Vector() { X = 0, Y = 0, Z = 0 };

            }
            else if (rfMemberLoad.load_direction == member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED || rfMemberLoad.load_direction == member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE || rfMemberLoad.load_direction == member_load_load_direction.LOAD_DIRECTION_LOCAL_Y)
            {
                momentVector = rfMemberLoad.load_type == member_load_load_type.LOAD_TYPE_MOMENT ? (new Vector() { X = 0, Y = rfMemberLoad.magnitude, Z = 0 }) : new Vector() { X = 0, Y = 0, Z = 0 };
                forceVector = rfMemberLoad.load_type == member_load_load_type.LOAD_TYPE_FORCE ? (new Vector() { X = 0, Y = rfMemberLoad.magnitude, Z = 0 }) : new Vector() { X = 0, Y = 0, Z = 0 };
            }
            else
            {
                momentVector = rfMemberLoad.load_type == member_load_load_type.LOAD_TYPE_MOMENT ? new Vector() { X = 0, Y = 0, Z = rfMemberLoad.magnitude } : new Vector() { X = 0, Y = 0, Z = 0 };
                forceVector = rfMemberLoad.load_type == member_load_load_type.LOAD_TYPE_FORCE ? new Vector() { X = 0, Y = 0, Z = rfMemberLoad.magnitude } : new Vector() { X = 0, Y = 0, Z = 0 };
            }

            BarUniformlyDistributedLoad bhLoad = new BarUniformlyDistributedLoad
            {
                Name=rfMemberLoad.comment,
                Objects = new BH.oM.Base.BHoMGroup<Bar>() { Elements = bhBars },
                Loadcase = bhLoadCase,
                Force = forceVector,
                Moment = momentVector
                
            };

            return bhLoad;
        }

        public static PointLoad FromRFEM(this rfModel.nodal_load nodeLoad, List<Node> bhNodes, Loadcase bhLoadCase)
        {


            PointLoad pointLoad;
            double[] force = new double[3];
            double[] moment = new double[3];

            if (nodeLoad.load_type == nodal_load_load_type.LOAD_TYPE_COMPONENTS)
            {
                force = new double[] { nodeLoad.components_force_x, nodeLoad.components_force_y, nodeLoad.components_force_z };
                moment = new double[] { nodeLoad.components_moment_x, nodeLoad.components_moment_y, nodeLoad.components_moment_z }; ;

            }
            else if (nodeLoad.load_type == nodal_load_load_type.LOAD_TYPE_MOMENT)
            {
                force = new double[] { 0, 0, 0 };
                moment = new double[] { nodeLoad.components_moment_x, nodeLoad.components_moment_y, nodeLoad.components_moment_z };

            }
            else
            { /*(nodeLoad.load_type == nodal_load_load_type.LOAD_TYPE_FORCE)*/
                force = new double[] { nodeLoad.components_force_x, nodeLoad.components_force_y, nodeLoad.components_force_z };
                moment = new double[] { 0, 0, 0 }; ;
            }


            PointLoad bhLoad = new PointLoad
            {
                Objects = new BH.oM.Base.BHoMGroup<Node>() { Elements = bhNodes },
                Loadcase = bhLoadCase,
                Force = BH.Engine.Geometry.Create.Vector(force[0], force[1], force[2]),
                Moment = BH.Engine.Geometry.Create.Vector(moment[0], moment[1], moment[2]),
                Name= nodeLoad.comment
            };


            return bhLoad;
        }

        public static AreaUniformlyDistributedLoad FromRFEM(this rfModel.surface_load surfaceload, Loadcase loadcase, List<Panel> panels)
        {

            Vector forceDirection;
            bool isProjected = false;


            //if (surfaceload.load_direction == surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE)
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(surfaceload.uniform_magnitude, 0, 0);
            //}
            //else if (surfaceload.load_direction == surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE)
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(0, surfaceload.uniform_magnitude, 0);
            //}
            //else if (surfaceload.load_direction == surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE)
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
            //}
            //else if (surfaceload.load_direction == surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED)
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(surfaceload.uniform_magnitude, 0, 0);
            //    isProjected = true;
            //}
            //else if (surfaceload.load_direction == surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED)
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(0, surfaceload.uniform_magnitude, 0);
            //    isProjected = true;
            //}
            //else if (surfaceload.load_direction == surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED)
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
            //    isProjected = true;
            //}
            //else
            //{
            //    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
            //}

            switch (surfaceload.load_direction)
            {
                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE:
                case surface_load_load_direction.LOAD_DIRECTION_LOCAL_X:
                    forceDirection = BH.Engine.Geometry.Create.Vector(surfaceload.uniform_magnitude, 0, 0);
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE:
                case surface_load_load_direction.LOAD_DIRECTION_LOCAL_Y:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, surfaceload.uniform_magnitude, 0);
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE:
                case surface_load_load_direction.LOAD_DIRECTION_LOCAL_Z:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED:
                    forceDirection = BH.Engine.Geometry.Create.Vector(surfaceload.uniform_magnitude, 0, 0);
                    isProjected = true;
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, surfaceload.uniform_magnitude, 0);
                    isProjected = true;
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
                    isProjected = true;
                    break;

                default:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
                    break;
            }


            AreaUniformlyDistributedLoad bhAreaload = BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, forceDirection, panels,LoadAxis.Global,isProjected,surfaceload.comment);

            return bhAreaload;
        }

        // convert Free Line Loads into Geometrical Line Loads
        public static GeometricalLineLoad FromRFEM(this rfModel.free_line_load rfLineload, Loadcase loadcase, List<Panel> panels)
        {


            Line line = new BH.oM.Geometry.Line() { Start = new Point() { X = rfLineload.load_location_first_x, Y = rfLineload.load_location_first_y, Z = 0 }, End = new Point() { X = rfLineload.load_location_second_x, Y = rfLineload.load_location_second_y, Z = 0 } };

            GeometricalLineLoad bhLineLoad = new GeometricalLineLoad()
            {
                Name=rfLineload.comment,
                Loadcase = loadcase,
                Location = line,
                ForceA = BH.Engine.Geometry.Create.Vector(0, 0, rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform),
                ForceB = BH.Engine.Geometry.Create.Vector(0, 0, rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform),

            };

            bhLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(bhLineLoad, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.FreeLineLoad }));
            //GeometricalLineLoad bhLineLoad = BH.Engine.Structure.Create.GeometricalLineLoad(line, loadcase, Vector.ZAxis, Vector.ZAxis, panels);


            return bhLineLoad;
        }

        // convert  Line Loads into Geometrical Line Loads
        public static GeometricalLineLoad FromRFEM(this rfModel.line_load rfLineload, Loadcase loadcase, Line line)
        {
            Vector impactA = new Vector();
            Vector impactB = new Vector();
            double impactMagnitudeA = rfLineload.load_distribution == line_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL ? rfLineload.magnitude_1 : rfLineload.magnitude;
            double impactMagnitudeB = rfLineload.load_distribution == line_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL ? rfLineload.magnitude_2 : rfLineload.magnitude;

            if (rfLineload.load_direction == line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE)
            {

                impactA.X = impactMagnitudeA;
                impactB.X = impactMagnitudeB;

            }
            else if (rfLineload.load_direction == line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE)
            {
                impactA.Y = impactMagnitudeA;
                impactB.Y = impactMagnitudeB;
            }
            else if (rfLineload.load_direction == line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE)
            {
                impactA.Z = impactMagnitudeA;
                impactB.Z = impactMagnitudeB;
            }
            else
            {
                BH.Engine.Base.Compute.RecordError($"The Load {rfLineload} within RFEM6 is has not direction that is Parallel to the X,Y or Z axist. The Load direction will be set to a null-vector!");


            }

            GeometricalLineLoad bhLineLoad = new GeometricalLineLoad()
            {
                Name=rfLineload.comment,
                Loadcase = loadcase,
                Location = line,
                ForceA = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_FORCE ? impactA : new Vector(),
                ForceB = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_FORCE ? impactB : new Vector(),
                MomentA = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_MOMENT ? impactA : new Vector(),
                MomentB = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_MOMENT ? impactB : new Vector(),

            };

            bhLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(bhLineLoad, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            return bhLineLoad;
        }

        

    }
}

