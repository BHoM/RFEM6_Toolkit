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


    public class LoadCase_Test

    {

        RFEM6Adapter adapter;

        Loadcase loadcase1;
        Loadcase loadcase2;
        Loadcase loadcase3;
        Loadcase loadcase4;
        Loadcase loadcase5;
        Loadcase loadcase6;
        Loadcase loadcase7;
        Loadcase loadcase8;
        Loadcase loadcase9;
        Loadcase loadcase10;
        Loadcase loadcase11;
        Loadcase loadcase12;
        Loadcase loadcase1_1NonDL;

        List<Loadcase> loadCaseList;

        //[TearDown]
        //public void TearDown()
        //{
        //    adapter.Wipeout();
        //}

        [SetUp]
        public void EveryTimeSetUp()
        {
            //adapter = new RFEM6Adapter(true);
            loadcase1 = new Loadcase() { Name = "LC1", Nature = LoadNature.Dead, Number = 1 };
            loadcase2 = new Loadcase() { Name = "LC2", Nature = LoadNature.Accidental, Number = 2 };
            loadcase3 = new Loadcase() { Name = "LC3", Nature = LoadNature.Live, Number = 3 };
            loadcase4 = new Loadcase() { Name = "LC4", Nature = LoadNature.Notional, Number = 4 };//Recognized as Dead
            loadcase5 = new Loadcase() { Name = "LC5", Nature = LoadNature.Other, Number = 5 };//recognized as Dead
            loadcase6 = new Loadcase() { Name = "LC6", Nature = LoadNature.Prestress, Number = 6 };
            loadcase7 = new Loadcase() { Name = "LC7", Nature = LoadNature.Seismic, Number = 7 };
            loadcase8 = new Loadcase() { Name = "LC8", Nature = LoadNature.Snow, Number = 8 };
            loadcase9 = new Loadcase() { Name = "LC9", Nature = LoadNature.SuperDead, Number = 9 };
            loadcase10 = new Loadcase() { Name = "LC10", Nature = LoadNature.Temperature, Number = 10 };
            loadcase11 = new Loadcase() { Name = "LC11", Nature = LoadNature.Wind, Number = 11 };
            loadcase12 = new Loadcase() { Name = "LC12", Nature = LoadNature.Wind };

            loadcase1_1NonDL = new Loadcase() { Name = "NumberNaturMissmatch", Nature = LoadNature.Wind, Number = 1 };


            loadCaseList = new List<Loadcase>() { loadcase1, loadcase2, loadcase3, loadcase4, loadcase5, loadcase6, loadcase7, loadcase8, loadcase9, loadcase10, loadcase11, loadcase12 };
        }

        [OneTimeSetUp]
        public void SetUpScenario()
        {
            adapter = new RFEM6Adapter(true);

        }

        [Test]
        public void PushPullOrder()
        {
            //Reverse List
            loadCaseList.Reverse();
            adapter.Push(loadCaseList);

            //Reverse list bakc to normal
            loadCaseList.Reverse();
            adapter.Pull(new FilterRequest() { Type = typeof(Loadcase) });
            List<Loadcase> pulledLoadCases = adapter.Pull(new FilterRequest() { Type = typeof(Loadcase) }).ToList().Select(p => (Loadcase)p).ToList();

            int i = 0;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 1;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i=2;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 3;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(LoadNature.Dead));

            i = 4;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(LoadNature.Dead));

            i = 5;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 6;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 7;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 8;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 9;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

            i = 10;
            Assert.IsTrue(pulledLoadCases[i].Name.Equals(loadCaseList[i].Name));
            Assert.IsTrue(pulledLoadCases[i].Number.Equals(loadCaseList[i].Number));
            Assert.IsTrue(pulledLoadCases[i].Nature.Equals(loadCaseList[i].Nature));

           

        }

        [Test]
        public void PushPullNo1NonDeadLoad()
        {
            adapter.Push(new List<Loadcase>() { loadcase1_1NonDL });

            adapter.Pull(new FilterRequest() { Type = typeof(Loadcase) });
            List<Loadcase> pulledLoadCases = adapter.Pull(new FilterRequest() { Type = typeof(Loadcase) }).ToList().Select(p => (Loadcase)p).ToList();
            
            Assert.IsTrue(pulledLoadCases[0].Name.Equals(loadcase1_1NonDL.Name));
            Assert.IsTrue(pulledLoadCases[0].Nature.Equals(LoadNature.Dead));
            Assert.IsTrue(pulledLoadCases[0].Number.Equals(loadcase1_1NonDL.Number));





        }




    }
}


