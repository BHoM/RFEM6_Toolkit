using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM6;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class Convert
    {

        public static IMaterialFragment FromRFEM(this rfModel.material rfMaterial)
        {

            string s = rfMaterial.generating_object_info;
            IMaterialFragment bhMaterial = null;

            String[] matParaArray = rfMaterial.comment.Split('|');

            if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_STEEL) || rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_REINFORCING_STEEL))
            {

                bhMaterial=BH.Engine.Library.Query.Match("Steel", rfMaterial.name.Split('|')[0], true, true) as IMaterialFragment;

            }

            else if (rfMaterial.material_type.Equals(rfModel.material_material_type.TYPE_CONCRETE))
            {

                bhMaterial = BH.Engine.Library.Query.Match("Concrete", rfMaterial.name.Split('|')[0], true, true) as IMaterialFragment;

            }

            return bhMaterial;
        }

    }
}
