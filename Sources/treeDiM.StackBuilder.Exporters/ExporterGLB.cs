#region Using directives
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Numerics;

using Sharp3D.Math.Core;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
#endregion

namespace treeDiM.StackBuilder.Exporters
{
    using VPOS = VertexPosition;
    using PRIMITIVE = PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty>;

    #region ExporterGLB
    public class ExporterGLB : Exporter3D
    {
        #region Static members
        public static string FormatName => "glb";
        #endregion

        #region Constructor
        public ExporterGLB() {}
        #endregion

        #region Override Exporter
        public override string Name => FormatName;
        public override string Extension => "glb";
        public override string Filter => "GL Transmission Format (*.glb)";
        public override void Export(AnalysisLayered analysis, ref Stream stream) { }
        public void Export(AnalysisLayered analysis, string filePath)
        {
            // solution
            var sol = analysis.SolutionLay;
            // scene 
            var scene = new SceneBuilder();

            if (analysis.Content is BoxProperties boxProperties)
            {
                var colors = boxProperties.Colors;
                var colorFilet = boxProperties.Colors[0];
                var colorTape = boxProperties.TapeColor;

                var meshPallet = BuildPalletMesh(analysis.Container as PalletProperties);
                var meshCase = BuildCaseMesh("Case", (float)boxProperties.Length, (float)boxProperties.Width, (float)boxProperties.Height,
                    colors, 0.0f, colorFilet,
                    boxProperties.TapeWidth.Activated ? (float)boxProperties.TapeWidth.Value : 0.0f, colorTape);
                var meshesInterlayer = BuildInterlayerMeshes(analysis);

                // add pallet mesh
                scene.AddRigidMesh(meshPallet, Matrix4x4.Identity);
                // add cases (+ interlayers) mesh 
                
                var layers = sol.Layers;
                foreach (ILayer layer in layers)
                {
                    if (layer is Layer3DBox layerBox)
                    {
                        foreach (var bPosition in layerBox)
                            scene.AddRigidMesh(meshCase, BoxPositionToMatrix4x4(bPosition));
                    }
                    else if (layer is Layer3DBoxIndexed layerBoxi)
                    {
                        foreach (var bposi in layerBoxi)
                            scene.AddRigidMesh(meshCase, BoxPositionToMatrix4x4(bposi.BPos));
                    }
                    else if (layer is InterlayerPos interlayerPos)
                    {
                        var interlayerProp = sol.Interlayers[interlayerPos.TypeId];
                        var bPosition = new BoxPosition(new Vector3D(
                            0.5 * (analysis.ContainerDimensions.X - interlayerProp.Length)
                            , 0.5 * (analysis.ContainerDimensions.Y - interlayerProp.Width)
                            , interlayerPos.ZLow),
                            HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                        scene.AddRigidMesh(meshesInterlayer[interlayerPos.TypeId], BoxPositionToMatrix4x4(bPosition));
                    }
                }
            }
            // add pallet cap if any
            if (analysis is AnalysisCasePallet analysisCasePallet && analysisCasePallet.HasPalletCap)
            {
                var capProperties = analysisCasePallet.PalletCapProperties;
                var bPosition = new BoxPosition(new Vector3D(
                    0.5 * (analysisCasePallet.PalletProperties.Length - capProperties.Length),
                    0.5 * (analysisCasePallet.PalletProperties.Width - capProperties.Width),
                    sol.BBoxLoad.PtMax.Z - capProperties.InsideHeight)
                    , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P
                    );

                scene.AddRigidMesh( BuildPalletCapMesh(
                    (float)capProperties.Length, (float)capProperties.Width, (float)capProperties.Height,
                    (float)capProperties.InsideLength, (float)capProperties.InsideWidth, (float)capProperties.InsideHeight,
                    capProperties.Color),
                    BoxPositionToMatrix4x4(bPosition) );
            }
            // save model
            var model = scene.ToGltf2();
            var fileExtension = Path.GetExtension(filePath).Trim('.');
            switch (fileExtension)
            {
                case "glb": model.SaveGLB(filePath); break;
                case "obj": model.SaveAsWavefront(filePath); break;
                default: new System.Exception($"Unexpected file extension : {fileExtension}"); break;
            }
        }
        #endregion

        #region Helpers
        private Matrix4x4 BoxPositionToMatrix4x4(BoxPosition boxPosition)
        {
            var m = boxPosition.Transformation.Matrix;
            m.Transpose();
            return new Matrix4x4(
                (float)m.M11, (float)m.M12, (float)m.M13, (float)m.M14,
                (float)m.M21, (float)m.M22, (float)m.M23, (float)m.M24,
                (float)m.M31, (float)m.M32, (float)m.M33, (float)m.M34,
                (float)m.M41, (float)m.M42, (float)m.M43, (float)m.M44
                );
        }
        private MeshBuilder<VPOS/*, VTEX*/> BuildCaseMesh(
            string meshName,
            float length, float width, float height,
            Color[] colorFaces,
            float filet,  Color colorFilet, 
            float tapeWidth, Color colorTape
            )
        {
            var mesh = new MeshBuilder<VPOS/*, VTEX*/>(meshName);
            //
            //        7--------------- 6                 T              { 3,0,4,7 }  // Left
            //       /|               /|                 | K            { 1,2,6,5 }  // Right 
            //      / |              / |                 |/             { 0,1,5,4 }  // Front
            //     /  3-------------/- 2           L---- | ----R        { 2,3,7,6 }  // Rear
            //    4--/------------ 5  /                 /|              { 0,3,2,1 }  // Bottom
            //    | /              | /                 F |              { 4,5,6,7 }  // Top
            //    |/               |/                    B      
            //    0----------------1
            //
            VPOS[] vertices = new VPOS[]
            {
                // 0
                new VPOS(0.0f, 0.0f, 0.0f),                           // 0
                new VPOS(0.0f + filet, 0.0f + filet, 0.0f), // B      // 1
                new VPOS(0.0f + filet, 0.0f, 0.0f + filet), // F      // 2
                new VPOS(0.0f, 0.0f + filet, 0.0f + filet), // L      // 3
                // 1
                new VPOS(length, 0.0f, 0.0f),                         // 4
                new VPOS(length - filet, 0.0f + filet, 0.0f), // B    // 5    
                new VPOS(length - filet, 0.0f, 0.0f + filet), // F    // 6
                new VPOS(length, 0.0f + filet, 0.0f + filet), // R    // 7
                // 2
                new VPOS(length, width, 0.0f),                        // 8
                new VPOS(length - filet, width - filet, 0.0f), // B   // 9
                new VPOS(length - filet, width, 0.0f + filet), // K   // 10
                new VPOS(length, width - filet, 0.0f + filet), // R   // 11
                // 3
                new VPOS(0.0f, width, 0.0f),                          // 12
                new VPOS(0.0f + filet, width - filet, 0.0f), // B     // 13
                new VPOS(0.0f + filet, width, 0.0f + filet), // K     // 14
                new VPOS(0.0f, width - filet, 0.0f + filet), // L     // 15
                // 4
                new VPOS(0.0f, 0.0f, height),                         // 16
                new VPOS(0.0f + filet, 0.0f + filet, height), // T    // 17
                new VPOS(0.0f + filet, 0.0f, height - filet), // F    // 18    
                new VPOS(0.0f, 0.0f + filet, height - filet), // L    // 19
                // 5
                new VPOS(length, 0.0f, height),                       // 20
                new VPOS(length - filet, 0.0f + filet, height), // T  // 21
                new VPOS(length - filet, 0.0f, height - filet), // F  // 22
                new VPOS(length, 0.0f + filet, height - filet), // R  // 23
                // 6
                new VPOS(length, width, height),                      // 24
                new VPOS(length - filet, width - filet, height), // T // 25
                new VPOS(length - filet, width, height - filet), // K // 26
                new VPOS(length, width - filet, height - filet), // R // 27    
                // 7
                new VPOS(0.0f, width, height),                        // 28
                new VPOS(0.0f + filet, width - filet, height), // T   // 29
                new VPOS(0.0f + filet, width, height - filet), // K   // 30
                new VPOS(0.0f, width - filet, height - filet),  // L   // 31
                // 8
                new VPOS(0.0f + filet, 0.5f*(width - tapeWidth), height), // 32
                new VPOS(length - filet, 0.5f*(width - tapeWidth), height), // 33
                new VPOS(length - filet, 0.5f*(width + tapeWidth), height), // 34
                new VPOS(0.0f + filet, 0.5f*(width + tapeWidth), height) //35
            };

            Vector3[] vectors = {
                // 0
                new Vector3(0.0f, 0.0f, 0.0f),                           // 0
                new Vector3(0.0f + filet, 0.0f + filet, 0.0f), // B      // 1
                new Vector3(0.0f + filet, 0.0f, 0.0f + filet), // F      // 2
                new Vector3(0.0f, 0.0f + filet, 0.0f + filet), // L      // 3
                // 1
                new Vector3(length, 0.0f, 0.0f),                         // 4
                new Vector3(length - filet, 0.0f + filet, 0.0f), // B    // 5    
                new Vector3(length - filet, 0.0f, 0.0f + filet), // F    // 6
                new Vector3(length, 0.0f + filet, 0.0f + filet), // R    // 7
                // 2
                new Vector3(length, width, 0.0f),                        // 8
                new Vector3(length - filet, width - filet, 0.0f), // B   // 9
                new Vector3(length - filet, width, 0.0f + filet), // K   // 10
                new Vector3(length, width - filet, 0.0f + filet), // R   // 11
                // 3
                new Vector3(0.0f, width, 0.0f),                          // 12
                new Vector3(0.0f + filet, width - filet, 0.0f), // B     // 13
                new Vector3(0.0f + filet, width, 0.0f + filet), // K     // 14
                new Vector3(0.0f, width - filet, 0.0f + filet), // L     // 15
                // 4
                new Vector3(0.0f, 0.0f, height),                         // 16
                new Vector3(0.0f + filet, 0.0f + filet, height), // T    // 17
                new Vector3(0.0f + filet, 0.0f, height - filet), // F    // 18    
                new Vector3(0.0f, 0.0f + filet, height - filet), // L    // 19
                // 5
                new Vector3(length, 0.0f, height),                       // 20
                new Vector3(length - filet, 0.0f + filet, height), // T  // 21
                new Vector3(length - filet, 0.0f, height - filet), // F  // 22
                new Vector3(length, 0.0f + filet, height - filet), // R  // 23
                // 6
                new Vector3(length, width, height),                      // 24
                new Vector3(length - filet, width - filet, height), // T // 25
                new Vector3(length - filet, width, height - filet), // K // 26
                new Vector3(length, width - filet, height - filet), // R // 27    
                // 7
                new Vector3(0.0f, width, height),                        // 28
                new Vector3(0.0f + filet, width - filet, height), // T   // 29
                new Vector3(0.0f + filet, width, height - filet), // K   // 30
                new Vector3(0.0f, width - filet, height - filet),  // L   // 31
                // 8
                new Vector3(0.0f + filet, 0.5f*(width - tapeWidth), height), // 32
                new Vector3(length - filet, 0.5f*(width - tapeWidth), height), // 33
                new Vector3(length - filet, 0.5f*(width + tapeWidth), height), // 34
                new Vector3(0.0f + filet, 0.5f*(width + tapeWidth), height) //35
                };

            int[,] qFace = new int[,]
            {
                { 15,  3, 19, 31 }, // left
                {  7, 11, 27, 23 }, // right
                {  2,  6, 22, 18 }, // front
                { 10, 14, 30, 26 }, // rear
                { 1,  13,  9,  5 }, // bottom
                { 17, 21, 33, 32 },  // top1
                { 35, 34, 25, 29 },  // top2
                { 32, 33, 34, 35 }   // tape
            };

            int[,] qFaceFilet = new int[,]
            {
                {  1,  5,  6,  2 },
                {  5,  9, 11,  7 },
                {  9, 13, 14, 10 },
                { 13,  1,  3, 15 },
                { 18, 22, 21, 17 },
                { 23, 27, 25, 21 },
                { 26, 30, 29, 25 },
                { 31, 19, 17, 29 },
                {  3,  2, 18, 19 },
                {  6,  7, 23, 22 },
                { 11, 10, 26, 27 },
                { 14, 15, 31, 30 }
            };
            int[,] edges = new int[,]
            {
                { 0, 4 },   // 0
                { 4, 8 },   // 1
                { 8, 12 },  // 2
                { 12, 0 },  // 3
                { 16, 20 }, // 4
                { 20, 24 }, // 5
                { 24, 28 }, // 6
                { 28, 16 }, // 7
                { 0, 16 },  // 8
                { 4, 20 },  // 9
                { 8, 24 },  // 10
                { 12, 28 }  // 11
            };
            var materialBlack = new MaterialBuilder()
                .WithUnlitShader()
                .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(Color.Black));

            for (int i = 0; i < 7; ++i)
            {
                MaterialBuilder materialColor = new MaterialBuilder()
                    .WithDoubleSide(true)
                    .WithChannelParam(KnownChannel.BaseColor,
                    ColorToVector4(colorFaces[i < 6 ? i : 5]));
                /*
                if (i == 1)
                {
                    materialColor = new MaterialBuilder().WithChannelImage(KnownChannel.BaseColor, @"D:\\testImage.png");
                    materialColor.GetChannel(KnownChannel.BaseColor).UseTexture().WithTransform(1.0f, 1.0f, 1.0f, 1.0f);
                }
                */
                var primFace = mesh.UsePrimitive(materialColor); // 1 2 6 5
                /*
                Vector2 v3 = new Vector2(0.0f, 0.0f);
                Vector2 v2 = new Vector2(1.0f, 0.0f);
                Vector2 v1 = new Vector2(1.0f, 1.0f);
                Vector2 v0 = new Vector2(0.0f, 1.0f);
                if (i == 1)
                    primFace.AddQuadrangle(
                        (vectors[qFace[i, 0]], v0 ),
                        (vectors[qFace[i, 1]], v1 ),
                        (vectors[qFace[i, 2]], v2 ),
                        (vectors[qFace[i, 3]], v3 )
                        );
                else
                */
                primFace.AddQuadrangle(vertices[qFace[i, 0]], vertices[qFace[i, 1]], vertices[qFace[i, 2]], vertices[qFace[i, 3]]);
            }
            if (tapeWidth > 0.0f)
            { 
                var materialTape = new MaterialBuilder()
                    .WithDoubleSide(true)
                    .WithMetallicRoughnessShader()
                    .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(colorTape));

                var primFace = mesh.UsePrimitive(materialTape);
                primFace.AddQuadrangle(vertices[qFace[7, 0]], vertices[qFace[7, 1]], vertices[qFace[7, 2]], vertices[qFace[7, 3]]);

                if (filet == 0)
                {
                    var primTapeBorder = mesh.UsePrimitive(materialBlack, 2);
                    primTapeBorder.AddLine(vertices[qFace[7, 0]], vertices[qFace[7, 1]]);
                    primTapeBorder.AddLine(vertices[qFace[7, 1]], vertices[qFace[7, 2]]);
                    primTapeBorder.AddLine(vertices[qFace[7, 2]], vertices[qFace[7, 3]]);
                    primTapeBorder.AddLine(vertices[qFace[7, 3]], vertices[qFace[7, 0]]);
                }
            }
            if (filet > 0)
            {
                var materialFilet = new MaterialBuilder()
                    .WithDoubleSide(true)
                    .WithMetallicRoughnessShader()
                    .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(colorFilet));
                var prim = mesh.UsePrimitive(materialFilet);
                for (int i = 0; i < 12; ++i)
                    prim.AddQuadrangle(
                        vertices[qFaceFilet[i, 0]],
                        vertices[qFaceFilet[i, 1]],
                        vertices[qFaceFilet[i, 2]],
                        vertices[qFaceFilet[i, 3]]
                        );
            }
            else
            {
                var prim = mesh.UsePrimitive(materialBlack, 2);
                for (int i = 0; i < 12; ++i)
                    prim.AddLine(vertices[edges[i, 0]], vertices[edges[i, 1]]);            
            }
            return mesh;
        }
        private MeshBuilder<VPOS/*, VTEX*/> BuildPalletMesh(PalletProperties palletProperties)
        {
            var mesh = new MeshBuilder<VPOS/*, VTEX*/>("pallet");
            Pallet pallet = new Pallet(palletProperties);

            // faces
            var materialFaces = new MaterialBuilder()
                .WithDoubleSide(true)
                .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(palletProperties.Color));
            var primFaces = mesh.UsePrimitive(materialFaces);
            // edges
            var materialBlack = new MaterialBuilder()
                .WithUnlitShader()
                .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(Color.Black));
            var primEdges = mesh.UsePrimitive(materialBlack, 2);

            foreach (var b in pallet.BuildListOfBoxes())
            {
                AddBox(primFaces, primEdges, (float)b.Length, (float)b.Width, (float)b.Height, 0.0f, b.BoxPosition);
            }
            return mesh;
        }
        private void AddBox(PRIMITIVE primFaces, PRIMITIVE primEdges, float length, float width, float height, float filet, BoxPosition boxPosition)
        {
            VPOS[] vertices = new VPOS[]
            {
                // 0
                new VPOS(0.0f, 0.0f, 0.0f),                           // 0
                new VPOS(0.0f + filet, 0.0f + filet, 0.0f), // B      // 1
                new VPOS(0.0f + filet, 0.0f, 0.0f + filet), // F      // 2
                new VPOS(0.0f, 0.0f + filet, 0.0f + filet), // L      // 3
                // 1
                new VPOS(length, 0.0f, 0.0f),                         // 4
                new VPOS(length - filet, 0.0f + filet, 0.0f), // B    // 5    
                new VPOS(length - filet, 0.0f, 0.0f + filet), // F    // 6
                new VPOS(length, 0.0f + filet, 0.0f + filet), // R    // 7
                // 2
                new VPOS(length, width, 0.0f),                        // 8
                new VPOS(length - filet, width - filet, 0.0f), // B   // 9
                new VPOS(length - filet, width, 0.0f + filet), // K   // 10
                new VPOS(length, width - filet, 0.0f + filet), // R   // 11
                // 3
                new VPOS(0.0f, width, 0.0f),                          // 12
                new VPOS(0.0f + filet, width - filet, 0.0f), // B     // 13
                new VPOS(0.0f + filet, width, 0.0f + filet), // K     // 14
                new VPOS(0.0f, width - filet, 0.0f + filet), // L     // 15
                // 4
                new VPOS(0.0f, 0.0f, height),                         // 16
                new VPOS(0.0f + filet, 0.0f + filet, height), // T    // 17
                new VPOS(0.0f + filet, 0.0f, height - filet), // F    // 18    
                new VPOS(0.0f, 0.0f + filet, height - filet), // L    // 19
                // 5
                new VPOS(length, 0.0f, height),                       // 20
                new VPOS(length - filet, 0.0f + filet, height), // T  // 21
                new VPOS(length - filet, 0.0f, height - filet), // F  // 22
                new VPOS(length, 0.0f + filet, height - filet), // R  // 23
                // 6
                new VPOS(length, width, height),                      // 24
                new VPOS(length - filet, width - filet, height), // T // 25
                new VPOS(length - filet, width, height - filet), // K // 26
                new VPOS(length, width - filet, height - filet), // R // 27    
                // 7
                new VPOS(0.0f, width, height),                        // 28
                new VPOS(0.0f + filet, width - filet, height), // T   // 29
                new VPOS(0.0f + filet, width, height - filet), // K   // 30
                new VPOS(0.0f, width - filet, height - filet)  // L   // 31
            };

            VPOS[] tVertices = new VPOS[32];
            int index = 0;
            foreach (var v in vertices)
            {
                v.ApplyTransform(BoxPositionToMatrix4x4(boxPosition));
                tVertices[index++] = v;
            }

            int[,] qFace = new int[,]
            {
                { 15,  3, 19, 31 }, // left
                {  7, 11, 27, 23 }, // right
                {  2,  6, 22, 18 }, // front
                { 10, 14, 30, 26 }, // rear
                { 1,  13,  9,  5 }, // bottom
                { 17, 21, 25, 29 }  // top
            };
            int[,] qFaceFilet = new int[,]
            {
                {  1,  5,  6,  2 },
                {  5,  9, 11,  7 },
                {  9, 13, 14, 10 },
                { 13,  1,  3, 15 },
                { 18, 22, 21, 17 },
                { 23, 27, 25, 21 },
                { 26, 30, 29, 25 },
                { 31, 19, 17, 29 },
                {  3,  2, 18, 19 },
                {  6,  7, 23, 22 },
                { 11, 10, 26, 27 },
                { 14, 15, 31, 30 }
            };
            int[,] edges = new int[,]
            {
                { 0, 4 },
                { 4, 8 },
                { 8, 12 },
                { 12, 0 },
                { 16, 20 },
                { 20, 24 },
                { 24, 28 },
                { 28, 16 },
                { 0, 16 },
                { 4, 20 },
                { 8, 24 },
                { 12, 28 }
            };
            for (int i = 0; i < 6; ++i)
            {
                primFaces.AddQuadrangle(tVertices[qFace[i, 0]], tVertices[qFace[i, 1]], tVertices[qFace[i, 2]], tVertices[qFace[i, 3]]);
            }
            if (filet > 0)
            {
                for (int i = 0; i < 12; ++i)
                    primFaces.AddQuadrangle(
                        tVertices[qFaceFilet[i, 0]],
                        tVertices[qFaceFilet[i, 1]],
                        tVertices[qFaceFilet[i, 2]],
                        tVertices[qFaceFilet[i, 3]]
                        );
            }
            else
            {
                for (int i = 0; i < 12; ++i)
                    primEdges.AddLine(tVertices[edges[i, 0]], tVertices[edges[i, 1]]);
            }
        }
        private List<MeshBuilder<VPOS/*, VTEX*/>> BuildInterlayerMeshes(AnalysisLayered analysis)
        {
            var lMesh =  new List<MeshBuilder<VPOS/*, VTEX*/>>();
            foreach (var interlayer in analysis.Interlayers)
            {
                lMesh.Add(BuildCaseMesh(
                    $"{interlayer.Name}",
                    (float)interlayer.Length, (float)interlayer.Width, (float)interlayer.Thickness,
                    Enumerable.Repeat(interlayer.Color, 6).ToArray(),
                    0.0f, Color.Black, 0.0f, Color.Beige));
            }
            return lMesh;
        }

        private MeshBuilder<VPOS/*, VTEX*/> BuildPalletCapMesh(
            float length, float width, float height,
            float innerLength, float innerWidth, float innerHeight,
            Color color)
        {
            VPOS[] vertices = new VPOS[]
            {
                new VPOS(0.0f, 0.0f, 0.0f),
                new VPOS(length, 0.0f, 0.0f),
                new VPOS(length, width, 0.0f),
                new VPOS(0.0f, width, 0.0f),
                new VPOS(0.0f, 0.0f, height),
                new VPOS(length, 0.0f, height),
                new VPOS(length, width, height),
                new VPOS(0.0f, width, height)
            };

            int[,] faces =
            {
                { 3,0,4,7 },    // left
                { 1,2,6,5 },    // Right 
                { 0,1,5,4 },    // Front
                { 2,3,7,6 },    // Rear
                { 4,5,6,7 }     // Top
            };
            int[,] edges =
            {
                { 0, 1 }, // bottom
                { 1, 2 },
                { 2, 3 },
                { 3, 0 },
                { 4, 5 }, // top
                { 5, 6 },
                { 6, 7 },
                { 7, 4 },
                { 0, 4 }, // side edges
                { 1, 5 },
                { 2, 6 },
                { 3, 7 }
            };
            // mesh
            var mesh = new MeshBuilder<VPOS/*, VTEX*/>("PalletCap");
            // faces
            var materialColor = new MaterialBuilder()
                .WithDoubleSide(true)
                .WithMetallicRoughnessShader()
                .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(color));
            var primFaces = mesh.UsePrimitive(materialColor);
            for (int i = 0; i < 5; ++i)
                primFaces.AddQuadrangle(vertices[faces[i, 0]], vertices[faces[i, 1]], vertices[faces[i, 2]], vertices[faces[i, 3]]);
            // edges
            var materialBlack = new MaterialBuilder()
                    .WithUnlitShader()
                    .WithChannelParam(KnownChannel.BaseColor, ColorToVector4(Color.Black));
            var primEdges = mesh.UsePrimitive(materialBlack, 2);
            for (int i = 0; i < 12; ++i)
                primEdges.AddLine(vertices[edges[i, 0]], vertices[edges[i, 1]]);
            return mesh;
        }

        static Vector4 ColorToVector4(Color color) => new Vector4(color.R / 255.0F, color.G / 255.0F, color.B / 255.0F, 1.0F);
        #endregion
    }
    #endregion
}
