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
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.Engine.Spatial;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Panel> ReadPanels(List<string> ids = null)
        {

            var panelNumbersByType = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SURFACE);
            var allRfPanels = panelNumbersByType.ToList().Select(n => m_Model.get_surface(n.no)).ToList();
            List<int> panelNumbers = panelNumbersByType.ToList().Select(p => p.no).ToList();

            List<Panel> bhPanels = new List<Panel>();

            Dictionary<int, Edge> edges = this.GetCachedOrReadAsDictionary<int, Edge>();
            Dictionary<int, ISurfaceProperty> surfaceProperties = this.GetCachedOrReadAsDictionary<int, ISurfaceProperty>();
            Dictionary<int, RFEMOpening> surfaceOpening = this.GetCachedOrReadAsDictionary<int, RFEMOpening>();

            HashSet<int> surfaceWithOpeningIDs = surfaceOpening.SelectMany(o => o.Value.SurfaceIDs).ToHashSet();

            Dictionary<int, HashSet<int>> surfaceOpeningIdDicionary = new Dictionary<int, HashSet<int>>();

            foreach (int s in panelNumbers)
            {

                HashSet<int> openingIds = new HashSet<int>();

                if (surfaceWithOpeningIDs.Contains(s))
                {


                    foreach (KeyValuePair<int, RFEMOpening> entry in surfaceOpening)
                    {

                        if (entry.Value.SurfaceIDs.ToHashSet().Contains(s))
                        {
                            openingIds.Add(entry.Value.Opening.GetRFEM6ID());
                        }


                    }

                }

                surfaceOpeningIdDicionary.Add(s, openingIds);

            }

            //ComparerPoin
            //foreach (rfModel.surface rfPanel in allRfPanels)
            //{

            //    var bhPanel= rfPanel.FromRFEM(edges, surfaceProperties, surfaceOpeningIdDicionary[rfPanel.no], surfaceOpening);
            //    bhPanels.Add(bhPanel);
            //    HashSet<Point> panelPointSet= new HasSet<Point> { bhPanel.ExternalEdges.Select(b => b.Curve.ControlPoints()) };

            //}

            

            return bhPanels;
        }

    }
}
