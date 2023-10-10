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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;
using Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public static partial class Convert
    {
        public static RFEMNodalSupport FromRFEM(this rfModel.nodal_support support)
        {

            BH.oM.Structure.Constraints.Constraint6DOF constraint = new BH.oM.Structure.Constraints.Constraint6DOF();
            constraint.TranslationX = (support.spring.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationY = (support.spring.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationZ = (support.spring.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationX = (support.rotational_restraint.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationY = (support.rotational_restraint.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationZ = (support.rotational_restraint.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            //constraint.TranslationalStiffnessX = support.spring.x;
            //constraint.TranslationalStiffnessY = support.spring.y;
            //constraint.TranslationalStiffnessZ = support.spring.z;
            //constraint.RotationalStiffnessX = support.rotational_restraint.x;
            //constraint.RotationalStiffnessY = support.rotational_restraint.y;
            //constraint.RotationalStiffnessZ = support.rotational_restraint.z;
            

            constraint.SetRFEM6ID(support.no);
            constraint.Name = support.name;
            
            RFEMNodalSupport rfemNodalSupport = new RFEMNodalSupport() {Constraint=constraint};
            rfemNodalSupport.SetRFEM6ID(support.no);
            
            return rfemNodalSupport;
        }


        public static RFEMLineSupport FromRFEM(this rfModel.line_support support)
        {

            BH.oM.Structure.Constraints.Constraint6DOF constraint = new BH.oM.Structure.Constraints.Constraint6DOF();
            constraint.TranslationX = (support.spring.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationY = (support.spring.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationZ = (support.spring.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationX = (support.rotational_restraint.x == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationY = (support.rotational_restraint.y == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationZ = (support.rotational_restraint.z == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            //constraint.TranslationalStiffnessX = support.spring.x;
            //constraint.TranslationalStiffnessY = support.spring.y;
            //constraint.TranslationalStiffnessZ = support.spring.z;
            //constraint.RotationalStiffnessX = support.rotational_restraint.x;
            //constraint.RotationalStiffnessY = support.rotational_restraint.y;
            //constraint.RotationalStiffnessZ = support.rotational_restraint.z;


            constraint.SetRFEM6ID(support.no);
            constraint.Name = support.name;

            RFEMLineSupport rfemLineSupport = new RFEMLineSupport() { Constraint = constraint,nodesIDs=support.lines.ToList() };
            rfemLineSupport.SetRFEM6ID(support.no);

            return rfemLineSupport;
        }


        public static RFEMHinge FromRFEM(this rfModel.member_hinge hinge)
        {



            BH.oM.Structure.Constraints.Constraint6DOF constraint = new BH.oM.Structure.Constraints.Constraint6DOF();
            constraint.TranslationX = (hinge.axial_release_n == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationY = (hinge.axial_release_vy == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.TranslationZ = (hinge.axial_release_vz == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationX = (hinge.moment_release_mt == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationY = (hinge.moment_release_my == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            constraint.RotationZ = (hinge.moment_release_mz == Double.PositiveInfinity ? oM.Structure.Constraints.DOFType.Fixed : oM.Structure.Constraints.DOFType.Free);
            //constraint.TranslationalStiffnessX = support.spring.x;
            //constraint.TranslationalStiffnessY = support.spring.y;
            //constraint.TranslationalStiffnessZ = support.spring.z;
            //constraint.RotationalStiffnessX = support.rotational_restraint.x;
            //constraint.RotationalStiffnessY = support.rotational_restraint.y;
            //constraint.RotationalStiffnessZ = support.rotational_restraint.z;
            

            //constraint.SetRFEM6ID(hinge.no);
            //constraint.Name = hinge.name;

            RFEMHinge rfemLineSupport = new RFEMHinge() {Constraint=constraint};
            rfemLineSupport.SetRFEM6ID(hinge.no);

            return rfemLineSupport;
        }


    }
}
