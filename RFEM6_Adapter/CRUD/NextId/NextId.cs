/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Numerics;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.Engine.Base;
using BH.oM.Structure.Loads;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        protected override object NextFreeId(Type objectType, bool refresh = false)
        {

            int index = 1;

            if (!refresh && m_FreeIds.TryGetValue(objectType, out index))
            {
                index++;
                m_FreeIds[objectType] = index;
                return index;
            }
            else
            {
                rfModel.object_types? rfType = objectType.ToRFEM6();

                if (!rfType.HasValue)
                {
                    return null;
                }

                int id = 0;

                if (objectType.Name.Equals("PointLoad")|| objectType.Name.Equals("BarUniformlyDistributedLoad"))
                {

                    id = m_Model.get_first_free_number(rfType.Value, 1);

                }
                else
                {
                    id = m_Model.get_first_free_number(rfType.Value, 0);
                }

                //id=rfType.Equals(rfModel.object_types.E_OBJECT_TYPE_NODE)?id-1:id;
                m_FreeIds[objectType] = id;
                return id;
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        Dictionary<Type, int> m_FreeIds = new Dictionary<Type, int>();

        /***************************************************/
    }
}
