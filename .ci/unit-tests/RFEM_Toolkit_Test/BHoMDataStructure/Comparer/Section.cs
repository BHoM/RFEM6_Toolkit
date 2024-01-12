/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.Adapter;
using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.Engine.Spatial;
using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Data.Requests;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RFEM_Toolkit_Test.Comparer_Tests
{
    internal class Section

    {

        RFEM6Adapter adapter;
        ISectionProperty SteelSection1;
        ISectionProperty SteelSection2;
        ISectionProperty ConcreteSection0;
        ISectionProperty ConcreteSection1;
        ISectionProperty GenericSectionGLTimber;
        ISectionProperty GenericSectionSawnTimber;
        IProfile RectProfileGLTimber;
        IProfile CircleProfileSawnTimber;
        IProfile ConcreteProfile1;
        IProfile ConcreteProfile2;

        IMaterialFragment Glulam;
        IMaterialFragment TimberC;
        Concrete Concrete0;
        Concrete Concrete1;

        RFEMSectionComparer comparer;

        //[SetUp]
        //public void Setup()
        //{
            
        //}

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {

            adapter = new RFEM6Adapter(true);
            comparer = new RFEMSectionComparer();

        }

        [Test]
        public void StandardConcreteSections()
        {
            /***************************************************/
            /**** Test Preparation                          ****/
            /***************************************************/
            Concrete0 = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as Concrete;
            Concrete1 = BH.Engine.Library.Query.Match("Concrete", "C45/55", true, true).DeepClone() as Concrete;


            ConcreteProfile1 = BH.Engine.Spatial.Create.CircleProfile(0.2);
            ConcreteSection0 = BH.Engine.Structure.Create.GenericSectionFromProfile(ConcreteProfile1, Concrete0, "ConcreteSection1");

            ConcreteProfile2 = BH.Engine.Spatial.Create.CircleProfile(0.5);
            ConcreteSection1 = BH.Engine.Structure.Create.GenericSectionFromProfile(ConcreteProfile2, Concrete1, "ConcreteSection2");

            adapter.Push(new List<ISectionProperty>() { ConcreteSection0});

            FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
            var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ConcreteSection)s).ToList().First();

            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            //Assert.IsTrue(comparer.Equals(sectionsPulled,ConcreteSection0));



        }

        [Test]
        public void StandardSteelSections()
        {
           //TODO: Implement
        }

        [Test]
        public void GenericTimberSections()
        {
            //TODO: Implement
        }

        [Test]
        public void GenericConcreteSections()
        {
            //TODO: Implement
        }


    }
}

