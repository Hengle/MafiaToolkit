﻿using Rendering.Core;
using ResourceTypes.Materials;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows;
using Utils.Logging;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace Rendering.Graphics
{
    public class ShaderInitParams
    {
        public struct ShaderFileEntryPoint
        {
            public string FilePath { get; set; }
            public string EntryPoint { get; set; }
            public string Target { get; set; }

            public ShaderFileEntryPoint(string InFilePath, string InEntryPoint, string InTarget)
            {
                FilePath = InFilePath;
                EntryPoint = InEntryPoint;
                Target = InTarget;
            }

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(EntryPoint) && !string.IsNullOrEmpty(Target);
            }
        }

        public ShaderInitParams() { }
        public ShaderInitParams(InputElementDescription[] InElements, ShaderFileEntryPoint InPSShader, ShaderFileEntryPoint InVSShader, ShaderFileEntryPoint InInstanceVSShader, ShaderFileEntryPoint InGSShader)
        {
            Elements = InElements;
            PixelShaderFile = InPSShader;
            VertexShaderFile = InVSShader;
            InstancedVertexShaderFile = InInstanceVSShader;
            GeometryShaderFile = InGSShader;
        }

        public InputElementDescription[] Elements { get; set; }
        public ShaderFileEntryPoint PixelShaderFile { get; set; }
        public ShaderFileEntryPoint VertexShaderFile { get; set; }
        public ShaderFileEntryPoint InstancedVertexShaderFile { get; set; }
        public ShaderFileEntryPoint GeometryShaderFile { get; set; }
    }

    public class BaseShader
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix4x4 world;
            public Matrix4x4 viewProjection;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DCameraBuffer
        {
            public Vector3 cameraPosition;
            public float padding;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct LightBuffer
        {
            public Vector4 ambientColor;
            public Vector4 diffuseColor;
            public Vector3 LightDirection;
            public float specularPower;
            public Vector4 specularColor;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct HighLightBuffer
        {
            public int instanceID;
            public uint s;//this is a placeholder, since shader buffers have to be the size of 16bytes*N
            public uint ss;
            public uint sss;
        }
        [StructLayout(LayoutKind.Sequential)]
        protected struct EditorParameterBuffer
        {
            public Vector3 selectionColour;
            public int renderMode;
        }

        public struct MaterialParameters
        {
            public MaterialParameters(IMaterial material, Vector3 vector)
            {
                MaterialData = material;
                SelectionColour = vector;
            }

            public IMaterial MaterialData { get; set; }
            public Vector3 SelectionColour { get; set; }
        }
        public ID3D11VertexShader OurVertexShader { get; set; }
        public ID3D11VertexShader OurInstanceVertexShader { get; set; }
        public ID3D11PixelShader OurPixelShader { get; set; }
        public ID3D11GeometryShader OurGeometryShader { get; set; }
        public ID3D11InputLayout Layout { get; set; }
        protected ID3D11Buffer ConstantMatrixBuffer { get; set; }
        protected ID3D11Buffer ConstantLightBuffer { get; set; }
        protected ID3D11Buffer ConstantCameraBuffer { get; set; }
        protected ID3D11Buffer ConstantHightlightBuffer { get; set; }
        protected ID3D11Buffer ConstantEditorParamsBuffer { get; set; }
        public ID3D11SamplerState SamplerState { get; set; }

        // These allow the editor to only make changes if the 
        // incoming changes are different.
        protected LightClass previousLighting = null;      
        protected Vector3 previousEditorParams;

        private const string ShaderPath = @"Shaders\";

        public BaseShader(ID3D11Device Dx11Device, ShaderInitParams InitParams)
        {
            if (!Init(Dx11Device, InitParams))
            {
                MessageBox.Show("Failed to construct shader!", "Toolkit");
            }
        }

        public virtual bool Init(ID3D11Device device, ShaderInitParams InitParams)
        {
            // Attempt to construct pixel shader
            if (InitParams.PixelShaderFile.IsValid())
            {
                Blob PixelBytecode = ConstructBytecode(InitParams.PixelShaderFile);
                OurPixelShader = device.CreatePixelShader(PixelBytecode);
                PixelBytecode.Dispose();
            }

            // Attempt to construct vertex shader
            if (InitParams.VertexShaderFile.IsValid())
            {
                Blob VertexBytecode = ConstructBytecode(InitParams.VertexShaderFile);
                OurVertexShader = device.CreateVertexShader(VertexBytecode);
                Layout = device.CreateInputLayout(InitParams.Elements, VertexBytecode);
                VertexBytecode.Dispose();
            }

            // Attempt to construct vertex shader
            if (InitParams.InstancedVertexShaderFile.IsValid())
            {
                Blob VertexBytecode = ConstructBytecode(InitParams.InstancedVertexShaderFile);
                OurInstanceVertexShader = device.CreateVertexShader(VertexBytecode);
                VertexBytecode.Dispose();
            }

            // Attempt to construct geometry shader
            if (InitParams.GeometryShaderFile.IsValid())
            {
                Blob GeometryBytecode = ConstructBytecode(InitParams.GeometryShaderFile);
                OurGeometryShader = device.CreateGeometryShader(GeometryBytecode);
                GeometryBytecode.Dispose();
            }

            SamplerDescription samplerDesc = new SamplerDescription()
            {
                Filter = Filter.Anisotropic,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLODBias = 0,
                MaxAnisotropy = 8,
                ComparisonFunction = ComparisonFunction.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinLOD = 0,
                MaxLOD = 0
            };

            SamplerState = device.CreateSamplerState(samplerDesc);

            ConstantCameraBuffer = ConstantBufferFactory.ConstructBuffer<DCameraBuffer>(device, "CameraBuffer");
            ConstantLightBuffer = ConstantBufferFactory.ConstructBuffer<LightBuffer>(device, "LightBuffer");
            ConstantMatrixBuffer = ConstantBufferFactory.ConstructBuffer<MatrixBuffer>(device, "MatrixBuffer");
            ConstantHightlightBuffer = ConstantBufferFactory.ConstructBuffer<HighLightBuffer>(device, "HighlightBuffer");
            ConstantEditorParamsBuffer = ConstantBufferFactory.ConstructBuffer<EditorParameterBuffer>(device, "EditorBuffer");

            return true;
        }

        public virtual void InitCBuffersFrame(ID3D11DeviceContext context, Camera camera, WorldSettings settings)
        {
            var cameraBuffer = new DCameraBuffer()
            {
                cameraPosition = camera.Position,
                padding = 0.0f
            };
            ConstantBufferFactory.UpdateVertexBuffer(context, ConstantCameraBuffer, 1, cameraBuffer);

            if (previousLighting == null || !previousLighting.Equals(settings.Lighting))
            {
                LightBuffer lightbuffer = new LightBuffer()
                {
                    ambientColor = settings.Lighting.AmbientColor,
                    diffuseColor = settings.Lighting.DiffuseColour,
                    LightDirection = settings.Lighting.Direction,
                    specularColor = settings.Lighting.SpecularColor,
                    specularPower = settings.Lighting.SpecularPower
                };
                previousLighting = settings.Lighting;
                ConstantBufferFactory.UpdatePixelBuffer(context, ConstantLightBuffer, 0, lightbuffer);
            }
        }

        public void setHightLightInstance(ID3D11DeviceContext context, int instanceID)
        {
            var hightlight = new HighLightBuffer()
            {
                instanceID = instanceID,
            };
            ConstantBufferFactory.UpdateVertexBuffer(context,ConstantHightlightBuffer,2,hightlight);
        }

        public virtual void SetSceneVariables(ID3D11DeviceContext context, Matrix4x4 WorldMatrix, Camera camera)
        {
            Matrix4x4 tMatrix = Matrix4x4.Transpose(WorldMatrix);

            MatrixBuffer matrixBuffer = new MatrixBuffer()
            {
                world = tMatrix,
                viewProjection = camera.ViewProjectionMatrixTransposed,
            };
            ConstantBufferFactory.UpdateVertexBuffer(context, ConstantMatrixBuffer, 0, matrixBuffer);
        }

        public virtual void SetShaderParameters(ID3D11Device device, ID3D11DeviceContext deviceContext, MaterialParameters matParams)
        {
            if (!previousEditorParams.Equals(matParams.SelectionColour))
            {
                var editorParams = new EditorParameterBuffer()
                { 
                    selectionColour = matParams.SelectionColour
                };

                ConstantBufferFactory.UpdatePixelBuffer(deviceContext, ConstantEditorParamsBuffer, 1, editorParams);
                previousEditorParams = editorParams.selectionColour;
            }

            //experiments with samplers; currently the toolkit doesn't not support any types.
            /*SamplerStateDescription samplerDesc = new SamplerStateDescription()
            {
                Filter = Filter.Anisotropic,
                AddressU = (material != null) ? (TextureAddressMode)material.Samplers["S000"].SamplerStates[0] : TextureAddressMode.Wrap,
                AddressV = (material != null) ? (TextureAddressMode)material.Samplers["S000"].SamplerStates[1] : TextureAddressMode.Wrap,
                AddressW = (material != null) ? (TextureAddressMode)material.Samplers["S000"].SamplerStates[2] : TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 16,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            SamplerState = new SamplerState(device, samplerDesc);*/
        }

        public void ResetShaderParameters(ID3D11Device device, ID3D11DeviceContext deviceContext)
        {
            var editorParams = new EditorParameterBuffer()
            {
                selectionColour = new Vector3(1,1,1)
            };

            ConstantBufferFactory.UpdatePixelBuffer(deviceContext, ConstantEditorParamsBuffer, 1, editorParams);
            previousEditorParams = editorParams.selectionColour;

            //experiments with samplers; currently the toolkit doesn't not support any types.
            /*SamplerStateDescription samplerDesc = new SamplerStateDescription()
            {
                Filter = Filter.Anisotropic,
                AddressU = (material != null) ? (TextureAddressMode)material.Samplers["S000"].SamplerStates[0] : TextureAddressMode.Wrap,
                AddressV = (material != null) ? (TextureAddressMode)material.Samplers["S000"].SamplerStates[1] : TextureAddressMode.Wrap,
                AddressW = (material != null) ? (TextureAddressMode)material.Samplers["S000"].SamplerStates[2] : TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 16,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            SamplerState = new SamplerState(device, samplerDesc);*/
        }

        public virtual void Render(ID3D11DeviceContext context, PrimitiveTopology type, int size, uint offset)
        {
            context.IASetInputLayout(Layout);

            // set shaders only if available
            if(OurVertexShader != null)
            {
                context.VSSetShader(OurVertexShader);
            }

            if (OurVertexShader != null)
            {
                context.PSSetShader(OurPixelShader);
                context.PSSetSampler(0, SamplerState);
            }

            if (OurVertexShader != null)
            {
                context.GSSetShader(OurGeometryShader);
            }

            context.DrawIndexed(size, (int)offset, 0); //Don't wanna see other meshes when testing instances

            Profiler.NumDrawCallsThisFrame++;
        }

        public virtual void RenderInstanced(ID3D11DeviceContext context, PrimitiveTopology type, int size, int offset, int count)
        {
            context.IASetInputLayout(Layout);

            // set shaders only if available
            if (OurInstanceVertexShader != null)
            {
                context.VSSetShader(OurInstanceVertexShader);
            }

            if (OurInstanceVertexShader != null)
            {
                context.PSSetShader(OurPixelShader);
                context.PSSetSampler(0, SamplerState);
            }

            if (OurInstanceVertexShader != null)
            {
                context.GSSetShader(OurGeometryShader);
            }

            context.DrawIndexedInstanced(size, count, offset, 0, 0);

            Profiler.NumDrawCallsThisFrame++;
        }

        public virtual void Shutdown() 
        {
            ConstantLightBuffer?.Dispose();
            ConstantLightBuffer = null;
            ConstantCameraBuffer?.Dispose();
            ConstantCameraBuffer = null;
            ConstantMatrixBuffer?.Dispose();
            ConstantMatrixBuffer = null;
            ConstantEditorParamsBuffer?.Dispose();
            ConstantEditorParamsBuffer = null;
            SamplerState?.Dispose();
            SamplerState = null;
            Layout?.Dispose();
            Layout = null;
            OurPixelShader?.Dispose();
            OurPixelShader = null;
            OurVertexShader?.Dispose();
            OurVertexShader = null;
            OurInstanceVertexShader?.Dispose();
            OurInstanceVertexShader = null;
            OurGeometryShader?.Dispose();
            OurGeometryShader = null;
        }

        public Blob ConstructBytecode(ShaderInitParams.ShaderFileEntryPoint ShaderFileData)
        {
            string Error;
            string ShaderFileName = ShaderPath + ShaderFileData.FilePath;

            Blob OurBytecode = null;
            Blob OurErrorcode = null;

            Compiler.CompileFromFile(ShaderFileName, ShaderFileData.EntryPoint, ShaderFileData.Target, out OurBytecode, out OurErrorcode);
            if(OurErrorcode != null)
            {
                Error = OurErrorcode.ConvertToString();
                Log.WriteLine(string.Format("DX11 Compiler Error: {0}", Error), LoggingTypes.ERROR, LogCategoryTypes.APPLICATION);
            }

            return OurBytecode;
        }
    }
}
