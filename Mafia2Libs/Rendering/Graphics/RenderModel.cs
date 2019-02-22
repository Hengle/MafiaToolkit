﻿using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Windows.Forms;
using Mafia2;
using System.Collections.Generic;
using ResourceTypes.FrameResource;

namespace Rendering.Graphics
{
    public class RenderModel
    {
        public struct ModelPart
        {
            public Material Material;
            public ulong MaterialHash;
            public ShaderResourceView Texture;
            public uint StartIndex;
            public uint NumFaces;
            public BaseShader Shader;
        }
        public ShaderResourceView AOTexture { get; set; }
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        public RenderBoundingBox BoundingBox { get; private set; }

        //new
        public struct LOD
        {
            public ModelPart[] ModelParts { get; set; }
            public VertexLayouts.NormalLayout.Vertex[] Vertices { get; set; }
            public ushort[] Indices { get; set; }
        }

        public LOD[] LODs { get; private set; }
        public Matrix Transform { get; private set; }
        public bool DoRender { get; set; }

        public RenderModel()
        {
            Transform = Matrix.Identity;
            BoundingBox = new RenderBoundingBox();
        }

        public bool Init(Device device, ShaderManager manager)
        {
            if (!InitBuffer(device))
            {
                MessageBox.Show("unable to init buffer");
                return false;
            }
            if (!InitializePartShaders(device, manager))
            {
                MessageBox.Show("unable to load texture");
                return false;
            }
            return true;
        }

        public void SetTransform(Vector3 position, Matrix33 rotation)
        {
            Matrix m_trans = Matrix.Identity;
            m_trans[0, 0] = rotation.M00;
            m_trans[0, 1] = rotation.M01;
            m_trans[0, 2] = rotation.M02;
            m_trans[1, 0] = rotation.M10;
            m_trans[1, 1] = rotation.M11;
            m_trans[1, 2] = rotation.M12;
            m_trans[2, 0] = rotation.M20;
            m_trans[2, 1] = rotation.M21;
            m_trans[2, 2] = rotation.M22;
            m_trans[3, 0] = position.X;
            m_trans[3, 1] = position.Y;
            m_trans[3, 2] = position.Z;
            Transform = m_trans;
        }

        public bool ConvertFrameToRenderModel(FrameObjectSingleMesh mesh, FrameGeometry geom, FrameMaterial mats, IndexBuffer[] indexBuffers, VertexBuffer[] vertexBuffers)
        {
            if (mesh == null || geom == null || mats == null || indexBuffers == null || vertexBuffers == null)
                return false;

            SetTransform(mesh.Matrix.Position, mesh.Matrix.Rotation);
            DoRender = true;
            BoundingBox = new RenderBoundingBox();
            BoundingBox.Init(mesh.Boundings);
            LODs = new LOD[geom.NumLods];

            for(int i = 0; i != geom.NumLods; i++)
            {
                LOD lod = new LOD();
                lod.Indices = indexBuffers[i].Data;
                lod.ModelParts = new ModelPart[mats.LodMatCount[i]];

                for (int z = 0; z != mats.Materials[i].Length; z++)
                {
                    lod.ModelParts[z] = new ModelPart();
                    lod.ModelParts[z].NumFaces = (uint)mats.Materials[i][z].NumFaces;
                    lod.ModelParts[z].StartIndex = (uint)mats.Materials[i][z].StartIndex;
                    lod.ModelParts[z].MaterialHash = mats.Materials[i][z].MaterialHash;
                    lod.ModelParts[z].Material = MaterialsManager.LookupMaterialByHash(lod.ModelParts[z].MaterialHash);
                }

                lod.Vertices = new VertexLayouts.NormalLayout.Vertex[geom.LOD[i].NumVertsPr];
                int vertexSize;
                Dictionary<VertexFlags, FrameLOD.VertexOffset> vertexOffsets = geom.LOD[i].GetVertexOffsets(out vertexSize);

                for (int x = 0; x != lod.Vertices.Length; x++)
                {
                    VertexLayouts.NormalLayout.Vertex vertex = new VertexLayouts.NormalLayout.Vertex();

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.Position))
                    {
                        int startIndex = x * vertexSize + vertexOffsets[VertexFlags.Position].Offset;
                        vertex.Position = VertexTranslator.ReadPositionDataFromVB(vertexBuffers[i].Data, startIndex, geom.DecompressionFactor, geom.DecompressionOffset);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.Tangent))
                    {
                        //int startIndex = x * vertexSize + vertexOffsets[VertexFlags.Position].Offset;
                       // vertex.Tangent = VertexTranslator.ReadTangentDataFromVB(vertexBuffers[i].Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.Normals))
                    {
                        int startIndex = x * vertexSize + vertexOffsets[VertexFlags.Normals].Offset;
                        vertex.Normal = VertexTranslator.ReadNormalDataFromVB(vertexBuffers[i].Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.BlendData))
                    {
                        //int startIndex = v * vertexSize + vertexOffsets[VertexFlags.BlendData].Offset;
                       // vertex.BlendWeight = VertexTranslator.ReadBlendWeightFromVB(vertexBuffer.Data, startIndex);
                       // vertex.BoneID = VertexTranslator.ReadBlendIDFromVB(vertexBuffer.Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.flag_0x80))
                    {
                        //unknown
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.TexCoords0))
                    {
                        int startIndex = x * vertexSize + vertexOffsets[VertexFlags.TexCoords0].Offset;
                        vertex.TexCoord0 = VertexTranslator.ReadTexcoordFromVB(vertexBuffers[i].Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.TexCoords1))
                    {
                        //int startIndex = v * vertexSize + vertexOffsets[VertexFlags.TexCoords1].Offset;
                        //vertex.UVs[1] = VertexTranslator.ReadTexcoordFromVB(vertexBuffer.Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.TexCoords2))
                    {
                        //int startIndex = v * vertexSize + vertexOffsets[VertexFlags.TexCoords2].Offset;
                        //vertex.UVs[2] = VertexTranslator.ReadTexcoordFromVB(vertexBuffer.Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.TexCoords7))
                    {
                        int startIndex = x * vertexSize + vertexOffsets[VertexFlags.TexCoords7].Offset;
                        vertex.TexCoord7 = VertexTranslator.ReadTexcoordFromVB(vertexBuffers[i].Data, startIndex);
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.flag_0x20000))
                    {
                        //unknown
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.flag_0x40000))
                    {
                        //unknown
                    }

                    if (geom.LOD[i].VertexDeclaration.HasFlag(VertexFlags.DamageGroup))
                    {
                        //int startIndex = v * vertexSize + vertexOffsets[VertexFlags.DamageGroup].Offset;
                        //vertex.DamageGroup = VertexTranslator.ReadDamageGroupFromVB(vertexBuffer.Data, startIndex);
                    }

                    lod.Vertices[x] = vertex;
                }
                LODs[i] = lod;
            }


            return true;
        }
        public void Shutdown()
        {
            ReleaseTextures();
            ShutdownBuffers();
        }
        private bool InitBuffer(Device device)
        {
            VertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, LODs[0].Vertices);
            IndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, LODs[0].Indices);

            BoundingBox.InitBuffer(device);
            return true;
        }
        private bool InitializePartShaders(Device device, ShaderManager manager)
        {
            TextureClass AOTextureClass = new TextureClass();
            ShaderResourceView m_Temp;
            bool result = true;

            RenderStorageSingleton.Instance.TextureCache.TryGetValue(0, out m_Temp);
            AOTexture = m_Temp;

            if (m_Temp == null)
            {
                AOTextureClass.Init(device, "texture.dds");
                RenderStorageSingleton.Instance.TextureCache.Add(0, AOTextureClass.TextureResource);
                AOTexture = AOTextureClass.TextureResource;
            }

            //m_Temp = null;

            for (int x = 0; x != LODs[0].ModelParts.Length; x++)
            {
                ModelPart part = LODs[0].ModelParts[x];
                if (part.Material == null)
                    part.Shader = manager.shaders[0];
                else
                    part.Shader = (manager.shaders.ContainsKey(LODs[0].ModelParts[x].Material.ShaderHash) ? manager.shaders[LODs[0].ModelParts[x].Material.ShaderHash] : manager.shaders[0]);
                LODs[0].ModelParts[x] = part;
            }

            BoundingBox.Shader = manager.shaders[1];
            return true;
        }
        private void ReleaseTextures()
        {
            for (int x = 0; x != LODs[0].ModelParts.Length; x++)
            {
                LODs[0].ModelParts[x].Texture?.Dispose();
                LODs[0].ModelParts[x].Texture = null;
            }
            AOTexture?.Dispose();
            AOTexture = null;
        }
        private void ReleaseModel()
        {
            LODs[0].Vertices = null;
            LODs[0].Indices = null;
            BoundingBox.ReleaseModel();
        }
        private void ShutdownBuffers()
        {
            BoundingBox.ShutdownBuffers();
            VertexBuffer?.Dispose();
            VertexBuffer = null;
            IndexBuffer?.Dispose();
            IndexBuffer = null;
        }

        public void Render(Device device, DeviceContext deviceContext, Camera camera, LightClass light)
        { 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<VertexLayouts.NormalLayout.Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R16_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            for (int i = 0; i != LODs[0].ModelParts.Length; i++)
            {
                LODs[0].ModelParts[i].Shader.SetShaderParamters(device, deviceContext, LODs[0].ModelParts[i].Material);
                LODs[0].ModelParts[i].Shader.SetSceneVariables(deviceContext, Transform, camera, light);
                deviceContext.PixelShader.SetShaderResource(1, AOTexture);
                LODs[0].ModelParts[i].Shader.Render(deviceContext, LODs[0].ModelParts[i].NumFaces * 3, LODs[0].ModelParts[i].StartIndex);
            }

            BoundingBox.Render(device, deviceContext, Transform, camera, light);
        }
    }
}
