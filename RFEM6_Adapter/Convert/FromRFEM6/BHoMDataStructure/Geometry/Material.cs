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
using System.Linq;

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
            string name = rfMaterial.Name.Split('|')[0].Trim();
            IBHoMObject result = null;

            if (!rfMaterial.UserDefined)
                result = MatchMaterialLibraryObject(rfMaterial.Name, matLibrary, out _)?.DeepClone();

            if (result == null)
                result = rfMaterial.FromRFEMProperties(name);

            if (result == null)
            {
                result = ((rfMaterial.MaterialType == StructureCore.Material.Types.MaterialType.Timber || (rfMaterial.MaterialType == StructureCore.Material.Types.MaterialType.Glass)) ?
                    (new BH.oM.Structure.MaterialFragments.GenericIsotropicMaterial()) :
                        (IBHoMObject)new BH.oM.Structure.MaterialFragments.GenericOrthotropicMaterial());

                result.Name = name;
                BH.Engine.Base.Compute.RecordWarning($"It is likely that the RFEM6 material {result.Name} has not corresponding element in the BHoM data set. It will be set to {result} instead when reading it, as this is the best guess.");
            }

            result.SetRFEM6ID(rfMaterial.No);

            return (IMaterialFragment)result;
        }

        /***************************************************/

        // Builds the material from the values RFEM holds for it, as used for user-defined materials.
        private static IMaterialFragment FromRFEMProperties(this StructureCore.Material rfMaterial, string name)
        {
            if (!rfMaterial.MaterialModel.ToString().StartsWith("Isotropic"))
                return null;

            StructureCore.Material.Types.TemperatureRow values = rfMaterial.Temperature?.Rows.FirstOrDefault();
            if (values == null || values.ElasticityModulusGlobal <= 0)
                return null;

            double youngsModulus = values.ElasticityModulusGlobal;
            double poissonsRatio = values.PoissonRatioGlobal;
            if (poissonsRatio <= 0 && values.ShearModulusGlobal > 0)
                poissonsRatio = youngsModulus / (2 * values.ShearModulusGlobal) - 1;

            double density = values.MassDensity;
            double thermalExpansion = values.ThermalExpansionCoefficientGlobal;

            switch (rfMaterial.MaterialType)
            {
                case StructureCore.Material.Types.MaterialType.Concrete:
                case StructureCore.Material.Types.MaterialType.FiberConcrete:
                    return BH.Engine.Structure.Create.Concrete(name, youngsModulus, poissonsRatio, thermalExpansion, density, 0, 0, BH.Engine.Adapters.RFEM6.Query.PsiStrengthFromName(name));
                case StructureCore.Material.Types.MaterialType.Steel:
                case StructureCore.Material.Types.MaterialType.Metal:
                case StructureCore.Material.Types.MaterialType.ReinforcingSteel:
                case StructureCore.Material.Types.MaterialType.PrestressingSteel:
                    return BH.Engine.Structure.Create.Steel(name, youngsModulus, poissonsRatio, thermalExpansion, density, 0, values.YieldStrength, values.UltimateTensileStrengthX);
                default:
                    return new GenericIsotropicMaterial
                    {
                        Name = name,
                        YoungsModulus = youngsModulus,
                        PoissonsRatio = poissonsRatio,
                        ThermalExpansionCoeff = thermalExpansion,
                        Density = density,
                    };
            }
        }
    }
}
