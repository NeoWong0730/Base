using Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ExportHumanPrefabWindow : EditorWindow
{
    [MenuItem("Tools/ExportHumanPrefab")]
    public static void OpenWindow()
    {
        ExportHumanPrefabWindow window = GetWindow<ExportHumanPrefabWindow>();
        window.Show();
    }

    [System.Serializable]
    public class ModelEntry
    {
        public bool bSelect;
        public string sAssetPath;
    }
    
    private bool bExportSkeleton;
    private bool bExportFaceMale;
    private bool bExportFaceFamale;

    private Vector2 vClothScrollPosition;
    private Vector2 vHairScrollPosition;
    private List<ModelEntry> mClothFiles;
    private List<ModelEntry> mHairFiles;

    private void OnEnable()
    {
        string[] clothGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { ExportHumanPrefab.sClothPath });
        mClothFiles = new List<ModelEntry>(clothGUIDs.Length);
        for (int i = 0; i < clothGUIDs.Length; ++i)
        {
            mClothFiles.Add(new ModelEntry() { sAssetPath = AssetDatabase.GUIDToAssetPath(clothGUIDs[i]), bSelect = false });
        }

        string[] hairGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { ExportHumanPrefab.sHairPath });
        mHairFiles = new List<ModelEntry>(hairGUIDs.Length);
        for (int i = 0; i < hairGUIDs.Length; ++i)
        {
            mHairFiles.Add(new ModelEntry() { sAssetPath = AssetDatabase.GUIDToAssetPath(hairGUIDs[i]), bSelect = false });
        }
    }    

    private void OnDisable()
    {
        mClothFiles.Clear();
        mClothFiles = null;

        mHairFiles.Clear();
        mHairFiles = null;
    }

    private void DrawModelList(List<ModelEntry> modelEntries, ref Vector2 scrollPosition)
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(EditorGUIUtility.standardVerticalSpacing + (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * Mathf.Min(10, modelEntries.Count)));
        for(int i = 0; i < modelEntries.Count; ++i)
        {
            modelEntries[i].bSelect = EditorGUILayout.ToggleLeft(modelEntries[i].sAssetPath, modelEntries[i].bSelect);
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Skeleton");
        bExportSkeleton = EditorGUILayout.ToggleLeft(ExportHumanPrefab.sHumanSkeletonFile, bExportSkeleton);
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Face");
        bExportFaceMale = EditorGUILayout.ToggleLeft(ExportHumanPrefab.sHumanMaleFile, bExportFaceMale);
        bExportFaceFamale = EditorGUILayout.ToggleLeft(ExportHumanPrefab.sHumanFamaleFile, bExportFaceFamale);
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Cloth");
        DrawModelList(mClothFiles, ref vClothScrollPosition);
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Hair");
        DrawModelList(mHairFiles, ref vHairScrollPosition);
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (GUILayout.Button("Export"))
        {
            if (bExportSkeleton)
            {
                string inputFile = ExportHumanPrefab.sHumanSkeletonFile;
                string inputFileName = Path.GetFileNameWithoutExtension(inputFile);
                string outputFile = $"{ExportHumanPrefab.sHumanOutputPath}/{inputFileName}.prefab";
                ExportHumanPrefab.ExportHumanSkeletonPrefab(inputFile, outputFile);
            }

            GameObject skeleton = AssetDatabase.LoadAssetAtPath<GameObject>(ExportHumanPrefab.sHumanSkeletonFile);
            Transform[] skeletonBones = skeleton.GetComponentsInChildren<Transform>();
            Dictionary<string, Transform> skeletonBoneMap = new Dictionary<string, Transform>(skeletonBones.Length);
            Dictionary<string, string> skeletonBonePathMap = new Dictionary<string, string>(skeletonBones.Length);
            for (int i = 0; i < skeletonBones.Length; ++i)
            {
                skeletonBoneMap.TryAdd(skeletonBones[i].name, skeletonBones[i]);
                skeletonBonePathMap.TryAdd(skeletonBones[i].name, ExportHumanPrefab.GetBonePath(skeleton.transform, skeletonBones[i]));
            }

            if (bExportFaceMale)
            {
                string inputFile = ExportHumanPrefab.sHumanMaleFile;
                string inputFileName = Path.GetFileNameWithoutExtension(inputFile);
                string outputFile = $"{ExportHumanPrefab.sHumanOutputPath}/face_{inputFileName}.prefab";
                ExportHumanPrefab.ExportHumanFacePrefab(inputFile, outputFile, skeletonBoneMap, skeletonBonePathMap);
            }

            if (bExportFaceFamale)
            {
                string inputFile = ExportHumanPrefab.sHumanFamaleFile;
                string inputFileName = Path.GetFileNameWithoutExtension(inputFile);
                string outputFile = $"{ExportHumanPrefab.sHumanOutputPath}/face_{inputFileName}.prefab";
                ExportHumanPrefab.ExportHumanFacePrefab(inputFile, outputFile, skeletonBoneMap, skeletonBonePathMap);
            }

            for (int i = 0; i < mClothFiles.Count; ++i)
            {
                if (mClothFiles[i].bSelect)
                {
                    string inputFile = mClothFiles[i].sAssetPath;
                    string inputFileName = Path.GetFileNameWithoutExtension(inputFile);
                    string outputFile = $"{ExportHumanPrefab.sClothOutputPath}/{inputFileName}.prefab";
                    ExportHumanPrefab.ExportHumanClothPrefab(inputFile, outputFile, skeletonBoneMap, skeletonBonePathMap);
                }
            }

            for (int i = 0; i < mHairFiles.Count; ++i)
            {
                if (mHairFiles[i].bSelect)
                {
                    string inputFile = mHairFiles[i].sAssetPath;
                    string inputFileName = Path.GetFileNameWithoutExtension(inputFile);
                    string outputFile = $"{ExportHumanPrefab.sHairOutputPath}/{inputFileName}.prefab";
                    ExportHumanPrefab.ExportHumanHairPrefab(inputFile, outputFile, skeletonBoneMap, skeletonBonePathMap);
                }
            }

            GenerationModelConfig();
        }
    }

    public static void GenerationModelConfig()
    {
        string[] clothGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[1] { ExportHumanPrefab.sClothOutputPath });
        string[] hairGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[1] { ExportHumanPrefab.sHairOutputPath });

        HumanPartConfig config = AssetDatabase.LoadAssetAtPath<HumanPartConfig>("Assets/Settings/HumanPartConfig.asset");
        if (!config)
        {
            config = ScriptableObject.CreateInstance<HumanPartConfig>();
            AssetDatabase.CreateAsset(config, "Assets/Settings/HumanPartConfig.asset");
        }

        config.mSkeletons = new string[] { "skeleton_common" };
        config.mFaces = new string[] { null, "face_male", "face_famale" };        
        config.mCloths = new string[clothGUIDs.Length + 1];
        config.mHairs = new string[hairGUIDs.Length + 1];

        for (int i = 0; i < clothGUIDs.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(clothGUIDs[i]);
            config.mCloths[i + 1] = Path.GetFileNameWithoutExtension(path);
        }

        for (int i = 0; i < hairGUIDs.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(hairGUIDs[i]);
            config.mHairs[i + 1] = Path.GetFileNameWithoutExtension(path);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssetIfDirty(config);
        AssetDatabase.Refresh();
    }    
}
