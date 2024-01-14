using Framework;
using Lib.AssetLoader;
using Lib.Core;
using System;
using System.IO;
using UnityEngine;

public class AppState_WaitHotFix : IAppState
{

    public EAppState State { get { return EAppState.WaitOtherAppHotFix; } }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void OnGUI()
    {

    }

    public void OnLateUpdate()
    {

    }


    public void OnApplicationPause(bool pause) { }
}
