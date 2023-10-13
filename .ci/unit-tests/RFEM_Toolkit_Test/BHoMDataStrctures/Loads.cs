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
using BH.oM.Base;
using BH.Engine.Structure;
using BH.oM.Structure.Loads;

namespace RFEM_Toolkit_Test.Loading
{


    public class PushPullLoad

    {

        RFEM6Adapter adapter;
        Node n1;
        Node n2;
        IMaterialFragment inputSection2;
        IMaterialFragment inputSection1;
        BarEndNodesDistanceComparer comparer;
        BarRelease release1;
        BarRelease release2;
        BarRelease release3;
        BarRelease release4;
        Bar bar;
      

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);
        }

     

        [Test]
        public void PushPullLoadCase()
        {
            n1 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            n2 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };

            ISectionProperty section1 = BH.Engine.Library.Query.Match("EU_SteelSections", "IPE 300", true, true) as ISectionProperty;
            ISectionProperty section2 = BH.Engine.Library.Query.Match("EU_SteelSections", "HE 1000 M", true, true) as ISectionProperty;

            bar = new Bar() { StartNode = n1, EndNode = n2, SectionProperty = section1 };

            //BH.oM.Base.BHoMGroup<Bar> group = new BH.oM.Base.BHoMGroup<Bar>() { Elements=new List<Bar> {bar } };

            //LoadCases
            Loadcase loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };

            var group0 = new BH.oM.Base.BHoMGroup<Bar>() { Elements = new List<Bar> { bar } };

            var barLoad0 = new BH.oM.Structure.Loads.BarUniformlyDistributedLoad() { Objects = group0, Loadcase = loadcaseDL };
            

            HashSet<ILoad> loadcaseSet = new HashSet<ILoad>() {barLoad0};

            adapter.Push(loadcaseSet.ToList());

            FilterRequest loadFilter = new FilterRequest() { Type = typeof(ILoad) };
            var loads = adapter.Pull(loadFilter).ToList();
            ILoad l0 = (ILoad)loads[0];



            //Assert.AreEqual(loadCases.Count(), loadcaseSet.Count());
            //Assert.True(loadcaseSet.Contains(lc0));

        }

     


    }
}