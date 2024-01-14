using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class MakeArtFont
{
    public static string assetPath = "Assets/ResourcesAB/Font";

    [MenuItem("Assets/LoadFont（选中目标文件夹）")]
    static void CarteNewFont()
    {

        UnityEngine.Object obj = Selection.activeObject;
        string setImgPath = AssetDatabase.GetAssetPath(obj);
        CombinImage(setImgPath.Replace("Assets/", ""), obj.name);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static Dictionary<string, string> replaces = new Dictionary<string, string>(2) {
        { "dot", "."}, };

    /// <summary>
    /// 调用此函数后使此图片合并
    /// </summary>
    /// <param name="sourceImg">粘贴的源图片</param>
    /// <param name="destImg">粘贴的目标图片</param>
    public static void CombinImage(string sourceImg, string fileName)
    {
        string[] pngs = Directory.GetFiles(Application.dataPath + "/" + sourceImg, "*.png");

        string texturePath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), assetPath) + "/" + fileName + "_texture.png";


        int maxWidth = 0;
        int maxHeight = 0;

        foreach (string path in pngs)
        {
            string subPngPath = path.Replace(Application.dataPath, "Assets");

            if (subPngPath.IndexOf("texture") == -1)
            {
                Texture tex2D = AssetDatabase.LoadAssetAtPath(subPngPath, typeof(Texture)) as Texture;
                maxWidth += tex2D.width;
                if (maxHeight < tex2D.height)
                {
                    maxHeight = tex2D.height;
                }
            }
        }
        Texture2D fullTexture = new Texture2D(maxWidth, maxHeight, TextureFormat.RGBA32, false);
        Color[] fullTcolors = fullTexture.GetPixels();
        for (int i = 0; i < fullTcolors.Length; i++)
        {
            fullTcolors[i].a = 0;
            fullTcolors[i].r = 0;
            fullTcolors[i].g = 0;
            fullTcolors[i].b = 0;
        }
        fullTexture.SetPixels(fullTcolors);
        Vector2Int currentPos = Vector2Int.zero;
        List<BmInfo> bmInfoList = new List<BmInfo>();

        foreach (string path in pngs)
        {
            string subPngPath = path.Replace(Application.dataPath, "Assets");

            if (subPngPath.IndexOf("texture") == -1)
            {

                SetTextureImporter(subPngPath);

                Texture2D tex2D = AssetDatabase.LoadAssetAtPath(subPngPath, typeof(Texture2D)) as Texture2D;
                Color[] colors = tex2D.GetPixels();
                fullTexture.SetPixels(currentPos.x, currentPos.y, tex2D.width, tex2D.height, colors);

                BmInfo bmInfo = new BmInfo();

                if (!replaces.TryGetValue(tex2D.name, out string name)) {
                    name = tex2D.name;
                }
                bmInfo.index = Uncode(name);
                bmInfo.width = tex2D.width;
                bmInfo.height = tex2D.height;
                bmInfo.x = currentPos.x;
                bmInfo.y = fullTexture.height - tex2D.height;

                bmInfoList.Add(bmInfo);

                currentPos.x += tex2D.width;
            }
        }

        byte[] full = fullTexture.EncodeToPNG();
        File.WriteAllBytes(texturePath, full);

        AssetDatabase.Refresh();

        texturePath = texturePath.Replace(Application.dataPath, "Assets");

        SetTextureImporter(texturePath);

        Texture texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture)) as Texture;
        CreateBitmapFont(fileName, texture, bmInfoList, maxWidth, maxHeight);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        //string fontPath = assetPath + "/" + fileName + "_font.fontsettings";
        //string copyfontPath = assetPath + "/" + fileName + "_fontbackup.fontsettings";
        //string materialPath = assetPath + "/" + fileName + "_mat.mat";

        //Font assetFont = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;
        //Debug.LogError("字数量:" + assetFont.characterInfo.Length);
        //AssetDatabase.CopyAsset(fontPath, copyfontPath);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public static void SetTextureImporter(string subPngPath)
    {
        TextureImporter ti = AssetImporter.GetAtPath(subPngPath) as TextureImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.mipmapEnabled = false;
        ti.isReadable = true;
        ti.filterMode = FilterMode.Trilinear;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        AssetDatabase.ImportAsset(subPngPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }


    /// <summary>
    /// 创建新的美术字体
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="bmInfoList"></param>
    /// <param name="columnNum"></param>
    /// <param name="lineNum"></param>
    public static void CreateBitmapFont(string fileName, Texture texture, List<BmInfo> bmInfoList, int maxWidth, int maxHeight)
    {

        //string assetPath = "Assets/Resources/UI/Font"; //GetAssetPath(AssetDatabase.GetAssetPath(texture));
        string fontPath = assetPath + "/" + fileName + "_font.fontsettings";
        string materialPath = assetPath + "/" + fileName + "_mat.mat";


        Material fontMaterial = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;//new Material(Shader.Find("UI/Default"));
        if (fontMaterial == null)
        {
            fontMaterial = new Material(Shader.Find("UI/Default"));
            fontMaterial.mainTexture = texture;
            AssetDatabase.CreateAsset(fontMaterial, materialPath);
        }
        else
        {
            fontMaterial.mainTexture = texture;
        }

        Font assetFont = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;

        string ApplicationDataPath = Application.dataPath.Replace("Assets", "") + fontPath;

        if (assetFont == null)
        {
            assetFont = new Font();
            AssetDatabase.CreateAsset(assetFont, fontPath);
        }



        assetFont.material = fontMaterial;
        CharacterInfo[] characters = new CharacterInfo[bmInfoList.Count];

        var texWidth = texture.width;
        var texHeight = texture.height;

        for (int i = 0; i < bmInfoList.Count; i++)
        {
            BmInfo bmInfo = bmInfoList[i];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;

            //---
            //info.uv.width = (float)bmInfo.width / (float)maxWidth;
            //info.uv.height = (float)bmInfo.height / (float)maxHeight;
            //info.uv.x = (float)bmInfo.x / (float)maxWidth;
            //info.uv.y = (1 - (float)bmInfo.y / (float)maxHeight) - info.uv.height;
            //info.vert.x = 0;
            //info.vert.y = 0;
            //info.vert.width = (float)bmInfo.width;
            //info.vert.height = -(float)bmInfo.height;
            //info.width = (float)bmInfo.width;

            //---
            var id = bmInfo.index;
            var x = (int)bmInfo.x;
            var y = (int)bmInfo.y;
            var width = (int)bmInfo.width;
            var height = (int)bmInfo.height;
            var xoffset = 0;
            var yoffset = 0;
            var xadvance = (int)bmInfo.width;

            float uvx = 1f * x / texWidth;
            float uvy = 1 - (1f * y / texHeight);
            float uvw = 1f * width / texWidth;
            float uvh = -1f * height / texHeight;

            info.uvBottomLeft = new Vector2(uvx, uvy);
            info.uvBottomRight = new Vector2(uvx + uvw, uvy);
            info.uvTopLeft = new Vector2(uvx, uvy + uvh);
            info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);

            info.minX = xoffset;
            info.minY = yoffset + height / 2;

            info.glyphWidth = width;
            info.glyphHeight = -height;
            info.advance = xadvance;

            characters[i] = info;
        }

        assetFont.characterInfo = characters;
        EditorUtility.SetDirty(assetFont);
        AssetDatabase.SaveAssets();
    }
    private static string GetAssetPath(string path)
    {
        return path == "" || path == null ? "Assets/" : path.Substring(0, path.LastIndexOf("/"));
    }
    //将中文字符转为10进制整数
    public static int Uncode(string str)
    {
        int outStr = 0;
        if (!string.IsNullOrEmpty(str))
        {
            if (str == "。")
            {
                outStr = (int)"."[0];
            }
            else
            {
                outStr = ((int)str[0]);
            }
        }
        return outStr;
    }

}
public class BmInfo
{
    public int index = 0;
    public float width = 0;
    public float height = 0;
    public float x = 0;
    public float y = 0;
}
