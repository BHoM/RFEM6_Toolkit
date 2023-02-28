using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<ISectionProperty> ReadSectionProperties(List<string> ids = null)
        {

            List<ISectionProperty> sectionList = new List<ISectionProperty>();

            var sectionNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
            var allSections = sectionNumbers.ToList().Select(n => model.get_section(n.no));

           //  List<rfModel.section>  allSections = new List<rfModel.section>();

            //foreach (var n in sectionNumbers)
            //{

            //    allSections.Add(model.get_section(n.no));

            //}
            Dictionary<int, IMaterialFragment> materials = this.GetCachedOrReadAsDictionary<int, IMaterialFragment>();

            foreach (var section in allSections)
            {

               

                IMaterialFragment material;
                if (!materials.TryGetValue(section.material, out material))
                {
                    material = model.get_material(section.material).FromRFEM();
                    materials[section.material] = material;
                }

                ISectionProperty bhSection = section.FromRFEM(material);

                sectionList.Add(bhSection);

            }

            return sectionList;
        }

    }
}
