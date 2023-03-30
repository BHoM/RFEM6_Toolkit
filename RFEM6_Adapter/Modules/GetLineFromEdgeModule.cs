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
using BH.oM.Base;
using BH.oM.Adapter;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.RFEM6;
using System.Collections;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Adapter.RFEM6
{
    [Description("Dependency module for fetching all Loadcase stored in a list of Loadcombinations.")]
    public class GetLineFromEdgeModule : IGetDependencyModule<Edge, RFEMLine>
    {
        public IEnumerable<RFEMLine> GetDependencies(IEnumerable<Edge> objects)
        {
            List< RFEMLine> lines = new List<RFEMLine>();
            foreach (Edge edge in objects)
            {
                RFEMLine rfLine =null;

                if(edge.Curve is Line line)
                {
                    rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = line.Start }, new Node { Position = line.End } }, LineType = RFEMLineType.Polyline };
                }
                //Add for other line types and add convert in terms of nodes accordingly
                if (edge.Curve is Arc arc)
                {

                    Point[] pts=arc.ControlPoints().ToArray();
                    
                    rfLine = new RFEMLine() { Nodes = new List<Node> { new Node { Position = pts[0] }, new Node { Position = pts[1] }, new Node { Position = pts[2] }, new Node { Position = pts[3] }, new Node { Position = pts[4] } }, LineType = RFEMLineType.Arc };
                }


                edge.Fragments.Add(rfLine);
                lines.Add(rfLine);
            }

            return lines;
        }
    }
}