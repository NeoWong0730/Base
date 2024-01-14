using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Google.Protobuf;
using System;
using Table;

namespace Logic
{
    partial class Sys_Role_Info : SystemModuleBase<Sys_Role_Info>
    {

        public enum EEvents
        {
            NetMsg_RoleInfo,
            NetMsg_RoleInfo_Open,
            OnClickSelect,
            FightTaget,
            NetMsg_RecvRoleAck,
        }

        /// <summary>
        /// 组队消息委托
        /// </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private RaycastHit[] m_RaycastHit = new RaycastHit[5];

        private ELayerMask SelectLayer = ELayerMask.OtherActor;

        private ELayerMask HitRayLayer = ELayerMask.NPC | ELayerMask.OtherActor | ELayerMask.Player;

        private List<Hero> m_SelectRole = new List<Hero>(5);
        public List<Hero> SelectRole { get { return m_SelectRole; } }

        private string m_CurApplyName = string.Empty;

       // private EType m_CurType = EType.Avatar;

        private List<InfoItem> mInfoItems = new List<InfoItem>();

        public List<InfoItem> InfoItemList { get { return mInfoItems; } }

        public bool FightTaget { get; set; } = false;
        public override void Init()
        {
            // Sys_Interactive.Instance.eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.LongPress, OnClick, true);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Sys_Input.Instance.onTouchRightUp += onTouchUp;
#endif
            Sys_Input.Instance.onTouchUp += onTouchUp;

            Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
        }
        public override void Dispose()
        {
            base.Dispose();

            //  Sys_Interactive.Instance.eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.LongPress, OnClick, false);
        }

        /// <summary>
        /// 向服务器请求人物信息
        /// </summary>
        /// <param name="roleName"></param>
        private void Send_Message_RoleInfo( ulong roleId )
        {
            CmdSocialGetBriefInfoReq info = new CmdSocialGetBriefInfoReq();

            info.RoleId = roleId;
            info.Name = ByteString.CopyFromUtf8(string.Empty);
            info.Classify = (uint)EGetBriefInfoClassify.Sys_Role_Info;
            NetClient.Instance.SendMessage((ushort)CmdSocial.GetBriefInfoReq, info);
        }


        private void OnEnterBattle(CSVBattleType.Data cSVBattleType)
        {
            CloseRoleInfo();
        }
        /// <summary>
        /// 相应点击回调
        /// </summary>
        /// <param name="pos"></param>
        private void onTouchUp(Vector2 pos)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
            {
                m_SelectRole.Clear();
                OpenRoleInfo(SelectRole);
                return;
            }
                
            OnTouchBigWorld(pos);
        }

        /// <summary>
        /// 处理点击消息，检测点中Avatar
        /// </summary>
        /// <param name="pos"></param>
        private void OnTouchBigWorld(Vector2 pos)
        {
            m_SelectRole.Clear();

            if (CameraManager.mCamera != null)
            {
                Ray ray = CameraManager.mCamera.ScreenPointToRay(pos);

                int layerMask = (int)(HitRayLayer);

                int count = Physics.RaycastNonAlloc(ray, m_RaycastHit, 500f, layerMask);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        RaycastHit hit = m_RaycastHit[i];

                        if (LayerMaskUtil.ContainLayerInt(ELayerMask.NPC, hit.collider.gameObject.layer))
                        {
                            m_SelectRole.Clear();
                            break;
                        }
                        if (LayerMaskUtil.ContainLayerInt(SelectLayer, hit.collider.gameObject.layer))
                        {
                            SceneActorWrap actorWrap = hit.collider.gameObject.GetComponent<SceneActorWrap>();

                            if (actorWrap != null && actorWrap.sceneActor != null && actorWrap.sceneActor is Hero)
                            {
                                Hero hero = actorWrap.sceneActor as Hero;
                                m_SelectRole.Add(hero);
                            }
                        }
                    }
                }
            }

            OpenRoleInfo(SelectRole);
        }

        private void OnTouchFighting(Vector2 pos)
        {
            if (CameraManager.mCamera != null)
            {
                Ray ray = CameraManager.mCamera.ScreenPointToRay(pos);

                int layerMask = (int)(HitRayLayer);

                RaycastHit hit;
                bool result = Physics.Raycast(ray, out hit, 500f, layerMask);

                if (result)
                {

                    SceneActorWrap actorWrap = hit.collider.gameObject.GetComponent<SceneActorWrap>();

                    if (actorWrap != null && actorWrap.sceneActor != null && actorWrap.sceneActor is Hero)
                    {
                        Hero hero = actorWrap.sceneActor as Hero;

                    }


                }
            }
        }

        /// <summary>
        /// 战斗中点击Avatar 弹出战斗指令菜单
        /// </summary>
        /// <param name="data"></param>
        private void OnClick(InteractiveEvtData data)
        {
            if (data == null)
                return;

            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight)
                return;

            if (Sys_Team.Instance.CanTakeWarCommandSign() == false)
                return;

            int result = ((int)(ELayerMask.Monster | ELayerMask.Partner)) & (1 << data.sceneActor.gameObject.layer);

            if (result == 0)
                return;


            UIManager.OpenUI(EUIID.UI_FrightingClick, false, data.sceneActor);
        }


        public bool CanMakeFightCommand()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight)
                return false;

            if (Sys_Team.Instance.CanTakeWarCommandSign() == false)
                return false;

            if (Sys_FunctionOpen.Instance.IsOpen(60111, false) == false)
                return false;
            return true;
        }

        public override void OnLogout()
        {
            base.OnLogout();

            m_SelectRole.Clear();
        }
        public Hero getSelectHero(int index)
        {
            if (index >= m_SelectRole.Count)
                return null;

            return m_SelectRole[index];
        }


        /// <summary>
        /// 点中Avatar ，打开人物信息界面
        /// </summary>
        /// <param name="heroes"></param>
        private void OpenRoleInfo(List<Hero> heroes) {
            if (heroes.Count == 1) {
                if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
                    OpenRoleInfo(heroes[0].UID, EType.Avatar);
                }
                else {
                    var hero = heroes[0];
                    bool isRed = hero.familyResBattleComponent.isRed;
                    if (isRed) {
                        OpenRoleInfo(hero.UID, EType.Avatar);
                    }
                    else {
                        Sys_FamilyResBattle.Instance.ReqAttack(hero, hero.uID, Sys_Map.Instance.CurMapId);
                    }
                }

                return;
            }

            if (heroes.Count > 0) {
                if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
                    if (!UIManager.IsOpen(EUIID.UI_Team_Player)) {
                        mInfoParmas.Clear();
                        mInfoParmas.eType = EType.Avatar;
                        mInfoParmas.mHeroes.AddRange(heroes);

                        UIManager.OpenUI(EUIID.UI_Team_Player, false, mInfoParmas);
                    }
                }
                else {
                    if (!UIManager.IsOpen(EUIID.UI_FamilyResBattleActorList)) {
                        mInfoParmas.Clear();
                        mInfoParmas.eType = EType.Avatar;

                        for (int i = 0, length = heroes.Count; i < length; ++i) {
                            var h = heroes[i].heroBaseComponent;
                            // if (h.IsCaptain || h.TeamID == 0) 
                            {
                                mInfoParmas.mHeroes.Add(heroes[i]);
                            }
                        }

                        UIManager.OpenUI(EUIID.UI_FamilyResBattleActorList, false, mInfoParmas);
                    }
                }

                return;
            }

            mInfoParmas.Clear();
            mInfoParmas.eType = EType.Avatar;
            mInfoParmas.mHeroes.AddRange(heroes);

            eventEmitter.Trigger<InfoParmas>(Sys_Role_Info.EEvents.OnClickSelect, mInfoParmas);
        }


        /// <summary>
        /// 服务器返回的人物信息
        /// </summary>
        /// <param name="info"></param>
        public void PushRoleInfo(CmdSocialGetBriefInfoAck info)
        {


            var msg = PopMsg(info.RoleId);

            if (msg == null)
            {
                eventEmitter.Trigger<CmdSocialGetBriefInfoAck>(Sys_Role_Info.EEvents.NetMsg_RoleInfo, info);

                return;
            }

            mInfoParmas.Clear();
            mInfoParmas.eType = msg.mType;
            mInfoParmas.mInfo = info;
            mInfoParmas.mCustom.AddRange(msg.mInfoItems);

            eventEmitter.Trigger(Sys_Role_Info.EEvents.NetMsg_RecvRoleAck);
            if (UIManager.IsOpen(EUIID.UI_Team_Player) == false)
            {
                UIManager.OpenUI(EUIID.UI_Team_Player, false, mInfoParmas);
                return;
            }

            eventEmitter.Trigger<InfoParmas>(Sys_Role_Info.EEvents.NetMsg_RoleInfo_Open, mInfoParmas);


        }

        /// <summary>
        /// 打开任务交互面板
        /// </summary>
        /// <param name="roleName">名称，用于服务器查询人物信息</param>
        /// <param name="eType">类型，决定面板的可交互的内容</param>
        public void OpenRoleInfo(ulong roleid, EType eType, List<InfoItem> infoItems = null)
        {
            if (roleid == 0)
                return;

            Send_Message_RoleInfo(roleid);

            AddMsg(roleid, eType, true, infoItems);
        }

        /// <summary>
        /// 关闭交互界面
        /// </summary>
        public void CloseRoleInfo()
        {
            // if (UIManager.IsOpen(EUIID.UI_Team_Player))

            UIManager.CloseUI(EUIID.UI_Team_Player);

            m_CurApplyName = string.Empty;
        }


    }

    partial class Sys_Role_Info : SystemModuleBase<Sys_Role_Info>
    {
        /// <summary>
        /// 交互类型
        /// </summary>
        public enum EType
        {
            None = 0,//用于其他系统自定义
            Avatar = 1,
            Chat = 2,
            Friend = 3,
            FromAvatar = 4, //多个Avatar中一个从服务器返回信息
            Family = 5,
            MsgBag = 6,
            UIFriend = 7,//好友界面头像点击
        }

        /// <summary>
        /// 交互界面自定义
        /// </summary>
        public class InfoItem
        {
            public string mName; //为空时 忽略不显示
            public bool mOnClickClose = true; //点击关闭
            public bool isNeedShow = true;    //是否满足显示要求
            public Action<DoRoleInfo> mClickAc; // 回调函数为空时 显示为灰态
        }

        /// <summary>
        /// 交互界面参数
        /// </summary>
        public class InfoParmas
        {
            public EType eType = EType.Avatar;
            public List<Hero> mHeroes = new List<Hero>();
            public List<InfoItem> mCustom = new List<InfoItem>();

            public CmdSocialGetBriefInfoAck mInfo = null;
            public void Clear()
            {
                eType = EType.Avatar;
                mHeroes.Clear();
                mCustom.Clear();

                mInfo = null;
            }

            public void Copy(InfoParmas value)
            {
                if (value == null)
                    return;

                Clear();

                eType = value.eType;
                mHeroes.AddRange(value.mHeroes);
                mCustom.AddRange(value.mCustom);

                mInfo = value.mInfo;
            }
        }

        /// <summary>
        /// 消息请求
        /// </summary>
        private class InfoMsg
        {
            public ulong roleid;
          
            public EType mType;
            public List<InfoItem> mInfoItems = new List<InfoItem>();

            public void Clear()
            {
                roleid = 0;
                if (mInfoItems != null)
                    mInfoItems.Clear();
            }


        }

        private InfoParmas mInfoParmas = new InfoParmas();

        private List<InfoMsg> mMsgPools = new List<InfoMsg>();

        private int mMsgMaxCount = 1;

       

        /// <summary>
        /// 过去人物在消息队列中的下标
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private int getInMsgPoolIndex(ulong roleid)
        {
            int index = mMsgPools.FindIndex(o => { return o.roleid == roleid; });

            return index;
        }


        private InfoMsg PopMsg(ulong roleid)
        {
            int index = getInMsgPoolIndex(roleid);

            if (index < 0)
                return null;

            var msg = mMsgPools[index];

            mMsgPools.RemoveAt(index);

            return msg;
        }
        /// <summary>
        /// 将人物加入消息队列中，当队列达到最大容量时，将去除头部的消息
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="eType"></param>
        /// <param name="isRefresh">如果为真，已存在与队列中则会替换</param>
        /// <param name="infoItems"></param>
        private void AddMsg(ulong roleid, EType eType, bool isRefresh = false, List<InfoItem> infoItems = null)
        {
            int index = getInMsgPoolIndex(roleid);

            if (index >= 0)
            {
                if (isRefresh)
                    RefreshMsg(roleid, eType, infoItems);

                return;
            }


            InfoMsg infoMsg = new InfoMsg();

            infoMsg.roleid = roleid;
            infoMsg.mType = eType;

            if (infoItems != null && infoItems.Count > 0)
                infoMsg.mInfoItems.AddRange(infoItems);


            if (mMsgPools.Count > mMsgMaxCount - 1)
                mMsgPools.RemoveRange(0, (mMsgPools.Count - (mMsgMaxCount - 1)));

            mMsgPools.Add(infoMsg);
        }

        /// <summary>
        /// 刷新已经在队列中的消息
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="eType"></param>
        /// <param name="infoItems"></param>
        private void RefreshMsg(ulong roleid, EType eType, List<InfoItem> infoItems = null)
        {
            int index = getInMsgPoolIndex(roleid);

            if (index < 0)
                return;

            InfoMsg infoMsg = new InfoMsg();

            infoMsg.roleid = roleid;
            infoMsg.mType = eType;

            if (infoItems != null && infoItems.Count > 0)
                infoMsg.mInfoItems.AddRange(infoItems);

            mMsgPools[index].Clear();

            mMsgPools[index] = infoMsg;

        }
    }


    partial class Sys_Role_Info : SystemModuleBase<Sys_Role_Info>
    {
        #region  信息处理 本地 来自网络
        public interface DoRoleInfo
        {
            ulong getRoleID();
            string getRoleName();
            uint getRoleOccule();
            uint getRoleOcculeRank();
            string getGuildName();
            ulong getGuildID();
            uint getLevel();

            uint getHeroID();

            ulong getTeamID();

            uint getHeadId();

            uint getHeadFrameId();

        }

        public class LocalRole : DoRoleInfo
        {
            public Hero hero { get; set; }

            public string getGuildName()
            {
                if (hero == null)
                    return string.Empty;

                return string.Empty;
            }
            public ulong getGuildID()
            {
                return 0;
            }

            public uint getHeadId()
            {
                if (hero == null || hero.heroBaseComponent == null)
                    return 0;

                return CharacterHelper.getHeadID(hero.heroBaseComponent.HeroID, hero.heroBaseComponent.HeadId);
            }

            public uint getHeadFrameId()
            {
                if (hero == null || hero.heroBaseComponent == null)
                    return 0;

                return CharacterHelper.getHeadFrameID(hero.heroBaseComponent.HeadFrameId);
            }

            public uint getLevel()
            {
                return 0;
            }

            public ulong getRoleID()
            {
                if (hero == null || hero.heroBaseComponent == null)
                    return 0;

                return hero.uID;
            }

            public string getRoleName()
            {
                if (hero == null || hero.heroBaseComponent == null)
                    return string.Empty;

                return hero.heroBaseComponent.Name;
            }

            public uint getRoleOccule()
            {
                if (hero == null || hero.careerComponent == null)
                    return 0;

                return (uint)hero.careerComponent.CurCarrerType;
            }

            public uint getRoleOcculeRank()
            {
                return 0;
            }


            public ulong getTeamID()
            {
                return 0;
            }

            public uint getHeroID()
            {
                if (hero == null || hero.heroBaseComponent == null)
                    return 0;

                return hero.heroBaseComponent.HeroID;
            }

        }

        public class fromServer : DoRoleInfo
        {
            public CmdSocialGetBriefInfoAck hero { get; set; }

            public string getGuildName()
            {
                if (hero == null)
                    return string.Empty;

                return hero.GuildName.ToStringUtf8();
            }

            public ulong getGuildID()
            {
                if(hero == null)
                    return 0;

                return hero.GuildId;
            }

            public uint getHeadId()
            {
                if (hero == null)
                    return 0;

                return CharacterHelper.getHeadID(hero.HeroId, hero.RoleHead);
            }

            public uint getHeadFrameId()
            {
                if (hero == null)
                    return 0;

                return CharacterHelper.getHeadFrameID(hero.RoleHeadFrame);
            }

            public uint getLevel()
            {
                if (hero == null)
                    return 0;

                return hero.Level;
            }

            public ulong getRoleID()
            {
                if (hero == null)
                    return 0;

                return hero.RoleId;
            }

            public string getRoleName()
            {
                if (hero == null)
                    return string.Empty;

                return hero.Name.ToStringUtf8();
            }

            public uint getRoleOccule()
            {
                return hero.Occ;
            }

            public uint getRoleOcculeRank()
            {
                return hero.CareerRank;
            }

            public ulong getTeamID()
            {
                return hero.TeamId;
            }

            public uint getHeroID()
            {
                return hero.HeroId;
            }
        }

        #endregion
    }
}
