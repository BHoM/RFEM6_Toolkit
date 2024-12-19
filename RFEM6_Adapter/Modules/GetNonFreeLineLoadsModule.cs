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
using BH.oM.Base;
using BH.oM.Adapter;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using System.Collections;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Loading.Interfaces;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6.Fragments.Enums;

namespace BH.Adapter.RFEM6
{
    [Description("Dependency module for fetching all IRFEMLineLoads stored in a list of Loadcombinations.")]
    public class GetNonFreeLineLoadsModule : IGetDependencyModule<GeometricalLineLoad, IRFEMLineLoad>
    {
        public IEnumerable<IRFEMLineLoad> GetDependencies(IEnumerable<GeometricalLineLoad> lineLoads)
        {
            List<IRFEMLineLoad> rfLineLoad = new List<IRFEMLineLoad>();
           
            foreach (GeometricalLineLoad linload in lineLoads)
            {

                if (linload.Name.ToUpper().Equals("FREE"))
                {

                    RFEMFreeLineLoad rfFreeLineLoad = new RFEMFreeLineLoad() { geometrialLineLoad=linload};

                     rfLineLoad.Add(rfFreeLineLoad);

                }
                else {


                    RFEMNonFreeLineLoad rfFreeLineLoad = new RFEMNonFreeLineLoad() { geometrialLineLoad = linload };

                    rfLineLoad.Add(rfFreeLineLoad);

                }

            }

            return rfLineLoad;
        }
    }
}

