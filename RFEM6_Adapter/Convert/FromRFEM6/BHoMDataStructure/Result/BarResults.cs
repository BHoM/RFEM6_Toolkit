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
		public static IResult FromRFEM(this List<double> val, int lc, double memberLength, double location, int memberNumber, BarResultType resultType)
		{


			IResult result;

			switch (resultType)
			{
				case BarResultType.BarForce:
					result = new BarForce(memberNumber, lc, -1, -1, Math.Round((double)location / memberLength, 4), 10, val[0], val[1], val[2], val[3], val[4], val[5]);
					break;
				case BarResultType.BarDisplacement:
					result = new BarDisplacement(memberNumber, lc, -1, -1, Math.Round((double)location / memberLength, 4), 10, val[0], val[1], val[2], val[3], val[4], val[5]);
					break;

				case BarResultType.BarDeformation:
					result = new BarDeformation(memberNumber, lc, -1, -1, Math.Round((double)location / memberLength, 4), 10, val[0], val[1], val[2], val[3], val[4], val[5]);
					break;

				case BarResultType.BarStrain:
					result = new BarStrain(memberNumber, lc, -1, -1, Math.Round((double)location / memberLength, 4), 10, val[0], val[1], val[2], 0,0,0,0,0,0);
					BH.Engine.Base.Compute.RecordWarning($"For BarResultType {resultType}, no bending around Y or Z axis or combined axis bending has been determined.");
					break;

				default:
					BH.Engine.Base.Compute.RecordError($"No conversion method for BarResultType {resultType} has been implemented.");
					return null;
			}


			return result;
		}

	}
}


