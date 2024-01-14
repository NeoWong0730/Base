using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using UnityEngine;
using Table;
using System.Collections.Generic;
using pbc = global::Google.Protobuf.Collections;

namespace Logic
{
    public partial class Sys_Map : SystemModuleBase<Sys_Map>
    {
        public enum EEvents
        {
            OnChangeMap,
            OnEnterMap,
            OnHeroTel,
            OnTransTipStart,
            OnTransTipIntterupt,
            OnAutoPathFind,
            OnInterruptPathFind,
            OnPathFindComplete,
            OnCloseMapView,
            OnTeleErr,
            OnLoadOK,
            OnRoleSameMapTel,
            OnCloseSelectMark,
            OnGenWayPoints,
            OnWayPointsEnd,

            OnFirstEnterMap,
            OnMapBitChanged,
            OnReadMapMail,
            OnReadMapResource,
            
            OnProtectBuffAdd,
            OnProtectBuffRemove,
            
            OnRemoveRole,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public uint LastMapId = 0;
        public uint CurMapId = 0;
        public uint svrPosX = 0u;
        public uint svrPosY = 0u;

        private bool IsEnterMap = false;
        public bool isLoadOk { get; private set; } = false;

        public bool IsTransTipOver = false;
        private CmdMapNpcTelReq reqTelNpc = new CmdMapNpcTelReq();

        public uint TeleErrMapId = 0;
        public string TeleErrFamilyName = "";
        public int LastExploreTab = (int)UI_MapExplore.ETabType.Explore;

        public enum ESwitchMapUI
        {
            UILoading2,
            NoLoading,
        }

        public ESwitchMapUI switchMode = ESwitchMapUI.UILoading2;

        public bool IsTelState = false;

        private Timer timerTelCD;

        public override void Init()
        {
            base.Init();

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.AddObjNtf, OnAddObjNtf, CmdMapAddObjNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RemoveObjNtf, OnRemoveObjNtf, CmdMapRemoveObjNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleEnterNtf, OnEnterNtf, CmdMapRoleEnterNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleMoveCorrect, OnMoveCorrect, CmdMapRoleMoveCorrect.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleMoveNtf, OnMapRoleMoveNtf, CmdMapRoleMoveNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.SameMapTelNtf, OnSameMapTel, CmdMapSameMapTelNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.TeleErrNtf, OnTeleErrNtf, CmdMapTeleErrNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.ChangeMountNtf, OnChangeMountNtf, CmdMapChangeMountNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.ChangeFollowPetNtf, OnChangeFollowPetNtf, CmdMapChangeFollowPetNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.ChangeTeamNtf, OnChangeTeamNtf, CmdMapChangeTeamNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.ChangeRoleSignNtf, OnChangeRoleSignNtf, CmdMapChangeRoleSignNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.ChangeRoleMoveSpeedNtf, OnChangeRoleSpeedNtf, CmdMapChangeRoleMoveSpeedNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.ChangeTeamLogoNtf, OnChangeTeamLogoNtf, CmdMapChangeTeamLogoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleActionNtf, OnRoleActionNtf, CmdMapRoleActionNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.UpdateMapGuildInfoNtf, OnUpdateMapGuildInfoNtf, CmdMapUpdateMapGuildInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleMapInfoNtf, this.OnReceiveMapBits, CmdMapRoleMapInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleRenameNtf, this.OnRoleReNameUpdate, CmdMapRoleRenameNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.UpdateRoleBgroupInfoNtf, OnUpdateRoleBgroupInfoNtf, CmdMapUpdateRoleBGroupInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.UpdateRoleBgroupQuitNtf, OnUpdateRoleBgroupQuitNtf, CmdMapUpdateRoleBGroupQuitNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.UpdateRoleBgroupPosNtf, OnUpdateRoleBgroupPosNtf, CmdMapUpdateRoleBGroupPosNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.UpdateRoleBgroupNameNtf, OnUpdateRoleBgroupNameNtf, CmdMapUpdateRoleBGroupNameNtf.Parser);
            
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.DataNty, this.OnCmdMapDataNty, CmdMapDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.AddRoleBuffNty, this.OnCmdMapAddRoleBuffNty, CmdMapAddRoleBuffNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.RemoveRoleBuffNty, this.OnCmdMapRemoveRoleBuffNty, CmdMapRemoveRoleBuffNty.Parser);
            
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.AddOtherRoleBuffNty, this.OnAddOtherRoleBuffNty, CmdMapAddOtherRoleBuffNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdMap.RemoveOtherRoleBuffNty, this.OnRemoveOtherRoleBuffNty, CmdMapRemoveOtherRoleBuffNty.Parser);
            TransProtectedDistance = float.Parse(CSVParam.Instance.GetConfData(312).str_value) / 10000f;
            TransThresholdValue = float.Parse(CSVParam.Instance.GetConfData(915).str_value) / 10000f;
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.RoleReturn,OnRoleReturnNtf, CmdMapRoleReturn.Parser);
            mapDataDict.Clear();

            Sys_Pet.Instance.eventEmitter.Handle<uint, bool>(Sys_Pet.EEvents.OnEquipDemonSpiritSphereLevelChange, OnEquipDemonSpiritSphereLevelChange, true);
        }

        public override void OnLogin()
        {
            this.firstEnterMaps.Clear();
            this.unReadedMapMails.Clear();
            this.unReadedMapResources.Clear();
        }

        public override void OnLogout()
        {
            IsEnterMap = false;
            LastMapId = 0;
            CurMapId = 0;
            timerTelCD?.Cancel();
            timerTelCD = null;

            this.firstEnterMaps.Clear();
            this.unReadedMapMails.Clear();
            this.unReadedMapResources.Clear();

            base.OnLogout();
        }

        #region net msg

        void OnRoleActionNtf(NetMsg msg)
        {
            CmdMapRoleActionNtf ntf = NetMsgUtil.Deserialize<CmdMapRoleActionNtf>(CmdMapRoleActionNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.Roleid == Sys_Role.Instance.RoleId)
                {
                    //RoleActionComponent roleActionComponent = World.GetComponent<RoleActionComponent>(GameCenter.mainHero);

                    if (GameCenter.mainHero != null)
                    {
                        RoleActionComponent roleActionComponent = GameCenter.mainHero.roleActionComponent;

                        if (roleActionComponent != null)
                        {
                            roleActionComponent.SelfPlayAction(ntf.Action.Actionid);
                        }
                    }
                }
                else
                {
                    //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), ntf.Roleid) as Hero;
                    Hero hero = GameCenter.GetSceneHero(ntf.Roleid);
                    if (hero != null)
                    {
                        //RoleActionComponent roleActionComponent = World.GetComponent<RoleActionComponent>(hero);
                        RoleActionComponent roleActionComponent = hero.roleActionComponent;
                        if (roleActionComponent != null)
                        {
                            RoleActionComponent.RoleAction roleAction = new RoleActionComponent.RoleAction();
                            roleAction.actionID = ntf.Action.Actionid;
                            roleAction.direction = ntf.Action.Direction;
                            roleAction.startTime = ntf.Action.Starttime;

                            roleActionComponent.SetCacheRoleAction(roleAction);
                        }
                    }
                }
            }
        }

        public void ReqLeave()
        {
            CmdMapLeaveCopyMapReq req = new CmdMapLeaveCopyMapReq();
            NetClient.Instance.SendMessage((ushort)CmdMap.LeaveCopyMapReq, req);
        }

        void OnChangeMountNtf(NetMsg msg)
        {
            CmdMapChangeMountNtf ntf = NetMsgUtil.Deserialize<CmdMapChangeMountNtf>(CmdMapChangeMountNtf.Parser, msg);
            if (ntf != null)
            {
                //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), ntf.RoleId) as Hero;
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    if (ntf.MountPetInfoId == 0)
                    {
                        hero.OffMount();
                    }
                    else
                    {
                        hero.animationComponent.UpdateHoldingAnimations(hero.heroBaseComponent.HeroID, hero.weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), hero.stateComponent.CurrentState, hero.modelGameObject);
                        if (ntf.PetSuitAppearance != 0)
                            hero.OnMount(ntf.MountPetInfoId, hero.uID * 1000000 + ntf.MountPetInfoId * 10 + 1, CSVPetEquipSuitAppearance.Instance.GetConfData(ntf.PetSuitAppearance).show_id, ntf.MountPetBuild, ntf.MountPetSoul);
                        else
                            hero.OnMount(ntf.MountPetInfoId, hero.uID * 1000000 + ntf.MountPetInfoId * 10 + 1, 0, ntf.MountPetBuild, ntf.MountPetSoul);
                    }
                }
            }
        }

        void OnChangeFollowPetNtf(NetMsg msg)
        {
            CmdMapChangeFollowPetNtf ntf = NetMsgUtil.Deserialize<CmdMapChangeFollowPetNtf>(CmdMapChangeFollowPetNtf.Parser, msg);
            if (ntf != null)
            {
                //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), ntf.RoleId) as Hero;
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    if (ntf.FollowPetInfo == 0)
                    {
                        hero.RemovePet();
                    }
                    else
                    {
                        if (ntf.PetSuitAppearance != 0)
                            hero.AddPet(ntf.FollowPetInfo, ntf.RoleId * 1000000 + ntf.FollowPetInfo * 10 + 2, CSVPetEquipSuitAppearance.Instance.GetConfData(ntf.PetSuitAppearance).show_id, ntf.FollowPetBuild, ntf.FollowPetSoul);
                        else
                            hero.AddPet(ntf.FollowPetInfo, ntf.RoleId * 1000000 + ntf.FollowPetInfo * 10 + 2, 0, ntf.FollowPetBuild, ntf.FollowPetSoul);
                    }
                }
            }
        }

        void OnEquipDemonSpiritSphereLevelChange(uint uid, bool magicSoul)
        {
            if (GameCenter.mainHero.Mount != null && GameCenter.mainHero.Mount.UID == uid)
            {
                if (magicSoul)
                {
                    EffectUtil.Instance.LoadEffect(GameCenter.mainHero.UID, CSVParam.Instance.GetConfData(1575).str_value, GameCenter.mainHero.Mount.fxRoot.transform, EffectUtil.EEffectTag.MagicSoul, 0, 1, 1, ELayerMask.Player);
                    GameCenter.mainHero.Mount.MagicSoul = true;
                }
                else
                {
                    EffectUtil.Instance.UnloadEffectByTag(GameCenter.mainHero.UID, EffectUtil.EEffectTag.MagicSoul);
                    GameCenter.mainHero.Mount.MagicSoul = false;
                }
            }

            if (GameCenter.mainHero.Pet != null && GameCenter.mainHero.Pet.UID == uid)
            {
                if (magicSoul)
                {
                    EffectUtil.Instance.LoadEffect(GameCenter.mainHero.UID, CSVParam.Instance.GetConfData(1575).str_value, GameCenter.mainHero.Pet.fxRoot.transform, EffectUtil.EEffectTag.MagicSoul, 0, 1, 1, ELayerMask.Player);
                    GameCenter.mainHero.Pet.MagicSoul = true;
                }
                else
                {
                    EffectUtil.Instance.UnloadEffectByTag(GameCenter.mainHero.UID, EffectUtil.EEffectTag.MagicSoul);
                    GameCenter.mainHero.Pet.MagicSoul = false;
                }
            }
        }

        void OnChangeTeamNtf(NetMsg msg)
        {
            CmdMapChangeTeamNtf ntf = NetMsgUtil.Deserialize<CmdMapChangeTeamNtf>(CmdMapChangeTeamNtf.Parser, msg);
            if (ntf != null)
            {
                //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), ntf.TeamInfo.RoleId) as Hero;
                Hero hero = GameCenter.GetSceneHero(ntf.TeamInfo.RoleId);
                if (hero != null)
                {
                    hero.heroBaseComponent.TeamID = ntf.TeamInfo.TeamId;
                    hero.heroBaseComponent.IsCaptain = ntf.TeamInfo.LeaderId == hero.uID;
                    hero.heroBaseComponent.TeamMemNum = ntf.TeamInfo.MemNum;
                    if (hero.Pet != null)
                    {
                        if (ntf.TeamInfo.TeamId != 0)
                        {
                            hero.Pet.SetLayerHide();
                        }
                        else
                        {
                            hero.Pet.ReturnCacheLayer();
                        }
                    }
                }
            }
        }

        void OnChangeRoleSignNtf(NetMsg msg)
        {
            CmdMapChangeRoleSignNtf ntf = NetMsgUtil.Deserialize<CmdMapChangeRoleSignNtf>(CmdMapChangeRoleSignNtf.Parser, msg);
            if (ntf != null)
            {
                //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), ntf.RoleId) as Hero;
                //hero.heroBaseComponent.bInFight = ntf.RoleSign == 1;
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    hero.heroBaseComponent.bInFight = ntf.RoleSign == 1;
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateHeroFunState, ntf.RoleId, ntf.RoleSign);
                }
            }
        }

        /// <summary>
        ///  1.队长更换对标   2.创建队伍   3.队长切换   4.玩家进入队伍后 队伍满员   5.满员状态离开队伍 teamLogoId*10 + flag(1:满员 0:未满员)
        /// </summary>
        /// <param name="netMsg"></param>
        void OnChangeTeamLogoNtf(NetMsg netMsg)
        {
            CmdMapChangeTeamLogoNtf cmdMapChangeTeamLogoNtf = NetMsgUtil.Deserialize<CmdMapChangeTeamLogoNtf>(CmdMapChangeTeamLogoNtf.Parser, netMsg);

            if (cmdMapChangeTeamLogoNtf.TeamLogo == 0) //不是队长的时候 会发0
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamLogo, cmdMapChangeTeamLogoNtf.RoleId);
            }
            else
            {
                uint teamLogoId = cmdMapChangeTeamLogoNtf.TeamLogo / 10;
                Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnCreateTeamLogo, cmdMapChangeTeamLogoNtf.RoleId, teamLogoId);
                if (cmdMapChangeTeamLogoNtf.TeamLogo % 10 == 1)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateTeamFx, cmdMapChangeTeamLogoNtf.RoleId);
                }
                else if (cmdMapChangeTeamLogoNtf.TeamLogo % 10 == 0)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamFx, cmdMapChangeTeamLogoNtf.RoleId);
                }
            }
        }

        void OnUpdateMapGuildInfoNtf(NetMsg netMsg)
        {
            CmdMapUpdateMapGuildInfoNtf ntf = NetMsgUtil.Deserialize<CmdMapUpdateMapGuildInfoNtf>(CmdMapUpdateMapGuildInfoNtf.Parser, netMsg);
            Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
            if (hero!=null)
            {
                hero.heroBaseComponent.FamilyName = ntf.GuildInfo.Name.ToStringUtf8();
                hero.heroBaseComponent.Pos = ntf.GuildInfo.Pos;
            }
            //UpdateFamliyNameBeforeBattleRescorce updateFamliyNameBeforeBattleRescorce = new UpdateFamliyNameBeforeBattleRescorce();
            //updateFamliyNameBeforeBattleRescorce.actorId = Sys_Role.Instance.RoleId;
            //updateFamliyNameBeforeBattleRescorce.name = ntf.GuildInfo.Name.ToStringUtf8();
            //updateFamliyNameBeforeBattleRescorce.pos = ntf.GuildInfo.Pos;
            DebugUtil.Log(ELogType.eFamilyBattle, "OnUpdateMapGuildInfoNtf===>  " + "name:  " +
                                                  ntf.GuildInfo.Name.ToStringUtf8() + "   pos: " + ntf.GuildInfo.Pos);
            //Sys_HUD.Instance.eventEmitter.Trigger<UpdateFamliyNameBeforeBattleRescorce>(Sys_HUD.EEvents.OnUpdateFamilyName, updateFamliyNameBeforeBattleRescorce);
        }

        void OnChangeRoleSpeedNtf(NetMsg msg)
        {
            CmdMapChangeRoleMoveSpeedNtf ntf = NetMsgUtil.Deserialize<CmdMapChangeRoleMoveSpeedNtf>(CmdMapChangeRoleMoveSpeedNtf.Parser, msg);
            if (ntf != null)
            {
                //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), ntf.RoleId) as Hero;
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    hero.movementComponent.fMoveSpeed = ntf.MoveSpeed / 10000f;
                }
            }
        }

        private void OnAddObjNtf(NetMsg msg)
        {
            //if (GameCenter.mainWorld == null)
            //    return;

            CmdMapAddObjNtf res = NetMsgUtil.Deserialize<CmdMapAddObjNtf>(CmdMapAddObjNtf.Parser, msg);

            for (int index = 0, len = res.Npcs.Count; index < len; index++)
            {
                //if (res.Npcs[index].InfoId != 101191)
                NPCHelper.AddNPC(res.Npcs[index]);
            }

            for (int index = 0, len = res.Roles.Count; index < len; index++)
            {
                GameCenter.AddHero(res.Roles[index]);
            }

            CoroutineManager.Instance.Start(NPCHelper.CreateNpcModelIEs(res.Npcs));
        }

        private void OnRemoveObjNtf(NetMsg msg)
        {
            //if (GameCenter.mainWorld == null)
            //    return;

            CmdMapRemoveObjNtf res = NetMsgUtil.Deserialize<CmdMapRemoveObjNtf>(CmdMapRemoveObjNtf.Parser, msg);

            foreach (var role in res.Role)
            {
                //GameCenter.mainWorld.DestroyActor(Hero.Type, role);
                GameCenter.RemoveHero(role);
                this.eventEmitter.Trigger<ulong>(EEvents.OnRemoveRole, role);
            }

            if (res.Flag == 0)
            {
                foreach (var npc in res.Npc)
                {
                    NPCHelper.DeleteNPC(npc);
                }
            }
            else if (res.Flag == 1)
            {
                foreach (var npcID in res.Npc)
                {
                    //Npc npc = GameCenter.mainWorld.GetActor(Npc.Type, npcID) as Npc;
                    if (GameCenter.TryGetSceneNPC(npcID, out Npc npc))
                    {
                        if (npc.cSVNpcData.type == (uint)ENPCType.WorldBoss)
                        {
                            WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () => { NPCHelper.DeleteNPC(npcID); }, true, (int)NPCEnum.B_WorldBossDead, npc.uID);
                        }
                        else
                        {
                            NPCHelper.DeleteNPC(npcID);
                        }
                    }
                }
            }
        }

        //频繁调度 缓存一份实例
        uint lastMoveX;
        uint lastMoveY;

        public void ReqMove(bool isTeam = false)
        {
            //DebugUtil.LogWarningFormat("UpLoad====2");
            Vector2 svrPos = PosConvertUtil.Client2Svr(GameCenter.mainHero.transform.position);
            CmdMapRoleMoveReq mMoveReq = new CmdMapRoleMoveReq();
            mMoveReq.PosX = (uint)Mathf.RoundToInt(svrPos.x);
            mMoveReq.PosY = (uint)Mathf.RoundToInt(svrPos.y);
            mMoveReq.Time = ((ulong)Sys_Time.Instance.GetServerTime() * 1000);

            if (mMoveReq.Mempos.Count > 0)
                mMoveReq.Mempos.Clear();

            if (isTeam)
                getTeamMembersPositions(ref mMoveReq);

            if (mMoveReq.PosX != lastMoveX || mMoveReq.PosY != lastMoveY)
            {
                //DebugUtil.LogWarningFormat("UpLoad====3");
                lastMoveX = mMoveReq.PosX;
                lastMoveY = mMoveReq.PosY;
                NetClient.Instance.SendMessage((ushort)CmdMap.RoleMoveReq, mMoveReq);
            }

        }

        List<TeamMemMove> teamMemMoves = new List<TeamMemMove>(5);

        private void getTeamMembersPositions(ref CmdMapRoleMoveReq cmdInfo)
        {
            int memcount = Sys_Team.Instance.TeamMemsCount;

            int moveCount = teamMemMoves.Count;

            int realIndex = 0;

            for (int i = 0; i < memcount; i++)
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(i);

                if (teamMem.IsLeave() || teamMem.IsOffLine())
                    continue;

                if (realIndex >= moveCount)
                {
                    teamMemMoves.Add(new TeamMemMove());
                    moveCount = teamMemMoves.Count;
                }

                //Hero hero = GameCenter.mainWorld.GetActor(Hero.Type, teamMem.MemId) as Hero;
                Hero hero = GameCenter.GetSceneHero(teamMem.MemId);
                Vector2 svrPos = Vector2.zero;

                if (hero != null && hero.transform != null)
                    svrPos = PosConvertUtil.Client2Svr(hero.transform.position);

                teamMemMoves[realIndex].PosX = hero != null && hero.transform != null ? (uint)svrPos.x : cmdInfo.PosX;
                teamMemMoves[realIndex].PosY = hero != null && hero.transform != null ? (uint)svrPos.y : cmdInfo.PosY;


                cmdInfo.Mempos.Add(teamMemMoves[realIndex]);

                realIndex++;
            }
        }

        private void OnMoveCorrect(NetMsg msg)
        {
            //DebugUtil.LogErrorFormat("OnMoveCorrect---");
            CmdMapRoleMoveCorrect res = NetMsgUtil.Deserialize<CmdMapRoleMoveCorrect>(CmdMapRoleMoveCorrect.Parser, msg);

            if (GameCenter.mainHero != null)
            {
                GameCenter.mainHero.syncTransformComponent.SyncNetPos(PosConvertUtil.Svr2Client(res.PosX, res.PosY));
                GameCenter.mainHero.movementComponent.TransformToPosImmediately(GameCenter.mainHero.syncTransformComponent.netPos);
            }
        }

        public void OnMapRoleMoveNtf(NetMsg msg)
        {
            CmdMapRoleMoveNtf res = NetMsgUtil.Deserialize<CmdMapRoleMoveNtf>(CmdMapRoleMoveNtf.Parser, msg);
            Hero hero = GameCenter.GetSceneHero(res.RoleId);
            DebugUtil.LogFormat(ELogType.eNone, "{0} {1}", res.RoleId, PosConvertUtil.Svr2Client(res.PosX, res.PosY));
            if (Sys_Team.Instance.NeedFollowCaptain(res.RoleId) && Sys_Role.Instance.RoleId == Sys_Team.Instance.CaptainRoleId)
                return;

            DebugUtil.LogFormat(ELogType.eNone, "{0} {1}", res.RoleId, PosConvertUtil.Svr2Client(res.PosX, res.PosY));
            if (hero != null)
            {
                hero.syncTransformComponent.SyncNetPos(PosConvertUtil.Svr2Client(res.PosX, res.PosY));
                hero.movementComponent.MoveTo(hero.syncTransformComponent.netPos);
            }
            else
            {
                //TODO:先去掉，服务器会发送非视野中的玩家移动的信息导致卡顿
                //DebugUtil.LogErrorFormat("not found hero roleId={0}", res.RoleId);
            }
        }

        public void ReqChangeMap(uint poterId, bool ui, uint posX = 0, uint posY = 0)
        {
            GameCenter.EnableMainHeroMove(false);
            IsTelState = true;

            CmdMapRoleChgMapReq req = new CmdMapRoleChgMapReq();
            req.Teleporter = poterId;
            req.BUi = ui;
            req.PosX = posX * 100;
            req.PosY = posY * 100;
            NetClient.Instance.SendMessage((ushort)CmdMap.RoleChgMapReq, req);

            timerTelCD?.Cancel();
            timerTelCD = Timer.Register(2f, () => { GameCenter.EnableMainHeroMove(true); });
        }

        public void ReqTelNpc(uint npcId, uint mapId, uint taskId = 0)
        {
            reqTelNpc.NpcId = npcId;

            if (UIManager.IsVisibleAndOpen(EUIID.UI_Menu))
            {
                IsTransTipOver = false;

                UIManager.CloseUI(EUIID.UI_PathFind);

                //TODO 预加载
                //if (mapId != CurMapId)
                //{
                //    LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
                //    if (lvPlay != null)
                //    {
                //        lvPlay.mSeamlessScene.SetPreloadMap(mapId);
                //    }
                //}

                List<ActionBase> actionBases = new List<ActionBase>();
                TransmitTipAction tipAction = ActionCtrl.Instance.CreateAction(typeof(TransmitTipAction)) as TransmitTipAction;
                if (tipAction != null)
                {
                    tipAction.TransNpcId = npcId;
                    tipAction.taskId = taskId;
                    tipAction.Init();

                    actionBases.Add(tipAction);
                }

                ActionCtrl.Instance.AddAutoActions(actionBases);
            }
            else
            {
                ExcuteTelNpc();
            }
        }

        public void ExcuteTelNpc()
        {
            IsTelState = true;
            IsTransTipOver = true;
            GameCenter.EnableMainHeroMove(false);
            NetClient.Instance.SendMessage((ushort)CmdMap.NpcTelReq, reqTelNpc);
        }

        private void OnSameMapTel(NetMsg msg)
        {
            CmdMapSameMapTelNtf res = NetMsgUtil.Deserialize<CmdMapSameMapTelNtf>(CmdMapSameMapTelNtf.Parser, msg);
            if (res.RoleId == Sys_Role.Instance.Role.RoleId)
            {
                if (GameCenter.mainHero != null)
                {
                    GameCenter.mainHero.syncTransformComponent.SyncNetPos(PosConvertUtil.Svr2Client(res.PosX, res.PosY));
                    GameCenter.mainHero.movementComponent.TransformToPosImmediately(GameCenter.mainHero.syncTransformComponent.netPos);
                }

                GameCenter.EnableMainHeroMove(true);
                eventEmitter.Trigger(EEvents.OnHeroTel);
                eventEmitter.Trigger(EEvents.OnAutoPathFind);

                //同地图传送，打点
                if (IsTelState)
                {
                    IsTelState = false;
                    OnTelHitPoint(true);
                }

                eventEmitter.Trigger(EEvents.OnRoleSameMapTel);
            }
            else
            {
                //Actor actor = GameCenter.mainWorld.GetActor(Hero.Type, res.RoleId);
                Hero hero = GameCenter.GetSceneHero(res.RoleId);
                if (hero != null)
                {
                    hero.syncTransformComponent.SyncNetPos(PosConvertUtil.Svr2Client(res.PosX, res.PosY));
                    hero.movementComponent.TransformToPosImmediately(hero.syncTransformComponent.netPos);

                    if (Sys_Team.Instance.isTeamMem(res.RoleId) && hero.movementComponent != null)
                    {
                        hero.movementComponent.Stop();
                    }
                }
            }

            //   DebugUtil.LogError("地图传送： " + CurMapId.ToString());
        }

        private void OnEnterNtf(NetMsg msg)
        {
            CmdMapRoleEnterNtf res = NetMsgUtil.Deserialize<CmdMapRoleEnterNtf>(CmdMapRoleEnterNtf.Parser, msg);

            if (CurMapId != res.MapId)
                LastMapId = CurMapId;
            CurMapId = res.MapId;

            svrPosX = res.PosX;
            svrPosY = res.PosY;

            isLoadOk = false;

            // 首次进入该地图
            if (res.First)
            {
                this.firstEnterMaps.Add(res.MapId);

                var csvMap = CSVMapInfo.Instance.GetConfData(res.MapId);
                var csvMapBit = CSVMapBitsInfo.Instance.GetConfData(res.MapId);
                if (!csvMapBit.mapEnable && csvMap.PromptForMapUnlocking)
                {
                    this.unReadedMapMails.Add(res.MapId);

                    this.eventEmitter.Trigger(EEvents.OnMapBitChanged);
                }
            }

            eventEmitter.Trigger<uint, uint>(Sys_Map.EEvents.OnChangeMap, LastMapId, CurMapId);

            if (!IsEnterMap)
            {
                IsEnterMap = true;
                LevelManager.EnterLevel(typeof(LvPlay));

                //记录新手路径埋点
                Sys_Role.Instance.CheckNewTrace();
            }
            else
            {
                GameCenter.ChangeMap();

            }

            // DebugUtil.LogError("地图加载： " + CurMapId.ToString());
            Sys_Weather.Instance.eventEmitter.Trigger(Sys_Weather.EEvents.OnRemoveWeatherMap);
        }

        public void LoadOKReq()
        {
            isLoadOk = true;

            CmdMapLoadOKRpt req = new CmdMapLoadOKRpt();
            NetClient.Instance.SendMessage((ushort)CmdMap.LoadOkrpt, req);

            // 还原原始切地图的形式
            switchMode = ESwitchMapUI.UILoading2;

            //生成地图传送点
            GenTelPoint();
            // 生成地图阻挡墙
            // GetWalls();
            // 生成地图安全区
            // GetSafeAreas();

            //切地图传送成功打点
            if (IsTelState)
            {
                IsTelState = false;
                OnTelHitPoint();
            }

            eventEmitter.Trigger(EEvents.OnLoadOK);

            //DebugUtil.LogError("地图加载好了");
        }

        private void OnTeleErrNtf(NetMsg msg)
        {
            CmdMapTeleErrNtf res = NetMsgUtil.Deserialize<CmdMapTeleErrNtf>(CmdMapTeleErrNtf.Parser, msg);
            List<TeleInfo> teleErrInfo = new List<TeleInfo>();
            int len = res.RoleInfo.Count;
            for (int i = 0; i < len; i++)
            {
                TeleInfo tele = new TeleInfo
                {
                    roleName = res.RoleInfo[i].RoleName.ToStringUtf8(),
                    needLvl = res.RoleInfo[i].NeedLvl,
                    needTask = res.RoleInfo[i].NeedTask
                };
                teleErrInfo.Add(tele);
            }

            TeleErrMapId = res.MapId;
            eventEmitter.Trigger(EEvents.OnTeleErr);
            if (res.MapId == 1510)
            {
                //家族领地
                TeleErrFamilyName = res.GuildName.ToStringUtf8();
                UIManager.OpenUI(EUIID.UI_FamilyMapCondition_Tips, false, teleErrInfo);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_MapCondition_Tips, false, teleErrInfo);
            }
        }

        #endregion

        #region SDK HitPoint

        public void OnTelHitPoint(bool isSameMap = false)
        {
            HitPointTeleporter hitPoint = new HitPointTeleporter();

            uint bfSceneId = isSameMap ? CurMapId : LastMapId;

            hitPoint.bfscene_id = bfSceneId.ToString();
            hitPoint.scene_id = CurMapId.ToString();
            hitPoint.result = "1";
            hitPoint.online_duration = Framework.TimeManager.GetElapseTime().ToString();


            if (Sys_Task.Instance.currentTaskEntry != null)
            {
                if (Sys_Task.Instance.currentTaskEntry.csvTask != null)
                {
                    if (Sys_Task.Instance.currentTaskEntry.csvTask.taskCategory
                        == (int)ETaskCategory.Trunk)
                    {
                        hitPoint.main_task_id = Sys_Task.Instance.currentTaskEntry.csvTask.id.ToString();
                    }
                    else
                    {
                        hitPoint.branch_task_id = Sys_Task.Instance.currentTaskEntry.csvTask.id.ToString();
                    }
                }
            }

            HitPointManager.HitPoint(HitPointTeleporter.Key, hitPoint);
        }

        #endregion

        private uint MapId = 0;
        Vector2 vector2 = Vector2.zero;
        Vector2 offect = Vector2.zero;
        Vector2 mapSize = Vector2.zero;
        Vector2 uiRatio = Vector2.zero;
        Vector2 position = Vector2.zero;

        /// <summary>
        /// 得到玩家坐标对于地图图片坐标(仅仅用打点)
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPointInTexture()
        {
            if (MapId != CurMapId)
            {
                MapId = CurMapId;
                vector2 = Vector2.zero;
                offect = Vector2.zero;
                mapSize = Vector2.zero;
                uiRatio = Vector2.zero;
                position = Vector2.zero;
                CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(CurMapId);
                if (null == cSVMapInfoData)
                {
                    return vector2;
                }

                if (null != cSVMapInfoData.ui_pos && cSVMapInfoData.ui_pos.Count >= 2)
                {
                    offect.x = cSVMapInfoData.ui_pos[0];
                    offect.y = cSVMapInfoData.ui_pos[1];
                }

                if (null != cSVMapInfoData.map_size && cSVMapInfoData.map_size.Count >= 2)
                {
                    mapSize.x = cSVMapInfoData.map_size[0];
                    mapSize.y = cSVMapInfoData.map_size[1];
                }

                //UI对应场景标准比例
                uiRatio.x = cSVMapInfoData.ui_length;
                uiRatio.y = cSVMapInfoData.ui_width;

                //GameCenter.mainHero.transform未赋值采用记录的服务器坐标
                position.x = Sys_Map.Instance.svrPosX / 100f;
                position.y = -Sys_Map.Instance.svrPosY / 100f; //服务器坐标
            }
            else
            {
                if (isLoadOk && null != GameCenter.mainHero && null != GameCenter.mainHero.transform)
                {
                    var pos = GameCenter.mainHero.transform.position;
                    position.x = pos.x;
                    position.y = pos.z; //服务器坐标
                }
                else
                {
                    position.x = Sys_Map.Instance.svrPosX / 100f;
                    position.y = -Sys_Map.Instance.svrPosY / 100f; //服务器坐标
                }
            }

            //客户端坐标系左上(0,0)
            position += offect;
            //服务器坐标系左下(0,0)
            vector2.x = uiRatio.x == 0 ? 0 : position.x / uiRatio.x * mapSize.x;
            vector2.y = uiRatio.y == 0 ? 0 : (1 - position.y / uiRatio.y) * mapSize.y;
            return vector2;
        }

        #region 进入地图的提示

        public HashSet<uint> unReadedMapMails = new HashSet<uint>();
        public HashSet<uint> unReadedMapResources = new HashSet<uint>();
        public HashSet<uint> firstEnterMaps = new HashSet<uint>();

        // 全量标记数据
        // 登陆，首次进入，以及读取的时候，需要下发
        private void OnReceiveMapBits(NetMsg msg)
        {
            CmdMapRoleMapInfoNtf ntf = NetMsgUtil.Deserialize<CmdMapRoleMapInfoNtf>(CmdMapRoleMapInfoNtf.Parser, msg);

            bool needTrigger = false;
            int bitSize = sizeof(byte) * 8;

            this.unReadedMapMails.Clear();
            if (ntf.EnterMaps != null)
            {
                for (int i = 0, length = ntf.EnterMaps.Length; i < length; ++i)
                {
                    var oneByte = ntf.EnterMaps[i];
                    for (int j = 0; j < bitSize; ++j)
                    {
                        var shift = (byte)(1 << j);
                        if ((oneByte & shift) == shift)
                        {
                            int index = i * bitSize + j + 1;

                            var mapBitsInfos = CSVMapBitsInfo.Instance.GetAll();
                            for (int i1 = 0, len = mapBitsInfos.Count; i1 < len; i1++)
                            {
                                var csv = mapBitsInfos[i1];
                                var csvMap = CSVMapInfo.Instance.GetConfData(csv.id);
                                if (!csv.mapEnable && csvMap.PromptForMapUnlocking && csv.mapBits == index)
                                {
                                    this.unReadedMapMails.Add(csv.id);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (ntf.ReadLetters != null)
            {
                for (int i = 0, length = ntf.ReadLetters.Length; i < length; ++i)
                {
                    var oneByte = ntf.ReadLetters[i];
                    for (int j = 0; j < bitSize; ++j)
                    {
                        var shift = (byte)(1 << j);
                        if ((oneByte & shift) == shift)
                        {
                            int index = i * bitSize + j + 1;

                            var mapBitsInfos = CSVMapBitsInfo.Instance.GetAll();
                            for (int i1 = 0, len = mapBitsInfos.Count; i1 < len; i1++)
                            {
                                var csv = mapBitsInfos[i1];
                                if (!csv.mapEnable && csv.mapBits == index)
                                {
                                    this.unReadedMapMails.Remove(csv.id);
                                    break;
                                }
                            }
                        }
                    }
                }

                needTrigger = true;
            }

            this.unReadedMapResources.Clear();

            var mapBitsInfoDatas = CSVMapBitsInfo.Instance.GetAll();
            for (int i1 = 0, len = mapBitsInfoDatas.Count; i1 < len; i1++)
            {
                var csv = mapBitsInfoDatas[i1];
                if (!csv.mapEnable)
                {
                    this.unReadedMapResources.Add(csv.id);
                }
            }
            if (ntf.ResourcePages != null)
            {
                for (int i = 0, length = ntf.ResourcePages.Length; i < length; ++i)
                {
                    var oneByte = ntf.ResourcePages[i];
                    for (int j = 0; j < bitSize; ++j)
                    {
                        var shift = (byte)(1 << j);
                        if ((oneByte & shift) == shift)
                        {
                            int index = i * bitSize + j + 1;

                            var mapBitsInfos = CSVMapBitsInfo.Instance.GetAll();
                            for (int i1 = 0, len = mapBitsInfos.Count; i1 < len; i1++)
                            {
                                var csv = mapBitsInfos[i1];
                                if (!csv.mapEnable && csv.mapBits == index)
                                {
                                    this.unReadedMapResources.Remove(csv.id);
                                    break;
                                }
                            }
                        }
                    }
                }

                needTrigger = true;
            }

            this.firstEnterMaps.Clear();

            if (needTrigger)
            {
                this.eventEmitter.Trigger(EEvents.OnMapBitChanged);
            }
        }

        public void ReqReadMapMail(uint mapId)
        {
            CmdMapReadMapLetterReq req = new CmdMapReadMapLetterReq();
            req.MapId = mapId;
            NetClient.Instance.SendMessage((ushort)CmdMap.ReadMapLetterReq, req);

            // 客户端立即记录，不等待回包
            this.OnReadMapMail(mapId);
        }

        public void ReqReadMapResource(uint mapId)
        {
            CmdMapChangeResourcePageReq req = new CmdMapChangeResourcePageReq();
            req.MapId = mapId;
            NetClient.Instance.SendMessage((ushort)CmdMap.ChangeResourcePageReq, req);

            // 客户端立即记录，不等待回包
            this.OnReadMapResource(mapId);
        }
        private void OnRoleReNameUpdate(NetMsg msg)
        {
            CmdMapRoleRenameNtf ntf = NetMsgUtil.Deserialize<CmdMapRoleRenameNtf>(CmdMapRoleRenameNtf.Parser, msg);
            ActorHUDNameUpdateEvt evt = new ActorHUDNameUpdateEvt();
            evt.id = ntf.RoleId;
            evt.eFightOutActorType = EFightOutActorType.OtherHero;
            evt.name = ntf.NewName.ToStringUtf8();
            Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorName, evt);
        }

        void OnUpdateRoleBgroupInfoNtf(NetMsg msg)
        {
            CmdMapUpdateRoleBGroupInfoNtf ntf = NetMsgUtil.Deserialize<CmdMapUpdateRoleBGroupInfoNtf>(CmdMapUpdateRoleBGroupInfoNtf.Parser, msg);
            if (ntf != null)
            {
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    if (ntf.RoleId == Sys_Role.Instance.RoleId)
                    {
                        Sys_Title.Instance.UpdateBGroupTitle();
                    }

                    if (hero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle) 
                    {
                        UpdateBGroupTitleEvt evt = new UpdateBGroupTitleEvt();
                        evt.actorId = ntf.RoleId;
                        evt.titleId = Sys_Title.Instance.bGroupTitle;
                        evt.name = ntf.Info.Name.ToStringUtf8();
                        evt.pos = ntf.Info.Pos;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBGroupTitleName, evt);
                    }
                }
            }
        }

        void OnUpdateRoleBgroupQuitNtf(NetMsg msg)
        {
            CmdMapUpdateRoleBGroupQuitNtf ntf = NetMsgUtil.Deserialize<CmdMapUpdateRoleBGroupQuitNtf>(CmdMapUpdateRoleBGroupQuitNtf.Parser, msg);
            if (ntf != null)
            {
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null) 
                {
                    if (ntf.RoleId == Sys_Role.Instance.RoleId) 
                    {
                        Sys_Title.Instance.RemoveBGroupTitle();
                    }

                    if (hero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle)
                    {
                        ClearTitleEvt evt=new ClearTitleEvt();
                        evt.actorId = ntf.RoleId;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearTitle, evt);
                    }
                }
            }
        }

        void OnUpdateRoleBgroupPosNtf(NetMsg msg)
        {
            CmdMapUpdateRoleBGroupPosNtf ntf = NetMsgUtil.Deserialize<CmdMapUpdateRoleBGroupPosNtf>(CmdMapUpdateRoleBGroupPosNtf.Parser, msg);
            if (ntf != null)
            {
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    if (ntf.RoleId == Sys_Role.Instance.RoleId)
                    {
                        Sys_Title.Instance.UpdateBGroupTitle();
                    }

                    if (hero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle) 
                    {
                        UpdateBGroupTitleEvt evt = new UpdateBGroupTitleEvt();
                        evt.actorId = ntf.RoleId;
                        evt.titleId = Sys_Title.Instance.bGroupTitle;
                        evt.name = hero.heroBaseComponent.bGroupName;
                        evt.pos = ntf.BgroupPos;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBGroupTitleName, evt);
                    }
                }
            }
        }

        void OnUpdateRoleBgroupNameNtf(NetMsg msg)
        {
            CmdMapUpdateRoleBGroupNameNtf ntf = NetMsgUtil.Deserialize<CmdMapUpdateRoleBGroupNameNtf>(CmdMapUpdateRoleBGroupNameNtf.Parser, msg);
            if (ntf != null)
            {
                Hero hero = GameCenter.GetSceneHero(ntf.RoleId);
                if (hero != null)
                {
                    if (ntf.RoleId == Sys_Role.Instance.RoleId)
                    {
                        Sys_Title.Instance.UpdateBGroupTitle();
                    }

                    if (hero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle) 
                    {
                        UpdateBGroupTitleEvt evt = new UpdateBGroupTitleEvt();
                        evt.actorId = ntf.RoleId;
                        evt.titleId = Sys_Title.Instance.bGroupTitle;
                        evt.name = hero.heroBaseComponent.bGroupName;
                        evt.pos = hero.heroBaseComponent.Pos;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBGroupTitleName, evt);
                    }
                }
            }
        }

        // 主角的buff存储，场景其他玩家的buff没有存储，只是有UI表现
        public Dictionary<uint, RoleMapBuffUnit> mainHeroBuffList =  new Dictionary<uint, RoleMapBuffUnit>();

        private void OnCmdMapDataNty(NetMsg netMsg) {
            mainHeroBuffList.Clear();
            CmdMapDataNty ntf = NetMsgUtil.Deserialize<CmdMapDataNty>(CmdMapDataNty.Parser, netMsg);
            if (ntf.BuffData != null && ntf.BuffData.Bufflist != null) {
                for (int i = 0, length = ntf.BuffData.Bufflist.Count; i < length; ++i) {
                    var one = ntf.BuffData.Bufflist[i];
                    mainHeroBuffList.Add(one.Buffid, one);
                }
            }
        }

        private void OnCmdMapAddRoleBuffNty(NetMsg netMsg) {
            CmdMapAddRoleBuffNty ntf = NetMsgUtil.Deserialize<CmdMapAddRoleBuffNty>(CmdMapAddRoleBuffNty.Parser, netMsg);
            if (!mainHeroBuffList.TryGetValue(ntf.Buff.Buffid, out var buff)) {
                buff = ntf.Buff;
                mainHeroBuffList.Add(ntf.Buff.Buffid, buff);
            }
            else {
                mainHeroBuffList[ntf.Buff.Buffid] = ntf.Buff;
            }

            var transfer = new FamilyResBattleComponent.Transfer1(Sys_Role.Instance.RoleId, ntf.Buff);
            this.eventEmitter.Trigger<FamilyResBattleComponent.Transfer1>(EEvents.OnProtectBuffAdd,  transfer);
        }
        
        private void OnCmdMapRemoveRoleBuffNty(NetMsg netMsg) {
            CmdMapRemoveRoleBuffNty ntf = NetMsgUtil.Deserialize<CmdMapRemoveRoleBuffNty>(CmdMapRemoveRoleBuffNty.Parser, netMsg);
            for (int i = 0, length = ntf.Buffids.Count; i < length; ++i) {
                var one = ntf.Buffids[i];
                mainHeroBuffList.Remove(one);
            }

            var transfer = new FamilyResBattleComponent.Transfer2(Sys_Role.Instance.RoleId, ntf.Buffids);
            this.eventEmitter.Trigger<FamilyResBattleComponent.Transfer2>(EEvents.OnProtectBuffRemove, transfer);
        }

        private void OnAddOtherRoleBuffNty(NetMsg netMsg) {
            CmdMapAddOtherRoleBuffNty ntf = NetMsgUtil.Deserialize<CmdMapAddOtherRoleBuffNty>(CmdMapAddOtherRoleBuffNty.Parser, netMsg);
            
            var transfer = new FamilyResBattleComponent.Transfer1(ntf.Roleid, ntf.Buff);
            this.eventEmitter.Trigger<FamilyResBattleComponent.Transfer1>(EEvents.OnProtectBuffAdd, transfer);
        }
        
        private void OnRemoveOtherRoleBuffNty(NetMsg netMsg) {
            CmdMapRemoveOtherRoleBuffNty ntf = NetMsgUtil.Deserialize<CmdMapRemoveOtherRoleBuffNty>(CmdMapRemoveOtherRoleBuffNty.Parser, netMsg);
            
            var transfer = new FamilyResBattleComponent.Transfer2(ntf.Roleid, ntf.Buffids);
            this.eventEmitter.Trigger<FamilyResBattleComponent.Transfer2>(EEvents.OnProtectBuffRemove, transfer);
        }
        private void OnRoleReturnNtf(NetMsg netMsg)
        {
            CmdMapRoleReturn ntf= NetMsgUtil.Deserialize<CmdMapRoleReturn>(CmdMapRoleReturn.Parser, netMsg);
            if (ntf != null)
            {
                Hero hero = GameCenter.GetSceneHero(ntf.Roleid);
                if (hero != null)
                {
                    ActorHUDNameUpdateEvt evt = new ActorHUDNameUpdateEvt();
                    evt.id = ntf.Roleid;
                    evt.eFightOutActorType = EFightOutActorType.OtherHero;
                    evt.name = hero.heroBaseComponent.Name;
                    evt.upBack = ntf.IsReturn;
                    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorName, evt);
                }
            }
        }
        private void OnFirstEnterMap(uint mapId)
        {
            this.eventEmitter.Trigger<uint>(EEvents.OnFirstEnterMap, mapId);

            // UIManager.CloseUI(EUIID.UI_MapFirstEnter, true);
            // UIManager.UpdateState();
            // UIManager.OpenUI(EUIID.UI_MapFirstEnter, false, mapId);
            // UIManager.UpdateState();
        }

        private void OnReadMapMail(uint mapId)
        {
            this.unReadedMapMails.Remove(mapId);
            this.eventEmitter.Trigger<uint>(EEvents.OnReadMapMail, mapId);
        }

        private void OnReadMapResource(uint mapId)
        {
            this.unReadedMapResources.Remove(mapId);
            this.eventEmitter.Trigger<uint>(EEvents.OnReadMapResource, mapId);
        }

        #endregion
    }
}


