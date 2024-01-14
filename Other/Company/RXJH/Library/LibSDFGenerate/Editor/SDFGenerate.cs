using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class SDFGenerate
{
    public static void Generate(Texture inTex, int downSample, int spread, Material material, string outputFile)
    {
        if (string.IsNullOrEmpty(outputFile))
            return;

        int width = inTex.width >> downSample;
        int height = inTex.height >> downSample;

        material.SetFloat("_range", spread);
        var outRt = RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32));
        Graphics.Blit(inTex, outRt);

        Texture2D outTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = outRt;
        outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        outTex.Apply();
        RenderTexture.active = activeRT;
        RenderTexture.ReleaseTemporary(outRt);

        var directory = Path.GetDirectoryName(outputFile);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllBytes(outputFile, outTex.EncodeToPNG());

        Object.Destroy(outTex);
    }

    public static void GenerateFaceMap9(Texture[] inTexs, Texture inBTex, int downSample, Material sdfMaterial, Material combineMaterial, Material combineFaceMaterial, string outputFile)
    {
        if (string.IsNullOrEmpty(outputFile))
            return;

        int texCount = 9;
        if (inTexs.Length != texCount)
            return;

        int width = inTexs[0].width >> downSample;
        int height = inTexs[0].height >> downSample;

        RenderTextureDescriptor textureDescriptor = new RenderTextureDescriptor(width, height);
        textureDescriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
        textureDescriptor.depthBufferBits = 0;

        RenderTexture[] rts = new RenderTexture[texCount];
        for (int i = 0; i < texCount; ++i)
        {
            rts[i] = RenderTexture.GetTemporary(textureDescriptor);
        }
        for (int i = 0; i < texCount; ++i)
        {
            Graphics.Blit(inTexs[i], rts[i], sdfMaterial);
        }
        for (int i = 0; i < texCount; ++i)
        {
            combineMaterial.SetTexture($"_MainTex{i}", rts[i]);
        }

        RenderTexture outRt = RenderTexture.GetTemporary(textureDescriptor);
        Graphics.Blit(rts[0], outRt, combineMaterial);

        for (int i = 0; i < texCount; ++i)
        {
            RenderTexture.ReleaseTemporary(rts[i]);
        }

        RenderTexture outRt2 = RenderTexture.GetTemporary(textureDescriptor);
        combineFaceMaterial.SetTexture("_MainTex", outRt);
        combineFaceMaterial.SetTexture("_BTex", inBTex);
        Graphics.Blit(outRt, outRt2, combineFaceMaterial);
        RenderTexture.ReleaseTemporary(outRt);

        Texture2D outTex = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
        RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = outRt2;
        outTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        outTex.Apply();
        RenderTexture.active = activeRT;
        RenderTexture.ReleaseTemporary(outRt2);

        var directory = Path.GetDirectoryName(outputFile);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllBytes(outputFile, outTex.EncodeToPNG());

        Object.DestroyImmediate(outTex);
    }
}
