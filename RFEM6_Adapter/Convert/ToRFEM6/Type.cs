using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static rfModel.object_types? ToRFEM6(this Type bhType)
        {
            if (bhType == typeof(Node))
            {
                return rfModel.object_types.E_OBJECT_TYPE_NODE;
            } 
            else if (bhType == typeof(Constraint6DOF))
            {
                return rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT;
            }
            else if (bhType == typeof(IMaterialFragment) || bhType.GetInterfaces().Contains(typeof(IMaterialFragment)))
            {
                return rfModel.object_types.E_OBJECT_TYPE_MATERIAL;
            }
            else if (bhType == typeof(ISectionProperty))
            {
                return rfModel.object_types.E_OBJECT_TYPE_SECTION;
            }
            else if (bhType == typeof(RFEMLine))
            {
                return rfModel.object_types.E_OBJECT_TYPE_LINE;
            }
            else if (bhType == typeof(Bar))
            {
                return rfModel.object_types.E_OBJECT_TYPE_MEMBER;
            }

            return null;

        }
    }
}
