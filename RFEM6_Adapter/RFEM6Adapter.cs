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

using Dlubal.Api.Common;
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
using System.IO;
using System.Reflection;


namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter : BHoMAdapter
    {
        private const string ClientAssemblyFolder = @"C:\ProgramData\BHoM\Assemblies\RFEM6_Client\";

        static RFEM6Adapter()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveClientAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveClientAssembly;
        }

        // Assembly.LoadFile gives each DLL its own load context (one AssemblyLoadContext per file on .NET 8),
        // so DlubalApi and Grpc.Core would see different copies of the gRPC types. Reuse what is already
        // loaded and otherwise LoadFrom, so all client assemblies share one context.
        private static Assembly ResolveClientAssembly(object sender, ResolveEventArgs args)
        {
            AssemblyName requested = new AssemblyName(args.Name);

            Assembly loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a =>
                !a.IsDynamic
                && string.Equals(a.GetName().Name, requested.Name, StringComparison.OrdinalIgnoreCase)
                && (requested.Version == null || a.GetName().Version >= requested.Version));

            if (loaded != null)
                return loaded;

            string assemblyPath = Path.Combine(ClientAssemblyFolder, requested.Name + ".dll");
            return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }

        // Dependency order matters: each DLL must be loaded after the ones it references.
        private static readonly string[] ClientAssemblyLoadOrder = { "Google.Protobuf", "Grpc.Core.Api", "Grpc.Core", "Grpc.Net.Common", "Grpc.Net.Client", "DlubalApi" };

        // Rhino's own resolver can answer a Grpc.Core request with any same-named DLL from another plug-in folder
        // (e.g. SOFiSTiK ships Grpc.Core 1.x), before our AssemblyResolve handler runs. Loading the client set
        // explicitly first means DlubalApi binds to these copies instead.
        private static string PreloadClientAssemblies()
        {
            List<string> failures = new List<string>();

            foreach (string name in ClientAssemblyLoadOrder)
            {
                string path = Path.Combine(ClientAssemblyFolder, name + ".dll");
                if (!File.Exists(path))
                    continue;

                Version fileVersion = AssemblyName.GetAssemblyName(path).Version;
                bool alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies().Any(a =>
                    !a.IsDynamic
                    && string.Equals(a.GetName().Name, name, StringComparison.OrdinalIgnoreCase)
                    && a.GetName().Version >= fileVersion);

                if (alreadyLoaded)
                    continue;

                try
                {
                    Assembly.LoadFrom(path);
                }
                catch (Exception ex)
                {
                    failures.Add($"- {name}: {ex.Message}");
                }
            }

            return string.Join("\n", failures);
        }

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter for RFEM6 using the Dlubal API II (gRPC). Requires RFEM 6.15.x.")]
        [Input("filePath", "Optional file path to the RFEM6 model. If left empty, the adapter will connect to the currently running RFEM6 instance.")]
        [Input("active", "Set to true to activate the adapter and establish a connection to RFEM6.")]
        [Input("apiKeyName", "Name of the API key stored in config.ini. Defaults to 'BHoM Test Key'.")]
        [Output("adapter", "The created RFEM6 adapter configured for the Dlubal API II.")]
        public RFEM6Adapter(string filePath = "", bool active = false, string apiKeyName = "BHoM Test Key")
        {
            m_apiKeyName = apiKeyName;

            if (active)
            {
                // The Adapter constructor can be used to configure the Adapter behaviour.
                // CreateOnly avoids the default IUpdate path (Delete then Create), which removes
                // materials before create_object_list and makes CrossSection.Material invalid.
                m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly;
                m_AdapterSettings.OnlyUpdateChangedObjects = false; // Setting this to true causes a Stackoverflow in some cases from the HashComparer called from the base FullCRUD.
                // The base Push creates every type group, dependencies included, at object level 0, so this setting controls all de-duplication.
                // Without it, nodes shared by panel edges are created once per edge and RFEM cannot close the surface boundaries.
                m_AdapterSettings.CreateOnly_DistinctObjects = true;

                AddAdapterModules();

                AdapterComparers = GenerateAdapterComparersSettings();

                DependencyTypes = GenerateDependencyTypes();

                AdapterIdFragmentType = typeof(RFEM6ID);
                m_AdapterSettings.UseAdapterId = true;

                m_filepath = filePath;

                m_isActive = true;
            }
            else
            {
                m_isActive = false;
            }

            //var loadedVersion = typeof(ApplicationRfem).Assembly.GetName().Version;
            //BH.Engine.Base.Compute.RecordNote($"RFEM6 Adapter referencing Dlubal API version: {loadedVersion}");
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private bool m_isActive = false;
        private string m_filepath = "";
        private string m_apiKeyName = "BHoM Test Key";
        // Holds IMessage objects. Typed as object so loading RFEM6Adapter doesn't require Google.Protobuf
        // before ResolveClientAssembly is registered (BHoM reflects over the adapter at startup).
        private readonly List<object> m_createQueue = new List<object>();
        private bool m_isBatchCreate = false;
        // RFEM objects read once per Push/Pull, keyed by RFEM type. Values are IMessage, typed as object for the same reason.
        private readonly Dictionary<Type, List<object>> m_modelSnapshot = new Dictionary<Type, List<object>>();

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
            if (!m_isActive)
            {
                BH.Engine.Base.Compute.RecordWarning("RFEM6 adapter is not active. Please set the 'active' input to true in the constructor.");
                return;
            }

            if (!ApplicationIsRunning())
            {
                BH.Engine.Base.Compute.RecordWarning("RFEM6 application is not running. Please start RFEM6 on your system.");
                return;
            }

            if (m_Application == null)
            {
                string preloadFailures = "";
                try
                {
                    preloadFailures = PreloadClientAssemblies();
                    m_Application = CreateApplication(m_apiKeyName);
                }
                catch (Exception ex) when (ex is MissingMethodException || ex is FileLoadException || ex is FileNotFoundException || ex is TypeLoadException)
                {
                    BH.Engine.Base.Compute.RecordError($"Could not start the Dlubal API client: {ex.Message}\n" +
                        "This usually means another plugin loaded a different copy of the gRPC / Protobuf / Dlubal assemblies into Rhino. Loaded copies:\n" +
                        DescribeLoadedClientAssemblies() +
                        (string.IsNullOrEmpty(preloadFailures) ? "" : "\nCould not preload from RFEM6_Client:\n" + preloadFailures));
                    m_Application = null;
                    return;
                }
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(m_filepath))
                    m_ModelId = m_Application.open_model(path: m_filepath);
                else
                    m_ModelId = GetOpenModel();
            }
            catch
            {
                m_ModelId = GetOpenModel();
            }
        }

        // Separate method so assembly binding failures inside DlubalApi surface here, not when Connect is JIT-compiled.
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static ApplicationRfem CreateApplication(string apiKeyName)
        {
            return new ApplicationRfem(apiKeyName: apiKeyName);
        }

        private static string DescribeLoadedClientAssemblies()
        {
            string[] prefixes = { "Grpc", "Google.Protobuf", "DlubalApi" };

            IEnumerable<string> lines = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && prefixes.Any(p => a.GetName().Name.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(a => a.GetName().Name)
                .Select(a =>
                {
                    string location;
                    try { location = string.IsNullOrEmpty(a.Location) ? "(no file)" : a.Location; }
                    catch { location = "(unknown)"; }
                    return $"- {a.GetName().Name} {a.GetName().Version} from {location}";
                });

            string result = string.Join("\n", lines);
            return string.IsNullOrEmpty(result) ? "- none" : result;
        }

        public void Disconnect()
        {
            if (m_Application != null)
            {
                m_Application.close_connection();
                m_Application = null;
            }

            m_ModelId = null;
        }

        public bool ApplicationIsRunning()
        {
            if (Process.GetProcessesByName("RFEM6").Count() > 0) return true;
            else return false;
        }

        public ModelId GetOpenModel()
        {
            try
            {
                return m_Application.get_active_model();
            }
            catch
            {
                string dateTimeSignature = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string modelName = $"New_Model_{dateTimeSignature}";
                return m_Application.create_model(name: modelName);
            }
        }

        protected static int GetNextFreeNumber(HashSet<int> usedNumbers)
        {
            int number = usedNumbers == null || usedNumbers.Count == 0 ? 1 : usedNumbers.Max() + 1;
            while (usedNumbers != null && usedNumbers.Contains(number))
                number++;

            return number;
        }

        private static int GetObjectNumber(IMessage obj)
        {
            return GetIntProperty(obj, "No");
        }

        private static int GetIntProperty(IMessage obj, string propertyName)
        {
            PropertyInfo property = obj?.GetType().GetProperty(propertyName);
            if (property == null)
                return 0;

            object value = property.GetValue(obj);
            return value is int number ? number : 0;
        }

        /***************************************************/
        /**** RFEM6 specific fields                     ****/
        /***************************************************/
        
        private ApplicationRfem m_Application;
        private ModelId m_ModelId;

        /***************************************************/
    }
}



