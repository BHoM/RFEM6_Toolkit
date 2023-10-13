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

using BH.Adapter;
using BH.oM.Base.Attributes;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Constraints;
using BH.oM.Geometry;
using BH.Engine.Base.Objects;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Loads;
using System.ServiceModel;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.oM.Adapter;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {

        private Dictionary<Type, List<Type>> GenerateDependencyTypes()
        {

            return new Dictionary<Type, List<Type>>
            {
                {typeof(Point), new List<Type> { typeof(Node) } },
                {typeof(Node), new List<Type> { typeof(RFEMNodalSupport) } },
                {typeof(Edge), new List<Type> { typeof(RFEMLineSupport),typeof(RFEMLine) } },
                {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(RFEMLine),typeof(RFEMHinge)} },
                {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(Panel), new List<Type> { typeof(ISurfaceProperty), typeof(Edge), typeof(Opening), typeof(RFEMOpening) } },
                {typeof(Opening), new List<Type> { typeof(Edge)} },
                {typeof(RigidLink), new List<Type> { typeof(LinkConstraint), typeof(Node) } },
                {typeof(FEMesh), new List<Type> { typeof(ISurfaceProperty), typeof(Node) } },
                {typeof(RFEMLine), new List<Type> {typeof(Node), typeof(RFEMLineSupport) } },
                {typeof(RFEMOpening), new List<Type> { typeof(Edge)} },


            };


        }

        private void AddAdapterModules()
        {


            BH.Adapter.Modules.Structure.ModuleLoader.LoadModules(this);
            this.AdapterModules.Add(new GetRFEMNodalSupportModule());
            this.AdapterModules.Add(new GetRFEMHingeModule());
            this.AdapterModules.Add(new GetRFEMLineSupportModule());
            this.AdapterModules.Add(new GetLineFromBarModule());
            this.AdapterModules.Add(new GetLineFromEdgeModule());
            this.AdapterModules.Add(new GetOpeningFromOpeningModule());

        }

        private Dictionary<Type, object> GenerateAdapterComparersSettings()
        {


            return new Dictionary<Type, object>
            {
                {typeof(Bar), new BarEndNodesDistanceComparer(3) },
                {typeof(Node), new NodeDistanceComparer(3) },
                {typeof(RFEMHinge), new RFEMHingeComparer() },
                {typeof(ISectionProperty), new RFEMSectionComparer() },
                {typeof(ISurfaceProperty), new RFEMSurfacePropertyComparer() },
                {typeof(IMaterialFragment), new NameOrDescriptionComparer() },
                {typeof(LinkConstraint), new NameOrDescriptionComparer() },
                {typeof(Constraint6DOF), new Constraint6DOFComparer()},
                {typeof(RFEMNodalSupport), new RFEMNodalSupportComparer()},
                {typeof(Edge), new EdgeComparer()},
                {typeof(RFEMLineSupport), new RFEMLineSupportComparer() },
                {typeof(RFEMLine), new RFEMLineComparer(3) },
                {typeof(Panel), new RFEMPanelComparer() },
                {typeof(Loadcase), new LoadCaseComparer() }

            };

        }

    }
}

