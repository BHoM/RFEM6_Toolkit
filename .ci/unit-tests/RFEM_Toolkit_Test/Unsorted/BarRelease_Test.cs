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
        public void PushPanel_Test()
        {

            //Node n0 = new Node() { Position = new Point() { X = 0, Y = 0, Z = 0 } };
            //Node n1 = new Node() { Position = new Point() { X = 10, Y = 0, Z = 0 } };
            //Concrete matConcrete = BH.Engine.Library.Query.Match("MaterialsEurope", "C90/105", true, true).DeepClone() as Concrete;
            //ISteelSection steelSection = BH.Engine.Library.Query.Match("EU_SteelSections", "HE1000M", true, true).DeepClone() as ISteelSection;



            //Constraint6DOF constraint0 = BH.Engine.Structure.Create.FixConstraint6DOF("");

            //Constraint6DOF constraint1 = BH.Engine.Structure.Create.PinConstraint6DOF("");

            //Node n3 = new Node() { Position = new Point() { X = 0, Y = 10, Z = 0 } };
            //Node n4 = new Node() { Position = new Point() { X = 10, Y = 10, Z = 0 } };
            ////ISteelSection steelSection = BH.Engine.Library.Query.Match("EU_SteelSections", "HE1000M", true, true).DeepClone() as ISteelSection;
            ////Constraint6DOF constraint0 = BH.Engine.Structure.Create.FixConstraint6DOF("");
            //Constraint6DOF constraint2 = BH.Engine.Structure.Create.Constraint6DOF(true,true,false,true,true,true);


            //BarRelease release0 = new BarRelease() { StartRelease=constraint0, EndRelease=constraint1};
            //BarRelease release1 = new BarRelease() { StartRelease = constraint0, EndRelease = constraint2 };


            //Bar bar0 = new Bar() {StartNode=n0, EndNode=n1,SectionProperty= steelSection,Release=release0};
            //Bar bar1 = new Bar() { StartNode = n3, EndNode = n4, SectionProperty = steelSection, Release = release1 };


            ////adapter.Push(new List<Bar>() { bar0,bar1 });

            BH.oM.Data.Requests.FilterRequest filterRequest1 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Opening) };
            var openings = adapter.Pull(filterRequest1);
            
            Opening opening =(Opening)openings.First();
            Constraint6DOF support = opening.Edges.First().Support;

            ////var material = materials.First();


            //BH.oM.Data.Requests.FilterRequest filter2 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(ISectionProperty) };
            //var sections = adapter.Pull(filter2);
            //var section=sections.First();

            ////BH.oM.Data.Requests.FilterRequest filterRequest2 = new BH.oM.Data.Requests.FilterRequest() { Type = typeof(Bar) };
            ////var bars = adapter.Pull(filterRequest2);
            ////var bar = hinges.First();

        }





    }
}
