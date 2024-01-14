using Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using Lib;
using UnityEngine.LowLevel;

public class App : MonoBehaviour
{
    private GameObject mLogoGameObject;
    public GameObject mVideoPlayer;
    public Camera mUICamera;
    public EventSystem mEventSystem;

    private IAppState AppState;

    private void Awake()
    {
        //SDKManager.Instance.Init();

        Application.targetFrameRate = 30;
        DebugUtil.LogFormat(ELogType.eNone, Graphics.activeTier.ToString());

        SceneManager.Init(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(mUICamera);
        DontDestroyOnLoad(mEventSystem);
        DontDestroyOnLoad(mVideoPlayer);

        CameraManager.SetUICamera(mUICamera);
        AppManager.mEventSystem = mEventSystem;
        VideoManager.Init(mVideoPlayer);

        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.lowMemory += OnLowMemory;

        GameObject mLogoAsset = Resources.Load("Logo") as GameObject;
        mLogoGameObject = Instantiate(mLogoAsset);

        Timer.Register(3, OnLogoClose);

        CoroutineManager.Instance.Initialize();
        TimeManager.StartUpdate();

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Texture2D tex = Resources.Load<Texture2D>("Texture/UI_Mouse");
        Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);
#endif

#if DEBUG_MODE
        TimeManager.StartFPSCalculate();
#endif
    }

    private void Start()
    {
        HotFixTipWordManager.Instance.Init();
        AppManager.NextAppState = EAppState.Game;
    }

    private void Update()
    {
        TimeManager.Update();
        //SDKManager.Instance.Update();
        AppStateTranslate();
        AppState?.OnUpdate();
    }

    private void FixedUpdate()
    {
        AppState?.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        AppState?.OnLateUpdate();
    }

    private void OnApplicationQuit()
    {
        //Todo
        //NetClinet.Instance.Dispose();
        //HotFixStateManager.Instance.OnExit();
        //SDKManager.Instance.OnExit();
        PlayerPrefs.Save();

        AppState?.OnExit();
        AudioManager.Instance.StopAll();
    }

    private void OnApplicationPause(bool pause)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnApplicationPause({0})", pause);

        if (pause)
            ;
            //SDKManager.Instance.Pause();
        else
            //SDKManager.Instance.Resume();

        AppState?.OnApplicationPause(pause);
    }

    private bool AppStateTranslate()
    {
        if (AppManager.NextAppState == EAppState.Invalid)
            return false;

        EAppState currentState = AppState == null ? EAppState.Invalid : AppState.State;

        if (AppManager.NextAppState == currentState)
            return false;

        DebugUtil.LogFormat(ELogType.eNone, "NextAppState({0})", AppManager.NextAppState);

        if (AppState != null)
        {
            AppState.OnExit();
            AppState = null;

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        switch (AppManager.NextAppState)
        {
            case EAppState.Game:
                AppState = new AppState_Game();
                break;
            case EAppState.HotFix:
                AppState = new AppState_HotFix();
                break;
            case EAppState.CheckVersion:
                AppState = new AppState_CheckVersion();
                break;
            case EAppState.WaitSdDK:
                AppState = new AppState_WaitSDK();
                break;
            case EAppState.WaitOtherAppHotfix:
                AppState = new AppState_WaitHotFix();
                break;
            case EAppState.PerformanceCheck:
                AppState = new AppState_PerformanceCheck();
                break;
            default:
                break;
        }
        AppManager.eAppState = AppManager.NextAppState;
        AppManager.NextAppState = EAppState.Invalid;
        if (AppState != null)
        {
            AppState.OnEnter();
        }

        return true;
    }

    private void OnLowMemory()
    {
        DebugUtil.LogError("OnLowMemory");
        if (AppState != null)
        {
            AppState.OnLowMemory();
        }
        else
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    private void OnLogoClose()
    {
        DestroyImmediate(mLogoGameObject);
        mLogoGameObject = null;
    }

    private void OnDestroy()
    {
        PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
    }

#if DEBUG_MODE
    private void OnGUI()
    {
        if (AppState != null)
        {
            AppState.OnGUI();
        }
    }
#endif
}
