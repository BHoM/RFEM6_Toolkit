using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BH.oM.Adapter;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Adapters.RFEM6;
using BH.Engine.Base;

using rfModel = Dlubal.WS.Rfem6.Model;

namespace BH.Adapter.RFEM6
{
    public partial class RFEM6Adapter
    {

        private bool CreateCollection(IEnumerable<Bar> bhBars)
        {

            //Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();
            //Dictionary<int, ISectionProperty> sections = this.GetCachedOrReadAsDictionary<int, ISectionProperty>();
            //int nextFreeLineId = model.get_first_free_number(rfModel.object_types.E_OBJECT_TYPE_LINE, 0);
        

            foreach (Bar bhBar in bhBars)
            {

                //Node n0 = bhBar.EndNode;
                ////nodes.TryGetValue(n0.GetRFEM6ID(), out n0);

                //Node n1 = bhBar.StartNode;
                ////nodes.TryGetValue(n1.GetRFEM6ID(), out n1);

                ////Line line = new Line();
                //rfModel.line rfLine= new rfModel.line()
                //{
                //    no = nextFreeLineId,
                //    definition_nodes = new int[] { n0.GetRFEM6ID(), n1.GetRFEM6ID() },
                //    type = rfModel.line_type.TYPE_POLYLINE,
                //};

                //model.set_line(rfLine);

                //ISectionProperty section = null;
                //sections.TryGetValue(bhBar.SectionProperty.GetRFEM6ID(), out section);

                rfModel.member rfMember = bhBar.ToRFEM6();
                model.set_member(rfMember);
                

            }

            return true;
        }

    }
}
