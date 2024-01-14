using Framework;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public struct UpdateFamilyTeamNumInBattleResource
    {
        public ulong actorId;
        public uint teamNum;
        public uint maxCount;
    }

    public struct UpdateFamliyNameBeforeBattleRescorce
    {
        public ulong actorId;
        public string name;
        public uint pos;
    }

    public struct NpcBattleCdEvt
    {
        public ulong actorId;
        public int cd;
    }

    public struct PlayActorLevelUpHudEvt
    {
        public ulong actorId;
    }
    public struct PlayActorAdvanceUpHudEvt
    {
        public ulong actorId;
    }
    public struct PlayActorReputationHudEvt
    {
        public ulong actorId;
    }

    public struct UpdateWorldBossHuDEvt
    {
        public ulong actorId;
        public uint level;
        public uint iconId;
    }

    public struct ClearWorldBossHuDEvt
    {
        public ulong actorId;
    }

    public struct UpdateActorFightStateEvt
    {
        public ulong actorId;
        public bool state;
    }

    public struct UpdateFavirabilityEvt
    {
        public ulong npcId;
        public uint val;
    }

    public struct ClearFavirabilityEvt
    {
        public uint npcId;
    }

    public struct CreateTitleEvt
    {
        public ulong actorId;
        public uint titleId;
        public string titleName;
        public uint pos;
    }

    public struct ClearTitleEvt
    {
        public ulong actorId;
    }

    public class UpdateBGroupTitleEvt
    {
        public ulong actorId;
        public uint titleId;
        public string name;
        public uint pos;
    }

    public class CreateEmotionEvt
    {
        public ulong actorId;           //场景中 
        public uint battleId;           //战斗内
        public GameObject gameObject;
        public uint emtionId;
    }

    public class CreateOrderHUDEvt
    {
        public uint senderId;
        public uint receiverId;
        public uint iconId;
    }

    public class CreateActorHUDEvt
    {
        public ulong id;
        public GameObject gameObject;
        public Vector3 offest;
        public uint appellation;//npc
        public string name;
        public EFightOutActorType eFightOutActorType;
        public bool isBack = false;
    }

    public class BuffHuDUpdateEvt   //更新当次buff显示
    {
        public uint id;             //battle uuid
        public uint buffid;
        public bool add;
        public int side;
        public uint count = 0;      //剩余计数 默认0不显示数量
    }

    public class BuffHUDFlashEvt
    {
        public uint id;     //battle uuid
        public uint buffid;
    }

    public class ActorHUDUpdateEvt
    {
        public ulong id;             //Npc id
        public uint iconId;         //状态改变之后要更新的图标
    }

    public class CreateOrUpdateActorHUDStateFlagEvt
    {
        public ulong id;
        public int type;        // 1:问号   2:感叹号   3:点点点   4:小手   5:灯泡
    }

    public class ActorHUDTitleNameUpdateEvt
    {
        public ulong id;
        public EFightOutActorType eFightOutActorType;
    }

    public class ActorHUDNameUpdateEvt
    {
        public ulong id;
        public EFightOutActorType eFightOutActorType;
        public string name;
        public bool upBack=false;
    }


    public struct ShowOrHideActorHUDEvt
    {
        public ulong id;
        public bool flag;
    }

    public class ShowOrHideActorsHUDEvt
    {
        public List<ulong> showIds;
        public List<ulong> hideIds;
    }

    public struct UpMountEvt
    {
        public ulong actorId;
        public uint mountId;
    }


    public class AttributeData
    {
        public uint fightUnitType;   //战斗单位类型  限玩家 伙伴 宠物
        public int UnitLevel;        //战斗单位等级
        public Dictionary<uint, uint> playerAttr = new Dictionary<uint, uint>(); //如果是玩家 就用这个数据结构 key:属性id value：属性值
        public uint notPlayerAttr;  //如果是怪物  宠物或者伙伴 用这个数据结构   怪物 宠物或者伙伴id
        public string playerName;
        public string petName;
        public uint ShapeShiftId;   //变身id 用来显示名字左边的变身图标
        public uint RoleCareer;     //角色职业id
    }

    public class CreateBloodEvt
    {
        public uint id;
        public int side;        //属于哪一方阵营
        public int ClientNum;
        public GameObject gameObject;
        public AttributeData attributeData;
    }

    public class ShowOrHideBDEvt
    {
        public uint id;
        public bool flag;
    }

    public class UpdateArrowEvt
    {
        public uint id;
        public bool active;
    }

    public class UpdateSparSkillEvt
    {
        public uint id;
        public bool active;
    }

    public class HpValueChangedEvt
    {
        public uint id;           //战斗单位id
        public float ratio;       //当前hp百分比
    }

    public class MpValueChangedEvt
    {
        public uint id;          //战斗单位id   
        public float ratio;      //当前Mp百分比
    }
    public class ShapeShiftChangedEvt
    {
        public uint id;             //战斗单位id   
        public uint ShapeShiftId;   //当前变身id
    }

    public class ShieldValueChangedEvt
    {
        public uint id;          //战斗单位id   
        public float ratio;      //当前护盾百分比
    }
    public class EnergyValueChangedEvt
    {
        public uint id;          //战斗单位id   
        public float ratio;      //当前能量百分比
    }
    public class TriggerBattleBubbleEvt
    {
        public uint battleid;       //战斗单位id
        public uint bubbleid;      //气泡id
        public ChatType chatType;       //当bubbleid=0时用
        public string content;      //当bubbleid=0时用
        public int ClientNum;       //客户端位置
    }

    public class TriggerExpressionBubbleEvt
    {
        public ulong id;
        public uint ownerType;  //0: npc  1:player
        public string content;
        public uint playInfoId;
        public uint npcInfoId;
        public float showTime;
        public GameObject gameObject;
        public uint bubbleId;
    }

    public class TriggerPlayerChatBubbleEvt
    {
        public ulong id;
        public ChatType chatType;
        public string content;
        public float showTime;
    }

    public class TriggerNpcBubbleEvt
    {
        public uint ownerType;  //0: npc  1:player
        public ulong npcid;
        public uint playInfoId;
        public uint npcInfoId;
        public uint bubbleid;
        public Action onComplete;
        public GameObject npcobj;
    }


    public class TriggerCutSceneBubbleEvt
    {
        public GameObject gameObject;
        public uint bubbleid;
        public Vector3 offest;
        public Action onComplete;
        public Camera camera;
    }


    public class TriggerAnimEvt
    {
        public uint id;                //战斗单位唯一id(被击方)
        public int finnaldamage;       //最终伤害值
        public int floatingdamage;     //属性加成值
        public AnimType AnimType;      //表现类型 
        public uint playType;           // 播放轨迹类型
        public int converCount = 1;    //合击次数，默认是一次
        public bool IsEnemy = false;
        public uint passiveId;     //被动技能名字
        public CombatUnitType attackType; //攻击方类型
        public CombatUnitType hitType;  //被击方类型
        public uint attackInfoId;       //攻击方配置id
        public uint hitInfoId;          //受击方id
        public uint race_attack;    //攻击方种族
        public uint race_hit;      //受击方种族

        public void Push()
        {
            CombatObjectPool.Instance.Push(this);
        }
    }

    public class TriggerSkillEvt
    {
        public uint id;                //战斗单位id
        public string skillcontent;    //技能内容
        public int clientNum;
    }


    [System.Flags]
    public enum AnimType
    {
        e_None = 0,
        e_Normal = 1,               //普攻
        e_Crit = 1 << 1,            //暴击
        e_AddHp = 1 << 2,           //加血
        e_Combo = 1 << 3,           //连击
        e_ConverAttack = 1 << 4,    //合击
        e_Miss = 1 << 5,            //特殊行为(闪避)
        e_Poison = 1 << 6,          //中毒
        e_Drunk = 1 << 7,           //醉酒
        e_Error = 1 << 8,           //特殊行为(失误)
        e_AddMp = 1 << 9,           //回蓝
        e_DeductMp = 1 << 10,       //扣蓝
        e_ExtractMp = 1 << 11,          //抽蓝
        e_PassiveDamage = 1 << 19,  //被动额外伤害&溅射伤害
        e_PassiveName = 1 << 20,    //被动名称
        e_MagicShort = 1 << 21,     //魔法不足
        e_EnergyShort = 1 << 22,    //能量不足
        e_Invalid = 1 << 24,        //未生效
    }

    [System.Flags]
    public enum EHudMoudle
    {
        e_Blood = 1,
        e_Bubble = 1 << 1,
        e_Anim = 1 << 2,
        e_Skill = 1 << 3,
        e_Actor = 1 << 4,
        e_Buff = 1 << 5,
    }

    /// <summary>
    /// 伤害类型表现表状态类 记录每个表现类型进行到了列表里哪个位置
    /// </summary>
    public class DamageShowTypeState
    {
        public uint ConfigId;
        public int Index = 0;
        public List<uint> TypeList;

        public uint GetCurId()
        {
            if (Index >= TypeList.Count)
            {
                Index = 0;
            }
            uint curId = TypeList[Index];
            Index++;
            return curId;
        }

        public void InitIndex()
        {
            Index = 0;
        }
    }


    public class Sys_HUD : SystemModuleBase<Sys_HUD>, ISystemModuleUpdate
    {
        public float DeltaTime { get { return GetDeltaTime(); } }

        public float UnscaledDeltaTime { get { return GetUnscaledDeltaTime(); } }


        public Dictionary<uint, GameObject> battleHuds = new Dictionary<uint, GameObject>(); //战斗内

        public Dictionary<uint, AttributeData> battleAttrs = new Dictionary<uint, AttributeData>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public List<EHudMoudle> eHudMoudles = new List<EHudMoudle>();

        public static string view_Battle;
        public static string view_Collect;
        public static string view_Game;
        public static string view_Survey;

        public static string view_Shop;
        public static string view_Cook;

        public Color current_ChatColor;
        public Color world_ChatColor;
        public Color team_ChatColor;
        public Color guild_ChatColor;
        public Color career_ChatColor;
        public Color braveTeam_ChatColor;

        public List<uint> mosterGroupIds = new List<uint>();
        public List<uint> battleTypes = new List<uint>();
        /// <summary>
        /// 伤害类型表现跳字状态  key:伤害类型表现表id | value:类型列表状态
        /// </summary>
        public Dictionary<uint, DamageShowTypeState> dictShowTypeListState = new Dictionary<uint, DamageShowTypeState>();
        public List<DamageShowTypeState> listShowTypeListState = new List<DamageShowTypeState>();
        /// <summary> 上一个被打对象id </summary>
        private  uint lastBeHitId = 0;
        private Timer showTypeTimer; 
        public enum EEvents
        {
            OnSetLayer,           //设置hud根节点层级
            OnRevertLayer,        //恢复hud根节点层级
            OnUpdateHUDMoudles,   //管理hud模块变化   

            OnCreateBlood,        //创建血条
            OnUpdateHp,           //更新hp
            OnUpdateMp,           //更新Mp
            OnUpdateShapeShift,   //更新变身状态
            OnUpdateShield,       //更新护盾值
            OnUpdateEnergy,       //更新能量值（气）
            OnUpdateArrow,        //更新战斗内单位头顶箭头(显示隐藏)
            OnUpdateSparSkill,        //更新战斗内主角能量点(显示隐藏)
            OnTriggerSecondAction,//触发二次行动
            OnShowOrHideBD,       //显示隐藏血条
            OnRemoveBattleUnit,   //移除战斗单位
            OnTriggerBattleBubble,//触发战斗内气泡
            OnTriggerAnim,        //跳字
            OnShowOrHideBuffHUD,  //失活或者激活buffhud
            OnUpdateBuffHUD,      //更新buffhud
            OnBuffHUDFalsh,       //buffhud闪烁
            OnClearBattleHUDs,    //清除战斗hud
            OnTriggerBattleInstructionFlag,//触发战斗指令标记
            OnClearBattleFlag,
            OnAddBattleOrder,     //战斗内技能轮播指令
            OnUndoBattleOrder,    //撤销战斗内技能轮播指令
            OnClearBattleOrder,   //清除战斗内技能轮播指令
            OnShowOrHideSelect,   //显示隐藏boss选择光圈
            OnSelected,           //显示隐藏boss选择光圈
            OnTriggerSkill,       //技能名称

            OnTriggerExpressionBubble, //触发表情气泡
            OnTriggerPlayerChatBubble,//聊天
            OnTriggerNpcBubble,   //触发npc气泡
            OnTriggerCutSceneBubble,//触发cutscene气泡
            OnCreateActorHUD,     //创建actorhud
            OnUpdateActorHUD,     //更新actorhud
            OnUpdateActorHUDTitleName,//更新actorhud名字
            OnUpdateActorName,      //更改角色名字
            OnRemoveActorHUD,     //移除actorhud
            OnActiveAllActorHUD,  //激活所有actorhud
            OnShowOrHideActorHUD, //显示隐藏某个actorhud
            OnShowOrHideActorsHUD, //显示隐藏一组actorhuds
            OnInActiveAllActorHUD,//失活所有actorhud
            OnClearActorFx,        //清除actor节点下 特效
            OnUpMount,             //上坐骑
            OnDownMount,            //下坐骑
            OnScaleUp,              //放大角色
            OnResetScale,           //重置角色
            OnCreateOrUpdateActorHUDStateFlag,  //创建或更新头顶状态
            OnClearActorHUDStateFlag,     //清除头顶状态
            OnClearActorHUDs,     //清除战斗外hud
            OnClearNpcBubbles,    //清除npc气泡
            OnClearCutSceenBubbles,//清除cutScene气泡
            OnUpdateTimeSand,     //刷新TimeSand
            OnCreateEmotion,      //创建心情表情
            OnClearEmotion,       //清除心情表情
            OnCreateTitle,        //添加称号
            OnClearTitle,         //隐藏称号
            OnUpdateFamilyTitleName,//更新家族称号
            OnUpdateBGroupTitleName,//更新勇者团称号
            OnUpdateFavirability, //更新好感度
            OnClearFavirability,  //去除好感度
            OnUpdateWorldBossHud, //更新世界boss
            OnClearWorldBossHud,  //清除世界boss
            OnUpdateSceneActorFightState,//更新场景actor战斗状态
            OnPlayActorLevelUpFx,  //表演等级提升特效
            OnPlayActorAdvanceUpFx,  //表演进阶提升特效
            OnPlayActorReputationUpFx,  //表演声望提升特效
            OnCreateNpcBattleCd,    //表现npc战斗倒计时
            OnUpdateHeroFunState,   //更新角色功能标识
            OnShowNpcArrow,         //显示npc箭头
            OnHideNpcArrow,         //隐藏npc箭头
            OnShowNpcSliderNotice,  //显示npc红色感叹号
            OnHideNpcSliderNotice,  //隐藏npc红色感叹号
            OnCreateTeamLogo,       //创建队标
            OnClearTeamLogo,        //删除队标
            OnCreateTeamFx,         //创建队标特效
            OnClearTeamFx,          //删除对标特效
            OnCreateFamilyBattle,   //进入家族资源战地图
            OnClearFamilyBattle,    //退出家族资源战地图
            OnUpdateFamilyName,     //更新家族名字(进入资源战之前)
            OnUpdateGuildBattleResource,//更新资源战矿图标
            OnUpdateFamilyTeamNum,      //更新家族资源战队伍人数
            OnUpdateGuildBattleName,//更新家族资源战头顶字色
            OnUpdateHeroName,       //更新角色头顶名字
        }

        public override void Init()
        {
            RegisterEvent(true);
            ParseData();
        }

        private void ParseData()
        {
            view_Battle = CSVParam.Instance.GetConfData(440).str_value;
            view_Game = CSVParam.Instance.GetConfData(441).str_value;
            view_Collect = CSVParam.Instance.GetConfData(442).str_value;
            view_Survey = CSVParam.Instance.GetConfData(443).str_value;
            view_Shop = CSVParam.Instance.GetConfData(461).str_value;
            view_Cook = CSVParam.Instance.GetConfData(460).str_value;

            Sys_Ini.Instance.Get<IniElement_IntArray>(1085, out IniElement_IntArray _current_ChatColor);
            current_ChatColor = new Color(_current_ChatColor.value[0] / 255f, _current_ChatColor.value[1] / 255f,
                _current_ChatColor.value[2] / 255f, _current_ChatColor.value[3] / 255f);

            Sys_Ini.Instance.Get<IniElement_IntArray>(1086, out IniElement_IntArray _world_ChatColor);
            world_ChatColor = new Color(_world_ChatColor.value[0] / 255f, _world_ChatColor.value[1] / 255f,
                _world_ChatColor.value[2] / 255f, _world_ChatColor.value[3] / 255f);

            Sys_Ini.Instance.Get<IniElement_IntArray>(1087, out IniElement_IntArray _team_ChatColor);
            team_ChatColor = new Color(_team_ChatColor.value[0] / 255f, _team_ChatColor.value[1] / 255f,
                _team_ChatColor.value[2] / 255f, _team_ChatColor.value[3] / 255f);

            Sys_Ini.Instance.Get<IniElement_IntArray>(1088, out IniElement_IntArray _guild_ChatColor);
            guild_ChatColor = new Color(_guild_ChatColor.value[0] / 255f, _guild_ChatColor.value[1] / 255f,
                _guild_ChatColor.value[2] / 255f, _guild_ChatColor.value[3] / 255f);

            Sys_Ini.Instance.Get<IniElement_IntArray>(1300, out IniElement_IntArray _career_ChatColor);
            career_ChatColor = new Color(_career_ChatColor.value[0] / 255f, _career_ChatColor.value[1] / 255f,
                _career_ChatColor.value[2] / 255f, _career_ChatColor.value[3] / 255f);

            Sys_Ini.Instance.Get<IniElement_IntArray>(1387, out IniElement_IntArray _braveTeam_ChatColor);
            braveTeam_ChatColor = new Color(_braveTeam_ChatColor.value[0] / 255f, _braveTeam_ChatColor.value[1] / 255f,
                _braveTeam_ChatColor.value[2] / 255f, _braveTeam_ChatColor.value[3] / 255f);

            battleTypes.Clear();
            mosterGroupIds.Clear();

            var aiBubbleChatDatas = CSVAiBubbleChat.Instance.GetAll();
            for (int i = 0, len = aiBubbleChatDatas.Count; i < len; i++)
            {
                CSVAiBubbleChat.Data cSVAiBubbleChatData = aiBubbleChatDatas[i];
                if (cSVAiBubbleChatData.type == 1)
                {
                    battleTypes.Add(cSVAiBubbleChatData.id);
                }
                else if (cSVAiBubbleChatData.type == 2)
                {
                    mosterGroupIds.Add(cSVAiBubbleChatData.id);
                }
            }

            var csvShowTypeDatas = CSVDamageShowType.Instance.GetAll();
            for (int i = 0; i < csvShowTypeDatas.Count; i++)
            {
                CSVDamageShowType.Data csvSTData = csvShowTypeDatas[i];
                DamageShowTypeState stateData = new DamageShowTypeState();
                stateData.ConfigId = csvSTData.id;
                stateData.TypeList = csvSTData.show_type;
                dictShowTypeListState.Add(csvSTData.id,stateData);
                listShowTypeListState.Add(stateData);
            }
        }

        public override void Dispose()
        {
            ClearBattleHUDs();
            ClearActorHUDs();
            RegisterEvent(false);
            lastBeHitId = 0;
            showTypeTimer?.Cancel();
        }

        private void RegisterEvent(bool toRegister)
        {
            Sys_CutScene.Instance.eventEmitter.Handle<uint>(Sys_CutScene.EEvents.OnLoaded, OnLoadedCutScene, toRegister);
        }

        public void OpenHud()
        {
            UIManager.OpenUI(EUIID.HUD);
            DebugUtil.LogFormat(ELogType.eHUD, "OpenUI(EUIID.HUD)");
        }

        public void CloseHud()
        {
            UIManager.CloseUI(EUIID.HUD, false, false);
            DebugUtil.LogFormat(ELogType.eHUD, "CloseUI(EUIID.HUD)");
        }

        private void OnLoadedCutScene(uint cutSceneId)
        {
            eventEmitter.Trigger(Sys_HUD.EEvents.OnInActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "OnLoadedCutScene.OnInActiveAllActorHUD");
            CSVCutScene.Data cSVCutSceneData = CSVCutScene.Instance.GetConfData(cutSceneId);
            if (cSVCutSceneData.isShowBubble)
            {
                OpenHud();
            }
            else
            {
                CloseHud();
            }
        }

        public void ClearBattleHUDs()
        {
            battleHuds.Clear();
            battleAttrs.Clear();
            eventEmitter.Trigger(EEvents.OnClearBattleHUDs);
        }

        public void ClearActorHUDs()
        {
            eventEmitter.Trigger(EEvents.OnClearActorHUDs);
        }

        public void SetNameRed(Text text, string content)
        {
            TextHelper.SetText(text, content, LanguageHelper.GetTextStyle(114));
        }

        public void SetOutFightNameText(EFightOutActorType eFightOutActorType, Text text, string content)
        {
            uint styleId = 0;
            string name = content;
            switch (eFightOutActorType)
            {
                case EFightOutActorType.None:
                    break;
                case EFightOutActorType.MainHero:
                case EFightOutActorType.Teammate:
                    styleId = Constants.OUTFIGHTHERONAMR;
                    break;
                case EFightOutActorType.OtherHero:
                    styleId = Constants.OUTFIGHTOTHERHERONAME;
                    break;
                case EFightOutActorType.Npc:
                case EFightOutActorType.Monster:
                    styleId = Constants.OUTFIGHTNPCNAME;
                    break;
                case EFightOutActorType.Partner:
                    break;
                default:
                    break;
            }
            
            TextHelper.SetText(text, name, LanguageHelper.GetTextStyle(styleId));
        }

        public void SetOutFightAppellationText(EFightOutActorType eFightOutActorType, Text text, uint lanId)
        {
            uint styleId = 0;
            switch (eFightOutActorType)
            {
                case EFightOutActorType.None:
                    break;
                case EFightOutActorType.MainHero:
                    break;
                case EFightOutActorType.Teammate:
                    break;
                case EFightOutActorType.OtherHero:
                    break;
                case EFightOutActorType.Npc:
                    styleId = Constants.OUTFIGHTNPCAPPEL;
                    break;
                case EFightOutActorType.Monster:
                    break;
                case EFightOutActorType.Partner:
                    break;
                default:
                    break;
            }
            if (eFightOutActorType == EFightOutActorType.Npc)
            {
                if (CSVNpcLanguage.Instance.GetConfData(lanId) == null)
                {
                    text.text = string.Empty;
                }
                else
                {
                    TextHelper.SetText(text, LanguageHelper.GetNpcTextContent(lanId, Sys_Role.Instance.Role.Name.ToStringUtf8()), LanguageHelper.GetTextStyle(styleId));
                }
            }
            else
            {
                if (CSVLanguage.Instance.GetConfData(lanId) == null)
                {
                    text.text = string.Empty;
                }
                else
                {
                    TextHelper.SetText(text, LanguageHelper.GetTextContent(lanId, Sys_Role.Instance.Role.Name.ToStringUtf8()), LanguageHelper.GetTextStyle(styleId));
                }
            }
        }

        public void SetFightNameText(EFightActorType eFightActorType, Text text, uint name)
        {
            uint styleId = 0;
            switch (eFightActorType)
            {
                case EFightActorType.None:
                    break;
                case EFightActorType.Hero:
                    styleId = Constants.INFIGHTHERONAME;
                    break;
                case EFightActorType.Pet:
                    styleId = Constants.INFIGHTHPETNAME;
                    break;
                case EFightActorType.Monster:
                    styleId = Constants.INFIGHTHMONSTERNAME;
                    break;
                case EFightActorType.Partner:
                    styleId = Constants.INFIGHTPARTNERNAME;
                    break;
                default:
                    break;
            }

            TextHelper.SetText(text, LanguageHelper.GetTextContent(name), LanguageHelper.GetTextStyle(styleId));
        }

        public void SetFightNameText(EFightActorType eFightActorType, Text text, string name)
        {
            uint styleId = 0;
            switch (eFightActorType)
            {
                case EFightActorType.None:
                    break;
                case EFightActorType.Hero:
                    styleId = Constants.INFIGHTHERONAME;
                    break;
                case EFightActorType.Pet:
                    styleId = Constants.INFIGHTHPETNAME;
                    break;
                case EFightActorType.Monster:
                    styleId = Constants.INFIGHTHMONSTERNAME;
                    break;
                case EFightActorType.Partner:
                    styleId = Constants.INFIGHTPARTNERNAME;
                    break;
                default:
                    break;
            }
            TextHelper.SetText(text, name, LanguageHelper.GetTextStyle(styleId));
        }

        public void AddHeroHUD(Hero hero)
        {
            CreateActorHUDEvt createActorHUDEvt = new CreateActorHUDEvt();
            createActorHUDEvt.id = hero.uID;
            createActorHUDEvt.gameObject = hero.gameObject;
            createActorHUDEvt.offest = new Vector3(0, 2.2f, 0);
            createActorHUDEvt.appellation = 0;
            createActorHUDEvt.name = hero.heroBaseComponent.Name;
            createActorHUDEvt.eFightOutActorType = EFightOutActorType.None;
            createActorHUDEvt.isBack = hero.heroBaseComponent.bIsReturn;
            if (Sys_Role.Instance.IsSelfRole(hero.UID))
            {
                createActorHUDEvt.eFightOutActorType = EFightOutActorType.MainHero;
            }
            else if (Sys_Team.Instance.isTeamMem(hero.uID))
            {
                createActorHUDEvt.eFightOutActorType = EFightOutActorType.Teammate;
            }
            else
            {
                createActorHUDEvt.eFightOutActorType = EFightOutActorType.OtherHero;
            }
            eventEmitter.Trigger<CreateActorHUDEvt>(EEvents.OnCreateActorHUD, createActorHUDEvt);


            //title
            if (hero.heroBaseComponent.TitleId != 0)
            {
                CreateTitleEvt createTitleEvt = new CreateTitleEvt();
                createTitleEvt.actorId = hero.uID;
                createTitleEvt.titleId = hero.heroBaseComponent.TitleId;
                if (hero.heroBaseComponent.TitleId == Sys_Title.Instance.familyTitle) 
                {
                    createTitleEvt.titleName = hero.heroBaseComponent.FamilyName;
                    createTitleEvt.pos = hero.heroBaseComponent.Pos;
                }
                else if (hero.heroBaseComponent.TitleId == Sys_Title.Instance.bGroupTitle)
                {
                    createTitleEvt.titleName = hero.heroBaseComponent.bGroupName;
                    createTitleEvt.pos = hero.heroBaseComponent.bGPos;
                }
                eventEmitter.Trigger<CreateTitleEvt>(EEvents.OnCreateTitle, createTitleEvt);
            }

            //teamLogo
            if (hero.heroBaseComponent.TeamLogeId != 0)
            {
                uint teamLogoId = hero.heroBaseComponent.TeamLogeId / 10;
                Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(EEvents.OnCreateTeamLogo, hero.UID, teamLogoId);
                if (hero.heroBaseComponent.TeamLogeId % 10 == 1)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnCreateTeamFx, hero.UID);
                }
                else if (hero.heroBaseComponent.TeamLogeId % 10 == 0)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearTeamFx, hero.UID);
                }
            }

            //scale
            if (hero.heroBaseComponent.Scale.y >= 1)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnScaleUp, hero.UID, (uint)(hero.heroBaseComponent.Scale.y * 100));
            }
            else if (hero.heroBaseComponent.Scale.y < 1)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnResetScale, hero.UID);
            }
        }


        public void CreatePlayerChatBubble(EFightActorType fightActorType, ulong id, string content, ChatType chatType)
        {
            if (chatType == ChatType.System)
            {
                return;
            }
            if (id == 0)//战斗内ai气泡
            {
                return;
            }
            if (GameCenter.mainHero != null && id == GameCenter.mainHero.uID)
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight)
                {
                    CreateOutFightChatBabble(id, content, chatType);
                }
                else
                {
                    CreateFightChatBabble(id, content, chatType);
                }
            }
            else
            {
                if (GameCenter.otherActorsDic.TryGetValue(id, out Hero otherHero) && otherHero != null)
                {
                    bool bOtherHeroInFight = false;
                    if (otherHero.heroBaseComponent != null)
                        bOtherHeroInFight = otherHero.heroBaseComponent.bInFight;

                    bool bMainHeroInFight = GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight;
                    if (bOtherHeroInFight != bMainHeroInFight)
                    {
                        return;
                    }
                    if (bMainHeroInFight)
                    {
                        CreateFightChatBabble(id, content, chatType);
                    }
                    else
                    {
                        CreateOutFightChatBabble(id, content, chatType);
                    }
                }
            }
        }

        private void CreateOutFightChatBabble(ulong id, string content, ChatType chatType)
        {
            TriggerPlayerChatBubbleEvt triggerPlayerChatBubbleEvt = new TriggerPlayerChatBubbleEvt();
            triggerPlayerChatBubbleEvt.id = id;
            triggerPlayerChatBubbleEvt.chatType = chatType;
            triggerPlayerChatBubbleEvt.content = content;
            triggerPlayerChatBubbleEvt.showTime = float.Parse(CSVParam.Instance.GetConfData(511).str_value) / 10000;
            Sys_HUD.Instance.eventEmitter.Trigger<TriggerPlayerChatBubbleEvt>(Sys_HUD.EEvents.OnTriggerPlayerChatBubble, triggerPlayerChatBubbleEvt);
        }

        private void CreateFightChatBabble(ulong id, string content, ChatType chatType)
        {
            if (Sys_Fight.Instance.FightHeros.TryGetValue(id, out FightHero fightHero))
            {
                TriggerBattleBubbleEvt tbb = CombatObjectPool.Instance.Get<TriggerBattleBubbleEvt>();
                tbb.battleid = fightHero.battleUnit.UnitId;
                tbb.bubbleid = 0;
                tbb.chatType = chatType;
                tbb.ClientNum = CombatHelp.ServerToClientNum(fightHero.battleUnit.Pos, CombatManager.Instance.m_IsNotMirrorPos);
                tbb.content = content;
                Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerBattleBubble, tbb);
                CombatObjectPool.Instance.Push(tbb);
            }
        }

        public void OnUpdate()
        {

        }

        /// <summary>
        /// 根据伤害类型表现id 获取跳字参数表id
        /// </summary>
        public uint GetDamageShowConfigIdByShowTypeId(uint showTypeId)
        {
            if (dictShowTypeListState.TryGetValue(showTypeId, out DamageShowTypeState stateData))
            {
                StartDamageShowTypeTimer();
                return stateData.GetCurId();
            }
            DebugUtil.Log(ELogType.eHUD, "CSVDamageShowType 伤害类型表现表没有id " + showTypeId.ToString());
            return 0;
        }
        /// <summary>
        /// 初始化伤害飘字队列
        /// </summary>
        public void InitDamageShowTypeState()
        {
            for (int i = 0; i < listShowTypeListState.Count; i++)
            {
                listShowTypeListState[i].InitIndex();
            }
        }
        /// <summary>
        /// 检测是否需要重置飘字位置队列
        /// </summary>
        /// <param name="curBeHitId"></param>
        public void CheckToInitDamageShowTypeState(uint curBeHitId)
        {
            if (curBeHitId != lastBeHitId)
            {
                InitDamageShowTypeState();
            }
            lastBeHitId = curBeHitId;
        }
        private void StartDamageShowTypeTimer()
        {
            showTypeTimer?.Cancel();
            var param = CSVParam.Instance.GetConfData(635);
            if (param != null)
            {
                float time = float.Parse(param.str_value) / 1000;
                showTypeTimer = Timer.Register(time, InitDamageShowTypeState);
            }
        }
    }

}


