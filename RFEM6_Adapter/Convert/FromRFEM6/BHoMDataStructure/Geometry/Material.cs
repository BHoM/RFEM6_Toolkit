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

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static IMaterialFragment FromRFEM(this rfModel.material rfMaterial, List<IMaterialFragment> matLibrary)
        {
            string matName = Regex.Replace(rfMaterial.name.Split('|')[0].ToString(), @"[^a-zA-Z0-9]", "");

            Dictionary<int, HashSet<IBHoMObject>> scorsDict_test = new Dictionary<int, HashSet<IBHoMObject>>();
            //bhSections.ForEach(z => scorsDict_test[BH.Engine.Search.Compute.MatchScore(sectionName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", "")], )));

            foreach (var z in matLibrary)
            {

                HashSet<IBHoMObject> value;
                int key = BH.Engine.Search.Compute.MatchScore(matName, Regex.Replace(z.Name, @"[^a-zA-Z0-9]", ""));
                if (scorsDict_test.TryGetValue(key, out value))
                {
                    scorsDict_test[key].Add((IBHoMObject)z);

                }
                else
                {

                    scorsDict_test[key] = new HashSet<IBHoMObject>() { z };
                }


            }
            var sortedSectNames_test = scorsDict_test.OrderByDescending(z => z.Key).ToDictionary(z => z.Key, z => z.Value);

            var result = sortedSectNames_test.Values.First().ToList()[0];

            if (sortedSectNames_test.Values.First().Count > 1)
            {
                result = sortedSectNames_test.Values.First().ToList()[0];

                foreach (var i in sortedSectNames_test.Values.First())
                {

                    string mod_name = Regex.Replace(i.Name, @"[^a-zA-Z0-9]", "");
                    string mod_sectionName = Regex.Replace(matName, @"[^a-zA-Z0-9]", "");

                    if (RFEM6Adapter.IsAnagramUsingSort(mod_name, mod_sectionName))
                    {

                        result = i;

                        break;
                    }


                }


            }

            result.SetRFEM6ID(rfMaterial.no);

            return (IMaterialFragment)result.DeepClone();
        }









    }
}
