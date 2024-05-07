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

namespace RFEM_Toolkit_Test.Elements
{


    public class MaterialTestClass

    {

        RFEM6Adapter adapter;
        IMaterialFragment glulam;
        IMaterialFragment timberC;
        IMaterialFragment timberT;
        IMaterialFragment timberD;

        IMaterialFragment concrete;
        IMaterialFragment steel;

        NameOrDescriptionComparer comparer; 



        [OneTimeSetUp]
        public void SetUpScenario()
        {

            /***************************************************/
            /**** Arrange                                   ****/
            /***************************************************/
            adapter = new RFEM6Adapter(true);

            comparer = new NameOrDescriptionComparer();

            glulam = BH.Engine.Library.Query.Match("Glulam", "GL 20C", true, true) as IMaterialFragment;
            steel = BH.Engine.Library.Query.Match("Steel", "S450", true, true) as IMaterialFragment;
            concrete = BH.Engine.Library.Query.Match("Concrete", "C30/37", true, true) as Concrete;

            timberC = BH.Engine.Library.Query.Match("SawnTimber", "C14", true, true) as IMaterialFragment;
            timberT = BH.Engine.Library.Query.Match("SawnTimber", "T8", true, true) as IMaterialFragment;
            timberD = BH.Engine.Library.Query.Match("SawnTimber", "D18", true, true) as IMaterialFragment;

        }

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }

        [Test]
        public void PushPullOFSteel()
        {

            /***************************************************/
            /**** Act                                    ****/
            /***************************************************/

            //Push it once
            adapter.Push(new List<IMaterialFragment>() { steel });
            adapter.Push(new List<IMaterialFragment>() { steel.DeepClone() });


            //Pull it   
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(IMaterialFragment) };
            var materialPulled = adapter.Pull(materialFilter).ToList();
            IMaterialFragment mp = (IMaterialFragment)materialPulled[0];


            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            //Null Check
            Assert.IsNotNull(mp);

            //Compares pushed to pulled material
            Assert.IsTrue(comparer.Equals(steel, mp));

            //Checks if only one material is pulled after double push
            Assert.AreEqual(1, materialPulled.Count);

        }


        [Test]
        public void PushPullOFConcrete()
        {
            /***************************************************/
            /**** Act                                       ****/
            /***************************************************/

            //Push it once
            adapter.Push(new List<IMaterialFragment>() { concrete });
            adapter.Push(new List<IMaterialFragment>() { concrete.DeepClone() });


            //Pull it   
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(IMaterialFragment) };
            var materialPulled = adapter.Pull(materialFilter).ToList();
            IMaterialFragment mp = (IMaterialFragment)materialPulled[0];



            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            //Null Check
            Assert.IsNotNull(mp);

            //Compares pushed to pulled material
            Assert.IsTrue(comparer.Equals(concrete, mp));

            //Checks if only one material is pulled after double push
            Assert.AreEqual(1, materialPulled.Count);


        }


        [Test]
        public void PushPullOfGlulam()
        {

            /***************************************************/
            /**** Act                                       ****/
            /***************************************************/


            //adapter.Push(new List<IMaterialFragment>() { steel });
            //adapter.Push(new List<IMaterialFragment>() { concrete });
            adapter.Push(new List<IMaterialFragment>() { glulam });
            adapter.Push(new List<IMaterialFragment>() { glulam.DeepClone() });


            //Pull it   
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(IMaterialFragment) };
            var materialPulled = adapter.Pull(materialFilter).ToList();
            IMaterialFragment mp = (IMaterialFragment)materialPulled[0];



            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            //Null Check
            Assert.IsNotNull(mp);

            //Compares pushed to pulled material
            Assert.IsTrue(comparer.Equals(glulam, mp));

            //Checks if only one material is pulled after double push
            Assert.AreEqual(1, materialPulled.Count);


        }


        [Test]
        public void PushPullOFTimber()
        {
            /***************************************************/
            /**** Act                                       ****/
            /***************************************************/

            //Push it once

            adapter.Push(new List<IMaterialFragment>() { timberC });
            adapter.Push(new List<IMaterialFragment>() { timberC });
            adapter.Push(new List<IMaterialFragment>() { timberT });
            adapter.Push(new List<IMaterialFragment>() { timberT });
            adapter.Push(new List<IMaterialFragment>() { timberD });
            adapter.Push(new List<IMaterialFragment>() { timberD });
            //adapter.Push(new List<IMaterialFragment>() { glulam.DeepClone() });

            //Pull it   
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(IMaterialFragment) };
            List<IMaterialFragment> materialPulled = adapter.Pull(materialFilter).ToList().Select(m => (IMaterialFragment)m).ToList();
            HashSet<IMaterialFragment> materialPulledSet = new HashSet<IMaterialFragment>(comparer);
            materialPulledSet.UnionWith(materialPulled);


            /***************************************************/
            /**** Assertions                                ****/
            /***************************************************/

            //Null Check
            Assert.IsNotNull(materialPulled);

            //Compares pushed to pulled material
            Assert.IsTrue(materialPulledSet.Contains(timberC));
            Assert.IsTrue(materialPulledSet.Contains(timberT));
            Assert.IsTrue(materialPulledSet.Contains(timberD));


            //Checks if only one material is pulled after double push
            Assert.AreEqual(3, materialPulled.Count);
        }



    }
}
