using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;
using System.Linq;
using Google.Protobuf.Collections;

namespace Logic
{
    /// <summary>
    /// 一级属性
    /// </summary>
    public enum EBaseAttr
    {
        None,
        Vit = 5,            //体力
        VitPre = 6,        //体力百分比
        Snh = 7,            //力量
        SnhPre = 8,         //力量百分比
        Inten = 9,          //强度
        IntenPre = 10,       //强度百分比
        Speed = 11,          //速度
        SpeedPre = 12,     //速度百分比
        Magic = 13,          //魔法   
        MagicPre = 14,       //魔法百分比
        SurplusPoint = -1,        //未分配的属性点数
        VitAssign = -2,          //已分配的体力
        SnhAssign = -3,         //已分配的力量
        IntenAssign = -4,      //已分配的强度
        SpeedAssign = -5,     //已分配的速度
        MagicAssign = -6,     //已分配的魔法
    }

    /// <summary>
    /// 二级属性
    /// </summary>
    public enum EPkAttr
    {
        None = 0,
        CurHp = -1,         //当前生命
        CurMp = -2,         //当前魔力
        Wind = 1,           //风属性
        Land = 2,           //地属性
        Water = 3,          //水属性
        Fire = 4,           //火属性
        MaxHp = 15,           //生命
        MaxHpPer = 16,         //生命百分比
        MaxMp = 17,            // 魔力
        MaxMpPer = 18,      //魔力百分比
        Atk = 19,                //攻击
        AtkPer = 20,          //攻击百分比
        Def = 21,               //防御
        DefPre = 22,         //防御百分比
        Agi = 23,             //敏捷
        AgiPre = 24,       //敏捷百分比
        Mgk = 25,           //魔攻
        MgkPre = 26,       //魔攻百分比
        Mnd = 27,           //精神
        MndPre = 28,      //精神百分比
        Rehp = 29,            //回复
        RehpPre = 30,        //回复百分比
        Rgs = 31,              //抗魔
        RgsPre = 32,         //抗磨百分比
        Charm = 33,           //魅力
        CharmPre = 34,      //魅力百分比
        MgcInc = 35,        //魔法增伤
        AdRed = 36,        //物理减伤
        AdInc = 37,          //物理增伤
        HitPer = 38,           //命中百分比
        AvdPer = 39,          //闪躲百分比
        CriPer = 40,            //必杀百分比
        ResiCriPer = 41,          //抵抗必杀概率
        CriDamage = 42,         //必杀伤害加成
        ResiCriDamage = 43,      //必杀伤害减免
        BeatBack = 44,             //反击概率
        BeatBackInc = 45,        //反击伤害加成
        ResiPoi = 46,          //抗中毒
        ResiDrunk = 47,     //抗酒醉
        ResiSleep = 48,     //抗昏睡
        ResiChaos = 49,     //抗混乱
        ResiPet = 50,        //抗石化
        ResiForget = 51,    //抗遗忘
        IgnorePoi = 52,     //中毒加深
        IgnoreDrunk = 53,   //酒醉加深
        IgnoreSleep = 54,   //忽视抗昏睡
        IgnoreChaos = 55,   //忽视抗混乱
        IgnorePet = 56,     //忽视抗石化
        IgnoreForget = 57,  //忽视抗遗忘
        WindInc = 58,       //风属性克制加深
        WindRed = 59,       //风属性被克减免
        LandInc = 60,       //地属性克制加深
        LandRed = 61,       //地属性被克减免
        WaterInc = 62,      //水属性克制加深
        WaterRed = 63,      //水属性被克减免
        FireInc = 64,          //火属性克制加深
        FireRed = 65,         //火属性被克减免
        JoinAtk = 66,         //合击概率
        IgnoreJoinAtk = 67,     //抵抗合击概率
        PetKnockUp = 68,       //宠物击飞概率
        IgnorePetKnockUp = 69,       //抵抗击飞概率
        AttkSpeedPer = 70,        //行动速度百分比
        RehpInc = 71,              //回复加成
        AttkSpeed = 72,          //行动速度
        ResiCtrl = 73,            //控制抗性
        IgnoreCtrl = 74,          //忽视控制
        Avd = 75,                   //闪避
        Hit = 76,                  //命中
        Cri = 77,                //必杀
        Beatbak = 78,            //反击
        WindInte = 79,       //强化风法
        LandInte = 80,         //强化地法
        WaterInte = 81,         //强化水法
        FireInte = 82,           //强化火法
        WindResi = 83,          //抗风
        LandResi = 84,          //抗地
        WaterResi = 85,        //抗水
        FireResi = 86,         //抗火
        HpDrain = 87,        //吸血比例
        HpDrainInc = 88,       //吸血增强
        CatchProb = 89,       //封印成功率
        PetExpInc = 90,       //宠物经验加成
        PetMpRed = 91,        //宠物耗蓝减少
        Flash=92,                  //闪现概率
        ElemInc = 93,          //元素压制
        ElemRed = 94,        //元素抵抗

        MoveSpeed = 101,    //移动速度
        RaceDragonInc = 102,               // 龙系克制增强
        RaceDragonRed = 103,               // 龙系被克减免
        RaceFlightInc = 104,               // 飞行系克制增强
        RaceFlightRed = 105,               // 飞行系被克减免
        RaceInsectInc = 106,               // 昆虫系克制增强
        RaceInsectRed = 107,               // 昆虫系被克减免
        RaceSpecialInc = 108,              // 特殊系克制增强
        RaceSpecialRed = 109,              // 特殊系被克减免
        RaceMetalInc = 110,                // 金属系克制增强
        RaceMetalRed = 111,                // 金属系被克减免
        RaceDeathlessInc = 112,            // 不死系克制增强
        RaceDeathlessRed = 113,            // 不死系被克减免
        RaceHumanInc = 114,                // 人形系克制增强
        RaceHumanRed = 115,                // 人形系被克减免
        RaceBeastInc = 116,                // 野兽系克制增强
        RaceBeastRed = 117,                // 野兽系被克减免
        RaceBotanyInc = 118,               // 植物系克制增强
        RaceBotanyRed = 119,               // 植物系被克减免
        RaceDemonInc = 120,                // 邪魔系克制增强
        RaceDemonRed = 121,                // 邪魔系被克减免
    }

    public enum ERoleViewType
    {
        None = 0,
        ViewAttr = 1,    //属性
        ViewAdd = 2,     //加点
        ViewAdvance = 3,     //进阶
        ViewProp = 4,         //调查
        ViewTitle = 5,       //称号
        ViewHpMp = 6,       //血蓝   
        Max = 7,
    }

    public enum EFriendInfoViewType
    {
        None = 0,
        ViewRoleAttr = 1,    //属性
        ViewPetInfo=2,
    }

    public partial class Sys_Attr : SystemModuleBase<Sys_Attr>
    {
        public Dictionary<uint, ulong> pkDiffs = new Dictionary<uint, ulong>();
        public List<uint> pkAttrsId = new List<uint>();
        public Dictionary<uint, long> pkAttrs = new Dictionary<uint, long>();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public Dictionary<uint, long> addAttrsPreview = new Dictionary<uint, long>();
        public Dictionary<uint, int> beforePoints = new Dictionary<uint, int>();
        public Dictionary<uint, int> addbeforeAttrsPreview = new Dictionary<uint, int>();
        public Dictionary<uint, int> baseAttrsafter = new Dictionary<uint, int>();

        public Dictionary<uint, uint> captainPointDic = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> aidPointDic = new Dictionary<uint, uint>();
        public Dictionary<uint, PrivilegeBuff> privilegeBuffDic = new Dictionary<uint, PrivilegeBuff>();
        public List<uint> privilegeBuffIdList = new List<uint>();

        public CmdAttrAttrNtf AttrNtf;
        public CmdAttrAddExpNtf AddExpNtf;
        private CSVWorldLevel.Data csvWorldLevelData;

        public ulong expMultiple;
        public ulong addexp;
        public ulong power;
        public ulong rolePower;

        public ulong hpPool;
        public ulong mpPool;
        public long curMp;
        public long curHp;
        public long surplusPoint;

        public int isChangeLevelInLogin;
        public ulong addExpInBattleEnd;

        public uint tutorLevel;
        public ulong tutorExp;

        public bool isHpAutoOpen;
        public bool isMpautoOpen;
        public uint curTransformCard;

        //方案切换
        public List<DtoBaseAttr> listBasePlans = new List<DtoBaseAttr>();
        public List<DtoPkAttr> listPkPlans = new List<DtoPkAttr>();
        public List<string> listPlansName = new List<string>();
        public uint curIndex;

        public enum EEvents : int
        {
            OnChangePotency,   //潜能点改变
            OnUpdateAttr,      // 属性更新
            OnBeforeChangePotency,  //预设潜能点改变
            OnUpdateBeforAttr,    //预设加点属性更新
            OnAddExp,            //新增经验
            OnRefreshAddAttr,    //刷新加点
            OnUpdateLevel,       //刷新等级
            OnUpdateNumber,   //刷新洗点道具个数
            OnSelectRoleViewType, //角色面板选择类型
            OnSelectFriendInfoViewType, //好友详情面板选择类型
            OnShowTitleView,      //是否显示title界面
            OnPreinstallChangePotency,     //预设潜能点改变
            OnHpMpPoolUpdate,       //血蓝池更新
            OnRoleHpMpUpdate,       //主角血蓝更新
            OnGradeUpdate,       //  评分更新
            OnDailyPointUpdate,     //每日点数更新（队长积分、援助值）
            OnPrivilegeBuffUpdate,    //特权buff更新  
            OnTutorInfoUpdate,    //导师信息更新  
            OnExtraExp,            //减少经验为负数（当前经验=0才会触发
            OnChangeTab,           //角色面板切换页签     
            OnSelectAddPointPlan,   //角色加点方案切换
            OnGetPointScheme,    //加点方案信息更新
            OnRenamePointScheme,  //加点方案改名
            OnSchemeUpdateAttr,  //加点方案属性更新
            OnGetBeforePoint,   //预设点回复
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.AttrNtf, OnAttrNtf, CmdAttrAttrNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.AllocPointReq, (ushort)CmdAttr.AllocPointRes, OnAllocPointRes, CmdAttrAllocPointRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.GetBeforePointReq, (ushort)CmdAttr.GetBeforePointRes, OnGetBeforePointRes, CmdAttrGetBeforePointRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.SetBeforePointReq, (ushort)CmdAttr.SetBeforePointRes, OnSetBeforePointRes, CmdAttrSetBeforePointRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.AddExpNtf, OnAddExpNtf, CmdAttrAddExpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.UpdateExpNtf, OnUpdateNtf, CmdAttrUpdateExpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.UpdatePoolNtf, OnUpdatePoolNtf, CmdAttrUpdatePoolNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.RoleHpNtf, OnRoleHpNtf, CmdAttrRoleHpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.RoleMpNtf, OnRoleMpNtf, CmdAttrRoleMpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.RoleHpMpNtf, OnRoleHpMpNtf, CmdAttrRoleHpMpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.PowerNtf, OnPowerNtf, CmdAttrPowerNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.GetDailyPointReq, (ushort)CmdAttr.GetDailyPointRes, OnGetDailyPointRes, CmdAttrGetDailyPointRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.PrivilegeBuffNtf, OnPrivilegeBuffNtf, CmdAttrPrivilegeBuffNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.PrivilegeBuffUpdateNtf, OnPrivilegeBuffUpdateNtf, CmdAttrPrivilegeBuffUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.HpMpItemCdntf, OnHpMpItemCdntf, CmdAttrHpMpItemCDNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.TutorInfoRes, OnTutorInfoRes, CmdAttrTutorInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.DetailInfoReq, (ushort)CmdSocial.DetailInfoAck, OnSocialDetailInfoAck, CmdSocialDetailInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.SetAutoHpMpPoolReq, (ushort)CmdAttr.SetAutoHpMpPoolRes, OnSetAutoHpMpPoolRes, CmdAttrSetAutoHpMpPoolRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.GmtextraExpNtf, OnGmtextraExpNtf, CmdAttrGMTExtraExpNtf.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.GetPointSchemeReq, (ushort)CmdAttr.GetPointSchemeRes, OnGetPointSchemeRes, CmdAttrGetPointSchemeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.SwitchPointSchemeReq, (ushort)CmdAttr.SwitchPointSchemeRes, OnSwitchPointSchemeRes, CmdAttrSwitchPointSchemeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.RenamePointSchemeReq, (ushort)CmdAttr.RenamePointSchemeRes, RenamePointSchemeRes, CmdAttrRenamePointSchemeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdAttr.AddPointSchemeReq, (ushort)CmdAttr.AddPointSchemeRes, OnAddPointSchemeRes, CmdAttrAddPointSchemeRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.SchemeUpdateAttrNtf, OnSchemeUpdateAttrNtf, CmdAttrSchemeUpdateAttrNtf.Parser);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.ChangePlan, OnChangePlan, true);
        }

        private void OnChangePlan(uint type, uint index)
        {
            if (type == (uint)Sys_Plan.EPlanType.RoleAttribute)
            {
                SwitchPointSchemeReq(index);
            }
        }

        public override void OnLogout()
        {
            pkDiffs.Clear();
            pkAttrs.Clear();
            pkAttrsId.Clear();
            captainPointDic.Clear();
            aidPointDic.Clear();
            privilegeBuffIdList.Clear();
            privilegeBuffDic.Clear();
            addexp = 0;
            isChangeLevelInLogin = 0;
            isHpAutoOpen = false;
            isMpautoOpen = false;
            csvWorldLevelData = null;
            listBasePlans.Clear();
            listPlansName.Clear();
            listPkPlans.Clear();
        }

        #region NetMessage
        private void OnAttrNtf(NetMsg msg)
        { 
            AttrNtf = NetMsgUtil.Deserialize<CmdAttrAttrNtf>(CmdAttrAttrNtf.Parser, msg);
            if (listBasePlans.Count > (int)curIndex)
            {
                listBasePlans[(int)curIndex] = AttrNtf.BaseAttr;
                listPkPlans[(int)curIndex] = AttrNtf.PkAttr;
            }
            for (int i = 0; i < AttrNtf.PkAttr.Attr.Count; ++i)
            {
                if (pkAttrs.ContainsKey(AttrNtf.PkAttr.Attr[i].AttrId))
                {
                    pkAttrs[AttrNtf.PkAttr.Attr[i].AttrId] = AttrNtf.PkAttr.Attr[i].AttrValue;
                }
                else
                {
                    pkAttrs.Add(AttrNtf.PkAttr.Attr[i].AttrId, AttrNtf.PkAttr.Attr[i].AttrValue);
                }
                if (!pkAttrsId.Contains(AttrNtf.PkAttr.Attr[i].AttrId)&&CSVAttr.Instance.ContainsKey(AttrNtf.PkAttr.Attr[i].AttrId))
                {
                    pkAttrsId.Add(AttrNtf.PkAttr.Attr[i].AttrId);
                }
            }
            Sys_Attr.Instance.pkAttrsId.Sort((dataA, dataB) =>
            {
                return CSVAttr.Instance.GetConfData(dataA).order_list.CompareTo(CSVAttr.Instance.GetConfData(dataB).order_list);
            });

            power = AttrNtf.Power;
            rolePower = AttrNtf.RolePower;
            hpPool = AttrNtf.HpPool;
            mpPool = AttrNtf.MpPool;
            curHp = AttrNtf.PkAttr.CurHp;
            curMp = AttrNtf.PkAttr.CurMp;
            surplusPoint= AttrNtf.BaseAttr.SurplusPoint;
            CalAttrDiff();

            isHpAutoOpen = AttrNtf.AutoHpPool;
            isMpautoOpen = AttrNtf.AutoMpPool;

            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnUpdateAttr);
        }

        public void AttrAllocPointReq(int vit, int snh, int inten, int speed, int magic, uint index)
        {
            CmdAttrAllocPointReq req = new CmdAttrAllocPointReq();
            req.Vit = vit;
            req.Snh = snh;
            req.Inten = inten;
            req.Speed = speed;
            req.Magic = magic;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdAttr.AllocPointReq, req);
        }

        private void OnAllocPointRes(NetMsg msg)
        {
            CmdAttrAllocPointRes res = NetMsgUtil.Deserialize<CmdAttrAllocPointRes>(CmdAttrAllocPointRes.Parser, msg);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.AddRoleAttribute);
        }

        public void AttrGetBeforePointReq(uint index)
        {
            CmdAttrGetBeforePointReq req = new CmdAttrGetBeforePointReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdAttr.GetBeforePointReq, req);
        }

        private void OnGetBeforePointRes(NetMsg msg)
        {
            CmdAttrGetBeforePointRes res = NetMsgUtil.Deserialize<CmdAttrGetBeforePointRes>(CmdAttrGetBeforePointRes.Parser, msg);
            beforePoints.Clear();
            beforePoints.Add(5, res.Vit);
            beforePoints.Add(7, res.Snh);
            beforePoints.Add(9, res.Inten);
            beforePoints.Add(11, res.Speed);
            beforePoints.Add(13, res.Magic);
            eventEmitter.Trigger<uint>(EEvents.OnGetBeforePoint,res.Index);
        }

        public void AttrSetBeforePointReq(int vit, int snh, int inten, int speed, int magic,uint index)
        {
            CmdAttrAllocPointReq req = new CmdAttrAllocPointReq();
            req.Vit = vit;
            req.Snh = snh;
            req.Inten = inten;
            req.Speed = speed;
            req.Magic = magic;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdAttr.SetBeforePointReq, req);
        }

        private void OnSetBeforePointRes(NetMsg msg)
        {
            CmdAttrSetBeforePointRes res = NetMsgUtil.Deserialize<CmdAttrSetBeforePointRes>(CmdAttrSetBeforePointRes.Parser, msg);
            beforePoints.Clear();
            beforePoints.Add(5, res.Vit);
            beforePoints.Add(7, res.Snh);
            beforePoints.Add(9, res.Inten);
            beforePoints.Add(11, res.Speed);
            beforePoints.Add(13, res.Magic);
            Sys_Attr.Instance.eventEmitter.Trigger<uint>(Sys_Attr.EEvents.OnUpdateBeforAttr,res.Index);
        }

        private void OnAddExpNtf(NetMsg msg)
        {
            AddExpNtf = NetMsgUtil.Deserialize<CmdAttrAddExpNtf>(CmdAttrAddExpNtf.Parser, msg);
            bool isChangeLevel = Sys_Role.Instance.Role.Level != AddExpNtf.Level;
            Sys_Role.Instance.Role.Level = AddExpNtf.Level;
            addexp = AddExpNtf.AddExp;
            Sys_Role.Instance.Role.Exp = AddExpNtf.Exp;
            expMultiple = AddExpNtf.ExpMultiple;
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnAddExp);
            if (addexp != 0)
            {
                if (Sys_Role.Instance.hasSyncFinished)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4303, Sys_Attr.Instance.addexp.ToString()));
                }
            }
            if (isChangeLevel)
            {
                Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnUpdateLevel);
                Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnUpdateAttr);
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, LanguageHelper.GetTextContent(11621, AddExpNtf.Level.ToString()), Sys_Chat.EMessageProcess.None);
                SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.onSDKExitReportExtraData, SDKManager.SDKReportState.LEVEL);

                if (GameCenter.mainHero != null)
                {
                    AudioUtil.PlayAudio(4001);
                    GameCenter.mainHero.heroFxComponent.UpdateLevelUpFx(true);
                    PlayActorLevelUpHudEvt evt = new PlayActorLevelUpHudEvt();
                    evt.actorId = GameCenter.mainHero.UID;
                    Sys_HUD.Instance.eventEmitter.Trigger<PlayActorLevelUpHudEvt>(Sys_HUD.EEvents.OnPlayActorLevelUpFx, evt);
                }
                isChangeLevelInLogin++;
                isChangeLevelInLogin = isChangeLevelInLogin > 2 ? 2 : isChangeLevelInLogin;
            }
            SetExpContent();
            if (AddExpNtf.Reason == (uint)LuckyDrawActiveReason.BattleEnd)
                addExpInBattleEnd = AddExpNtf.AddExp;
        }

        private void OnUpdateNtf(NetMsg msg)
        {
            DebugUtil.Log(ELogType.eAttr, "OnUpdateNtf");
            CmdAttrUpdateExpNtf response = NetMsgUtil.Deserialize<CmdAttrUpdateExpNtf>(CmdAttrUpdateExpNtf.Parser, msg);
            Sys_Role.Instance.Role.Exp = response.Exp;
            eventEmitter.Trigger(EEvents.OnAddExp);
            SetExpContent();
        }

        private void OnUpdatePoolNtf(NetMsg msg)
        {
            CmdAttrUpdatePoolNtf response = NetMsgUtil.Deserialize<CmdAttrUpdatePoolNtf>(CmdAttrUpdatePoolNtf.Parser, msg);
            if (hpPool < response.HpPool)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4450, (response.HpPool - hpPool).ToString()));
            }
            hpPool = response.HpPool;
            if (mpPool < response.MpPool)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4451, (response.MpPool - mpPool).ToString()));
            }
            mpPool = response.MpPool;
            eventEmitter.Trigger(EEvents.OnHpMpPoolUpdate);
        }

        private void OnRoleHpNtf(NetMsg msg)
        {
            CmdAttrRoleHpNtf response = NetMsgUtil.Deserialize<CmdAttrRoleHpNtf>(CmdAttrRoleHpNtf.Parser, msg);
            if(response.CurHp> curHp)
            {
                string content = LanguageHelper.GetTextContent(1012002, Sys_Role.Instance.Role.Name.ToStringUtf8(), (response.CurHp- curHp).ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            }
            curHp = response.CurHp;
            pkAttrs[15] = response.MaxHp;
            eventEmitter.Trigger(EEvents.OnRoleHpMpUpdate);

        }

        private void OnRoleMpNtf(NetMsg msg)
        {
            CmdAttrRoleMpNtf response = NetMsgUtil.Deserialize<CmdAttrRoleMpNtf>(CmdAttrRoleMpNtf.Parser, msg);
            if (response.CurMp > curMp)
            {
                string content = LanguageHelper.GetTextContent(1012003, Sys_Role.Instance.Role.Name.ToStringUtf8(), (response.CurMp - curMp).ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            }
            curMp = response.CurMp;
            pkAttrs[17] = response.MaxMp;
            eventEmitter.Trigger(EEvents.OnRoleHpMpUpdate);
             
        }

        private void OnRoleHpMpNtf(NetMsg msg)
        {
            CmdAttrRoleHpMpNtf response = NetMsgUtil.Deserialize<CmdAttrRoleHpMpNtf>(CmdAttrRoleHpMpNtf.Parser, msg);
            if (response.CurHp > curHp)
            {
                string content = LanguageHelper.GetTextContent(1012002, Sys_Role.Instance.Role.Name.ToStringUtf8(), (response.CurHp - curHp).ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            }
            if (response.CurMp > curMp)
            {
                string content = LanguageHelper.GetTextContent(1012003, Sys_Role.Instance.Role.Name.ToStringUtf8(), (response.CurMp - curMp).ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            }
            curMp = response.CurMp;
            curHp = response.CurHp;
            eventEmitter.Trigger(EEvents.OnRoleHpMpUpdate);
        }

        private void OnPowerNtf(NetMsg msg)
        {
            CmdAttrPowerNtf response = NetMsgUtil.Deserialize<CmdAttrPowerNtf>(CmdAttrPowerNtf.Parser, msg);
            power = response.Power;
            rolePower = response.RolePower;
            eventEmitter.Trigger(EEvents.OnGradeUpdate);
        }

        public void GetDailyPointReq()
        {
            CmdAttrGetDailyPointReq req = new CmdAttrGetDailyPointReq();
            NetClient.Instance.SendMessage((ushort)CmdAttr.GetDailyPointReq, req);
        }

        private void OnGetDailyPointRes(NetMsg msg)
        {
            CmdAttrGetDailyPointRes res = NetMsgUtil.Deserialize<CmdAttrGetDailyPointRes>(CmdAttrGetDailyPointRes.Parser, msg);
            captainPointDic.Clear();
            aidPointDic.Clear();
            for (int i = 0; i < res.CaptainPoint.Count; ++i)
            {
                captainPointDic.Add(res.CaptainPoint[i].Id, res.CaptainPoint[i].Value);
            }
            for (int i = 0; i < res.AidPoint.Count; i++)
            {
                aidPointDic.Add(res.AidPoint[i].Id, res.AidPoint[i].Value);
            }
            eventEmitter.Trigger(EEvents.OnDailyPointUpdate);
        }

        public void RePointReq(uint itemId, uint index)
        {
            CmdAttrRePointReq req = new CmdAttrRePointReq();
            req.ItemId = itemId;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdAttr.RePointReq, req);
        }

        private void OnPrivilegeBuffNtf(NetMsg msg)
        {
            CmdAttrPrivilegeBuffNtf response = NetMsgUtil.Deserialize<CmdAttrPrivilegeBuffNtf>(CmdAttrPrivilegeBuffNtf.Parser, msg);
            privilegeBuffDic.Clear();
            privilegeBuffIdList.Clear();
            for (int i = 0; i < response.PrivilegeBuffs.Count; ++i)
            {
                privilegeBuffDic.Add(response.PrivilegeBuffs[i].Id, response.PrivilegeBuffs[i]);
                privilegeBuffIdList.Add(response.PrivilegeBuffs[i].Id);
                if (response.PrivilegeBuffs[i].Id==5 && response.PrivilegeBuffs[i].Params.Count!=0)
                {
                    curTransformCard = response.PrivilegeBuffs[i].Params[0];
                }                               
            }
            CheckPrivilegeBuffDataExpire();
        }
        private void OnPrivilegeBuffUpdateNtf(NetMsg msg)
        {
            CmdAttrPrivilegeBuffUpdateNtf response = NetMsgUtil.Deserialize<CmdAttrPrivilegeBuffUpdateNtf>(CmdAttrPrivilegeBuffUpdateNtf.Parser, msg);
            if (response.Op == 0)
            {
                privilegeBuffDic.Add(response.PrivilegeBuff.Id, response.PrivilegeBuff);
                privilegeBuffIdList.Add(response.PrivilegeBuff.Id);
                if (response.PrivilegeBuff.Id == 5)
                {
                    curTransformCard = response.PrivilegeBuff.Params[0];
                    Sys_Transfiguration.Instance.eventEmitter.Trigger<uint>(Sys_Transfiguration.EEvents.OnUseChangeCard, curTransformCard);
                }
            }
            else if (response.Op == 1)
            {
                if (privilegeBuffDic.ContainsKey(response.PrivilegeBuff.Id))
                {
                    privilegeBuffDic.Remove(response.PrivilegeBuff.Id);
                    privilegeBuffIdList.Remove(response.PrivilegeBuff.Id);
                    if (response.PrivilegeBuff.Id == 5)
                    {
                        curTransformCard =0;
                    }
                }
            }
            else
            {
                if (privilegeBuffDic.ContainsKey(response.PrivilegeBuff.Id))
                {
                    privilegeBuffDic[response.PrivilegeBuff.Id] = response.PrivilegeBuff;
                    if (response.PrivilegeBuff.Id == 5)
                    {
                        curTransformCard = response.PrivilegeBuff.Params[0];
                        Sys_Transfiguration.Instance.eventEmitter.Trigger<uint>(Sys_Transfiguration.EEvents.OnUseChangeCard, curTransformCard);

                    }
                }
            }
            CheckPrivilegeBuffDataExpire();
            eventEmitter.Trigger<uint, uint>(EEvents.OnPrivilegeBuffUpdate, response.PrivilegeBuff.Id, response.Op);
        }

        private void OnHpMpItemCdntf(NetMsg netMsg)
        {
            CmdAttrHpMpItemCDNtf cmdAttrHpMpItemCDNtf = NetMsgUtil.Deserialize<CmdAttrHpMpItemCDNtf>(CmdAttrHpMpItemCDNtf.Parser, netMsg);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012001, cmdAttrHpMpItemCDNtf.Cd.ToString()));
        }

        //手动删除的时候force=true，到期删除force=false
        public void DelPrivilegeBuffReq(uint buffId, bool force) 
        {
            CmdAttrDelPrivilegeBuffReq req = new CmdAttrDelPrivilegeBuffReq();
            req.BuffId = buffId;
            req.Force = force;
            NetClient.Instance.SendMessage((ushort)CmdAttr.DelPrivilegeBuffReq, req);
        }
        /// <summary>
        /// 导师信息请求
        /// </summary>
        public void TutorInfoReq()
        {
            CmdAttrTutorInfoReq req = new CmdAttrTutorInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdAttr.TutorInfoReq, req);
        }
        /// <summary>
        /// 导师信息返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnTutorInfoRes(NetMsg msg)
        {
            CmdAttrTutorInfoRes response = NetMsgUtil.Deserialize<CmdAttrTutorInfoRes>(CmdAttrTutorInfoRes.Parser, msg);
            if (response != null)
            {
                tutorLevel = response.Level;
                tutorExp = response.Exp;
                Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnTutorInfoUpdate);
            }
        }

        //人物详情请求
        public void SocialDetailInfoReq(ulong roleId)
        {
            CmdSocialDetailInfoReq req = new CmdSocialDetailInfoReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdSocial.DetailInfoReq, req);
        }

        //人物详情回复
        private void OnSocialDetailInfoAck(NetMsg msg)
        {
            CmdSocialDetailInfoAck ack = NetMsgUtil.Deserialize<CmdSocialDetailInfoAck>(CmdSocialDetailInfoAck.Parser, msg);
            if (ack == null)
            {
                return;
            }
            FriendInfoEvt evt = new FriendInfoEvt();
            evt.roleId = ack.RoleId;
            evt.roleBase = ack.BaseInfo;
            evt.power = ack.Attrs.Power;
            evt.rolePower = ack.Attrs.RolePower;
            evt.pkAttrsId = new List<uint>();
            evt.pkAttrs = new Dictionary<uint, long>();
            evt.baseAttrs2AttrAssigh = new Dictionary<uint, int>();
            for (int i = 0; i <ack.Attrs.PkAttr.Attr.Count; ++i)
            {
                if (evt.pkAttrs.ContainsKey(ack.Attrs.PkAttr.Attr[i].AttrId))
                {
                    evt.pkAttrs[ack.Attrs.PkAttr.Attr[i].AttrId] = ack.Attrs.PkAttr.Attr[i].AttrValue;
                }
                else
                {
                    evt.pkAttrs.Add(ack.Attrs.PkAttr.Attr[i].AttrId, ack.Attrs.PkAttr.Attr[i].AttrValue);
                }
                if (!evt.pkAttrsId.Contains(ack.Attrs.PkAttr.Attr[i].AttrId))
                {
                    evt.pkAttrsId.Add(ack.Attrs.PkAttr.Attr[i].AttrId);
                }
            }  
            evt.baseAttrs2AttrAssigh.Add(5, ack.Attrs.BaseAttr.VitAssign);
            evt.baseAttrs2AttrAssigh.Add(7, ack.Attrs.BaseAttr.SnhAssign);
            evt.baseAttrs2AttrAssigh.Add(9, ack.Attrs.BaseAttr.IntenAssign);
            evt.baseAttrs2AttrAssigh.Add(11, ack.Attrs.BaseAttr.SpeedAssign);
            evt.baseAttrs2AttrAssigh.Add(13, ack.Attrs.BaseAttr.MagicAssign);
            evt.pkAttrsId.Sort((dataA, dataB) =>
            {
                return CSVAttr.Instance.GetConfData(dataA).order_list.CompareTo(CSVAttr.Instance.GetConfData(dataB).order_list);
            });
            if (ack.PetInfo != null)
            {
                evt.clientPet = new ClientPet(ack.PetInfo);
            }
            evt.fashoinDic = new Dictionary<uint, List<dressData>>();
            evt.fashoinDic = Sys_Fashion.Instance.GetDressData(ack.FashionInfo, ack.BaseInfo.HeroId);
            evt.weaponFashionId = GetRankFashionWeaponId(ack.FashionInfo);
            evt.headId = ack.HeadPhoto;
            evt.headFrameId = ack.HeadFrame;
            UIManager.OpenUI(EUIID.UI_Friend_Attribute,false,evt);
        }
        
        public void SetAutoHpMpPoolReq(bool isHpOn,bool isMpOn)
        {
            CmdAttrSetAutoHpMpPoolReq req = new CmdAttrSetAutoHpMpPoolReq();
            req.AutoHpPool = isHpOn;
            req.AutoMpPool = isMpOn;
            NetClient.Instance.SendMessage((ushort)CmdAttr.SetAutoHpMpPoolReq, req);
        }

        private void OnSetAutoHpMpPoolRes(NetMsg msg)
        {
            CmdAttrSetAutoHpMpPoolRes res = NetMsgUtil.Deserialize<CmdAttrSetAutoHpMpPoolRes>(CmdAttrSetAutoHpMpPoolRes.Parser, msg);
            isHpAutoOpen = res.AutoHpPool;
            isMpautoOpen = res.AutoMpPool;
        }

        private void OnGmtextraExpNtf(NetMsg msg)
        {
            CmdAttrGMTExtraExpNtf cmdAttrGMTExtraExpNtf = NetMsgUtil.Deserialize<CmdAttrGMTExtraExpNtf>(CmdAttrGMTExtraExpNtf.Parser, msg);
            Sys_Role.Instance.Role.ExtraExp = cmdAttrGMTExtraExpNtf.ExtraExp;
            eventEmitter.Trigger(EEvents.OnExtraExp);
        }

        private uint GetRankFashionWeaponId(RepeatedField<MapRoleFashionInfo> fashInfo)
        {
            List<uint> dre = new List<uint>();
            for (int i = 0; i < fashInfo.Count; i++)
            {
                dre.Add(fashInfo[i].FashionId);
            }
            return Sys_Fashion.Instance.GetDressedWeaponFashionId(dre);
        }

        public void GetPointSchemeReq()
        {
            if (listBasePlans.Count == 0)
            {
                CmdAttrGetPointSchemeReq req = new CmdAttrGetPointSchemeReq();
                NetClient.Instance.SendMessage((ushort)CmdAttr.GetPointSchemeReq, req);
            }
        }

        private void OnGetPointSchemeRes(NetMsg msg)
        {
            CmdAttrGetPointSchemeRes res = NetMsgUtil.Deserialize<CmdAttrGetPointSchemeRes>(CmdAttrGetPointSchemeRes.Parser, msg);
            listBasePlans.Clear();
            listPlansName.Clear();
            listPkPlans.Clear();
            for (int i = 0; i < res.BaseAttrList.Count; ++i)
            {
                listBasePlans.Add(res.BaseAttrList[i]);
            }
            for (int i = 0; i < res.Name.Count; ++i)
            {
                listPlansName.Add(res.Name[i].ToStringUtf8());
            }
            for (int i = 0; i < res.PkAttrList.Count; ++i)
            {
                listPkPlans.Add(res.PkAttrList[i]);
            }
            curIndex = res.Index;
            Sys_Attr.Instance.eventEmitter.Trigger(EEvents.OnGetPointScheme);
        }

        public void AddPointSchemeReq()
        {
            CmdAttrAddPointSchemeReq req = new CmdAttrAddPointSchemeReq();
            NetClient.Instance.SendMessage((ushort)CmdAttr.AddPointSchemeReq, req);
        }

        private void OnAddPointSchemeRes(NetMsg msg)
        {
            CmdAttrAddPointSchemeRes res = NetMsgUtil.Deserialize<CmdAttrAddPointSchemeRes>(CmdAttrAddPointSchemeRes.Parser, msg);
            listBasePlans.Add(res.BaseAttrList);
            listPkPlans.Add(res.PkAttrList);
            listPlansName.Add(res.Name.ToStringUtf8());
            uint index = (uint)listBasePlans.Count - 1;
            Sys_Attr.Instance.eventEmitter.Trigger(EEvents.OnGetPointScheme);
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.RoleAttribute, index);
        }

        public void SwitchPointSchemeReq(uint index)
        {
            CmdAttrSwitchPointSchemeReq req = new CmdAttrSwitchPointSchemeReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdAttr.SwitchPointSchemeReq, req);
        }

        private void OnSwitchPointSchemeRes(NetMsg msg)
        {
            CmdAttrSwitchPointSchemeRes res = NetMsgUtil.Deserialize<CmdAttrSwitchPointSchemeRes>(CmdAttrSwitchPointSchemeRes.Parser, msg);
            curIndex = res.Index;
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.RoleAttribute, curIndex);
        }

        public void RenamePointSchemeReq(uint index,string name)
        {
            CmdAttrRenamePointSchemeReq req = new CmdAttrRenamePointSchemeReq();
            req.Index = index;
            req.Name= FrameworkTool.ConvertToGoogleByteString(name);
            NetClient.Instance.SendMessage((ushort)CmdAttr.RenamePointSchemeReq, req);
        }

        private void RenamePointSchemeRes(NetMsg msg)
        {
            CmdAttrRenamePointSchemeRes res = NetMsgUtil.Deserialize<CmdAttrRenamePointSchemeRes>(CmdAttrRenamePointSchemeRes.Parser, msg);
            if (listPlansName.Count >= res.Index)
            {
                listPlansName[(int)res.Index] = res.Name.ToStringUtf8();
            }
            Sys_Attr.Instance.eventEmitter.Trigger<uint>(EEvents.OnRenamePointScheme, res.Index);
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, (uint)Sys_Plan.EPlanType.RoleAttribute, res.Index, res.Name.ToStringUtf8());
        }

        private void OnSchemeUpdateAttrNtf(NetMsg msg)
        {
            CmdAttrSchemeUpdateAttrNtf ntf = NetMsgUtil.Deserialize<CmdAttrSchemeUpdateAttrNtf>(CmdAttrSchemeUpdateAttrNtf.Parser, msg);
            if(listPkPlans.Count> ntf.Index)
            {
                listPkPlans[(int)ntf.Index] = ntf.PkAttrList;
            }
            if (listBasePlans.Count > ntf.Index)
            {
                listBasePlans[(int)ntf.Index] = ntf.BaseAttrList;
            }
            Sys_Attr.Instance.eventEmitter.Trigger(EEvents.OnSchemeUpdateAttr);
        }
        #endregion

        #region Function

        private void CalAttrDiff()
        {
            Array attrAttrs = System.Enum.GetValues(typeof(EPkAttr));

            if (pkDiffs.Count != 0)
            {
                foreach (var attr in pkAttrs)
                {
                    if (attr.Key >= 1)
                    {
                        Table.CSVAttr.Data infoData = Table.CSVAttr.Instance.GetConfData(attr.Key);
                        if (infoData != null && infoData.attr_type != 0 && pkAttrs.ContainsKey(attr.Key) && pkDiffs.ContainsKey(attr.Key))
                        {
                            long diff = (pkAttrs[attr.Key]) - (long)(pkDiffs[attr.Key]);
                            if (diff != 0)
                            {
                                Sys_Hint.Instance.PushContent_Property(attr.Key, diff);
                            }
                        }
                    }
                }
            }

            foreach (var attr in pkAttrs)
            {
                if (attr.Key != 0 && pkAttrs.ContainsKey(attr.Key))
                {
                    pkDiffs[attr.Key] = (ulong)pkAttrs[attr.Key];
                }
                else
                {
                    pkDiffs[attr.Key] = 0;
                }
            }
        }

        public void SetExpContent()
        {
            if (addexp == 0)
                return;
            string content = null;

            if (expMultiple == 100)
            {
                content = LanguageHelper.GetTextContent(4303, ((int)addexp).ToString());
            }
            else
            {
                content = LanguageHelper.GetTextContent(4305, ((int)addexp).ToString(), ((int)expMultiple).ToString());
            }
            Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            addexp = 0;
        }

        public string GetAttrValue(CSVAttr.Data attrData, long attrValue)
        {
            if (attrData.show_type == 2)
            {
                return (attrValue / 10000f).ToString("P2");
            }
            else
            {
                return attrValue.ToString();
            }
        }

        public void CheckPrivilegeBuffDataExpire()
        {
            privilegeBuffIdList.Sort((dataA, dataB) =>
            {
                if (CSVPrivilege.Instance.GetConfData(dataA).SortId > CSVPrivilege.Instance.GetConfData(dataB).SortId)
                    return 1;
                else if (CSVPrivilege.Instance.GetConfData(dataA).SortId < CSVPrivilege.Instance.GetConfData(dataB).SortId)
                    return -1;
                else
                {
                    if (dataA > dataB)
                        return 1;
                    else
                        return -1;
                }
            });
            uint nowTime = Sys_Time.Instance.GetServerTime();
            foreach (var data in privilegeBuffDic)
            {
                if (data.Value.ExpireTime != 0)
                {
                    if (nowTime < data.Value.ExpireTime)
                    {
                        Timer timer;
                        uint time = data.Value.ExpireTime - nowTime;
                        timer = Timer.Register(time, () =>
                        {
                            DelPrivilegeBuffReq(data.Key, false);
                        }, null, false, true);
                    }
                }
            }
        }

        #region CaptainPoint And AidPoint
        public uint GetDailyCurCaptainPoint()
        {
            uint point = 0;
            foreach (var item in Sys_Attr.Instance.captainPointDic)
            {
                point += item.Value;
            }
            return point;
        }

        public uint GetDailyMaxCaptainPoint()
        {
            uint point = 0;
            foreach (var item in CSVCaptainPoint.Instance.GetAll())
            {
                point += CSVBattleType.Instance.GetConfData(item.BattleType).CaptainUpperLimit;
            }
            return point;
        }

        public uint GetDailyCurAidPoint()
        {
            uint point = 0;
            foreach (var item in Sys_Attr.Instance.aidPointDic)
            {
                point += item.Value;
            }
            return point;
        }

        public uint GetDailyMaxAidPoint()
        {
            uint point = 0;
            foreach (var item in CSVAidPoint.Instance.GetAll())
            {
                point += CSVBattleType.Instance.GetConfData(item.BattleType).AidUpperLimit;
            }
            return point;
        }

        #endregion

        #region WorldLevel
        public uint GetServerMultiple(uint Id)
        {
            uint percent = 100;
            CSVWorldLevel.Data data = CSVWorldLevel.Instance.GetConfData(Id);
            uint realLv = GetRealLv();
            if (data.world_level >= realLv)
            {
                int worldLvsubroleLv = (int)data.world_level - (int)realLv;
                for (int i = 0; i < data.up_level.Count; ++i)
                {
                    if (worldLvsubroleLv >= data.up_level[i])
                    {
                        percent = data.up_percent[i];
                        break;
                    }
                }
            }
            else
            {
                int roleLvSubWorldLv =  (int)realLv- (int)data.world_level;
                for (int i = 0; i < data.down_level.Count; ++i)
                {
                    if (roleLvSubWorldLv >= data.down_level[i])
                    {
                        percent = data.down_percent[i];
                        break;
                    }
                }
            }   
            return percent;
        }

        public float GetLimitInCareerMultiple()
        {
            return 100.0f;
    
        }

        public uint GetRealLv()
        {
            foreach (var data in CSVCharacterAttribute.Instance.GetAll())
            {
                ulong totalLevel;
                if (Sys_Role.Instance.Role.Level == 1)
                {
                    totalLevel = Sys_Role.Instance.Role.Exp;
                }
                else
                {
                    totalLevel = Sys_Role.Instance.Role.Exp + CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).totol_exp;
                }
                if (data.totol_exp > totalLevel)
                {
                    return data.id-1;
                }
            }
            return 100;
        }

        public float GetExpMultiple( )
        {
            if (CSVWorldLevel.Instance.TryGetValue(Sys_Role.Instance.openServiceDay, out csvWorldLevelData) && csvWorldLevelData != null) { }
            else
            {
                uint maxKey = CSVWorldLevel.Instance.GetKeys().Max();
                csvWorldLevelData = CSVWorldLevel.Instance.GetConfData(maxKey);
            }
            int lv = 0;
            int.TryParse(CSVParam.Instance.GetConfData(261).str_value, out lv);
            float a = 0;
            if (Sys_Role.Instance.Role.Level >= lv)
            {
                a = GetServerMultiple(csvWorldLevelData.id);
            }
            else
            {
                a = 100;
            }
            return a;
        }

        public float GetExpRealPrecent()
        {
            if (GameCenter.mainHero == null)
                return 0;
            float num = 0;
            num = GetExpMultiple();
            return num;
        }

        public long GetRoleBaseSpeed()
        {
            if (!Sys_Attr.Instance.pkAttrs.TryGetValue(101, out long roleSpeed))
            {
                roleSpeed = 40000;
            }

            if (Sys_Pet.Instance.mountPetUid != 0)
            {
                roleSpeed = roleSpeed - Sys_Pet.Instance.GetMountPetSpeed();
            }

            return roleSpeed;
        }
        #endregion

        public void GetMoreInfo(ulong roleId, uint intimacyId, bool isOnLine)
        {
            if (CSVFriendIntimacy.Instance.TryGetValue(intimacyId, out CSVFriendIntimacy.Data cSVFriendIntimacyData) && cSVFriendIntimacyData.RoleViewLevel < 1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13014));
                return;
            }
            if (!isOnLine)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13015));
                return;
            }
            SocialDetailInfoReq(roleId);
        }

        public bool CheckWorldLevelChanged()
        {
            uint lastDay = Sys_Role.Instance.lastClickOpenServiceDay;
            if (CSVWorldLevel.Instance.ContainsKey(lastDay))
            {
               return  Sys_Role.Instance.openServiceDay != lastDay;
            }
            else
            {
                return false;
            }
        }

        public int GetAssighPointByAttrId(uint attrId,uint index)
        {
            int assighPoint = 0;
            if(Sys_Attr.Instance.listBasePlans.Count< index)
            {
                return assighPoint;
            }
            if (attrId == 5)
            {
                assighPoint = Sys_Attr.Instance.listBasePlans[(int)index].VitAssign;
            }
            else if (attrId == 7)
            {
                assighPoint = Sys_Attr.Instance.listBasePlans[(int)index].SnhAssign;
            }
            else if (attrId == 9)
            {
                assighPoint = Sys_Attr.Instance.listBasePlans[(int)index].IntenAssign;
            }
            else if (attrId == 11)
            {
                assighPoint = Sys_Attr.Instance.listBasePlans[(int)index].SpeedAssign;
            }
            else if (attrId == 13)
            {
                assighPoint = Sys_Attr.Instance.listBasePlans[(int)index].MagicAssign;
            }
            else if (attrId == 0)
            {
                assighPoint = Sys_Attr.Instance.listBasePlans[(int)index].SurplusPoint;
            }
            return assighPoint;
        }

        public long GetPkAttrByIndex(int index, uint attrId)
        {
            for(int i=0;i< listPkPlans[index].Attr.Count; ++i)
            {
                if (listPkPlans[index].Attr[i].AttrId == attrId)
                {
                    return listPkPlans[index].Attr[i].AttrValue;
                }
            }
            return 0;
        }

        #endregion
    }
}