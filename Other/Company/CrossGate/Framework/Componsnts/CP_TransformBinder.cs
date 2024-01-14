using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CP_TransformBinder : MonoBehaviour
{
    // 当前gameobject将来挂载的gameobject
    // 当前gameobject将来挂载的gameobject下面的路径
    //[FormerlySerializedAs("isAuto")]
    //[FormerlySerializedAs("ignoreOuterParent")]
    [Header("忽略动态设置 parent  [如果可以直接在timelinePrefab中找到parent,则直接拖拽，然后勾选这里，否则不拖拽，也不勾选]")]
    public bool ignoreOuterParent = false;
    public Transform parent;
    public string path = "";

    public CP_TransformBinder Init(Transform parent)
    {
        if (!ignoreOuterParent)
        {
            this.parent = parent;
        }
        return this;
    }
    public CP_TransformBinder Init(string path)
    {
        this.path = path;
        return this;
    }

    public void Bind()
    {
        if(parent != null && path != null)
        {
            Transform binder = parent.Find(path);
            transform.SetParent(binder, false);
        }
    }
}
