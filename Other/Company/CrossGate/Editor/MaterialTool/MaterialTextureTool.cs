using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using System.IO;

public class MaterialTextureTool : EditorWindow
{
    private Texture2D mIDTexture;
    private Texture2D mTexture;

    private NativeArray<Color32> mIDDatas;
    private NativeArray<byte> mDatas;

    private int channelCount = 0;
    private Vector2 pos = Vector2.zero;

    private bool autoParse = false;
    private int idCount = 0;

    [System.Serializable]
    public struct MaterialArea
    {
        public Color32 IDMask;
        public Color32 MaterialValue;
    }

    //private NativeArray<MaterialArea> MaterialValue;
    private MaterialTextureSetting materialTextureSetting;

    [MenuItem("__Tools__/MaterialTextureTool")]
    public static void Create()
    {
        MaterialTextureTool window = EditorWindow.CreateWindow<MaterialTextureTool>();
        window.Show();
    }

    private void OnDisable()
    {
        //if (MaterialValue.IsCreated)
        //{
        //    MaterialValue.Dispose();
        //}
    }

    private void OnGUI()
    {
        bool idInitDirty = false;

        Texture2D newIDTexture = (Texture2D)EditorGUILayout.ObjectField("IDTexture", mIDTexture, typeof(Texture2D), false);
        if (newIDTexture != mIDTexture)
        {
            mIDTexture = newIDTexture;
            idInitDirty = true;
        }

        Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField("Texture", mTexture, typeof(Texture2D), false);
        if (newTexture != mTexture)
        {
            mTexture = newTexture;
            idInitDirty = true;
        }

        if (mIDTexture == null || mTexture == null)
            return;

        if (mIDTexture.texelSize != mTexture.texelSize)
        {
            EditorGUILayout.LabelField("ID图和材质图尺寸不一致");
            return;
        }

        if (idInitDirty)
        {
            MyAssetProcessor.stopTextureImporter = true;

            TextureImporter textureImporter = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(mIDTexture)) as TextureImporter;
            textureImporter.isReadable = true;
            textureImporter.sRGBTexture = true;
            textureImporter.streamingMipmaps = false;
            textureImporter.mipmapEnabled = false;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.alphaSource = TextureImporterAlphaSource.None;
            TextureImporterPlatformSettings textureImporterPlatformSettings = textureImporter.GetDefaultPlatformTextureSettings();
            textureImporterPlatformSettings.format = TextureImporterFormat.RGBA32;
            textureImporter.SetPlatformTextureSettings(textureImporterPlatformSettings);
            textureImporter.SaveAndReimport();

            TextureImporter textureImporter2 = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(mTexture)) as TextureImporter;
            textureImporter2.isReadable = true;
            textureImporter2.sRGBTexture = false;
            textureImporter2.streamingMipmaps = false;
            textureImporter2.mipmapEnabled = false;
            TextureImporterPlatformSettings textureImporterPlatformSettings2 = textureImporter2.GetDefaultPlatformTextureSettings();
            textureImporterPlatformSettings2.format = TextureImporterFormat.RGBA32;
            textureImporter2.SetPlatformTextureSettings(textureImporterPlatformSettings2);
            textureImporter2.SaveAndReimport();

            mIDDatas = mIDTexture.GetRawTextureData<Color32>();
            mDatas = mTexture.GetRawTextureData<byte>();

            MyAssetProcessor.stopTextureImporter = false;

            channelCount = mDatas.Length / (mTexture.width * mTexture.height);

            if (autoParse)
            {
                List<MaterialArea> materialAreas = new List<MaterialArea>();
                EditorUtility.DisplayProgressBar("分析ID纹理", "请稍后", 0.5f);

                HashSet<Color32> ids = new HashSet<Color32>();
                int mIDDatasLength = mIDDatas.Length;
                for (int i = 0; i < mIDDatasLength; ++i)
                {
                    Color32 color = mIDDatas[i];
                    color.a = byte.MaxValue;

                    if (!ids.Contains(color))
                    {
                        ids.Add(color);
                        MaterialArea area = new MaterialArea();
                        area.IDMask = color;
                        Color32 color32 = new Color32();
                        for (int j = 0; j < channelCount; ++j)
                        {
                            color32[j] = mDatas[i * channelCount + j];
                        }

                        area.MaterialValue = color32;
                        materialAreas.Add(area);
                    }
                }
                
                //MaterialValue = new NativeArray<MaterialArea>(materialAreas.ToArray(), Allocator.Persistent);
                EditorUtility.ClearProgressBar();
            }         
            else
            {
                materialTextureSetting = null;

                string path = AssetDatabase.GetAssetPath(mIDTexture);
                path = Path.ChangeExtension(path, "asset");
                //string fullPath = Application.dataPath + "/" + path;
                materialTextureSetting = AssetDatabase.LoadAssetAtPath<MaterialTextureSetting>(path);
                if(materialTextureSetting == null)
                {
                    materialTextureSetting = new MaterialTextureSetting();
                    AssetDatabase.CreateAsset(materialTextureSetting, path);
                }
                idCount = materialTextureSetting.MaterialValue.Count;
            }
        }

        bool isDirty = false;
        if (!autoParse)
        {
            EditorGUILayout.BeginHorizontal();
            idCount = EditorGUILayout.IntField("ID 数量", idCount);
            if (GUILayout.Button("设置"))
            {
                if(materialTextureSetting.MaterialValue.Count > idCount)
                {
                    materialTextureSetting.MaterialValue.RemoveRange(idCount, materialTextureSetting.MaterialValue.Count - idCount);
                }
                else if(materialTextureSetting.MaterialValue.Count < idCount)
                {
                    for(int i = materialTextureSetting.MaterialValue.Count; i < idCount; ++i)
                    {
                        materialTextureSetting.MaterialValue.Add(new MaterialArea());
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        pos = EditorGUILayout.BeginScrollView(pos);
        for (int i = 0; i < materialTextureSetting.MaterialValue.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            MaterialArea v = materialTextureSetting.MaterialValue[i];
            if (!autoParse)
            {
                Color32 IDColor = EditorGUILayout.ColorField("IDColor", v.IDMask);
                if (!v.IDMask.Equals(IDColor))
                {
                    v.IDMask = IDColor;
                    isDirty = true;
                }
            }
            else
            {
                EditorGUILayout.ColorField("IDColor", v.IDMask);
            }

            Color32 color = EditorGUILayout.ColorField("Metallic(R) Smooth(G)", v.MaterialValue);
            if (!v.MaterialValue.Equals(color))
            {
                v.MaterialValue = color;
                isDirty = true;
            }

            materialTextureSetting.MaterialValue[i] = v;

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (isDirty && materialTextureSetting.MaterialValue.Count > 0)
        {
            NativeArray<MaterialArea> MaterialValue = new NativeArray<MaterialArea>(materialTextureSetting.MaterialValue.ToArray(), Allocator.TempJob);

            MaterialCulcalatorJob job = new MaterialCulcalatorJob()
            {
                channelCount = channelCount,
                materialValue = MaterialValue,
                ids = mIDDatas,
                output = mDatas,
            };

            JobHandle jobHandle = job.Schedule<MaterialCulcalatorJob>(mIDDatas.Length, 32);
            jobHandle.Complete();
            MaterialValue.Dispose();
            //mTexture.LoadRawTextureData<byte>(mDatas);
            mTexture.Apply();
        }

        if (GUILayout.Button("Save"))
        {
            byte[] bytes = mTexture.EncodeToTGA();
            string outputfile1 = AssetDatabase.GetAssetPath(mTexture);
            File.WriteAllBytes(outputfile1, bytes);
        }
    }

    [BurstCompile]
    struct MaterialCulcalatorJob : IJobParallelFor
    {
        [ReadOnly] public int channelCount;
        [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<MaterialArea> materialValue;
        [ReadOnly] public NativeArray<Color32> ids;
        [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<byte> output;

        public void Execute(int index)
        {
            for (int i = 0; i < materialValue.Length; ++i)
            {
                Color32 mask = ids[index];
                Color32 id = materialValue[i].IDMask;

                if (mask.r == id.r && mask.g == id.g && mask.b == id.b)
                {
                    Color32 v = materialValue[i].MaterialValue;
                    output[index * channelCount] = v.r;
                    output[index * channelCount + 1] = v.g;
                    break;
                }
            }
        }
    }
}
