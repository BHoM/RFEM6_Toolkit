/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.Engine.Base.Objects;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Loads;
using System.ServiceModel;
using System.ServiceModel.Primitives;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter for RFEM6.")]
        [Output("The created RFEM6 adapter.")]
        public RFEM6Adapter(bool active = false)
        {
            // The Adapter constructor can be used to configure the Adapter behaviour.
            // For example:
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly; // Adapter `Push` Action simply calls "Create" method.

            // See the wiki, the AdapterSettings object and other Adapters to see how it can be configured.
            //AdapterIdFragmentType = typeof(RFEMId);
            BH.Adapter.Modules.Structure.ModuleLoader.LoadModules(this);

            AdapterComparers = new Dictionary<Type, object>
            {
                {typeof(Bar), new BH.Engine.Structure.BarEndNodesDistanceComparer(3) },
                {typeof(Node), new BH.Engine.Structure.NodeDistanceComparer(3) },
                {typeof(ISectionProperty), new BHoMObjectNameOrToStringComparer() },
                {typeof(IMaterialFragment), new BHoMObjectNameComparer() },
                {typeof(LinkConstraint), new BHoMObjectNameComparer() },
            };

            DependencyTypes = new Dictionary<Type, List<Type>>
            {
                {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } },
                {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(RigidLink), new List<Type> { typeof(LinkConstraint), typeof(Node) } },
                {typeof(FEMesh), new List<Type> { typeof(ISurfaceProperty), typeof(Node) } },
                {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
                {typeof(Panel), new List<Type> { typeof(ISurfaceProperty) } },
                {typeof(ILoad), new List<Type> { typeof(Loadcase) } },
                {typeof(LoadCombination), new List<Type> { typeof(Loadcase) } }
            };



            if (active)
            {
                //System.Security.Permissions.ReflectionPermission permission = new System.Security.Permissions.ReflectionPermission(System.Security.Permissions.ReflectionPermissionFlag.ReflectionEmit);
                // creates new model
                string modelName = "MyTestModel";
                string modelUrl = application.new_model(modelName);//throws error: This operation is not supported on .NET Standard as Reflection.Emit is not available.'


                //string modelUrl = application.get_active_model();

                // connects to RFEM6/RSTAB9 model
                model = new RfemModelClient(Binding, new EndpointAddress(modelUrl));
                model.reset();


            }
        }

        // You can add any other constructors that take more inputs here. 

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        // You can add any private variable that should be in common to any other adapter methods here.
        // If you need to add some private methods, please consider first what their nature is:
        // if a method does not need any external call (API call, connection call, etc.)
        // we place them in the Engine project, and then reference them from the Adapter.
        // See the wiki for more information.


        //RFEM stuff ----------------------------
        RfemModelClient model;
        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");

        private static BasicHttpBinding Binding
        {
            get
            {
                BasicHttpBinding binding = new BasicHttpBinding { SendTimeout = new TimeSpan(0, 0, 180), UseDefaultWebProxy = true, };
                return binding;
            }
        }
        private static RfemApplicationClient application = new RfemApplicationClient(Binding, Address);

        /***************************************************/
    }
}
