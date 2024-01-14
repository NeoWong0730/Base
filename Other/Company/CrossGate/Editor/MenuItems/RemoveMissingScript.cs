using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine.SceneManagement;

/***unity代码批量修改Remove Missing Script和批量修改指定组件的内容 ***/
/***一些脚本已经被废弃，但是这些废弃脚本还是被绑定在某些预制体中，这时候运行就会产生很多Missing Script的警告信息，这些警告虽不影响代码的实际运行，
 * 但是一个大项目肯定不能出现的N多的警告信息，并且这里通过手动去找肯定不现实，所以这里我们就同一个脚本去实现自动去遍历所有的prefab然后移除Missing的组件，下面是具体的代码信息。
1.首先在工程中创建一个Editro文件夹，将脚本放在Editor文件夹下
2.通过选中文件，通过编辑器实现遍历该文件夹下的所有prefab:
3.找到prefab,去遍历她的所有子物体，并实现直接移除相应的空组件：
***/


/// <summary>
/// 移除项目中go上丢失的脚本
/// </summary>
public class RemoveMissingScript
{
    [MenuItem("__Tools__/对象移除丢失脚本")]
    static void GetAllGo()
    {
        object[] obj = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
        for (int i = 0; i < obj.Length; i++)
        {
            string ext = System.IO.Path.GetExtension(obj[i].ToString());
            Debug.Log("name:" + ext);
            if (!ext.Contains(".GameObject"))
            {
                continue;
            }

            GameObject go = (GameObject)obj[i];

            // 移除子物体下的脚本丢失
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
            {
                CleanMissingScript(trans.gameObject, go);
            }
        }

    }

    static void CleanMissingScript(GameObject go, GameObject parentGO)
    {
        //由于Unity版本更新，以下代码不起作用了
        //var components = go.GetComponents<Component>();
        //var serializedObject = new SerializedObject(go);
        //var prop = serializedObject.FindProperty("m_Component");
        //int r = 0;
        //for (int j = 0; j < components.Length; j++)
        //{
        //    if (components[j] == null)
        //    {
        //        prop.DeleteArrayElementAtIndex(j - r);
        //        Debug.LogWarning("成功移除丢失脚本，gameObject name: " + go.name + " ---父类prefab name：" + parentGO.name);
        //        r++;
        //    }
        //}
        //serializedObject.ApplyModifiedProperties();


        //替换为官方提供的接口 -- yd
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
    }



    [MenuItem("__Tools__/清除所有场景的丢失的脚本")]
    static void ClearAllScenesMissingScript()
    {
        if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂停，请不要操作！", "确定");
        }

        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
            return;
        }

        if (EditorUtility.DisplayDialog("提示", "该过程大概需要2-3分钟,继续请按确定", "确定", "取消"))
        {
            ClearScenesMissingScript();
            EditorUtility.DisplayDialog("完成", "清除所有场景所挂载 Missing 脚本", "确定");
        }
    }

    static void ClearScenesMissingScript()
    {
        //获得当前场景
        List<string> SceneNames = new List<string>();
        AddressableAssetSettings assetSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        AddressableAssetGroup group = assetSettings.FindGroup("Scene");
        if (group != null)
        {
            foreach (AddressableAssetEntry entry in group.entries)
            {
                SceneNames.Add(entry.AssetPath);
            }
        }

        for (int i = 0; i < SceneNames.Count; i++)
        {
            //打开场景
            Scene scene = EditorSceneManager.OpenScene(SceneNames[i], OpenSceneMode.Single);
            Debug.LogError("打开场景:" + SceneNames[i]+" ,移除丢失的脚本");
            //获得scene下的第一层所有结点
            GameObject[] rootGameobjects = scene.GetRootGameObjects();
            for (int j = 0; j < rootGameobjects.Length; j++)
            {
                //获得第一层结点下所有的子节点
                foreach (var item in rootGameobjects[j].transform.GetComponentsInChildren<Transform>(true))
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(item.gameObject);
                }
            }
            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
        }
    }


    
    private List<GameObject> GetAllSceneObjectsWithInactive()
    {
       var allTransForm = Resources.FindObjectsOfTypeAll(typeof(Transform));
        var previousSelection = Selection.objects;
        Selection.objects = allTransForm.Cast<Transform>().Where(x => x!=null).Select(x => x.gameObject).Where(x => x != null && !x.activeInHierarchy).Cast<UnityEngine.Object>().ToArray();
        var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        Selection.objects = previousSelection;
        return selectedTransforms.Select(tr => tr.gameObject).ToList();
    }



    // 获取场景中所有目标对象（包括不激活的对象）不包括Prefabs:
    List<T> FindSceneObject<T>(string _SceneName) where T : UnityEngine.Component
    {
        List<T> objectsInScene = new List<T>();
        foreach (var go in Resources.FindObjectsOfTypeAll<T>())
        {
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue;
            if (EditorUtility.IsPersistent(go.transform.root.gameObject))// 如果对象位于Scene中，则返回false
                continue;
            if (_SceneName != go.gameObject.scene.name)
                continue;
            Debug.LogFormat("gameObject:{0},scene:{1}", go.gameObject.name, go.gameObject.scene.name);
            objectsInScene.Add(go);
        }
        return objectsInScene;
    }

}