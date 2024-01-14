using UnityEditor;
using UnityEngine;

public class SDFGenerateWindow : EditorWindow
{
    private Texture2D[] mTexture2Ds = new Texture2D[9];
    private Texture2D mTexB;
    private int nSpread = 6;
    private int nDownSample = 0;
    private Material mCombineMaterial;

    [MenuItem("Tools/SDFGenerate")]
    public static void ShowWindow()
    {
        SDFGenerateWindow sDFGenerateWindow = GetWindow<SDFGenerateWindow>();
        sDFGenerateWindow.Show();
    }

    private void OnGUI()
    {
        for (int i = 0; i < mTexture2Ds.Length; ++i)
        {
            mTexture2Ds[i] = EditorGUILayout.ObjectField(i.ToString(), mTexture2Ds[i], typeof(Texture2D), false) as Texture2D;
        }

        mTexB = EditorGUILayout.ObjectField("B Channel", mTexB, typeof(Texture2D), false) as Texture2D;

        //EditorGUILayout.ObjectField(mCombineMaterial, typeof(Material), false);
        nDownSample = EditorGUILayout.IntSlider("½µµÍ³ß´ç¼¶±ð", nDownSample, 0, 4);
        nSpread = EditorGUILayout.IntSlider($"Spread = {1 << nSpread}", nSpread, 0, 16);

        if (GUILayout.Button("Generate"))
        {
            string path = EditorUtility.SaveFilePanel("Save", UnityEngine.Application.dataPath, "SDFMap", "png");

            if(string.IsNullOrEmpty(path)) { return; }

            Shader sdfShader = Shader.Find("SDF/Generate");
            Shader combineShader = Shader.Find("SDF/Combine");
            Shader combineFaceShader = Shader.Find("SDF/CombineFaceSDF");

            Material sdfMat = new Material(sdfShader);
            sdfMat.SetFloat("_range", 1 << nSpread);
            Material combineMat = new Material(combineShader);
            Material combineFaceMat = new Material(combineFaceShader);

            SDFGenerate.GenerateFaceMap9(mTexture2Ds, mTexB, nDownSample, sdfMat, combineMat, combineFaceMat, path);
            Object.DestroyImmediate(sdfMat);
            Object.DestroyImmediate (combineMat);
            Object.DestroyImmediate(combineFaceMat);
            mCombineMaterial = combineMat;
        }
    }
}
