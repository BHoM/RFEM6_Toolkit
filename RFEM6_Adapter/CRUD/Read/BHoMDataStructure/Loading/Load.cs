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
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.Engine.Base;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {


        private List<ILoad> ReadBarLoad(List<string> ids = null)
        {
            //Find all possible Load cases
            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            List<int> loadCaseIds = loadCaseMap.Keys.ToList();
            Dictionary<int, Bar> memberMap = this.GetCachedOrReadAsDictionary<int, Bar>();


            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER_LOAD);

            IEnumerable<rfModel.member_load> foundLoadCases = numbers.SelectMany(n => n.children.ToList().Select(child => m_Model.get_member_load(child, n.no)));

            List<ILoad> loadCases = new List<ILoad>();
            foundLoadCases=foundLoadCases.OrderBy(n => n.load_case).ThenBy(t=>t.no);
            foreach (rfModel.member_load memberLoad in foundLoadCases)
            {

                loadCases.Add(memberLoad.FromRFEM(memberLoad.members.ToList().Select(m => memberMap[m]).ToList(), loadCaseMap[memberLoad.load_case]));

            }

            return loadCases;
        }

        private List<ILoad> ReadPointLoad(List<string> ids = null)
        {
            //Find all possible Load cases
            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Node> memberMap = this.GetCachedOrReadAsDictionary<int, Node>();


            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODAL_LOAD);

            //IEnumerable<rfModel.nodal_load> founeNodalLoad = numbers.ToList().Select(n => m_Model.get_nodal_load(n.children[0], n.no));
            IEnumerable<rfModel.nodal_load> founeNodalLoad = numbers.SelectMany(n => n.children.Select(child => m_Model.get_nodal_load(child, n.no)));

            List<ILoad> loads = new List<ILoad>();
            foreach (rfModel.nodal_load nodeLoad in founeNodalLoad)
            {
                if (nodeLoad.load_type == rfModel.nodal_load_load_type.LOAD_TYPE_MASS)
                {

                    BH.Engine.Base.Compute.RecordWarning($"Nodal Loads of type {nodeLoad.load_type} can not be read from RFEM6. If possible convert them to either Force, Moment or Components!");
                    continue;
                }

                loads.Add(nodeLoad.FromRFEM(nodeLoad.nodes.ToList().Select(m => memberMap[m]).ToList(), loadCaseMap[nodeLoad.load_case]));

            }

            return loads;
        }




        private List<ILoad> ReadAreaLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            //Find all possible Load cases
            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Panel> panelMap = this.GetCachedOrReadAsDictionary<int, Panel>();

            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SURFACE_LOAD);

            //IEnumerable<rfModel.surface_load> foundSurfaceLoad = numbers.ToList().Select(n => m_Model.get_surface_load(n.children[0], n.no));
            IEnumerable<rfModel.surface_load> foundSurfaceLoad = numbers.SelectMany(n => n.children.Select(child => m_Model.get_surface_load(child, n.no)));

            foundSurfaceLoad = foundSurfaceLoad.OrderBy(n => n.load_case).ThenBy(t => t.no);
            foreach (rfModel.surface_load surfaceLoad in foundSurfaceLoad)
            {
                List<Panel> panels = surfaceLoad.surfaces.ToList().Select(s => panelMap[s]).ToList();
                Loadcase loadcase = loadCaseMap[surfaceLoad.load_case];

                if (!(surfaceLoad.load_distribution is rfModel.surface_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM))
                {
                    Engine.Base.Compute.RecordNote("The current RFEM6 includes Surfaceloads with a non-uniformal load distributeion, these Loads will not be pulled.");
                    continue;
                }

                loads.Add(surfaceLoad.FromRFEM(loadCaseMap[surfaceLoad.load_case], panels));

            }

            return loads;
        }

        private List<ILoad> ReadFreeLineLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Panel> panelMap = this.GetCachedOrReadAsDictionary<int, Panel>();


            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_FREE_LINE_LOAD);
            //rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.line);


            //IEnumerable<rfModel.free_line_load> foundFreeLineLoads = numbers.ToList().Select(n => m_Model.get_free_line_load(n.children[0], n.no));
            IEnumerable<rfModel.free_line_load> foundFreeLineLoads = numbers.SelectMany(n => n.children.Select(child => m_Model.get_free_line_load(child, n.no)));


            foreach (rfModel.free_line_load freeLineLoad in foundFreeLineLoads)
            {
                List<Panel> panelList;
                if (freeLineLoad.surfaces.ToList().First() == 0)
                {
                    panelList = new List<Panel>();
                }
                else
                {

                    panelList = freeLineLoad.surfaces.ToList().Select(s => panelMap[s]).ToList();

                }
                loads.Add(freeLineLoad.FromRFEM(loadCaseMap[freeLineLoad.load_case], panelList));

            }

            return loads;
        }


        private List<ILoad> ReadLineLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Edge> edgeDictionary = this.GetCachedOrReadAsDictionary<int, Edge>();


            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_LINE_LOAD);
            //rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.line);


            //IEnumerable<rfModel.line_load> foundFreeLineLoads = numbers.ToList().Select(n => m_Model.get_line_load(n.children[0], n.no));
            IEnumerable<rfModel.line_load> foundFreeLineLoads = numbers.SelectMany(n => n.children.Select(child => m_Model.get_line_load(child, n.no)));


            foreach (rfModel.line_load lineLoad in foundFreeLineLoads)
            {

                List<ICurve> curveList = lineLoad.lines.ToList().Select(l => edgeDictionary[l].Curve).Where(cu => (cu is Line) || (cu is Polyline && cu.ControlPoints().Count == 2)).ToList();
                List<Line> lines = curveList.Select(k => new Line() { Start = k.ControlPoints().First(), End = k.ControlPoints().First() }).ToList();
                lines.ForEach(l => loads.Add(lineLoad.FromRFEM(loadCaseMap[lineLoad.load_case], l)));

            }

            return loads;
        }

        private void UpdateLoadIdDictionary(ILoad load)
        {




            //Determin LoadType. Lineloads are handled differently as there is the need to discriminate between free and non-free line loads
            var rfLoadType = load.GetType().ToRFEM6().Value;
            bool lineLoadhasFragments = false;
            bool isFreeLineLoad = false;
            if (load is GeometricalLineLoad geomLineLoad)
            {

                lineLoadhasFragments = load.Fragments.ToList().Any(f => f.GetType().Name == "RFEM6GeometricalLineLoadTypes");
                isFreeLineLoad = lineLoadhasFragments ? BH.Engine.Base.Query.GetAllFragments(load)[0].PropertyValue("geometrialLineLoadType").ToString().Equals("FreeLineLoad") : true;

                rfLoadType = (lineLoadhasFragments || isFreeLineLoad) ? rfModel.object_types.E_OBJECT_TYPE_FREE_LINE_LOAD : rfModel.object_types.E_OBJECT_TYPE_LINE_LOAD;
            }
            else
            {

                rfLoadType = load.GetType().ToRFEM6().Value;
            }

            //Check if the loadcase is already in the dictionary
            if (m_LoadcaseLoadIdDict.TryGetValue(load.Loadcase, out Dictionary<String, int> loadIdDict))
            {
                String type = load.GetType().Name;

                if (load is GeometricalLineLoad geoLineLoad)
                {
                    // Assuming 'Designation' is the property that indicates 'Free' or 'NonFree'
                    //type += "_" + geoLineLoad.Name; // e.g., "GeometricalLineLoad_Free"
                    type += "_" + (!isFreeLineLoad ? "NonFreeLineLoad" : "FreeLineLoad");

                }

                if (loadIdDict.TryGetValue(type, out int id))
                {
                    loadIdDict[type] = id + 1;
                }
                else
                {
                    int k = m_Model.get_first_free_number(rfLoadType, load.Loadcase.GetRFEM6ID());
                    loadIdDict.Add(type, k);
                }
            }
            else
            {
                var d = new Dictionary<string, int>();
                String type = load.GetType().Name;
                if (load is GeometricalLineLoad geoLineLoad)
                {
                    //type += "_" + geoLineLoad.Name;
                    type += "_" + (!isFreeLineLoad ? "NonFreeLineLoad" : "FreeLineLoad");
                    //var rfLoadType = geoLineLoad.Name == "Free" ? rfModel.object_types.E_OBJECT_TYPE_FREE_LINE_LOAD : rfModel.object_types.E_OBJECT_TYPE_LINE_LOAD;
                    d.Add(type, m_Model.get_first_free_number(rfLoadType, load.Loadcase.GetRFEM6ID()));
                    m_LoadcaseLoadIdDict.Add(load.Loadcase, d);

                }
                else
                {
                    d.Add(type, m_Model.get_first_free_number(load.GetType().ToRFEM6().Value, load.Loadcase.GetRFEM6ID()));
                    m_LoadcaseLoadIdDict.Add(load.Loadcase, d);
                }

            }

        }



    }

}


