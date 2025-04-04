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
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;
using BH.Engine.Adapter;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Geometry;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static RFEMOpening FromRFEM(this rfModel.opening rfOpening, Dictionary<int, Edge> edgeDict, List<int> surfaceIDs)
        {

            List<ICurve> curves = new List<ICurve>();

            rfOpening.boundary_lines.ToList().ForEach(l => curves.Add(edgeDict[l].Curve));

            PolyCurve polyCurve = Engine.Geometry.Create.PolyCurve(curves);

            var polyCurves = Engine.Geometry.Compute.Join(new List<PolyCurve>() { polyCurve });

            Opening opening = Engine.Structure.Create.Opening(Engine.Geometry.Modify.Close(polyCurves.First()));

            List<Edge> edges = new List<Edge>();
            rfOpening.boundary_lines.ToList().ForEach(l => edges.Add(edgeDict[l]));
            Opening o = new Opening() { Edges = edges };

            opening.SetRFEM6ID(rfOpening.no);
            o.SetRFEM6ID(rfOpening.no);

            //RFEMOpening rfemOpening = new RFEMOpening() { Opening = opening, SurfaceIDs = surfaceIDs };
            RFEMOpening rfemOpening = new RFEMOpening() { Opening = o, SurfaceIDs = surfaceIDs };

            rfemOpening.SetRFEM6ID(rfOpening.no);

            if (rfOpening.comment.Count() != 0)
            {
                BH.Engine.Base.Modify.SetPropertyValue(rfemOpening, "Comment", rfOpening.comment);
            }
            return rfemOpening;

        }

    }
}


