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
using BH.Engine.Geometry;
using BH.Engine.Structure;
using BH.oM.Analytical.Elements;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullPanels0
    {
        RFEM6Adapter adapter;
        RFEMPanelComparer comparer;
        Opening opening1;
        Opening opening2;
        Panel panel1;
        Panel panel2;
        Edge edge1;
        Edge edge2;
        Edge edge3;
        Edge edge4;
        Edge edge5;
        Edge edge6;
        Edge edge7;
        Edge edge8;
        Edge edge9;
        Edge edge10;
        Edge edge11;
        Edge edge12;


        [OneTimeSetUp]
        public void InitializeOpenings()
        {
            adapter = new RFEM6Adapter(true);
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [Test]
        public void PullOfMaterial()
        {


            //Pull it
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var materialPulled = adapter.Pull(materialFilter).ToList();
            ISectionProperty mp = (ISectionProperty)materialPulled[0];

        }

    }
}




