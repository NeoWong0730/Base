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
        VideoManager.Play(Lib.AssetLoader.AssetPath.GetVideoFullUrl("Config/Video/cutscene_login_01.mp4"), true);
        //AppManager.OpenLaunchUI();
        if (VersionHelper.eHotFixMode != EHotFixMode.Close)
        {
            UIHotFix.Create();
        }

        //HotFixManager.Instance.OnEnter();
        HotFixStateManager.Instance.OnEnter();
    }

    public void OnExit()
    {
        //HotFixManager.Instance.OnExit();
        HotFixStateManager.Instance.OnExit();
        UIHotFix.Destroy();
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        string hotFixFlag = string.Format("{0}/{1}", Application.persistentDataPath, AssetPath.sHotFixFlag);
        if (File.Exists(hotFixFlag))
        {
            string readProcess = File.ReadAllText(hotFixFlag);
            int hotFixProcess = 0;
            if (int.TryParse(readProcess, out hotFixProcess) && hotFixProcess == AppManager.ProcessID)
            {
                File.Delete(hotFixFlag);
            }
            WaitAppHotFixManager.Instance.SendMessageToOtherProcess(HotFixStateManager.Instance.CurrentHotFixState == EHotFixState.Success);
        }
#endif
    }

    public void OnUpdate()
    {
        //HotFixManager.Instance.OnUpdate();
        HotFixStateManager.Instance.OnUpdate();
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
