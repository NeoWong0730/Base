using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ModifyTexture : MonoBehaviour, ITextureCombine
{
    public static readonly int gPositionMirrorID = Shader.PropertyToID("PositionMirror");
    public static readonly int gRSMatrixID = Shader.PropertyToID("RSMatrix");
    public static readonly int gBaseColorID = Shader.PropertyToID("_BaseColor");
    public static readonly int gMainTextureID = Shader.PropertyToID("_MainTex");

    public enum ETextureSize : int
    {
        _2 = 2,
        _4 = 4,
        _8 = 8,
        _16 = 16,
        _32 = 32,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
    }
    
    public Material mTargetMaterial;
    [NonSerialized]
    public Texture2D mDestinationTex;
    public string sTexturePropertyName = "_BaseMap";
    public ETextureSize nDestSize = ETextureSize._256;
    public ETextureSize nResolutionSize = ETextureSize._1024;

    public Material mBaseMaterial;
    public Texture2D mBaseTexture;
    public Color mBaseColor = Color.white;

    [HideInInspector] public DetailAsset[] detailDatas;
    [HideInInspector] public DetailModifyValue[] detailModifyValues;
    [HideInInspector] public DetailAreaAsset detailAreaAsset;

    public bool useCompress = false;

    private bool isDirty = false;

    private void Reset()
    {
        Clear();
    }

    private void OnDestroy()
    {
        Clear();
    }

    [ContextMenu("SetDirty")]
    public void SetDirty()
    {
        if (isDirty)
            return;

        isDirty = true;
        global::TextureCombine.Add(this);
    }

    private void Clear()
    {
        if (mDestinationTex)
        {
            Destroy(mDestinationTex);
        }
    }

    public void OnExecute(ScriptableRenderContext context, CommandBuffer cmd, TextureCombinePass textureCombinePass)
    {
        if (!isDirty)
            return;

        isDirty = false;

        if (!mTargetMaterial)
            return;

        int texturePropertyID = Shader.PropertyToID(sTexturePropertyName);
        if (!mTargetMaterial.HasProperty(texturePropertyID))
            return;

        RenderTargetHandle temporaryRT = new RenderTargetHandle();
        temporaryRT.Init("_TextureCombineRT");

        cmd.GetTemporaryRT(temporaryRT.id, (int)nDestSize, (int)nDestSize, 0, FilterMode.Trilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 1);

        //先覆盖基础贴图纹理
        cmd.SetRenderTarget(temporaryRT.Identifier());

        if (mBaseMaterial)
        {
            MaterialPropertyBlock baseMaterialPropertyBlock = new MaterialPropertyBlock();
            baseMaterialPropertyBlock.SetColor(gBaseColorID, mBaseColor);
            if (mBaseTexture)
            {
                baseMaterialPropertyBlock.SetTexture(gMainTextureID, mBaseTexture);
            }

            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, mBaseMaterial, 0, 1, baseMaterialPropertyBlock);
        }

        if (detailAreaAsset != null && detailAreaAsset.mDetailAreas != null)
        {
            //再叠加细节
            for (int i = 0; i < detailDatas.Length; ++i)
            {
                DetailAsset detailAsset = detailDatas[i];
                if (detailAsset == null)
                    continue;

                Material material = detailAsset.mMaterial;
                if (!material)
                    continue;

                if (detailAreaAsset.mDetailAreas.Length < detailAsset.AreaID)
                    continue;

                DetailModifyValue modifyValue = detailModifyValues[i];
                DetailArea detailArea = detailAreaAsset.mDetailAreas[detailAsset.AreaID];

                MaterialPropertyBlock materialPropertyBlock = null;
                if (detailAsset.useColor)
                {
                    materialPropertyBlock = new MaterialPropertyBlock();
                    materialPropertyBlock.SetColor(gBaseColorID, modifyValue.vColor);
                }

                //materialPropertyBlock.SetTexture(gMainTextureID, data.mTexture);                                
                float x = modifyValue.fX < 0.5f ?
                    math.lerp(detailArea.xRange.x, detailArea.xRange.y, modifyValue.fX * 2) :
                    math.lerp(detailArea.xRange.y, detailArea.xRange.z, modifyValue.fX * 2 - 1);

                float y = modifyValue.fY < 0.5f ?
                    math.lerp(detailArea.yRange.x, detailArea.yRange.y, modifyValue.fY * 2) :
                    math.lerp(detailArea.yRange.y, detailArea.yRange.z, modifyValue.fY * 2 - 1);

                float2 pos = new float2(x, y);
                float rotate = modifyValue.fRotate < 0.5 ?
                    math.lerp(detailArea.fRotateRange.x, detailArea.fRotateRange.y, modifyValue.fRotate * 2) :
                    math.lerp(detailArea.fRotateRange.y, detailArea.fRotateRange.z, modifyValue.fRotate * 2 - 1);

                float s = modifyValue.fScale < 0.5 ?
                    math.lerp(detailArea.fScaleRange.x, detailArea.fScaleRange.y, modifyValue.fScale * 2) :
                    math.lerp(detailArea.fScaleRange.y, detailArea.fScaleRange.z, modifyValue.fScale * 2 - 1);

                float3 scale = new float3(detailAsset.vTextureSize / (float)nResolutionSize * s, 1);

                bool2 mirror = detailArea.vMirror;

                //float2x2 rs = math.inverse(math.mul(float2x2.Rotate(rotate), float2x2.Scale(scale.xy)));

                Quaternion q0 = Quaternion.AngleAxis(rotate, Vector3.forward);
                if (!(mirror.x || mirror.y))
                {
                    Matrix4x4 mat = Matrix4x4.TRS(new Vector3(pos.x, pos.y, 0), q0, scale);
                    cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat, material, 0, 1, materialPropertyBlock);
                }
                else
                {
                    Quaternion q1 = Quaternion.AngleAxis(-rotate, Vector3.forward);

                    if (mirror.x && mirror.y)
                    {
                        float3 posV3 = new float3(pos + 0.5f, 0);

                        float3 pos0 = posV3;
                        float3 pos1 = posV3 * new float3(1, -1, 0);
                        float3 pos2 = posV3 * new float3(-1, 1, 0);
                        float3 pos3 = posV3 * new float3(-1, -1, 0);

                        Matrix4x4 mat0 = Matrix4x4.TRS(pos0, q0, new Vector3(scale.x, scale.y, 1));
                        Matrix4x4 mat1 = Matrix4x4.TRS(pos1, q1, new Vector3(scale.x, -scale.y, 1));
                        Matrix4x4 mat2 = Matrix4x4.TRS(pos2, q1, new Vector3(-scale.x, scale.y, 1));
                        Matrix4x4 mat3 = Matrix4x4.TRS(pos2, q0, new Vector3(-scale.x, -scale.y, 1));

                        //cmd.DrawMeshInstanced(RenderingUtils.fullscreenMesh, 0, material, 1, new Matrix4x4[] { mat0, mat1, mat2, mat3 }, 4, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat0, material, 0, 1, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat1, material, 0, 1, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat2, material, 0, 1, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat3, material, 0, 1, materialPropertyBlock);
                    }
                    else if (mirror.x)
                    {
                        float3 posV3 = new float3(pos.x + 0.5f, pos.y, 0);
                        float3 pos0 = posV3;
                        float3 pos1 = posV3 * new float3(-1, 1, 0);

                        Matrix4x4 mat0 = Matrix4x4.TRS(pos0, q0, new Vector3(scale.x, scale.y, 1));
                        Matrix4x4 mat1 = Matrix4x4.TRS(pos1, q1, new Vector3(-scale.x, scale.y, 1));

                        //cmd.DrawMeshInstanced(RenderingUtils.fullscreenMesh, 0, material, 1, new Matrix4x4[] { mat0, mat1}, 2, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat0, material, 0, 1, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat1, material, 0, 1, materialPropertyBlock);
                    }
                    else if (mirror.y)
                    {
                        float3 posV3 = new float3(pos.x, pos.y + 0.5f, 0);
                        float3 pos0 = posV3;
                        float3 pos1 = posV3 * new float3(1, -1, 0);

                        Matrix4x4 mat0 = Matrix4x4.TRS(pos0, q0, new Vector3(scale.x, scale.y, 1));
                        Matrix4x4 mat1 = Matrix4x4.TRS(pos1, q1, new Vector3(scale.x, -scale.y, 1));

                        //cmd.DrawMeshInstanced(RenderingUtils.fullscreenMesh, 0, material, 1, new Matrix4x4[] { mat0, mat1 }, 2, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat0, material, 0, 1, materialPropertyBlock);
                        cmd.DrawMesh(RenderingUtils.fullscreenMesh, mat1, material, 0, 1, materialPropertyBlock);
                    }
                }
            }
        }
        //合成的纹理已经是线性空间纹理了
        //不需要标记 sRGB 

        int copySrcSize = (int)nDestSize;
        GraphicsFormat format = GraphicsFormat.R8G8B8A8_UNorm;

        //压缩
        if (useCompress && textureCombinePass.m_ComputeShader)
        {
            ComputeShader computeShader = textureCombinePass.m_ComputeShader;

#if UNITY_ANDROID && !UNITY_EDITOR
                format = GraphicsFormat.RGBA_ETC2_UNorm;
                computeShader.DisableKeyword("_COMPRESS_BC3");
                computeShader.EnableKeyword("_COMPRESS_ETC2");
#else
            format = GraphicsFormat.RGBA_DXT5_UNorm;
            computeShader.DisableKeyword("_COMPRESS_ETC2");
            computeShader.EnableKeyword("_COMPRESS_BC3");
#endif

            copySrcSize = (int)nDestSize >> 2;

            RenderTargetHandle temporaryCompressRT = default;
            temporaryCompressRT = new RenderTargetHandle();
            temporaryCompressRT.Init("_CompressRT");

            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(copySrcSize, copySrcSize)
            {
                depthBufferBits = 0,
                depthStencilFormat = GraphicsFormat.None,
                graphicsFormat = GraphicsFormat.R32G32B32A32_UInt,
                enableRandomWrite = true,
            };
            cmd.GetTemporaryRT(temporaryCompressRT.id, descriptor);

            int kernelHandle = computeShader.FindKernel("CSMain");
            cmd.SetComputeTextureParam(computeShader, kernelHandle, "RenderTexture0", temporaryRT.Identifier());
            cmd.SetComputeTextureParam(computeShader, kernelHandle, "Result", temporaryCompressRT.Identifier());
            cmd.SetComputeIntParams(computeShader, "DestRect", new int[] { 0, 0, (int)nDestSize, (int)nDestSize });
            cmd.DispatchCompute(computeShader, kernelHandle, (copySrcSize + 7) / 8, (copySrcSize + 7) / 8, 1);

            cmd.ReleaseTemporaryRT(temporaryRT.id);

            temporaryRT = temporaryCompressRT;
        }

        if (mDestinationTex)
        {
            if (!(mDestinationTex.width == (int)nDestSize
                && mDestinationTex.height == (int)nDestSize
                && mDestinationTex.graphicsFormat == format))
            {
                DestroyImmediate(mDestinationTex);
                mDestinationTex = new Texture2D((int)nDestSize, (int)nDestSize, format, TextureCreationFlags.None);
            }
        }
        else
        {
            mDestinationTex = new Texture2D((int)nDestSize, (int)nDestSize, format, TextureCreationFlags.None);
        }

        cmd.CopyTexture(temporaryRT.Identifier(), 0, 0, 0, 0, copySrcSize, copySrcSize, mDestinationTex, 0, 0, 0, 0);
        cmd.ReleaseTemporaryRT(temporaryRT.id);

        if (mTargetMaterial)
        {
            mTargetMaterial.SetTexture(texturePropertyID, mDestinationTex);
        }
    }

    [ContextMenu("SaveTexture")]
    public void SaveTexture()
    {
        //RenderTexture renderTexture = RenderTexture.GetTemporary(mDestinationTex.width, mDestinationTex.height, 0, mDestinationTex.graphicsFormat);
        //Graphics.CopyTexture(mDestinationTex, renderTexture);
        //RenderTexture.active = renderTexture;
        //mDestinationTex.ReadPixels(new Rect(0, 0, mDestinationTex.width, mDestinationTex.height), 0, 0);
        //mDestinationTex.Compress(false);
        //System.IO.File.WriteAllBytes($"{Application.dataPath}/x.png", mDestinationTex.EncodeToPNG());
    }
}
