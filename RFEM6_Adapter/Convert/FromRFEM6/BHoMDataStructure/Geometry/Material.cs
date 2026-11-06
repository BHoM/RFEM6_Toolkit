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

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;

using BH.Engine.Base;
using BH.oM.Base;
using StructureCore = Dlubal.Api.Rfem.StructureCore;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static IMaterialFragment FromRFEM(this StructureCore.Material rfMaterial, List<IMaterialFragment> matLibrary)
        {
            IMaterialFragment libraryMatch = MatchMaterialLibraryObject(rfMaterial.Name, matLibrary, out _);
            IBHoMObject result;

            if (libraryMatch == null)
            {
                result = ((rfMaterial.MaterialType == StructureCore.Material.Types.MaterialType.Timber || (rfMaterial.MaterialType == StructureCore.Material.Types.MaterialType.Glass)) ?
                    (new BH.oM.Structure.MaterialFragments.GenericIsotropicMaterial()) :
                        (IBHoMObject)new BH.oM.Structure.MaterialFragments.GenericOrthotropicMaterial());

                result.Name = rfMaterial.Name.Split('|')[0];
                BH.Engine.Base.Compute.RecordWarning($"It is likely that the RFEM6 material {result.Name} has not corresponding element in the BHoM data set. It will be set to {result} instead when reading it, as this is the best guess.");
            }
            else
            {
                result = libraryMatch;
            }

            result = result.DeepClone();
            result.SetRFEM6ID(rfMaterial.No);

            return (IMaterialFragment)result;
        }
    }
}


