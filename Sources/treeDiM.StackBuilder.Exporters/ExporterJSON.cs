#region Using directives
using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Exporters.Json;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    class ExporterJSON : Exporter
    {
        public ExporterJSON() {}
        public override string Name => "json";
        public override string Filter => "json|*.json";
        public override string Extension => "json";
        public override void Export(AnalysisLayered analysis, ref Stream stream)
        {
            SolutionLayered sol = analysis.SolutionLay;
            string nameContainer = analysis.Container.Name;
            string nameContent = analysis.Content.Name;

            // Geometry container
            Geometry geomContainer = new Geometry()
            {
                Uuid = Guid.NewGuid().ToString(),
                Type = "Geometry",
                Data = new Data()
                {
                    Vertices = new List<double>(),
                    Normals = new List<double>(),
                    Uvs = new List<List<double>>(),
                    Faces = new List<int>()
                }
            };

            // Material container
            Material matContainer = new Material()
            {
                Uuid = Guid.NewGuid().ToString(),
                Type = "Material",
                Name = string.Format("Mat_{0}", nameContainer)
            };

            // Geometry content
            Geometry geomContent = new Geometry()
            {
            };

            // Material content
            Material matContent = new Material()
            {
                Uuid = Guid.NewGuid().ToString(),
                Type = "Material",
                Name = string.Format("Mat_{0}", nameContent)

            };

            RootObject rootObj = new RootObject()
            {
                Geometries = new List<Geometry>() { geomContainer, geomContent },
                Materials = new List<Material>() { matContainer, matContent },
                Object = new Json.Object()
            };
            string filePath = string.Empty;
            File.WriteAllText( filePath, JsonConvert.SerializeObject(rootObj) );
        }
    }
}
