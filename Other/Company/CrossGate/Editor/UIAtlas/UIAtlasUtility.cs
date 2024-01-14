using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEngine.UI;

public class UIAtlasUtility
{
    static Dictionary<string, string> floderToAtlas = null;
    
    public static void Clear()
    {
        floderToAtlas.Clear();
        floderToAtlas = null;
    }

    public static void GenAtlasMap(string[] atlasPaths)
    {
        floderToAtlas = new Dictionary<string, string>(atlasPaths.Length);

        for (int atlasIndex = 0; atlasIndex < atlasPaths.Length; ++atlasIndex)
        {
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPaths[atlasIndex]);

            Object[] objs = atlas.GetPackables();
            for (int i = 0; i < objs.Length; ++i)
            {
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (AssetDatabase.IsValidFolder(path))
                {
                    floderToAtlas[path] = atlas.name;
                    //是文件夹
                }
                else
                {
                    //是文件
                }
            }
        }            
    }

    public static string[] GetSpriteFromSpriteAtlas(SpriteAtlas atlas)
    {
        List<string> folders = new List<string>();
        Object[] objs = atlas.GetPackables();
        for (int i = 0; i < objs.Length; ++i)
        {
            string path = AssetDatabase.GetAssetPath(objs[i]);
            if (AssetDatabase.IsValidFolder(path))
            {
                folders.Add(path);
                //是文件夹
            }
            else
            {
                //是文件
            }
        }
        string[] spritePaths = AssetDatabase.FindAssets("t:sprite", folders.ToArray());
        for (int i = 0; i < spritePaths.Length; ++i)
        {
            spritePaths[i] = AssetDatabase.GUIDToAssetPath(spritePaths[i]);
        }
        return spritePaths;
    }

    public static int SetSpritesReadable(string[] spritePaths, bool readable, string title)
    {
        for (int i = 0; i < spritePaths.Length; i++)
        {
            if (EditorUtility.DisplayCancelableProgressBar(title, "Set Sprites Readable", (float)i / spritePaths.Length))
            {
                return 1;
            }

            string spritePath = spritePaths[i];
            TextureImporter t_Importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if(t_Importer.isReadable == readable)
            {
                continue;
            }

            t_Importer.isReadable = readable;
            t_Importer.SaveAndReimport();
        }
        return 0;
    }
    public static int GetSprites(string[] spritePaths, out Sprite[] sprites, string title)
    {
        sprites = new Sprite[spritePaths.Length];
        for (int i = 0; i < spritePaths.Length; i++)
        {
            if (EditorUtility.DisplayCancelableProgressBar(title, "加载要打包的 Sprites", (float)i / spritePaths.Length))
            {
                return 1;
            }

            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(spritePaths[i]);
        }
        return 0;
    }

    public static int GetTexture2D(Sprite[] sprites, out Texture2D[] textures, string title)
    {
        textures = new Texture2D[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            if (EditorUtility.DisplayCancelableProgressBar(title, "获取要打包的 Texture", (float)i / sprites.Length))
            {
                return 1;
            }

            Sprite sprite = sprites[i];
            Texture2D texture;
            if (sprite.pivot == new Vector2(0.5f, 0.5f) && sprite.border == Vector4.zero)
            {
                texture = sprite.texture;
            }
            else
            {
                Rect rect = sprites[i].rect;
                Color[] colors = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
                texture = new Texture2D((int)rect.width, (int)rect.height);
                texture.SetPixels(colors);
            }

            textures[i] = texture;
        }
        return 0;
    }

    public static int CreateAtlas(string[] spritePaths, string atlasPath, string title)
    {        
        string atlasRelativePath = string.Format("Assets/{0}.asset", atlasPath);
        string textureRelativePath = string.Format("Assets/{0}.png", atlasPath);
        string textureAbsolutePath = string.Format("{0}/{1}.png", Application.dataPath, atlasPath);
        
        //删除Texture已有的Sprite
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(textureRelativePath);
        for (int i = 0; i < objs.Length; i++)
        {
            if(EditorUtility.DisplayCancelableProgressBar(title, "删除Texture已有的Sprite", (float)i / objs.Length))
            {
                return 1;
            }

            Object obj = objs[i];
            Sprite sprite = obj as Sprite;
            if (sprite != null)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(obj));
            }
        }

        //设置精灵为可读
        int rlt = SetSpritesReadable(spritePaths, true, title);
        if (rlt != 0)
        {
            return rlt;
        }

        rlt = GetSprites(spritePaths, out Sprite[] sprites, title);
        if (rlt != 0)
        {
            return rlt;
        }

        //获取要打包的纹理
        rlt = GetTexture2D(sprites, out Texture2D[] textures, title);
        if (rlt != 0)
        {
            return rlt;
        }

        EditorUtility.DisplayProgressBar(title, "Pack Textures", 0.7f);

        // 打包成Atlas
        Texture2D atlasTex = new Texture2D(32, 32);
        Rect[] atlasRects = atlasTex.PackTextures(textures, 1, 4096);
        //将Atlas Texture写入本地        
        File.WriteAllBytes(textureAbsolutePath, atlasTex.EncodeToPNG());
        AssetDatabase.Refresh();        

        //设置Atlas的sprite
        SpriteMetaData[] spritesheet = new SpriteMetaData[atlasRects.Length];
        for (int i = 0; i < spritesheet.Length; i++)
        {
            EditorUtility.DisplayProgressBar(title, "Gen Sprite Meta Data", (float)i / spritesheet.Length);

            SpriteMetaData spriteMetaData = new SpriteMetaData();

            Sprite sprite = sprites[i];
            spriteMetaData.name = sprite.name;
            spriteMetaData.rect = atlasRects[i];
            spriteMetaData.rect.Set(spriteMetaData.rect.x * atlasTex.width, spriteMetaData.rect.y * atlasTex.height, spriteMetaData.rect.width * atlasTex.width, spriteMetaData.rect.height * atlasTex.height);
            spriteMetaData.alignment = 9;
            Rect rect = sprite.rect;
            spriteMetaData.pivot = new Vector2(sprite.pivot.x / rect.width, sprite.pivot.y / rect.height);
            spriteMetaData.border = sprite.border;
            spritesheet[i] = spriteMetaData;
        }

        EditorUtility.DisplayProgressBar(title, "Set Texture Importer", 0.7f);
        // 设置Atlas Texture属性
        TextureImporter textureImporter = AssetImporter.GetAtPath(textureRelativePath) as TextureImporter;
        //imp.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.spritesheet = spritesheet;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;        
        textureImporter.mipmapEnabled = false;                
        textureImporter.SaveAndReimport();        
        AssetDatabase.ImportAsset(textureRelativePath);
        AssetDatabase.Refresh();

        EditorUtility.DisplayProgressBar(title, "Gen Atlas", 0.7f);

        objs = AssetDatabase.LoadAllAssetsAtPath(textureRelativePath);
        List<Sprite> spriteDic = new List<Sprite>(objs.Length);
        UIAtlas atlas = ScriptableObject.CreateInstance<UIAtlas>();
        for (int i = 0; i < objs.Length; i++)
        {
            Object obj = objs[i];
            Sprite sprite = obj as Sprite;
            if (sprite != null)
            {
                spriteDic.Add(sprite);
            }
        }
        atlas.InternalSetData(spriteDic.ToArray());        
        AssetDatabase.CreateAsset(atlas, atlasRelativePath);
        AssetDatabase.Refresh();        
        EditorUtility.ClearProgressBar();
        return 0;
    }

    public static void TranslateUIPrefab(string prefabPath, string prefabExportPath, string atlasPath)
    {
        GameObject srcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject newPrefab = GameObject.Instantiate<GameObject>(srcPrefab);        

        Image[] images = newPrefab.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; ++i)
        {
            Image image = images[i];
            string path = AssetDatabase.GetAssetPath(image.sprite);
            int splitIndex = path.LastIndexOf('/');
            
            if(splitIndex < 0)
            {
                continue;
            }

            string atlasName = path.Remove(splitIndex);
            string spriteName = path.Remove(0, splitIndex + 1);
            Debug.LogFormat("{0} {1}", atlasName, spriteName);
            int exIndex = spriteName.LastIndexOf('.');
            if (exIndex > 0)
            {
                spriteName = spriteName.Remove(exIndex);
            }            
            
            floderToAtlas.TryGetValue(atlasName, out atlasName);
            if(atlasName == null)
            {
                continue;
            }
            string atlasloadPath = string.Format("{0}/{1}.asset", atlasPath, atlasName);
            UIAtlas uIAtlas = AssetDatabase.LoadAssetAtPath<UIAtlas>(atlasloadPath);
            if (uIAtlas != null)
            {
                Sprite sprite = uIAtlas.GetSprite(spriteName);
                if(sprite == null)
                    Debug.LogError(string.Format("预设:{0}, {1}不存在{2}", prefabPath, atlasName, spriteName));
                else
                    image.sprite = sprite;
            }
            else
            {
                Debug.LogError("不存在uIAtlas：" + atlasloadPath);
            }
        }
        
        string exportPath = prefabPath.Replace("Assets/ResourcesAB/UI", "Assets/ResourcesAB/UIExport");
        Debug.Log(exportPath);
        string dir = Path.GetDirectoryName(exportPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            AssetDatabase.Refresh();
        }

        PrefabUtility.SaveAsPrefabAsset(newPrefab, exportPath);
        
        GameObject.DestroyImmediate(newPrefab);
    }
}

