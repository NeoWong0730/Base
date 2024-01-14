using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;

namespace Logic {
    /// CutScene系统
    public partial class Sys_CutScene : SystemModuleBase<Sys_CutScene>, ISystemModuleUpdate {
        // 组队模式下，是否队长此时正在cutscene播放
        public bool isCaptainPlaying { get; private set; } = false;
        public bool isCutSceneTeleport = false;

        public override void Init() {
            TimelineLifeCircle.eventEmitter.Handle<CutSceneArg>(ETimelineLifeCircle.OnBehaviourPlay, this.Start, true);
            TimelineLifeCircle.eventEmitter.Handle<CutSceneArg>(ETimelineLifeCircle.OnBehaviourPause, this.End, true);

            TimelineLifeCircle.eventEmitter.Handle(ETimelineLifeCircle.OnGraphStart, this.Start, true);
            TimelineLifeCircle.eventEmitter.Handle(ETimelineLifeCircle.OnGraphStop, this.End, true);

            VideoPlayerLifeCircleBehaviour.eventEmitter.Handle(EVideoPlayerCircle.OnGraphStart, this.Start, true);
            VideoPlayerLifeCircleBehaviour.eventEmitter.Handle(EVideoPlayerCircle.OnGraphStop, this.End, true);

            // cutscene同步协议
            EventDispatcher.Instance.AddEventListener((ushort)CmdTeam.SyncCutScene, (ushort)CmdTeam.SyncCutScene, this.OnEnterCutscene, CmdTeamSyncCutScene.Parser);
        }

        public override void OnSyncFinished() {
            this.toReadyPlayCutscenes.Clear();
        }

        public void OnUpdate() {
            if (this.toReadyPlayCutscenes.Count > 0) {
                if (this.CanPlay()) {
                    this.OnServerTrigger(this.toReadyPlayCutscenes[0]);
                    this.toReadyPlayCutscenes.Clear();
                }
            }
        }
        public override void OnLogout() {
            this.End(true, true);
        }

        #region 回包处理
        private void OnServerTrigger(uint cutsceneId) {
            this.triggerType = ETimelineTriggerType.Server;

            this.seriesCutSceneId = cutsceneId;
            // 如果队员之前刚好在播放cutscene,那么忽略来自队长的同步cutscene
            this.OnTrigger(cutsceneId);
        }

        private void OnTrigger(uint cutsceneId) {
            if (!this.isPlaying) {
                this.isPlaying = true;
                this.Play(cutsceneId);
            }
        }

        private void OnEnterCutscene(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eCutScene, "OnEnterCutscene");

            // 队长不应该接收到这个消息
            // 没有离队的队员接收
            CmdTeamSyncCutScene response = NetMsgUtil.Deserialize<CmdTeamSyncCutScene>(CmdTeamSyncCutScene.Parser, msg);
            this.isCutSceneTeleport = response.Teleport;

            bool can = this.CanPlay();
            if (can) {
                this.OnServerTrigger(response.CutSceneId);
            }
            else {
                this.toReadyPlayCutscenes.Add(response.CutSceneId);
            }
        }
        #endregion

        private List<uint> toReadyPlayCutscenes = new List<uint>();
        private bool CanPlay() {

            if (this.isPlaying)
                return false;

            if (Sys_FunctionOpen.Instance.isRunning)
                return false;

            if (Sys_Fight.Instance.IsFight())
                return false;

            return true;
        }

        #region 请求
        public void SkipCutscene(uint cutsceneId, bool skip) {
            DebugUtil.LogFormat(ELogType.eCutScene, "CutScene: SkipCutscene:{0}", cutsceneId);

            if (cutsceneId == CreateRole_CUtsceneId) {
                return;
            }

            CmdTeamSyncCutSceneEnd req = new CmdTeamSyncCutSceneEnd();
            req.CutSceneId = cutsceneId;
            req.Skip = skip;
            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncCutSceneEnd, req);
        }
        #endregion
    }
}