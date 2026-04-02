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
using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.oM.Data.Requests;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using Dlubal.WS.Rfem6.Model;
using BH.Engine.Geometry;
using System.Security.Permissions;
using BH.oM.Base;

namespace RFEM_Toolkit_Test.Elements
{


    public class LoadCombination_Test

    {

        RFEM6Adapter adapter;


        [SetUp]
        public void EveryTimeSetUp()
        {
            //adapter = new RFEM6Adapter(active:true);
        }

        [OneTimeSetUp]
        public void SetUpScenario()
        {
            adapter = new RFEM6Adapter(active:true);

        }

  //      [Test]
  //      public void PullLoadCombination()
  //      {

           
  //         var res= adapter.Pull(new FilterRequest() { Type = typeof(LoadCombination) });

  //         var a =  res.ToArray().Count();

		//}   
        
        [Test]
        public void PushLoadCombination()
        {

			LoadCombination lc = new LoadCombination();

			var res= adapter.Push(new List<Object>() { lc });


		}



    }
}


