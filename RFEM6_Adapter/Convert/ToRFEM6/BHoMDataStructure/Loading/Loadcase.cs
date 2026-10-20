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
using BH.oM.Structure.Loads;

using ActionCategory = Dlubal.Api.Rfem.Loading.LoadCase.Types.ActionCategory;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static RfLoading.LoadCase ToRFEM6(this Loadcase bhLoadcase, int analysisSettingsNo)
        {
            RfLoading.LoadCase rfLoadCase = new RfLoading.LoadCase
            {
                No = bhLoadcase.Number,
                AnalysisType = RfLoading.LoadCase.Types.AnalysisType.StaticAnalysis,
                StaticAnalysisSettings = analysisSettingsNo,
                ActionCategory = bhLoadcase.Nature.ToRFEM6(),
            };

            if (!string.IsNullOrWhiteSpace(bhLoadcase.Name))
                rfLoadCase.Name = bhLoadcase.Name;

            return rfLoadCase;
        }

        public static ActionCategory ToRFEM6(this LoadNature loadNature)
        {
            switch (loadNature)
            {
                case LoadNature.Dead:
                    return ActionCategory.PermanentG;
                case LoadNature.SuperDead:
                    return ActionCategory.PermanentImposedGq;
                case LoadNature.Snow:
                    return ActionCategory.SnowIceLoadsFinlandIcelandQs;
                case LoadNature.Live:
                    return ActionCategory.ImposedLoadsCategoryADomesticResidentialAreasQiA;
                case LoadNature.Temperature:
                    return ActionCategory.TemperatureNonFireQt;
                case LoadNature.Accidental:
                    return ActionCategory.AccidentalActionsA;
                case LoadNature.Prestress:
                    return ActionCategory.PrestressP;
                case LoadNature.Seismic:
                    return ActionCategory.SeismicActionsAe;
                case LoadNature.Wind:
                    return ActionCategory.WindQw;
                default:
                    BH.Engine.Base.Compute.RecordWarning($"Load cases of nature {loadNature} have no RFEM6 equivalent and will be pushed as permanent (dead) load cases.");
                    return ActionCategory.PermanentG;
            }
        }
    }
}
