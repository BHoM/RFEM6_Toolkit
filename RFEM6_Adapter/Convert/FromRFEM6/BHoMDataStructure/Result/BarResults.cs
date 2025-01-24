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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;
using BH.oM.Structure.Loads;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using Dlubal.WS.Rfem6.Model;
using BH.oM.Adapters.RFEM6.BHoMDataStructure.SupportDatastrures;
using BH.oM.Adapters.RFEM6.Fragments.Enums;
using System.Linq.Expressions;
using System.Diagnostics;
using BH.oM.Base;
using BH.oM.Analytical.Results;
using BH.oM.Structure.Results;
using BH.oM.Structure.Requests;

namespace BH.Adapter.RFEM6
{
	public static partial class Convert
	{

		public static IResult FromRFEM(this members_internal_forces_row memberInternalForces, int lc, double memberLength)
		{

			var f = memberInternalForces.row;

			BarForce barForce = new BarForce(memberInternalForces.row.member_no, lc, -1, -1, Math.Round((double)memberInternalForces.row.location / memberLength, 4), 10, f.internal_force_v, f.internal_force_vy, f.internal_force_vz, f.internal_force_mt, f.internal_force_my, f.internal_force_mz);

			return barForce;
		}

		public static IResult FromRFEM(this members_local_deformations_row memberInternalForces, int lc, double memberLength)
		{

			var f = memberInternalForces.row;

			BarForce barForce = new BarForce(memberInternalForces.row.member_no, lc, -1, -1, Math.Round((double)memberInternalForces.row.location / memberLength, 4), 10, f.displacement_x, f.displacement_y, f.displacement_z, f.rotation_x, f.rotation_y, f.rotation_z);

			return barForce;
		}

		public static IResult FromRFEM(this members_global_deformations_row memberInternalForces, int lc, double memberLength)
		{

			var f = memberInternalForces.row;

			BarForce barForce = new BarForce(memberInternalForces.row.member_no, lc, -1, -1, Math.Round((double)memberInternalForces.row.location / memberLength, 4), 10, f.displacement_x, f.displacement_y, f.displacement_z, f.rotation_x, f.rotation_y, f.rotation_z);

			return barForce;
		}

		public static IResult FromRFEM(this members_strains_row memberInternalForces, int lc, double memberLength)
		{

			var f = memberInternalForces.row;

			BarForce barForce = new BarForce(memberInternalForces.row.member_no, lc, -1, -1, Math.Round((double)memberInternalForces.row.location / memberLength, 4), 10, f.strain_eps_x, f.strain_gamma_xy, f.strain_gamma_xz, f.strain_kappa_x, f.strain_kappa_y, f.strain_kappa_y);

			return barForce;
		}


	}
}


