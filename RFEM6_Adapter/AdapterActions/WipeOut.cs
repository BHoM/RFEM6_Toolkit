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

using BH.oM.Adapter;
using BH.oM.Structure.Elements;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {
      

        public void Wipeout()
        {
            try
            {
                this.Connect();

                List<rfModel.object_types> allObjectTypes = new List<rfModel.object_types>() {
                    rfModel.object_types.E_OBJECT_TYPE_MATERIAL,
                    rfModel.object_types.E_OBJECT_TYPE_SECTION,
                    rfModel.object_types.E_OBJECT_TYPE_THICKNESS,
                    rfModel.object_types.E_OBJECT_TYPE_NODE,
                    rfModel.object_types.E_OBJECT_TYPE_LINE,
                    rfModel.object_types.E_OBJECT_TYPE_MEMBER,
                    rfModel.object_types.E_OBJECT_TYPE_SURFACE,
                    rfModel.object_types.E_OBJECT_TYPE_OPENING,
                    rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT,
                    rfModel.object_types.E_OBJECT_TYPE_LINE_SUPPORT

                };


                foreach (var o in allObjectTypes) {
                    

                //Delete all Nodes
                //rfModel.object_types objectType = rfModel.object_types.E_OBJECT_TYPE_NODE;
                rfModel.object_with_children[] nodeNumbers = m_Model.get_all_object_numbers_by_type(o);
                nodeNumbers = nodeNumbers.ToList().Where(n => n.no != 0).ToArray();
                nodeNumbers.ToList().ForEach(n => m_Model.delete_object(o, n.no, 1));

                //m_Model.delete_object(o, 1, 1);
                }


                ////Delete all Nodes
                //rfModel.object_types objectType = rfModel.object_types.E_OBJECT_TYPE_NODE;
                //rfModel.object_with_children[] nodeNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_NODE);
                //nodeNumbers = nodeNumbers.ToList().Where(n => n.no != 0).ToArray();
                //nodeNumbers.ToList().ForEach(n => m_Model.delete_object(objectType, n.no, 1));
                ////IEnumerable<rfModel.node> allRfNodes = nodeNumbers.Length > 1 ? nodeNumbers.ToList().Select(n => m_Model.get_node(n.no)) : new List<rfModel.node>();


            }
            finally
            {
                this.Disconnect();
            }

        }

    }
}
