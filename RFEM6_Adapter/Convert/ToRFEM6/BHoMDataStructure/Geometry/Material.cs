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

using System;
using System.Collections.Generic;

using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Base;
using BH.oM.Structure.MaterialFragments;
using EngineQuery = BH.Engine.Adapters.RFEM6.Query;
using StructureCore = Dlubal.Api.Rfem.StructureCore;


namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        private const double StandardGravity = 9.80665;

        public static string ToRFEM6MaterialName(this IMaterialFragment material)
        {
            return GetMaterialName(material);
        }

        public static StructureCore.Material ToRFEM6(this IMaterialFragment material, int materialNo = 0)
        {
            string materialName = GetMaterialName(material);

            StructureCore.Material rfMaterial = new StructureCore.Material
            {
                Name = materialName,
            };

            if (materialNo > 0)
                rfMaterial.No = materialNo;

            if (material is IBHoMObject bhomMaterial)
            {
                string comment = bhomMaterial.ToRFEM6Comment();
                if (!string.IsNullOrWhiteSpace(comment))
                    rfMaterial.Comment = comment;
            }

            return rfMaterial;
        }

        /// <summary>
        /// RFEM material defined by its properties rather than looked up in the RFEM catalogue by name.
        /// </summary>
        public static StructureCore.Material ToRFEM6UserDefined(this IIsotropic material, int materialNo = 0)
        {
            StructureCore.Material rfMaterial = ((IMaterialFragment)material).ToRFEM6(materialNo);

            rfMaterial.UserDefined = true;
            rfMaterial.UserDefinedNameEnabled = true;
            rfMaterial.MaterialType = RfemMaterialType(material);
            rfMaterial.MaterialModel = StructureCore.Material.Types.MaterialModel.IsotropicLinearElastic;
            rfMaterial.DefinitionType = StructureCore.Material.Types.DefinitionType.DerivedG;
            rfMaterial.Temperature = new StructureCore.Material.Types.TemperatureTable();
            rfMaterial.Temperature.Rows.Add(new StructureCore.Material.Types.TemperatureRow
            {
                No = 1,
                ElasticityModulusGlobal = material.YoungsModulus,
                ShearModulusGlobal = material.YoungsModulus / (2 * (1 + material.PoissonsRatio)),
                PoissonRatioGlobal = material.PoissonsRatio,
                MassDensity = material.Density,
                SpecificWeight = material.Density * StandardGravity,
                ThermalExpansionCoefficientGlobal = material.ThermalExpansionCoeff,
            });

            return rfMaterial;
        }

        /// <summary>
        /// True when RFEM can create the material from its catalogue name alone. BHoM dataset materials are in the
        /// RFEM catalogue, except MaterialsUSA lightweight concrete, which RFEM does not list.
        /// </summary>
        public static bool IsRFEM6CatalogueMaterial(this IMaterialFragment material, ISet<string> libraryMaterialNames)
        {
            if (!(material is IIsotropic isotropic) || isotropic.YoungsModulus <= 0)
                return true;

            if (EngineQuery.IsUsaLightweightConcreteName(material.Name))
                return false;

            return libraryMaterialNames != null && material.Name != null && libraryMaterialNames.Contains(material.Name);
        }

        private static StructureCore.Material.Types.MaterialType RfemMaterialType(IIsotropic material)
        {
            switch (material)
            {
                case Concrete _:
                    return StructureCore.Material.Types.MaterialType.Concrete;
                case Steel _:
                    return StructureCore.Material.Types.MaterialType.Steel;
                case Aluminium _:
                    return StructureCore.Material.Types.MaterialType.Aluminum;
                default:
                    return StructureCore.Material.Types.MaterialType.Basic;
            }
        }

        private static String GetMaterialName(IMaterialFragment material)
        {
            switch (material.GetType().Name)
            {
                case "Steel":
                    return material.Name;
                case "Concrete":
                    string rfemUsaConcreteName = EngineQuery.RfemUsaConcreteName(material.Name);
                    return string.IsNullOrEmpty(rfemUsaConcreteName) ? material.Name : rfemUsaConcreteName;
                case "Timber":
                case "Glulam":
                    return material.Name.Replace(" ", "");
                default:
                    return material.Name;
            }
        }   
    
    }
}

