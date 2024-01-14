using Framework;
using Lib.Core;
using Net;


public class AppState_Game : IAppState
{
    public EAppState State
    {
        get
        {
            return EAppState.Game;
        }
    }

    private IAssembly assembly = null;

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
    protected IStaticMethod startMethod;
    protected IStaticMethod exitMethod;
    protected IStaticMethod updateMethod;
    protected IStaticMethod lateUpdateMethod;
    protected IStaticMethod fixedUpdateMethod;
    protected IStaticMethod lowMemory;
    protected IStaticMethod onGUI;
    protected IStaticMethod onApplicationPause;
#endif    

    private bool bFunctionCreated = false;
    private bool bStart = false;
    private static System.Threading.Tasks.Task _loadTask;

    private void LoadDll()
    {
#if DEBUG_MODE
        float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif

#if ILRUNTIME_MODE
        assembly = new ILRuntimeAssembly();
#elif MONO_REFLECT_MODE
        assembly = new MonoAssembly();
#else
        assembly = new InnerAssembly();
#endif

        assembly.Load();
        AssemblyManager.Instance.RegisterAssembly(assembly);

#if DEBUG_MODE
        DebugUtil.LogTimeCost(ELogType.eNone, "LoadDll()", ref timePoint);
#endif
    }

    private void AsyncLoad()
    {
#if ILRUNTIME_MODE
        (assembly as ILRuntimeAssembly).Binding();
#endif
    }

    private void CreateFunction()
    {
#if DEBUG_MODE
        float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        startMethod = assembly.CreateStaticMethod("Logic.Core.GameMain", "Start", 0);
        exitMethod = assembly.CreateStaticMethod("Logic.Core.GameMain", "Exit", 0);
        updateMethod = assembly.CreateStaticMethod("Logic.Core.GameMain", "Update", 0);
        lateUpdateMethod = assembly.CreateStaticMethod("Logic.Core.GameMain", "LateUpdate", 0);
        fixedUpdateMethod = assembly.CreateStaticMethod("Logic.Core.GameMain", "FixedUpdate", 0);
        lowMemory = assembly.CreateStaticMethod("Logic.Core.GameMain", "OnLowMemory", 0);
        onGUI = assembly.CreateStaticMethod("Logic.Core.GameMain", "OnGUI", 0);
        onApplicationPause = assembly.CreateStaticMethod("Logic.Core.GameMain", "OnApplicationPause", 1);

        //PreventEnterHotfix();
#endif

#if DEBUG_MODE
        DebugUtil.LogTimeCost(ELogType.eNone, "assembly.CreateStaticMethod", ref timePoint);
#endif

        LogicStaticMethodDispatcher.Init();

#if DEBUG_MODE
        DebugUtil.LogTimeCost(ELogType.eNone, "LogicStaticMethodDispatcher.Init()", ref timePoint);
#endif
    }

    private void DoStart()
    {
#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        startMethod?.Run();
#else
        Logic.Core.GameMain.Start();
#endif
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        AspectRotioController.Instance.InitStart();
#endif
    }

    private void UnloadDll()
    {
#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        startMethod = null;
        exitMethod = null;
        updateMethod = null;
        lateUpdateMethod = null;
        fixedUpdateMethod = null;
        lowMemory = null;
        onGUI = null;
        onApplicationPause = null;
#endif
        LogicStaticMethodDispatcher.UnInit();
        AssemblyManager.Instance.RegisterAssembly(null);
        assembly.Unload();
    }

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
    // 调用此处，会只执行框架层代码，不执行热更层代码
    public void PreventEnterHotfix() {
        startMethod = null;
        exitMethod = null;
        updateMethod = null;
        lateUpdateMethod = null;
        fixedUpdateMethod = null;
        lowMemory = null;
        onGUI = null;
        onApplicationPause = null;
    }
#endif

    public void OnEnter()
    {
        HitPointManager.HitPoint("game_open_loading");

        AppManager.LoadUIProgressBar();

        HitPointManager.HitPoint("game_load_script_start");

        LoadDll();

        HitPointManager.HitPoint("game_load_script_success");

        AppManager.InitGameProgress = 0.03f;

        _loadTask = System.Threading.Tasks.Task.Run(AsyncLoad);
    }

    public void OnExit()
    {
        if (_loadTask != null)
        {
            if (_loadTask.Status == System.Threading.Tasks.TaskStatus.Running)
            {
                _loadTask.Wait();
            }
            _loadTask.Dispose();
            _loadTask = null;
        }

        if (bStart)
        {
#if MONO_REFLECT_MODE || ILRUNTIME_MODE
            exitMethod?.Run();
#else
            Logic.Core.GameMain.Exit();
#endif
        }

        UnloadDll();
    }

    public void OnUpdate()
    {
        if (!bStart)
        {
            if (!bFunctionCreated)
            {
                if (_loadTask.IsCompleted)
                {
                    _loadTask.Dispose();
                    _loadTask = null;

                    CreateFunction();

                    AppManager.InitGameProgress = 0.06f;
                    bFunctionCreated = true;
                }
            }
            else
            {
                DoStart();
                AppManager.InitGameProgress = 0.1f;
                bStart = true;
            }

            return;
        }

        NetClient.Instance.Update();

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        updateMethod?.Run();
#else
        Logic.Core.GameMain.Update();
#endif        

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        AspectRotioController.Instance.Update();        
#endif        
    }

    public void OnLateUpdate()
    {
        if (!bStart)
            return;

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        lateUpdateMethod?.Run();
#else
        Logic.Core.GameMain.LateUpdate();
#endif
    }

    public void OnFixedUpdate()
    {
        if (!bStart)
            return;

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        fixedUpdateMethod?.Run();
#else
        Logic.Core.GameMain.FixedUpdate();
#endif
    }

    public void OnLowMemory()
    {
        if (!bStart)
            return;

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        lowMemory?.Run();
#else
        Logic.Core.GameMain.OnLowMemory();
#endif
    }

    public void OnGUI()
    {
        if (!bStart)
            return;

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        onGUI?.Run();
#else
        Logic.Core.GameMain.OnGUI();
#endif
    }

    public void OnApplicationPause(bool pause)
    {
        if (!bStart)
            return;

#if MONO_REFLECT_MODE || ILRUNTIME_MODE
        onApplicationPause?.Run(pause);
#else
        Logic.Core.GameMain.OnApplicationPause(pause);
#endif
    }
}
