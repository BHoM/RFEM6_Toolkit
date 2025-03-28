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
		ISectionProperty steelSection1;
		ISectionProperty steelSection2;
		ISectionProperty steelSection3;
		ISectionProperty steelSection4;
		ISectionProperty concreteSection0;
		ISectionProperty concreteSection1;
		ISectionProperty concreteSection2;
		ISectionProperty concreteSection3;
		ISectionProperty genericSectionGLTimber;
		ISectionProperty genericSectionSawnTimber;
		IProfile rectProfileGLTimber;
		IProfile circleProfileSawnTimber;
		IProfile concreteProfile0;
		IProfile concreteProfile1;

		IMaterialFragment glulam;
		IMaterialFragment timberC;
		Concrete concrete0;
		Concrete concrete1;
		Concrete concrete01;
		Concrete concrete11;

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
			/****          Arrange                          ****/
			/***************************************************/
			concrete0 = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as Concrete;
			concrete01 = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as Concrete;

			concrete1 = BH.Engine.Library.Query.Match("Concrete", "C45/55", true, true).DeepClone() as Concrete;
			concrete11 = BH.Engine.Library.Query.Match("Concrete", "C45/55", true, true).DeepClone() as Concrete;


			concreteProfile0 = BH.Engine.Spatial.Create.CircleProfile(0.2);
			concreteSection0 = BH.Engine.Structure.Create.GenericSectionFromProfile(concreteProfile0, concrete0, "ConcreteSection1");

			concreteProfile1 = BH.Engine.Spatial.Create.CircleProfile(0.5);
			concreteSection1 = BH.Engine.Structure.Create.GenericSectionFromProfile(concreteProfile1, concrete1, "ConcreteSection2");

			adapter.Push(new List<ISectionProperty>() { concreteSection0 });

			FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
			var sectionsPulled = adapter.Pull(sectionFilter).Select(s => (ConcreteSection)s).ToList().First();

			/***************************************************/
			/**** Asserti                                   ****/
			/***************************************************/

			//Assert.IsTrue(comparer.Equals(sectionsPulled,ConcreteSection0));



		}

		[Test]
		public void StandardSteelSections()
		{


			/***************************************************/
			/****          Arrange                          ****/
			/***************************************************/

			steelSection1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true).DeepClone() as ISectionProperty;
			steelSection2 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true).DeepClone() as ISectionProperty;
			
			adapter.Push(new List<ISectionProperty>(){steelSection1});
			FilterRequest sectionFilter = new FilterRequest() { Type = typeof(ISectionProperty) };
			var tst = adapter.Pull(sectionFilter);
			steelSection3 = adapter.Pull(sectionFilter).Select(s => (ISectionProperty)s).ToList().First();

			steelSection4 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 550", true, true).DeepClone() as ISectionProperty;

			/***************************************************/
			/****          Act                              ****/
			/***************************************************/

			bool OrgSec1_Vs_OrgSec1 = comparer.Equals(steelSection1, steelSection1);
			bool OrgSec1_Vs_OrgSec2 = comparer.Equals(steelSection1, steelSection2);
			bool OrgSec1_Vs_PullSec3 = comparer.Equals(steelSection1, steelSection3);
			bool OrgSec1_Vs_OrgSec4 = comparer.Equals(steelSection1, steelSection4);


			/***************************************************/
			/****          Assert                           ****/
			/***************************************************/
			
			//Checking Steel Section Agains it self
			Assert.IsTrue(OrgSec1_Vs_OrgSec1);

			// Checking Steel Section agains its Copy 
			Assert.IsTrue(OrgSec1_Vs_OrgSec2);

			// Checking Steel Section agains its Push/Pulled version
			Assert.IsTrue(OrgSec1_Vs_PullSec3);

			// Checking Steel Sectin agains a differnt Steel Section
			Assert.IsTrue(!OrgSec1_Vs_OrgSec4);


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


