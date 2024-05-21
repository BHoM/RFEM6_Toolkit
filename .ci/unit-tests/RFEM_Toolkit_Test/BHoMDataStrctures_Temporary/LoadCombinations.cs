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
using BH.oM.Base;
using BH.Engine.Structure;
using BH.oM.Structure.Loads;

namespace RFEM_Toolkit_Test.Loading
{


    public class PushPullLoadCombinations

    {

        RFEM6Adapter adapter;
        Loadcase loadcaseDL;
        Loadcase loadcaseSDL;
        Loadcase loadcaseLL;
        Loadcase loadcaseTemp;
        Loadcase loadcaseWind;
        Loadcase loadcaseAccident;
        Loadcase loadcaseCase;

        List<Loadcase> loadCaseList;

        List<double> loadCaseFactors;
        LoadCombination loadCombination;

        [OneTimeSetUp]
        public void InitializeRFEM6Adapter()
        {
            adapter = new RFEM6Adapter(true);

            //LoadCases
            loadcaseDL = new Loadcase() { Name = "DeadLoad", Nature = LoadNature.Dead, Number = 1 };
            loadcaseSDL = new Loadcase() { Name = "SuperDeadLoad", Nature = LoadNature.SuperDead, Number = 2 };
            loadcaseLL = new Loadcase() { Name = "LiveLoad", Nature = LoadNature.Live, Number = 3 };
            loadcaseTemp = new Loadcase() { Name = "TemperatureLoad", Nature = LoadNature.Temperature, Number = 4 };
            loadcaseWind = new Loadcase() { Name = "WindLoad", Nature = LoadNature.Wind, Number = 5 };
            loadcaseAccident = new Loadcase() { Name = "AccidentalLoad", Nature = LoadNature.Accidental, Number = 11 };
            loadcaseCase = new Loadcase() { Name = "SnowLoad", Nature = LoadNature.Snow, Number = 12 };

            loadCaseList = new List<Loadcase>() { loadcaseDL, loadcaseSDL, loadcaseLL, loadcaseTemp, loadcaseWind, loadcaseAccident, loadcaseCase };


            loadCaseFactors = new List<double>() { 1, 2, 3, 4, 5, 6, 7 };

            loadCombination = BH.Engine.Structure.Create.LoadCombination("LoadCombi1", 1, loadCaseList, loadCaseFactors);

        }


        [Test]
        public void PushPullLoadCase()
        {

            adapter.Push(new List<IBHoMObject> { loadCombination });


        }


    }
}