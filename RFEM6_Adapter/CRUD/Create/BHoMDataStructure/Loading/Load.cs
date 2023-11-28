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

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<ILoad> bhLoads)
        {
            //Surfaceis for GeometricalLineLoad
            int[] surfaceIds = new int[] { };

            foreach (ILoad bhLoad in bhLoads)
            {


                if (bhLoad is AreaUniformlyDistributedLoad)
                {

                    UpdateLoadIdDictionary(bhLoad);
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    surface_load rfemAreaLoad = (bhLoad as AreaUniformlyDistributedLoad).ToRFEM6(id);
                    m_Model.set_surface_load(bhLoad.Loadcase.GetRFEM6ID(), rfemAreaLoad);
                    continue;
                }

                object nodalLoadType = MomentOfForceLoad(bhLoad);
                if (nodalLoadType is null) continue;

                if (bhLoad is BarUniformlyDistributedLoad)
                {

                    if (!DirectionVectorIsXYZAxisParallel((bhLoad as BarUniformlyDistributedLoad).Force))
                    {

                        BH.Engine.Base.Compute.RecordError($"The Force Vector of {bhLoad} is not aligned with the X, Y or Z axis. Please make sure your Barload is either parallel to the X, Y or Z axis!");

                        continue;
                    }
                    if (!DirectionVectorIsXYZAxisParallel((bhLoad as BarUniformlyDistributedLoad).Moment))
                    {

                        BH.Engine.Base.Compute.RecordError($"The Moment Vector of {bhLoad} is not aligned with the X, Y or Z axis. Please make sure your Barload is either parallel to the X, Y or Z axis!");

                        continue;
                    }

                    UpdateLoadIdDictionary(bhLoad);
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    member_load member_load = (bhLoad as BarUniformlyDistributedLoad).ToRFEM6((member_load_load_type)nodalLoadType, id);
                    var rfMemberLoad = member_load;
                    m_Model.set_member_load(bhLoad.Loadcase.GetRFEM6ID(), rfMemberLoad);
                    continue;
                }

                if (bhLoad is PointLoad)
                {

                    UpdateLoadIdDictionary(bhLoad);
                    int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                    nodal_load rfPointLoad = (bhLoad as PointLoad).ToRFEM6((nodal_load_load_type)nodalLoadType, id);
                    m_Model.set_nodal_load(bhLoad.Loadcase.GetRFEM6ID(), rfPointLoad);
                    continue;

                }

                if (bhLoad is GeometricalLineLoad)
                {
                    // Handling Non-Free Line Loads
                    if (bhLoad.Name != "Free")
                    {
                        List<Panel> panelCachList = GetCachedOrRead<Panel>();
                        var lineCachDict = GetCachedOrReadAsDictionary<Line, int>();
                        //var lineProspects = panelCachList.SelectMany(p => p.ExternalEdges.SelectMany(e=>e.Curve.ControlPoints().Count==2)).Distinct();
                        var edgeProspects = panelCachList.SelectMany(p => p.ExternalEdges).Where(e => (e.Curve is Line) || (e.Curve is Polyline && e.Curve.ControlPoints().Count == 2)).Distinct().ToHashSet();
                        HashSet<Edge> edgeProsepectSet = new HashSet<Edge>(edgeProspects, new EdgeComparer());
                        bool locationLineIsValid = edgeProsepectSet.Contains(new Edge { Curve = (bhLoad as GeometricalLineLoad).Location });

                        if (locationLineIsValid)
                        {

                            UpdateLoadIdDictionary(bhLoad);
                            int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                            var locatedEdte=edgeProsepectSet.Where(e => (new EdgeComparer()).Equals(e, new Edge() { Curve = (bhLoad as GeometricalLineLoad).Location })).FirstOrDefault();
                            line_load rfLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(id,locatedEdte.GetRFEM6ID());
                            m_Model.set_line_load(bhLoad.Loadcase.GetRFEM6ID(), rfLineLoad);
                            continue;
                        }
                        else
                        {
                            BH.Engine.Base.Compute.RecordError($"The Location Line of {(bhLoad as GeometricalLineLoad)} is not valid. Please make sure your Location Line is located on a panel Edge! Also make sure that the location line and the panel line are oriented in the same way!");
                            continue;
                        }


                        //var lineProspects = panelCachList.SelectMany(p => p.ExternalEdges).Select(e => e.Curve).Where(c => (c is Line) || (c is Polyline && c.ControlPoints().Count == 2)).Distinct().ToList();

                        //rfemLineList.Any(p=>p.ExternalEdges.Any(e=>((e.Curve is Line)&&(new HashSet<Point>() {e.Curve.ControlPoints}))));

                        //int[] currrSurfaceIds = new int[] { };

                        //UpdateLoadIdDictionary(bhLoad);
                        //if (surfaceIds.Count() == 0 && (bhLoad as GeometricalLineLoad).Objects is null)
                        //{
                        //    surfaceIds = m_Model.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
                        //}
                        //if (!((bhLoad as GeometricalLineLoad).Objects is null))
                        //{
                        //    currrSurfaceIds = (bhLoad as GeometricalLineLoad).Objects.Elements.ToList().Select(e => (e as Panel).GetRFEM6ID()).ToArray();

                        //}
                        //else
                        //{
                        //    currrSurfaceIds = surfaceIds;
                        //}


                        //int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                        //free_line_load rfFreeLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(id, currrSurfaceIds);
                        //m_Model.set_free_line_load(bhLoad.Loadcase.GetRFEM6ID(), rfFreeLineLoad);


                    }

                    else
                    {
                        // Hadling Free Line Loads
                        int[] currrSurfaceIds = new int[] { };

                        UpdateLoadIdDictionary(bhLoad);
                        if (surfaceIds.Count() == 0 && (bhLoad as GeometricalLineLoad).Objects is null)
                        {
                            surfaceIds = m_Model.get_all_object_numbers(object_types.E_OBJECT_TYPE_SURFACE, 0);
                        }
                        if (!((bhLoad as GeometricalLineLoad).Objects is null))
                        {
                            currrSurfaceIds = (bhLoad as GeometricalLineLoad).Objects.Elements.ToList().Select(e => (e as Panel).GetRFEM6ID()).ToArray();

                        }
                        else
                        {
                            currrSurfaceIds = surfaceIds;
                        }


                        int id = m_LoadcaseLoadIdDict[bhLoad.Loadcase][bhLoad.GetType().Name];
                        free_line_load rfFreeLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(id, currrSurfaceIds);
                        m_Model.set_free_line_load(bhLoad.Loadcase.GetRFEM6ID(), rfFreeLineLoad);
                        continue;
                    }

                }

                //else if (bhLoad is GeometricalLineLoad)
                //{
                //    Node n0 = new Node() { Position = (bhLoad as GeometricalLineLoad).Location.Start };
                //    Node n1 = new Node() { Position = (bhLoad as GeometricalLineLoad).Location.End };

                //    int lineNo = nestedNodeToIDMap[n0][n1];

                //    line_load rfLineLoad = (bhLoad as GeometricalLineLoad).ToRFEM6(new List<int>() { lineNo });
                //    m_Model.set_line_load(bhLoad.Loadcase.GetRFEM6ID(), rfLineLoad);

                //}



            }

            return true;
        }


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
                //bhLoadType = nodal_load_load_type.LOAD_TYPE_FORCE;
                bhLoadType = forceHasBeenSet == true ? nodal_load_load_type.LOAD_TYPE_FORCE : nodal_load_load_type.LOAD_TYPE_MOMENT;

            }
            else if (bhLoad is BarUniformlyDistributedLoad)
            {

                BarUniformlyDistributedLoad bhBarLoad = bhLoad as BarUniformlyDistributedLoad;
                momentHasBeenSet = !(bhBarLoad.Moment.X == 0 && bhBarLoad.Moment.Y == 0 && bhBarLoad.Moment.Z == 0);
                forceHasBeenSet = !(bhBarLoad.Force.X == 0 && bhBarLoad.Force.Y == 0 && bhBarLoad.Force.Z == 0);
                //bhLoadType = member_load_load_type.LOAD_TYPE_FORCE;
                bhLoadType = forceHasBeenSet == true ? member_load_load_type.LOAD_TYPE_FORCE : member_load_load_type.LOAD_TYPE_MOMENT;
                //BarLoadErrorMessage(bhBarLoad.Force);
                //BarLoadErrorMessage(bhBarLoad.Moment);

            }
            else if (bhLoad is GeometricalLineLoad)
            {

                GeometricalLineLoad geolLineload = bhLoad as GeometricalLineLoad;
                if ((geolLineload.ForceA.Length() == 0 && geolLineload.ForceB.Length() == 0) && (geolLineload.MomentA.Length() == 0 && geolLineload.MomentB.Length() == 0)) { return null; }

                if (Math.Abs(BH.Engine.Geometry.Query.IsParallel(geolLineload.ForceA, geolLineload.ForceB)) != 1 && (geolLineload.ForceA.Length() + geolLineload.ForceB.Length() > 0))
                {
                    return null;
                }
                else if (Math.Abs(BH.Engine.Geometry.Query.IsParallel(geolLineload.MomentA, geolLineload.MomentA)) != 1 && (geolLineload.MomentA.Length() + geolLineload.MomentB.Length() > 0))
                {
                    return null;
                }
                else
                {
                    momentHasBeenSet = !(geolLineload.MomentA.X == 0 && geolLineload.MomentA.Y == 0 && geolLineload.MomentA.Z == 0);
                    forceHasBeenSet = !(geolLineload.ForceA.X == 0 && geolLineload.ForceA.Y == 0 && geolLineload.ForceA.Z == 0);
                    //bhLoadType = member_load_load_type.LOAD_TYPE_FORCE;
                    bhLoadType = forceHasBeenSet == true ? member_load_load_type.LOAD_TYPE_FORCE : member_load_load_type.LOAD_TYPE_MOMENT;
                    //BarLoadErrorMessage(bhBarLoad.Force);
                    //BarLoadErrorMessage(bhBarLoad.Moment);
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

                return null;
            }


        }


        //private void BarLoadErrorMessage(Vector vector) {

        //    if (vector.X != 0 && (!(vector.Y == 0 || vector.Z == 0))){ 

        //        BH.Engine.Base.Compute.RecordWarning($"The Load Vector {vector} is not aligned with the X, Y or Z axis. Please make sure your Barload is either parallel to the X, Y or Z axis!");

        //    }    


        //}

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

    }

}
