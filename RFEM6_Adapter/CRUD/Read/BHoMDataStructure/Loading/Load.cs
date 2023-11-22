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
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

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

            //IEnumerable<rfModel.member_load> foundLoadCases = numbers.ToList().Select(n => m_Model.get_member_load(n.no, n.children[0]));
            //IEnumerable<rfModel.member_load> foundLoadCases = numbers.ToList().Select(n => m_Model.get_member_load(n.children[0], n.no));

            IEnumerable<rfModel.member_load> foundLoadCases = numbers.SelectMany(n => n.children.ToList().Select(child => m_Model.get_member_load(child, n.no)));



            List<ILoad> loadCases = new List<ILoad>();
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

            IEnumerable<rfModel.nodal_load> founeNodalLoad = numbers.ToList().Select(n => m_Model.get_nodal_load(n.children[0], n.no));


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

            IEnumerable<rfModel.surface_load> foundSurfaceLoad = numbers.ToList().Select(n => m_Model.get_surface_load(n.children[0], n.no));

            foreach (rfModel.surface_load surfaceLoad in foundSurfaceLoad)
            {
                List<Panel> panels = surfaceLoad.surfaces.ToList().Select(s => panelMap[s]).ToList();
                Loadcase loadcase = loadCaseMap[surfaceLoad.load_case];

                if (!(surfaceLoad.load_distribution is rfModel.surface_load_load_distribution.LOAD_DISTRIBUTION_UNIFORM))
                {
                    Engine.Base.Compute.RecordNote("The current RFEM6 includes Surfaceloads with a non-uniformal load distributeion, these Loads will not be pulled.");
                    continue;
                }

                loads.Add(surfaceLoad.FromRFEM(loadCaseMap[surfaceLoad.load_case], panels ));

            }

            return loads;
        }

        private List<ILoad> ReadLineLoad(List<string> ids = null)
        {
            List<ILoad> loads = new List<ILoad>();

            Dictionary<int, Loadcase> loadCaseMap = this.GetCachedOrReadAsDictionary<int, Loadcase>();
            Dictionary<int, Panel> panelMap = this.GetCachedOrReadAsDictionary<int, Panel>();


            rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_FREE_LINE_LOAD);
            //rfModel.object_with_children[] numbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.line);


            IEnumerable<rfModel.free_line_load> foundFreeLineLoads = numbers.ToList().Select(n => m_Model.get_free_line_load(n.children[0], n.no));

            foreach (rfModel.free_line_load freeLineLoad in foundFreeLineLoads)
            {

                loads.Add(freeLineLoad.FromRFEM(loadCaseMap[freeLineLoad.load_case], freeLineLoad.surfaces.ToList().Select(s => panelMap[s]).ToList()));

            }

            return loads;
        }


        private void UpdateLoadIdDictionary(ILoad load) { 



            if (m_LoadcaseLoadIdDict.TryGetValue(load.Loadcase, out Dictionary<String, int> loadIdDict))
            {

                String type = load.GetType().Name;

                if (loadIdDict.TryGetValue(type, out int id))
                {
                    loadIdDict[type] = id + 1;
                }
                else
                {

                    int k = m_Model.get_first_free_number(load.GetType().ToRFEM6().Value, load.Loadcase.GetRFEM6ID());
                    loadIdDict.Add(type, k);

                }


            }
            else
            {


                var d = new Dictionary<string, int>();
                d.Add(load.GetType().Name, m_Model.get_first_free_number(load.GetType().ToRFEM6().Value, load.Loadcase.GetRFEM6ID()));
                m_LoadcaseLoadIdDict.Add(load.Loadcase, d);

            }



        }

    }
}
