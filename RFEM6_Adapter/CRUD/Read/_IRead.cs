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
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Results;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Requests;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        // This method gets called when appropriate by the Pull method contained in the base Adapter class.
        // It gets called once per each Type.
        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {

            try
            {
                if (type == typeof(Node))
                    return ReadNodes(ids as dynamic);
                else if (type == typeof(RFEMNodalSupport))
                    return ReadRFEMNodalSupports(ids as dynamic);
                else if (type == typeof(RFEMHinge))
                    return ReadRFEMHinges(ids as dynamic);
                else if (type == typeof(ISectionProperty) || type.GetInterfaces().Contains(typeof(ISectionProperty)))
                    return ReadSectionProperties(ids as dynamic);
                else if (type == typeof(IMaterialFragment) || type.GetInterfaces().Contains(typeof(IMaterialFragment)))
                    return ReadMaterial(ids as dynamic);
                else if (type == typeof(Line) || type == typeof(RFEMLine))
                    return ReadLines(ids as dynamic);
                else if (type == typeof(Bar))
                    return ReadBars(ids as dynamic);
                else if (type == typeof(Panel))
                    return ReadPanels(ids as dynamic);
                else if (type == typeof(ISurfaceProperty))
                    return ReadSurfaceProperties(ids as dynamic);
                else if (type == typeof(Edge))
                    return ReadEdges(ids as dynamic);
                else if (type == typeof(RFEMOpening))
                    return ReadRFEMOpening(ids as dynamic);
                else if (type == typeof(Opening))
                    return ReadOpening(ids as dynamic);
                else if (type == typeof(Loadcase))
                    return ReadLoadCase(ids as dynamic);
                else if (type == typeof(BarUniformlyDistributedLoad))
                    return ReadBarLoad(ids as dynamic);
                else if (type == typeof(PointLoad))
                    return ReadPointLoad(ids as dynamic);
                else if (type == typeof(AreaUniformlyDistributedLoad))
                    return ReadAreaLoad(ids as dynamic);
                else if (type == typeof(GeometricalLineLoad))
                {
                    List<ILoad> loadList = new List<ILoad>();
                    loadList.AddRange(ReadFreeLineLoad(ids as dynamic));
                    loadList.AddRange(ReadLineLoad(ids as dynamic));

                    return loadList;
                }
                else if (type == typeof(ILoad))
                {
                    List<ILoad> loadList = new List<ILoad>();
                    loadList.AddRange(ReadPointLoad(ids as dynamic));
                    loadList.AddRange(ReadBarLoad(ids as dynamic));
                    loadList.AddRange(ReadFreeLineLoad(ids as dynamic));
                    loadList.AddRange(ReadLineLoad(ids as dynamic));
                    loadList.AddRange(ReadAreaLoad(ids as dynamic));
                    return loadList;
                }
                else if (type == typeof(NodeResultRequest))
                {
                    BH.Engine.Base.Compute.RecordError("Pull Of Results can not be filtered by type. Please use the 'NodeResultRequest' component instead.");
                    return null;
                }
            }
            finally
            {
                // AppUnlock();
            }
            return new List<IBHoMObject>(); ;
        }

        /***************************************************/

    }
}



