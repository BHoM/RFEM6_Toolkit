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
using BH.Engine.Structure;
using BH.oM.Adapters.RFEM6;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.RFEM6
{
    public class RFEMLoadComparer : IEqualityComparer<ILoad>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMLoadComparer()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(ILoad load0, ILoad load1)
        {
            //return false;

            if (load0.GetHashCode().Equals(load1.GetHashCode())) return true;

            LoadCaseComparer loadCaseComparer = new LoadCaseComparer();
            if (!loadCaseComparer.Equals(load0.Loadcase, load1.Loadcase)) return false;

            if (load0.Axis != load1.Axis) return false;

            if (load0.Projected != load1.Projected) return false;

            if (!load0.GetType().Equals(load1.GetType())) return false;

            if (load0 is GeometricalLineLoad)
            {

                Line line0 = ((GeometricalLineLoad)load0).Location;
                Line line1 = ((GeometricalLineLoad)load1).Location;

                NodeDistanceComparer comparer = new NodeDistanceComparer(3);

                if (!comparer.Equals(new Node() { Position = line0.Start }, new Node() { Position = line1.Start })) return false;

                if (!comparer.Equals(new Node() { Position = line0.End }, new Node() { Position = line1.End })) return false;



            }
            if (load0 is PointLoad)
            {
                //Line line0 = ((PointLoad)load0).;
                //Line line1 = ((PointLoad)load1).;
            }
            if (load0 is BarUniformlyDistributedLoad)
            {

                //((BarUniformlyDistributedLoad)load0).

            }
            if (load0 is AreaUniformlyDistributedLoad)
            {

                if ((load0 as AreaUniformlyDistributedLoad).Pressure.X != (load1 as AreaUniformlyDistributedLoad).Pressure.X) return false;
                if ((load0 as AreaUniformlyDistributedLoad).Pressure.Y != (load1 as AreaUniformlyDistributedLoad).Pressure.Y) return false;
                if ((load0 as AreaUniformlyDistributedLoad).Pressure.Z != (load1 as AreaUniformlyDistributedLoad).Pressure.Z) return false;




            }



            return true;

        }

        /***************************************************/

        public int GetHashCode(ILoad load)
        {

            //return surfaceSupport.GetHashCode();

            return 0;

        }


        /***************************************************/


    }



}






