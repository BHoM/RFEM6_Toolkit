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
using BH.Engine.Base;
using BH.oM.Adapter;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
#if RFEM6_8_2
    public partial class RFEM6AdapterV8_2
#elif RFEM6_12_11
    public partial class RFEM6AdapterV12_11
#endif
    {

        private bool CreateCollection(IEnumerable<Constraint6DOF> supports)
        {

            // Adding ID to avoid warning!
            foreach (Constraint6DOF c in supports)
            {
                c.SetRFEM6ID(c.FindFragment<Constraint6DOF>().GetRFEM6ID());

            }

            return true;

        }

    }
}



