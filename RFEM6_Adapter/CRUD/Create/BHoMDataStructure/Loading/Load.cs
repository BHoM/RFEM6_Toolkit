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
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Loads;
using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;
using System.Security.Cryptography;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.Engine.Structure;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<ILoad> bhLoads)
        {
            //Container for Potential Surface IDs. Surface IDs are onle relevant when using Free Line Loads
            int[] surfaceIds = new int[] { };

            foreach (ILoad bhLoad in bhLoads)
            {


                if (bhLoad is AreaUniformlyDistributedLoad)
                {
                    // Updating the load dictionary
                    UpdateLoadIdDictionary(bhLoad);

                    //Call Panel Load Methond to update the Panel ID Dictionary
                    this.GetCachedOrReadAsDictionary<int, Panel>();
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    surface_load rfemAreaLoad = (bhLoad as AreaUniformlyDistributedLoad).ToRFEM6(id);
                    var currrSurfaceIds = (bhLoad as AreaUniformlyDistributedLoad).Objects.Elements.ToList().Select(e => m_PanelIDdict[e as Panel]).ToArray();
                    rfemAreaLoad.surfaces = currrSurfaceIds;
                    //rfemAreaLoad. = currrSurfaceIds;
                    m_Model.set_surface_load(bhLoad.Loadcase.Number, rfemAreaLoad);

                    continue;
                }


                object nodalLoadType = null;
                if (!(bhLoad is GeometricalLineLoad))
                {
                    nodalLoadType = MomentOfForceLoad(bhLoad);
                    if (nodalLoadType is null) continue;
                }


                if (bhLoad is BarUniformlyDistributedLoad)
                {

                    if (!DirectionVectorIsXYZAxisParallel((bhLoad as BarUniformlyDistributedLoad).Force))
                    {

                        BH.Engine.Base.Compute.RecordError($"The Force Vector of {bhLoad} is not aligned with the X, Y or Z axis. Please make sure your Barload is either parallel to the X, Y or Z axis!");

                        //continue;
                    }
                    if (!DirectionVectorIsXYZAxisParallel((bhLoad as BarUniformlyDistributedLoad).Moment))
                    {

                        BH.Engine.Base.Compute.RecordError($"The Moment Vector of {bhLoad} is not aligned with the X, Y or Z axis. Please make sure your Barload is either parallel to the X, Y or Z axis!");

                        //continue;
                    }

                    var splitLoadList=SplitLoadIntoAxisAlignedLoads((bhLoad as BarUniformlyDistributedLoad));

                    foreach (var b in splitLoadList)
                    {
                        UpdateLoadIdDictionary(bhLoad);
                        int id_ = m_LoadcaseLoadIdDict[b.Loadcase][b.GetType().Name];
                        member_load member_load_ = (b as BarUniformlyDistributedLoad).ToRFEM6((member_load_load_type)nodalLoadType, id_);
                        m_Model.set_member_load(bhLoad.Loadcase.Number, member_load_);
                    }


                    //UpdateLoadIdDictionary(bhLoad);
                    //int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    //member_load member_load = (bhLoad as BarUniformlyDistributedLoad).ToRFEM6((member_load_load_type)nodalLoadType, id);
                    //var rfMemberLoad = member_load;
                    //m_Model.set_member_load(bhLoad.Loadcase.Number, rfMemberLoad);
                    continue;
                }

                if (bhLoad is PointLoad)
                {

                    UpdateLoadIdDictionary(bhLoad);
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    nodal_load rfPointLoad = (bhLoad as PointLoad).ToRFEM6((nodal_load_load_type)nodalLoadType, id);
                    m_Model.set_nodal_load(bhLoad.Loadcase.Number, rfPointLoad);
                    continue;

                }

                if (bhLoad is GeometricalLineLoad)
                {
                    // Handling Non-Free Line Loads

                    //Checking if the GeometricalLineLoad has a geometricalLineLoadType Fragment
                    bool lineLoadhasFragments = bhLoad.Fragments.ToList().Any(f => f.GetType().Name == "RFEM6GeometricalLineLoadTypes");


                    if (!lineLoadhasFragments)
                    {

                        BH.Engine.Base.Compute.RecordWarning($"{bhLoad} has no geometricalLineLoadType set. As a default value geometricalLineLoadType.geometricalLineLoadEnum has been set to FreeLineLoad.\n In case you want to generate LineLoads please add the fragment geometricalLineLoadTyp to the {bhLoad} and set the parameters accordingly.");
                    }


                    //If Load is a Non-Free Line Load
                    if (lineLoadhasFragments && BH.Engine.Base.Query.GetAllFragments(bhLoad)[0].PropertyValue("geometrialLineLoadType").ToString() != "FreeLineLoad")

                    {
                        EdgeComparer edgeComparer = new EdgeComparer();

                        //Checking Getting all panel Edges/Lines and reviewing for prospect Edges/Lines
                        List<Panel> panelCachList = GetCachedOrRead<Panel>();
                        var lineCachDict = GetCachedOrReadAsDictionary<Line, int>();
                        var edgeProspects = panelCachList.SelectMany(p => p.ExternalEdges).Where(e => (e.Curve is Line) || (e.Curve is Polyline && e.Curve.ControlPoints().Count == 2)).Distinct().ToHashSet();
                        HashSet<Edge> edgeProsepectSet = new HashSet<Edge>(edgeProspects, edgeComparer);

                        //Getting the prosepcted line with the ID attached
                        bool locationLineIsValid = edgeProsepectSet.Contains(new Edge { Curve = (bhLoad as GeometricalLineLoad).Location });

                        //Local Line is Valid, if ther is a Panel with the bespoke like in it
                        if (locationLineIsValid)
                        {
                            UpdateLoadIdDictionary(bhLoad);
                            int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name + "_NonFreeLineLoad"];
                            var locatedEdte = edgeProsepectSet.Where(e => (edgeComparer).Equals(e, new Edge() { Curve = (bhLoad as GeometricalLineLoad).Location })).FirstOrDefault();
                            line_load rfLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(id, locatedEdte.GetRFEM6ID(), (line_load_load_type)MomentOfForceLoad(bhLoad));
                            m_Model.set_line_load(bhLoad.Loadcase.Number, rfLineLoad);
                            continue;
                        }
                        else
                        {
                            BH.Engine.Base.Compute.RecordError($"The Location Line of {(bhLoad as GeometricalLineLoad)} is not valid. Please make sure your Location Line is located on a panel Edge! Also make sure that the location line and the panel line are oriented in the same way!");
                            continue;
                        }

                    }
                    // If Load is a FreeLineLoad
                    else
                    {
                        // Checking if Moment and Force Vectors have been mixed. Free Line Loads only allow Forces or only Moments
                        if ((bhLoad as GeometricalLineLoad).MomentA.Length() > 0 || (bhLoad as GeometricalLineLoad).MomentB.Length() > 0)
                        {
                            BH.Engine.Base.Compute.RecordError($"Free Line Loads do not allow Moments. Please remove the Moment Vector from the Load {bhLoad}!");
                            continue;
                        }


                        int[] currrSurfaceIds = new int[] { };

                        // Updating the load dictionary
                        UpdateLoadIdDictionary(bhLoad);

                        //Has surface been added to the Load? If not , get all surface IDs
                        if (surfaceIds.Count() == 0 && (bhLoad as GeometricalLineLoad).Objects is null || (bhLoad as GeometricalLineLoad).Objects.Elements.Count == 0 || ((bhLoad as GeometricalLineLoad).Objects.Elements.First() is null))
                        {
                            surfaceIds = m_Model.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
                        }
                        // If Surface has been added to the Load, get current Surface IDs
                        if (!((bhLoad as GeometricalLineLoad).Objects is null) && (bhLoad as GeometricalLineLoad).Objects.Elements.Count() != 0 && !((bhLoad as GeometricalLineLoad).Objects.Elements.First() is null))
                        {
                            //currrSurfaceIds = (bhLoad as GeometricalLineLoad).Objects.Elements.ToList().Select(e => (e as Panel).GetRFEM6ID()).ToArray();
                            List<Panel> panelCachList = GetCachedOrRead<Panel>();
                            currrSurfaceIds = (bhLoad as GeometricalLineLoad).Objects.Elements.ToList().Select(e => m_PanelIDdict[e as Panel]).ToArray();


                        }
                        else
                        {
                            currrSurfaceIds = surfaceIds;
                        }
                        // 
                        int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name + "_FreeLineLoad"];
                        free_line_load rfFreeLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(id, currrSurfaceIds);
                        m_Model.set_free_line_load(bhLoad.Loadcase.Number, rfFreeLineLoad);
                        continue;
                    }

                }

            }

            return true;
        }

        //Metho checks if ILoad is either a Moment or a Force Load
        private object MomentOfForceLoad(ILoad bhLoad)
        {

            bool momentHasBeenSet;
            bool forceHasBeenSet;
            object bhLoadType;

            if (bhLoad is PointLoad)
            {

                PointLoad bhPointLoad = bhLoad as PointLoad;
                momentHasBeenSet = !(bhPointLoad.Moment.X == 0 && bhPointLoad.Moment.Y == 0 && bhPointLoad.Moment.Z == 0);
                forceHasBeenSet = !(bhPointLoad.Force.X == 0 && bhPointLoad.Force.Y == 0 && bhPointLoad.Force.Z == 0);
                bhLoadType = forceHasBeenSet == true ? nodal_load_load_type.LOAD_TYPE_FORCE : nodal_load_load_type.LOAD_TYPE_MOMENT;

            }
            else if (bhLoad is BarUniformlyDistributedLoad)
            {

                BarUniformlyDistributedLoad bhBarLoad = bhLoad as BarUniformlyDistributedLoad;
                momentHasBeenSet = !(bhBarLoad.Moment.X == 0 && bhBarLoad.Moment.Y == 0 && bhBarLoad.Moment.Z == 0);
                forceHasBeenSet = !(bhBarLoad.Force.X == 0 && bhBarLoad.Force.Y == 0 && bhBarLoad.Force.Z == 0);
                bhLoadType = forceHasBeenSet == true ? member_load_load_type.LOAD_TYPE_FORCE : member_load_load_type.LOAD_TYPE_MOMENT;

            }
            else if (bhLoad is GeometricalLineLoad)
            {
                //Implicitly assuming that the GeometricalLineLoad is is a Non-Free Line Load as Free Line Loads only allow forces

                GeometricalLineLoad geolLineload = bhLoad as GeometricalLineLoad;
                if ((geolLineload.ForceA.Length() == 0 && geolLineload.ForceB.Length() == 0) && (geolLineload.MomentA.Length() == 0 && geolLineload.MomentB.Length() == 0)) { return null; }

                if (Math.Abs(BH.Engine.Geometry.Query.IsParallel(geolLineload.ForceA, geolLineload.ForceB)) != 1 && (geolLineload.ForceA.Length() > 0 && geolLineload.ForceB.Length() > 0))
                {
                    return null;
                }
                else if (Math.Abs(BH.Engine.Geometry.Query.IsParallel(geolLineload.MomentA, geolLineload.MomentA)) != 1 && (geolLineload.MomentA.Length() > 0 && geolLineload.MomentB.Length() > 0))
                {
                    return null;
                }
                else
                {
                    //boolean for check of atleas one moment Vector that has been set
                    momentHasBeenSet = !(geolLineload.MomentA.X == 0 && geolLineload.MomentA.Y == 0 && geolLineload.MomentA.Z == 0);
                    momentHasBeenSet = momentHasBeenSet || !(geolLineload.MomentB.X == 0 && geolLineload.MomentB.Y == 0 && geolLineload.MomentB.Z == 0);

                    //boolean for check of atleas one force Vector that has been set
                    forceHasBeenSet = !(geolLineload.ForceA.X == 0 && geolLineload.ForceA.Y == 0 && geolLineload.ForceA.Z == 0);
                    forceHasBeenSet = forceHasBeenSet || !(geolLineload.ForceB.X == 0 && geolLineload.ForceB.Y == 0 && geolLineload.ForceB.Z == 0);

                    bhLoadType = forceHasBeenSet == true ? line_load_load_type.LOAD_TYPE_FORCE : line_load_load_type.LOAD_TYPE_MOMENT;
                }
            }
            else
            {
                return null;
            }


            if (momentHasBeenSet && forceHasBeenSet)
            {

                BH.Engine.Base.Compute.RecordError($"The Load {bhLoad} does both include definitions for Moment Vector and Force Vector. Please try to seperate those and push them individually!");
                return null;
            }

            if (!momentHasBeenSet && !forceHasBeenSet)
            {
                BH.Engine.Base.Compute.RecordError($"The Load {bhLoad} does not include Vectors representing Moments or Forces!");
                return null;
            }


            else if (momentHasBeenSet)
            {
                return bhLoadType;

            }
            else if (forceHasBeenSet)
            {
                return bhLoadType;

            }
            else
            {
                BH.Engine.Base.Compute.RecordError($"Something went wrong! Please Check the Load {bhLoad}!");
                return null;
            }


        }


        private bool DirectionVectorIsXYZAxisParallel(Vector vector)
        {
            bool isParallel = false;

            if (vector.X != 0 && ((vector.Y == 0 && vector.Z == 0)))
            {

                isParallel = true;

            }
            else if (vector.Y != 0 && ((vector.X == 0 && vector.Z == 0)))
            {


                isParallel = true;

            }
            else if (vector.Z != 0 && ((vector.X == 0 && vector.Y == 0)))
            {


                isParallel = true;

            }
            else if (vector.X == 0 && vector.Y == 0 && vector.Z == 0)
            {


                isParallel = true;

            }


            return isParallel;

        }

        private  List<BarUniformlyDistributedLoad> SplitLoadIntoAxisAlignedLoads( BarUniformlyDistributedLoad barLoad)
        {
            // Only works for BarUniformlyDistributedLoad

            //if (barLoad == null)
            //    return null;

            // Example: Create a dictionary with "X", "Y", "Z" as keys and corresponding values from a vector
            var moment_dict = new Dictionary<string, double>
            {
                { "X", barLoad.Moment.X },
                { "Y", barLoad.Moment.Y },
                { "Z", barLoad.Moment.Z }
            };

            var force_dict = new Dictionary<string, double>
            {
                { "X", barLoad.Force.X },
                { "Y", barLoad.Force.Y },
                { "Z", barLoad.Force.Z }
            };


            int nonZeroMoment = moment_dict.Values.Count(x => Math.Abs(x) > 1e-8);
            int nonZeroForce = force_dict.Values.Count(x => Math.Abs(x) > 1e-8);

            // If only one axis is non-zero for moment or force, return as is
            if ((nonZeroMoment == 1 && nonZeroForce == 0) || (nonZeroForce == 1 && nonZeroMoment == 0))
                return new List<BarUniformlyDistributedLoad> { barLoad };

            List<BarUniformlyDistributedLoad> resultList = new List<BarUniformlyDistributedLoad>();

            int index = 0;
            foreach (KeyValuePair<string, double> kvp in moment_dict)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                if (Math.Abs(kvp.Value) > 0)
                {
                    BarUniformlyDistributedLoad barMoment = barLoad.DeepClone();
                    barMoment.Force = BH.Engine.Geometry.Create.Vector(0, 0, 0);
                    double x_coord = index == 0 ? barMoment.Moment.X : 0;
                    double y_coord = index == 1 ? barMoment.Moment.Y : 0;
                    double z_coord = index == 2 ? barMoment.Moment.Z : 0;
                    barMoment.Moment = new BH.oM.Geometry.Vector() { X = x_coord, Y = y_coord, Z = z_coord };
                    barMoment.Name = $"{barLoad.Name}_{kvp.Key}_Moment";
                    resultList.Add(barMoment);

                }
                index++;
            }

            index = 0;
            foreach (KeyValuePair<string, double> kvp in force_dict)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                if (Math.Abs(kvp.Value) > 0)
                {
                    BarUniformlyDistributedLoad barForce = barLoad.DeepClone();
                    barForce.Moment = BH.Engine.Geometry.Create.Vector(0, 0, 0);
                    double x_coord = index == 0 ? barForce.Force.X : 0;
                    double y_coord = index == 1 ? barForce.Force.Y : 0;
                    double z_coord = index == 2 ? barForce.Force.Z : 0;
                    barForce.Force = new BH.oM.Geometry.Vector() { X = x_coord, Y = y_coord, Z = z_coord };
                    barForce.Name = $"{barLoad.Name}_{kvp.Key}_Force";
                    resultList.Add(barForce);

                }
                index++;
            }

            // If both are zero, or more than one axis is non-zero, splitting is required or invalid
            // This method currently does not implement splitting, so return null
            return resultList;
        }

    }

}


