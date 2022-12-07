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
            List<IMaterialFragment> materialList = new List<IMaterialFragment>();
            rfModel.object_with_children[] materialsNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MATERIAL);
            List<rfModel.material> materials = new List<rfModel.material>();

            foreach (var i in materialsNumbers)
            {

                //materials.Add(model.get_material(i.no));
                materialList.Add(Convert.FromRFEM(model.get_material(i.no)));

            }

            //foreach (rfModel.material m in materials)
            //{
                
            //    materialList.Add(Convert.FromRFEM(m));

            //}


            


            
            return materialList;
        }

    }
}
