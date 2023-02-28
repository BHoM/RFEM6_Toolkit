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
using BH.oM.Structure.Elements;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Structure;

namespace BH.Adapter.RFEM6
{
    public class RFEMLineComparer : IEqualityComparer<RFEMLine>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public RFEMLineComparer()
        {
            m_nodeComparer = new NodeDistanceComparer();
        }

        /***************************************************/

        public RFEMLineComparer(int decimals)
        {
            m_nodeComparer = new NodeDistanceComparer(decimals);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(RFEMLine line1, RFEMLine line2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(line1, line2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(line1, null) || Object.ReferenceEquals(line2, null))
                return false;

            if (m_nodeComparer.Equals(line1.StartNode, line2.StartNode))
            {
                return m_nodeComparer.Equals(line1.EndNode, line2.EndNode);
            }
            else if (m_nodeComparer.Equals(line1.StartNode, line2.EndNode))
            {
                return m_nodeComparer.Equals(line1.EndNode, line2.StartNode);
            }

            return false;
        }

        /***************************************************/

        public int GetHashCode(RFEMLine line)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(line, null)) return 0;

            return m_nodeComparer.GetHashCode(line.StartNode) ^ m_nodeComparer.GetHashCode(line.EndNode);
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private NodeDistanceComparer m_nodeComparer;


        /***************************************************/
    }
}




