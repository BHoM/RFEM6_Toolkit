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


namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter for RFEM6.")]
        [Output("The created RFEM6 adapter.")]
        public RFEM6Adapter(string filePath = "", bool active = false)
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

        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private string m_filepath = "";

        public Dictionary<Loadcase, Dictionary<String, int>> m_LoadcaseLoadIdDict = new Dictionary<Loadcase, Dictionary<String, int>>(new LoadCaseComparer());
        public Dictionary<Panel, int> m_PanelIDdict = new Dictionary<Panel, int>(new RFEMPanelComparer());

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

        //RFEM stuff ----------------------------
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
        private static RfemApplicationClient m_Application = new RfemApplicationClient(Binding, Address);

        /***************************************************/
    }
}



