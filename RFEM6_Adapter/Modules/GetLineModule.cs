using BH.oM.Base;
using BH.oM.Adapter;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Adapters.RFEM6;
using System.Collections;

namespace BH.Adapter.RFEM6
{
    [Description("Dependency module for fetching all Loadcase stored in a list of Loadcombinations.")]
    public class GetLineModule : IGetDependencyModule<Bar, RFEMLine>
    {
        public IEnumerable<RFEMLine> GetDependencies(IEnumerable<Bar> objects)
        {
            List< RFEMLine> lines = new List<RFEMLine>();
            foreach (Bar bar in objects)
            {
                RFEMLine rfLine = new RFEMLine() { StartNode = bar.StartNode, EndNode = bar.EndNode };
                bar.Fragments.Add(rfLine);
                lines.Add(rfLine);
            }

            return lines;
        }
    }
}