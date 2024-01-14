using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSlotConfig : ScriptableObject
{
    public int[] mSlotIDs;
    public string[] mSlotPaths;
    private Dictionary<int, string> mSlotIDToPath;

    public bool TryGetSlotPath(int slotID, out string path)
    {
        if (mSlotIDToPath == null)
        {
            mSlotIDToPath = new Dictionary<int, string>(mSlotIDs.Length);
            for (int i = 0; i < mSlotIDs.Length; ++i)
            {
                mSlotIDToPath.Add(mSlotIDs[i], mSlotPaths[i]);
            }
        }
        return mSlotIDToPath.TryGetValue(slotID, out path);
    }
}
