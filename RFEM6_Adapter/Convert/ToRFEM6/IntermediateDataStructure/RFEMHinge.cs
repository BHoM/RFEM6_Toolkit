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
using BH.Engine.Base;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {

        public static rfModel.member_hinge ToRFEM6(this RFEMHinge hinge)
        {

            rfModel.member_hinge rfNode = new rfModel.member_hinge()
            {
                no = hinge.GetRFEM6ID(),
                axial_release_n = StiffnessTranslationBHToRF("" + hinge.Constraint.TranslationX),
                axial_release_nSpecified = true,
                //axial_release_n_nonlinearity = member_hinge_axial_release_n_nonlinearity.NONLINEARITY_TYPE_NONE,
                //axial_release_n_nonlinearitySpecified = true,
                axial_release_vy = StiffnessTranslationBHToRF("" + hinge.Constraint.TranslationY),
                axial_release_vySpecified = true,
                //axial_release_vy_nonlinearity = member_hinge_axial_release_vy_nonlinearity.NONLINEARITY_TYPE_NONE,
                //axial_release_vy_nonlinearitySpecified = true,
                axial_release_vz = StiffnessTranslationBHToRF("" + hinge.Constraint.TranslationZ),
                axial_release_vzSpecified = true,
                //axial_release_vz_nonlinearity = member_hinge_axial_release_vz_nonlinearity.NONLINEARITY_TYPE_NONE,
                //axial_release_vz_nonlinearitySpecified = true,
                moment_release_mt = StiffnessTranslationBHToRF("" + hinge.Constraint.RotationX),
                moment_release_mtSpecified = true,
                //moment_release_mt_nonlinearity = member_hinge_moment_release_mt_nonlinearity.NONLINEARITY_TYPE_NONE,
                //moment_release_mt_nonlinearitySpecified = true,
                moment_release_my = StiffnessTranslationBHToRF("" + hinge.Constraint.RotationY),
                moment_release_mySpecified = true,
                //moment_release_my_nonlinearity = member_hinge_moment_release_my_nonlinearity.NONLINEARITY_TYPE_NONE,
                //moment_release_my_nonlinearitySpecified = true,
                moment_release_mz = StiffnessTranslationBHToRF("" + hinge.Constraint.RotationZ),
                moment_release_mzSpecified = true,
                //moment_release_mz_nonlinearity = member_hinge_moment_release_mz_nonlinearity.NONLINEARITY_TYPE_DIAGRAM,
                //moment_release_mz_nonlinearitySpecified = true,

                //no = 1,
                //user_defined_name_enabled = true,
                //user_defined_name_enabledSpecified = true,
                //name = "Scripted hinge",
                //coordinate_system = "Local",
                //axial_release_n = 0,
                //axial_release_nSpecified = true,
                //axial_release_n_nonlinearity = member_hinge_axial_release_n_nonlinearity.NONLINEARITY_TYPE_NONE,
                //axial_release_n_nonlinearitySpecified = true,
                //axial_release_vy = double.PositiveInfinity,
                //axial_release_vySpecified = true,
                //axial_release_vy_nonlinearity = member_hinge_axial_release_vy_nonlinearity.NONLINEARITY_TYPE_NONE,
                //axial_release_vy_nonlinearitySpecified = true,
                //axial_release_vz = double.PositiveInfinity,
                //axial_release_vzSpecified = true,
                //axial_release_vz_nonlinearity = member_hinge_axial_release_vz_nonlinearity.NONLINEARITY_TYPE_NONE,
                //axial_release_vz_nonlinearitySpecified = true,
                //comment = "Scripted hinge comment",
                //diagram_around_z_end = member_hinge_diagram_around_z_end.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                //diagram_around_z_endSpecified = true,
                //diagram_around_z_is_sorted = true,
                //diagram_around_z_is_sortedSpecified = true,
                //diagram_around_z_start = member_hinge_diagram_around_z_start.DIAGRAM_ENDING_TYPE_CONTINUOUS,
                //diagram_around_z_startSpecified = true,
                //diagram_around_z_symmetric = true,
                //diagram_around_z_symmetricSpecified = true,
                //diagram_around_z_table = new[] { new member_hinge_diagram_around_z_table_row() {
                //    no = 1,
                //    row = new member_hinge_diagram_around_z_table(){
                //    rotation = 0.01,
                //    rotationSpecified = true,
                //    moment = 20000,
                //    momentSpecified = true,
                //    spring = 2e+06,
                //    springSpecified = true,
                //    // note = "rrfrefre",
                //    }
                //}
                //},
                //members = "",
                //moment_release_mt = double.PositiveInfinity,
                //moment_release_mtSpecified = true,
                //moment_release_mt_nonlinearity = member_hinge_moment_release_mt_nonlinearity.NONLINEARITY_TYPE_NONE,
                //moment_release_mt_nonlinearitySpecified = true,
                //moment_release_my = double.PositiveInfinity,
                //moment_release_mySpecified = true,
                //moment_release_my_nonlinearity = member_hinge_moment_release_my_nonlinearity.NONLINEARITY_TYPE_NONE,
                //moment_release_my_nonlinearitySpecified = true,
                //moment_release_mz = double.PositiveInfinity,
                //moment_release_mzSpecified = true,
                //moment_release_mz_nonlinearity = member_hinge_moment_release_mz_nonlinearity.NONLINEARITY_TYPE_DIAGRAM,
                //moment_release_mz_nonlinearitySpecified = true,
                //partial_activity_along_x_negative_slippage = 0,
                //partial_activity_along_x_negative_slippageSpecified = true,
                //partial_activity_along_x_positive_slippage = 0,
                //partial_activity_along_x_positive_slippageSpecified = true,
                //partial_activity_along_y_negative_slippage = 0,
                //partial_activity_along_y_negative_slippageSpecified = true,
                //partial_activity_along_y_positive_slippage = 0,
                //partial_activity_along_y_positive_slippageSpecified = true,
                //partial_activity_along_z_negative_slippage = 0,
                //partial_activity_along_z_negative_slippageSpecified = true,
                //partial_activity_along_z_positive_slippage = 0,
                //partial_activity_along_z_positive_slippageSpecified = true,
                //partial_activity_around_x_negative_slippage = 0,
                //partial_activity_around_x_negative_slippageSpecified = true,
                //partial_activity_around_x_positive_slippage = 0,
                //partial_activity_around_x_positive_slippageSpecified = true,


            };

            return rfNode;

        }

    }
}


