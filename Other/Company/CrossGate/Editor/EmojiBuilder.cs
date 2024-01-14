/*

	Description:Create the Atlas of emojis and its data texture.

	How to use?
	1)
		Put all emojies in Asset/Framework/Resource/Emoji/Input.
		Multi-frame emoji name format : Name_Index.png , Single frame emoji format: Name.png
	2)
		Excute EmojiText->Build Emoji from menu in Unity.
	3)
		It will outputs two textures and a txt in Emoji/Output.
		Drag emoji_tex to "Emoji Texture" and emoji_data to "Emoji Data" in UGUIEmoji material.
	4)
		Repair the value of "Emoji count of every line" base on emoji_tex.png.
	5)
		It will auto copys emoji.txt to Resources, and you can overwrite relevant functions base on your project.
	
	Author:zouchunyi
	E-mail:zouchunyi@kingsoft.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework;
using Unity.Mathematics;

public static class EmojiBuilder
{
    private static readonly string gDataPath = Application.dataPath;
    private const string gOutputPath = "Assets/ResourcesAB/Emoji/";
    private const string gInputPath = "Projects/Emoji/";

    struct EmojiInfoString
    {
        public string key;
        public string x;
        public string y;
    }

    public struct EmojiBuildSettings
    {
        public int emojiSize;
        public int emojiSpace;
        public Vector2Int emojiLayout;
    }

    private const int EmojiSize = 64;//the size of emoji.

    [MenuItem("EmojiText/Build Emoji")]
    public static void BuildEmoji()
    {
        int emojiSpace = 0;
        string inputFullPath = Path.Combine(Application.dataPath, gInputPath);
        string[] dirs = Directory.GetDirectories(inputFullPath);

        //收集所有的表情序列帧文件
        int totalFrames = 0;
        Dictionary<int, List<string>> sourceDic = new Dictionary<int, List<string>>();
        for (int i = 0; i < dirs.Length; ++i)
        {
            string dirPath = dirs[i];
            string dirName = Path.GetFileName(dirPath);
            //解析表情ID
            int index = dirName.IndexOf(" ");
            int id;
            if (index <= 0 || !int.TryParse(dirName.Substring(0, index), out id))
            {
                Debug.LogErrorFormat("{0} id信息没有解析成功", dirPath);
                continue;
            }

            string[] files = Directory.GetFiles(dirPath, "*.png");
            for (int j = 0; j < files.Length; ++j)
            {
                string filePath = files[j];
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                filePath = filePath.Replace('\\', '/');
                filePath = filePath.Remove(0, gDataPath.Length - 6);

                int frame = 0;
                if (!int.TryParse(fileName, out frame))
                {
                    continue;
                }

                TextureImporter sourceTextureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
                sourceTextureImporter.isReadable = true;
                sourceTextureImporter.filterMode = FilterMode.Point;
                sourceTextureImporter.mipmapEnabled = false;
                sourceTextureImporter.sRGBTexture = true;
                sourceTextureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                sourceTextureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                sourceTextureImporter.maxTextureSize = 64;
                sourceTextureImporter.SaveAndReimport();

                List<string> filePaths = null;
                if (!sourceDic.TryGetValue(id, out filePaths))
                {
                    filePaths = new List<string>();
                    sourceDic.Add(id, filePaths);
                }

                filePaths.Add(filePath);
                ++totalFrames;
            }
        }

        AssetDatabase.Refresh();

        if (!Directory.Exists(gOutputPath))
        {
            Directory.CreateDirectory(gOutputPath);
        }

        //Dictionary<int, EmojiInfo> emojiAssetDatas = new Dictionary<int, EmojiInfo>();

        int4 atlasAndLayoutSize = ComputeAtlasSize(totalFrames, EmojiSize, emojiSpace);
        int2 atlasSize = atlasAndLayoutSize.xy;
        int2 layoutSize = atlasAndLayoutSize.zw;
        int2 dataSize = ComputeDataSize(layoutSize);

        Texture2D atlasTex = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false);
        Texture2D dataTex = new Texture2D(dataSize.x, dataSize.y, TextureFormat.RGBA32, false);

        int currentX = 0;
        int currentY = 0;

        
        List<int> sortKeys = new List<int>(sourceDic.Count);
        foreach (var k in sourceDic.Keys)
        {
            int index = 0;
            while (index < sortKeys.Count)
            {
                if (k < sortKeys[index])
                {
                    break;                    
                }
                ++index;
            }
            sortKeys.Insert(index, k);
        }        

        List<EmojiInfo> emojiList = new List<EmojiInfo>(sourceDic.Count);
        foreach (var k in sortKeys)
        {
            List<string> filePsths = sourceDic[k];

            //string t = System.Convert.ToString(filePsths.Count - 1, 2);
            //至少会有1帧 这里记录的是1帧以外额外的帧数 理论上最多rgb=9帧 rgba=17帧
            int frameCount = filePsths.Count - 1;
            float r = (frameCount & 1) * 1f;
            float g = (frameCount & 2) * 1f;
            float b = (frameCount & 4) * 1f;
            float a = (frameCount & 8) * 1f;
            Color dataColor = new Color(r, g, b, a);

            EmojiInfo info = new EmojiInfo();
            info.key = (uint)k;
            info.x = (float)currentX / dataSize.x;
            info.y = (float)currentY / dataSize.y;
            info.size = (float)EmojiSize / atlasSize.x;
            emojiList.Add(info);

            for (int index = 0; index < filePsths.Count; ++index)
            {
                string path = filePsths[index];
                Texture2D sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                //sourceTexture.Resize(EmojiSize, EmojiSize);
                Color[] colors = sourceTexture.GetPixels(0);

                atlasTex.SetPixels(currentX * (EmojiSize + emojiSpace), currentY * (EmojiSize + emojiSpace), EmojiSize, EmojiSize, colors);
                dataTex.SetPixel(currentX, currentY, dataColor);

                ++currentX;
                if (currentX >= layoutSize.x)
                {
                    currentX = 0;
                    ++currentY;
                }
            }
        }

        byte[] bytes1 = atlasTex.EncodeToPNG();
        string outputfile1 = gOutputPath + "emoji_tex.png";
        File.WriteAllBytes(outputfile1, bytes1);

        byte[] bytes2 = dataTex.EncodeToPNG();
        string outputfile2 = gOutputPath + "emoji_data.png";
        File.WriteAllBytes(outputfile2, bytes2);        

        FormatTexture();
        AssetDatabase.Refresh();

        //Payne ========>

        Texture emojiTex = AssetDatabase.LoadAssetAtPath<Texture>(outputfile1);
        Texture emojiData = AssetDatabase.LoadAssetAtPath<Texture>(outputfile2);

        EmojiAsset emojiAsset = AssetDatabase.LoadAssetAtPath<EmojiAsset>(gOutputPath + "emoji.asset");
        if (emojiAsset == null)
        {
            emojiAsset = ScriptableObject.CreateInstance<EmojiAsset>();

            Shader shader = Shader.Find("UI/EmojiFont");
            Material mat = new Material(shader);
            emojiAsset.material = mat;

            AssetDatabase.CreateAsset(emojiAsset, gOutputPath + "emoji.asset");
            AssetDatabase.AddObjectToAsset(mat, gOutputPath + "emoji.asset");
        }

        emojiAsset.emojiList = emojiList;        

        emojiAsset.material.SetTexture("_EmojiTex", emojiTex);
        emojiAsset.material.SetTexture("_EmojiDataTex", emojiData);
        emojiAsset.material.SetFloat("_EmojiSize", layoutSize.x);

        EditorUtility.SetDirty(emojiAsset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //Payne ========<

        EditorUtility.DisplayDialog("Success", "Generate Emoji Successful!", "OK");
    }

    private static int4 ComputeAtlasSize(int count, int emojiSize, int space)
    {
        long total = count * emojiSize * emojiSize;

        int size = 32;
        while (size <= 2048)
        {
            int c = (size + space) / (emojiSize + space);
            if (c * c >= count)
            {
                return new int4(size, size, c, c);
            }
            size <<= 1;
        }

        return int4.zero;
    }

    private static int2 ComputeDataSize(int2 size)
    {
        int realSize = 2;
        while (realSize <= 2048)
        {
            if (realSize >= size.x && realSize >= size.y)
            {
                return new int2(realSize, realSize);
            }
            realSize <<= 1;
        }

        return int2.zero;
    }

    private static void FormatTexture()
    {
        TextureImporter emojiTex = AssetImporter.GetAtPath(gOutputPath + "emoji_tex.png") as TextureImporter;
        emojiTex.filterMode = FilterMode.Point;
        emojiTex.mipmapEnabled = false;
        emojiTex.sRGBTexture = true;
        emojiTex.alphaSource = TextureImporterAlphaSource.FromInput;
        emojiTex.textureCompression = TextureImporterCompression.Uncompressed;
        emojiTex.SaveAndReimport();

        TextureImporter emojiData = AssetImporter.GetAtPath(gOutputPath + "emoji_data.png") as TextureImporter;
        emojiData.filterMode = FilterMode.Point;
        emojiData.mipmapEnabled = false;
        emojiData.sRGBTexture = false;
        emojiData.alphaSource = TextureImporterAlphaSource.None;
        emojiData.textureCompression = TextureImporterCompression.Uncompressed;
        emojiData.SaveAndReimport();
    }
}
