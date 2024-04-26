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
using BH.Engine.Analytical;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using Dlubal.WS.Rfem6.Model;

namespace RFEM_Toolkit_Test.Elements
{


    public class SectionFuzzyMatching_Test

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
            adapter = new RFEM6Adapter(true);
            comparer = new RFEMSectionComparer();
        }

        [TearDown]
        public void TearDown()
        {
            //adapter.Wipeout();
        }

        [Test]
        public void PushPullOfSteelSection()
        {
            ///////////////////////////
            var sectionLibrary = BH.Engine.Library.Query.Library("EU_SteelSections");
            var k=sectionLibrary.Select(s=>s.Name).Select(k=>(string)k).ToList();
            
            var weirdStringName = "cHs-42.4X.32";
            Dictionary<string, int> scorsDict= new Dictionary<string, int>();
            k.ForEach(z=>scorsDict.Add(z,BH.Engine.Search.Compute.MatchScore(weirdStringName,z)));
            
            var scorsDictSorted = scorsDict.OrderByDescending(z=>z.Value).ToDictionary(z=>z.Key,z=>z.Value);
            var sortedSectNames=scorsDictSorted.Keys.ToList();

            var foundName= sortedSectNames[0];


            /////////////////////////////
            //var sectionLibrary0 = BH.Engine.Library.Query.Library("UK_SteelSections");
            //var k0 = sectionLibrary.Select(s => s.Name).Select(k => (string)k).ToList();

            //var weirdStringName0 = "cHs-42.4X.32";
            //Dictionary<string, int> scorsDict0 = new Dictionary<string, int>();
            //k.ForEach(z => scorsDict.Add(z, BH.Engine.Search.Compute.MatchScore(weirdStringName, z)));

            //var scorsDictSorted0 = scorsDict.OrderByDescending(z => z.Value).ToDictionary(z => z.Key, z => z.Value);
            //var sortedSectNames0 = scorsDictSorted.Keys.ToList();

            //var foundName0 = sortedSectNames[0];

            //Read Standard BHoM Steel Libraries
            List<IBHoMObject> sectionList = new List<IBHoMObject>();
            var eu_Steel_Section = BH.Engine.Library.Query.Library("EU_SteelSections");
            var uk_Steel_Section = BH.Engine.Library.Query.Library("UK_SteelSections");
            var us_Steel_Section = BH.Engine.Library.Query.Library("US_SteelSections");
            sectionList.AddRange(eu_Steel_Section);
            sectionList.AddRange(uk_Steel_Section);
            sectionList.AddRange(us_Steel_Section);

            var sec = RFEM6Adapter.ReadStandardSteelSections(weirdStringName, sectionList);

        }

        [Test]
        public void PushPullOfConcreteSection()
        {

           


        }

       

    }

}
