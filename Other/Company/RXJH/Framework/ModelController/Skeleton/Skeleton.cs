using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public Transform mRoot;
    /// <summary>   
    /// 通过资源的形式可以在多个实例中共享引用
    /// </summary>
    public SkeletonSlotConfig mSkeletonSlotConfig;

    public Transform GetBoneByPath(string path)
    {        
        return mRoot?.Find(path);
    }    

    public Transform GetSlotByID(int id)
    {
        if (!mSkeletonSlotConfig)
            return null;

        if (mSkeletonSlotConfig.TryGetSlotPath(id, out string path))
        {
            return mRoot?.Find(path);
        }

        return null;
    }
}
