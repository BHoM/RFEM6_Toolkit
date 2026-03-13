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

#if RFEM6_8_2
using System.Reflection;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Adapter.RFEM6
{
    public class RFEM6AdapterV8_2 : RFEM6AdapterBase
    {
        static RFEM6AdapterV8_2()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var name = new System.Reflection.AssemblyName(args.Name);
                if (name.Name == "RFEMWebServiceLibrary" && name.Version?.Minor == 8)
                    return Assembly.LoadFile(@"C:\ProgramData\BHoM\Assemblies\RFEM6_Client\RFEM6_V8_2\RFEMWebServiceLibrary.dll");
                return null;
            };
        }

        [Description("Adapter for RFEM6 using Dlubal Web Service API version 6.8.2. Use this adapter for RFEM6 versions prior to 6.12.11.")]
        [Input("filePath", "Optional file path to the RFEM6 model. Defaults to the currently running instance.")]
        [Input("active", "Set to true to activate the adapter.")]
        [Output("adapter", "The created RFEM6 adapter for API version 6.8.2.")]
        public RFEM6AdapterV8_2(string filePath = "", bool active = false)
            : base(filePath, active) { }
    }
}
#endif
