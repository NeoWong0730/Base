using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class CreateFontEditor : Editor
{

    [MenuItem("Tools/CreateBMFont")]
    static void CreateFontE()
    {
        string strReplace = "Assets/Resources/";

        Object obj = Selection.activeObject;
        string fntPath = AssetDatabase.GetAssetPath(obj);
        if (fntPath.IndexOf(".fnt") == -1)
        {
            return;
        }

        string texturePath = fntPath.Replace(".fnt", "_0");
        texturePath = texturePath.Replace(strReplace, "");
        Texture2D texture = Resources.Load<Texture2D>(texturePath);
        Debug.Log("texture--" + texture);

        string customFontPath = fntPath.Replace(".fnt", ".fontsettings");
        if (!File.Exists(customFontPath))
            return;

        fntPath = fntPath.Replace("Assets", "");
        string fullPath = Application.dataPath + fntPath;
        Debug.Log("fullPath ==" + fullPath);

        List<CharacterInfo> charList = new List<CharacterInfo>();

        StreamReader reader = new StreamReader(new FileStream(fullPath, FileMode.Open));
        Regex reg = new Regex(@"char id=(?<id>\d+)\s+x=(?<x>\d+)\s+y=(?<y>\d+)\s+width=(?<width>\d+)\s+height=(?<height>\d+)\s+xoffset=(?<xoffset>\d+)\s+yoffset=(?<yoffset>\d+)\s+xadvance=(?<xadvance>\d+)\s+");
        string line = reader.ReadLine();
        int lineHeight = 0;
        int texWidth = texture.width;
        int texHeight = texture.height;

        while (line != null)
        {
            if (line.IndexOf("char id=") != -1)
            {
                Match match = reg.Match(line);
                if (match != Match.Empty)
                {
                    var id = System.Convert.ToInt32(match.Groups["id"].Value);
                    var x = System.Convert.ToInt32(match.Groups["x"].Value);
                    var y = System.Convert.ToInt32(match.Groups["y"].Value);
                    var width = System.Convert.ToInt32(match.Groups["width"].Value);
                    var height = System.Convert.ToInt32(match.Groups["height"].Value);
                    var xoffset = System.Convert.ToInt32(match.Groups["xoffset"].Value);
                    var yoffset = System.Convert.ToInt32(match.Groups["yoffset"].Value);
                    var xadvance = System.Convert.ToInt32(match.Groups["xadvance"].Value);

                    CharacterInfo info = new CharacterInfo();
                    info.index = id;
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

                    charList.Add(info);
                }
            }
            else if (line.IndexOf("scaleW=") != -1)
            {
                //
            }

            line = reader.ReadLine();
        }

        Debug.Log("--success");
        Font customFont = AssetDatabase.LoadAssetAtPath<Font>(customFontPath);
        customFont.characterInfo = charList.ToArray();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
