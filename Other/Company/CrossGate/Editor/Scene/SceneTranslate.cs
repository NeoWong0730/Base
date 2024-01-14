using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.Mathematics;
using System.IO;
using Unity.Collections;
using UnityEngine.Rendering;
using Lib.Core;

public class SceneTranslate : EditorWindow
{
    [MenuItem("__Tools__/SceneTranslate")]
    public static void Create()
    {
        SceneTranslate window = EditorWindow.CreateWindow<SceneTranslate>();
        window.Show();
    }

    [MenuItem("__Tools__/检修场景昼夜系统配置")]
    public static void CheckDayAndNightProfile()
    {
        int rlt = EditorUtility.DisplayDialogComplex("检修场景昼夜系统配置", "该过程需要较长时间", "检修", "取消", "取消");
        if (0 != rlt)
        {
            return;
        }

        string[] sceneNames = AssetDatabase.FindAssets(sceneFilter, new string[] { rootPath, targetPath });
        if (sceneNames == null)
            return;

        for (int i = 0; i < sceneNames.Length; ++i)
        {
            string sceneName = AssetDatabase.GUIDToAssetPath(sceneNames[i]);
            if (EditorUtility.DisplayCancelableProgressBar("检查并修复场景天气配置", sceneName, (float)i / sceneNames.Length))
            {
                break;
            }

            Scene scene = EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Single);
            if (scene == null)
                continue;

            GameObject[] roots = scene.GetRootGameObjects();
            for (int j = roots.Length - 1; j >= 0; --j)
            {
                Transform transform = roots[j].transform;

                if (transform.name.Equals("Root"))
                {
                    Transform root = transform.Find("DayAndNight");
                    if (root != null && root.TryGetComponent<DayAndNightDynamic>(out DayAndNightDynamic dayAndNightDynamic))
                    {
                        if (dayAndNightDynamic.mProfile == null)
                        {
                            Debug.LogErrorFormat("{0} 修复丢失的天气配置", sceneName);
                            dayAndNightDynamic.mProfile = AssetDatabase.LoadAssetAtPath<DayAndNightProfile>("Assets/Settings/DayAndNightProfile.asset");                            
                            EditorSceneManager.SaveScene(scene, sceneName);
                        }
                    }

                    break;
                }
            }            
        }

        EditorUtility.ClearProgressBar();
    }

    public class SceneBlockData
    {
        [SerializeField]
        public int boundIndex;
        [SerializeField]
        public List<GameObjectData> datas;
    }

    const string sceneFilter = "t:scene";
    const string rootPath = "Assets/ResourcesAB/Scene";
    const string targetPath = "Assets/ResourcesAB/SceneExport";

    string srcPath;

    const int gPriorityCount = 3;
    Dictionary<string, HashSet<Transform>>[] prefabAssets = new Dictionary<string, HashSet<Transform>>[gPriorityCount];
    HashSet<MeshRenderer>[] meshRenderers = new HashSet<MeshRenderer>[gPriorityCount];

    List<SceneBlockData[]> blockDatas = new List<SceneBlockData[]>(gPriorityCount);
    List<GameObject> unuseGameObjects = new List<GameObject>();
    List<Component> unuseComponents = new List<Component>();

    string[] sceneNames = null;
    bool[] sceneToggle = null;
    int index = 0;
    bool selected;
    Vector2 pos;
    //HashSet<string> needs = new HashSet<string>() { "tree", "small", "widget", "Fx ", "Ani", "house", "water", "light" };

    private NativeArray<float2> mPositionOffsets;

    string srcScenePath;

    private void OnEnable()
    {
        sceneNames = AssetDatabase.FindAssets(sceneFilter, new string[] { rootPath });
        if (sceneNames != null)
        {
            for (int i = 0; i < sceneNames.Length; ++i)
            {
                sceneNames[i] = AssetDatabase.GUIDToAssetPath(sceneNames[i]);
            }
            sceneToggle = new bool[sceneNames.Length];
        }
    }

    private void OnGUI()
    {
        srcPath = GUILayout.TextField(srcPath);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("全选"))
        {
            for (int i = 0; i < sceneToggle.Length; ++i)
            {
                sceneToggle[i] = true;
            }
        }

        if (GUILayout.Button("全不选"))
        {
            for (int i = 0; i < sceneToggle.Length; ++i)
            {
                sceneToggle[i] = false;
            }
        }

        GUILayout.EndHorizontal();

        pos = EditorGUILayout.BeginScrollView(pos);
        if (sceneNames != null)
        {
            for (int i = 0; i < sceneNames.Length; ++i)
            {
                if(string.IsNullOrWhiteSpace(srcPath) || sceneNames[i].Contains(srcPath))
                {
                    sceneToggle[i] = EditorGUILayout.ToggleLeft(sceneNames[i], sceneToggle[i]);
                }                
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Translate"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            srcScenePath = EditorSceneManager.GetActiveScene().path;

            if (sceneNames != null)
            {
                for (int i = 0; i < sceneNames.Length; ++i)
                {
                    if (sceneToggle[i])
                    {
                        if (EditorUtility.DisplayCancelableProgressBar(string.Format("正在转化场景 {0}/{1}", i.ToString(), sceneNames.Length.ToString()), sceneNames[i], (float)i / sceneNames.Length))
                        {
                            break;
                        }
                        ExportOneScene(sceneNames[i], targetPath);
                    }
                }
                EditorUtility.ClearProgressBar();
            }

            EditorSceneManager.OpenScene(srcScenePath);
        }

        if (GUILayout.Button("Clear"))
        {
            ClearExportScene(sceneNames);            
        }
    }

    private void ClearExportScene(string[] srcScenes)
    {
        HashSet<string> hasScenes = new HashSet<string>();
        for (int i = 0; i < sceneNames.Length; ++i)
        {
            hasScenes.Add(sceneNames[i].Replace("/Scene/", "/SceneExport/"));
        }

        string[] scenes = AssetDatabase.FindAssets(sceneFilter, new string[] { targetPath });
        if (scenes == null)
            return;

        for (int i = 0; i < scenes.Length; ++i)
        {
            string scene = AssetDatabase.GUIDToAssetPath(scenes[i]);
            if (EditorUtility.DisplayCancelableProgressBar(string.Format("正在清理场景 {0}/{1}", i.ToString(), scenes.Length.ToString()), scene, (float)i / scenes.Length))
            {
                break;
            }
            
            if (!hasScenes.Contains(scene))
            {
                AssetDatabase.DeleteAsset(scene);
                string folder = scene.Remove(scene.Length - Path.GetExtension(scene).Length);
                AssetDatabase.DeleteAsset(folder);
                Debug.Log(scene);
                Debug.Log(folder);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private void ExportOneScene(string srcPath, string detPath)
    {        
        string det = Path.Combine(detPath, Path.GetFileName(srcPath));

        string fileName = Path.GetFileNameWithoutExtension(det);
        string filePath = Path.Combine(Path.GetDirectoryName(det), fileName);
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        //拷贝场景
        //AssetDatabase.CopyAsset(srcPath, det); 
        //File.Copy(Application.dataPath + "/../" + srcPath, Application.dataPath + "/../" + det, true);

        //使用另存为的方式，不会重新生成meta
        Scene srcScene = EditorSceneManager.OpenScene(srcPath, OpenSceneMode.Single);
        EditorSceneManager.SaveScene(srcScene, det, true);        

        //打开导出的场景
        Scene scene = EditorSceneManager.OpenScene(det, OpenSceneMode.Single);        
        EditorSceneManager.SaveScene(scene);

        string sceneName = scene.name;

        //删除所有的非root下的节点，并找出root节点
        Transform rootTransform = null;
        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = roots.Length - 1; i >= 0; --i)
        {
            Transform transform = roots[i].transform;

            if (transform.name.Equals("Root", System.StringComparison.Ordinal))
            {
                rootTransform = transform;
            }
            else
            {
                unuseGameObjects.Add(transform.gameObject);
            }
        }

        //不存在root的是制作方式不对的
        if (rootTransform == null)
        {
            Debug.LogErrorFormat("场景{0}没有Root节点", sceneName);
            Lightmapping.lightingDataAsset = null;
            return;
        }

        //空场景警告
        int childCount = rootTransform.childCount;
        if (childCount <= 0)
        {
            Debug.LogAssertionFormat("场景{0} Root节点下没有内容", sceneName);
        }

        //场景信息处理=========>
        DayAndNightDynamic dayAndNightDynamic = null;
        Terrain terrain = null;
        BatchRender batchRender = null;

        for (int i = 0; i < childCount; ++i)
        {
            Transform transform = rootTransform.GetChild(i);
            string nodeName = transform.name;

            if (string.Equals(nodeName, "Bounds", System.StringComparison.Ordinal))
            {
                unuseGameObjects.Add(transform.gameObject);
                continue;
            }

            if (!transform.gameObject.activeSelf)
            {
                unuseGameObjects.Add(transform.gameObject);
                continue;
            }

            if (string.Equals(nodeName, "Terrain", System.StringComparison.Ordinal))
            {
                terrain = transform.GetComponent<Terrain>();
                batchRender = transform.GetComponent<BatchRender>();
                continue;
            }

            if (string.Equals(nodeName, "ClickCollider", System.StringComparison.Ordinal))
            {
                MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
                MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
                unuseComponents.Add(meshRenderer);
                unuseComponents.Add(meshFilter);
                continue;
            }

            if (string.Equals(nodeName, "DayAndNight", System.StringComparison.Ordinal))
            {
                if (transform.TryGetComponent<DayAndNightDynamic>(out dayAndNightDynamic))
                {
                    if (dayAndNightDynamic.mProfile == null)
                    {
                        Debug.LogErrorFormat("{0} 修复丢失的天气配置", sceneName);
                        dayAndNightDynamic.mProfile = AssetDatabase.LoadAssetAtPath<DayAndNightProfile>("Assets/Settings/DayAndNightProfile.asset");
                    }
                }
                continue;
            }

            ExportOneNode(transform, sceneName);
        }

        //统计需要BatchRender的内容
        List<RenderData> renderDatas = new List<RenderData>();
        List<Matrix4x4> matrix4x4s = new List<Matrix4x4>();
        List<AABB> aabbs = new List<AABB>();
        int count = TranslateBatchPrefab(renderDatas, matrix4x4s, aabbs);
        if (count > 0)
        {
            SceneBatchRenderer sceneBatchRenderer = rootTransform.GetNeedComponent<SceneBatchRenderer>();
            sceneBatchRenderer.mRenderDatas = renderDatas.ToArray();
            sceneBatchRenderer.mMatrix4X4s = matrix4x4s.ToArray();
            sceneBatchRenderer.mBounds = aabbs.ToArray();
        }

        //清理没用的节点以及脚本
        for (int i = 0; i < unuseGameObjects.Count; ++i)
        {
            if (unuseGameObjects[i] != null)
            {
                DestroyImmediate(unuseGameObjects[i]);
            }
        }

        for (int i = 0; i < unuseComponents.Count; ++i)
        {
            DestroyImmediate(unuseComponents[i]);
        }

        unuseGameObjects.Clear();
        unuseComponents.Clear();

        //清理
        for (int i = 0; i < prefabAssets.Length; ++i)
        {
            prefabAssets[i]?.Clear();
            meshRenderers[i]?.Clear();
        }        

        //清理没有子节点的节点
        childCount = rootTransform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            DestroyUnContentNode(rootTransform.GetChild(i));
        }

        blockDatas.Clear();
        
        //地形处理
        if (terrain != null)
        {
            //统计草
            if (batchRender != null)
            {
                Vector3 extents = terrain.terrainData.size / 2f;
                extents.y = 2f;
                Vector3 center = terrain.GetPosition() + extents;

                AABB bounds = new AABB();
                bounds.Center = center;
                extents.y = 1;
                bounds.Extents = extents;

                int3 size = new int3(); //(int3)math.ceil(bounds.Size / 8);
                size = batchRender.vQTreeSize;

                QTree qTree = new QTree();
                qTree.SetSize(bounds, size, 5);

                SceneInstanceData sceneInstanceData = new SceneInstanceData();
                BakeGrassData(terrain, batchRender.mRenderDatas, batchRender.maxDensity, qTree, sceneInstanceData);

                string instanceDataPath = det.Replace(".unity", "/" + fileName + "_InstanceData.asset");
                SceneInstanceData sceneInstanceDataSave = AssetDatabase.LoadAssetAtPath<SceneInstanceData>(instanceDataPath);
                if (sceneInstanceDataSave)
                {
                    EditorUtility.CopySerialized(sceneInstanceData, sceneInstanceDataSave);
                    EditorUtility.SetDirty(sceneInstanceDataSave);
                }
                else
                {
                    sceneInstanceDataSave = sceneInstanceData;
                    AssetDatabase.CreateAsset(sceneInstanceDataSave, instanceDataPath);
                }

                SceneInstanceRender sceneInstanceRender = rootTransform.GetNeedComponent<SceneInstanceRender>();
                sceneInstanceRender.mip = 5;
                sceneInstanceRender.bounds = bounds;
                sceneInstanceRender.gridSize = size;
                sceneInstanceRender.sInstanceDataPath = fileName + "_InstanceData";

                DestroyImmediate(batchRender);
            }

            string terrainDataPath = det.Replace(".unity", "/" + fileName + "_TerrainData.asset");

            TerrainData terrainDataSave = AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataPath);
            if (terrainDataSave)
            {
                EditorUtility.CopySerialized(terrain.terrainData, terrainDataSave);
            }
            else
            {
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(terrain.terrainData), terrainDataPath);
                terrainDataSave = AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataPath);
            }

            terrainDataSave.detailPrototypes = null;
            terrainDataSave.treePrototypes = null;
            terrainDataSave.RefreshPrototypes();
            terrainDataSave.SetDetailResolution(0, 8);

            terrain.terrainData = terrainDataSave;
            terrain.bakeLightProbesForTrees = false;
            terrain.deringLightProbesForTrees = false;            
            terrain.drawInstanced = true;
            terrain.drawTreesAndFoliage = false;

            TerrainCollider terrainCollider = terrain.GetComponent<TerrainCollider>();
            if (terrainCollider != null)
            {
                terrainCollider.terrainData = terrainDataSave;
            }

            EditorUtility.SetDirty(terrainDataSave);
            AssetDatabase.SaveAssets();
        }

        //统计昼夜系统 灯光
        DayAndNightTool.CollectLightMapMesh();

        //统计lightMap
        if (LightmapSettings.lightmaps != null && LightmapSettings.lightmaps.Length > 0)
        {
            RendererLightMapInfo[] rendererLightMapInfos = CollectRendererLightMapInfo(rootTransform.gameObject);
            TerrainLightMapInfo[] terrainLightMapInfos = CollectTerrainLightMapInfo(rootTransform.gameObject);

            if (rendererLightMapInfos != null || terrainLightMapInfos != null)
            {
                LightMapAsset lightMapAsset = CreateLightMapAsset(LightmapSettings.lightmaps);
                string lightMapAssetPath = det.Replace(".unity", "/" + fileName + "_LightMapData.asset");

                LightMapAsset saveLightMapAsset = AssetDatabase.LoadAssetAtPath<LightMapAsset>(lightMapAssetPath);
                if (saveLightMapAsset)
                {
                    EditorUtility.CopySerialized(lightMapAsset, saveLightMapAsset);
                    EditorUtility.SetDirty(saveLightMapAsset);
                }
                else
                {
                    saveLightMapAsset = lightMapAsset;
                    AssetDatabase.CreateAsset(saveLightMapAsset, lightMapAssetPath);
                    saveLightMapAsset = AssetDatabase.LoadAssetAtPath<LightMapAsset>(lightMapAssetPath);
                }
                AssetDatabase.SaveAssets();

                //GameObject gameObject = new GameObject("SceneLightMapData", typeof(SceneLightMapData));
                //gameObject.transform.SetParent(rootTransform);
                //gameObject.transform.SetSiblingIndex(0);
                SceneLightMapData sceneLightMapData = rootTransform.GetNeedComponent<SceneLightMapData>();
                sceneLightMapData.mRendererLightMapInfos = rendererLightMapInfos;
                sceneLightMapData.mTerrainLightMapInfos = terrainLightMapInfos;
                //sceneLightMapData.mLightMapData = saveLightMapAsset;
                sceneLightMapData.mLightMapDataAddress = fileName + "_LightMapData";
                sceneLightMapData.mLightMapData = null;
            }
        }

        Lightmapping.lightingDataAsset = null;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
    
    private LightMapAsset CreateLightMapAsset(LightmapData[] lightmapDatas)
    {
        LightMapAsset lightMapAsset = new LightMapAsset();
        lightMapAsset.lightmapColor = new Texture2D[lightmapDatas.Length];
        lightMapAsset.lightmapDir = new Texture2D[lightmapDatas.Length];
        lightMapAsset.shadowMask = new Texture2D[lightmapDatas.Length];
        
        for (int i = 0; i < lightmapDatas.Length; ++i)
        {
            if (lightmapDatas[i].lightmapColor)
            {             
                lightMapAsset.lightmapColor[i] = lightmapDatas[i].lightmapColor;
            }

            if (lightmapDatas[i].lightmapDir)
            {                
                lightMapAsset.lightmapDir[i] = lightmapDatas[i].lightmapDir;
            }

            if (lightmapDatas[i].shadowMask)
            {                
                lightMapAsset.shadowMask[i] = lightmapDatas[i].shadowMask;
            }
        }

        return lightMapAsset;
    }
    private RendererLightMapInfo[] CollectRendererLightMapInfo(GameObject root)
    {
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);// GameObject.FindObjectsOfType<MeshRenderer>();
        if (renderers == null && renderers.Length < 1)
            return null;

        List<RendererLightMapInfo> infos = new List<RendererLightMapInfo>(renderers.Length);

        for (int i = 0; i < renderers.Length; ++i)
        {
            MeshRenderer renderer = renderers[i];
            if ((int)(GameObjectUtility.GetStaticEditorFlags(renderer.gameObject) & StaticEditorFlags.ContributeGI) != 0
                && renderer.receiveGI == ReceiveGI.Lightmaps
                && renderer.lightmapIndex > -1)
            {
                RendererLightMapInfo info = new RendererLightMapInfo();
                info.mRenderer = renderer;
                info.nLightmapIndex = renderer.lightmapIndex;
                info.vLightmapScaleOffset = renderer.lightmapScaleOffset;

                infos.Add(info);
            }

            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        }

        if (infos.Count < 1)
            return null;

        return infos.ToArray();
    }
    private TerrainLightMapInfo[] CollectTerrainLightMapInfo(GameObject root)
    {
        Terrain[] terrains = root.GetComponentsInChildren<Terrain>(true);// GameObject.FindObjectsOfType<MeshRenderer>();
        if (terrains == null && terrains.Length < 1)
            return null;

        List<TerrainLightMapInfo> infos = new List<TerrainLightMapInfo>(terrains.Length);

        for (int i = 0; i < terrains.Length; ++i)
        {
            Terrain terrain = terrains[i];
            if ((int)(GameObjectUtility.GetStaticEditorFlags(terrain.gameObject) & StaticEditorFlags.ContributeGI) != 0                
                && terrain.lightmapIndex > -1)
            {
                TerrainLightMapInfo info = new TerrainLightMapInfo();
                info.mTerrain = terrain;
                info.nLightmapIndex = terrain.lightmapIndex;
                info.vLightmapScaleOffset = terrain.lightmapScaleOffset;

                infos.Add(info);
            }

            terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        }

        if (infos.Count < 1)
            return null;

        return infos.ToArray();
    }

    private void DestroyUnContentNode(Transform transform)
    {
        if (!transform.gameObject.activeSelf)
        {
            DestroyImmediate(transform.gameObject);
            return;
        }

        Component[] components = transform.gameObject.GetComponents<Component>();
        int componentCount = components.Length;

        if (componentCount > 1)
            return;

        int childCount = transform.childCount;
        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; --i)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject))
                {
                    PrefabUtility.UnpackPrefabInstance(transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                }

                DestroyUnContentNode(transform.GetChild(i));
            }
        }

        if (transform.childCount <= 0)
        {
            DestroyImmediate(transform.gameObject);
        }
    }

    private void TranslatePrefab(SceneGameObjectData sceneGameObjectData, QTree qTree, string detPath)
    {
        for (int i = 0; i < gPriorityCount; ++i)
        {
            blockDatas.Add(new SceneBlockData[qTree.vSize.x * qTree.vSize.y * qTree.vSize.z]);
        }

        int nodeCount = 0;
        int gameObjectCount = 0;

        int pathCount = 0;
        for (int i = 0; i < gPriorityCount; ++i)
        {
            Dictionary<string, HashSet<Transform>> transformDic = prefabAssets[i];
            if (transformDic != null)
            {
                pathCount += transformDic.Count;
            }
        }

        sceneGameObjectData.prefabPaths = new string[pathCount];

        int assetIndex = 0;
        for (int i = 0; i < gPriorityCount; ++i)
        {
            SceneBlockData[] sceneBlockDatas = blockDatas[i];

            Dictionary<string, HashSet<Transform>> transformDic = prefabAssets[i];
            if (transformDic == null)
                continue;

            foreach (var kv in transformDic)
            {
                sceneGameObjectData.prefabPaths[assetIndex] = kv.Key.Replace('\\', '/').Remove(0, "Assets/ResourcesAB/".Length);

                HashSet<Transform> transforms = kv.Value;
                foreach (Transform transform in transforms)
                {
                    if (transform == null)
                        continue;

                    int gridIndex = qTree.GetIndexByPosition(transform.position);
                    if (gridIndex >= sceneBlockDatas.Length)
                    {
                        Debug.LogErrorFormat("{0} 超出边界", transform.name);
                        continue;
                    }

                    SceneBlockData sceneBlockData = sceneBlockDatas[gridIndex];
                    if (sceneBlockData == null)
                    {
                        sceneBlockData = new SceneBlockData();
                        sceneBlockDatas[gridIndex] = sceneBlockData;
                        sceneBlockData.boundIndex = gridIndex;
                        sceneBlockData.datas = new List<GameObjectData>();

                        ++nodeCount;
                    }

                    GameObjectData gameObjectData = new GameObjectData();
                    gameObjectData.prefabPathIndex = assetIndex;
                    gameObjectData.position = transform.position;
                    gameObjectData.rotation = transform.rotation;
                    gameObjectData.scale = transform.lossyScale;

                    sceneBlockData.datas.Add(gameObjectData);
                    ++gameObjectCount;
                }

                ++assetIndex;
            }
        }

        BakeGameObject(blockDatas, qTree, sceneGameObjectData, gameObjectCount, nodeCount);
    }

    private void ExportOneNode(Transform transform, string sceneName)
    {
        string nodeName = transform.gameObject.name;

        //不需要导出,我了方便起见烘培了lightmap的都先不导出
        if (!nodeName.StartsWith("_"))
            return;

        int rootChildCount = transform.childCount;
        if (rootChildCount <= 0)
        {
            unuseGameObjects.Add(transform.gameObject);
            return;
        }

        //获取优先级
        int index = nodeName.IndexOf('_', 1);
        string s = index < 0 ? null : nodeName.Substring(1, index - 1);
        int priority = 0;
        if (!int.TryParse(s, out priority) || priority < 0 || priority >= gPriorityCount)
        {
            Debug.LogErrorFormat("场景{0} 需要导出的节点{1} 优先级信息设置错误", sceneName, nodeName);
            return;
        }

        for (int i = 0; i < rootChildCount; ++i)
        {
            ExportGameObject(transform.GetChild(i), priority);
        }
    }

    private void ExportGameObject(Transform transform, int priority)
    {
        if(!transform.gameObject.activeSelf)
        {
            unuseGameObjects.Add(transform.gameObject);
            return;
        }

        bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject);        
        bool isLoadAsset = false;

        if(transform.TryGetComponent<Collider>(out Collider collider))
        {
            unuseComponents.Add(collider);

            if (isPrefab)
            {
                PrefabUtility.UnpackPrefabInstance(transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                isPrefab = false;
            }
        }

        if (transform.TryGetComponent<SceneNodeSplit>(out SceneNodeSplit sceneNodeSplit))
        {
            unuseComponents.Add(sceneNodeSplit);

            if (isPrefab)
            {
                PrefabUtility.UnpackPrefabInstance(transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                isPrefab = false;
            }

            if (sceneNodeSplit.splitDatas != null)
            {
                for (int i = 0; i < sceneNodeSplit.splitDatas.Length; ++i)
                {
                    SceneNodeSplit.SplitData child = sceneNodeSplit.splitDatas[i];
                    if (child.transform == null)
                    {
                        continue;
                    }

                    if (child.pProcessType == SceneNodeSplit.EProcessType.Export)
                    {
                        ExportGameObject(child.transform, priority);
                    }
                }
                return;
            }
        }

        string prefabPath = null;
        if (isPrefab)
        {
            prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(transform.gameObject);            
            isLoadAsset = prefabPath.StartsWith("Assets/ResourcesAB");
        }

        //如果预制体是在主动加载的资源目录 则使用异步加载的方案
        if (isLoadAsset)
        {
            //HashSet<Transform> transforms;
            //if (prefabAssets[priority] == null)
            //{
            //    prefabAssets[priority] = new Dictionary<string, HashSet<Transform>>();
            //}
            //if (!prefabAssets[priority].TryGetValue(prefabPath, out transforms))
            //{
            //    transforms = new HashSet<Transform>();
            //    prefabAssets[priority].Add(prefabPath, transforms);
            //}
            //transforms.Add(transform);
            //
            //unuseGameObjects.Add(transform.gameObject);
            return;
        }
        
        if(isPrefab)
        {
            PrefabUtility.UnpackPrefabInstance(transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        }

        MeshRenderer renderer = transform.GetComponent<MeshRenderer>();
        if (meshRenderers[priority] == null)
            meshRenderers[priority] = new HashSet<MeshRenderer>();

        if (renderer != null && renderer.enabled)
        {
            meshRenderers[priority].Add(renderer);
        }

        //如果还有子节点的话 就仅仅删除组件
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; ++i)
            {
                ExportGameObject(transform.GetChild(i), priority);
            }

            if (renderer != null)
            {
                unuseComponents.Add(renderer);
                if (transform.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                {
                    unuseComponents.Add(meshFilter);
                }
            }
        }
        else
        {
            if (renderer != null)
            {
                unuseGameObjects.Add(renderer.gameObject);         
            }                
        }        
    }

    private void BakeGrassData(Terrain terrain, List<InstancingRenderData> renderDatas, int maxDensity, QTree cull, SceneInstanceData output)
    {
        mPositionOffsets = new NativeArray<float2>(9, Allocator.Persistent);
        mPositionOffsets[0] = new float2(0.5f, 0.5f);
        mPositionOffsets[1] = new float2(0.25f, 0.25f);
        mPositionOffsets[2] = new float2(0.75f, 0.75f);
        mPositionOffsets[3] = new float2(0.25f, 0.75f);
        mPositionOffsets[4] = new float2(0.75f, 0.25f);
        mPositionOffsets[5] = new float2(0.25f, 0.5f);
        mPositionOffsets[6] = new float2(0.75f, 0.5f);
        mPositionOffsets[7] = new float2(0.5f, 0.25f);
        mPositionOffsets[8] = new float2(0.5f, 0.75f);

        if (terrain == null || renderDatas == null || renderDatas.Count == 0 || cull == null || output == null)
            return;
        NativeArray<float> heightMaps = UpdateHeightMap(terrain);

        int gridCount = cull.vSize.x * cull.vSize.z;
        List<BlockNode> nodes = new List<BlockNode>(gridCount + gridCount / 2);
        List<BlockTree> trees = new List<BlockTree>(renderDatas.Count);
        List<Matrix4x4> matrices = new List<Matrix4x4>(1024 * 1024);

        List<Mesh> meshs = new List<Mesh>(renderDatas.Count);
        List<Material> materials = new List<Material>(renderDatas.Count);
        List<int> lods = new List<int>();
        List<float> windingMaxs = new List<float>();
        List<float> collideWindingMaxs = new List<float>();

        for (int i = 0; i < renderDatas.Count; ++i)
        {
            if (renderDatas[i].mesh == null || renderDatas[i].material == null)
                continue;

            nodes.Clear();
            int rootCount = BakeGrassLayerData(terrain, heightMaps, renderDatas[i], maxDensity, cull, matrices, nodes, i, 5);

            if (nodes.Count > 0)
            {
                BlockTree blockTree = new BlockTree();
                blockTree.rootCount = rootCount;
                blockTree.nodes = nodes.ToArray();
                trees.Add(blockTree);

                meshs.Add(renderDatas[i].mesh);
                materials.Add(renderDatas[i].material);
                lods.Add(0);
                windingMaxs.Add(renderDatas[i].windingMax);
                collideWindingMaxs.Add(renderDatas[i].collideWindingMax);
            }
        }

        output.instanceIndexTree = trees.ToArray();
        output.instanceMatrices = matrices.ToArray();
        output.meshPath = meshs.ToArray();
        output.materialPath = materials.ToArray();
        output.lod = lods.ToArray();
        output.windingMaxs = windingMaxs.ToArray();
        output.collideWindingMaxs = collideWindingMaxs.ToArray();

        matrices.Clear();
        mPositionOffsets.Dispose();
    }

    private void Add(int[] mipIndex, int layer)
    {
        if (layer >= mipIndex.Length)
            return;

        if (mipIndex[layer] == 3)
        {
            mipIndex[layer] = 0;
            Add(mipIndex, layer + 1);
        }
        else
        {
            mipIndex[layer] += 1;
        }
    }

    private int3 Pos(int[] mipIndex)
    {
        int3 pos = 0;
        for (int i = mipIndex.Length - 1; i >= 0; --i)
        {
            pos.z = pos.z * 2 + mipIndex[i] / 2;
            pos.x = pos.x * 2 + mipIndex[i] % 2;
        }
        return pos;
    }

    private void BakeGameObject(List<SceneBlockData[]> blockDatas, QTree cull, SceneGameObjectData output, int gameObjectCount, int nodeCount)
    {
        List<GameObjectData> transformDatas = new List<GameObjectData>(gameObjectCount);
        List<BlockNode> nodes = new List<BlockNode>(nodeCount);
        List<BlockTree> trees = new List<BlockTree>(blockDatas.Count);

        for (int i = 0; i < blockDatas.Count; ++i)
        {
            nodes.Clear();
            int rootCount = BakeGameObjectLOD(cull, blockDatas[i], transformDatas, nodes, 5, i);
            if (nodes.Count > 0)
            {
                BlockTree blockTree = new BlockTree();
                blockTree.rootCount = rootCount;
                blockTree.nodes = nodes.ToArray();
                trees.Add(blockTree);
            }
        }

        output.gameObjectIndexTree = trees.ToArray();
        output.gameObjectTramsforms = transformDatas.ToArray();
    }

    private int BakeGameObjectLOD(QTree cull, SceneBlockData[] blockDatas, List<GameObjectData> transformDatas, List<BlockNode> nodes, int mip, int lod)
    {
        int gridCount = blockDatas.Length;

        for (int i = 0; i < gridCount; ++i)
        {
            int srcIndex = QTree.TreeNodeIndexToOrderIndex(i, cull.vSize.xz);
            SceneBlockData sceneBlockData = blockDatas[srcIndex];

            if (sceneBlockData != null)
            {
                BlockNode node = new BlockNode();
                node.boundIndex = sceneBlockData.boundIndex;
                transformDatas.AddRange(sceneBlockData.datas);

                node.dataStart = transformDatas.Count - sceneBlockData.datas.Count;
                node.dataEnd = transformDatas.Count;

                nodes.Add(node);
            }
        }

        return GenTree(nodes, mip, cull.vSize.xz);
    }

    private int BakeGrassLayerData(Terrain terrain, NativeArray<float> heightMaps, InstancingRenderData renderData, int maxDensity, QTree cull, List<Matrix4x4> matrices, List<BlockNode> nodes, int layer, int mip)
    {
        Vector3 terrainPosition = terrain.GetPosition();
        TerrainData terrainData = terrain.terrainData;
        int heightmapResolution = terrain.terrainData.heightmapResolution;
        int detailResolution = terrain.terrainData.detailResolution;

        float2 detailPerCull = new float2(detailResolution, detailResolution) / cull.vSize.xz;

        int[,] detailData = TerrainUtil.GetDetailMap(terrain, 0, 0, detailResolution, detailResolution, layer);
        float[,,] alphaData = TerrainUtil.GetAlphaMap(terrain, 0, 0, 0, 0);
        float3 randomScale = renderData.randomScale;
        float3 scale = renderData.scale;
        float3 rotate = renderData.rotate;
        int[] alphaMasks = renderData.alphaMasks;
        float threshold = renderData.threshold;

        int gridCount = cull.vSize.x * cull.vSize.z;

        int3 pos = 0;

        for (int i = 0; i < gridCount; ++i)
        {
            pos.xz = QTree.TreeNodeIndexToPos(i);

            float2 start = pos.xz * detailPerCull;
            float2 end = (pos.xz + 1) * detailPerCull;
            int count = BakeChunckData(terrain, heightMaps, detailData, alphaData, alphaMasks, threshold, maxDensity, matrices, mPositionOffsets, (int2)start, (int2)end, randomScale, rotate, scale);
            if (count > 0)
            {
                BlockNode node = new BlockNode();

                node.boundIndex = pos.z * cull.vSize.x + pos.x;
                node.dataStart = matrices.Count - count;
                node.dataEnd = matrices.Count;

                nodes.Add(node);
            }
        }

        return GenTree(nodes, mip, cull.vSize.xz);
    }

    private int GenTree(List<BlockNode> nodes, int mip, int2 size)
    {
        int s = 0;
        int e = nodes.Count;

        if (e == 0)
            return 0;

        for (int i = 1; i < mip; ++i)
        {
            if (s >= nodes.Count)
            {
                Debug.LogErrorFormat("s {0}", s);
                break;
            }

            int currentParentIndex = -1;

            for (int j = s; j < e; ++j)
            {
                int index = QTree.OrderIndexToTreeNodeIndex(nodes[j].boundIndex, size);

                Debug.LogFormat("{0} -> {1}", nodes[j].boundIndex, index);

                index /= 4;

                if (index > currentParentIndex)
                {
                    //增加下一个
                    currentParentIndex = index;
                    BlockNode node = new BlockNode();
                    node.boundIndex = QTree.TreeNodeIndexToOrderIndex(currentParentIndex, size >> 1);
                    node.childStart = j;
                    node.dataStart = nodes[j].dataStart;
                    node.childEnd = j + 1;
                    node.dataEnd = nodes[j].dataEnd;

                    nodes.Add(node);
                }
                else if (index < currentParentIndex)
                {
                    Debug.LogErrorFormat("boundIndex 错误");
                }
                else
                {
                    BlockNode node = nodes[nodes.Count - 1];
                    node.childEnd = j + 1;
                    node.dataEnd = nodes[j].dataEnd;
                    nodes[nodes.Count - 1] = node;
                }
            }

            s = e;
            e = nodes.Count;

            size >>= 1;
        }

        return e - s;
    }

    private int BakeChunckData(Terrain terrain, NativeArray<float> heightMaps, int[,] detailData, float[,,] alphaData, int[] alphaMasks, float threshold, int maxDensity, List<Matrix4x4> list, NativeArray<float2> offset, int2 start, int2 end, float3 seedAndRange, float3 rotate, float3 scale)
    {
        int count = 0;

        int heightmapResolution = terrain.terrainData.heightmapResolution;
        int detailResolution = terrain.terrainData.detailResolution;
        int alphamapWidth = terrain.terrainData.alphamapWidth;

        float3 TerrainSize = terrain.terrainData.size;
        float2 TileSize = TerrainSize.xz / terrain.terrainData.detailResolution;
        float3 SeedAndRange = new float3(seedAndRange.z, seedAndRange.x, seedAndRange.y);
        float3 TerrainPosition = terrain.GetPosition();
        float2 alphaDD = new float2((float)alphamapWidth / detailResolution, (float)alphamapWidth / detailResolution);

        for (int y = start.y; y < end.y; ++y)
        {
            for (int x = start.x; x < end.x; ++x)
            {
                int detailCount = math.min(maxDensity, detailData[y, x]);

                float2 posXZ = TerrainUtil.GetIndexPostion(terrain, x, y);
                float posY = TerrainUtil.GetPointHeight(terrain, new Vector3(posXZ.x, 0, posXZ.y));

                float alphaScale = 1f;
                if (alphaMasks != null && alphaMasks.Length > 0)
                {
                    float alpha = 0;
                    for (int maskIndex = 0; maskIndex < alphaMasks.Length; ++maskIndex)
                    {
                        alpha += alphaData[(int)(y * alphaDD.y), (int)(x * alphaDD.x), alphaMasks[maskIndex]];
                    }

                    if (alpha <= threshold)
                    {
                        detailCount = 0;
                    }
                    else
                    {
                        detailCount = math.min(math.max(1, (int)(detailCount * alpha)), detailCount);
                        alphaScale = math.lerp(0.5f, 1f, (alpha - threshold) / (1f - threshold));
                    }
                }

                for (int i = 0; i < detailCount; ++i)
                {
                    int3 posAndIndex = new int3(x, y, i);

                    float dd = (float)heightmapResolution / (float)detailResolution;

                    float positionY = heightMaps[(int)(posAndIndex.y * dd) * heightmapResolution + (int)(posAndIndex.x * dd)] * TerrainSize.y;
                    float3 position = new float3((float)posAndIndex.x / detailResolution * TerrainSize.x, positionY, (float)posAndIndex.y / detailResolution * TerrainSize.z);

                    float s = noise.snoise(position.xz * SeedAndRange.x);
                    float randomScale = math.max(math.lerp(SeedAndRange.y, SeedAndRange.z, s) * alphaScale, SeedAndRange.y);

                    position += new float3(mPositionOffsets[posAndIndex.z].x * TileSize.x, 0, mPositionOffsets[posAndIndex.z].y * TileSize.y);
                    position += TerrainPosition;

                    list.Add(float4x4.TRS(position, quaternion.Euler(rotate), scale * randomScale));
                    //list.Add(new float4(position, scale));

                    ++count;
                }
            }
        }
        return count;
    }

    private NativeArray<float> UpdateHeightMap(Terrain terrain)
    {
        float[,,] alphaData = TerrainUtil.GetAlphaMap(terrain, 0, 0, 0, 0);
        int heightmapResolution = terrain.terrainData.heightmapResolution;

        NativeArray<float> heightMaps = new NativeArray<float>(heightmapResolution * heightmapResolution, Allocator.Persistent);
        float[,] heightMap = TerrainUtil.GetHeightMap(terrain, 0, 0, 0, 0);

        for (int y = 0; y < heightmapResolution; ++y)
        {
            for (int x = 0; x < heightmapResolution; ++x)
            {
                heightMaps[y * heightmapResolution + x] = heightMap[y, x];
            }
        }

        return heightMaps;
    }

    public struct EditorRenderData
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Material material;
        public int layer;
        public ShadowCastingMode shadowCastingMode;
        public bool receiveShadows;
        public bool invertCulling;
        public Bounds bounds;
        public Transform transform;
        public int priority;
    }

    public class RenderDataComparer : IEqualityComparer<EditorRenderData>
    {
        public bool Equals(EditorRenderData x, EditorRenderData y)
        {
            return
            x.mesh == y.mesh &&
            x.subMeshIndex == y.subMeshIndex &&
            x.material == y.material &&
            x.layer == y.layer &&
            x.shadowCastingMode == y.shadowCastingMode &&
            x.receiveShadows == y.receiveShadows &&
            x.invertCulling == y.invertCulling;// &&
            //x.bounds == y.bounds;
        }

        public int GetHashCode(EditorRenderData obj)
        {
            return 0;
        }
    }

    private int TranslateBatchPrefab(List<RenderData> renderDatas, List<Matrix4x4> matrix4x4s, List<AABB> bounds)
    {
        Dictionary<EditorRenderData, List<EditorRenderData>> datas = new Dictionary<EditorRenderData, List<EditorRenderData>>(new RenderDataComparer());        

        for (int i = 0; i < gPriorityCount; ++i)
        {
            HashSet<MeshRenderer> transforms = meshRenderers[i];
            if (transforms == null)
                continue;

            int priorityCount = i;

            foreach (MeshRenderer meshRenderer in transforms)
            {
                if (meshRenderer == null)
                    continue;

                if (!meshRenderer.TryGetComponent<MeshFilter>(out MeshFilter meshFilter) || meshFilter.sharedMesh == null)
                    continue;

                for (int j = 0; j < meshRenderer.sharedMaterials.Length; ++j)
                {
                    EditorRenderData renderData = new EditorRenderData();
                    renderData.mesh = meshFilter.sharedMesh;
                    renderData.subMeshIndex = meshRenderer.subMeshStartIndex + j;
                    renderData.material = meshRenderer.sharedMaterials[j];
                    renderData.layer = meshRenderer.gameObject.layer;
                    renderData.shadowCastingMode = meshRenderer.shadowCastingMode;
                    renderData.receiveShadows = meshRenderer.receiveShadows;
                    renderData.invertCulling = false;
                    renderData.bounds = meshRenderer.bounds;
                    renderData.transform = meshRenderer.transform;
                    renderData.priority = priorityCount;

                    List<EditorRenderData> mats;
                    if (meshFilter.sharedMesh == null || renderData.material == null)
                    {
                        Debug.LogErrorFormat("{0} mesh 或者 material 丢失", meshRenderer.gameObject.name);
                        continue;
                    }

                    if (!datas.TryGetValue(renderData, out mats))
                    {
                        mats = new List<EditorRenderData>();
                        datas[renderData] = mats;
                    }
                    mats.Add(renderData);
                }
            }
        }

        foreach (var kv in datas)
        {
            List<EditorRenderData> editorRenderDatas = kv.Value;

            if (editorRenderDatas.Count <= 0)
                continue;

            EditorRenderData data = kv.Key;

            //填充运行时的数据
            RenderData renderData = new RenderData();
            renderData.mesh = data.mesh;
            renderData.subMeshIndex = data.subMeshIndex;
            renderData.material = data.material;
            renderData.layer = data.layer;
            renderData.shadowCastingMode = data.shadowCastingMode;
            renderData.receiveShadows = data.receiveShadows;
            //renderData.invertCulling = data.invertCulling;
            renderData.instanceCount = kv.Value.Count;
            renderData.lod = data.priority;

            MinMaxAABB minMaxAABB = new MinMaxAABB();
            for (int i = 0; i < kv.Value.Count; ++i)
            {
                matrix4x4s.Add(kv.Value[i].transform.localToWorldMatrix);

                AABB aabb = new AABB();
                aabb.Center = kv.Value[i].bounds.center;
                aabb.Extents = kv.Value[i].bounds.extents;
                aabb.Extents = math.max(new float3(0.01f), aabb.Extents);
                bounds.Add(aabb);

                if (i == 0)
                {
                    minMaxAABB = aabb;
                }
                else
                {
                    minMaxAABB.Encapsulate(aabb);
                }
            }

            AABB b = minMaxAABB;
            renderData.bounds = new Bounds(b.Center, b.Size);

            renderDatas.Add(renderData);
        }

        return matrix4x4s.Count;
    }
}