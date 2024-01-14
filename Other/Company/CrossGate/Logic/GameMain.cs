using Framework;
using Lib.Core;
using Net;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Logic.Core
{
    public static class GameMain
    {
        public enum EState
        {
            None,
            Running,

            PreloadShader,
            PreloadTable,
            PreloadAsset,
            RegisterSystem,
            InitSystem,
        }

        private static EState eState = EState.None;
        private static System.Threading.Tasks.Task _loadCsvTask;
        //private static PerformanceCheck performanceCheck;
        public static CultureInfo beijingCulture = new CultureInfo("zh-cn");
        public static CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        
#if DEBUG_MODE
        static GameObject mDebugPanel = null;
        public static int LastExecuteTime = 0;
#endif

        public enum ERegion {
            MainLand, // 大陆
            TW, // 台湾
            HK, // 香港
            MO, // 澳门
            Other, // 其他
        }

        public static ERegion GetRegion() {
            ERegion r = ERegion.MainLand;
            if (currentCulture.Name.Equals("zh-CN", StringComparison.OrdinalIgnoreCase)) {
                r = ERegion.MainLand;
            } else if (currentCulture.Name.Equals("zh-TW", StringComparison.OrdinalIgnoreCase)) {
                r = ERegion.TW;
            } else if (currentCulture.Name.Equals("zh-HK", StringComparison.OrdinalIgnoreCase)) {
                r = ERegion.HK;
            } else if (currentCulture.Name.Equals("zh-MO", StringComparison.OrdinalIgnoreCase)) {
                r = ERegion.MO;
            } else {
                r = ERegion.Other;
            }

            return r;
        }

        public static ProcedureManager Procedure
        {
            get;
            private set;
        }

        public static FsmManager Fsm
        {
            get;
            private set;
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        private static Lib.AssetLoader.FilesDownloader filesDownloader;
#endif
        public static void Start()
        {
            SDKManager.SDKSetCanExitVariable();
            HitPointManager.HitPoint("game_load_config_start");

#if DEBUG_MODE            
            float timePoint = Time.realtimeSinceStartup;
#endif

            OptionManager.Instance.Init();
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "OptionManager.Instance.Init()", ref timePoint);
#endif


            UIManager.Init();
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "UIManager.Init()", ref timePoint);
#endif

            LevelManager.onLevelSwitchBegin += OnLevelSwitchBegin;
            LevelManager.onLevelSwitchEnd += OnLevelSwitchEnd;

            //TODO:这个需要处理关卡切换时的逻辑
            Fsm = new FsmManager();
            Procedure = new ProcedureManager();
            //Procedure.Initialize(Fsm
            //    , new ProcedureNormal()
            //    , new ProcedureFight()
            //    , new ProcedureCutScene()
            //    , new ProcedureInteractive());
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "Procedure.Initialize()", ref timePoint);
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
            filesDownloader = Lib.AssetLoader.AssetMananger.Instance.CopyToPersistent(CSVRegister.GetAllFiles());
#endif

            ShaderManager.StartLoad();
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "ShaderManager.StartLoad()", ref timePoint);
#endif

            eState = EState.PreloadShader;
        }
        private static void OnLevelSwitchBegin(Type scriptType, LevelParams levelParams)
        {
            if (levelParams.eLoadingType == ELoadingType.eNormal)
            {
                SDKManager.SDK_SetGameFightStatus(true);
                UIManager.OpenUI(EUIID.UI_Loading);
                UIManager.UpdateState();
            }
        }

        private static void OnLevelSwitchEnd(Type scriptType, LevelParams levelParams)
        {
            if (levelParams.eLoadingType == ELoadingType.eNormal)
            {
                //if (scriptType == typeof(LvPlay))
                //{
                //    //先预加载 loadingUI 直到进lvplay后删除
                //    UIManager.CloseUI(EUIID.UI_Loading, true);
                //}
                //else
                //{
                //    UIManager.CloseUI(EUIID.UI_Loading, true, false);
                //}

                UIManager.CloseUI(EUIID.UI_Loading, true, false);

                //同步防沉迷的状态
                SDKManager.SDK_SetGameFightStatus(false);
            }
        }

        private static void InitBaseManager()
        {
            // 设置安全区域
            CanvasPropertyOverrider.Clear();
            foreach (var kvp in Table.CSVDevice.Instance.GetAll())
            {
                if (!string.IsNullOrEmpty(kvp.name)) {
                    CanvasPropertyOverrider.HandleRight(true);
                    CanvasPropertyOverrider.AddSpecialAreas(kvp.name, kvp.x, kvp.y, kvp.width, kvp.height);
                }
            }

#if DEBUG_MODE
            float timePoint = Time.realtimeSinceStartup;
#endif

            TimelineManager.Init();
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "TimelineManager.Init()", ref timePoint);
#endif

            SceneManager.RegisterCallback();
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "SceneManager.RegisterCallback()", ref timePoint);
#endif
            
            AudioManager.Instance.Init();
            // 如果想热更修改开关，可以修改这里
            AudioManager.Instance.use3dSound = true;
            AudioManager.Instance.usePHASEInIOS = SDKManager.GetiOSPhaseCanPlay();
#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "AudioManager.Instance.Init()", ref timePoint);
#endif
        }

        public static void Onloaded()
        {
#if DEBUG_MODE
#if UNITY_EDITOR
            //收集从表格加载完到结束loaded使用GetAll的表格==》
            System.Collections.Generic.HashSet<string> ss = Framework.Table.TableDebugUtil.EndCollectGetAll();

            System.Text.StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            foreach (string s in ss)
            {
                stringBuilder.Append(s);
                stringBuilder.Append(".Instance.GetAll();\n");
            }
            DebugUtil.Log(ELogType.eTable, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder));
            //收集从表格加载完到结束loaded使用GetAll的表格==《
#endif

            mDebugPanel = new GameObject("DebugPanel");
            mDebugPanel.AddComponent<DebugPanel>();
            UnityEngine.Object.DontDestroyOnLoad(mDebugPanel);
#endif

            UIManager.OpenUI(EUIID.UI_Hint);
            AppManager.DestroyUIProgressBar();
        }

        public static void Update()
        {
#if DEBUG_MODE
            float timePoint = Time.realtimeSinceStartup;
#endif
            switch (eState)
            {
                case EState.Running:
                    {
                        SystemModuleManager.OnUpdate();
                        LevelManager.Update();
                        Fsm.Update();
                        UIManager.Update();
                        OptionManager.Instance.WriteIfDirty();
                    }
                    break;

                case EState.PreloadShader:
                    {
                        //CSVRegister.UpdateLoad();
                        //TODO ： 需要状态控制
#if !UNITY_EDITOR && UNITY_ANDROID
                        if (ShaderManager.bLoaded && filesDownloader.isFinish)
#else
                        if (ShaderManager.bLoaded)
#endif
                        {
#if !UNITY_EDITOR && UNITY_ANDROID
                            filesDownloader = null;
#endif
                            SetDefaultQualityLevel();

                            _loadCsvTask = System.Threading.Tasks.Task.Run(AsyncLoad);
                            //System.Threading.ThreadPool.QueueUserWorkItem(AsyncLoad);
                            eState = EState.PreloadTable;

                            AppManager.InitGameProgress = 0.3f;
                        }
                        else
                        {
#if !UNITY_EDITOR && UNITY_ANDROID
                            AppManager.InitGameProgress = filesDownloader.Progress * 0.2f + 0.1f;
#endif
                        }
                    }
                    break;

                case EState.PreloadTable:
                    {
                        if ((_loadCsvTask.IsCompleted || _loadCsvTask.IsFaulted)
                            && CSVRegister.isFinished)
                        {
#if DEBUG_MODE && UNITY_EDITOR
                            Framework.Table.TableDebugUtil.BegineCollectGetAll();
#endif
                            _loadCsvTask.Dispose();
                            _loadCsvTask = null;

                            HitPointManager.HitPoint("game_load_config_success");

                            //GlobalAssets.Preload();
                            AppManager.InitGameProgress = 0.8f;
                            eState = EState.PreloadAsset;                            
                        }
                        else
                        {
                            //TODO:临时判断等线程执行需要时间 所以我就按复制进度是1的时候当成csv解析进度是0，
                            //等下次改打表工具的时候一并改到CSVRegister逻辑里面
                            AppManager.InitGameProgress = CSVRegister.Progress * 0.5f + 0.3f;
                        }
                    }
                    break;

                case EState.PreloadAsset:
                    {
                        InitBaseManager();

                        AppManager.InitGameProgress = 0.81f;
                        eState = EState.RegisterSystem;
                    }
                    break;

                case EState.RegisterSystem:
                    {                        
                        //SystemModuleManager.Register();
                        SystemModuleManager.StartInit();
                        eState = EState.InitSystem;
                    }
                    break;

                case EState.InitSystem:
                    {
                        if (SystemModuleManager.InitProgress())
                        {
                            Onloaded();
                            LevelManager.EnterLevel(typeof(LvLogin), new LevelParams() { eLoadingType = ELoadingType.eUnuseLoading, arg = null, bCanSwitchToEqualLevelType = false });

                            UIManager.PreloadUI(EUIID.UI_Login);//先预加载登录界面 保证登录界面资源优先加载

                            GlobalAssets.Preload();

                            eState = EState.Running;
                            AppManager.InitGameProgress = 1f;
                        }
                        else
                        {
                            AppManager.InitGameProgress = SystemModuleManager.Progress() * 0.18f + 0.81f;
                        }
                    }
                    break;
            }
#if DEBUG_MODE
            float ddt = UnityEngine.Time.realtimeSinceStartup - timePoint;
            LastExecuteTime = (int)(ddt * 1000000);
#endif
        }

        public static void LateUpdate()
        {
            if (eState == EState.Running)
            {
                LevelManager.LateUpdate();
                Logic.Core.UIScheduler.Update();
                UIManager.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
            else if (eState == EState.PreloadAsset)
            {
                Logic.Core.UIScheduler.Update();
                UIManager.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        public static void FixedUpdate()
        {

        }

        public static bool hasExited { get; private set; } = false;

        public static void Exit()
        {
#if DEBUG_MODE && UNITY_EDITOR
            Framework.Table.TableDebugUtil.EndCollectGetAll();
#endif

            if (_loadCsvTask != null)
            {
                if (_loadCsvTask.Status == System.Threading.Tasks.TaskStatus.Running)
                {
                    _loadCsvTask.Wait();
                }
                _loadCsvTask.Dispose();
                _loadCsvTask = null;
            }

#if !UNITY_EDITOR && UNITY_ANDROID
            filesDownloader?.Close();
#endif            
            UIManager.UnInit();
            EventDispatcher.Instance.Clear();
            SystemModuleManager.UnInit();
            SystemModuleManager.Unregister();
            PoolManager.DisposeAll();
            CSVRegister.Unload();
            AudioManager.Instance.UnInit();
            ShaderManager.UnInit();
            FontManager.UnInit();
            Procedure.Shutdown();
            Fsm.Shutdown();
            GlobalAssets.Unload();

            hasExited = true;
        }

        public static void OnLowMemory()
        {
            SystemModuleManager.OnLowMemory();
            LevelManager.OnLowMemory();
            PoolManager.DisposeAll();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public static void OnGUI()
        {
            UIManager.OnGUI_UIStack();
            //PoolManager.Debug();
        }

        public static void OnApplicationPause(bool pause)
        {
            SystemModuleManager.OnApplicationPause(pause);
        }

        private static void AsyncLoad()
        {
            try
            {
                CSVRegister.Load();

                //在线程中把 loading期间需要遍历的表格先全部加载,减少卡顿
                //Table.CSVDevice.Instance.GetAll();
                //Table.CSVFamilySkillUp.Instance.GetAll();
                //Table.CSVPedigreedDraw.Instance.GetAll();
                //Table.CSVRedEnvelopRain.Instance.GetAll();
                //Table.CSVBattlePassUpgrade.Instance.GetAll();
                //Table.CSVDrop.Instance.GetAll();
                //Table.CSVOrnamentsUpgrade.Instance.GetAll();
                //Table.CSVFunctionOpen.Instance.GetAll();
                //Table.CSVGoddessTopic.Instance.GetAll();
                //Table.CSVAdventureCriminal.Instance.GetAll();
                //Table.CSVAdventureProgress.Instance.GetAll();
                //Table.CSVFirstCharge.Instance.GetAll();
                //Table.CSVActivityTarget.Instance.GetAll();
                //Table.CSVCumulativeReward.Instance.GetAll();
                //Table.CSVCook.Instance.GetAll();
                //Table.CSVCommodityList.Instance.GetAll();
                //Table.CSVFamilyReception.Instance.GetAll();
                //Table.CSVIndustryTask.Instance.GetAll();
                //Table.CSVDailyActivity.Instance.GetAll();
                //Table.CSVBOOSFightPlayMode.Instance.GetAll();
                //Table.CSVLifeSkillLevel.Instance.GetAll();
                //Table.CSVFashionSuit.Instance.GetAll();
                //Table.CSVFashionClothes.Instance.GetAll();
                //Table.CSVFashionAccessory.Instance.GetAll();
                //Table.CSVFashionWeapon.Instance.GetAll();
                //Table.CSVInstance.Instance.GetAll();
                //Table.CSVInstanceDaily.Instance.GetAll();
                //Table.CSVGuideGroup.Instance.GetAll();
                //Table.CSVPartnerOccupation.Instance.GetAll();
                //Table.CSVHorn.Instance.GetAll();
                //Table.CSVItem.Instance.GetAll();
                //Table.CSVCommand.Instance.GetAll();
                //Table.CSVAiBubbleChat.Instance.GetAll();
                //Table.CSVAnnouncement.Instance.GetAll();

                SystemModuleManager.Register();
            }
            catch (Exception e)
            {
                DebugUtil.LogException(e);
            }
        }

        private static void SetAndroidQuality()
        {
            string deviceModel = SystemInfo.deviceModel;
            bool isEmulator = SDKManager.SDKISEmulator();

            Stream steam = Lib.AssetLoader.AssetMananger.Instance.LoadStream("Config/DeviceQualityLevel.txt");
            StreamReader sr = new StreamReader(steam, System.Text.Encoding.UTF8);

            //第一行配置分数线 
            int scoreMiddle = 0;
            int scoreHigh = 0;
            string line = sr.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] ss = line.Split('|');
                if (ss.Length > 1)
                {
                    int.TryParse(ss[0], out scoreHigh);
                    int.TryParse(ss[1], out scoreMiddle);
                }
            }

            int performanceScore = AppManager.nPerformanceScore;
            int qualityLevel = 0;

            //根据评分设置质量等级
            if (performanceScore >= scoreHigh)
            {
                qualityLevel = 2;
            }
            else if (performanceScore >= scoreMiddle)
            {
                qualityLevel = 1;
            }
            else
            {
                qualityLevel = 0;
            }

            if (!isEmulator)
            {
                //下面配置特定机型预设参数
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)
                        || (!line.StartsWith(deviceModel, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    string[] ss = line.Split('|');
                    if (ss.Length < 2)
                    {
                        break;
                    }

                    if (int.TryParse(ss[1], out qualityLevel))
                    {
                        break;
                    }
                }
            }

            sr.Dispose();
            sr.Close();
            steam.Dispose();
            steam.Close();

            
            if (isEmulator)
            {
                //根据策划强烈要求，模拟器最低默认设置为 中
                qualityLevel = Mathf.Max(qualityLevel, (int)EQuality.Middle);
            }

            DebugUtil.LogFormat(ELogType.eNone, "{0} isEmulator = {1} performance_score = {2}; recommend_quality = {3}",
                SystemInfo.deviceModel,
                isEmulator.ToString(),
                performanceScore.ToString(),
                qualityLevel.ToString());

            OptionManager.Instance.SetRecommendQuality(qualityLevel, performanceScore);

            HitPointPerformanceScore hitPointPerformanceScore = new HitPointPerformanceScore();
            hitPointPerformanceScore.device_model = SystemInfo.deviceModel;
            hitPointPerformanceScore.system_version = SystemInfo.operatingSystem;
            hitPointPerformanceScore.graphics_device_version = SystemInfo.graphicsDeviceVersion;
            hitPointPerformanceScore.performance_score = performanceScore;
            hitPointPerformanceScore.recommend_quality = qualityLevel;
            hitPointPerformanceScore.is_emulator = isEmulator ? 1 : 0;
            HitPointManager.HitPoint("performance_score", hitPointPerformanceScore);
        }
        private static void SetIOSQuality()
        {
            //https://www.theiphonewiki.com/wiki/Models
            //iPhone 6 = iPhone7,2
            //iPhone 6 Plus = iPhone7,1
            //iPhone 6s = iPhone8,1
            //iPhone 7 = iPhone9,1
            //iPhone 8 = iPhone10,1

            string deviceModel = SystemInfo.deviceModel;

            if (deviceModel.StartsWith("iPhone", StringComparison.Ordinal))
            {
                string s = deviceModel.Remove(0, "iPhone".Length);

                int index = s.IndexOf(',');
                if (index > -1)
                {
                    s = s.Remove(index);
                }

                if (int.TryParse(s, out int iphoneType) && iphoneType < 10)
                {
                    OptionManager.Instance.SetRecommendQuality((int)EQuality.Low, 0);
                }
                else
                {
                    OptionManager.Instance.SetRecommendQuality((int)EQuality.High, 0);
                }
            }
            else if (deviceModel.StartsWith("iPad", StringComparison.Ordinal))
            {
                string s = deviceModel.Remove(0, "iPad".Length);

                int index = s.IndexOf(',');
                if (index > -1)
                {
                    s = s.Remove(index);
                }

                if (int.TryParse(s, out int ipadType) && ipadType < 11)
                {
                    OptionManager.Instance.SetRecommendQuality((int)EQuality.Low, 0);
                }
                else
                {
                    OptionManager.Instance.SetRecommendQuality((int)EQuality.High, 0);
                }
            }
            else
            {
                OptionManager.Instance.SetRecommendQuality((int)EQuality.High, 0);
            }
        }

        private static void SetDefaultQualityLevel()
        {            
#if UNITY_ANDROID
            SetAndroidQuality();
#elif UNITY_IOS
            SetIOSQuality();
#else
            OptionManager.Instance.SetRecommendQuality((int)EQuality.High, 0);
#endif
        }
    }
}
