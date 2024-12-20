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
using BH.Adapter.RFEM6;
using BH.Engine.Base;
using BH.oM.Data.Requests;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Structure;

namespace RFEM_Toolkit_Test.Elements
{


    public class PushPullMaterials

    {

        RFEM6Adapter adapter;
        IMaterialFragment concrete0;
        IMaterialFragment concrete1;
        NameOrDescriptionComparer comparer;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);
        }

        [TearDown]
        public void TearDown()
        {
            adapter.Wipeout();
        }

        [Test]
        public void SinglePushPullOfMaterial()
        {
            comparer = new NameOrDescriptionComparer();

            //Define Material
            concrete0 = BH.Engine.Library.Query.Match("Concrete", "C30/37", true, true).DeepClone() as IMaterialFragment;

            //Push it once
            adapter.Push(new List<IMaterialFragment>() { concrete0 });

            //Pull it
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(IMaterialFragment) };
            var materialPulled = adapter.Pull(materialFilter).ToList();
            IMaterialFragment mp = (IMaterialFragment)materialPulled[0];

            //Check
            Assert.IsNotNull(mp);
            Assert.IsTrue(comparer.Equals(concrete0, mp));            
        }

        [Test]
        public void DoublePushPullOfMaterial()
        {
            comparer = new NameOrDescriptionComparer();

            //Define Material
            concrete0 = BH.Engine.Library.Query.Match("Concrete", "C30/37", true, true).DeepClone() as IMaterialFragment;
            concrete1 = BH.Engine.Library.Query.Match("Concrete", "C16/20", true, true).DeepClone() as IMaterialFragment;

            //Push them twice
            adapter.Push(new List<IMaterialFragment>() { concrete0 });
            adapter.Push(new List<IMaterialFragment>() { concrete0, concrete1 });

            //Pull it
            FilterRequest materialFilter = new FilterRequest() { Type = typeof(IMaterialFragment) };
            var materialsPulled = adapter.Pull(materialFilter).ToList();
            IMaterialFragment mp1 = (IMaterialFragment)materialsPulled[0];
            IMaterialFragment mp2 = (IMaterialFragment)materialsPulled[1];

            //Check
            Assert.IsNotNull(mp1);
            Assert.IsNotNull(mp2);
            Assert.IsTrue(comparer.Equals(concrete0, mp1));
            Assert.IsTrue(comparer.Equals(concrete1, mp2));
            Assert.IsFalse(comparer.Equals(mp1, mp2));
            Assert.AreEqual(materialsPulled.Count, 2);
        }

    }
}

