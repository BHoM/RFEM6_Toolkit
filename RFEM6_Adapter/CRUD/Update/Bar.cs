//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using BH.oM.Adapter;
//using BH.oM.Structure.Elements;
//using BH.oM.Structure.SectionProperties;
//using BH.oM.Structure.MaterialFragments;

//using rfModel = Dlubal.WS.Rfem6.Model;

//namespace BH.Adapter.RFEM6
//{
//    public partial class RFEM6Adapter : BHoMAdapter
//    {
//        /***************************************************/
//        /**** Update Node                               ****/
//        /***************************************************/

//        private bool UpdateObjects(IEnumerable<Bar> bars)
//        {
//            bool success = true;

//            Dictionary<int, ISectionProperty> sections = this.GetCachedOrReadAsDictionary<int, ISectionProperty>();
//            Dictionary<int, Node> nodes = this.GetCachedOrReadAsDictionary<int, Node>();

//            foreach (Bar bar in bars)
//            {
//                int sectionID = bar.SectionProperty.GetRFEM6ID();
//                ISectionProperty section;
//                sections.TryGetValue(sectionID, out section);

//                Node node0;
//                nodes.TryGetValue(sectionID, out node0);
//                Node node1;
//                nodes.TryGetValue(sectionID, out node1);



//                model.set_member(bar.ToRFEM6(section.Material.GetRFEM6ID(), materialName));
//            }

//            return success;
//        }

//    }
//}
