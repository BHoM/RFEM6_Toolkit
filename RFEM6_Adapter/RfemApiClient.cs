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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapters.RFEM6;
using BH.oM.Adapters.RFEM6.IntermediateDatastructure.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using Dlubal.Api.Common;
using Dlubal.Api.Rfem;
using Google.Protobuf;
using Grpc.Core;

namespace BH.Adapter.RFEM6
{
    /// <summary>
    /// Batch-oriented helpers for the Dlubal API II gRPC client.
    /// Each public method maps to a single API call to minimise token usage.
    /// </summary>
    public partial class RFEM6Adapter
    {
        public const string DefaultApiKeyName = "BHoM Test Key";

        /// <summary>
        /// When no file path is supplied, let RFEM resolve the active model itself.
        /// Passing an explicit model id here has caused internal gRPC errors in some setups.
        /// </summary>
        protected ModelId GetModelIdForApiCall()
        {
            return string.IsNullOrWhiteSpace(m_filepath) ? null : m_ModelId;
        }

        /// <summary>
        /// Reads every RFEM object needed for the given BHoM types with a single get_object_list call.
        /// GetObjectList serves these types from the snapshot until ClearModelSnapshot is called.
        /// </summary>
        protected void LoadModelSnapshot(IEnumerable<Type> bhTypes, bool forPull)
        {
            m_modelSnapshot.Clear();

            if (m_Application == null || bhTypes == null)
                return;

            Dictionary<Type, IMessage> templateMap = new Dictionary<Type, IMessage>();
            foreach (Type bhType in bhTypes.Where(t => t != null).Distinct())
                AddRfemTemplates(bhType, forPull, templateMap);

            List<IMessage> templates = templateMap.Values.ToList();
            if (templates.Count == 0)
                return;

            ModelId modelId = GetModelIdForApiCall();

            try
            {
                List<IMessage> results = modelId == null
                    ? m_Application.get_object_list(templates)
                    : m_Application.get_object_list(templates, modelId: modelId);

                foreach (IMessage template in templates)
                    m_modelSnapshot[template.GetType()] = new List<object>();

                foreach (IMessage result in results)
                {
                    if (result != null && m_modelSnapshot.TryGetValue(result.GetType(), out List<object> list))
                        list.Add(result);
                }
            }
            catch (RpcException ex)
            {
                m_modelSnapshot.Clear();
                BH.Engine.Base.Compute.RecordWarning($"RFEM6 get_object_list for the model snapshot failed. {FormatRpcException(ex)} Falling back to reading each type separately.");
            }
        }

        protected void ClearModelSnapshot()
        {
            m_modelSnapshot.Clear();
        }

        /// <summary>
        /// Adds the RFEM object types that pushing or pulling the given BHoM type reads, including the
        /// types of its dependencies, so the whole action is served by one get_object_list call.
        /// </summary>
        private static void AddRfemTemplates(Type bhType, bool forPull, Dictionary<Type, IMessage> templates)
        {
            void Add(params IMessage[] messages)
            {
                foreach (IMessage message in messages)
                {
                    if (!templates.ContainsKey(message.GetType()))
                        templates[message.GetType()] = message;
                }
            }

            if (typeof(Bar).IsAssignableFrom(bhType))
            {
                Add(new StructureCore.Member(), new StructureCore.Node(), new TypesForNodes.NodalSupport(), new TypesForMembers.MemberHinge(), new StructureCore.CrossSection(), new StructureCore.Material());
                // Members reference their nodes directly, so pulling bars does not need lines.
                if (!forPull)
                    Add(new StructureCore.Line());
            }
            else if (typeof(BH.oM.Structure.Elements.Node).IsAssignableFrom(bhType))
                Add(new StructureCore.Node(), new TypesForNodes.NodalSupport());
            else if (typeof(ISectionProperty).IsAssignableFrom(bhType))
                Add(new StructureCore.CrossSection(), new StructureCore.Material());
            else if (typeof(IMaterialFragment).IsAssignableFrom(bhType))
                Add(new StructureCore.Material());
            else if (typeof(ISurfaceProperty).IsAssignableFrom(bhType))
                Add(new StructureCore.Thickness(), new StructureCore.Material());
            else if (typeof(Panel).IsAssignableFrom(bhType))
                Add(new StructureCore.Surface(), new StructureCore.Opening(), new StructureCore.Line(), new StructureCore.Node(), new TypesForLines.LineSupport(), new StructureCore.Thickness(), new StructureCore.Material());
            else if (typeof(Opening).IsAssignableFrom(bhType) || typeof(RFEMOpening).IsAssignableFrom(bhType))
                Add(new StructureCore.Opening(), new StructureCore.Line(), new StructureCore.Node(), new TypesForLines.LineSupport());
            else if (typeof(Edge).IsAssignableFrom(bhType) || typeof(RFEMLine).IsAssignableFrom(bhType) || typeof(BH.oM.Geometry.Line).IsAssignableFrom(bhType))
                Add(new StructureCore.Line(), new StructureCore.Node(), new TypesForLines.LineSupport());
            else if (typeof(Constraint6DOF).IsAssignableFrom(bhType) || typeof(RFEMNodalSupport).IsAssignableFrom(bhType))
                Add(new TypesForNodes.NodalSupport());
            else if (typeof(RFEMLineSupport).IsAssignableFrom(bhType))
                Add(new TypesForLines.LineSupport());
            else if (typeof(Edge).IsAssignableFrom(bhType))
                Add(new StructureCore.Line(), new StructureCore.Node(), new TypesForLines.LineSupport(), new TypesForSpecialObjects.LineReleaseType(), new StructureAdvanced.LineRelease());
            else if (typeof(RFEMHinge).IsAssignableFrom(bhType))
                Add(new TypesForMembers.MemberHinge());
            else if (typeof(Loadcase).IsAssignableFrom(bhType))
                Add(new RfLoading.LoadCase(), new RfLoading.StaticAnalysisSettings());
            else if (typeof(LoadCombination).IsAssignableFrom(bhType))
                Add(new RfLoading.LoadCombination(), new RfLoading.LoadCase(), new RfLoading.StaticAnalysisSettings());
            else if (typeof(PointLoad).IsAssignableFrom(bhType))
            {
                Add(new RfLoads.NodalLoad());
                AddRfemTemplates(typeof(Loadcase), forPull, templates);
                AddRfemTemplates(typeof(BH.oM.Structure.Elements.Node), forPull, templates);
            }
            else if (typeof(BarUniformlyDistributedLoad).IsAssignableFrom(bhType))
            {
                Add(new RfLoads.MemberLoad());
                AddRfemTemplates(typeof(Loadcase), forPull, templates);
                AddRfemTemplates(typeof(Bar), forPull, templates);
            }
            else if (typeof(AreaUniformlyDistributedLoad).IsAssignableFrom(bhType))
            {
                Add(new RfLoads.SurfaceLoad(), new RfLoads.FreePolygonLoad());
                AddRfemTemplates(typeof(Loadcase), forPull, templates);
                AddRfemTemplates(typeof(Panel), forPull, templates);
            }
            else if (typeof(GeometricalLineLoad).IsAssignableFrom(bhType))
            {
                Add(new RfLoads.FreeLineLoad(), new RfLoads.LineLoad());
                AddRfemTemplates(typeof(Loadcase), forPull, templates);
                AddRfemTemplates(typeof(Panel), forPull, templates);
            }
            else if (typeof(ILoad).IsAssignableFrom(bhType))
            {
                AddRfemTemplates(typeof(PointLoad), forPull, templates);
                AddRfemTemplates(typeof(BarUniformlyDistributedLoad), forPull, templates);
                AddRfemTemplates(typeof(AreaUniformlyDistributedLoad), forPull, templates);
                AddRfemTemplates(typeof(GeometricalLineLoad), forPull, templates);
            }
        }

        /// <summary>
        /// Numbers of RFEM objects of type T that already exist in the model or are queued in this push.
        /// </summary>
        protected HashSet<int> UsedNumbers<T>() where T : class, new()
        {
            HashSet<int> used = new HashSet<int>(GetObjectList(new T()).Select(o => GetObjectNumber((IMessage)o)).Where(n => n > 0));
            foreach (object queued in m_createQueue)
            {
                if (queued is T)
                    used.Add(GetObjectNumber((IMessage)queued));
            }

            return used;
        }

        /// <summary>
        /// Load numbers are only unique within a load case.
        /// </summary>
        protected HashSet<int> UsedLoadNumbers<T>(int loadCase) where T : class, new()
        {
            IEnumerable<IMessage> loads = GetObjectList(new T()).Cast<IMessage>().Concat(m_createQueue.OfType<T>().Cast<IMessage>());
            return new HashSet<int>(loads.Where(l => GetIntProperty(l, "LoadCase") == loadCase).Select(GetObjectNumber).Where(n => n > 0));
        }

        /// <summary>
        /// Returns the preferred number when it is free, otherwise the next free number, and marks it as used.
        /// </summary>
        protected static int ClaimNumber(int preferred, HashSet<int> used)
        {
            int number = preferred <= 0 || used.Contains(preferred) ? GetNextFreeNumber(used) : preferred;
            used.Add(number);
            return number;
        }

        // T must be an IMessage. It's not a constraint because BHoM loads RFEM6Adapter before Google.Protobuf can be resolved.
        protected List<T> GetObjectList<T>(T template) where T : class, new()
        {
            if (m_modelSnapshot.TryGetValue(typeof(T), out List<object> cached))
                return cached.OfType<T>().ToList();

            if (m_Application == null)
                return new List<T>();

            List<IMessage> query = new List<IMessage> { (IMessage)(template ?? new T()) };
            ModelId modelId = GetModelIdForApiCall();

            try
            {
                List<IMessage> results = modelId == null
                    ? m_Application.get_object_list(query)
                    : m_Application.get_object_list(query, modelId: modelId);

                return results.OfType<T>().ToList();
            }
            catch (RpcException ex)
            {
                BH.Engine.Base.Compute.RecordWarning($"RFEM6 get_object_list for {typeof(T).Name} failed. {FormatRpcException(ex)} Free numbers will start from 1.");
                return new List<T>();
            }
        }

        /// <summary>
        /// Queues a converted RFEM object. The list is sent once via create_object_list.
        /// </summary>
        protected void CreateObject(IMessage obj)
        {
            EnqueueCreate(obj);
        }

        /// <summary>
        /// Queues converted RFEM objects. The mixed list is sent once via create_object_list.
        /// </summary>
        protected void CreateObjectList(IEnumerable<IMessage> objects)
        {
            EnqueueCreate(objects);
        }

        protected void EnqueueCreate(IMessage obj)
        {
            if (obj != null)
                m_createQueue.Add(obj);
        }

        protected void EnqueueCreate(IEnumerable<IMessage> objects)
        {
            if (objects == null)
                return;

            foreach (IMessage obj in objects)
            {
                if (obj != null)
                    m_createQueue.Add(obj);
            }
        }

        /// <summary>
        /// Sends the queued objects the same way as the Dlubal C# cantilever example:
        /// one List&lt;IMessage&gt; into create_object_list. If that call is rejected
        /// (common with Dlubal.Api.NetFramework 2.14.x), create each object with
        /// create_object in the same order.
        /// </summary>
        protected void FlushCreateQueue()
        {
            List<IMessage> objectList = new List<IMessage>();
            foreach (IMessage obj in m_createQueue.OfType<IMessage>().OrderBy(GetCreateOrder).ThenBy(GetObjectNumber))
                objectList.Add(obj);

            m_createQueue.Clear();

            if (objectList.Count == 0)
                return;

            if (m_Application == null || m_ModelId == null)
            {
                BH.Engine.Base.Compute.RecordError("RFEM6 model connection failed. Ensure RFEM6 is running, an API key is configured in config.ini, and the adapter 'active' input is true.");
                return;
            }

            string payloadIssues = ValidateCreatePayload(objectList);
            if (!string.IsNullOrWhiteSpace(payloadIssues))
            {
                BH.Engine.Base.Compute.RecordError($"RFEM6 create payload is invalid and was not sent. {payloadIssues} Objects: {DescribeObjectList(objectList)}.");
                return;
            }

            BH.Engine.Base.Compute.RecordNote($"Sent to RFEM6: {string.Join(", ", objectList.GroupBy(o => o.GetType().Name).Select(g => $"{g.Count()} {g.Key}"))}.");

            try
            {
                SendCreateObjectList(objectList);
            }
            catch (RpcException mixedEx)
            {
                BH.Engine.Base.Compute.RecordWarning($"create_object_list failed for {objectList.Count} object(s): {DescribeObjectList(objectList)}. {FormatRpcException(mixedEx)} The C# client often rejects this call even for a valid mixed list; creating objects one by one with create_object.");

                // RFEM keeps the objects it created before the failing one, so those are skipped rather than sent again.
                HashSet<(Type, int, int)> created = ExistingObjectKeys(objectList);
                List<IMessage> remaining = objectList.Where(o => !created.Contains(ObjectKey(o))).ToList();

                foreach (IMessage obj in remaining)
                {
                    try
                    {
                        SendCreateObject(obj);
                    }
                    catch (RpcException objectEx)
                    {
                        BH.Engine.Base.Compute.RecordError($"RFEM6 rejected {DescribeTypeName(obj)} {DescribeObject(obj)}. {FormatRpcException(objectEx)}");
                    }
                }
            }
        }

        private static (Type, int, int) ObjectKey(IMessage obj)
        {
            return (obj.GetType(), GetIntProperty(obj, "LoadCase"), GetObjectNumber(obj));
        }

        /// <summary>
        /// Keys of objects of the given types that are in the model right now, read without the push snapshot.
        /// </summary>
        private HashSet<(Type, int, int)> ExistingObjectKeys(IEnumerable<IMessage> objects)
        {
            List<IMessage> templates = objects.GroupBy(o => o.GetType()).Select(g => (IMessage)Activator.CreateInstance(g.Key)).ToList();
            ModelId modelId = GetModelIdForApiCall();

            try
            {
                List<IMessage> results = modelId == null
                    ? m_Application.get_object_list(templates)
                    : m_Application.get_object_list(templates, modelId: modelId);

                return new HashSet<(Type, int, int)>(results.Where(r => r != null && GetObjectNumber(r) > 0).Select(ObjectKey));
            }
            catch (RpcException ex)
            {
                BH.Engine.Base.Compute.RecordWarning($"Could not read back the model after create_object_list failed, so every object will be sent again. {FormatRpcException(ex)}");
                return new HashSet<(Type, int, int)>();
            }
        }

        private void SendCreateObjectList(List<IMessage> objectList)
        {
            ModelId modelId = GetModelIdForApiCall();
            if (modelId == null)
                m_Application.create_object_list(objs: objectList);
            else
                m_Application.create_object_list(objs: objectList, modelId: modelId);
        }

        private void SendCreateObject(IMessage obj)
        {
            ModelId modelId = GetModelIdForApiCall();
            if (modelId == null)
                m_Application.create_object(obj);
            else
                m_Application.create_object(obj, modelId: modelId);
        }

        private static string DescribeTypeName(IMessage obj)
        {
            return obj.GetType().Name;
        }

        protected void UpdateObjectList(IEnumerable<IMessage> objects)
        {
            List<IMessage> objectList = objects?.OfType<IMessage>().ToList();
            if (objectList == null || objectList.Count == 0 || m_Application == null)
                return;

            if (objectList.Count == 1)
                m_Application.update_object(objectList[0], modelId: GetModelIdForApiCall());
            else
                m_Application.update_object_list(objectList, modelId: GetModelIdForApiCall());
        }

        // Every object must come after the objects it references.
        private static int GetCreateOrder(IMessage obj)
        {
            switch (obj)
            {
                case StructureCore.Material _: return 0;
                case StructureCore.CrossSection _: return 1;
                case StructureCore.Thickness _: return 2;
                case TypesForMembers.MemberHinge _: return 3;
                case TypesForSpecialObjects.LineReleaseType _: return 3;
                case StructureCore.Node _: return 4;
                case StructureCore.Line _: return 5;
                case StructureAdvanced.LineRelease _: return 6;
                case StructureCore.Member _: return 7;
                case StructureCore.Surface _: return 8;
                case StructureCore.Opening _: return 9;
                case TypesForNodes.NodalSupport _: return 10;
                case TypesForLines.LineSupport _: return 11;
                case RfLoading.StaticAnalysisSettings _: return 12;
                case RfLoading.LoadCase _: return 13;
                case RfLoading.LoadCombination _: return 14;
                default: return 99;
            }
        }

        private static string ValidateCreatePayload(IEnumerable<IMessage> objects)
        {
            List<string> issues = new List<string>();

            foreach (IMessage obj in objects)
            {
                switch (obj)
                {
                    case StructureCore.Material material when string.IsNullOrWhiteSpace(material.Name):
                        issues.Add("Material has an empty name.");
                        break;
                    case StructureCore.CrossSection section when string.IsNullOrWhiteSpace(section.Name):
                        issues.Add("Cross-section has an empty name.");
                        break;
                    case StructureCore.CrossSection section when section.Material <= 0:
                        issues.Add($"Cross-section '{section.Name}' references material 0. The material must exist or be in the same create list.");
                        break;
                    case StructureCore.Node node when node.No <= 0:
                        issues.Add("Node has No=0.");
                        break;
                    case StructureCore.Line line when line.No <= 0:
                        issues.Add("Line has No=0.");
                        break;
                    case StructureCore.Line line when line.Type == StructureCore.Line.Types.Type.Circle && line.CircleRadius <= 0:
                        issues.Add($"Line No={line.No} is a circle with radius {line.CircleRadius}.");
                        break;
                    case StructureCore.Line line when line.Type != StructureCore.Line.Types.Type.Circle && (line.DefinitionNodes.Count < 2 || line.DefinitionNodes.Any(n => n <= 0)):
                        issues.Add($"Line No={line.No} has invalid definition nodes [{string.Join(",", line.DefinitionNodes)}].");
                        break;
                    case StructureCore.Surface surface when surface.No <= 0 || surface.Thickness <= 0 || surface.BoundaryLines.Count == 0 || surface.BoundaryLines.Any(l => l <= 0):
                        issues.Add($"Surface No={surface.No} has Thickness={surface.Thickness} and boundary lines [{string.Join(",", surface.BoundaryLines)}]; all must be > 0.");
                        break;
                    case StructureCore.Opening opening when opening.No <= 0 || opening.BoundaryLines.Count == 0 || opening.BoundaryLines.Any(l => l <= 0):
                        issues.Add($"Opening No={opening.No} has boundary lines [{string.Join(",", opening.BoundaryLines)}]; all must be > 0.");
                        break;
                    case StructureCore.Thickness thickness when thickness.Material <= 0:
                        issues.Add($"Thickness '{thickness.Name}' references material 0.");
                        break;
                    case RfLoading.LoadCase loadCase when loadCase.No <= 0 || loadCase.StaticAnalysisSettings <= 0:
                        issues.Add($"Load case No={loadCase.No} has StaticAnalysisSettings={loadCase.StaticAnalysisSettings}; both must be > 0.");
                        break;
                    case RfLoads.NodalLoad load when load.LoadCase <= 0 || load.Nodes.Count == 0 || load.Nodes.Any(n => n <= 0):
                        issues.Add($"Nodal load No={load.No} in load case {load.LoadCase} has nodes [{string.Join(",", load.Nodes)}].");
                        break;
                    case RfLoads.MemberLoad load when load.LoadCase <= 0 || load.Members.Count == 0 || load.Members.Any(n => n <= 0):
                        issues.Add($"Member load No={load.No} in load case {load.LoadCase} has members [{string.Join(",", load.Members)}].");
                        break;
                    case RfLoads.SurfaceLoad load when load.LoadCase <= 0 || load.Surfaces.Count == 0 || load.Surfaces.Any(n => n <= 0):
                        issues.Add($"Surface load No={load.No} in load case {load.LoadCase} has surfaces [{string.Join(",", load.Surfaces)}].");
                        break;
                    case RfLoads.LineLoad load when load.LoadCase <= 0 || load.Lines.Count == 0 || load.Lines.Any(n => n <= 0):
                        issues.Add($"Line load No={load.No} in load case {load.LoadCase} has lines [{string.Join(",", load.Lines)}].");
                        break;
                    case StructureCore.Member member when member.No <= 0:
                        issues.Add("Member has No=0.");
                        break;
                    case StructureCore.Member member when member.Line <= 0 || member.CrossSectionStart <= 0 || member.CrossSectionEnd <= 0:
                        issues.Add($"Member No={member.No} has Line={member.Line}, CrossSectionStart={member.CrossSectionStart} and CrossSectionEnd={member.CrossSectionEnd}; line and both section ends must be > 0.");
                        break;
                }
            }

            return string.Join(" ", issues);
        }

        private static string DescribeObjectList(IEnumerable<IMessage> objects)
        {
            return string.Join(", ", objects.Select(DescribeObject));
        }

        private static string DescribeObject(IMessage obj)
        {
            if (obj == null)
                return "null";

            int no = GetObjectNumber(obj);
            string noText = no > 0 ? $" No={no}" : string.Empty;

            switch (obj)
            {
                case StructureCore.Material material:
                    return $"Material{NameOrEmpty(material.Name)}{noText} HasNo={material.HasNo}";
                case StructureCore.CrossSection section:
                    return $"CrossSection{NameOrEmpty(section.Name)} Material={section.Material}{noText} HasNo={section.HasNo}";
                case StructureCore.Node node:
                    return $"Node{noText} ({node.Coordinate1}, {node.Coordinate2}, {node.Coordinate3}) HasCoordinate1={node.HasCoordinate1}";
                case StructureCore.Line line:
                    return $"Line{noText} Nodes=[{string.Join(",", line.DefinitionNodes)}] HasType={line.HasType}";
                case StructureCore.Member member:
                    return $"Member{noText} Line={member.Line} Section={member.CrossSectionStart} HasType={member.HasType}";
                default:
                    return obj.GetType().Name + noText;
            }
        }

        private static string NameOrEmpty(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? string.Empty : $" '{name}'";
        }

        private static string FormatRpcException(RpcException ex)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"gRPC {ex.Status.StatusCode}");

            if (!string.IsNullOrWhiteSpace(ex.Status.Detail))
                message.Append($": {ex.Status.Detail}");
            else
                message.Append(": RFEM returned no detail. This usually means an invalid payload (unrecognised section/material name, No=0, or a cross-section whose material is missing).");

            if (ex.Trailers != null)
            {
                foreach (Metadata.Entry trailer in ex.Trailers)
                {
                    if (!string.IsNullOrWhiteSpace(trailer.Value) && !trailer.Key.StartsWith("grpc-", StringComparison.OrdinalIgnoreCase))
                        message.Append($" [{trailer.Key}={trailer.Value}]");
                }
            }

            if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                message.Append($" Inner: {ex.InnerException.Message}");

            return message.ToString();
        }
    }
}
