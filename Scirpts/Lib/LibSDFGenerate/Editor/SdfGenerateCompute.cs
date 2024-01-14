using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SdfGenerateCompute))]
public class SdfGenerateComputeInsp : Editor
{
    public override void OnInspectorGUI() {
        base.DrawDefaultInspector();

        var sdfGenerate = target as SdfGenerateCompute;
        if (GUILayout.Button("gen")) {
            sdfGenerate.gen();

        }
    }
}
public class SdfGenerateCompute : MonoBehaviour
{
    public RenderTexture rt;
    public Texture texture;
    public ComputeShader shader;
    public int spread = 16;

    public bool save = true;
    public string savePath;

    public Vector2 size = new Vector2(1024, 1024);


    public void gen() {
        var sdfGenerate = this;
        if (sdfGenerate.texture == null) {
            Debug.LogErrorFormat("texture is null ");
            return;
        }

        if (sdfGenerate.shader == null) {
            Debug.LogErrorFormat("shader is null ");
            return;
        }        

        if (sdfGenerate.rt != null) {
            sdfGenerate.rt.Release();
            sdfGenerate.rt = null;
        }
        
        sdfGenerate.rt = new RenderTexture((int)sdfGenerate.size.x, (int)sdfGenerate.size.y, 32, RenderTextureFormat.ARGB32);

        var input_rt = RenderTexture.GetTemporary(new RenderTextureDescriptor(sdfGenerate.rt.width, sdfGenerate.rt.height, sdfGenerate.rt.format));

        Graphics.Blit(texture, input_rt);

        RenderTexture t = sdfGenerate.rt;
        t.enableRandomWrite = true;
        t.Create();

        var computeShader = shader;

        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(kernel, "_MainTex", input_rt);
        computeShader.SetTexture(kernel, "outputTexture", t);
        computeShader.SetInt("_halfRange", spread/2);
        computeShader.SetVector("_MainTex_Size", new Vector4(t.width, t.height, 0, 0));


        computeShader.Dispatch(kernel, t.width / 8, t.height / 8, 1);

        RenderTexture.ReleaseTemporary(input_rt);

        if (save) {
            //SdfGenerate.savePng(this.rt, this.savePath);
        }
    }
}