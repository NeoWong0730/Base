using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class UITool : EditorWindow
{
    static private bool isHotfix = false;

    [UnityEditor.MenuItem("UITool/UI")]
    public static void Create()
    {
        isHotfix = false;
        UITool uITool = CreateWindow<UITool>("UITool");        
        uITool.Show();
    }

    [UnityEditor.MenuItem("UITool/HotFixUI")]
    public static void Create2()
    {
        isHotfix = true;
        UITool uITool = CreateWindow<UITool>("UITool");        
        uITool.Show();
    }

    private Vector2 scrPos;
    private Vector2 scrPos2;
    private string[] spriteAtlasPaths;
    private bool[] spriteSelected;

    private string[] prefabPaths;
    private bool[] prefabSelected;

    private int toolBarIndex = 0;
    private string[] toolBarContents = new string[] { "Atlas", "Prefab", "UI预制引用", "单个预制", "纹理引用", "多余纹理" };    

    private List<AssetData> prefabDatas = null;
    private System.Type[] allType = null;
    private HashSet<System.Type> showType = new HashSet<System.Type>();

    private Dictionary<string, List<Object>> usedTexture = new Dictionary<string, List<Object>>();
    private Dictionary<Object, AssetData> allAssetDatas = new Dictionary<Object, AssetData>();

    private List<string[]> allTexture = new List<string[]>();
    private bool[] allTextureSelect = null;
    private string[] allTextureFolders = null;

    private string atlasImagePath = "Assets/Projects/Image/";

    private string atlasGroup = "Atlas";

    private bool showAll = false;
    private bool showAtlas = false;
    private bool filterOther = false;
   
    private GameObject _openedUIObject = null;
    public Dictionary<string, List<Image>> _atlassInfo = new Dictionary<string, List<Image>>();

    private HashSet<string> errorAtlas = new HashSet<string>();
    private string sFindButton = "查找";

    private string[] excessTextures;
    private Vector2 excessTexturesPos;    

    public class AssetData
    {
        public string sName;
        public Object prefab;
        public List<Object> atlass = new List<Object>();
        public Dictionary<System.Type, List<AssetData>> references = new Dictionary<System.Type, List<AssetData>>();
        public bool foldout;        

        public bool hasErrorAtlas = false;
        public bool hasWarningAtlas = false;        
    }

    private void OnEnable()
    {
        spriteAtlasPaths = AssetDatabase.FindAssets("t:spriteatlas", new string[] { "Assets/ResourcesAB/Atlas" });
        for (int i = 0; i < spriteAtlasPaths.Length; ++i)
        {
            spriteAtlasPaths[i] = AssetDatabase.GUIDToAssetPath(spriteAtlasPaths[i]);
        }
        spriteSelected = new bool[spriteAtlasPaths.Length];

        UIAtlasUtility.GenAtlasMap(spriteAtlasPaths);

        if (isHotfix)
        {
            prefabPaths = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });
        }
        else
        {
            prefabPaths = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/ResourcesAB/UI" });
        }
        
        for (int i = 0; i < prefabPaths.Length; ++i)
        {
            prefabPaths[i] = AssetDatabase.GUIDToAssetPath(prefabPaths[i]);
        }
        prefabSelected = new bool[prefabPaths.Length];
    }

    private void OnDisable()
    {
        UIAtlasUtility.Clear();
        spriteAtlasPaths = null;
        spriteSelected = null;
        prefabPaths = null;
        prefabSelected = null;
        prefabDatas = null;
        allType = null;
        showType.Clear();
        usedTexture.Clear();
        allAssetDatas.Clear();

        _atlassInfo.Clear();

        System.GC.Collect();
    }

    private void OnGUI()
    {
        toolBarIndex = GUILayout.Toolbar(toolBarIndex, toolBarContents);

        if (toolBarIndex == 0)
        {
            OnGUI_Atlas();
        }
        else if (toolBarIndex == 1)
        {
            OnGUI_Prefab();
        }
        else if (toolBarIndex == 2)
        {
            OnGUI_Analysis();
        }
        else if (toolBarIndex == 3)
        {
            OnGUI_OneUIPrefab();
        }
        else if (toolBarIndex == 4)
        {
            OnGUI_Texture();
        }
        else if (toolBarIndex == 5)
        {
            OnGUI_ExcessTexture();
        }
    }

    private void OnGUI_Atlas()
    {
        scrPos = EditorGUILayout.BeginScrollView(scrPos);
        for (int i = 0; i < spriteAtlasPaths.Length; ++i)
        {
            spriteSelected[i] = EditorGUILayout.ToggleLeft(spriteAtlasPaths[i], spriteSelected[i]);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("生成"))
        {
            string outputPath = "ResourcesAB/AtlasExport";
            string AtlasExport = string.Format("{0}/{1}", Application.dataPath, outputPath);
            if (!Directory.Exists(AtlasExport))
            {
                Directory.CreateDirectory(AtlasExport);
                AssetDatabase.Refresh();
            }

            List<string> needs = new List<string>();
            for (int i = 0; i < spriteAtlasPaths.Length; ++i)
            {
                if (spriteSelected[i] == false)
                    continue;

                needs.Add(spriteAtlasPaths[i]);
            }

            for (int i = 0; i < needs.Count; ++i)
            {
                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(needs[i]);
                if (spriteAtlas == null)
                {
                    Debug.LogErrorFormat("加载{0}失败", needs[i]);
                    continue;
                }

                string title = string.Format("生成{0} ({1}/{2})", spriteAtlas.name, i + 1, needs.Count);

                string[] spritePaths = UIAtlasUtility.GetSpriteFromSpriteAtlas(spriteAtlas);
                int rlt = UIAtlasUtility.CreateAtlas(spritePaths, string.Format("{0}/{1}", outputPath, spriteAtlas.name), title);
                if (rlt != 0)
                {
                    break;
                }
                Debug.LogFormat("生成{0}完成", spriteAtlas.name);
            }
        }
    }

    private void OnGUI_Prefab()
    {
        scrPos = EditorGUILayout.BeginScrollView(scrPos);
        for (int i = 0; i < prefabPaths.Length; ++i)
        {
            prefabSelected[i] = EditorGUILayout.ToggleLeft(prefabPaths[i], prefabSelected[i]);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("全选"))
        {
            for (int i = 0; i < prefabSelected.Length; ++i)
            {
                prefabSelected[i] = true;
            }
        }

        if (GUILayout.Button("全不选"))
        {
            for (int i = 0; i < prefabSelected.Length; ++i)
            {
                prefabSelected[i] = false;
            }
        }

        if (GUILayout.Button("生成"))
        {
            if (isHotfix)
            {
                string[] dependencies = AssetDatabase.GetDependencies(prefabPaths);

                List<string> froms = new List<string>();
                List<string> tos = new List<string>();

                for (int j = 0; j < dependencies.Length; ++j)
                {
                    string dependencie = dependencies[j];

                    if (dependencie.StartsWith(atlasImagePath))
                    {                        
                        string to = "Assets/Resources/Image/" + Path.GetFileName(dependencie);
                        
                        if (AssetDatabase.LoadMainAssetAtPath(to) == null)
                        {
                            AssetDatabase.CopyAsset(dependencie, to);
                        }                        

                        froms.Add(AssetDatabase.AssetPathToGUID(dependencie));
                        tos.Add(AssetDatabase.AssetPathToGUID(to));
                    }
                }

                //string from = "Assets/Projects/HotfixUI/";
                //string to = "Assets/Resources";
                for (int i = 0; i < prefabPaths.Length; ++i)
                {
                    ChangeAssets(prefabPaths[i], froms, tos);
                }
            }
            else
            {
                string atlasPath = "Assets/ResourcesAB/AtlasExport";
                string outputPath = "ResourcesAB/UIExport";
                string UIExport = string.Format("{0}/{1}", Application.dataPath, outputPath);

                // if (!Directory.Exists(UIExport))
                // {
                //     Directory.CreateDirectory(UIExport);
                //     AssetDatabase.Refresh();
                // }

                for (int i = 0; i < prefabPaths.Length; ++i)
                {
                    if (prefabSelected[i] == false)
                        continue;

                    UIAtlasUtility.TranslateUIPrefab(prefabPaths[i], outputPath, atlasPath);
                }
            }
        }
    }

    private void OnGUI_Analysis()
    {
        if(_openedUIObject != null)
        {
            OnGUI_OneUIPrefab();
            return;
        }            

        if (GUILayout.Button("分析"))
        {
            _AnalysisPrefabReferences();
            _AnalysisAtlas();
        }

        if (prefabDatas == null)
            return;

        scrPos = EditorGUILayout.BeginScrollView(scrPos);
        for (int i = 0; i < prefabDatas.Count; ++i)
        {
            AssetData prefabData = prefabDatas[i];
            OnGUI_AssetData(prefabData);
        }
        EditorGUILayout.EndScrollView();

        if (allType == null)
            return;

        //scrPos2 = EditorGUILayout.BeginScrollView(scrPos2);
        for (int i = 0; i < allType.Length; ++i)
        {
            bool show = EditorGUILayout.ToggleLeft(allType[i].ToString(), showType.Contains(allType[i]));
            if (show)
            {
                showType.Add(allType[i]);
            }
            else
            {
                showType.Remove(allType[i]);
            }
        }

        EditorGUILayout.Space();
        filterOther = EditorGUILayout.ToggleLeft("是否过滤其他", filterOther);
        showAtlas = EditorGUILayout.ToggleLeft(atlasGroup, showAtlas);
        //EditorGUILayout.EndScrollView();
    }

    private void OnGUI_Texture()
    {
        if (GUILayout.Button("分析"))
        {
            _AnalysisPrefabReferences();
            _AnalysisAtlas();
        }                

        if (allTextureFolders == null)
            return;

        showAll = EditorGUILayout.ToggleLeft("是否显示所有Image", showAll);

        scrPos = EditorGUILayout.BeginScrollView(scrPos);
        for (int i = 0; i < allTextureFolders.Length; ++i)
        {
            allTextureSelect[i] = EditorGUILayout.Foldout(allTextureSelect[i], allTextureFolders[i]);
            if (allTextureSelect[i])
            {
                EditorGUI.indentLevel += 1;
                string[] textures = allTexture[i];
                for (int j = 0; j < textures.Length; ++j)
                {
                    EditorGUILayout.BeginHorizontal();
                    bool has = usedTexture.TryGetValue(textures[j], out List<Object> objs);
                    if (showAll || !has || objs.Count == 0)
                    {
                        EditorGUILayout.LabelField(has ? objs.Count.ToString() : "0");
                        EditorGUILayout.LabelField(textures[j]);
                    }
                    EditorGUILayout.EndHorizontal();

                    if(has && showAll)
                    {
                        for (int k = 0; k < objs.Count; ++k)
                        {
                            EditorGUILayout.ObjectField(objs[k], typeof(Object), false);
                        }
                    }                    
                }
                EditorGUI.indentLevel -= 1;
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnGUI_AssetData(AssetData prefabData)
    {
        if(filterOther)
        {
            bool has = false;
            foreach(var v in showType)
            {
                if(prefabData.references.ContainsKey(v))
                {
                    has = true;
                    break;
                }
            }

            if (!has)
            {
                return;
            }                
        }

        if (prefabData.hasErrorAtlas)
        {
            GUI.color = Color.red;
        }
        else if(prefabData.hasWarningAtlas)
        {
            GUI.color = Color.yellow;
        }
        prefabData.foldout = EditorGUILayout.Foldout(prefabData.foldout, prefabData.sName, true);

        GUI.color = Color.white;

        if (prefabData.foldout)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.ObjectField(prefabData.prefab, typeof(GameObject), false);
            EditorGUILayout.Space();

            foreach (var kv in prefabData.references)
            {
                if (!showType.Contains(kv.Key))
                    continue;

                EditorGUILayout.LabelField(kv.Key.Name);

                EditorGUI.indentLevel += 1;
                List<AssetData> deps = kv.Value;
                for (int k = 0; k < deps.Count; ++k)
                {
                    //EditorGUILayout.ObjectField(deps[k], kv.Key, false);
                    OnGUI_AssetData(deps[k]);
                }
                EditorGUI.indentLevel -= 1;
            }

            if(prefabData.atlass.Count > 0 && showAtlas)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(atlasGroup);
                EditorGUI.indentLevel += 1;
                for (int j = 0; j < prefabData.atlass.Count; ++j)
                {                    
                    EditorGUILayout.ObjectField(prefabData.atlass[j], typeof(DefaultAsset), false);                 
                }
                EditorGUI.indentLevel -= 1;
            }                       

            EditorGUI.indentLevel -= 1;
        }        
    }

    private void OnGUI_OneUIPrefab()
    {        
        _openedUIObject = EditorGUILayout.ObjectField(_openedUIObject, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("分析"))
        {
            _atlassInfo.Clear();

            GameObject go = _openedUIObject;

            Image[] images = go.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; ++i)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Find", null, (float)i / prefabPaths.Length))
                {
                    break;
                }

                Image image = images[i];
                if (image.sprite != null)
                {
                    string s = AssetDatabase.GetAssetPath(image.sprite);
                    if (s.StartsWith("Assets/Projects/Image"))
                    {
                        s = Path.GetDirectoryName(s);
                        if (!_atlassInfo.TryGetValue(s, out List<Image> objs))
                        {
                            objs = new List<Image>();
                            _atlassInfo.Add(s, objs);
                        }
                        objs.Add(image);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            //_openedUIObject = go;
        }

        scrPos = EditorGUILayout.BeginScrollView(scrPos);
        foreach (var kv in _atlassInfo)
        {
            EditorGUILayout.LabelField(kv.Key);
            EditorGUI.indentLevel += 1;
            List<Image> objs = kv.Value;
            for (int j = 0; j < objs.Count; ++j)
            {
                EditorGUILayout.ObjectField(objs[j], typeof(Image), false);
            }
            EditorGUI.indentLevel -= 1;
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnGUI_ExcessTexture()
    {
        if (GUILayout.Button("分析"))
        {
            CheckExcessTexture();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("加载Sprite"))
        {
            string path = Application.dataPath + "/../Sprite0.csv";
            excessTextures = File.ReadAllLines(path);
        }

        if (GUILayout.Button("加载Texture"))
        {
            string path = Application.dataPath + "/../UITexture0.csv";
            excessTextures = File.ReadAllLines(path);
        }
        EditorGUILayout.EndHorizontal();

        if (excessTextures != null)
        {
            excessTexturesPos = EditorGUILayout.BeginScrollView(excessTexturesPos);
            for (int i = 0; i < excessTextures.Length; ++i)
            {
                if(string.IsNullOrWhiteSpace(excessTextures[i]))
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(excessTextures[i]);
                if (GUILayout.Button(sFindButton))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Texture2D>(excessTextures[i]);
                    if(obj)
                    {
                        Selection.activeObject = obj;
                    }
                    else
                    {
                        excessTextures[i] = string.Empty;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private AssetData _AnalysisOnePrefabReferences(string prefabPath, Object root, HashSet<string> parents)
    {
        AssetData assetData;
        if (allAssetDatas.TryGetValue(root, out assetData))
        {
            return assetData;
        }

        assetData = new AssetData();
        allAssetDatas.Add(root, assetData);

        assetData.sName = prefabPath;
        assetData.prefab = root;

        string[] dependencies = AssetDatabase.GetDependencies(prefabPath, false);

        for (int j = 0; j < dependencies.Length; ++j)
        {
            string dependencie = dependencies[j];

            Object obj = AssetDatabase.LoadMainAssetAtPath(dependencie);
            System.Type type = obj.GetType();

            if (dependencie.StartsWith(atlasImagePath))
            {
                int index = dependencie.LastIndexOf('/');
                if (index >= 0)
                {
                    string atlasName = dependencie.Remove(index);
                    Object atlasFolder = AssetDatabase.LoadAssetAtPath<UnityEditor.DefaultAsset>(atlasName);

                    if (!assetData.atlass.Contains(atlasFolder))
                    {
                        assetData.atlass.Add(atlasFolder);
                        if(atlasFolder.name.EndsWith("ICON", System.StringComparison.Ordinal))
                        {
                            assetData.hasErrorAtlas = true;
                        }
                    }                    
                }                
            }
            else
            {
                if (!assetData.references.TryGetValue(type, out List<AssetData> deps))
                {
                    deps = new List<AssetData>();
                    assetData.references[type] = deps;

                    if (!showType.Contains(type))
                    {
                        showType.Add(type);
                    }
                }

                AssetData childAssetData = _AnalysisOnePrefabReferences(dependencie, obj, parents);
                if(childAssetData.hasErrorAtlas)
                {
                    assetData.hasWarningAtlas = true;
                }
                deps.Add(childAssetData);
            }

            if (type == typeof(Texture2D))
            {
                Texture2D texture = obj as Texture2D;
                if(!usedTexture.TryGetValue(dependencie, out List<Object> objs))
                {
                    objs = new List<Object>();
                    usedTexture.Add(dependencie, objs);
                }
                objs.Add(assetData.prefab);
            }
        }

        return assetData;
    }

    private void _AnalysisPrefabReferences()
    {
        allAssetDatas.Clear();
        showType.Clear();        
        HashSet<string> parents = new HashSet<string>();

        prefabDatas = new List<AssetData>(prefabPaths.Length);
        for (int i = 0; i < prefabPaths.Length; ++i)
        {
            string prefabPath = prefabPaths[i];

            if (EditorUtility.DisplayCancelableProgressBar("_AnalysisPrefabReferences", prefabPath, (float)i / prefabPaths.Length))
            {
                break;
            }

            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            AssetData prefabData = _AnalysisOnePrefabReferences(prefabPath, root, parents);
            prefabDatas.Add(prefabData);
        }

        allType = new System.Type[showType.Count];
        showType.CopyTo(allType);
        showType.Clear();
        allAssetDatas.Clear();

        EditorUtility.ClearProgressBar();
    }

    private void _AnalysisAtlas()
    {
        allTextureFolders = AssetDatabase.GetSubFolders("Assets/Projects/Image");
        allTextureSelect = new bool[allTextureFolders.Length];

        allTexture = new List<string[]>(allTextureFolders.Length);
        for (int i = 0; i < allTextureFolders.Length; ++i)
        {
            EditorUtility.DisplayCancelableProgressBar("_AnalysisPrefabReferences", allTextureFolders[i], (float)i / allTextureFolders.Length);            
            string[] spritePaths = AssetDatabase.FindAssets("t:sprite", new string[] { allTextureFolders[i] });
            for(int j = 0; j < spritePaths.Length; ++j)
            {
                spritePaths[j] = AssetDatabase.GUIDToAssetPath(spritePaths[j]);
            }
            allTexture.Add(spritePaths);
        }
        EditorUtility.ClearProgressBar();
    }

    public static void ChangeAssets(string path, List<string> from, List<string> to)
    {
        string metaPath = Application.dataPath + "/../" + path;
        string s = File.ReadAllText(metaPath);
        for (int i = 0; i < from.Count; ++i)
        {
            s = s.Replace(from[i], to[i]);
        }
        File.WriteAllText(metaPath, s);
    }
    
    private void CheckExcessTexture()
    {
        Dictionary<string, List<string>> textureReference = new Dictionary<string, List<string>>(2048);

        int rlt = EditorUtility.DisplayDialogComplex("导出可能多余的纹理", "该过程需要较长时间", "导出", "取消", "取消");
        if (rlt != 0)
            return;

        string spriteFolder = "Assets/Projects/Image";

        string textureTitle = "获取所有的Texture";
        string textureFolder = "Assets/ResourcesAB/Texture";

        string uiTitle = "检查UI引用";
        string uiFolder = "Assets/ResourcesAB/UI";

        string csvTitle = "写入表格";

        string[] ids = AssetDatabase.FindAssets("t:Texture", new string[] { textureFolder, spriteFolder });
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(textureTitle, path, (float)i / (float)ids.Length))
            {
                break;
            }

            textureReference.Add(path, null);
        }

        ids = AssetDatabase.FindAssets("t:Prefab", new string[] { uiFolder });
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(uiTitle, path, (float)i / (float)ids.Length))
            {
                break;
            }

            string[] deps = AssetDatabase.GetDependencies(path, true);

            for (int j = 0; j < deps.Length; ++j)
            {
                string dep = deps[j];
                if (textureReference.TryGetValue(dep, out List<string> refs))
                {
                    if (refs == null)
                    {
                        refs = new List<string>();
                        textureReference[dep] = refs;
                    }
                    refs.Add(path);
                }
            }
        }

        int index = 0;
        StreamWriter streamWriter = File.CreateText(Application.dataPath + "/../UITexture.csv");
        StreamWriter streamWriter0 = File.CreateText(Application.dataPath + "/../UITexture0.csv");
        StreamWriter streamWriter1 = File.CreateText(Application.dataPath + "/../Sprite0.csv");

        streamWriter.Write("texture");
        streamWriter.Write(',');
        streamWriter.Write("count");
        streamWriter.Write('\n');
        foreach (var v in textureReference)
        {
            string name = v.Key;
            List<string> ss = v.Value;
            if (EditorUtility.DisplayCancelableProgressBar(csvTitle, name, (float)index / (float)textureReference.Count))
            {
                break;
            }

            ++index;

            streamWriter.Write(name);
            streamWriter.Write(',');

            if (ss == null)
            {
                streamWriter.Write('0');
                streamWriter.Write(',');
                streamWriter.Write('\n');

                if(name.StartsWith(spriteFolder, System.StringComparison.Ordinal))
                {
                    streamWriter1.Write(name);
                    streamWriter1.Write('\n');
                }
                else
                {
                    streamWriter0.Write(name);
                    streamWriter0.Write('\n');
                }               
            }
            else
            {
                streamWriter.Write(ss.Count);
                streamWriter.Write(',');
                for (int i = 0; i < ss.Count; ++i)
                {
                    streamWriter.Write(ss[i]);
                    if (i < ss.Count - 1)
                    {
                        streamWriter.Write(',');
                    }
                    else
                    {
                        streamWriter.Write('\n');
                    }
                }
            }
        }
        streamWriter.Dispose();
        streamWriter.Close();

        streamWriter0.Dispose();
        streamWriter0.Close();

        streamWriter1.Dispose();
        streamWriter1.Close();

        EditorUtility.ClearProgressBar();
        System.GC.Collect();
    }

    [MenuItem("GameObject/UI/剥离纹理")]
    public static void UIRawImage()
    {
        GameObject go = Selection.activeGameObject;

        RawImage[] rawImages = go.GetComponentsInChildren<RawImage>(true);
        for (int i = 0; i < rawImages.Length; ++i)
        {
            RawImage rawImage = rawImages[i];
            string texturePath = AssetDatabase.GetAssetPath(rawImage.texture);
            if (texturePath.StartsWith("Assets/ResourcesAB/Texture"))
            {
                string bundleName = texturePath.Replace("Assets/ResourcesAB/", "");
                if (bundleName.StartsWith("Texture/Big/", System.StringComparison.Ordinal))
                {
                    string filePath = bundleName.Substring(0, bundleName.LastIndexOf("/") + 1);
                    bundleName = bundleName.Replace(filePath, "Texture/Big/");
                }

                if (!rawImage.gameObject.TryGetComponent<RawImageLoader>(out RawImageLoader rawImageLoader))
                {
                    rawImageLoader = rawImage.gameObject.AddComponent<RawImageLoader>();
                }

                rawImageLoader._sAssetPath = bundleName;
                rawImage.enabled = false;
                rawImage.texture = null;

                Debug.Log(MenuItem_App.FullHierarchyPath(rawImage.gameObject));
            }
        }

        Image[] images = go.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; ++i)
        {
            Image image = images[i];
            if(!image.sprite)
            {
                continue;
            }

            string texturePath = AssetDatabase.GetAssetPath(image.sprite.texture);
            if (texturePath.StartsWith("Assets/ResourcesAB/Texture"))
            {
                Debug.LogWarning(MenuItem_App.FullHierarchyPath(image.gameObject));
            }
        }
    }

    [MenuItem("GameObject/UI/剥离特效")]
    public static void UIEffect()
    {
        GameObject go = Selection.activeGameObject;
        CheckEffect(go.transform);
        UnityEditor.EditorUtility.SetDirty(go);
    }


    private static void DeleteChildren(Transform root)
    {
        int count = root.childCount;
        for (int i = count - 1; i >= 0; --i)
        {
            Transform transform = root.GetChild(i);
            Object.DestroyImmediate(transform.gameObject);
        }
    }

    private static void CheckEffect(Transform root)
    {
        int count = root.childCount;
        for (int i = count - 1; i >= 0; --i)
        {
            Transform transform = root.GetChild(i);
            if (PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject))
            {
                //if (PrefabUtility.HasPrefabInstanceAnyOverrides(transform.gameObject, false))
                //{
                //    Debug.LogWarning(MenuItem_App.FullHierarchyPath(transform.gameObject) + " HasPrefabInstanceAnyOverrides");
                //    continue;
                //}

                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(transform.gameObject);

                if (!path.StartsWith("Assets/ResourcesAB/Prefab/Fx", System.StringComparison.Ordinal))
                {
                    Debug.LogWarning(MenuItem_App.FullHierarchyPath(transform.gameObject) + " not has address");
                    continue;
                }
                
                Component[] components = transform.gameObject.GetComponents<Component>();
                if (components.Length > 1)
                {
                    string s = string.Empty;

                    for (int j = 0; j < components.Length; ++j)
                    {
                        s += " ";
                        s += components[j].GetType().Name;
                    }

                    Debug.LogWarning(MenuItem_App.FullHierarchyPath(transform.gameObject) + s);
                    continue;
                }

                Debug.Log(MenuItem_App.FullHierarchyPath(transform.gameObject));
                PrefabUtility.UnpackPrefabInstance(transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                
                path = path.Remove(0, "Assets/ResourcesAB/".Length);              

                AssetLoader assetLoader = transform.gameObject.AddComponent<AssetLoader>();

                SerializedObject serializedObject = new SerializedObject(assetLoader);

                SerializedProperty serializedProperty = serializedObject.FindProperty("sAssetName");
                serializedProperty.stringValue = path;

                serializedProperty = serializedObject.FindProperty("m_Parent");
                serializedProperty.objectReferenceValue = transform;

                serializedObject.ApplyModifiedProperties();

                UnityEngine.Rendering.SortingGroup sortingGroup = transform.gameObject.AddComponent<UnityEngine.Rendering.SortingGroup>();

                UILocalSortingGroup localSortingGroup = transform.gameObject.AddComponent<UILocalSortingGroup>();
                localSortingGroup.nSorting = 1;
                localSortingGroup.mSortingGroup = sortingGroup;

                DeleteChildren(transform);
            }
            else
            {
                CheckEffect(transform);
            }
        }
    }
}
