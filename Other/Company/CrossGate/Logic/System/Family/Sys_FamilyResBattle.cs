using System;
using System.Collections.Generic;
using pbc = global::Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using static Logic.UI_Family;

namespace Logic {
    // 家族资源争夺战
    public partial class Sys_FamilyResBattle : SystemModuleBase<Sys_FamilyResBattle> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init() {
            EventDispatcher.Instance.AddEventListener((ushort) CmdGuildBattle.CheckApplyCondReq, (ushort) CmdGuildBattle.CheckApplyCondRes, this.OnRecvCheckApplyCondRes,
                CmdGuildBattleCheckApplyCondRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdGuildBattle.ApplyReq, (ushort) CmdGuildBattle.ApplyNty, this.OnRecvApply, CmdGuildBattleApplyNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.StateNty, this.OnRecvStateNty, CmdGuildBattleStateNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdGuildBattle.RecoveryReq, (ushort) CmdGuildBattle.RecoveryNty, this.OnRecvRecovery, CmdGuildBattleRecoveryNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.PunishNty, this.OnRecvPunishNty, CmdGuildBattlePunishNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdGuildBattle.HandInResourceReq, (ushort) CmdGuildBattle.HandInResourceRes, this.OnRecvHandInResourceRes,
                CmdGuildBattleHandInResourceRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.ResultNty, this.OnRecvResultNty, CmdGuildBattleResultNty.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.DataNty, this.OnRecvDataNty, CmdGuildBattleDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.AddRoleDataNty, this.OnRecvAddRoleDataNty, CmdGuildBattleAddRoleDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.UpdateRoleDataNty, this.OnRecvUpdateRoleDataNty, CmdGuildBattleUpdateRoleDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.UpdateGuildDataNty, this.OnRecvUpdateGuildDataNty, CmdGuildBattleUpdateGuildDataNty.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuild.UpdateGuildBattleInfoNty, this.OnRecvUpdateGuildBattleInfoNty, CmdGuildUpdateGuildBattleInfoNty.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.GuildBattleResourceNtf, this.OnRecvResourceNtf, CmdMapGuildBattleResourceNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.GuildBattleMapTeamDataNtf, this.OnRecvMapTeamDataNtf, CmdMapGuildBattleMapTeamDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.DisplayHintNty, this.OnRecvDisplayHintNty, CmdGuildBattleDisplayHintNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.UpdateResourceDataNty, this.OnRecvUpdateResourceDataNty, CmdGuildBattleUpdateResourceDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuild.UpdateGuildBattleScoreNty, this.OnRecvUpdateGuildBattleScoreNty, CmdGuildUpdateGuildBattleScoreNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.DisplayApplyHintNty, this.OnRecvDisplayApplyHintNty, CmdGuildBattleDisplayApplyHintNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.ApplyHintConfigNty, this.OnApplyHintConfigNty, CmdGuildBattleApplyHintConfigNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.UpdateCoreTeamNty, this.OnUpdateCoreTeamNty, CmdGuildBattleUpdateCoreTeamNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdGuildBattle.CoreTeamDataNty, this.OnCoreTeamDataNty, CmdGuildBattleCoreTeamDataNty.Parser);

            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, this.OnReconnectStart, true);
            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStartReq, this.OnReconnectStart, true);

            Sys_Map.Instance.eventEmitter.Handle<uint, uint>(Sys_Map.EEvents.OnChangeMap, this.OnChangeMap, true);
            Sys_Family.Instance.eventEmitter.Handle<CmdGuildSceneInfoNtf>(Sys_Family.EEvents.OnSceneInfoNtf, this.OnSceneInfoNtf, true);
            Sys_Map.Instance.eventEmitter.Handle<ulong>(Sys_Map.EEvents.OnRemoveRole, this.OnRemoveRole, true);

            Framework.CollisionWall.onAction = this.OnWallAction;
        }

        public override void OnSyncFinished() {
            var csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(1);
            if (csvFamilyResBattleParameter != null) {
                if (!int.TryParse(csvFamilyResBattleParameter.Parameter, out this.SignupLimitedFamilyLevel)) {
                    this.SignupLimitedFamilyLevel = 2;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(2);
            if (csvFamilyResBattleParameter != null) {
                if (!int.TryParse(csvFamilyResBattleParameter.Parameter, out this.SignupLimitedFamilyMemberCount)) {
                    this.SignupLimitedFamilyMemberCount = 10;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(3);
            if (csvFamilyResBattleParameter != null) {
                string[] args = csvFamilyResBattleParameter.Parameter.Split('|');
                if (!int.TryParse(args[0], out this.SignupLimitedFamilyActiveMemberLevel)) {
                    this.SignupLimitedFamilyActiveMemberLevel = 40;
                }

                if (!int.TryParse(args[1], out this.SignupLimitedFamilyActiveMemberCount)) {
                    this.SignupLimitedFamilyActiveMemberCount = 5;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(7);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.EnterNpcId);
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(8);
            if (csvFamilyResBattleParameter != null) {
                if (!int.TryParse(csvFamilyResBattleParameter.Parameter, out this.EnterLimitedPlayerLevel)) {
                    this.EnterLimitedPlayerLevel = 10;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(9);
            if (csvFamilyResBattleParameter != null) {
                if (!int.TryParse(csvFamilyResBattleParameter.Parameter, out this.EnterLimitedPlayerJoinFamilyDays)) {
                    this.EnterLimitedPlayerJoinFamilyDays = 2;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(10);
            if (csvFamilyResBattleParameter != null) {
                if (!uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.maxAttackDistance)) {
                    this.maxAttackDistance = 10;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(12);
            if (csvFamilyResBattleParameter != null) {
                if (!uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlewinMaxRes)) {
                    this.battlewinMaxRes = 10000;
                }
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(13);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlewinFamilyDropId);
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(14);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlefailFamilyDropId);
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(15);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlepingFamilyDropId);
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(26);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlewinPersonalDropId);
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(27);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlefailPersonalDropId);
            }

            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(28);
            if (csvFamilyResBattleParameter != null) {
                uint.TryParse(csvFamilyResBattleParameter.Parameter, out this.battlepingPersonalDropId);
            }

            this.preWins.Clear();
            csvFamilyResBattleParameter = CSVFamilyResBattleParameter.Instance.GetConfData(36);
            if (csvFamilyResBattleParameter != null) {
                string[] segs = csvFamilyResBattleParameter.Parameter?.Split('|');
                if (segs != null) {
                    for (int i = 0, length = segs.Length; i < length; ++i) {
                        if (uint.TryParse(csvFamilyResBattleParameter.Parameter, out uint ttt)) {
                            this.preWins.Add(ttt);
                        }
                    }
                }
            }

            #region 测试

            //this.InFamilyBattle = true;
            //this.stage = EStage.ReadyBattle;
            //this.hasSigned = false;
            //this.hasBlue = true;
            //this.blueFamlilyName = "浓痰搅屎会";
            //this.blueServerName = "HHH";

            #endregion

            // 重连成功之后，还原标志位
            this.fromReconnect = false;
        }

        private bool fromReconnect = false;

        private void OnReconnectStart() {
            this.fromReconnect = true;
        }

        public override void OnLogout() {
            this.fromReconnect = false;
            this.InFamilyBattle = false;
            this.isBattleFieldEnd = true;
            this.Resource = 0;
            this.hasEnterOpenCount = 0;
            this.timerTryEnter?.Cancel();
        }

        #region 网络交互

        // 报名提示
        public Action PendingAction;

        // 阶段变化
        private void OnRecvStateNty(NetMsg msg) {
            CmdGuildBattleStateNty res = NetMsgUtil.Deserialize<CmdGuildBattleStateNty>(CmdGuildBattleStateNty.Parser, msg);
            EStage oldStage = this.stage;
            this.stage = (EStage) res.State;
            this.endTimeOfStage = res.Expiretime;

            this.eventEmitter.Trigger<uint, uint, long>(EEvents.OnStageChanged, (uint) oldStage, res.State, this.endTimeOfStage);
            this.TrySetWall();

            // 添加定时器的原因主要是因为stage数据和data数据是两个包来的，只能互相等一等
            // 可以转移到onsync中，但是和断线重连又有关系，就用timer吧
            this.timerTryEnter?.Cancel();
            this.timerTryEnter = Timer.RegisterOrReuse(ref this.timerTryEnter, 0.1f, this.TryOpenEnterMsgBox);

            if (Sys_Role.Instance.hasSyncFinished) {
                // 增量阶段
                if (oldStage != EStage.Battle && this.stage == EStage.Battle) {
                    if (this.InFamilyBattle) {
                        if (this.hasSigned) {
                            UIManager.OpenUI(EUIID.UI_FamilyResBattleBeginTip);
                        }
                    }
                    else {
                        // 
                    }
                }
            }
            else {
                // 登陆/重连 阶段
                //if (this.stage == EStage.Signup && !Sys_Role.Instance.hasReconnectSync && !this.hasSigned) {
                //    //string key = string.Format("{0}FamilyResBattleSignupTime", Sys_Role.Instance.RoleId);
                //    //float lastTime = PlayerPrefs.GetFloat(key, 0f);
                //    //float curTime = Sys_Time.Instance.GetServerTime();
                //    //if (lastTime == 0f || (curTime > lastTime + 86400 * 2)) 
                //    {
                //        //PlayerPrefs.SetFloat(key, curTime);
                //        this.PendingAction = this.TryOpenSignupMsgBox;
                //    }
                //}
            }
        }

        // 报名条件
        public void ReqCanSignup() {
            CmdGuildBattleCheckApplyCondReq req = new CmdGuildBattleCheckApplyCondReq();
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.CheckApplyCondReq, req);
        }

        private void OnRecvCheckApplyCondRes(NetMsg msg) {
            CmdGuildBattleCheckApplyCondRes res = NetMsgUtil.Deserialize<CmdGuildBattleCheckApplyCondRes>(CmdGuildBattleCheckApplyCondRes.Parser, msg);
            this.eventEmitter.Trigger<CmdGuildBattleCheckApplyCondRes>(EEvents.OnSignupConditionChanged, res);
        }

        // 报名
        public void ReqApply() {
            CmdGuildBattleApplyReq req = new CmdGuildBattleApplyReq();
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.ApplyReq, req);
        }

        private void OnRecvApply(NetMsg msg) {
            CmdGuildBattleApplyNty res = NetMsgUtil.Deserialize<CmdGuildBattleApplyNty>(CmdGuildBattleApplyNty.Parser, msg);
            bool old = this.hasSigned;
            this.noTurn = res.ApplyState == 2;
            this.hasSigned = (res.ApplyState == 1) || (res.ApplyState == 2);

            this.hasBlue = res.Oppo != null;
            if (this.hasBlue) {
                // 有匹配对手
                this.blueFamlilyId = res.Oppo.GuildId;
                this.blueFamlilyName = res.Oppo.GuildName.ToStringUtf8();

                this.blueServerId = res.Oppo.GameId;
                this.blueServerName = res.Oppo.GameName.ToStringUtf8();
            }
            else {
                // 无匹配对手
                this.blueFamlilyId = default;
                this.blueFamlilyName = default;

                this.blueServerId = default;
                this.blueServerName = default;
            }

            this.eventEmitter.Trigger<bool, bool>(EEvents.OnSignupChanged, old, this.hasSigned);
        }

        // 进入战场
        public void ReqEnterBattleField() {
            if (this.InFamilyBattle) {
                return;
            }

            //if (this.stage == EStage.ReadyBattle || this.stage == EStage.Battle) {
            //    CmdGuildBattleEnterReq req = new CmdGuildBattleEnterReq();
            //    NetClient.Instance.SendMessage((ushort)CmdGuildBattle.EnterReq, req);
            //}

            CmdGuildBattleEnterReq req = new CmdGuildBattleEnterReq();
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.EnterReq, req);
            this.ClearBattleFieldData();
        }

        public void ReqLeave() {
            Sys_Map.Instance.ReqLeave();
        }

        #region 攻击敌人

        // 点击地面
        private void OnClickOrTouch(bool down) {
            if (isFollowing && this.attackRoleId != 0 && beginFollowFrame != Time.frameCount) {
                this.StopFollow();
            }
        }

        // 控制摇杆
        private void OnLeftJoystick(Vector2 v2, float f) {
            if (isFollowing && this.attackRoleId != 0 && beginFollowFrame != Time.frameCount) {
                this.StopFollow();
            }
        }

        private void OnRightJoystick(Vector2 v2, float f) {
            if (isFollowing && this.attackRoleId != 0 && beginFollowFrame != Time.frameCount) {
                this.StopFollow();
            }
        }

        private void OnRemoveRole(ulong roleId) {
            if (isFollowing && this.attackRoleId != 0 && roleId == this.attackRoleId) {
                this.StopFollow();
            }
        }

        // 战斗
        private ulong attackRoleId = 0;
        private bool isFollowing = false;
        private int beginFollowFrame;

        private void StopFollow() {
            GameCenter.mainHero?.movementComponent?.Stop(false);
            isFollowing = false;
            this.attackRoleId = 0;
            beginFollowFrame = default;

            Sys_Input.Instance.onClickOrTouch -= this.OnClickOrTouch;
            Sys_Input.Instance.onLeftJoystick -= this.OnLeftJoystick;
            Sys_Input.Instance.onRightJoystick -= this.OnRightJoystick;
        }

        public void ReqAttack(Hero hero, ulong roleId, uint mapId) {
            if (this.stage != EStage.Battle) {
                return;
            }

            // 己方在战斗中
            if (hero.heroBaseComponent.bInFight) {
                return;
            }

            // 非队长不能点击别人进行战斗
            if (!Sys_Team.Instance.canManualOperate) {
                return;
            }

            // 敌方在战斗中
            if (this.rolesInBattle.Contains(roleId)) {
                return;
            }

            // 对方在安全区
            if (this.IsInSafeArea(roleId)) {
                return;
            }

            /*
            // 己方成员需要大于地方成员个数
            uint selfMemberCount = Sys_Team.Instance.HaveTeam ? (uint)Sys_Team.Instance.TeamMemsCount : 1u;
            // 地方是一个人呢？
            this.teamMemberCounts.TryGetValue(roleId, out uint enemyMemCount);
            enemyMemCount = Math.Min(enemyMemCount, 1);

            if (selfMemberCount < enemyMemCount) {
                return;
            }
            */

            bool IsDistanceValid(Hero target, out Vector2 bluePos) {
                var redPosition = GameCenter.mainHero?.transform?.position ?? Vector3.zero;
                redPosition.y = 0f;
                var bluePosition = target.transform.position;
                bluePosition.y = 0f;
                var distance = Vector3.Distance(redPosition, bluePosition);

                bluePos = new Vector2(bluePosition.x, bluePosition.z);

                return distance > this.maxAttackDistance;
            }

            if (IsDistanceValid(hero, out Vector2 targetPos)) {
                var blueMovement = hero.GetMovementComponent();
                var redMovement = GameCenter.mainHero?.GetMovementComponent();
                if (blueMovement != null && redMovement != null) {
                    bool CanFollow() {
                        // 对方在安全区
                        if (this.IsInSafeArea(roleId)) {
                            return false;
                        }

                        return true;
                    }

                    void OnFollow() {
                        this.ReqAttack(roleId);
                        StopFollow();
                    }

                    this.isFollowing = true;
                    this.beginFollowFrame = Time.frameCount;
                    this.attackRoleId = roleId;
                    redMovement.FollowToMove(redMovement.fMoveSpeed, maxAttackDistance - 0.1f, blueMovement.transform, CanFollow, OnFollow, StopFollow);

                    Sys_Input.Instance.onClickOrTouch += this.OnClickOrTouch;
                    Sys_Input.Instance.onLeftJoystick += this.OnLeftJoystick;
                    Sys_Input.Instance.onRightJoystick += this.OnRightJoystick;
                }
            }
            else {
                this.ReqAttack(roleId);
            }
        }

        private void ReqAttack(ulong roleId) {
            if (!this.InFamilyBattle || isBattleFieldEnd) {
                return;
            }

            if (!GameCenter.otherActorsDic.TryGetValue(roleId, out Hero hero)) {
                return;
            }

            if (hero.familyResBattleComponent.InProtecting) {
                var remain = hero.familyResBattleComponent.remain;
                string str = LanguageHelper.GetTextContent(3230000104, remain.ToString());
                Sys_Hint.Instance.PushContent_Normal(str);
                return;
            }

            CmdGuildBattleAttackReq req = new CmdGuildBattleAttackReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.AttackReq, req);
        }

        #endregion

        private long rebornExpiretime;
        private Timer timer;

        public bool InReborn {
            get { return Sys_Time.Instance.GetServerTime() <= this.rebornExpiretime; }
        }

        // 打输重生
        private void OnRecvPunishNty(NetMsg msg) {
            CmdGuildBattlePunishNty res = NetMsgUtil.Deserialize<CmdGuildBattlePunishNty>(CmdGuildBattlePunishNty.Parser, msg);
            this.rebornExpiretime = res.Expiretime;
            UIManager.OpenUI(EUIID.UI_FamilyResBattleReborn, false, res.Expiretime);

            Sys_Input.Instance.bEnableJoystick = false;
            Sys_Input.Instance.bForbidControl = true;
            Sys_Input.Instance.bForbidTouch = true;
            float diff = res.Expiretime - (float) Sys_Time.Instance.GetServerTime();
            //Debug.LogError("--diff :" + diff.ToString());
            this.timer?.Cancel();
            this.timer = Timer.RegisterOrReuse(ref this.timer, diff, () => {
                //this.TrySetWall(true);
                Sys_Input.Instance.bEnableJoystick = true;
                Sys_Input.Instance.bForbidControl = false;
                Sys_Input.Instance.bForbidTouch = false;
            });
            //this.TrySetWall(false);

            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
        }

        // 小地图
        public void FocusMiniMapReq(bool toFocus) {
            CmdGuildBattleFocusMiniMapReq req = new CmdGuildBattleFocusMiniMapReq();
            req.Open = toFocus;
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.FocusMiniMapReq, req);
        }

        // 疗伤
        //public void ReqRecovery(uint npcUid) {
        //    CmdGuildBattleRecoveryReq req = new CmdGuildBattleRecoveryReq();
        //    req.NpcUid = npcUid;
        //    NetClient.Instance.SendMessage((ushort)CmdGuildBattle.RecoveryReq, req);
        //}
        private void OnRecvRecovery(NetMsg msg) {
            CmdGuildBattleRecoveryNty res = NetMsgUtil.Deserialize<CmdGuildBattleRecoveryNty>(CmdGuildBattleRecoveryNty.Parser, msg);
            if (res != null) {
                EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, CSVEffect.Instance.GetConfData(3000104138).effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Recover);
            }
        }

        // 提交资源
        public void ReqSubmitRes(uint npcUid) {
            CmdGuildBattleHandInResourceReq req = new CmdGuildBattleHandInResourceReq();
            req.NpcUid = npcUid;
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.HandInResourceReq, req);
        }

        private void OnRecvHandInResourceRes(NetMsg msg) {
            CmdGuildBattleHandInResourceRes res = NetMsgUtil.Deserialize<CmdGuildBattleHandInResourceRes>(CmdGuildBattleHandInResourceRes.Parser, msg);
            UIManager.OpenUI(EUIID.UI_FamilyResBattleSubmitResult, false, new Tuple<long, long>(res.Former, res.Current));

            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
        }

        public Action pendingForOpenResult;

        // 战场是否结束
        public bool isBattleFieldEnd { get; private set; }

        private void OnRecvResultNty(NetMsg msg) {
            CmdGuildBattleResultNty res = NetMsgUtil.Deserialize<CmdGuildBattleResultNty>(CmdGuildBattleResultNty.Parser, msg);

            void OpenResult() {
                UIManager.OpenUI(EUIID.UI_FamilyResBattleResult, false, res.Result);
            }

            this.isBattleFieldEnd = true;
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
            // GameCenter.onSelfHeroCreated -= this._TryGuideNpc;

            ActionCtrl.Instance.Reset();
            this.timer?.Cancel();
            Sys_Input.Instance.bEnableJoystick = true;
            Sys_Input.Instance.bForbidControl = false;
            Sys_Input.Instance.bForbidTouch = false;

            this.timerTryEnter?.Cancel();

            bool isInFight = Sys_Fight.Instance.IsFight();
            if (isInFight) {
                this.pendingForOpenResult = OpenResult;
            }
            else {
                this.pendingForOpenResult = null;
                OpenResult();
            }
        }

        public void OnRecvDataNty(NetMsg msg) {
            DebugUtil.Log(ELogType.eFamilyBattle, "OnRecvDataNty");
            CmdGuildBattleDataNty res = NetMsgUtil.Deserialize<CmdGuildBattleDataNty>(CmdGuildBattleDataNty.Parser, msg);
            this.ClearBattleFieldData();

            for (int i = 0, length = res.Resources.Count; i < length; ++i) {
                var r = res.Resources[i];
                if (!this.allRes.TryGetValue(r.Type, out var _)) {
                    var t = new Res() {
                        resId = r.Type,
                        max = r.MaxCount,
                        leftCount = r.LeftCount,
                        nextRefreshTime = r.FreshTime
                    };
                    this.allRes.Add(r.Type, t);
                }
            }

            bool hasFind = false;
            for (int i = 0, length = res.Guilds.Count; i < length; ++i) {
                var info = res.Guilds[i];
                this.guilds.Add(info.GuildId, info);

                if (hasFind) {
                    break;
                }

                for (int j = 0; j < info.Members.Count; j++) {
                    var role = info.Members[j];
                    if (role.RoleId == Sys_Role.Instance.RoleId) {
                        this.redCampId = info.Camp;
                        this.redFamilyId = info.GuildId;
                        hasFind = true;
                        break;
                    }
                }
            }

            for (int i = 0, length = res.Guilds.Count; i < length; ++i) {
                var info = res.Guilds[i];

                if (info.GuildId != this.redFamilyId) {
                    this.blueCampId = info.Camp;
                    this.blueFamilyId = info.GuildId;
                }
            }

            // guilds长度为2，才可以这样子
            // 确保此时查找 familyid的时候， 有相关family数据
            var ls = this.guilds[this.redFamilyId].Members;
            for (int i = 0, length = ls.Count; i < length; ++i) {
                var role = ls[i];
                if (role != null && !this.redRoles.TryGetValue(role.RoleId, out var _)) {
                    this.redRoles.Add(role.RoleId, role);
                    this.allRols.Add(role);
                }
            }

            ls = this.guilds[this.blueFamilyId].Members;
            for (int i = 0, length = ls.Count; i < length; ++i) {
                var role = ls[i];
                if (role != null && !this.blueRoles.TryGetValue(role.RoleId, out var _)) {
                    this.blueRoles.Add(role.RoleId, role);
                    this.allRols.Add(role);
                }
            }

            this.TryPreWin();
            this.TrySetWall();

            this.InFamilyBattle = true;
            this.isBattleFieldEnd = false;
            this.eventEmitter.Trigger(EEvents.OnResChanged);
            this.eventEmitter.Trigger(EEvents.OnEnter);
        }

        private void TryPreWin() {
            if (Sys_Role.Instance.hasSyncFinished) {
                bool hasFind = false;
                BattleGuildMapData kv = null;
                foreach (var kvp in this.guilds) {
                    if (hasFind) {
                        break;
                    }

                    long total = kvp.Value.Resource;
                    int i = this.preWins.Count - 1;
                    for (; i >= 0; i--) {
                        if (this.preWins[i] <= total) {
                            kv = kvp.Value;
                            hasFind = true;
                            break;
                        }
                    }
                }

                if (hasFind) {
                    UIManager.OpenUI(EUIID.UI_FamilyResBattlePreWin, false, new Tuple<ulong, uint>(kv.GuildId, kv.Resource));
                }
            }
        }

        // 玩家个数逐渐增加 增量数据
        private void OnRecvAddRoleDataNty(NetMsg msg) {
            CmdGuildBattleAddRoleDataNty res = NetMsgUtil.Deserialize<CmdGuildBattleAddRoleDataNty>(CmdGuildBattleAddRoleDataNty.Parser, msg);
            bool isRed = res.GuildId == this.redFamilyId;
            if (isRed) {
                if (!this.redRoles.TryGetValue(res.Role.RoleId, out var _)) {
                    this.redRoles.Add(res.Role.RoleId, res.Role);
                    this.allRols.Add(res.Role);

                    this.eventEmitter.Trigger(EEvents.OnResChanged);
                }
            }
            else {
                if (!this.blueRoles.TryGetValue(res.Role.RoleId, out var _)) {
                    this.blueRoles.Add(res.Role.RoleId, res.Role);
                    this.allRols.Add(res.Role);

                    this.eventEmitter.Trigger(EEvents.OnResChanged);
                }
            }
        }

        // 玩家信息变化 增量数据
        public void OnRecvUpdateRoleDataNty(NetMsg msg) {
            CmdGuildBattleUpdateRoleDataNty res = NetMsgUtil.Deserialize<CmdGuildBattleUpdateRoleDataNty>(CmdGuildBattleUpdateRoleDataNty.Parser, msg);
            uint selfOldRes = 0;
            uint selfNewRes = 0;
            for (int i = 0, length = res.Role.Count; i < length; ++i) {
                var info = res.Role[i];

                if (this.redRoles.TryGetValue(info.RoleId, out var role)) {
                    if (role.RoleId == Sys_Role.Instance.RoleId) {
                        selfOldRes = role.Resource;
                        selfNewRes = info.Resource;
                    }

                    role.Resource = info.Resource;
                    role.Score = info.Score;

                    this.eventEmitter.Trigger(EEvents.OnResChanged);
                }
                else if (this.blueRoles.TryGetValue(info.RoleId, out role)) {
                    if (role.RoleId == Sys_Role.Instance.RoleId) {
                        selfOldRes = role.Resource;
                        selfNewRes = info.Resource;
                    }

                    role.Resource = info.Resource;
                    role.Score = info.Score;

                    this.eventEmitter.Trigger(EEvents.OnResChanged);
                }
            }

            if (selfOldRes != selfNewRes && Sys_Team.Instance.canManualOperate) {
                this.eventEmitter.Trigger(EEvents.OnMyResChanged);
            }
        }

        private void OnRecvUpdateGuildDataNty(NetMsg msg) {
            CmdGuildBattleUpdateGuildDataNty res = NetMsgUtil.Deserialize<CmdGuildBattleUpdateGuildDataNty>(CmdGuildBattleUpdateGuildDataNty.Parser, msg);
            if (this.guilds.TryGetValue(res.GuildId, out var info)) {
                info.Resource = res.Resource;

                this.eventEmitter.Trigger(EEvents.OnResChanged);
            }
        }

        private void OnRecvUpdateGuildBattleInfoNty(NetMsg msg) {
            CmdGuildUpdateGuildBattleInfoNty res = NetMsgUtil.Deserialize<CmdGuildUpdateGuildBattleInfoNty>(CmdGuildUpdateGuildBattleInfoNty.Parser, msg);
            this._ProcessActivity(res.Info);
        }

        public uint Resource;

        // 第一次进入族战地图的时候，下发，包中只有主角的数据，其他是通过maprole下发
        // 视野内的数据变更的时候，也下发,包括主角和其他视野内的人
        private void OnRecvResourceNtf(NetMsg msg) {
            CmdMapGuildBattleResourceNtf res = NetMsgUtil.Deserialize<CmdMapGuildBattleResourceNtf>(CmdMapGuildBattleResourceNtf.Parser, msg);
            DebugUtil.Log(ELogType.eFamilyBattle, "OnRecvResourceNtf   " + "resourceId:  " + res.Resource);
            ulong roleId = res.RoleId;
            uint selfOldRes = 0;
            uint selfNewRes = 0;

            if (res.RoleId == Sys_Role.Instance.RoleId) {
                this.Resource = res.Resource;
                if (GameCenter.mainHero != null && GameCenter.mainHero.familyResBattleComponent != null) {
                    var cp = GameCenter.mainHero.familyResBattleComponent;
                    selfOldRes = cp.resource;
                    selfNewRes = res.Resource;
                    cp.resource = selfNewRes;

                    cp.AssignRes(selfNewRes);
                }
            }
            else {
                if (GameCenter.otherActorsDic.TryGetValue(roleId, out Hero hero)) {
                    var cp = hero.familyResBattleComponent;
                    if (cp != null) {
                        cp.resource = res.Resource;
                    }
                }
            }

            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateFamilyBattle, res.RoleId);
            Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateGuildBattleResource, res.RoleId, res.Resource);
            if (Sys_Team.Instance.canManualOperate && selfOldRes != selfNewRes) {
                if (selfNewRes != 0) {
                    if (GameCenter.mainHero != null) {
                        this._TryGuideNpc();
                    }

                    // else {
                    //     // 防止在hero没有创建的时候，协议数据已经下发
                    //     GameCenter.onSelfHeroCreated += this._TryGuideNpc;
                    // }
                }
                else if (selfOldRes != 0 && selfNewRes == 0) {
                    this._TryCancelGuideNpc();
                }

                this.eventEmitter.Trigger(EEvents.OnMyResChanged);
            }
        }

        public void _TryGuideNpc() {
            uint npcId = 0;
            var csv = CSVFamilyResBattleCamp.Instance.GetConfData(this.redCampId);
            if (csv != null) {
                npcId = csv.submitNpcId;
            }

            Sys_Map.Instance.eventEmitter.Trigger<uint>(Sys_Map.EEvents.OnGenWayPoints, npcId);
        }

        private void _TryCancelGuideNpc() {
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
        }

        private void OnRecvMapTeamDataNtf(NetMsg netMsg) {
            CmdMapGuildBattleMapTeamDataNtf ntf = NetMsgUtil.Deserialize<CmdMapGuildBattleMapTeamDataNtf>(CmdMapGuildBattleMapTeamDataNtf.Parser, netMsg);

            bool ishero = false;
            if (ntf.RoleId == Sys_Role.Instance.RoleId) {
                ishero = true;
                this.TeamNum = ntf.Leader == null ? 0 : ntf.Leader.MemberCount;
                this.MaxCount = ntf.Leader == null ? 0 : ntf.Leader.MaxCount;
            }

            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateFamilyBattle, ntf.RoleId);

            UpdateFamilyTeamNumInBattleResource updateFamilyTeamNumInBattleResource = new UpdateFamilyTeamNumInBattleResource();
            updateFamilyTeamNumInBattleResource.actorId = ntf.RoleId;
            updateFamilyTeamNumInBattleResource.teamNum = ntf.Leader == null ? 0 : ntf.Leader.MemberCount;
            updateFamilyTeamNumInBattleResource.maxCount = ntf.Leader == null ? 0 : ntf.Leader.MaxCount;
            Sys_HUD.Instance.eventEmitter.Trigger<UpdateFamilyTeamNumInBattleResource>(Sys_HUD.EEvents.OnUpdateFamilyTeamNum, updateFamilyTeamNumInBattleResource);

            DebugUtil.Log(ELogType.eFamilyBattle, "    OnRecvMapTeamDataNtf  :" + "isHero:  " + ishero + "   roleId:  " + ntf.RoleId + "TeamNum: " + updateFamilyTeamNumInBattleResource.teamNum +
                                                  "MaxCount: " + updateFamilyTeamNumInBattleResource.maxCount);

            uint teamNum = ntf.Leader == null ? 0 : ntf.Leader.MemberCount;
            if (!this.teamMemberCounts.TryGetValue(ntf.RoleId, out var _)) {
                this.teamMemberCounts.Add(ntf.RoleId, teamNum + 1);
            }
            else {
                this.teamMemberCounts[ntf.RoleId] = teamNum + 1;
            }
        }

        public Action PendingActionForRule;

        private void OnRecvDisplayHintNty(NetMsg netMsg) {
            CmdGuildBattleDisplayHintNty ntf = NetMsgUtil.Deserialize<CmdGuildBattleDisplayHintNty>(CmdGuildBattleDisplayHintNty.Parser, netMsg);
            this.PendingActionForRule = () => { Sys_CommonCourse.Instance.OpenCommonCourse(3, 301, 30103); };
        }

        private void OnRecvUpdateResourceDataNty(NetMsg netMsg) {
            CmdGuildBattleUpdateResourceDataNty ntf = NetMsgUtil.Deserialize<CmdGuildBattleUpdateResourceDataNty>(CmdGuildBattleUpdateResourceDataNty.Parser, netMsg);
            for (int i = 0, length = ntf.Resources.Count; i < length; ++i) {
                var one = ntf.Resources[i];
                if (this.allRes.TryGetValue(one.Type, out var t)) {
                    t.max = one.MaxCount;
                    t.leftCount = one.LeftCount;
                    t.nextRefreshTime = one.FreshTime;
                }
                else {
                    t = new Res() {
                        resId = one.Type,
                        max = one.MaxCount,
                        leftCount = one.LeftCount,
                        nextRefreshTime = one.FreshTime,
                    };
                    this.allRes.Add(one.Type, t);
                }
            }

            this.eventEmitter.Trigger(EEvents.OnResChanged);
        }

        public uint score { get; private set; }

        private void OnRecvUpdateGuildBattleScoreNty(NetMsg netMsg) {
            CmdGuildUpdateGuildBattleScoreNty ntf = NetMsgUtil.Deserialize<CmdGuildUpdateGuildBattleScoreNty>(CmdGuildUpdateGuildBattleScoreNty.Parser, netMsg);
            this.score = ntf.Score;
        }

        private void OnRecvDisplayApplyHintNty(NetMsg netMsg) {
            CmdGuildBattleDisplayApplyHintNty ntf = NetMsgUtil.Deserialize<CmdGuildBattleDisplayApplyHintNty>(CmdGuildBattleDisplayApplyHintNty.Parser, netMsg);
            this.PendingAction = this.OpenSignupMsgBox;

            this.eventEmitter.Trigger(EEvents.OnReceiveSignupNtf);
        }

        public void ReqChoseSignupSetting(bool toChoose) {
            CmdGuildBattleConfigureApplyHintReq req = new CmdGuildBattleConfigureApplyHintReq();
            req.Open = toChoose;
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.ConfigureApplyHintReq, req);
        }

        private void OnApplyHintConfigNty(NetMsg netMsg) {
            CmdGuildBattleApplyHintConfigNty ntf = NetMsgUtil.Deserialize<CmdGuildBattleApplyHintConfigNty>(CmdGuildBattleApplyHintConfigNty.Parser, netMsg);
            var old = this.chooseSignupSetting;
            if (ntf.Hintswitch != null) {
                this.chooseSignupSetting = ntf.Hintswitch.Open;
            }
            else {
                this.chooseSignupSetting = false;
            }

            if (old != this.chooseSignupSetting) {
                this.eventEmitter.Trigger(EEvents.OnSignupSettingChanged);
            }
        }

        public bool isFocusing = true;

        public void ReqFocusTeamList() {
            CmdGuildBattleFocusCoreTeamReq req = new CmdGuildBattleFocusCoreTeamReq();
            req.Open = isFocusing;
            NetClient.Instance.SendMessage((ushort) CmdGuildBattle.FocusCoreTeamReq, req);
        }

        private void OnCoreTeamDataNty(NetMsg netMsg) {
            CmdGuildBattleCoreTeamDataNty ntf = NetMsgUtil.Deserialize<CmdGuildBattleCoreTeamDataNty>(CmdGuildBattleCoreTeamDataNty.Parser, netMsg);
            isFocusing = ntf.Open;

            guildTeams.Clear();
            if (isFocusing) {
                for (int i = 0, length = ntf.Data.Guilds.Count; i < length; ++i) {
                    var guildId = ntf.Data.Guilds[i].GuildId;
                    var ls = new List<BattleCoreTeamMapData>();
                    ls.AddRange(ntf.Data.Guilds[i].Teams);
                    ls.Sort((l, r) => { return (int) l.Rank - (int) r.Rank; });

                    guildTeams.Add(guildId, ls);
                }
            }

            eventEmitter.Trigger<bool>(EEvents.OnReceiveTeamOrRankChanged, false);
        }

        // 排名变化，队伍数量变化，或者team内部数据变化
        private void OnUpdateCoreTeamNty(NetMsg netMsg) {
            CmdGuildBattleUpdateCoreTeamNty ntf = NetMsgUtil.Deserialize<CmdGuildBattleUpdateCoreTeamNty>(CmdGuildBattleUpdateCoreTeamNty.Parser, netMsg);
            var guildId = ntf.GuildId;
            if (guildTeams.TryGetValue(guildId, out var ls)) {
                // 倒序删除，防止2,3名被删除，然后ls.remove(2)然后ls.remove(3)出错
                for (int i = ntf.Updates.Count - 1; i >= 0; --i) {
                    var team = ntf.Updates[i];
                    int index = (int) team.Rank - 1;
                    if (team.Roles == null || team.Roles.Count <= 0) {
                        // 某个team整体退出战场
                        // rank从1开始
                        ls.RemoveAt(index);
                    }
                    else {
                        if (index >= ls.Count) { // 新增
                            ls.Add(team);
                        }
                        else {
                            // 替换 排名变化，或者某个team内部数据变化，则数据整体覆盖
                            ls[index] = team;
                        }
                    }
                }
                
                eventEmitter.Trigger<bool>(EEvents.OnReceiveTeamOrRankChanged, true);
            }
        }

        private void OnChangeMap(uint lastMapId, uint curMapId) {
            DebugUtil.Log(ELogType.eFamilyBattle, "OnChangeMap   " + "last: " + lastMapId + "cur: " + curMapId);
            if (curMapId != 1450) {
                if (this.InFamilyBattle) {
                    Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
                    this.ClearBattleFieldData();
                    this._CloseUIs();
                    //
                    // ActionCtrl.Instance.Reset();
                    // this.timer?.Cancel();
                    // Sys_Input.Instance.bEnableJoystick = true;
                    // Sys_Input.Instance.bForbidControl = false;
                    // Sys_Input.Instance.bForbidTouch = false;
                    //
                    // this.timerTryEnter?.Cancel();
                }

                this.InFamilyBattle = false;
                if (GameCenter.mainHero == null) {
                    return;
                }

                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearFamilyBattle, GameCenter.mainHero.uID);

                ActorHUDTitleNameUpdateEvt actorHUDTitleNameUpdateEvt = new ActorHUDTitleNameUpdateEvt();
                actorHUDTitleNameUpdateEvt.id = GameCenter.mainHero.uID;
                actorHUDTitleNameUpdateEvt.eFightOutActorType = EFightOutActorType.MainHero;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName, actorHUDTitleNameUpdateEvt);
            }
            else {
                UIManager.ClearUntilMain(false);
            }
        }

        private void _CloseUIs() {
            UIManager.CloseUI(EUIID.UI_FamilyResBattleActorList);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleIntroduce);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleMap);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleRank);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleReborn);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleResult);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleSubmitResult);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleTeam);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleTop);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleBeginTip);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleMapHoldResPlayer);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleCDRes);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleTeamMember);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleFamilyRank);
        }

        private void OnSceneInfoNtf(CmdGuildSceneInfoNtf ntf) {
            if (ntf.Guildbattle != null) {
                this._ProcessActivity(ntf.Guildbattle);
            }
        }

        public uint weeklyUsedCount { get; private set; }
        private Timer weeklyTimer;

        private void _ProcessActivity(SceneGuildBattleInfo info) {
            this.weeklyUsedCount = info.WeeklyTimes;

            long now = Sys_Time.Instance.GetServerTime();
            long diff = info.ResetTime - now;
            // 一周刷新次数
            // timer充当update使用
            this.weeklyTimer?.Cancel();
            this.weeklyTimer = Timer.Register(diff, () => { this.weeklyUsedCount = 0; });
        }

        private void OnWallAction(bool toAwake) {
            if (toAwake) {
                this.TrySetWall();
            }
        }

        private void TrySetWall(bool flag = true) {
            var wall = Framework.CollisionWall.Instance;
            if (wall != null) {
                wall.Ctrl(this.redCampId == (uint) Framework.CollisionWall.ECamp.Left, (this.stage == EStage.Battle) && flag);
            }
        }

        #endregion

        #region 逻辑

        public bool IsSignupMember(ulong uid) {
            bool rlt = true;
            // 家族族长，副族长
            uint pos = 100;
            var fd = Sys_Family.Instance.familyData;
            if (fd != null) {
                var mb = fd.CheckMember(uid);
                if (mb != null) {
                    pos = mb.Position % 10000;
                }
            }

            Sys_Family.FamilyData.EFamilyStatus familyStatus = (Sys_Family.FamilyData.EFamilyStatus) (pos);
            var csv = CSVFamilyPostAuthority.Instance.GetConfData((uint) familyStatus);
            if (csv != null) {
                rlt &= (csv.BattleEnroll != 0);
            }
            else {
                rlt = false;
            }

            return rlt;
        }

        // 能否报名
        public bool CanSingup(ulong uid, out ESignupReason reason) {
            bool rlt = true;
            reason = ESignupReason.Valid;
            // 家族等级 >=2
            rlt &= (Sys_Family.Instance.familyData.GetGuildLevel() >= (uint) Sys_FamilyResBattle.Instance.SignupLimitedFamilyLevel);
            if (!rlt) {
                reason = ESignupReason.InValidFalimyLevel;
                return rlt;
            }

            rlt &= (!this.hasSigned);
            if (!rlt) {
                return rlt;
            }

            // 家族人数 >=10
            //rlt &= (Sys_Family.Instance.GetGuildMemberNum() >= (uint)Sys_FamilyResBattle.Instance.SignupLimitedFamilyMemberCount);
            //if (!rlt) {
            //    reason = ESignupReason.InValidFalimyMemberCount;
            //    return rlt;
            //}

            // 家族族长，副族长
            uint pos = 100;
            var fd = Sys_Family.Instance.familyData;
            if (fd != null) {
                var mb = fd.CheckMember(uid);
                if (mb != null) {
                    pos = mb.Position % 10000;
                }
            }

            Sys_Family.FamilyData.EFamilyStatus familyStatus = (Sys_Family.FamilyData.EFamilyStatus) (pos);
            var csv = CSVFamilyPostAuthority.Instance.GetConfData((uint) familyStatus);
            if (csv != null) {
                rlt &= (csv.BattleEnroll != 0);
            }
            else {
                rlt = false;
            }

            if (!rlt) {
                reason = ESignupReason.NotLeaderOrViceLeader;
                return rlt;
            }

            return rlt;
        }

        // 家族资源战报名已经开始，是否前往报名
        public void OpenSignupMsgBox() {
            PromptBoxParameter.Instance.Clear();
            //PromptBoxParameter.Instance.SetCountdown(10f, PromptBoxParameter.ECountdown.Cancel);
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(3230000036).words;
            PromptBoxParameter.Instance.SetConfirm(true, OnConform);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

            void OnConform() {
                UIManager.OpenUI(EUIID.UI_Family, false, new UI_FamilyOpenParam() {
                    familyMenuEnum = (uint) EFamilyMenu.FamilyPVP,
                });
                UIManager.OpenUI(EUIID.UI_FamilyResBattleMain, false, 1u);
            }
        }

        public bool CanEnter(out EEnterBattleReason reason) {
            reason = EEnterBattleReason.Valid;
            bool rlt = (this.stage == EStage.Battle);
            if (!rlt) {
                reason = EEnterBattleReason.InvalidStage;
                return rlt;
            }

            rlt &= (this.hasSigned);
            if (!rlt) {
                reason = EEnterBattleReason.NotSignup;
                return rlt;
            }

            if (!Sys_Team.Instance.HaveTeam) {
                rlt &= (Sys_Role.Instance.Role.Level >= this.EnterLimitedPlayerLevel);
                if (!rlt) {
                    reason = EEnterBattleReason.InvalidLevel;
                    return rlt;
                }
            }
            else {
                //
            }

            return rlt;
        }

        private Timer timerTryEnter;

        private int hasEnterOpenCount = 0;

        // 家族资源战已经开始，是否前往参战
        public void TryOpenEnterMsgBox() {
            bool can = this.CanEnter(out EEnterBattleReason reason) && this.hasBlue && !this.InFamilyBattle;
            if (!can) {
                return;
            }

            can = Sys_Team.Instance.canManualOperate;
            if (!can) {
                return;
            }

            can = !Sys_Instance.Instance.IsInInstance && !Sys_CutScene.Instance.isPlaying && !Sys_Fight.Instance.IsFight();
            if (!can) {
                return;
            }

            can = this.hasEnterOpenCount <= 0;
            if (!can) {
                return;
            }

            void _DoOpen() {
                bool canOpen = PromptBoxParameter.Instance.OpenActivityPriorityPromptBox(PromptBoxParameter.EPriorityType.FamilyResBattle,
                    CSVLanguage.Instance.GetConfData(3230000034).words,
                    OnConform);

                if (canOpen) {
                    ++this.hasEnterOpenCount;
                }

                void OnConform() {
                    if (Sys_Instance.Instance.IsInInstance || Sys_CutScene.Instance.isPlaying || Sys_Fight.Instance.IsFight()) {
                    }
                    else {
                        ActionCtrl.Instance.MoveToTargetNPCAndInteractive(this.EnterNpcId);
                    }
                }
            }

            //Debug.LogError(Sys_Role.Instance.hasSyncFinished + "  " + this.fromReconnect + "   " + UIManager.IsVisibleAndOpen(EUIID.UI_Menu));
            if (!Sys_Role.Instance.hasSyncFinished) {
                _DoOpen();
            }
            else {
                if (!this.fromReconnect) {
                    if (UIManager.IsVisibleAndOpen(EUIID.UI_Menu)) {
                        _DoOpen();
                    }
                }
            }
        }

        // 家族资源战已经开始，是否离开战场
        public void OpenLeaveMsgBox() {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.SetCountdown(10f, PromptBoxParameter.ECountdown.Cancel);
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(3230000035).words;
            PromptBoxParameter.Instance.SetConfirm(true, OnConform);
            PromptBoxParameter.Instance.SetCancel(true, null);

            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

            void OnConform() {
                this.ReqLeave();
            }
        }

        public bool HasResource(out uint resId) {
            bool rlt = this.HasResource(Sys_Role.Instance.RoleId, out resId);
            return rlt;
        }

        public bool HasResource(ulong roleId, out uint resId) {
            resId = 0;
            if (roleId == Sys_Role.Instance.RoleId) {
                resId = this.Resource;
                return resId != 0;
            }
            else {
                if (GameCenter.otherActorsDic.TryGetValue(roleId, out Hero hero)) {
                    bool valid = hero.familyResBattleComponent != null;
                    if (valid) {
                        resId = hero.familyResBattleComponent.resource;
                    }

                    return resId != 0;
                }

                return false;
            }
        }

        // 当前账号所属阵营
        public Framework.CollisionWall.ECamp MyCampId() {
            return this.redCampId == 1 ? Framework.CollisionWall.ECamp.Left : Framework.CollisionWall.ECamp.Right;
        }

        public bool IsInSafeArea() {
            return this.IsInSafeArea(Sys_Role.Instance.RoleId);
        }

        public bool IsInSafeArea(SceneActor actor) {
            if (actor != null && actor.transform != null) {
                return Sys_Npc.Instance.IsInSafeArea(Sys_Map.Instance.CurMapId, actor.transform);
            }

            return false;
        }

        public bool IsInSafeArea(ulong roleId) {
            if (this.redRoles.TryGetValue(roleId, out var role) || this.blueRoles.TryGetValue(roleId, out role)) {
                //SceneActor actor = GameCenter.mainWorld.GetActor(typeof(Hero), roleId) as SceneActor;
                Hero actor = GameCenter.GetSceneHero(roleId);
                if (actor != null) {
                    // 视野外
                    return this.IsInSafeArea(actor);
                }
            }

            return false;
        }

        #endregion
    }

    public partial class Sys_FamilyResBattle : SystemModuleBase<Sys_FamilyResBattle> {
        public enum EEvents {
            OnStageChanged, // 阶段变化
            OnSignupChanged, // 报名
            OnSignupConditionChanged, // 报名条件

            OnMyResChanged, // 资源变化
            OnResChanged, // 资源变化
            OnEnter, // 进入战场

            OnEnterField, // 进入战场
            OnLeaveField, // 离开战场

            OnSignupSettingChanged, // 报名设置状态修改
            OnReceiveSignupNtf, // 可报名通知
            
            OnReceiveTeamOrRankChanged, // 队伍变化或者排名变化

            OnProtectBuffAdd,
            OnProtectBuffRemove,
        }

        // 和表格匹配
        public enum EStage {
            UnOpen = 0, // 活动未开启
            Signup = 1, // 报名阶段
            Match = 2, // 活动匹配阶段
            ReadyBattle = 3, // 站前准备阶段
            Battle = 4, // 战斗阶段
            BattleEnd = 5, // 战斗结束阶段 -> 报名阶段
        }

        public EStage stage { get; set; } = EStage.UnOpen;
        public long endTimeOfStage = 0;

        // 是否有匹配到敌方
        public bool noTurn = false;

        // 是否报名
        public bool hasSigned { get; private set; } = false;

        public bool chooseSignupSetting { get; private set; } = true;

        // 是否在战场内
        public bool _inFamilyBattle = false;

        public bool InFamilyBattle {
            get { return this._inFamilyBattle; }
            /*private*/
            set {
                var old = this._inFamilyBattle;
                this._inFamilyBattle = value;
                if (old != this._inFamilyBattle) {
                    if (this._inFamilyBattle) {
                        this.eventEmitter.Trigger<bool, bool>(EEvents.OnEnterField, old, this._inFamilyBattle);
                    }
                    else {
                        this.eventEmitter.Trigger<bool, bool>(EEvents.OnLeaveField, old, this._inFamilyBattle);
                    }
                }
            }
        }

        //自己头顶的数据
        public uint TeamNum { get; private set; }

        //自己头顶的数据
        public uint MaxCount { get; private set; }

        #region 敌方家族信息

        public bool hasBlue { get; private set; } = false;
        public uint blueServerId;
        public string blueServerName;

        public ulong blueFamlilyId;
        public string blueFamlilyName;

        #endregion

        #region 战场数据

        // 退出战场的时候清理，否则浪费内存
        public void ClearBattleFieldData() {
            this.allRols?.Clear();
            this.redRoles?.Clear();
            this.allRes?.Clear();
            this.teamMemberCounts.Clear();
            this.rolesInBattle?.Clear();
            this.blueRoles?.Clear();
            this.guilds?.Clear();
        }

        // 队伍成员个数
        public Dictionary<ulong, uint> teamMemberCounts = new Dictionary<ulong, uint>();

        // 是否在战斗中
        public HashSet<ulong> rolesInBattle = new HashSet<ulong>();

        public class Res {
            public uint resId;
            public uint max;
            public uint leftCount;
            public long nextRefreshTime;
        }

        // 每个成员的资源个数
        public SortedDictionary<uint, Res> allRes = new SortedDictionary<uint, Res>();

        public Dictionary<ulong, BattleRoleMapData> redRoles = new Dictionary<ulong, BattleRoleMapData>();
        public Dictionary<ulong, BattleRoleMapData> blueRoles = new Dictionary<ulong, BattleRoleMapData>();
        public List<BattleRoleMapData> allRols = new List<BattleRoleMapData>();

        public Dictionary<ulong, BattleGuildMapData> guilds = new Dictionary<ulong, BattleGuildMapData>();
        public Dictionary<ulong, List<BattleCoreTeamMapData>> guildTeams = new Dictionary<ulong, List<BattleCoreTeamMapData>>();
        
        // 排行榜
        // cd和排行榜数据
        public long cd;
        public List<RankUnitData> ranks = new List<RankUnitData>();

        // 整个公会全部队伍的总分
        public long TotalScore(ulong guildId) {
            long ret = 0;
            if (guildTeams.TryGetValue(guildId, out var ls)) {
                for (int i = 0, length = ls.Count; i < length; ++i) {
                    ret += TotalScore(guildId, (int)ls[i].Rank);
                }
            }

            return ret;
        }
        
        // 公会中某个rank队伍的总分
        public long TotalScore(ulong guildId, int rank) {
            long ret = 0;
            if (guildTeams.TryGetValue(guildId, out var ls)) {
                for (int i = 0, length = ls.Count; i < length; ++i) {
                    if (ls[i].Roles != null && ls[i].Roles.Count > 0 && ls[i].Rank == rank) {
                        for (int j = 0, lengthJ = ls[i].Roles.Count; j < lengthJ; ++j) {
                            ret += ls[i].Roles[j].TotalScore;
                        }
                    }
                }
            }

            return ret;
        }

        public uint TotalRes(ulong familyId) {
            if (!this.guilds.TryGetValue(familyId, out var g)) {
                return 0;
            }
            else {
                return g.Resource;
            }
        }

        public uint redCampId;
        public uint blueCampId;

        public ulong redFamilyId;
        public ulong blueFamilyId;

        public enum ERoleSortType {
            All,
            Red,
            Blue,
        }

        public List<BattleRoleMapData> GetRoles(ERoleSortType type) {
            List<BattleRoleMapData> ls = new List<BattleRoleMapData>();
            if (type == ERoleSortType.All) {
                ls = this.allRols;
            }
            else if (type == ERoleSortType.Red) {
                foreach (var item in this.redRoles) {
                    ls.Add(item.Value);
                }
            }
            else if (type == ERoleSortType.Blue) {
                foreach (var item in this.blueRoles) {
                    ls.Add(item.Value);
                }
            }

            return ls;
        }

        public BattleGuildMapData GetFamilyByRoleId(ulong roleId) {
            if (this.redRoles.TryGetValue(roleId, out var _)) {
                return this.guilds[this.redFamilyId];
            }
            else if (this.blueRoles.TryGetValue(roleId, out var _)) {
                return this.guilds[this.blueFamilyId];
            }

            return null;
        }

        // 阻挡强使用
        public long GetRemainReadyBattleTime() {
            long rlt = -1;
            if (this.stage == EStage.ReadyBattle) {
                rlt = this.endTimeOfStage - Sys_Time.Instance.GetServerTime();
            }

            return rlt;
        }

        #endregion

        #region // 表格数据

        // 报名
        public int SignupLimitedFamilyLevel;
        public int SignupLimitedFamilyMemberCount;
        public int SignupLimitedFamilyActiveMemberLevel;
        public int SignupLimitedFamilyActiveMemberCount;

        // 进入战场
        public uint EnterNpcId;
        public int EnterLimitedPlayerLevel;
        public int EnterLimitedPlayerJoinFamilyDays;

        public uint maxAttackDistance;

        public uint battlewinMaxRes;

        public uint battlewinFamilyDropId;
        public uint battlefailFamilyDropId;
        public uint battlepingFamilyDropId;

        public uint battlewinPersonalDropId;
        public uint battlefailPersonalDropId;
        public uint battlepingPersonalDropId;

        public List<uint> preWins = new List<uint>();

        #endregion

        // 报名失败原因
        public enum ESignupReason {
            ActivityUnOpen = 0, // 活动没开启
            NotLeaderOrViceLeader, // 非leader
            InValidFalimyLevel, // 家族等级不达标
            InValidFalimyMemberCount, // 家族成员个数不足
            InvalidFamilyActiveMemberCount, //
            Valid, // 全部合理
        }

        // 进战场失败原因
        public enum EEnterBattleReason {
            InvalidStage,
            NotSignup, // 未报名

            InvalidLevel, // 等级不达标
            InvalidDurationInGuild, // 在家族时长不达标

            NotSameFamily, // 有队伍成员不在家族中
            SomeoneZanLiInTeam, // 暂离
            SomeoneOfflineInTeam, // 离线

            Valid,
        }
    }
}