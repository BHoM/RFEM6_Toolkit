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
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

namespace RFEM_Toolkit_Test.Elements
{


    public class Section_Refactored_Test
    {
        RFEM6Adapter adapter;
        ISectionProperty steelSection1;
        ISectionProperty steelSection2;
        ISectionProperty concreteSection0;
        ISectionProperty concreteSection1;
        ISectionProperty genericSectionGLTimber;
        ISectionProperty genericSectionSawnTimber;
        IProfile rectProfileGLTimber;
        IProfile circleProfileSawnTimber;
        IProfile concreteProfile1;
        IProfile concreteProfile2;

        IMaterialFragment glulam;
        IMaterialFragment timberC;
        Concrete concrete0;
        Concrete concrete1;

        RFEMSectionComparer comparer;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(active:true);
            comparer = new RFEMSectionComparer();
        }

        [TearDown]
        public void TearDown()
        {
            //adapter.Wipeout();
        }

        [Test]
        public void PullOfSection()
        {
            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/

           
            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ISectionProperty)s).ToList();
            //HashSet<ISectionProperty> sectionPulledSet = new HashSet<ISectionProperty>(comparer);
            //sectionPulledSet.UnionWith(sectionsPulled.ToHashSet());

        }

    }
}

