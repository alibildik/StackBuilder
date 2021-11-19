#region Using directives
using System;
using System.IO;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public abstract class Exporter
    {
        public abstract void Export(AnalysisLayered analysis, ref Stream stream);
        public abstract string Name { get; }
        public abstract string Extension { get; }
        public abstract string Filter { get; }
    }
}
