using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine.Rendering;
using System.Reflection;


public class AnalysisMaterial : EditorWindow
{
    public class ShaderData
    {
        public int[] passTypes;
        public List<List<string>> keyworlds;
    }

    public class Entity
    {
        public bool isShow = false;
        public Shader shader;
        public Dictionary<string, MaterialGroup> materialGroup = new Dictionary<string, MaterialGroup>();
    }

    public class MaterialGroup
    {
        public bool isShow = false;        
        public List<Material> materials = new List<Material>();
    }

    [MenuItem("__Tools__/ClearProgressBar")]
    private static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("__Tools__/Analysis Material")]
    private static void CreateWindow()
    {
        if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂定，请不要操作！", "确定");
            return;
        }

        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
            return;
        }

        AnalysisMaterial win = EditorWindow.CreateWindow<AnalysisMaterial>("Analysis - Material");
        if (win == null)
            return;

        win.position = new Rect(300, 100, 500, 600);
        win.Show();
    }

    private static List<Entity> _materials = new List<Entity>();
    //private static Dictionary<Shader, Entity> _materials = new Dictionary<Shader, Entity>();
    private Vector2 _pos = Vector2.zero;

    private bool useAlphaTest = false;

    private void OnGUI()
    {
        if (GUILayout.Button("Analysis"))
        {
            DoAnalysisMaterial();
        }

        if (GUILayout.Button("AnalysisVariants"))
        {
            DoAnalysisVariants();
        }

        if (GUILayout.Button("Clear Material Property"))
        {
            DoClearMaterialProperty();
        }

        if (_materials != null && _materials.Count > 0)
        {
            EditorGUILayout.LabelField("shader count", _materials.Count.ToString());
            _pos = EditorGUILayout.BeginScrollView(_pos);

            useAlphaTest = EditorGUILayout.ToggleLeft("过滤", useAlphaTest);

            for (int i = 0; i < _materials.Count; ++i)
            {
                Entity entity = _materials[i];
                EditorGUILayout.BeginHorizontal();
                entity.isShow = EditorGUILayout.Foldout(entity.isShow, "  ");
                EditorGUILayout.ObjectField(entity.shader, typeof(Shader), false);
                EditorGUILayout.LabelField(entity.materialGroup.Count.ToString());
                //if(string.Equals(entity.shader.name, "Toon/ToonLit") && GUILayout.Button("转换"))
                //{                    
                //    TranslationToonLitToToonLit_Outline(entity);
                //}

                if (string.Equals(entity.shader.name, "Toon/ToonLit_OutLine", StringComparison.Ordinal) && GUILayout.Button("清理keywords"))
                {
                    foreach (var kv in entity.materialGroup)
                    {
                        MaterialGroup materialGroup = kv.Value;
                        for (int k = 0; k < materialGroup.materials.Count; ++k)
                        {
                            Material material = materialGroup.materials[k];

                            if (EditorUtility.DisplayCancelableProgressBar("Clear Material Property", material.name, (float)k / (float)materialGroup.materials.Count))
                            {
                                break;
                            }
                            
                            Toon.ToonLitGUI.SetMaterialKeywords_ToonLit(material);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
                else if (string.Equals(entity.shader.name, "Toon/ToonBuild", StringComparison.Ordinal) && GUILayout.Button("清理keywords"))
                {
                    foreach (var kv in entity.materialGroup)
                    {
                        MaterialGroup materialGroup = kv.Value;
                        for (int k = 0; k < materialGroup.materials.Count; ++k)
                        {
                            Material material = materialGroup.materials[k];

                            if (EditorUtility.DisplayCancelableProgressBar("Clear Material Property", material.name, (float)k / (float)materialGroup.materials.Count))
                            {
                                break;
                            }

                            Toon.ToonBuildGUI.SetMaterialKeywords_ToonBuild(material);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
                else if (string.Equals(entity.shader.name, "Toon/ToonTree", StringComparison.Ordinal) && GUILayout.Button("清理keywords"))
                {
                    foreach (var kv in entity.materialGroup)
                    {
                        MaterialGroup materialGroup = kv.Value;
                        for (int k = 0; k < materialGroup.materials.Count; ++k)
                        {
                            Material material = materialGroup.materials[k];

                            if (EditorUtility.DisplayCancelableProgressBar("Clear Material Property", material.name, (float)k / (float)materialGroup.materials.Count))
                            {
                                break;
                            }

                            Toon.ToonBuildGUI.SetMaterialKeywords_ToonBuild(material);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
                /*
                if (GUILayout.Button("clear"))
                {
                    foreach (var kv in entity.materialGroup)
                    {
                        MaterialGroup materialGroup = kv.Value;
                        for (int k = 0; k < materialGroup.materials.Count; ++k)
                        {
                            Material material = materialGroup.materials[k];

                            if (EditorUtility.DisplayCancelableProgressBar("Clear Material Property", material.name, (float)k / (float)materialGroup.materials.Count))
                            {
                                break;
                            }

                            ClearMaterialProperty(material);
                            if (string.Equals(entity.shader.name, "Custom/AddtiveUVAnim"))// || string.Equals(entity.shader.name, "Custom/SubUVAnim")
                            {
                                BaseShaderGUI.SetupMaterialBlendMode(material);
                                UVAnimGUI.SetMaterialKeywords_AddtiveUVAnim(material);
                            }
                            else if (string.Equals(entity.shader.name, "Custom/AlphaBlended"))
                            {
                                BaseShaderGUI.SetupMaterialBlendMode(material);
                                EffectAlphaGUI.SetMaterialKeywords_AddtiveUVAnim(material);
                            }
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
                */

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                if (entity.isShow)
                {
                    foreach (var kv in entity.materialGroup)
                    {
                        MaterialGroup materialGroup = kv.Value;
                        if (useAlphaTest && !kv.Key.Contains("_ALPHATEST_ON"))
                            continue;

                        EditorGUILayout.BeginHorizontal();                        
                        materialGroup.isShow = EditorGUILayout.Foldout(materialGroup.isShow, kv.Key);
                        EditorGUILayout.LabelField(materialGroup.materials.Count.ToString());
                        EditorGUILayout.EndHorizontal();                        
                        if (materialGroup.isShow)
                        {
                            EditorGUI.indentLevel++;
                            for (int k = 0; k < materialGroup.materials.Count; ++k)
                            {
                                EditorGUILayout.ObjectField(materialGroup.materials[k], typeof(Material), false);                                
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                EditorGUI.indentLevel--;
                // if(GUILayout.Button("Test"))
                // {
                //     ShaderVariantCollection shaderVariantCollection = new ShaderVariantCollection();
                //     ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                //     shaderVariant.shader = entity.shader;
                //     shaderVariantCollection.Add(shaderVariant);
                //     ShaderData shaderData = GetShaderKeywords(shaderVariantCollection, entity.shader);
                //     Debug.Log(shaderData.keyworlds.Count);
                //     Debug.Log(shaderData.passTypes.Length);
                // }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private static string[] _MAIN_LIGHT_SHADOWS = new string[1] { "_MAIN_LIGHT_SHADOWS" };
    private static string[] _SHADOWS_SOFT = new string[1] { "_SHADOWS_SOFT" };
    private static string[] INSTANCING_ON = new string[1] { "INSTANCING_ON" };
    private static string[] LIGHTMAP_ON = new string[1] { "LIGHTMAP_ON" };

    private static string[] _CommonKeyword = new string[] {
    "_MAIN_LIGHT_SHADOWS",
    "_SHADOWS_SOFT",
    "INSTANCING_ON",
    "LIGHTMAP_ON",
    };

    private List<string[]> keys = new List<string[]>(3);

    //readonly ShaderTagId LightMode = new ShaderTagId("LightMode");
    //readonly ShaderTagId ShadowCaster = new ShaderTagId("ShadowCaster");
    //readonly ShaderTagId LightweightForward = new ShaderTagId("LightweightForward");
    //readonly ShaderTagId UniversalForward = new ShaderTagId("UniversalForward");
    //readonly ShaderTagId DepthOnly = new ShaderTagId("DepthOnly");
    //readonly ShaderTagId Meta = new ShaderTagId("Meta");
    //readonly ShaderTagId SRPDefaultUnlit = new ShaderTagId("SRPDefaultUnlit");
    //readonly ShaderTagId NormalsRendering = new ShaderTagId("NormalsRendering");
    //readonly ShaderTagId Universal2D = new ShaderTagId("Universal2D");
    //readonly ShaderTagId Lightweight2D = new ShaderTagId("Lightweight2D");
    //readonly ShaderTagId Picking = new ShaderTagId("Picking");
    //readonly ShaderTagId SceneSelectionPass = new ShaderTagId("SceneSelectionPass");

    public static HashSet<string> GetAlwaysIncludeShaders()
    {
        HashSet<string> shaders = new HashSet<string>();

        var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
        var SerializedObj = new SerializedObject(graphicsSettingsObj);
        var arrayProp = SerializedObj.FindProperty("m_AlwaysIncludedShaders");
        for (int i = 0; i < arrayProp.arraySize; ++i)
        {
            var arrayElem = arrayProp.GetArrayElementAtIndex(i);
            shaders.Add(arrayElem.objectReferenceValue.name);
        }

        return shaders;
    }


    public static void DoAnalysisVariants()
    {
        HashSet<string> shaders = GetAlwaysIncludeShaders();

        ShaderVariantCollection variantCollection = new ShaderVariantCollection();

        int count = _materials.Count;
        int index = 0;

        List<string> keys = new List<string>(8);

        for (int i = 0; i < _materials.Count; ++i)
        {
            Shader shader = _materials[i].shader;
            Entity entity = _materials[i];

            if (shaders.Contains(shader.name))
            {
                continue;
            }

            if (shader.name.StartsWith("UnityChanToonShader", System.StringComparison.Ordinal))
            {
                Debug.LogErrorFormat("跳过了 {0} 的变体收集", shader.name);
                continue;
            }

            ShaderVariantCollection tempShaderVariantCollection = new ShaderVariantCollection();
            ShaderVariantCollection.ShaderVariant tempVariant = new ShaderVariantCollection.ShaderVariant();
            tempVariant.shader = shader;
            tempShaderVariantCollection.Add(tempVariant);

            ShaderData shaderData = GetShaderKeywords(tempShaderVariantCollection, shader);

            int j = 0;
            foreach (string keywordString in entity.materialGroup.Keys)
            {
                ++j;

                if (EditorUtility.DisplayCancelableProgressBar(string.Format("{0}({1}/{2})", shader.name, index, count), keywordString, (float)j / (float)entity.materialGroup.Count))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }                

                for (int passIndex = 0; passIndex < shaderData.passTypes.Length; ++passIndex)
                {
                    UnityEngine.Rendering.PassType passType = (UnityEngine.Rendering.PassType)shaderData.passTypes[passIndex];

                    if (passType == PassType.Meta || passType == PassType.Deferred || passType == PassType.ForwardAdd || passType == PassType.LightPrePassBase || passType == PassType.LightPrePassFinal)
                        continue;

                    string[] keywords = keywordString.Split(';'); //new string[entity.materialGroup[j].shaderKeywords.Length + _CommonKeyword.Length];
                    //Array.Copy(entity.materialGroup[j].shaderKeywords, keywords, entity.materialGroup[j].shaderKeywords.Length);
                    //Array.Copy(_CommonKeyword, 0, keywords, entity.materialGroup[j].shaderKeywords.Length, _CommonKeyword.Length);

                    if (keywords.Length > 0)
                    {
                        keywords = shaderData.keyworlds[passIndex].Intersect(keywords).ToArray();
                    }                   

                    bool isSuccess = true;
                    ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant();
                    try
                    {                        
                        if (keywords.Length > 0)
                        {
                            variant = new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);
                        }
                        else
                        {
                            variant = new ShaderVariantCollection.ShaderVariant(shader, passType);
                        }
                    }
                    catch (Exception e)
                    {
                        isSuccess = false;
                        Debug.LogWarningFormat("{0} | {1} {2}", e.Message, passType.ToString(), string.Join(";", keys));
                        //Debug.LogException(e);
                    }

                    if (isSuccess)
                    {
                        variantCollection.Add(variant);
                    }
                }
            }
            ++index;
        }

        AssetDatabase.CreateAsset(variantCollection, "Assets/ResourcesAB/Shader/SVAuto.shadervariants");
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    /*
    public static void DoAnalysisVariants_Blackup()
    {
        HashSet<string> shaders = GetAlwaysIncludeShaders();

        ShaderVariantCollection variantCollection = new ShaderVariantCollection();

        int count = _materials.Count;
        int index = 0;

        List<string> keys = new List<string>(8);

        for (int i = 0; i < _materials.Count; ++i)
        {
            Shader shader = _materials[i].shader;
            Entity entity = _materials[i];

            if (shaders.Contains(shader.name))
            {
                continue;
            }

            if (shader.name.StartsWith("UnityChanToonShader"))
            {
                Debug.LogErrorFormat("跳过了 {0} 的变体收集", shader.name);
                continue;
            }

            for (int j = 0; j < entity.materialGroup.Count; ++j)
            {
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("{0}({1}/{2})", shader.name, index, count), entity.materialGroup[j].name, (float)j / (float)entity.materialGroup.Count))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }

                for (int passIndex = 0; passIndex < shader.passCount; ++passIndex)
                {
                    UnityEngine.Rendering.PassType passType = UnityEngine.Rendering.PassType.ScriptableRenderPipeline;

                    ShaderTagId tag = shader.FindPassTagValue(passIndex, LightMode);
                    if (tag == LightweightForward || tag == UniversalForward || tag == Universal2D || tag == Lightweight2D || tag == SceneSelectionPass || tag == Picking)
                    {
                        passType = PassType.ScriptableRenderPipeline;
                    }
                    else if (tag == Meta)
                    {
                        passType = PassType.Meta;
                        continue;
                    }
                    else if (tag == SRPDefaultUnlit)
                    {
                        passType = PassType.ScriptableRenderPipelineDefaultUnlit;
                    }
                    else if (tag == ShadowCaster || tag == DepthOnly)
                    {
                        passType = PassType.ShadowCaster;
                    }
                    else
                    {
                        Debug.LogFormat("shader {0} pass {1} tag = {2}", shader.name, passIndex, tag.name);
                        passType = PassType.Normal;
                    }

                    keys.Clear();
                    string[] keywords = entity.materialGroup[j].shaderKeywords;

                    for (int k = 0; k < keywords.Length; ++k)
                    {
                        //                         ShaderKeyword shaderKeyword = new ShaderKeyword(entity.materials[i].shader, keywords[k]);
                        //                         if (string.Equals("_", keywords[k]) 
                        //                             || string.Equals("_EMISSIVE_SIMPLE", keywords[k]) 
                        //                             || string.Equals("_OUTLINE_NML", keywords[k])
                        //                             || string.Equals("_SHADEMAP_OFF", keywords[k])
                        //                             || !shaderKeyword.IsValid())
                        //                         {
                        //                             Debug.LogFormat("shader {4} key {0} KeywordType {1} {2} {3}", keywords[k], ShaderKeyword.GetGlobalKeywordType(shaderKeyword).ToString(), shaderKeyword.index, shaderKeyword.IsValid(), entity.materials[i].shader.name);
                        //                         }
                        //                         else
                        //                         {
                        //                             keys.Add(keywords[k]);
                        //                         }

                        if (string.Equals("_", keywords[k]))
                        {
                            continue;
                        }

                        bool isHas = true;
                        try
                        {
                            ShaderVariantCollection.ShaderVariant v = new ShaderVariantCollection.ShaderVariant(shader, passType, keywords[k]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarningFormat("{0} | {1} {2}", e.Message, passType.ToString(), keywords[k]);
                            isHas = false;
                        }
                        if (isHas)
                        {
                            keys.Add(keywords[k]);
                        }
                    }

                    bool isSuccess = true;
                    ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant();
                    try
                    {
                        variant = new ShaderVariantCollection.ShaderVariant(shader, passType, keys.ToArray());

                    }
                    catch (Exception e)
                    {
                        isSuccess = false;
                        Debug.LogWarningFormat("{0} | {1} {2}", e.Message, passType.ToString(), string.Join(";", keys));
                        //Debug.LogException(e);
                    }

                    if (isSuccess)
                    {
                        variantCollection.Add(variant);
                    }
                }
            }
            ++index;
        }

        AssetDatabase.CreateAsset(variantCollection, "Assets/ResourcesAB/Shader/SVAuto.shadervariants");
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    */
    private static void AnalysisVariants(int index, List<string[]> compileKeyWords, string[] shaderKeywords, ShaderVariantCollection variantCollection, Shader shader, UnityEngine.Rendering.PassType passType)
    {
        string[] keyWords = compileKeyWords[index];

        for (int i = 0; i < keyWords.Length; ++i)
        {
            List<string> keys = new List<string>(shaderKeywords);
            keys.Add(keyWords[i]);
            string[] keysArray = keys.ToArray();

            bool isSuccess = true;
            ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant();
            try
            {
                variant = new ShaderVariantCollection.ShaderVariant(shader, passType, keysArray);
            }
            catch (Exception e)
            {
                isSuccess = false;
                Debug.LogErrorFormat("{0} {1} {2}", shader.name, passType.ToString(), keysArray);
                Debug.LogException(e);
            }

            if (isSuccess)
            {
                variantCollection.Add(variant);
            }

            if (index + 1 < compileKeyWords.Count)
            {
                AnalysisVariants(index + 1, compileKeyWords, keysArray, variantCollection, shader, passType);
            }
        }
    }

    private void OnDisable()
    {
        _materials.Clear();
        System.GC.Collect();
    }

    private void OnDestroy()
    {
        _materials.Clear();
        System.GC.Collect();
    }

    private static string GetMaterialKeywords(Material material, bool joinCull)
    {
        HashSet<string> rlt = new HashSet<string>();
        string[] ss = material.shaderKeywords;

        if (joinCull && material.HasProperty("_Cull"))
        {
            rlt.Add(((BaseShaderGUI.RenderFace)material.GetFloat("_Cull")).ToString());
        }

        if (material.enableInstancing)
        {
            rlt.Add("INSTANCING_ON");
        }

        for (int i = 0; i < ss.Length; ++i)
        {            
            rlt.Add(ss[i]);
        }

        rlt.Remove("_");
        
        return string.Join(";", rlt);
    }

    public static void DoAnalysisMaterial()
    {
        _materials.Clear();

        Dictionary<Shader, Entity> materialDic = new Dictionary<Shader, Entity>();

        string[] ids = AssetDatabase.FindAssets("t:Material");
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar("AnalysisMaterial", path, (float)i / (float)ids.Length))
            {
                break;
            }

            if (path.StartsWith("Packages/", System.StringComparison.Ordinal))
            {
                continue;
            }

            if (path.StartsWith("Assets/Arts/ceshi", System.StringComparison.Ordinal))
            {
                continue;
            }

            if (path.StartsWith("Assets/Designer_Editor", System.StringComparison.Ordinal))
            {
                continue;
            }

            if (path.StartsWith("Assets/GameToolEditor", System.StringComparison.Ordinal))
            {
                continue;
            }

            if (path.StartsWith("Assets/Test", System.StringComparison.Ordinal))
            {
                continue;
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path) as Material;

            Entity entity = null;
            if (!materialDic.TryGetValue(material.shader, out entity))
            {
                entity = new Entity();
                entity.shader = material.shader;
                materialDic.Add(material.shader, entity);                
            }

            string keywords = GetMaterialKeywords(material, true);
            if (!entity.materialGroup.TryGetValue(keywords, out MaterialGroup materialGroup))
            {
                materialGroup = new MaterialGroup();
                entity.materialGroup.Add(keywords, materialGroup);
            }
            materialGroup.materials.Add(material);
        }

        var enumerator = materialDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int i = 0;
            for (; i < _materials.Count; ++i)
            {
                if (string.Compare(enumerator.Current.Value.shader.name, _materials[i].shader.name) < 0)
                {
                    break;
                }
            }
            _materials.Insert(i, enumerator.Current.Value);

            foreach (var kv in enumerator.Current.Value.materialGroup)
            {
                kv.Value.materials.Sort((x, y) => { return string.Compare(x.name, y.name); });
            }
        }

        EditorUtility.ClearProgressBar();
    }

    public static void DoClearMaterialProperty()
    {
        _materials.Clear();

        string[] ids = AssetDatabase.FindAssets("t:Material");
        for (int i = 0; i < ids.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);

            if (EditorUtility.DisplayCancelableProgressBar("Clear Material Property", path, (float)i / (float)ids.Length))
            {
                break;
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path) as Material;

            if (material != null)
            {
                ClearMaterialProperty(material);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/ClearMaterialProperty")]
    private static void ClearMaterialProperty()
    {
        Material material = Selection.activeObject as Material;
        if (material != null)
        {
            ClearMaterialProperty(material);
        }
    }

    private static void ClearMaterialProperty(Material material)
    {
        SerializedObject source = new SerializedObject(material);
        SerializedProperty emission = source.FindProperty("m_SavedProperties");
        SerializedProperty texEnvs = emission.FindPropertyRelative("m_TexEnvs");
        SerializedProperty floats = emission.FindPropertyRelative("m_Floats");
        SerializedProperty colors = emission.FindPropertyRelative("m_Colors");

        ClearMaterialSerializedProperty(material, texEnvs);
        ClearMaterialSerializedProperty(material, floats);
        ClearMaterialSerializedProperty(material, colors);

        source.ApplyModifiedProperties();
        EditorUtility.SetDirty(material);
    }

    private static void ClearMaterialSerializedProperty(Material material, SerializedProperty property)
    {
        for (int i = property.arraySize - 1; i >= 0; --i)
        {
            SerializedProperty s1 = property.GetArrayElementAtIndex(i);
            if (s1 == null)
            {
                Debug.LogErrorFormat("{0} s1 == null", material.name);
                continue;
            }

            SerializedProperty s2 = s1.FindPropertyRelative("first");
            if (s2 == null)
            {
                Debug.LogErrorFormat("{0} first == null", material.name);
                continue;
            }

            string propertyName = s2.stringValue;
            if (!material.HasProperty(propertyName))
            {
                property.DeleteArrayElementAtIndex(i);
            }
        }
    }

    public static void Clear()
    {
        _materials.Clear();
        System.GC.Collect();
    }

    delegate void GetShaderVariantEntriesFiltered(Shader shader, int maxEntries, string[] filterKeywords, ShaderVariantCollection excludeCollection, out int[] passTypes, out string[] keywordLists, out string[] remainingKeywords);
    delegate string[] GetAllGlobalKeywords();
    delegate string[] GetShaderGlobalKeywords(Shader shader);

    //internal static void GetShaderVariantEntriesFiltered(
    //    Shader shader, 
    //    int maxEntries,
    //    string[] filterKeywords,
    //    ShaderVariantCollection excludeCollection,
    //    out int[] passTypes,
    //    out string[] keywordLists,
    //    out string[] remainingKeywords)
    //{     
    //
    //}

    private static MethodInfo _GetShaderVariantEntriesFiltered = null;
    //private static ShaderVariantCollection _shaderVariantCollection = new ShaderVariantCollection();
    private static string[] _filterKeywords = null;// new string[] {};
    private static string[] _filterKeywordsEmpty = new string[] {};

    private static ShaderData GetShaderKeywords(ShaderVariantCollection shaderVariantCollection, Shader shader)
    {
        if (_filterKeywords == null)
        {
            //GetAllGlobalKeywords func = typeof(ShaderUtil).GetMethod("GetAllGlobalKeywords", BindingFlags.NonPublic | BindingFlags.Static).CreateDelegate(typeof(GetAllGlobalKeywords)) as GetAllGlobalKeywords;
            //_filterKeywords = func.Invoke();
            //string s = string.Join(",\n", _filterKeywords);
            //Debug.Log(s);
            _filterKeywords = new string[] {
                "FOG_LINEAR",
                "FOG_EXP",
                "FOG_EXP2",
                "DIRLIGHTMAP_COMBINED",
                "DYNAMICLIGHTMAP_ON",
                "LIGHTMAP_SHADOW_MIXING",
                "SHADOWS_SHADOWMASK",
                "EDITOR_VISUALIZATION",
                "USE_STRUCTURED_BUFFER",
                "SOFTPARTICLES_ON",
                "VERTEXLIGHT_ON",
                "UNITY_HDR_ON",
                "LOD_FADE_CROSSFADE",
                "UNITY_SINGLE_PASS_STEREO",
                "USE_STRUCTURED_BUFFER",
            };            
        }

        if (_GetShaderVariantEntriesFiltered == null)
        {
            _GetShaderVariantEntriesFiltered = typeof(ShaderUtil).GetMethod("GetShaderVariantEntriesFiltered", BindingFlags.NonPublic | BindingFlags.Static);//.CreateDelegate(typeof(GetShaderVariantEntriesFiltered)) as GetShaderVariantEntriesFiltered; ;
        }

        int[] passTypes = new int[] { };
        string[] keywords = new string[] { };
        string[] remainingKeywords = new string[] { };


        object[] args = new object[]
        {
            shader,
            -1,
            _filterKeywords,
            shaderVariantCollection,
            passTypes,
            keywords,
            remainingKeywords
        };

        ParameterInfo[] parameterInfos = _GetShaderVariantEntriesFiltered.GetParameters();

        _GetShaderVariantEntriesFiltered.Invoke(null, args);   
            
        //_GetShaderVariantEntriesFiltered(shader, 256, _filterKeywords, _shaderVariantCollection, out passTypes, out keywords, out remainingKeywords);        

        ShaderData shaderData = new ShaderData();
        shaderData.passTypes = args[4] as int[];
        if (shaderData.passTypes.Length > 0)
        {
            var kws = args[5] as string[];
            shaderData.keyworlds = new List<List<string>>(kws.Length);
            for (int i = 0; i < kws.Length; ++i)
            {
                shaderData.keyworlds.Add(new List<string>(kws[i].Split(' ')));
            }
        }
        else
        {
            object[] args2 = new object[]
            {
                shader,
                -1,
                _filterKeywordsEmpty,
                shaderVariantCollection,
                passTypes,
                keywords,
                remainingKeywords
            };

            _GetShaderVariantEntriesFiltered.Invoke(null, args2);

            shaderData.passTypes = args2[4] as int[];
            var kws = args2[5] as string[];
            shaderData.keyworlds = new List<List<string>>(kws.Length);
            for (int i = 0; i < kws.Length; ++i)
            {
                shaderData.keyworlds.Add(new List<string>(kws[i].Split(' ')));
            }
        }
        
        return shaderData;
    }

    private void TranslationToonLitToToonLit_Outline(Entity entity)
    {
        Shader toShader = Shader.Find("Toon/ToonLit_OutLine");

        foreach (MaterialGroup group in entity.materialGroup.Values)
        {
            for (int i = 0; i < group.materials.Count; ++i)
            {
                Material material = group.materials[i];
                material.shader = toShader;
                material.SetShaderPassEnabled("Outline", false);
            }
        }
    }    
}
