using Framework;
using Lib.AssetLoader;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    sealed public class LvPlay : LevelBase
    {
        //private int nLevelState = 0;
        private int nLoadStage = 0;        
        private float fStartExitMapTime = 0;
        //TODO 写到LvPlay 数据类 GameCenter EPlayState
        //private bool bLoading = true;

        //玩家输入控制
        public PlayerControlSystem mPlayerControlSystem = null;

        public PathFindControlSystem mPathFindControlSystem = null;

        public WayPointSystem mWayPointSystem = null;
        // 在线自动挂机检测
        public HangUpTipsSystem mHangUpTipsSystem = null;
        // 在线自动挂机检测提示框
        public HangUpSystem mHangUpSystem = null;

        public RoleActionSystem mRoleActionSystem = null;

        public StateSystem mStateSystem = null;

        public FollowSystem mFollowSystem = null;
        //移动
        public MovementSystem mMovementSystem = null;
        //上传主角位置
        public UploadTransformSystem mUploadTransformSystem = null;
        //检测附加可交互的Npc
        public NearbyNpcSystem mNearbyNpcSystem = null;
        //传送门检测
        public CheckTeleporterSystem mCheckTeleporterSystem = null;
        //其他玩家可见性检测
        public CheckHeroVisualSystem mCheckHeroVisualSystem = null;
        //头顶显示
        public NPCHUDSystem mNPCHUDSystem = null;

        public NpcActiveListenerSystem mNpcActiveListenerSystem = null;

        public NPCActionListenerSystem mNPCActionListenerSystem = null;

        public ActiveMonsterSystem mActiveMonsterSystem = null;

        public NPCAreaCheckSystem mNPCAreaCheckSystem = null;

        public WorldBossSystem mWorldBossSystem = null;

        public VirtualNpcSystem mVirtualNpcSystem = null;
        // 检测主角和任务npc的位置关系，便于接受/追踪任务
        public TaskListenerSystem mTaskListenerSystem = null;

        public PetControlSystem mPetControlSystem = null;
        //地图检测
        public SeamlessMapSystem mSeamlessMapSystem = null;
        //同步主角位置到shader
        public SendPositionToShaderSystem mSendPositionToShader = null;

#if DEBUG_MODE
        public int LastExecuteFrame = 0;
        public int LastExecuteTime = 0; //microsecond 微秒
        public string name;
#endif

        private void CreateSystems()
        {
            mLevelSystems = new List<LevelSystemBase>(24);
            mLevelSystemDic = new Dictionary<Type, LevelSystemBase>(24);
                        
            mPlayerControlSystem = GetOrCreateSystem<PlayerControlSystem>();
            mPathFindControlSystem = GetOrCreateSystem<PathFindControlSystem>();
            mWayPointSystem = GetOrCreateSystem<WayPointSystem>();
            mHangUpTipsSystem = GetOrCreateSystem<HangUpTipsSystem>();
            mHangUpSystem = GetOrCreateSystem<HangUpSystem>();

            mRoleActionSystem = GetOrCreateSystem<RoleActionSystem>();
            mStateSystem = GetOrCreateSystem<StateSystem>();
            mFollowSystem = GetOrCreateSystem<FollowSystem>();
            mMovementSystem = GetOrCreateSystem<MovementSystem>();

            mUploadTransformSystem = GetOrCreateSystem<UploadTransformSystem>();
            mNearbyNpcSystem = GetOrCreateSystem<NearbyNpcSystem>();
            mCheckTeleporterSystem = GetOrCreateSystem<CheckTeleporterSystem>();            
            mCheckHeroVisualSystem = GetOrCreateSystem<CheckHeroVisualSystem>();

            mNPCHUDSystem = GetOrCreateSystem<NPCHUDSystem>();
            mNpcActiveListenerSystem = GetOrCreateSystem<NpcActiveListenerSystem>();                        
            mNPCActionListenerSystem = GetOrCreateSystem<NPCActionListenerSystem>();
            mActiveMonsterSystem = GetOrCreateSystem<ActiveMonsterSystem>();
            mNPCAreaCheckSystem = GetOrCreateSystem<NPCAreaCheckSystem>();
            mWorldBossSystem = GetOrCreateSystem<WorldBossSystem>();
            mVirtualNpcSystem = GetOrCreateSystem<VirtualNpcSystem>();
            mTaskListenerSystem = GetOrCreateSystem<TaskListenerSystem>();
            mPetControlSystem = GetOrCreateSystem<PetControlSystem>();
            mSeamlessMapSystem = GetOrCreateSystem<SeamlessMapSystem>();
            mSendPositionToShader = GetOrCreateSystem<SendPositionToShaderSystem>();

            for (int i = 0; i < mLevelSystems.Count; ++i)
            {
                LevelSystemBase levelSystem = mLevelSystems[i];
                levelSystem.offsetFrame = i;
                levelSystem.isActive = false;
            }
        }

        private void DestroySystems()
        {
            for (int i = 0; i < mLevelSystems.Count; ++i)
            {
                LevelSystemBase levelSystem = mLevelSystems[i];
                levelSystem.OnDestroy();
            }
            mLevelSystems.Clear();

            mSeamlessMapSystem = null;
            mPlayerControlSystem = null;
            mPathFindControlSystem = null;
            mNearbyNpcSystem = null;
            mCheckTeleporterSystem = null;
            mUploadTransformSystem = null;
            mCheckHeroVisualSystem = null;
            mWayPointSystem = null;
            mHangUpTipsSystem = null;
            mHangUpSystem = null;
            mRoleActionSystem = null;
            mSendPositionToShader = null;
            mFollowSystem = null;
            mMovementSystem = null;
            mNpcActiveListenerSystem = null;
            mStateSystem = null;
            mNPCHUDSystem = null;
            mNPCActionListenerSystem = null;
            mActiveMonsterSystem = null;
            mNPCAreaCheckSystem = null;
            mWorldBossSystem = null;
            mVirtualNpcSystem = null;
            mTaskListenerSystem = null;
            mPetControlSystem = null;
        }

        public override void OnEnter(LevelParams param, Type fromLevelType)
        {
            base.OnEnter(param, fromLevelType);

            GameCenter.mLvPlay = this;

            HitPointManager.HitPoint("game_server_enter");

            //mLevelPreload.Preload<GameObject>(UIConfig.GetConfData(EUIID.UI_Loading2).prefabPath);
            UIManager.PreloadUI(EUIID.UI_CutScenePre);

            UIManager.PreloadUI(EUIID.UI_MainInterface);
            UIManager.PreloadUI(EUIID.UI_Menu);

            UIManager.PreloadUI(EUIID.UI_Loading2);
            
            //UIManager.PreloadUI(EUIID.HUD);
            //UIManager.PreloadUI(EUIID.UI_Trade);
            //UIManager.PreloadUI(EUIID.UI_Equipment);
            //UIManager.PreloadUI(EUIID.UI_OperationalActivity);
            
            AnimationClipProload.Preload(mLevelPreload);

            mLevelPreload.PreOpenScene(EScene.World.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);

            mLevelPreload.StartLoad();

            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, true);            

            CreateSystems();

            GameCenter.nLoadStage = 0;
        }

        private void DistinguishSwitchMap()
        {
            if (Sys_Map.Instance.switchMode == Sys_Map.ESwitchMapUI.UILoading2)
            {

                if (Sys_Net.Instance.bDefaultConnected)
                    Sys_Net.Instance.bDefaultConnected = false;
                else
                    UIManager.OpenUI(EUIID.UI_Loading2, true);

                UIManager.UpdateState();
            }
            //else if (Sys_Map.Instance.switchMode == Sys_Map.ESwitchMapUI.NoLoading) {
            //    // 不打开loadingUI
            //    UIManager.UpdateState();
            //}
        }

        private void OnEnterMap()
        {
            if (Sys_Map.Instance.CurMapId != 1401u)
            {
                //TODO:打开过度UI
                DistinguishSwitchMap();
                GameCenter.nLoadStage = -1;
                fStartExitMapTime = Time.unscaledTime;
            }
            else
            {
                mSeamlessMapSystem.SetPosition(Sys_Map.Instance.CurMapId, new Vector2(Sys_Map.Instance.svrPosX / 100f, -Sys_Map.Instance.svrPosY / 100f));
                GameCenter.nLoadStage = 0;
            }
        }

        public override float OnLoading()
        {
            if (nLoadStage == 0) //0.4
            {
                mLevelPreload.OnLoading();

                if (mLevelPreload.CheckLoaded())
                {
                    mSeamlessMapSystem.SetPosition(Sys_Map.Instance.CurMapId, new Vector2(Sys_Map.Instance.svrPosX / 100f, -Sys_Map.Instance.svrPosY / 100f));

                    GameMain.Procedure.Shutdown();
                    GameMain.Procedure.Initialize(GameMain.Fsm, new ProcedureNormal(), new ProcedureFight(), new ProcedureCutScene(), new ProcedureInteractive());
                    nLoadStage = 1;

                    return 0.4f;
                }

                return mLevelPreload.fProgress * 0.4f;
            }
            else if (nLoadStage == 1) //0.6f
            {
                if (mSeamlessMapSystem.IsMainSceneLoaded())
                {
                    nLoadStage = 2;
                    GameCenter.InitWorld();
                }

                return 0.6f;
            }
            else if (nLoadStage == 2)
            {
                if (GameCenter.IsInitialFinish)
                {
                    nLoadStage = 3;
                    OnExcuteCutScene();
                }

                GameCenter.OnExcuteActions();
                return GameCenter.InitialProgress * 0.35f + 0.6f;
            }
            else if (nLoadStage == 3)
            {
                if (!Sys_Role.Instance.hasSyncFinished)
                {
                    Debug.LogError("hasSyncFinished is false----");
                    return 1f;
                }
            }

            return 1f;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

#if UNITY_ANDROID || UNITY_IPHONE
            SDKManager.SDKPaiLianTu();
#endif

#if UNITY_ANDROID
            //选择角色进入后乐变小包设置
            if (SDKManager.SDKLeBianSmallApk())
            {
                if (NetworkHelper.IsWifi())
                {
                    if (Sys_Role.Instance.Role.Level >= 20)//20 可以配置 -老用户开启一次性下载弹框提示
                    {
                        SDKManager.SDKLeBianDownloadFullRes();
                    }
                    else
                    {
                        SDKManager.SDKLeBianPlayLoadOpen();
                    }
                }
                else
                {
                    if (Sys_Role.Instance.Role.Level >= 20)//20 可以配置 -老用户开启一次性下载弹框提示
                    {
                        SDKManager.SDKLeBianDownloadFullRes();
                    }
                    else
                    {
                        //默认true：关闭对移动网络情况下乐变边玩边下的控制
                        if (SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.CloseLeBianPlayLoadInMobile.ToString(), out string paramsValue))
                        {
                            if (SDKManager.SDKLeBianNeedDownLoadTotalSize(out string tempFile) > 0)
                            {
                                PromptBoxParameter.Instance.Clear();
                                PromptBoxParameter.Instance.title = CSVLanguage.Instance.GetConfData(2106008).words;
                                PromptBoxParameter.Instance.content = string.Format(CSVLanguage.Instance.GetConfData(2106009).words, tempFile);
                                PromptBoxParameter.Instance.SetConfirm(true, () => {
                                    SDKManager.SDKLeBianPlayLoadOpen();
                                }, 2106010);//"后台下载"
                                PromptBoxParameter.Instance.SetCancel(true, null, 2106011);//以后再说
                                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                            }
                        }
                        else
                        {
                            SDKManager.SDKLeBianPlayLoadOpen();
                        }
                    }
                }
            }
#endif
        }

        private void OnExcuteCutScene()
        {
            ShowSceneActors(false);
            Sys_CutScene.Instance.TryDoCreateCharacterCutScene(Sys_CutScene.CreateRole_CUtsceneId, (_, __) =>
            {
                //先开UI
                UIManager.OpenUI(EUIID.UI_MainInterface);
                UIManager.OpenUI(EUIID.UI_HornHUD);
                ShowSceneActors(true);
            });
        }

        private void ShowSceneActors(bool isShow)
        {
            int cullingMask = CameraManager.GetMainCameraCullingMask();

            if (isShow)
            {
                cullingMask |= (int)ELayerMask.OtherActor;
                cullingMask |= (int)ELayerMask.NPC;
                cullingMask |= (int)ELayerMask.Player;
                cullingMask |= (int)ELayerMask.Partner;
                cullingMask &= ~(int)ELayerMask.Monster;
                cullingMask &= ~(int)ELayerMask.HidingSceneActor;
            }
            else
            {
                cullingMask &= ~(int)ELayerMask.OtherActor;
                cullingMask &= ~(int)ELayerMask.NPC;
                cullingMask &= ~(int)ELayerMask.Player;
                cullingMask &= ~(int)ELayerMask.Partner;
                cullingMask |= (int)ELayerMask.Monster;
                cullingMask |= (int)ELayerMask.HidingSceneActor;
            }

            CameraManager.SetMainCameraCullingMask(cullingMask);
        }

        public override void OnUpdate()
        {
            switch (GameCenter.nLoadStage)
            {
                case -1:
                    {
                        if (Time.unscaledTime - fStartExitMapTime >= GameCenter.fSwitchMapFadeOutTime + 0.05f)
                        {
                            mSeamlessMapSystem.SetPosition(Sys_Map.Instance.CurMapId, new Vector2(Sys_Map.Instance.svrPosX / 100f, -Sys_Map.Instance.svrPosY / 100f));
                            GameCenter.nLoadStage = 0;
                        }
                    }
                    break;
                case 0:
                    {
                        //场景加载完成的处理
                        //nloadstage = 1,2, 是隔两帧去保证当前navmesh是新地图navmesh,再去处理entermap
                        if (mSeamlessMapSystem.IsMainSceneLoaded())
                        {
                            GameCenter.nLoadStage = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        GameCenter.nLoadStage = 2;
                        AssetMananger.Instance.UnloadUnusedAssets(true);
                    }
                    break;
                case 2:
                    {
                        GameCenter.nLoadStage = 3;                        

                        CSVMapInfo.Data data = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
                        if (data != null)
                        {
                            AudioUtil.PlayAudio(data.sound_bgm);
                        }

                        Sys_Map.Instance.LoadOKReq();
                        GameCenter.EnterMap();

                        //GameCenter.CreateCacheParnters();
                        GameCenter.CreateSuperHero(1080);

                        //Transform transform = GameCenter.mainHero?.transform;
                        //GameCenter.mCameraController.virtualCamera.lookPoint = transform ? transform.position : Sys_Map.Instance.EnterPos;
                    }
                    break;
                case 3:
                    {                        
#if false
#if USE_SPLIT_FRAME
                        int index = TimeManager.GetFrameSplitCount() - 12 - 1;
                        index += 4;
                        if (TimeManager.CanExecute(index))
                        {
#endif
                        GameCenter.UpdateSystem();
#if USE_SPLIT_FRAME
                        }
#endif
#endif
                    }
                    break;

                default:
                    break;
            }

#if DEBUG_MODE
            float t = UnityEngine.Time.realtimeSinceStartup;
#endif

            CombatManager.Instance.OnUpdate();

#if DEBUG_MODE
            int ddt = (int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000);
            if (ddt >= 1000)
            {
                LastExecuteTime = ddt;
                LastExecuteFrame = UnityEngine.Time.frameCount;
            }                        
#endif

            //TODO:调度时机优化
            ActionCtrl.Instance.Update();
            CollectionCtrl.Instance.Update();
        }

        public override void OnExit(Type toLevelType)
        {
            nLoadStage = 0;

            SDKManager.UninitGMESDK();

            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, false);

            DestroySystems();

            SceneManager.UnLoadAllScene();

            if (!this.mType.Equals(toLevelType))
            {
                base.OnExit(toLevelType);
            }

            GameCenter.mLvPlay = null;
        }

#if DEBUG_MODE

        Vector3 templookPoint = new Vector3(14f, -1.1f, -8.52f);

        float duration;
        Vector3 strength = Vector3.zero;
        int vibrato = 10;
        float randomness = 90;
        bool fadeOut = true;

        public override void OnGUI()
        {
            float scale = Screen.height / 720f;
            Rect safeArea = Screen.safeArea;
            Rect HierarchyRect = new Rect(safeArea.x + 390 * scale, safeArea.y, safeArea.width - 780 * scale, safeArea.height);

            //GUILayout.BeginArea(HierarchyRect, GUI.skin.box);
            //mJoystickInput.enabled = GUILayout.Toggle(mJoystickInput.enabled, "虚拟摇杆");
            //Sys_Input.Instance.bForbidTouch = GUILayout.Toggle(Sys_Input.Instance.bForbidTouch, "禁用点击");

            GUILayout.Label(string.Format("Pith(x) = {0}", GameCenter.mCameraController.virtualCamera.pith.ToString()));
            GameCenter.mCameraController.virtualCamera.pith = Mathf.Floor(GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.pith, 0, 360));

            GUILayout.Label(string.Format("Yaw(y) = {0}", GameCenter.mCameraController.virtualCamera.yaw.ToString()));
            GameCenter.mCameraController.virtualCamera.yaw = Mathf.Floor(GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.yaw, 0, 360));

            GUILayout.Label(string.Format("Roll(z) = {0}", GameCenter.mCameraController.virtualCamera.roll.ToString()));
            GameCenter.mCameraController.virtualCamera.roll = Mathf.Floor(GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.roll, 0, 360));

            GUILayout.Label(string.Format("Distance = {0}", GameCenter.mCameraController.virtualCamera.distance.ToString()));
            GameCenter.mCameraController.virtualCamera.distance = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.distance, 0, 100);

            GUILayout.Label(string.Format("Fov = {0}", GameCenter.mCameraController.virtualCamera.fov.ToString()));
            GameCenter.mCameraController.virtualCamera.fov = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.fov, 4, 179);

            if (GameCenter.mCameraController.virtualCamera.TargetCamera != null)
            {
                GUILayout.Label(string.Format("Near = {0}", GameCenter.mCameraController.virtualCamera.TargetCamera.nearClipPlane.ToString()));
                GameCenter.mCameraController.virtualCamera.TargetCamera.nearClipPlane = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.TargetCamera.nearClipPlane, 0.001f, 100f);
            }

            GUILayout.Label(string.Format("Look Target Offset = {0}", GameCenter.mCameraController.virtualCamera.lookPointOffset.ToString()));
            float x = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.lookPointOffset.x, 0, 10);
            float y = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.lookPointOffset.y, 0, 10);
            float z = GUILayout.HorizontalSlider(GameCenter.mCameraController.virtualCamera.lookPointOffset.z, 0, 10);
            GameCenter.mCameraController.virtualCamera.lookPointOffset = new Vector3(x, y, z);

            templookPoint = GUIExtension.Vector3Field("templookPoint", templookPoint);

            GameCenter.mCameraController.virtualCamera.fixedLookPoint = templookPoint;
            GameCenter.mCameraController.virtualCamera.Recalculation();

            GUILayout.Label(string.Format("Look Target = {0}", GameCenter.mCameraController.virtualCamera.fixedLookPoint.ToString()));

            duration = GUIExtension.FloatField("duration", duration);
            strength = GUIExtension.Vector3Field("strength", strength);
            vibrato = GUIExtension.IntField("vibrato", vibrato);
            randomness = GUIExtension.FloatField("randomness", randomness);
            fadeOut = GUILayout.Toggle(fadeOut, "fadeOut");

            if (GUILayout.Button("震动"))
            {
                GameCenter.mCameraController.DoShake(duration, strength, vibrato, randomness, fadeOut);
            }

            //GUILayout.EndArea();
        }
#endif

    }
}