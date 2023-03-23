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
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<Panel> bhPanels)
        {
            Dictionary<int, Edge> edges = this.GetCachedOrReadAsDictionary<int, Edge>();
            foreach (Panel bhPanel in bhPanels) 
            {
                bhPanel.ExternalEdges.Select(e=>e.GetRFEM6ID()).ToArray();
                List<int> edgeList=new List<int>();


                foreach (var e in bhPanel.ExternalEdges) { 
                
                    edgeList.Add(e.GetRFEM6ID());
                
                }
                int[] edgeArray = edgeList.ToArray();

                rfModel.surface surface = new rfModel.surface
                {
                    no = bhPanel.GetRFEM6ID(),
                    material = bhPanel.Property.Material.GetRFEM6ID(),
                    materialSpecified = true,
                    thickness = bhPanel.Property.GetRFEM6ID(),
                    // boundary_lines = bhPanel.ExternalEdges.Select(e => e.GetRFEM6ID()).ToArray(),
                    boundary_lines = edgeArray,
                    type = rfModel.surface_type.TYPE_STANDARD,
                    typeSpecified = true,
                    geometry = rfModel.surface_geometry.GEOMETRY_PLANE,
                    geometrySpecified = true,

                };

                var rfSurfaceProperties = m_Model.get_thickness(bhPanel.Property.GetRFEM6ID());
                HashSet<int> collectionOFPropertyNo = rfSurfaceProperties.assigned_to_surfaces.ToHashSet();
        
                m_Model.set_surface(surface);
            }
          

            return true;
        }

    }
}
