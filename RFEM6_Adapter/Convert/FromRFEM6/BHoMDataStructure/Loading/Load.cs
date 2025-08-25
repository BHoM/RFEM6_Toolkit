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
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using System.Linq.Expressions;
using System.Diagnostics;
using BH.oM.Base;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static BarUniformlyDistributedLoad FromRFEM(this List<rfModel.member_load> rfMemberLoad_list, List<Bar> bhBars, Loadcase bhLoadCase)
        {

            Vector forceVector = new Vector() { X = 0, Y = 0, Z = 0 };
            Vector momentVector = new Vector() { X = 0, Y = 0, Z = 0 };

            foreach (rfModel.member_load rfMemberLoadItem in rfMemberLoad_list)
            {

                switch (rfMemberLoadItem.load_direction)
                {
                    case member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED:
                    case member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE:
                    case member_load_load_direction.LOAD_DIRECTION_LOCAL_X:

                        if (rfMemberLoadItem.load_type == member_load_load_type.LOAD_TYPE_MOMENT)
                        {
                            momentVector = momentVector + new Vector() { X = rfMemberLoadItem.magnitude, Y = 0, Z = 0 };
                        }
                        else if (rfMemberLoadItem.load_type == member_load_load_type.LOAD_TYPE_FORCE)
                        {
                            forceVector = forceVector + new Vector() { X = rfMemberLoadItem.magnitude, Y = 0, Z = 0 };

                        }

                        break;

                    case member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED:
                    case member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE:
                    case member_load_load_direction.LOAD_DIRECTION_LOCAL_Y:

                        if (rfMemberLoadItem.load_type == member_load_load_type.LOAD_TYPE_MOMENT)
                        {
                            momentVector = momentVector + new Vector() { X = 0, Y = rfMemberLoadItem.magnitude, Z = 0 };
                        }
                        else if (rfMemberLoadItem.load_type == member_load_load_type.LOAD_TYPE_FORCE)
                        {
                            forceVector = forceVector + new Vector() { X = 0, Y = rfMemberLoadItem.magnitude, Z = 0 };
                        }

                        break;
                    default:


                        if (rfMemberLoadItem.load_type == member_load_load_type.LOAD_TYPE_MOMENT)
                        {
                            momentVector = momentVector + new Vector() { X = 0, Y = 0, Z = rfMemberLoadItem.magnitude };
                        }
                        else if (rfMemberLoadItem.load_type == member_load_load_type.LOAD_TYPE_FORCE)
                        {
                            forceVector = forceVector + new Vector() { X = 0, Y = 0, Z = rfMemberLoadItem.magnitude };
                        }

                        break;

                }



            }

            rfModel.member_load rfMemberLoad = rfMemberLoad_list.First();
            LoadAxis axis = LoadAxis.Global;
            bool isProjected = false;

            switch (rfMemberLoad.load_direction)
            {

                case member_load_load_direction.LOAD_DIRECTION_LOCAL_X:
                case member_load_load_direction.LOAD_DIRECTION_LOCAL_Y:
                case member_load_load_direction.LOAD_DIRECTION_LOCAL_Z:
                    axis = LoadAxis.Local;
                    break;
                case member_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED:
                case member_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED:
                case member_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED:
                    isProjected = true;
                    break;
                default:
                    break;
            }

            string loadName = rfMemberLoad.comment?.Split(new[] { "::" }, StringSplitOptions.None).ElementAtOrDefault(1) ?? string.Empty;
            string guid = rfMemberLoad.comment.Split(new[] { "::" }, StringSplitOptions.None).Last();

            BarUniformlyDistributedLoad bhLoad = new BarUniformlyDistributedLoad
            {
                Name = loadName,
                Objects = new BH.oM.Base.BHoMGroup<Bar>() { Elements = bhBars },
                Loadcase = bhLoadCase,
                Force = forceVector,
                Moment = momentVector,
                Axis = axis,
                Projected = isProjected
            };
           bhLoad.SetRFEM6ID(guid);

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
                Name = nodeLoad.comment
            };


            return bhLoad;
        }

        public static AreaUniformlyDistributedLoad FromRFEM(this rfModel.surface_load surfaceload, Loadcase loadcase, List<Panel> panels)
        {

            Vector forceDirection;
            bool isProjected = false;
            LoadAxis axis = LoadAxis.Global;

            switch (surfaceload.load_direction)
            {
                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE:
                    forceDirection = BH.Engine.Geometry.Create.Vector(surfaceload.uniform_magnitude, 0, 0);
                    break;
                case surface_load_load_direction.LOAD_DIRECTION_LOCAL_X:
                    forceDirection = BH.Engine.Geometry.Create.Vector(surfaceload.uniform_magnitude, 0, 0);
                    axis = LoadAxis.Local;
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, surfaceload.uniform_magnitude, 0);
                    break;
                case surface_load_load_direction.LOAD_DIRECTION_LOCAL_Y:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, surfaceload.uniform_magnitude, 0);
                    axis = LoadAxis.Local;
                    break;

                case surface_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
                    break;
                case surface_load_load_direction.LOAD_DIRECTION_LOCAL_Z:
                    forceDirection = BH.Engine.Geometry.Create.Vector(0, 0, surfaceload.uniform_magnitude);
                    axis = LoadAxis.Local;
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


            AreaUniformlyDistributedLoad bhAreaload = BH.Engine.Structure.Create.AreaUniformlyDistributedLoad(loadcase, forceDirection, panels, axis, isProjected, surfaceload.comment);

            return bhAreaload;
        }

        // convert Free Line Loads into Geometrical Line Loads
        public static GeometricalLineLoad FromRFEM(this rfModel.free_line_load rfLineload, Loadcase loadcase, List<Panel> panels)
        {


            Line loadLocationLine = new BH.oM.Geometry.Line() { Start = new Point() { X = rfLineload.load_location_first_x, Y = rfLineload.load_location_first_y, Z = 0 }, End = new Point() { X = rfLineload.load_location_second_x, Y = rfLineload.load_location_second_y, Z = 0 } };
            LoadAxis axis = LoadAxis.Global;
            bool isProjected = false;
            Vector impactA = new Vector();
            Vector impactB = new Vector();

            switch (rfLineload.load_direction)
            {
                case free_line_load_load_direction.LOAD_DIRECTION_LOCAL_X:
                    impactA.X = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.X = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    axis = LoadAxis.Local;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_LOCAL_Y:
                    impactA.Y = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.Y = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    axis = LoadAxis.Local;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_LOCAL_Z:
                    impactA.Z = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.Z = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    axis = LoadAxis.Local;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_TRUE:
                    impactA.X = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.X = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_TRUE:
                    impactA.Y = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.Y = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_TRUE:
                    impactA.Z = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.Z = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_PROJECTED:
                    impactA.X = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.X = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    isProjected = true;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_PROJECTED:
                    impactA.Y = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.Y = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    isProjected = true;
                    break;
                case free_line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_PROJECTED:
                    impactA.Z = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_first : rfLineload.magnitude_uniform;
                    impactB.Z = rfLineload.magnitude_secondSpecified ? rfLineload.magnitude_second : rfLineload.magnitude_uniform;
                    isProjected = true;
                    break;
                default:
                    break;

            }


            BHoMGroup<IAreaElement> relatedPanels = new BHoMGroup<IAreaElement>() { Elements = panels.Select(p => (IAreaElement)p).ToList() };


            GeometricalLineLoad bhLineLoad = new GeometricalLineLoad()
            {
                Name = rfLineload.comment,
                Loadcase = loadcase,
                Location = loadLocationLine,
                ForceA = impactA,
                ForceB = impactB,
                Axis = axis,
                Projected = isProjected,
                Objects = relatedPanels

            };

            bhLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(bhLineLoad, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.FreeLineLoad }));
            //GeometricalLineLoad bhLineLoad = BH.Engine.Structure.Create.GeometricalLineLoad(line, loadcase, Vector.ZAxis, Vector.ZAxis, panels);


            return bhLineLoad;
        }

        // convert NON Free Line Loads into Geometrical Line Loads
        public static GeometricalLineLoad FromRFEM(this rfModel.line_load rfLineload, Loadcase loadcase, Line line)
        {
            Vector impactA = new Vector();
            Vector impactB = new Vector();
            double impactMagnitudeA = rfLineload.load_distribution == line_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL ? rfLineload.magnitude_1 : rfLineload.magnitude;
            double impactMagnitudeB = rfLineload.load_distribution == line_load_load_distribution.LOAD_DISTRIBUTION_TRAPEZOIDAL ? rfLineload.magnitude_2 : rfLineload.magnitude;
            bool projected = false;
            LoadAxis axis = LoadAxis.Global;


            switch (rfLineload.load_direction)
            {
                case line_load_load_direction.LOAD_DIRECTION_LOCAL_X:
                    axis = LoadAxis.Local;
                    impactA.X = impactMagnitudeA;
                    impactB.X = impactMagnitudeB;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_LOCAL_Y:
                    impactA.Y = impactMagnitudeA;
                    impactB.Y = impactMagnitudeB;
                    axis = LoadAxis.Local;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_LOCAL_Z:
                    impactA.Z = impactMagnitudeA;
                    impactB.Z = impactMagnitudeB;
                    axis = LoadAxis.Local;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE:
                    impactA.X = impactMagnitudeA;
                    impactB.X = impactMagnitudeB;
                    projected = false;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_PROJECTED:
                    impactA.X = impactMagnitudeA;
                    impactB.X = impactMagnitudeB;
                    projected = true;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE:
                    impactA.Y = impactMagnitudeA;
                    impactB.Y = impactMagnitudeB;
                    projected = false;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_PROJECTED:
                    impactA.Y = impactMagnitudeA;
                    impactB.Y = impactMagnitudeB;
                    projected = true;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE:
                    impactA.Z = impactMagnitudeA;
                    impactB.Z = impactMagnitudeB;
                    projected = false;
                    break;
                case line_load_load_direction.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_PROJECTED:
                    impactA.Z = impactMagnitudeA;
                    impactB.Z = impactMagnitudeB;
                    projected = true;
                    break;
                default:
                    BH.Engine.Base.Compute.RecordWarning($"The Load {rfLineload} within RFEM6 is has not direction that is Parallel to the X,Y or Z axist. The Load direction will be set to a null-vector!");
                    break;
            }


            GeometricalLineLoad bhLineLoad = new GeometricalLineLoad()
            {
                Name = rfLineload.comment,
                Loadcase = loadcase,
                Location = line,
                ForceA = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_FORCE ? impactA : new Vector(),
                ForceB = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_FORCE ? impactB : new Vector(),
                MomentA = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_MOMENT ? impactA : new Vector(),
                MomentB = rfLineload.load_type == rfModel.line_load_load_type.LOAD_TYPE_MOMENT ? impactB : new Vector(),
                Axis = axis,
                Projected = projected

            };

            bhLineLoad = (GeometricalLineLoad)BH.Engine.Base.Modify.AddFragment(bhLineLoad, (new RFEM6GeometricalLineLoadTypes() { geometrialLineLoadType = GeometricalLineLoadTypesEnum.NonFreeLineLoad }));

            return bhLineLoad;
        }



    }
}


