#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    public abstract class Exporter3D : Exporter
    {

        private static Exporter3D[] Exporters =>
            new Exporter3D[]
            {
                new ExporterCollada(),
                new ExporterGLB()
            };

        public static Exporter GetByName(string name)
        {
            foreach (Exporter exp in Exporters)
            {
                if (string.Equals(exp.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    return exp;
            }
            throw new ExceptionInvalidName(name);
        }
    }
}
