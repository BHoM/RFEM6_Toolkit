using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private List<Bar> ReadBars(List<string> ids = null)
        {

            List<Bar> barList = new List<Bar>();

            var barNumber = model.get_all_object_numbers_by_type(rfModel.object_types.E_OBJECT_TYPE_MEMBER);
            
            
            //var allRfMembers = barNumber.ToList().Select(n => model.get_member(n.no));
            //var allRfMembers =
            List<rfModel.member> allRfMembers = new List<rfModel.member>();

            foreach (var n in barNumber) {

                allRfMembers.Add(model.get_member(n.no));

            }

            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();
            Dictionary<int, ISectionProperty> sections = this.GetCachedOrReadAsDictionary<int, ISectionProperty>();

            foreach (var rfMember in allRfMembers)
            {

                Node node0 = null;
                nodes.TryGetValue(rfMember.nodes[0], out node0);

                Node node1 = null;
                nodes.TryGetValue(rfMember.nodes[1], out node1);

                ISectionProperty section = null;
                sections.TryGetValue(rfMember.section_end, out section);

                Bar bhBar = rfMember.FromRFEM(node0,node1,section);

                barList.Add(bhBar);

            }

            return barList;
        }

    }
}
