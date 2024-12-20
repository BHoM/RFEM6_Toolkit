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
using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Data.Library;
using BH.Engine.Library;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Results;

namespace RFEM_Toolkit_Test
{


    public class BarRelease_Test
    {

        BH.Adapter.RFEM6.RFEM6Adapter adapter;

        [SetUp]
        public void Setup()
        {
            //adapter.Wipeout();
        }

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
        }

        [Test]
        public void PullResults()
        {

            //BH.Engine.Adapter.Query.

            
            //FilterRequest resultFilter = new FilterRequest() { Type = typeof(RFEM6NodalSupportReaction) };

            //var materialPulled = adapter.Pull(resultFilter).ToList();




        }





    }
}

