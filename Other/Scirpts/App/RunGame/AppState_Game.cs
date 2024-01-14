using Framework;
using Lib;
using Logic;

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

    private bool bFunctionCreate = false;
    private bool bStart = false;
    private static System.Threading.Tasks.Task _loadTask;

    private void LoadDll()
    {
#if DEBUG_MODE
        float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif
        assembly = new InnerAssembly();
        assembly.Load();
        AssemblyManager.Instance.RegisterAssembly(assembly);
#if DEBUG_MODE
        DebugUtil.LogTimeCost(ELogType.eNone, "LoadDll()", ref timePoint);
#endif
    }

    private void AsyncLoad()
    {

    }

    private void CreateFunction()
    {
#if DEBUG_MODE
        float timePoint = UnityEngine.Time.realtimeSinceStartup;
        DebugUtil.LogTimeCost(ELogType.eNone, "assembly.CreateStaticMethod", ref timePoint);
#endif

        //Todo
        //LogicStaticMethodDispatcher.Init();
#if DEBUG_MODE
        DebugUtil.LogTimeCost(ELogType.eNone, "LogicStaticMethodDispatcher.Init()", ref timePoint);
#endif
    }

    private void DoStart()
    {
        GameMain.Start();
    }

    private void UnloadDll()
    {
        LogicStaticMethodDispatcher.UnInit();
        AssemblyManager.Instance.RegisterAssembly(null);
        assembly.Unload();
    }

    public void OnEnter()
    {
        //Todo
        //AppManager.LoadUIProgressBar();
        LoadDll();
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
            GameMain.Exit();
        }

        UnloadDll();
    }

    public void OnUpdate()
    {
        if (!bStart)
        {
            if (!bFunctionCreate)
            {
                if (_loadTask.IsCompleted)
                {
                    _loadTask.Dispose();
                    _loadTask = null;

                    CreateFunction();

                    AppManager.InitGameProgress = 0.06f;
                    bFunctionCreate = true;
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

        //Todo
        //NetClient.Instance.Update();

        GameMain.Update();
    }

    public void OnLateUpdate()
    {
        if (!bStart)
            return;

        GameMain.LateUpdate();
    }

    public void OnFixedUpdate()
    {
        if (!bStart)
            return;

        GameMain.OnFixedUpdate();
    }

    public void OnLowMemory()
    {
        if (!bStart)
            return;

        GameMain.OnLowMemory();
    }

    public void OnGUI()
    {
        if (!bStart)
            return;

        GameMain.OnGUI();
    }

    public void OnApplicationPause(bool pause)
    {
        GameMain.OnApplicationPause(pause);
    }
}