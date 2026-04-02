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
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Base;
using BH.oM.Structure.SectionProperties;
using System.Text.RegularExpressions;
using BH.oM.Physical.Materials;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static IMaterialFragment FromRFEM(this rfModel.material rfMaterial, List<IMaterialFragment> matLibrary)
        {

            // Picking section of material and make it alpha numberic only
            string matName = Regex.Replace(rfMaterial.name.Split('|')[0].ToString(), @"[^a-zA-Z0-9]", "");

            Dictionary<int, HashSet<IBHoMObject>> matchingScoreDict = new Dictionary<int, HashSet<IBHoMObject>>();

            // Filling the dictionary with matching scores
            foreach (var z in matLibrary)
            {
                HashSet<IBHoMObject> value;
                int key = BH.Engine.Search.Compute.MatchScore(matName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", ""));
                if (matchingScoreDict.TryGetValue(key, out value))
                {
                    matchingScoreDict[key].Add((IBHoMObject)z);
                }
                else
                {
                    matchingScoreDict[key] = new HashSet<IBHoMObject>() { z };
                }

            }

            // Pick all elements with the highes score
            var sortedMatchingScoreDict = matchingScoreDict.OrderByDescending(z => z.Key).ToDictionary(z => z.Key, z => z.Value);
            var result = sortedMatchingScoreDict.Values.First().ToList()[0];



            // If there are more than one element with the highest score, check for anagrams
            if (sortedMatchingScoreDict.Values.First().Count > 1)
            {
                result = sortedMatchingScoreDict.Values.First().ToList()[0];

                foreach (var i in sortedMatchingScoreDict.Values.First())
                {
                    // Remove all special characters from the name
                    string mod_name = Regex.Replace(i.Name, @"[^a-zA-Z0-9]", "");
                    string mod_sectionName = Regex.Replace(matName, @"[^a-zA-Z0-9]", "");

                    if (Convert.IsAnagramUsingSort(mod_name, mod_sectionName))
                    {
                        result = i;
                        break;
                    }
                }
            }


            // If Material match is below 80 the assumption is that the is no matchi in libaray and default material will begreated
            if (sortedMatchingScoreDict.Keys.First() < 80)
            {
                // If the material is timber or glass, it is assumed to be isotropic, otherwise orthotropic
                result = ((rfMaterial.material_type.Equals(material_material_type.TYPE_TIMBER) || (rfMaterial.material_type.Equals(material_material_type.TYPE_GLASS)) ?
                    (new BH.oM.Structure.MaterialFragments.GenericIsotropicMaterial()) :
                        (IBHoMObject)new BH.oM.Structure.MaterialFragments.GenericOrthotropicMaterial()));


                result.Name = rfMaterial.name.Split('|')[0];
                BH.Engine.Base.Compute.RecordWarning($"It is likely that the RFEM6 material {result.Name} has not corresponding element in the BHoM data set. It will be set to {result} instead when reading it, as this is the best guess.");
            }

            result=result.DeepClone();
            result.SetRFEM6ID(rfMaterial.no);
            
            return (IMaterialFragment)result;
        }









    }
}


