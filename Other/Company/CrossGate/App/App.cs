using Framework;
using Lib.Core;
using Net;
using UnityEngine;
using UnityEngine.EventSystems;

public class App : MonoBehaviour
{
    public GameObject mVideoPlayer = null;
    public Camera mUICamera;
    public EventSystem mEventSystem;

    private IAppState AppState = null;
    private GameObject mLogoGameObject = null;

    private void Awake()
    {
        SDKManager.Init(InitBuglyCallBack);

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        AspectRotioController.Instance.InitData();
        AspectRotioController.Instance.SetNotFullScreen();
#endif

        Application.targetFrameRate = 30;

        //if (PlayerPrefs.HasKey("GraphicsTier"))
        //{
        //    Graphics.activeTier = (UnityEngine.Rendering.GraphicsTier)PlayerPrefs.GetInt("GraphicsTier");            
        //}
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

#if DEBUG_MODE
        DebugUtil.Load();
        GameObject go = Resources.Load("Reporter") as GameObject;
        go = Instantiate(go);
        DontDestroyOnLoad(go);
#endif
        //加载Logo
        GameObject mLogoAsset = Resources.Load("Logo") as GameObject;
        mLogoGameObject = Instantiate(mLogoAsset);
        //2秒后关闭
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
        HitPointManager.HitPoint("game_game_start");

        //AtlasManager.Register();
        HotFixTipWordManager.Instance.Init();
        AppManager.NextAppState = EAppState.PerformanceCheck;
        /*
                if (SDKManager.GetHaveSDK() && !SDKManager.bInit)
                {
                    AppManager.NextAppState = EAppState.WaitSDK;
        #if UNITY_STANDALONE_WIN && USE_PCSDK
                    SDKManager.SDKInit();
        #endif
                }
                else
                {
                    AppManager.NextAppState = EAppState.CheckVersion;
                }
        */
    }

    private void Update()
    {        
        TimeManager.Update();     

        SDKManager.Update();        

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

    /// <summary>
    /// 游戏退出    
    /// </summary>
    private void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        AspectRotioController.Instance.UnregisterHotKey();
        SDKManager.ACESDKExitProcess();
#endif
        NetClient.Instance.Dispose();
        HotFixStateManager.Instance.OnExit();
        SDKManager.OnExit();
        PlayerPrefs.Save();
        AtlasManager.UnRegister();

        AppState?.OnExit();
        AudioManager.Instance.StopAll();
    }

    /// <summary>
    /// 游戏暂停
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnApplicationPause({0})", pause);
        if (pause)
            SDKManager.Pause();
        else
            SDKManager.Resume();

        AppState?.OnApplicationPause(pause);
    }

    private bool AppStateTranslate()
    {
        if (AppManager.NextAppState == EAppState.Invalid)
            return false;

        EAppState currentState = AppState == null ? EAppState.Invalid : AppState.State;

        //是否需要同状态reset(暂时没需求)
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
            case EAppState.WaitSDK:
                AppState = new AppState_WaitSDK();
                break;
            case EAppState.WaitOtherAppHotFix:
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

    public void InitBuglyCallBack()
    {
        // 开启SDK的日志打印，发布版本请务必关闭
        BuglyAgent.ConfigDebugMode(false);

#if UNITY_IPHONE || UNITY_IOS
        BuglyAgent.InitWithAppId ("924e67b13a");
#elif UNITY_ANDROID
        BuglyAgent.InitWithAppId("f2a1cc0a3c");
#endif
        // 如果你确认已在对应的iOS工程或Android工程中初始化SDK，那么在脚本中只需启动C#异常捕获上报功能即可
        BuglyAgent.EnableExceptionHandler();
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