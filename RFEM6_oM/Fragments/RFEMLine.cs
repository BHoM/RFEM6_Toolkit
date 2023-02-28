using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using System.ComponentModel;
using BH.oM.Structure.Elements;
namespace BH.oM.Adapters.RFEM6
{
    public class RFEMLine : BHoMObject, IFragment
    {
        [Description("Defines the start position of the element. Note that Nodes can contain Supports which should not be confused with Releases.")]
        public virtual Node StartNode { get; set; }
        [Description("Defines the end position of the element. Note that Nodes can contain Supports which should not be confused with Releases.")]
        public virtual Node EndNode { get; set; }
    }
}