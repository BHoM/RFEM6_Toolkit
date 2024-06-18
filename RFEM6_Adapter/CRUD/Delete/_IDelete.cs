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

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        // Basic Delete method that deletes objects depending on their Type and Id. 
        // It gets called by the Push or by the Remove Adapter Actions.
        // Its implementation is facultative (not needed for a simple export/import scenario). 
        // Toolkits need to implement (override) this only to get the full CRUD to work.
        protected override int IDelete(Type type, IEnumerable<object> ids, ActionConfig actionConfig = null)
        {
            rfModel.object_types? rfemType = type.ToRFEM6();

            if (!rfemType.HasValue)
            {
                //Avoind warking when type is edge
                if (type.Name == "Edge") { return 0; }

                // Log a warning if the type is not supported
                BH.Engine.Base.Compute.RecordWarning($"Delete not implemented for obejcts of type {type.Name}.");
                return 0;
            }

            int deleteCount = 0;
            foreach (int id in ids.OfType<int>())
            {

                if (rfemType.Value == rfModel.object_types.E_OBJECT_TYPE_LOAD_CASE|| rfemType.Value == rfModel.object_types.E_OBJECT_TYPE_NODAL_LOAD|| rfemType.Value == rfModel.object_types.E_OBJECT_TYPE_MEMBER_LOAD || rfemType.Value == rfModel.object_types.E_OBJECT_TYPE_LINE_LOAD) { continue; }
                m_Model.delete_object(rfemType.Value, id, 0);
                deleteCount++;
            }
            return deleteCount;
        }

        // There are more virtual Delete methods you might want to override and implement.
        // Check the base BHoM_Adapter solution and the wiki for more info.
  
        /***************************************************/
    }
}


