﻿using Framework;

public class AppState_PerformanceCheck : IAppState
{
    private static PerformanceCheck performanceCheck;

    public EAppState State { get { return EAppState.PerformanceCheck; } }

    public void OnApplicationPause(bool pause)
    {
        
    }

    public void OnEnter()
    {
        performanceCheck = new PerformanceCheck();
        performanceCheck.Start();
    }

    public void OnUpdate()
    {
        if (performanceCheck.IsFinished())
        {
            if (SDKManager.sdk.IsHaveSdk && !SDKManager.bInit)
            {
                AppManager.NextAppState = EAppState.WaitSDK;
#if UNITY_STANDALONE_WIN && USE_PCSDK
                //SDKManager.SDKInit();
#endif
            }
            else
            {
                AppManager.NextAppState = EAppState.CheckVersion;
            }
        }
    }

    public void OnExit()
    {
        AppManager.nPerformanceScore = performanceCheck.nPerformanceScore;        
        performanceCheck.End();
        performanceCheck = null;
    }

    public void OnFixedUpdate()
    {        
    }

    public void OnGUI()
    {     
    }

    public void OnLateUpdate()
    {        
    }

    public void OnLowMemory()
    {        
    }
}