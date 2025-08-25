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
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Structure;
using BH.oM.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Results;
using BH.oM.Structure.SectionProperties;
using Dlubal.WS.Rfem6.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using rfModel = Dlubal.WS.Rfem6.Model;

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
                    CreateLoad(bhLoad as AreaUniformlyDistributedLoad);
                    continue;
                }

                if (bhLoad is BarUniformlyDistributedLoad)
                {
                    CreateLoad(bhLoad as BarUniformlyDistributedLoad);
                    continue;
                }

                if (bhLoad is PointLoad)
                {
                    CreateLoad(bhLoad as PointLoad);
                    continue;
                }

                if (bhLoad is GeometricalLineLoad)
                {
                    // Handling Non-Free Line Loads

                    //Checking if the GeometricalLineLoad has a geometricalLineLoadType Fragment
                    bool lineLoadhasFragments = bhLoad.Fragments.ToList().Any(f => f.GetType().Name == "RFEM6GeometricalLineLoadTypes");
                    if (!lineLoadhasFragments)
                        BH.Engine.Base.Compute.RecordWarning($"{bhLoad} has no geometricalLineLoadType set. As a default value geometricalLineLoadType.geometricalLineLoadEnum has been set to FreeLineLoad.\n In case you want to generate LineLoads please add the fragment geometricalLineLoadTyp to the {bhLoad} and set the parameters accordingly.");

                    // checking if the GeometricalLineLoad has a geometricalLineLoadType Fragment and if it is not FreeLineLoad
                    bool isNonFreeLineLoad = lineLoadhasFragments && BH.Engine.Base.Query.GetAllFragments(bhLoad)[0].PropertyValue("geometrialLineLoadType").ToString() != "FreeLineLoad";

                    //If Load is a Non-Free Line Load
                    if (lineLoadhasFragments && isNonFreeLineLoad){
                        CreateLoad_NonFreeLineLoad(bhLoad as GeometricalLineLoad);
                        continue;
                    }
                    else
                    {
                    // Handling FreeLineLoad


                        // Checking if Moment and Force Vectors have been mixed. Free Line Loads only allow Forces or only Moments
                        if ((bhLoad as GeometricalLineLoad).MomentA.Length() > 0 || (bhLoad as GeometricalLineLoad).MomentB.Length() > 0)
                        {
                            BH.Engine.Base.Compute.RecordError($"Free Line Loads do not allow Moments. Please remove the Moment Vector from the Load {bhLoad}!");
                            continue;
                        }

                        CreateLoad_FreeLineLoad(bhLoad as GeometricalLineLoad, ref surfaceIds);

                        
                        continue;
                    }

                }

            }

            return true;
        }

        //Metho checks if ILoad is either a Moment or a Force Load
        private object MomentOrForceLoad(ILoad bhLoad)
        {

            bool momentHasBeenSet;
            bool forceHasBeenSet;
            object bhLoadType;


            if (bhLoad is PointLoad)
            {

                PointLoad bhPointLoad = bhLoad as PointLoad;
                momentHasBeenSet = !((bhLoad as PointLoad).Moment.Length() < 1e-8);
                forceHasBeenSet = !((bhLoad as PointLoad).Force.Length() < 1e-8);
                bhLoadType = forceHasBeenSet == true ? nodal_load_load_type.LOAD_TYPE_FORCE : nodal_load_load_type.LOAD_TYPE_MOMENT;

            }
            else if (bhLoad is BarUniformlyDistributedLoad)
            {

                BarUniformlyDistributedLoad bhBarLoad = bhLoad as BarUniformlyDistributedLoad;
                momentHasBeenSet = !(bhBarLoad.Moment.Length() < 1e-8);
                forceHasBeenSet = !(bhBarLoad.Force.Length() < 1e-8);
                bhLoadType = forceHasBeenSet == true ? member_load_load_type.LOAD_TYPE_FORCE : member_load_load_type.LOAD_TYPE_MOMENT;

            }
            else if (bhLoad is GeometricalLineLoad)
            {
                //Implicitly assuming that the GeometricalLineLoad is is a Non-Free Line Load as Free Line Loads only allow forces

                GeometricalLineLoad geolLineload = bhLoad as GeometricalLineLoad;

                if (geolLineload.ForceA.Length() < 1e-8 && geolLineload.ForceB.Length() < 1e-8
                    && geolLineload.MomentA.Length() < 1e-8 && geolLineload.MomentB.Length() < 1e-8)
                {
                    return null;
                }

                if ((geolLineload.ForceA.Length() > 0 && geolLineload.ForceB.Length() > 0) &&
                    Math.Abs(BH.Engine.Geometry.Query.IsParallel(geolLineload.ForceA, geolLineload.ForceB)) != 1)
                {
                    return null;
                }
                else if ((geolLineload.MomentA.Length() > 0 && geolLineload.MomentB.Length() > 0) &&
                    Math.Abs(BH.Engine.Geometry.Query.IsParallel(geolLineload.MomentA, geolLineload.MomentA)) != 1)
                {
                    return null;
                }
                else
                {
                    //boolean for check of atleas one moment Vector that has been set
                    momentHasBeenSet = geolLineload.MomentA.Length() > 1e-8 || geolLineload.MomentB.Length() > 1e-8;

                    //boolean for check of atleas one force Vector that has been set
                    forceHasBeenSet =geolLineload.ForceA.Length() > 1e-8 || geolLineload.ForceB.Length() > 1e-8;

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



        private void CreateLoad(AreaUniformlyDistributedLoad bhLoad)
        {
            UpdateLoadIdDictionary(bhLoad);
            //Call Panel Load Methond to update the Panel ID Dictionary
            this.GetCachedOrReadAsDictionary<int, Panel>();
            int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
            surface_load rfemAreaLoad = (bhLoad as AreaUniformlyDistributedLoad).ToRFEM6(id);
            var currrSurfaceIds = (bhLoad as AreaUniformlyDistributedLoad).Objects.Elements.ToList().Select(e => m_PanelIDdict[e as Panel]).ToArray();
            rfemAreaLoad.surfaces = currrSurfaceIds;
            m_Model.set_surface_load(bhLoad.Loadcase.Number, rfemAreaLoad);

        }

        private void CreateLoad(BarUniformlyDistributedLoad bhLoad)
        {

            object loadType = null;

            var splitLoadList = SplitLoadIntoAxisParallelLoads((bhLoad as BarUniformlyDistributedLoad));

            foreach (var b in splitLoadList)
            {
                loadType = MomentOrForceLoad(b);
                UpdateLoadIdDictionary(bhLoad);
                int id_ = m_LoadcaseLoadIdDict[b.Loadcase][b.GetType().Name];
                member_load member_load_ = (b as BarUniformlyDistributedLoad).ToRFEM6((member_load_load_type)loadType, id_);
                m_Model.set_member_load(bhLoad.Loadcase.Number, member_load_);
            }
        }

        private void CreateLoad(PointLoad bhLoad)
        {
            object loadType = null;
            loadType = MomentOrForceLoad(bhLoad);
            UpdateLoadIdDictionary(bhLoad);
            int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
            nodal_load rfPointLoad = (bhLoad as PointLoad).ToRFEM6((nodal_load_load_type)loadType, id);
            m_Model.set_nodal_load(bhLoad.Loadcase.Number, rfPointLoad);

        }

        private void CreateLoad_NonFreeLineLoad(GeometricalLineLoad bhLoad)
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
                line_load rfLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(id, locatedEdte.GetRFEM6ID(), (line_load_load_type)MomentOrForceLoad(bhLoad));
                m_Model.set_line_load(bhLoad.Loadcase.Number, rfLineLoad);
            }
            else
            {
                BH.Engine.Base.Compute.RecordError($"The Location Line of {(bhLoad as GeometricalLineLoad)} is not valid. Please make sure your Location Line is located on a panel Edge! Also make sure that the location line and the panel line are oriented in the same way!");
            }

        }

        private void CreateLoad_FreeLineLoad(GeometricalLineLoad bhLoad,ref int[] surfaceIds)
        {

            //Container for Potential Surface IDs. Surface IDs are onle relevant when using Free Line Loads
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

        }


        private List<BarUniformlyDistributedLoad> SplitLoadIntoAxisParallelLoads(BarUniformlyDistributedLoad barLoad)
        {
      

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
            {
                barLoad.Name = $"Name::{barLoad.Name}::GUID::{barLoad.BHoM_Guid}";
                return new List<BarUniformlyDistributedLoad> { barLoad };
            }

            List<BarUniformlyDistributedLoad> resultList = new List<BarUniformlyDistributedLoad>();

            int index = 0;
            foreach (KeyValuePair<string, double> kvp in moment_dict)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                if (Math.Abs(kvp.Value) > 0)
                {
                    //BarUniformlyDistributedLoad barMoment = barLoad.DeepClone();
                    BH.oM.Geometry.Vector vec_f = BH.Engine.Geometry.Create.Vector(0, 0, 0);
                    BH.oM.Geometry.Vector vec_m = BH.Engine.Geometry.Create.Vector(0, 0, 0);

                    BarUniformlyDistributedLoad barMoment = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(barLoad.Loadcase, barLoad.Objects, BH.Engine.Geometry.Create.Vector(0, 0, 0), BH.Engine.Geometry.Create.Vector(0, 0, 0), barLoad.Axis, barLoad.Projected, barLoad.Name);
                    double x_coord = index == 0 ? barLoad.Moment.X : 0;
                    double y_coord = index == 1 ? barLoad.Moment.Y : 0;
                    double z_coord = index == 2 ? barLoad.Moment.Z : 0;
                    barMoment.Force = BH.Engine.Geometry.Create.Vector(0, 0, 0);
                    barMoment.Moment = new BH.oM.Geometry.Vector() { X = x_coord, Y = y_coord, Z = z_coord };
                    barMoment.Name = $"Name::{barLoad.Name}::GUID::{barLoad.BHoM_Guid}";
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
                    //BarUniformlyDistributedLoad barForce = barLoad.DeepClone();
                    BarUniformlyDistributedLoad barForce = BH.Engine.Structure.Create.BarUniformlyDistributedLoad(barLoad.Loadcase, barLoad.Objects, BH.Engine.Geometry.Create.Vector(0, 0, 0), BH.Engine.Geometry.Create.Vector(0, 0, 0), barLoad.Axis, barLoad.Projected, barLoad.Name);
                    barForce.Moment = BH.Engine.Geometry.Create.Vector(0, 0, 0);
                    double x_coord = index == 0 ? barLoad.Force.X : 0;
                    double y_coord = index == 1 ? barLoad.Force.Y : 0;
                    double z_coord = index == 2 ? barLoad.Force.Z : 0;
                    barForce.Force = new BH.oM.Geometry.Vector() { X = x_coord, Y = y_coord, Z = z_coord };
                    barForce.Name = $"Name::{barLoad.Name}::GUID::{barLoad.BHoM_Guid}";
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


