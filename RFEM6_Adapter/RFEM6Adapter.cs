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
using System.Diagnostics;
using System.Reflection;


namespace BH.Adapter.RFEM6
{
#if RFEM6_8_2
    public partial class RFEM6AdapterV8_2 : BHoMAdapter
#elif RFEM6_12_11
    public partial class RFEM6AdapterV12_11 : BHoMAdapter
#else
    public partial class RFEM6Adapter : BHoMAdapter
#endif
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

#if RFEM6_8_2
        static RFEM6AdapterV8_2()
#elif RFEM6_12_11
        static RFEM6AdapterV12_11()
#else
        static RFEM6Adapter()
#endif
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var name = new System.Reflection.AssemblyName(args.Name);
#if RFEM6_8_2
                if (name.Name == "RFEMWebServiceLibrary" && name.Version?.Minor == 8)
                    return System.Reflection.Assembly.LoadFile(@"C:\ProgramData\BHoM\Assemblies\RFEM6_Client\RFEM6_V8_2\RFEMWebServiceLibrary.dll");
#elif RFEM6_12_11
                if (name.Name == "RFEMWebServiceLibrary" && name.Version?.Minor == 12)
                    return System.Reflection.Assembly.LoadFile(@"C:\ProgramData\BHoM\Assemblies\RFEM6_Client\RFEM6_V12_11\RFEMWebServiceLibrary.dll");
#else
                if (name.Name == "RFEMWebServiceLibrary" && name.Version?.Minor == 12)
                    return System.Reflection.Assembly.LoadFile(@"C:\ProgramData\BHoM\Assemblies\RFEM6_Client\RFEM6_V12_11\RFEMWebServiceLibrary.dll");
#endif
                return null;
            };
        }

#if RFEM6_8_2
        [Description("Adapter for RFEM6 using Dlubal Web Service API version 6.8.2. Intended for use with RFEM6 versions prior to 6.12.11. For RFEM6 version 6.12.11 or newer, use RFEM6AdapterV12_11 or RFEM6Adapter instead.")]
        [Input("filePath", "Optional file path to the RFEM6 model. If left empty, the adapter will connect to the currently running RFEM6 instance.")]
        [Input("active", "Set to true to activate the adapter and establish a connection to RFEM6.")]
        [Output("adapter", "The created RFEM6 adapter configured for API version 6.8.2.")]
#elif RFEM6_12_11
        [Description("Adapter for RFEM6 using Dlubal Web Service API version 6.12.11. Intended for use with RFEM6 version 6.12.11 or newer. For RFEM6 versions prior to 6.12.11, use RFEM6AdapterV8_2 instead.")]
        [Input("filePath", "Optional file path to the RFEM6 model. If left empty, the adapter will connect to the currently running RFEM6 instance.")]
        [Input("active", "Set to true to activate the adapter and establish a connection to RFEM6.")]
        [Output("adapter", "The created RFEM6 adapter configured for API version 6.12.11.")]
#else
        [Description("Adapter for RFEM6 using the latest supported Dlubal Web Service API (version 6.12.11 or newer). This is the recommended adapter for use with current RFEM6 installations. For RFEM6 versions prior to 6.12.11, use RFEM6AdapterV8_2 instead.")]
        [Input("filePath", "Optional file path to the RFEM6 model. If left empty, the adapter will connect to the currently running RFEM6 instance.")]
        [Input("active", "Set to true to activate the adapter and establish a connection to RFEM6.")]
        [Output("adapter", "The created RFEM6 adapter configured for the latest supported API version.")]
#endif
#if RFEM6_8_2
        public RFEM6AdapterV8_2(string filePath = "", bool active = false)
#elif RFEM6_12_11
        public RFEM6AdapterV12_11(string filePath = "", bool active = false)
#else
        public RFEM6Adapter(string filePath = "", bool active = false)
#endif
        {

            if (active)
            {
                // The Adapter constructor can be used to configure the Adapter behaviour.
                m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.FullPush; // Adapter `Push` Action simply calls "Create" method.
                                                                                  //m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly;
                m_AdapterSettings.OnlyUpdateChangedObjects = false; // Setting this to true causes a Stackoverflow in some cases from the HashComparer called from the base FullCRUD.
                m_AdapterSettings.CreateOnly_DistinctObjects = false;

                AddAdapterModules();

                AdapterComparers = GenerateAdapterComparersSettings();

                DependencyTypes = GenerateDependencyTypes();

                AdapterIdFragmentType = typeof(RFEM6ID);

                m_filepath = filePath;

                m_isActive = true;
            }
            else
            {
                m_isActive = false;
            }

            //var loadedVersion = typeof(Dlubal.WS.Rfem6.Model.RfemModelClient).Assembly.GetName().Version;
            //BH.Engine.Base.Compute.RecordNote($"RFEM6 Adapter referencing Dlubal Web Service API version: {loadedVersion}");
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private bool m_isActive = false;
        private string m_filepath = "";

        /***************************************************/
        /**** Public  Fields                           ****/
        /***************************************************/

        public Dictionary<Loadcase, Dictionary<String, int>> m_LoadcaseLoadIdDict = new Dictionary<Loadcase, Dictionary<String, int>>(new LoadCaseComparer());
        public Dictionary<Panel, int> m_PanelIDdict = new Dictionary<Panel, int>(new RFEMPanelComparer());
        public Dictionary<Constraint6DOF,int> m_NodalSupportDictionary = new Dictionary<Constraint6DOF, int>(new Constraint6DOFComparer());

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // You can add any private variable that should be in common to any other adapter methods here.
        // If you need to add some private methods, please consider first what their nature is:
        // if a method does not need any external call (API call, connection call, etc.)
        // we place them in the Engine project, and then reference them from the Adapter.
        // See the wiki for more information.


        public void Connect()
        {
            if (m_Application == null) m_Application = new RfemApplicationClient(Binding, Address);
            if (!m_isActive)
            {
                BH.Engine.Base.Compute.RecordWarning("RFEM6 adapter is not active. Please set the 'active' input to true in the constructor.");
                return;
            }

            if (ApplicationIsRunning())
            {
                string modelUrl = "";

                try
                {
                    modelUrl=m_Application.open_model(m_filepath);

                }
                catch
                {
                    modelUrl=GetOpenModel();

                }

                m_Model = new RfemModelClient(Binding, new EndpointAddress(modelUrl));

            }
            else
            {

                BH.Engine.Base.Compute.RecordWarning("RFEM6 application is not running. Please start RFEM6 on your system.");

            }
        }

        public void Disconnect()
        {
            m_Model.close_connection();
            m_Model = null;
        }

        public bool ApplicationIsRunning()
        {
            if (Process.GetProcessesByName("RFEM6").Count() > 0) return true;
            else return false;
        }

        public string GetOpenModel()
        {

            string modelUrl = "";

            try
            {
                modelUrl = m_Application.get_active_model();
                return modelUrl;
            }
            catch
            {
                string dateTimeSignature = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string modelName = $"New_Model_{dateTimeSignature}";
                modelUrl = m_Application.new_model(modelName);
                return modelUrl;
            }
        }

        /***************************************************/
        /**** RFEM6 specific fields                     ****/
        /***************************************************/
        
        private RfemModelClient m_Model;
        public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");

        private static BasicHttpBinding Binding
        {
            get
            {
                BasicHttpBinding binding = new BasicHttpBinding { SendTimeout = new TimeSpan(0, 0, 180), UseDefaultWebProxy = true, MaxReceivedMessageSize = 2147483647 };
                return binding;
            }
        }
        private static RfemApplicationClient m_Application;

        /***************************************************/
    }
}




