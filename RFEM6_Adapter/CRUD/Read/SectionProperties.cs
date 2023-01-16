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

        private List<IProfile> ReadSectionProperties(List<string> ids = null)
        {

            List<IProfile> sectionList = new List<IProfile>();

            var sectionNumbers = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_SECTION);
            var allSections = sectionNumbers.ToList().Select(n => model.get_section(n.no));

            string name = "";

            // get library sections
            //Get all sections
            var cs1 = BH.Engine.Library.Query.Library("Structure\\SectionProperties");
            var cs2 = BH.Engine.Library.Query.Match("Structure\\SectionProperties", name, true, true);


            foreach (var section in allSections) {

                sectionList.Add(section.FromRFEM());

            }

            return sectionList;
        }

    }
}
