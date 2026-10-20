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
        public static Loadcase FromRFEM(this RfLoading.LoadCase rfLoadCase)
        {
            Loadcase bhLoadcase = new Loadcase
            {
                Name = string.IsNullOrWhiteSpace(rfLoadCase.Name) ? null : rfLoadCase.Name,
                Nature = rfLoadCase.ActionCategory.FromRFEM(),
                Number = rfLoadCase.No,
            };
            bhLoadcase.SetRFEM6ID(rfLoadCase.No);
            return bhLoadcase;
        }

        public static LoadNature FromRFEM(this ActionCategory actionCategory)
        {
            switch (actionCategory)
            {
                case ActionCategory.PermanentG:
                    return LoadNature.Dead;
                case ActionCategory.PermanentImposedGq:
                    return LoadNature.SuperDead;
                case ActionCategory.PrestressP:
                    return LoadNature.Prestress;
                case ActionCategory.ImposedLoadsCategoryADomesticResidentialAreasQiA:
                    return LoadNature.Live;
                case ActionCategory.SnowIceLoadsFinlandIcelandQs:
                    return LoadNature.Snow;
                case ActionCategory.WindQw:
                    return LoadNature.Wind;
                case ActionCategory.TemperatureNonFireQt:
                    return LoadNature.Temperature;
                case ActionCategory.AccidentalActionsA:
                    return LoadNature.Accidental;
                case ActionCategory.SeismicActionsAe:
                    return LoadNature.Seismic;
                default:
                    return LoadNature.Other;
            }
        }
    }
}
