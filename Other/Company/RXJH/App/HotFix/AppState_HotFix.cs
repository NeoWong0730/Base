using Framework;
using Lib.AssetLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AppState_HotFix : IAppState
{
    public EAppState State
    {
        get
        {
            return EAppState.HotFix;
        }
    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnLateUpdate() { }

    public void OnFixedUpdate() { }

    public void OnGUI(){}

    public void OnApplicationPause(bool pause) { }

    public void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
