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

using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.Engine.Base;
using BH.oM.Adapters.RFEM6;
using BH.oM.Base;
using StructureCore = Dlubal.Api.Rfem.StructureCore;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static ISectionProperty FromRFEM_Standardized_Steel(this StructureCore.CrossSection rfSection, List<IBHoMObject> bhSections, IMaterialFragment sectionMaterials)
        {
            ISectionProperty parametricSection = TryCreateSectionFromParametricRfemName(rfSection.Name, sectionMaterials);
            if (parametricSection != null)
            {
                parametricSection.SetRFEM6ID(rfSection.No);
                return parametricSection;
            }

            IBHoMObject libraryMatch = MatchLibraryObject(
                rfSection.Name,
                bhSections,
                BH.Engine.Adapters.RFEM6.Query.RfemSectionLibraryLookupKey,
                MinimumSectionMatchScore,
                out _);

            ISectionProperty result;
            if (libraryMatch == null)
            {
                result = new ExplicitSection() { Name = rfSection.Name.Split('|')[0], Material = sectionMaterials };
                BH.Engine.Base.Compute.RecordWarning($"It is likely that the RFEM6 section {result.Name} has no corresponding element in the BHoM dataset. It will be read as ExplicitSection instead.");
            }
            else
            {
                result = (ISectionProperty)libraryMatch;
            }

            ISectionProperty cloned = ((IBHoMObject)result).DeepClone() as ISectionProperty;
            cloned.Material = sectionMaterials;
            cloned.SetRFEM6ID(rfSection.No);
            return cloned;
        }
    }
}


