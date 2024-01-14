using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Experimental.Rendering;

public class TextureTool : Editor
{
    private static string assetPath0 = "Assets/ResourcesAB/Texture";
    private static string assetPath1 = "Assets/Arts";

    private static string assetPath_ignore0 = "Assets/ResourcesAB/Texture/Paint";

    private static string assetPath_readable0 = "Assets/ResourcesAB/Texture/Big/Map";
    private static string assetPath_readable1 = "Assets/ResourcesAB/Texture/LittleGame";

    private static string[] assetPath = new string[] { assetPath0, assetPath1 };

    private static string charTexturePath = "Assets/Arts/Charactor/Char";
    private static string charactorTexturePath = "Assets/Arts/Charactor";
    private static string sceneTexturePath = "Assets/Arts/Scene";

    private static string terrainFlag = "_ML_terrain_";

    private static string eyeFlag = "_eye_";
    private static string hairFlag = "_hair_";
    private static string bodyFlag = "_body_";

    private static HashSet<string> supportsExt = new HashSet<string> { ".tga", ".png" };

    [MenuItem("Assets/RenderTextureToTexture2D")]
    static void RenderTextureToTexture2D()
    {
        RenderTexture rt = Selection.activeObject as RenderTexture;
        if (rt)
        {
            Texture2D texture2D = new Texture2D(rt.width, rt.height, GraphicsFormat.R8G8B8A8_UNorm, 0, TextureCreationFlags.None);
            RenderTexture.active = rt;

            texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture2D.Apply();

            byte[] bytes1 = texture2D.EncodeToPNG();

            string outputfile1 = AssetDatabase.GetAssetPath(Selection.activeObject);//"Assets/test.png";
            outputfile1 = Path.ChangeExtension(outputfile1, "png");

            System.IO.File.WriteAllBytes(outputfile1, bytes1);

            DestroyImmediate(texture2D);
        }
    }

    [MenuItem("__Tools__/Set Texture")]
    static void AllTextureProcess()
    {
        int rlt = EditorUtility.DisplayDialogComplex("设置纹理属性", "该过程需要较长时间", "设置", "取消", "检测");
        if (0 == rlt)
        {
            string[] ids = AssetDatabase.FindAssets("t:Texture", assetPath);

            for (int i = 0; i < ids.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(ids[i]);
                if (EditorUtility.DisplayCancelableProgressBar("设置纹理属性", string.Format("({0}/{1}){2}", i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
                {
                    break;
                }

                OneTextureProcess(path);
            }
        }
        else if (2 == rlt)
        {
            Debug.ClearDeveloperConsole();

            string[] ids = AssetDatabase.FindAssets("t:Texture", assetPath);

            stringBuilder = new System.Text.StringBuilder();
            stringBuilderAll = new System.Text.StringBuilder();

            for (int i = 0; i < ids.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(ids[i]);
                if (EditorUtility.DisplayCancelableProgressBar("设置纹理属性", string.Format("({0}/{1}){2}", i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
                {
                    break;
                }

                OneTextureCheck(path);
            }

            StreamWriter streamWriter = File.CreateText(Application.dataPath + "/../ImagesLog.txt");
            streamWriter.Write(stringBuilderAll.ToString());
            streamWriter.Dispose();
            streamWriter.Close();

            stringBuilderAll.Clear();
            stringBuilderAll = null;

            stringBuilder.Clear();
            stringBuilder = null;
        }

        EditorUtility.ClearProgressBar();

        System.GC.Collect();

        AssetDatabase.Refresh();

        System.GC.Collect();
    }

    static System.Text.StringBuilder stringBuilder;
    static System.Text.StringBuilder stringBuilderAll;

    static void OneTextureCheck(string path)
    {
        stringBuilder.Clear();

        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);

        bool hasError = false;
        stringBuilder.AppendFormat("{0} ({1}X{2})", path, texture.width.ToString(), texture.height.ToString());

        if (0 != texture.width % 4)
        {
            hasError = true;
            stringBuilder.Append(" (宽)不是4的倍数,");
        }

        if (0 != texture.height % 4)
        {
            hasError = true;
            stringBuilder.Append(" (高)不是4的倍数,");
        }

        //if (!Mathf.IsPowerOfTwo(texture.width))
        //{
        //    hasError = true;
        //    stringBuilder.Append(" (宽)不是2的幂次,");
        //}
        //
        //if (!Mathf.IsPowerOfTwo(texture.height))
        //{
        //    hasError = true;
        //    stringBuilder.Append(" (高)不是2的幂次,");
        //}

        if (texture.width > 1024 || texture.height > 1024)
        {
            hasError = true;
            stringBuilder.Append(" 尺寸过大");
        }

        if (hasError)
        {
            string s = stringBuilder.ToString();
            Debug.LogError(s);

            stringBuilderAll.AppendLine(s);

            string from = Application.dataPath + "/../" + path;
            string to = Application.dataPath + "/../Images/" + path;

            string dir = Path.GetDirectoryName(to);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Copy(from, to);
        }

        DestroyImmediate(texture, true);
    }
    static void OneTextureProcess(string path)
    {
        TextureImporter textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null)
        {
            Debug.LogErrorFormat("{0} 获取TextureImporter失败", path);
            return;
        }
        SetTextureImporter(textureImporter, path);
        AssetDatabase.WriteImportSettingsIfDirty(path);
    }    
    public static void SetTextureImporter(TextureImporter textureImporter, string path, bool forceResetSize = false, bool reimport = false)
    {
        if (!textureImporter)
            return;

        if (!path.StartsWith(assetPath0, System.StringComparison.Ordinal) && !path.StartsWith(assetPath1, System.StringComparison.Ordinal))
            return;

        if (path.StartsWith(assetPath_ignore0, System.StringComparison.Ordinal))
            return;

        if (path.StartsWith(charactorTexturePath, System.StringComparison.Ordinal) || path.StartsWith(sceneTexturePath, System.StringComparison.Ordinal))
        {
            textureImporter.mipmapEnabled = true;
        }
        else
        {
            string fileName = Path.GetFileName(path);
            if (fileName.Contains(terrainFlag))
            {
                textureImporter.mipmapEnabled = true;
            }
            else
            {
                textureImporter.mipmapEnabled = false;
            }
        }

        if (!path.StartsWith(assetPath_readable0, System.StringComparison.Ordinal)
            && !path.StartsWith(assetPath_readable1, System.StringComparison.Ordinal))
        {
            textureImporter.isReadable = false;
        }

        TextureImporterPlatformSettings AndroidImporter = textureImporter.GetPlatformTextureSettings("Android");
        TextureImporterPlatformSettings iPhoneImporter = textureImporter.GetPlatformTextureSettings("iOS");

        if (textureImporter.textureType == TextureImporterType.NormalMap)
        {
            AndroidImporter.format = TextureImporterFormat.ETC2_RGBA8;
            iPhoneImporter.format = TextureImporterFormat.ASTC_4x4;
        }
        else
        {
            if (textureImporter.alphaSource == TextureImporterAlphaSource.FromInput && !textureImporter.DoesSourceTextureHaveAlpha())
            {
                textureImporter.alphaSource = TextureImporterAlphaSource.None;
            }

            if (textureImporter.alphaSource != TextureImporterAlphaSource.None)
            {
                bool isValidFormat = AndroidImporter.format == TextureImporterFormat.ETC2_RGBA8 || AndroidImporter.format == TextureImporterFormat.ETC2_RGBA8Crunched || AndroidImporter.format == TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA;
                if (!isValidFormat || reimport)
                {
                    AndroidImporter.format = TextureImporterFormat.ETC2_RGBA8;
                }

                iPhoneImporter.format = TextureImporterFormat.ASTC_4x4;
            }
            else
            {
                bool isValidFormat = AndroidImporter.format == TextureImporterFormat.ETC_RGB4Crunched || AndroidImporter.format == TextureImporterFormat.ETC_RGB4 || AndroidImporter.format == TextureImporterFormat.ETC2_RGBA8;
                if (!isValidFormat || reimport)
                {
                    if (path.StartsWith(charTexturePath, System.StringComparison.Ordinal))
                    {
                        AndroidImporter.format = TextureImporterFormat.ETC2_RGBA8;
                    }
                    else
                    {
                        AndroidImporter.format = TextureImporterFormat.ETC_RGB4;
                    }
                }

                if (path.StartsWith(charTexturePath, System.StringComparison.Ordinal))
                {
                    iPhoneImporter.format = TextureImporterFormat.ASTC_4x4;
                }
                else
                {
                    iPhoneImporter.format = TextureImporterFormat.ASTC_8x8;
                }
            }
        }

        if (path.StartsWith(charactorTexturePath, System.StringComparison.Ordinal))
        {
            ResetMaxTextureSize(textureImporter, AndroidImporter, iPhoneImporter, path, forceResetSize);
        }

        AndroidImporter.compressionQuality = 100;
        AndroidImporter.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;

        iPhoneImporter.compressionQuality = 100;
        iPhoneImporter.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;

        AndroidImporter.overridden = true;
        iPhoneImporter.overridden = true;

        textureImporter.SetPlatformTextureSettings(AndroidImporter);
        textureImporter.SetPlatformTextureSettings(iPhoneImporter);
    }
    private static void ResetMaxTextureSize(TextureImporter textureImporter, TextureImporterPlatformSettings AndroidImporter, TextureImporterPlatformSettings iPhoneImporter, string path, bool forceResetSize = false)
    {
        bool needSetAndroidMaxSize = forceResetSize || (!AndroidImporter.overridden || AndroidImporter.maxTextureSize == 8192);
        bool needSetiPhoneMaxSize = forceResetSize || (!iPhoneImporter.overridden || iPhoneImporter.maxTextureSize == 8192);
        if (needSetAndroidMaxSize || needSetiPhoneMaxSize)
        {
            object[] args = new object[2] { 0, 0 };
            System.Reflection.MethodInfo methodInfo = typeof(TextureImporter).GetMethod("GetWidthAndHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(textureImporter, args);

            int width = (int)args[0];
            int height = (int)args[1];

            int maxTextureSize;
            if (path.Contains(eyeFlag))
            {
                maxTextureSize = 128;
            }
            else if (path.Contains(bodyFlag))
            {
                maxTextureSize = 256;
            }
            else if (path.Contains(hairFlag))
            {
                maxTextureSize = 256;
            }
            else
            {
                maxTextureSize = Mathf.ClosestPowerOfTwo(Mathf.Max(height, width)) >> 1;
            }

            if (needSetAndroidMaxSize)
            {
                AndroidImporter.maxTextureSize = maxTextureSize;
            }

            if (needSetiPhoneMaxSize)
            {
                iPhoneImporter.maxTextureSize = maxTextureSize;
            }
        }
    }   

    [MenuItem("__Tools__/纹理设置/角色")]
    private static void CharactorTextureSettings()
    {
        string title = "设置角色纹理";
        if (!EditorUtility.DisplayDialog(title, "该过程需要较长时间", "生成", "取消"))
            return;

        string format = "({0}/{1}){2}";

        string[] inRootPaths = new string[] { charactorTexturePath };
        string[] ids = AssetDatabase.FindAssets("t:Texture", inRootPaths);
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(title, string.Format(format, i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                continue;

            TextureImporterPlatformSettings AndroidImporter = importer.GetPlatformTextureSettings("Android");
            TextureImporterPlatformSettings iPhoneImporter = importer.GetPlatformTextureSettings("iOS");

            ResetMaxTextureSize(importer, AndroidImporter, iPhoneImporter, path, true);

            importer.mipmapEnabled = true;
            importer.streamingMipmaps = true;

            importer.SetPlatformTextureSettings(AndroidImporter);
            importer.SetPlatformTextureSettings(iPhoneImporter);
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
        EditorUtility.ClearProgressBar();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(title, "完成", "好的");
    }

    [MenuItem("__Tools__/纹理设置/场景")]
    private static void ScenesTextureMipmap()
    {
        string title = "设置场景纹理 开启mipmap";
        if (!EditorUtility.DisplayDialog(title, "该过程需要较长时间", "生成", "取消"))
            return;

        string format = "({0}/{1}){2}";

        //场景
        string[] inRootPaths = new string[] { sceneTexturePath };
        string[] ids = AssetDatabase.FindAssets("t:Texture", inRootPaths);
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(title, string.Format(format, i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                continue;

            importer.mipmapEnabled = true;
            importer.streamingMipmaps = true;

            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
        EditorUtility.ClearProgressBar();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(title, "完成", "好的");
    }

    [MenuItem("__Tools__/纹理设置/导出角色纹理信息")]
    private static void OutputCharTextureInfo()
    {
        OutputTextureInfo("CharactorTextureInfo", new string[] { charactorTexturePath });
    }

    [MenuItem("__Tools__/纹理设置/导出场景纹理信息")]
    private static void OutputSceneTextureInfo()
    {
        OutputTextureInfo("SceneTextureInfo", new string[] { sceneTexturePath });
    }

    [MenuItem("__Tools__/纹理设置/导出UI纹理信息")]
    private static void OutputUITextureInfo()
    {
        OutputTextureInfo("UITextureInfo", new string[] { assetPath0 });
    }

    static string gou = "√";
    static string cha = "×";

    private static void OutputTextureInfo(string infoName, string[] inPaths)
    {
        string title = "导出纹理信息" + infoName;
        if (!EditorUtility.DisplayDialog(title, "该过程需要较长时间", "生成", "取消"))
            return;

        string format = "({0}/{1}){2}";
        string csvFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}\n";
        object[] args = new object[2] { 0, 0 };

        string infoPath = Path.GetFullPath(Application.dataPath + "/../" + infoName + ".csv");

        FileStream fileStream = File.Open(infoPath, FileMode.Create, FileAccess.Write);
        fileStream.Dispose();
        fileStream.Close();

        File.AppendAllText(infoPath, "路径,宽,高,其他,4的倍数,NPOT,就近宽,就近高,MipMap,MipMapStream,Android.overridden,Android.maxSize,Android.format,iPhone.overridden,iPhone.maxSize,iPhone.format\n", System.Text.Encoding.UTF8);
        
        string[] inRootPaths = inPaths;
        string[] ids = AssetDatabase.FindAssets("t:Texture", inRootPaths);
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(title, string.Format(format, i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                continue;            

            TextureImporterPlatformSettings AndroidImporter = importer.GetPlatformTextureSettings("Android");
            TextureImporterPlatformSettings iPhoneImporter = importer.GetPlatformTextureSettings("iOS");

            args[0] = 0;
            args[1] = 0;
            System.Reflection.MethodInfo methodInfo = typeof(TextureImporter).GetMethod("GetWidthAndHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(importer, args);

            int width = (int)args[0];
            int height = (int)args[1];

            int NOT_W = width;
            int NOT_H = height;

            bool multipleOf4 = true;

            bool POT_W = Mathf.IsPowerOfTwo(width);
            if (!POT_W)
            {
                NOT_W = Mathf.NextPowerOfTwo(width);
                if (0 != width % 4)
                {
                    multipleOf4 = false;
                }
            }

            bool POT_H = Mathf.IsPowerOfTwo(height);
            if (!POT_H)
            {                
                NOT_H = Mathf.NextPowerOfTwo(height);
                if (0 != height % 4)
                {
                    multipleOf4 = false;
                }
            }

            float WHR = (float)width / (float)height;
            bool isXiChangTiao = (WHR > 2 || WHR < 0.5);

            string otherDes = string.Empty;
            if(isXiChangTiao)
            {
                otherDes = "细长 ";
            }

            if(NOT_W > 1024 || NOT_H > 1024)
            {
                otherDes += "尺寸过大 ";
            }

            string s = string.Format(csvFormat,
                path,                
                width.ToString(), height.ToString(),
                otherDes,
                multipleOf4 ? gou : cha, (POT_W && POT_H) ? gou : cha,
                NOT_W.ToString(), NOT_H.ToString(),
                importer.mipmapEnabled ? gou : cha, importer.streamingMipmaps ? gou : cha,
                AndroidImporter.overridden ? gou : cha, AndroidImporter.maxTextureSize.ToString(), AndroidImporter.format.ToString(),
                iPhoneImporter.overridden ? gou : cha, iPhoneImporter.maxTextureSize.ToString(), iPhoneImporter.format.ToString()
                );


            File.AppendAllText(infoPath, s);

            if (!multipleOf4)
            {
                string from = Application.dataPath + "/../" + path;
                string to = Application.dataPath + "/../Image/multipleOf4/" + path;

                string dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(from, to);
            }

            if (!isXiChangTiao)
            {
                string from = Application.dataPath + "/../" + path;
                string to = Application.dataPath + "/../Image/XiChangTiao/" + path;

                string dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(from, to);
            }

            if (NOT_W > 1024 || NOT_H > 1024)
            {
                string from = Application.dataPath + "/../" + path;
                string to = Application.dataPath + "/../Image/sobig/" + path;

                string dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(from, to);
            }

            if(!(POT_W && POT_H))
            {
                string from = Application.dataPath + "/../" + path;
                string to = Application.dataPath + "/../Image/NPOT/" + path;

                string dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(from, to);
            }
        }

        EditorUtility.ClearProgressBar();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(title, "完成", "好的");
    }

    //[MenuItem("__Tools__/纹理设置/角色2")]
    private static void CharactorTextureSettings2()
    {
        string title = "设置角色纹理";
        if (!EditorUtility.DisplayDialog(title, "该过程需要较长时间", "生成", "取消"))
            return;

        string format = "({0}/{1}){2}";

        string[] inRootPaths = new string[] { charTexturePath };
        string[] ids = AssetDatabase.FindAssets("t:Texture", inRootPaths);
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(title, string.Format(format, i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                continue;

            SetTextureImporter(importer, path, false, true);

            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
        EditorUtility.ClearProgressBar();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(title, "完成", "好的");
    }

    [MenuItem("__Tools__/纹理设置/角色3")]
    private static void CharactorTextureSettings3()
    {
        string title = "设置角色纹理";
        if (!EditorUtility.DisplayDialog(title, "该过程需要较长时间", "生成", "取消"))
            return;

        string format = "({0}/{1}){2}";

        string[] inRootPaths = new string[] { charactorTexturePath };
        string[] ids = AssetDatabase.FindAssets("t:Texture", inRootPaths);
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(title, string.Format(format, i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                continue;

            TextureImporterPlatformSettings AndroidImporter = importer.GetPlatformTextureSettings("Android");
            TextureImporterPlatformSettings iPhoneImporter = importer.GetPlatformTextureSettings("iOS");

            if (iPhoneImporter.maxTextureSize < AndroidImporter.maxTextureSize)
            {
                iPhoneImporter.maxTextureSize = AndroidImporter.maxTextureSize;

                importer.SetPlatformTextureSettings(iPhoneImporter);
                AssetDatabase.WriteImportSettingsIfDirty(path);
            }                        
        }
        EditorUtility.ClearProgressBar();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(title, "完成", "好的");
    }

    [MenuItem("__Tools__/纹理设置/图集原始纹理")]
    private static void ImageSettings()
    {
        string title = "设置图集原始纹理";
        if (!EditorUtility.DisplayDialog(title, "该过程需要较长时间", "生成", "取消"))
            return;

        string format = "({0}/{1}){2}";

        string[] inRootPaths = new string[] { "Assets/Projects/Image" };
        string[] ids = AssetDatabase.FindAssets("t:Texture", inRootPaths);
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(title, string.Format(format, i.ToString(), ids.Length.ToString(), path), (float)i / (float)ids.Length))
            {
                break;
            }

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (!importer)
                continue;

            TextureImporterPlatformSettings defaultImporter = importer.GetDefaultPlatformTextureSettings();
            defaultImporter.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SetPlatformTextureSettings(defaultImporter);
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
        EditorUtility.ClearProgressBar();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(title, "完成", "好的");
    }
}
