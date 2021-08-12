#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sharp3D.Math.Core;

using log4net;
using log4net.Config;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Graphics;
#endregion

namespace treeDiM.StackBuilder.Engine.Multiblock.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            _log.Info("Started...");

            // instantiate pallet
            var dimContainer = new Vector3D(1200.0, 1000.0, 1700.0);
            var dimCase = new Vector3D(400.0, 350.0, 100.0);

            var multiblockSolutions = 


            _log.Info("Done...");
        }

        protected static ILog _log = LogManager.GetLogger(typeof(Program));
    }
}
