/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;
using BH.Engine.Adapter;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        //public static Panel FromRFEM(this rfModel.surface rfSurface, Dictionary<int, Edge> edgeDict, Dictionary<int, ISurfaceProperty> surfaceProperty, Dictionary<int,  HashSet<int>> openingIDDict, int rfPanelNo, Dictionary<int, RFEMOpening> surfaceOpening)
        public static Panel FromRFEM(this rfModel.surface rfSurface, Dictionary<int, Edge> edgeDict, Dictionary<int, ISurfaceProperty> surfaceProperty, HashSet<int> openingIDs, Dictionary<int, RFEMOpening> surfaceOpening)
        {

            //HashSet<int> openingIDs = new HashSet<int>();

            //openingIDDict.TryGetValue(rfPanelNo,out openingIDs);


            List<int> rfEdgeNumbers = rfSurface.boundary_lines.ToList();
            List<Edge> bhEdges = rfEdgeNumbers.Select(n => edgeDict[n]).ToList();
            Panel panel = new Panel();


            if (openingIDs.Count > 0)
            {

                List<Opening> openingins = new List<Opening>();

                foreach (int o in openingIDs)
                {
                    Opening opening = surfaceOpening[o].Opening;

                    openingins.Add(opening);

                }

                panel = Engine.Structure.Create.Panel(bhEdges, openingins, surfaceProperty[rfSurface.thickness]);

            }

            else
            {


                panel = Engine.Structure.Create.Panel(bhEdges, null, surfaceProperty[rfSurface.thickness]);

            }

            panel.SetRFEM6ID(rfSurface.no);

            if (rfSurface.comment.Count() != 0)
            {
                BH.Engine.Base.Modify.SetPropertyValue(panel, "Comment", rfSurface.comment);
            }
            return panel;
        }

    }
}



