/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        // This method gets called when appropriate by the Push method contained in the base Adapter class.
        // Unlike the Create, Delete and Read, this method already exposes a simple implementation: it calls Delete and then Create.
        // It can be overridden here keeping in mind the following:
        // - it gets called once per each Type, and if equal objects are found;
        // - the object equality is tested through this.AdapterComparers, that need to be implemented for each type.
        // See the wiki for more info.


        protected override bool IUpdate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            //return ICreate<T>(objects, actionConfig);
            if (!UpdateObjects(objects as dynamic))
                return base.IUpdate(objects, actionConfig);
            return true;
        }

        /***************************************************/

        private bool UpdateObjects(IEnumerable<IBHoMObject> objects)
        {
            return false;
        }

        /***************************************************/

    }


    /***************************************************/


}



