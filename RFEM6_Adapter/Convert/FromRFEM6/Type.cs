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

        public static Type FromRFEM(rfModel.object_types rfType)
        {

            if (rfType == rfModel.object_types.E_OBJECT_TYPE_NODE)
            {
                return typeof(Node);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_NODAL_SUPPORT)
            {  
                return typeof(Constraint6DOF);
            }
            else if(rfType== rfModel.object_types.E_OBJECT_TYPE_MATERIAL)
            {
                return typeof(IMaterialFragment);
            }
            else if (rfType == rfModel.object_types.E_OBJECT_TYPE_SECTION)
            {
                return typeof(ISectionProperty);
            }
      

            return null;
        }

    }
}
