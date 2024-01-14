using System;
using System.Collections.Generic;
using Framework;
using Lib.AssetLoader;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Timeline;
using UnityEngine.Video;
using System.IO;

namespace Logic {
    /// CutScene系统
    public partial class Sys_CutScene : SystemModuleBase<Sys_CutScene> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents {
            OnTrigger,

            OnStart,
            OnEnd,

            OnRealStart,
            OnLoaded,
            OnRealEnd,
        }

        public enum ETimelineTriggerType {
            Client,
            Server,
        }

        // 必须在得到RoleId后使用
        public string CreateCharacterCutScene {
            get {
                //return null;
                string roleId = Sys_Role.Instance.Role.RoleId.ToString();
                return PlayerPrefs.GetString(roleId + "CreateCharacterCutScene");
            }
            set {
                string roleId = Sys_Role.Instance.Role.RoleId.ToString();
                PlayerPrefs.SetString(roleId + "CreateCharacterCutScene", roleId);
            }
        }

        private Action<uint, uint> onRealEnd = null;
        private void OnRealEnd(bool result) {
            DebugUtil.LogFormat(ELogType.eCutScene, "result: {0}", result);
            uint currentCutSceneId = result ? this.csvCutscene.id : 0;
            this.onRealEnd?.Invoke(this.seriesCutSceneId, currentCutSceneId);
            this.onRealEnd = null;

            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "Sys_CutScene.OnActiveAllActorHUD");
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearCutSceenBubbles);
            this.eventEmitter.Trigger<uint, uint>(EEvents.OnRealEnd, this.seriesCutSceneId, currentCutSceneId);
        }
        private bool isCharacterLoaded = false;

        private bool isCharacterAnimationLoaded = false;
        private List<AnimationClip> animationClips = new List<AnimationClip>();
        private TimelineClip[] clips;

        public bool isPlaying { get; set; } = false;
        public bool isRealPlaying { get; set; } = false;

        private bool isReady = false;

        private GameObject gameObject;
        private AsyncOperationHandle<GameObject> mHandle;

        public AssetsGroupLoader assetsGroupLoader = new AssetsGroupLoader();

        public CSVCutScene.Data csvCutscene { get; private set; }

        private HeroLoader heroLoader;
        private GameObject mainPlayerModel;
        private TimelineDirector timelineDirector;
        private VirtualCameraAssigner virtualCameraAssigner;
        private AnimationTrackSetter animationTrackSetter;
        private VideoPlayer videoPlayer;
        public Camera timeline3DCamera;
        private List<GameObject> hideThings = new List<GameObject>();

        // seriesCutSceneId是联播cutscene的第一个id
        private uint seriesCutSceneId;
        private ETimelineTriggerType triggerType = ETimelineTriggerType.Server;

        //private float cutsceneBeginTime;

        public static uint CreateRole_CUtsceneId = 5000003;

        //公用workStream
        private WS_CommunalAIManagerEntity _communalAIManagerEntity;

        // 创角之后的cutscene
        // 将来是否会存在这种需要记录性质的cutscene
        public void TryDoCreateCharacterCutScene(uint cutSceneId, Action<uint, uint> onRealEndAction = null) {
            Action<uint, uint> onFinish = (_, __) => {
                this.CreateCharacterCutScene = string.Empty;
                onRealEndAction?.Invoke(_, __);
            };
            this.TryDoCutScene(cutSceneId, onFinish, string.IsNullOrEmpty(this.CreateCharacterCutScene) && Sys_Role.Instance.Role.Level <= 1 && Sys_Map.Instance.CurMapId == 1530);
            //this.TryDoCutScene(cutSceneId, onFinish, true);
        }
        // 客户端调用
        public void TryDoCutScene(uint cutSceneId, Action<uint, uint> onRealEndAction = null, bool condition = true) {
            this.onRealEnd = onRealEndAction;
            if (condition && !this.isPlaying) {
                DebugUtil.LogFormat(ELogType.eCutScene, "cutSceneId: {0}", cutSceneId);

                this.triggerType = ETimelineTriggerType.Client;
                this.seriesCutSceneId = cutSceneId;

                this.OnTrigger(cutSceneId);
            }
            else {
                this.OnRealEnd(false);
            }
        }

        private Timer timer;
        private void Play(uint cutsceneId) {
            this.hideThings.Clear();

            this.csvCutscene = CSVCutScene.Instance.GetConfData(cutsceneId);
            if (this.csvCutscene != null) {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight) {
                    GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterCutScene);
                }

                this.PrePlay(this.csvCutscene);

                this.timer?.Cancel();
                this.timer = Timer.Register(0.15f, () => {
                    string path = this.csvCutscene.path;
                    if (this.csvCutscene.type == 1) // 视频
                    {
                        path = "Prefab/Video/VideoPrefab.prefab";
                    }

                    //Debug.LogFormat("差錯日志 id: {0}, type:{1} path:{2}", this.csvCutscene.id, this.csvCutscene.type, path);
                    AddressablesUtil.InstantiateAsync(ref this.mHandle, path, this.OnAddressableAssetLoaded, true, TimelineManager.root);
                });
            }
        }
        private void PrePlay(CSVCutScene.Data csv) {
            this.isReady = false;
            if (csv.forEnter != 0) {
                var tp = new Tuple<int, int, Func<bool>, float>(1, csv.forEnter, () => {
                    return this.isReady;
                }, UI_CutScenePre.TOTAL);

                UIManager.OpenUI(EUIID.UI_CutScenePre, true, tp);
                UIManager.SendMsg(EUIID.UI_CutScenePre, tp);
                UIManager.UpdateState();
            }

            //this.cutsceneBeginTime = Time.time;

            // 关闭环境音
            AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.SceneSound, 0);
            if (csv.blockSingleMusic) {
                OptionManager.Instance.SetFloat(OptionManager.EOptionID.BGMValue, 0f, true);
            }

            this.HitPointCutscene(csv.id, 0);
            this.cutsceneStartTime = Sys_Time.Instance.GetServerTime();

            // 关闭断线重连UI
            Sys_Net.Instance.DisableReconnect(Sys_Net.EReconnectSwitchType.CutScene, true);
            this.eventEmitter.Trigger<uint, uint>(EEvents.OnStart, this.seriesCutSceneId, csv.id);
        }

        private uint cutsceneStartTime = 0;

        public void HitPointCutscene(uint cutsceneId, int status, bool isSkip = false) {

            HitPointCutScene hitPoint = new HitPointCutScene();
            hitPoint.plot_id = cutsceneId.ToString();
            hitPoint.is_skip = isSkip ? "1" : "0";
            hitPoint.scene_id = Sys_Map.Instance.CurMapId.ToString();
            if (status == 1)
                hitPoint.plot_duration = (Sys_Time.Instance.GetServerTime() - this.cutsceneStartTime).ToString();

            HitPointManager.HitPoint(HitPointCutScene.Key, hitPoint);
        }

        private void Start() {
            this.isRealPlaying = true;
            this.eventEmitter.Trigger<uint, uint>(EEvents.OnRealStart, this.seriesCutSceneId, this.csvCutscene.id);
        }
        private void End() {
            this.End(false);
        }

        // Cutscene结束的时候执行
        // manual为true表示手动点击跳过
        public void End(bool manual, bool force = false) {
            if (this.isPlaying && gameObject != null) {
                //Debug.LogFormat("差錯日志 cutscene end {0} {1} {2}", this.csvCutscene.id, this.csvCutscene.type, this.csvCutscene.path);
                this.HitPointCutscene(this.csvCutscene.id, 1, manual);

                AddressablesUtil.ReleaseInstance(ref this.mHandle, this.OnAddressableAssetLoaded);

                this.heroLoader?.Dispose();
                this.heroLoader = null;

                if (this.videoPlayer != null) {
                    this.videoPlayer.Stop();
                }
                if (this.gameObject != null) {
                    GameObject.Destroy(this.gameObject);
                }
                this.gameObject = null;

                this.timer?.Cancel();

                for (int i = 0, length = this.hideThings.Count; i < length; ++i) {
                    GameObject go = this.hideThings[i];
                    if (go != null) {
                        go.SetActive(true);
                    }
                }
                this.hideThings.Clear();

                if (UIManager.IsOpen(EUIID.UI_CutSceneTop)) {
                    UIManager.CloseUI(EUIID.UI_CutSceneTop);
                }

                // keep behind
                this.videoPlayer = null;
                this.gameObject = null;
                this.timeline3DCamera = null;

                // 清理上一个资源
                this.assetsGroupLoader.UnloadAll();
                this.animationClips.Clear();
                this.clips = null;
                this.animationTrackSetter?.Recovery();

                this.EndDissolve();
                this.mainPlayerModel = null;
                this.timelineDirector = null;
                this.virtualCameraAssigner = null;
                this.animationTrackSetter = null;

                // 通知天气
                this.data.isStart = false;
                this.data.weatherId = (uint)this.csvCutscene.weather;
                this.data.dayOrNightId = this.csvCutscene.dayNight;
                Sys_Weather.Instance.eventEmitter.Trigger<CutSceneWeatherData>(Sys_Weather.EEvents.OnCutSceneWeather, this.data);

                this.isRealPlaying = false;

                // 连续播放多个
                if (!manual && this.csvCutscene.nextId != 0) {
                    this.isPlaying = false;
                    this.OnTrigger(this.csvCutscene.nextId);
                }
                else {
                    // 每次播放完毕的时候，都开启断线重连，因为如果联网模式下，timline/cutscene最后一针请求server去exit的时候。没网的话，就会一直卡在最后一帧的界面表现上，所以需要断线重连出来
                    // 盖一下，刷新一下整体UI
                    Sys_Net.Instance.DisableReconnect(Sys_Net.EReconnectSwitchType.CutScene, false);

                    // 环境音
                    AudioManager.Instance.SetVolume((uint)AudioUtil.EAudioType.SceneSound, OptionManager.Instance.GetBoolean(OptionManager.EOptionID.Sound) ? OptionManager.Instance.GetFloat(OptionManager.EOptionID.SoundValue) : 1f);

                    OptionManager.Instance.CancelOverride(OptionManager.EOptionID.BGMValue);
                    AudioUtil.PlayMapBGM();

                    if (this.triggerType == ETimelineTriggerType.Server) {
                        if (Sys_Team.Instance.isCaptain() || !Sys_Team.Instance.HaveTeam || Sys_Team.Instance.isPlayerLeave()) {
                            if (this.isCutSceneTeleport) {
                                Sys_Map.Instance.IsTelState = true;
                                GameCenter.EnableMainHeroMove(false);
                            }

                            this.SkipCutscene(this.seriesCutSceneId, true);
                        }
                    }

                    this.isCutSceneTeleport = false;

                    if (!force) {
                        // 将来该调用在外部调用，需要区分是从战斗，还是normal进入的cutscene
                        if (GameMain.Procedure != null && GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight) {
                            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                        }
                        this.OnRealEnd(true);

                        if (this.csvCutscene.forExit != 0) {
                            //Sys_Map.Instance.switchMode = Sys_Map.ESwitchMapUI.NoLoading;
                            var tp = new Tuple<int, int, Func<bool>, float>(0, this.csvCutscene.forExit, () => {
                                return true;
                            }, UI_CutScenePre.TOTAL);
                            UIManager.OpenUI(EUIID.UI_CutScenePre, true, tp);
                            UIManager.SendMsg(EUIID.UI_CutScenePre, tp);
                            UIManager.UpdateState();
                            // fadeout之后设置
                            // this.isPlaying = false;
                        }
                        else {
                            this.isPlaying = false;

                            // 防止cutscene时长小于ui出现
                            if (UIManager.IsOpen(EUIID.UI_CutScenePre)) {
                                UIManager.CloseUI(EUIID.UI_CutScenePre, false, false);
                            }
                        }
                    }
                    else {
                        this.isPlaying = false;

                        // 防止cutscene时长小于ui出现
                        if (UIManager.IsOpen(EUIID.UI_CutScenePre)) {
                            UIManager.CloseUI(EUIID.UI_CutScenePre, false, false);
                        }
                    }
                }

                if (this._communalAIManagerEntity != null) {
                    this._communalAIManagerEntity.Dispose();
                    this._communalAIManagerEntity = null;
                }
            }
        }

        #region 资源加载
        private void OnAddressableAssetLoaded(AsyncOperationHandle<GameObject> handle) {
            this.gameObject = handle.Result;
            if (this.gameObject != null) {
                this.gameObject.transform.SetParent(TimelineManager.root, false);
                this.ProcessTimeline(this.gameObject);
            }
            else {
                Debug.LogError("资源不合理");
            }
        }

        HashSet<uint> actionsHashSet = new HashSet<uint>();
        private void ProcessTimeline(GameObject gameObject) {
            if (this.csvCutscene.type == 1) // 视屏
            {
                this.videoPlayer = gameObject.transform.Find("VideoPlayer").GetComponent<VideoPlayer>();
                if (this.videoPlayer != null) {
                    string path = AssetPath.GetVideoFullUrl(this.csvCutscene.path);
                    this.videoPlayer.errorReceived -= this.OnReceiveError;
                    this.videoPlayer.errorReceived += this.OnReceiveError;
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        path = Path.ChangeExtension(path, ".webm");
#endif
                    this.videoPlayer.url = path;
                    this.isReady = true;

                    if (this.csvCutscene.forEnter == 0) {
                        this.OnReady();
                    }
                }
            }
            else {
                Camera[] cameras = this.gameObject.transform.GetComponentsInChildren<Camera>(true);
                for (int i = 0, length = cameras.Length; i < length; ++i) {
                    var camera = cameras[i];
                    this.timeline3DCamera = camera;
                    if (!camera.gameObject.TryGetComponent<AutoCameraStack>(out AutoCameraStack _)) {
                        camera.gameObject.AddComponent<AutoCameraStack>();
                    }
                }

                this.timelineDirector = gameObject.transform.Find("timeline").GetComponent<TimelineDirector>();
                this.virtualCameraAssigner = this.timelineDirector.GetComponent<VirtualCameraAssigner>();
                this.animationTrackSetter = this.timelineDirector.GetComponent<AnimationTrackSetter>();

                this.timelineDirector.Collect();
                gameObject.SetActive(false);

                // 动态加载主角
                if (this.csvCutscene.isLoadMainPlayer) {
                    this.isCharacterLoaded = false;
                    this.isCharacterAnimationLoaded = false;

                    uint heroId = Sys_Role.Instance.Role.HeroId;
                    bool isHigh = !this.csvCutscene.isHighModel;
                    uint weaponId = isHigh ? Constants.UMARMEDID : Sys_Equip.Instance.GetCurWeapon();

                    this.heroLoader = HeroLoader.Create(isHigh);
                    this.heroLoader.LoadHero(heroId, weaponId, isHigh ? ELayerMask.Default : ELayerMask.CutSceneActor, Sys_Fashion.Instance.GetDressData(), this.OnCharacterAddressableAssetLoaded, isHigh);

                    this.clips = this.timelineDirector.GetBindingTrackClips<AnimationTrack>("mainPlayerAnimations");
                    if (this.animationTrackSetter != null) {
                        this.animationTrackSetter.clips = this.clips;

                        this.assetsGroupLoader.UnloadAll();
                        this.animationClips.Clear();

                        uint realHeroId = !this.csvCutscene.isHighModel ? Hero.GetHighModelID(heroId) : heroId;
                        List<string> animationPaths;
                        this.actionsHashSet.Clear();
                        if (this.csvCutscene.actions != null) {
                            for (int i = 0; i < this.csvCutscene.actions.Count; ++i) {
                                this.actionsHashSet.Add(this.csvCutscene.actions[i]);
                            }
                            AnimationComponent.GetAnimationPaths(realHeroId, weaponId, out animationPaths, this.actionsHashSet, this.csvCutscene.actions);
                        }
                        else {
                            AnimationComponent.GetAnimationPaths(realHeroId, weaponId, out animationPaths, null, null);
                        }

                        if (animationPaths != null) {
                            for (int index = 0, len = animationPaths.Count; index < len; index++) {
                                this.assetsGroupLoader.AddLoadTask(animationPaths[index]);
                            }
                        }
                        else {
                            DebugUtil.LogError($"animationPaths is null heroID: {heroId}");
                        }

                        this.assetsGroupLoader.StartLoad(null, this.OnAnimationLoaded, null);

                    }
                    else {
                        this.OnAnimationLoaded();
                    }
                }
                else {
                    this.isCharacterLoaded = true;
                    this.OnAnimationLoaded();

                    this.eventEmitter.Trigger<uint>(EEvents.OnLoaded, this.csvCutscene.id);
                }
            }

            if (this.csvCutscene.hideThings != null) {
                for (int i = 0, length = this.csvCutscene.hideThings.Count; i < length; ++i) {
                    string path = this.csvCutscene.hideThings[i];
                    GameObject go = GameObject.Find(path);
                    if (go != null) {
                        this.hideThings.Add(go);
                        go.SetActive(false);
                    }
                }
            }
        }

        private void OnReceiveError(VideoPlayer source, string errorMsg) {
            Debug.LogErrorFormat("暂不支持视频 播放 {0}", errorMsg);
            this.End(true);
        }
        private void OnAnimationLoaded() {
            if (this.animationTrackSetter != null) {
                this.animationClips.Clear();
                for (int i = 0, length = this.assetsGroupLoader.assetRequests.Count; i < length; ++i) {
                    AnimationClip clip = this.assetsGroupLoader.assetRequests[i].Result as AnimationClip;
                    this.animationClips.Add(clip);
                }
            }

            this.isCharacterAnimationLoaded = true;
            this.whenCharacterOrAnimationLoaded();
        }
        private void OnCharacterAddressableAssetLoaded(GameObject handle) {
            GameObject model = handle;
            model.transform.SetParent(this.gameObject.transform.Find("model/mainPlayer"), false);
            this.mainPlayerModel = model;

            this.ProcessModel(model);

            this.isCharacterLoaded = true;
            this.whenCharacterOrAnimationLoaded();
        }
        private void whenCharacterOrAnimationLoaded() {
            if (this.isCharacterLoaded && this.isCharacterAnimationLoaded) {
                if (this.animationTrackSetter != null) {
                    this.animationTrackSetter.Restore();

                    // 为了简单：不校验i的合理性
                    for (int i = 0, length = this.animationClips.Count; i < length; ++i) {
                        this.animationTrackSetter.SetClip(i, this.animationClips[i]);
                    }
                }

                this.isReady = true;
                if (this.csvCutscene.forEnter == 0) {
                    this.OnReady();
                }
            }
        }


        private CutSceneWeatherData data = new CutSceneWeatherData();
        public void OnReady() {
            if (this.gameObject != null) {
                this.gameObject.SetActive(true);
            }

            // 视屏
            if (this.csvCutscene.type == 1) {
                this.videoPlayer.Play();
            }
            else {
                this.timelineDirector.RebuildGraphAndPlay();

                // 通知天气
                this.data.isStart = true;
                this.data.weatherId = (uint)this.csvCutscene.weather;
                this.data.dayOrNightId = this.csvCutscene.dayNight;
                Sys_Weather.Instance.eventEmitter.Trigger<CutSceneWeatherData>(Sys_Weather.EEvents.OnCutSceneWeather, this.data);
            }

            UIManager.OpenUI(EUIID.UI_CutSceneTop, false, new Tuple<CSVCutScene.Data, Action>(this.csvCutscene, () => {
                this.End(true);
            }));
        }
        private void ProcessModel(GameObject model) {
            if (this.timelineDirector != null) {
                this.eventEmitter.Trigger<uint>(EEvents.OnLoaded, this.csvCutscene.id);

                Animator modelAnimator = model?.GetComponent<Animator>();
                if (modelAnimator == null) {
                    modelAnimator = model.AddComponent<Animator>();
                }

                Transform mesh = model.transform.GetChild(0);
                CP_BehaviourCollector collector = model.GetComponent<CP_BehaviourCollector>();
                if (collector != null) {
                    collector.Enable(false);
                }

                this.virtualCameraAssigner?.Bind(mesh);

                Animator meshAnimator = mesh?.GetComponent<Animator>();
                if (meshAnimator != null) {
                    this.timelineDirector.SetBinding("mainPlayer_" + Sys_Role.Instance.Role.HeroId.ToString(), meshAnimator);
                    this.timelineDirector.SetBinding("mainPlayer_move", meshAnimator);
                    this.timelineDirector.SetBinding("mainPlayer_mesh", mesh);
                    this.timelineDirector.SetBinding("mainPlayerAnimations", meshAnimator);
                    this.timelineDirector.SetBinding("mainPlayerUnDollyCart", modelAnimator);
                }
                else {
                    DebugUtil.Log(ELogType.eCutScene, "model dont have animator");
                }
            }
        }

        #endregion
    }
}