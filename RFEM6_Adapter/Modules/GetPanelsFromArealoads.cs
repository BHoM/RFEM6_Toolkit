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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    [Description("Dependency module returning the panels of area loads that do not yet have an RFEM6 number, so they are pushed before the loads.\n" +
                 "The structural adapter module only handles IAreaElement, which does not match the Panel dependency type of this adapter.")]
    public class GetPanelFreomAreaUniformlyDistributedLoadModule : IGetDependencyModule<AreaUniformlyDistributedLoad, Panel>
    {
        public IEnumerable<Panel> GetDependencies(IEnumerable<AreaUniformlyDistributedLoad> objects)
        {
            return objects
                .Where(l => l?.Objects?.Elements != null)
                .SelectMany(l => l.Objects.Elements.OfType<Panel>())
                .Where(p => p.GetRFEM6ID() <= 0)
                .Distinct()
                .ToList();
        }
    }
}
