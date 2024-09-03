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
using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.oM.Data.Requests;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Structure.Requests;

namespace RFEM_Toolkit_Test.Elements
{


	public class NodalResultTestClass

	{

		RFEM6Adapter adapter;

		Line line0;
		Line line1;
		Line line2;
		Line line3;



		IMaterialFragment glulam;
		IMaterialFragment timberC;
		//IMaterialFragment timberT;
		//IMaterialFragment timberD;

		Concrete concrete;
		//IMaterialFragment steel;

		ISectionProperty steelSection;
		ISectionProperty concreteSection;
		ISectionProperty genericSectionGLTimber;
		ISectionProperty genericSectionSawnTimber;

		Bar barSteelSection;
		Bar barConcreteSection;
		Bar barGLTimber;
		Bar barSawTimber;

		BarEndNodesDistanceComparer comparer = new BarEndNodesDistanceComparer(3);

		[TearDown]
		public void TearDown()
		{
			adapter.Wipeout();
		}

		[OneTimeSetUp]
		public void InitializeRFEM6Adapter()
		{
			adapter = new RFEM6Adapter(true);




		}

		[Test]
		public void ReadResult()
		{

			NodeResultRequest request = new NodeResultRequest();
			request.Cases = new List<Object> { 1 };
			request.ObjectIds = new List<object> { 1, 2, 3, 4, 5 ,6};
			request.ResultType = NodeResultType.NodeReaction;

			//Push it once
			var obj = adapter.Pull(request);

			obj.First();

			////Pull it
			//FilterRequest edgeFilter = new FilterRequest() { Type = typeof(Edge) };
			//var edgePulled = adapter.Pull(edgeFilter).ToList();
			//Edge ep = (Edge)edgePulled[0];

			////Check
			//Assert.IsNotNull(ep);
			//Assert.True(comparer.Equals(edge1, ep));
		}



	}
}
