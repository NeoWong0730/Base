using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
public class SceneWindow : DebugWindowBase
{
    public class HierarchyTreeNode
    {
        public static string enable = "enable";
        public static string disable = "disable";
        public static string add = "+";
        public static string cut = "-";

        public GameObject gameObject;
        public Scene scene;
        public Component[] Components;

        public int nDepth;
        public bool bExpansion;

        public List<HierarchyTreeNode> nodes;
        private static string sActive = "active";

        public void GetChild()
        {
            if (scene.IsValid())
            {
                GameObject[] roots = scene.GetRootGameObjects();
                int childCount = roots.Length;
                if (childCount > 0)
                {
                    if (nodes == null)
                    {
                        nodes = new List<HierarchyTreeNode>(childCount);
                    }
                    else
                    {
                        nodes.Capacity = childCount;
                    }

                    for (int i = 0; i < childCount; ++i)
                    {
                        HierarchyTreeNode node = new HierarchyTreeNode();
                        node.bExpansion = false;
                        node.gameObject = roots[i];
                        node.nDepth = nDepth + 1;
                        nodes.Add(node);
                    }
                }
            }
            else if (gameObject != null)
            {
                Transform transform = gameObject.transform;
                int childCount = transform.childCount;
                if (childCount > 0)
                {
                    if (nodes == null)
                    {
                        nodes = new List<HierarchyTreeNode>(childCount);
                    }
                    else
                    {
                        nodes.Capacity = childCount;
                    }

                    for (int i = 0; i < transform.childCount; ++i)
                    {
                        HierarchyTreeNode node = new HierarchyTreeNode();
                        node.bExpansion = false;
                        node.gameObject = transform.GetChild(i).gameObject;
                        node.nDepth = nDepth + 1;
                        nodes.Add(node);
                    }
                }
            }
        }

        public int Draw(int layer, Vector4 size, SceneWindow window, GUIStyle style)
        {
            if (gameObject == null && !scene.IsValid())
                return layer;

            if (scene.IsValid() || gameObject.activeSelf)
            {
                style.normal.textColor = Color.white;
            }
            else
            {
                style.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            }        

            if (scene.IsValid() || gameObject.transform.childCount > 0)
            {
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Normal;

                if (GUI.Button(new Rect(size.z * nDepth, layer * size.y, size.y, size.y - size.w), bExpansion ? cut : add, style))
                {
                    bExpansion = !bExpansion;
                }
            }
            else
            {
                bExpansion = false;
            }


            if (scene.IsValid())
            {                
                style.alignment = TextAnchor.MiddleLeft;
                style.fontStyle = FontStyle.Bold;
                if (GUI.Button(new Rect(size.z * nDepth + size.y * 1.1f, layer * size.y, size.x, size.y - size.w), scene.name, style))
                {
                    if(window.selected != this)
                    {
                        if(window.selected != null)
                        {
                            window.selected.Components = null;
                        }

                        window.selected = null;
                    }
                }
            }
            else
            {                
                style.alignment = TextAnchor.MiddleLeft;
                if (window.selected == this)
                {
                    style.fontStyle = FontStyle.Italic;
                }
                else
                {
                    style.fontStyle = FontStyle.Normal;
                }

                if (GUI.Button(new Rect(size.z * nDepth + size.y * 1.1f, layer * size.y, size.x, size.y - size.w), gameObject.name, style))
                {
                    if (window.selected != this)
                    {
                        if (window.selected != null)
                        {
                            window.selected.Components = null;
                        }

                        window.selected = this;
                        Components = gameObject.GetComponents<Component>();
                    }
                }
            }


            ++layer;

            if (!bExpansion)
                return layer;

            if (nodes == null)
            {
                GetChild();
            }

            if (nodes == null)
                return layer;

            for (int i = 0; i < nodes.Count; ++i)
            {
                layer = nodes[i].Draw(layer, size, window, style);
            }
            return layer;
        }

        public void DrawInspector()
        {
            if(gameObject != null)
            {
                gameObject.SetActive(GUILayout.Toggle(gameObject.activeSelf, gameObject.name));
                GUILayout.Label(gameObject.tag);
                GUILayout.Label(gameObject.layer.ToString());
                GUILayout.Label(gameObject.transform.localPosition.ToString());
                GUILayout.Label(gameObject.transform.localRotation.ToString());
                GUILayout.Label(gameObject.transform.localScale.ToString());

                if(Components != null)
                {
                    for (int i = 0; i < Components.Length; ++i)
                    {
                        Behaviour behaviour = Components[i] as Behaviour;                        
                        if (behaviour != null)
                        {
                            Camera camera = behaviour as Camera;
                            if(camera != null)
                            {
                                bool requiresDepthOption = camera.GetUniversalAdditionalCameraData().requiresDepthOption == CameraOverrideOption.On;
                                bool requiresColorOption = camera.GetUniversalAdditionalCameraData().requiresColorOption == CameraOverrideOption.On;

                                requiresDepthOption = GUILayout.Toggle(requiresDepthOption, "depth");
                                requiresColorOption = GUILayout.Toggle(requiresColorOption, "color");

                                camera.GetUniversalAdditionalCameraData().requiresDepthOption = requiresDepthOption ? CameraOverrideOption.On : CameraOverrideOption.Off;
                                camera.GetUniversalAdditionalCameraData().requiresColorOption = requiresColorOption ? CameraOverrideOption.On : CameraOverrideOption.Off;
                            }
                            behaviour.enabled = GUILayout.Toggle(behaviour.enabled, Components[i].GetType().Name);
                        }
                        else
                        {
                            GUILayout.Label(Components[i].GetType().Name);
                        }
                    }
                }                
            }
            else if(scene.IsValid())
            {

            }
        }
    }       

    #region Debug    
    Vector4 sizeAndOffset = new Vector4(200, 66, 32, 18);
    private string sRefreshHierarchy = "刷新";
    private Vector2 hierarchyPos;
    private Vector2 inspectorPos;
    private List<HierarchyTreeNode> nodes = new List<HierarchyTreeNode>();
    private int layerCount = 0;
    private HierarchyTreeNode selected;
    private float depthIndentsP = 1f;//缩进0 -1
    private float fThisScale = 1f;//缩进0 -1

    public SceneWindow(int id) : base(id) { }
    private void OnGUIHierarchy()
    {
        Vector4 size = sizeAndOffset * fScale * fThisScale;
        float h = layerCount * size.y;

        size.z *= depthIndentsP;

        hierarchyPos = GUI.BeginScrollView(new Rect(16 * fScale, 96 * fScale, 500 * fScale, (Screen.height - 200 * fScale)), hierarchyPos, new Rect(0, 0, 400 * fScale, Mathf.Max(500 * fScale, h)));

        if (nodes != null)
        {
            int layer = 0;
            for (int i = 0; i < nodes.Count; ++i)
            {
                layer = nodes[i].Draw(layer, size, this, gUIStyle);
            }
            layerCount = layer + 1;
        }
        GUI.EndScrollView();
    }

    private void OnGUIInspector()
    {
        if (selected == null)
            return;

        GUILayout.BeginArea(new Rect(532 * fScale, 96 * fScale, 500 * fScale, (Screen.height - 100 * fScale)));
        inspectorPos = GUILayout.BeginScrollView(inspectorPos);

        selected.DrawInspector();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    #endregion

    GUIStyle gUIStyle;
    // Start is called before the first frame update
    public override void OnAwake()
    {    
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene arg0)
    {
        Refresh();
    }

    private void Refresh()
    {
        nodes.Clear();

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

            HierarchyTreeNode node = new HierarchyTreeNode();
            node.scene = scene;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if (Logic.Core.UIManager.mRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.Core.UIManager.mRoot.gameObject;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if (Logic.GameCenter.mainWorldRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.GameCenter.mainWorldRoot.gameObject;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if (Logic.GameCenter.fightActorRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.GameCenter.fightActorRoot.gameObject;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if (Logic.GameCenter.modelShowRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.GameCenter.modelShowRoot.gameObject;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if (Logic.GameCenter.skillPreViewRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.GameCenter.skillPreViewRoot.gameObject;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if (Logic.GameCenter.sceneShowRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.GameCenter.sceneShowRoot;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        //if(Logic.NPCHelper.cacheNpcsRoot != null)
        //{
        //    HierarchyTreeNode node = new HierarchyTreeNode();
        //    node.gameObject = Logic.NPCHelper.cacheNpcsRoot;
        //    node.nDepth = 0;
        //    node.bExpansion = false;
        //    nodes.Add(node);
        //}

        //if (Logic.Core.SceneActor.CacheSceneActorsRoot != null)
        //{
        //    HierarchyTreeNode node = new HierarchyTreeNode();
        //    node.gameObject = Logic.Core.SceneActor.CacheSceneActorsRoot;
        //    node.nDepth = 0;
        //    node.bExpansion = false;
        //    nodes.Add(node);
        //}

        if (Logic.Core.UIManager.mUICamera != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.Core.UIManager.mUICamera.gameObject;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }

        if(Logic.GameCenter.sceneShowRoot != null)
        {
            HierarchyTreeNode node = new HierarchyTreeNode();
            node.gameObject = Logic.GameCenter.sceneShowRoot;
            node.nDepth = 0;
            node.bExpansion = false;
            nodes.Add(node);
        }
    }

    public override void WindowFunction(int id)
    {
        if(gUIStyle == null)
        {
            gUIStyle = new GUIStyle(GUIStyle.none);            
            gUIStyle.fontStyle = FontStyle.Bold;
            gUIStyle.alignment = TextAnchor.MiddleLeft;
        }

        gUIStyle.fontSize = (int)(32 * fScale);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(sRefreshHierarchy))
        {
            Refresh();
        }

        fThisScale = GUILayout.HorizontalSlider(fThisScale, 0, 2);
        depthIndentsP = GUILayout.HorizontalSlider(depthIndentsP, 0, 1);
        GUILayout.EndHorizontal();

        OnGUIHierarchy();
        OnGUIInspector();
    }
}
