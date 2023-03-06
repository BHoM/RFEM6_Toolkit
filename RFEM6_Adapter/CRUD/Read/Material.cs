using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<IMaterialFragment> ReadMaterial(List<string> ids = null)
        {

            //var concreteLib = BH.Engine.Library.Query.Library("Concrete");
            //var steelLib = BH.Engine.Library.Query.Library("Steel");

            List<IMaterialFragment> materialList = new List<IMaterialFragment>();
            rfModel.object_with_children[] materialsNumbers = m_Model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
            List<rfModel.material> allMaterials = new List<rfModel.material>();

            foreach(var n in materialsNumbers) {

                allMaterials.Add(m_Model.get_material(n.no));

            }


            if (ids==null) {

                foreach (var rfMaterial in allMaterials)
                {


                    IMaterialFragment material = rfMaterial.FromRFEM();

                    materialList.Add(material);

                }

            }
          
            return materialList;
        }

    }
}
