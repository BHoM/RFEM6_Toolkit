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

        private bool CreateCollection(IEnumerable<IMaterialFragment> materialFragments)
        {
            
            rfModel.object_with_children[] materials = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
            var materialNumbers = materials.ToList().Select(m=>m.no);
            
            List<rfModel.material> materialList = new List<rfModel.material>();

            foreach (int m in materialNumbers)
            {

                materialList.Add(model.get_material(m));

            }        

            foreach (IMaterialFragment m in materialFragments)
            {

                int materialNo = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_MATERIAL, 0);

                rfModel.material rfMaterial = Convert.ToRFEM6(m,materialNo);

                bool materialExistAlready = materialList.Any(k=>k.name.Split(' ')[0].Equals(rfMaterial.name));

                if (!materialExistAlready) {
                    model.set_material(rfMaterial);
                }

            }

            return true;
        }

    }
}
