using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using System;

public struct BoneSyncJob : IDisposableAnimationJob
{
    public NativeArray<TransformStreamHandle> SrcHandles;
    public NativeArray<TransformStreamHandle> DstHandles;

    public void Dispose()
    {
        SrcHandles.Dispose();
        DstHandles.Dispose();
    }

    public void ProcessAnimation(AnimationStream stream)
    {
        for (var i = 0; i < SrcHandles.Length; ++i)
        {
            //if (!SrcHandles[i].IsValid(stream))
            //    continue;
            SrcHandles[i].GetGlobalTR(stream, out Vector3 position, out Quaternion rotation);
            DstHandles[i].SetGlobalTR(stream, position, rotation, true);
        }
    }

    public void ProcessRootMotion(AnimationStream stream)
    {

    }
}
