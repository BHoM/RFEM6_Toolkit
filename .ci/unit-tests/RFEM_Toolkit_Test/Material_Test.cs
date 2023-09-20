/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
//using BH.Adapter.RFEM6;
//using BH.Engine.Base;
//using BH.oM.Adapters.RFEM6;
//using BH.oM.Data.Requests;
//using BH.oM.Geometry;
//using BH.oM.Structure.Constraints;
//using BH.oM.Structure.Elements;
//using BH.oM.Data.Library;
//using Dlubal.WS.Rfem6.Model;
//using BH.oM.Physical.Materials;
//using BH.oM.Structure.MaterialFragments;
//using BH.Engine.Structure;
//using BH.oM.Structure;

//namespace RFEM_Toolkit_Test
//{


//    public class Material_Tests
//    {

//        BH.Adapter.RFEM6.RFEM6Adapter adapter;

//        [SetUp]
//        public void Setup()
//        {
//            //adapter.Wipeout();
//        }

//        [OneTimeSetUp]
//        public void InitializeRFEM6Adapter()
//        {
//            adapter = new BH.Adapter.RFEM6.RFEM6Adapter(true);
//        }

//        [Test]
//        public void PushMaterial()
//        {


//            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as IMaterialFragment;
//            var steel = BH.Engine.Library.Query.Match("Steel", "S235", true, true).DeepClone() as IMaterialFragment;
//            //var steel = BH.Engine.Library.Query.Datasets("EU_SteelSections")[0].Data[0] as IMaterialFragment;

//            adapter.Push(new List<IMaterialFragment>() { concrete,steel });

//            BH.oM.Data.Requests.FilterRequest filter = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(IMaterialFragment) };
//            var pulledMaterials = adapter.Pull(filter).ToList();



//            var mat1=pulledMaterials[0];
//            var mat2 = pulledMaterials[1];


//            NameOrDescriptionComparer comp = new NameOrDescriptionComparer();


//            Assert.True(comp.Equals((IProperty)mat1,(IProperty)concrete));


//        }





//    }
//}