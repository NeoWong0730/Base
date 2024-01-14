using Net;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using System;
using Lib.Core;
using Google.Protobuf.Collections;
using Table;
using Framework;


namespace Logic
{
    public enum EActivityTopic
    {
        None=0,
        PetExpediton=1,//宠物探险
        ItemExChange =2,//道具兑换
        ActivityMall =3,//特惠商店
        ActivitySign =4,//签到
        Fashion =5,//时装
        RedEnvelopeRain =6,//红包雨
        ActivitySavingsBank =7,//鼠王存钱罐
        TaskGuide=8,//节日任务引导
        CoinMall=9,//代币商城
    }
    public partial  class Sys_ActivityTopic : SystemModuleBase<Sys_ActivityTopic>, ISystemModuleUpdate
    {
        #region 系统函数

        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivitySignTakeReq, (ushort)CmdActivityRuler.CmdActivitySignTakeRes, OnLimitedActivitySignTakeRes, CmdActivitySignTakeRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivitySignTake2Req, (ushort)CmdActivityRuler.CmdActivitySignTake2Res, OnMergeServerSignTakeRes, CmdActivitySignTakeRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySignTakeRes, OnLimitedActivitySignTakeRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySignTake2Res, OnMergeServerSignTakeRes);
            }
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, OnUpdateActivityOperationRuler, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }

        public override void Init()
        {
            ProcessEvents(true);
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public void OnUpdate()
        {
            if (signRefreshTime == 0)
            {
                return;
            }
            var nowtime = Sys_Time.Instance.GetServerTime();
            if (nowtime > signRefreshTime&& !isDataReq)
            {
                OnLimitedActivityDataReq();
                isDataReq = true;
            }

        }
        public override void OnLogin()
        {
            isDataReq = false;
            isTaskGuide = true;
            signRefreshTime = 0;
        }
        public override void OnLogout()
        {
            base.OnLogout();
            ActivitySignDictionary.Clear();
            CommonActivityTimeDictionary.Clear();
        }
        #endregion
        #region net
        /// <summary>
        /// 检测刷新
        /// </summary>
        public void OnLimitedActivityDataReq()
        {
            CmdActivityDataReq req = new CmdActivityDataReq();
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityDataReq, req);

        }
        /// <summary>
        /// 领取请求
        /// </summary>
        /// <param name="_day"></param>
        public void OnLimitedActivitySignTakeReq(uint _day)
        {//这里回具体天数1-第一天 2-第二天
            CmdActivitySignTakeReq req = new CmdActivitySignTakeReq();
            req.SignDay = _day;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivitySignTakeReq, req);
        }
        /// <summary>
        /// 领取返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnLimitedActivitySignTakeRes(NetMsg msg)
        {
            CmdActivitySignTakeRes res = NetMsgUtil.Deserialize<CmdActivitySignTakeRes>(CmdActivitySignTakeRes.Parser, msg);
            if (ActivitySignDictionary.ContainsKey(0))
            {
                ActivitySignDictionary[0].UpdateServerList(res.SignDay);
            }
            eventEmitter.Trigger(EEvents.OnCommonActivityUpdate);
        }

        public void OnMergeServerSignTakeReq(uint _day)
        {//这里回具体天数1-第一天 2-第二天
            CmdActivitySignTake2Req req = new CmdActivitySignTake2Req();
            req.SignDay = _day;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivitySignTake2Req, req);
        }
        private void OnMergeServerSignTakeRes(NetMsg msg)
        {
            CmdActivitySignTake2Res res = NetMsgUtil.Deserialize<CmdActivitySignTake2Res>(CmdActivitySignTake2Res.Parser, msg);
            if (ActivitySignDictionary.ContainsKey(1))
            {
                ActivitySignDictionary[1].UpdateServerList(res.SignDay);
            }
            eventEmitter.Trigger(EEvents.OnCommonActivityUpdate);
        }
        #endregion
        #region DataInit
        private void OnUpdateActivityOperationRuler()
        {
            var aInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.LimitedActivityPanel);
            if (aInfo != null)
            {
                TopicId = aInfo.infoId;
                activityTimeList= ReturnDateList(aInfo.csvData.Begining_Date, aInfo.csvData.Duration_Day);
                prefabName = aInfo.csvData.PreformId;
                InitTopicActivityData();
            }

            ActivitySignDictionary.Clear();
            var ceil = new ActivitySignCeil();
            InitSignCeil(EActivityRulerType.LimitedActivitySign,ceil);
            ActivitySignDictionary.Add(0,ceil);
            ceil = new ActivitySignCeil();
            InitSignCeil(EActivityRulerType.MergeServerSign, ceil);
            ActivitySignDictionary.Add(1,ceil);
            eventEmitter.Trigger(EEvents.OnCommonActivityUpdate);
        }
        private void InitTopicActivityData()
        {
            CommonActivityTimeDictionary.Clear();
            var asDatas = CSVActivityUiJump.Instance.GetAll();
            for (int i = 0; i < asDatas.Count; i++)
            {
                if (asDatas[i].ActivityId == TopicId)
                {
                    var _dataSingle = asDatas[i];
                    var _preId = _dataSingle.PreformId;
                    if (_dataSingle.Begining_Date == 0)
                    {
                        CommonActivityTimeDictionary[_preId] = activityTimeList;
                    }
                    else
                    {
                        CommonActivityTimeDictionary[_preId] = ReturnDateList(_dataSingle.Begining_Date, _dataSingle.Duration_Day);
                    }
                    CommonActivityTimeDictionary[_preId].Add(_dataSingle.id);
                }
            }
        }
        public List<uint> ReturnDateList(uint _begin, uint _dura)
        {
            List<uint> _dlist = new List<uint>();
            _dlist.Add(_begin);
            _dlist.Add(_begin + _dura * 3600 * 24);
            return _dlist;
        }
        private void InitSignCeil(EActivityRulerType type, ActivitySignCeil ceil)
        {
            ActivityInfo aInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(type);
            if (aInfo != null)
            {
                ceil.IdOrType = aInfo.infoId;
                ceil.activityTimeList =ReturnDateList(aInfo.csvData.Begining_Date, aInfo.csvData.Duration_Day);
                ceil.InitSignCSVData();
            }
            else
            {
                ceil.IdOrType = 0;
            }
        }
        public void InitServerSign(uint id,ACSignData data)
        {
            if (data != null)
            {
                if (ActivitySignDictionary.ContainsKey(id))
                {
                    ActivitySignDictionary[id].IniteSignServerData(data.DayState);
                }
            }
        }

        #endregion
        #region Function
        public void TopicReturnTime(List<uint> list,ref DateTime start,ref DateTime end)
        {
            if (list.Count<2) return;
            start = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(list[0]));
            end = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(list[1]));
        }
        public bool CheckLimitedActivityOpen()
        {//检查活动是否开启
            if (!Sys_FunctionOpen.Instance.IsOpen(52401))
            {
                return false;
            }
            uint nowtime = Sys_Time.Instance.GetServerTime();
            if (TopicId !=0)
            {
                nowMenuTime = activityTimeList;
                menuId = TopicId;
                if (nowMenuTime.Count>=2&&nowtime < TimeManager.ConvertFromZeroTimeZone(nowMenuTime[1]) )
                {
                    return true;
                }

            }
            if(ActivitySignDictionary.ContainsKey(0))
            {
                nowMenuTime = ActivitySignDictionary[0].activityTimeList;
                menuId = ActivitySignDictionary[0].IdOrType;
                if (nowMenuTime.Count >= 2&&nowtime < TimeManager.ConvertFromZeroTimeZone(nowMenuTime[1]))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckLimitedActivityShow()
        {//检查活动界面是否显示
            if (!CheckLimitedActivityOpen())
            {
                return false;
            }
            uint nowtime = Sys_Time.Instance.GetServerTime();
            if (nowtime >= TimeManager.ConvertFromZeroTimeZone(nowMenuTime[0]) && nowtime <= TimeManager.ConvertFromZeroTimeZone(nowMenuTime[1]))
            {
                return true;
            }

            return false;
        }
        public bool CheckPanelActivityOpen(uint _id)
        {//检查主题面板上的活动时间

            if (CommonActivityTimeDictionary.ContainsKey(_id))
            {
                uint nowtime = Sys_Time.Instance.GetServerTime();
                if (nowtime >= TimeManager.ConvertFromZeroTimeZone(CommonActivityTimeDictionary[_id][0]))
                {
                    return true;
                }

            }
            return false;
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            eventEmitter.Trigger(EEvents.OnCommonActivityUpdate);

        }
        /// <summary>
        /// 检测 通用活动 第一次弹出
        /// </summary>
        /// <returns></returns>
        public bool CheckCommonActivityFaceTips()
        {
            if (TopicId==0)
            {
                return false;
            }
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "CommonActivity" + TopicId.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 通用活动 第一次弹出
        /// </summary>
        public void SetCommonActivityFaceTip()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "CommonActivity" + TopicId.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
            }
        }
        public GameObject CreateActivityCellGameobject(string prefabNode)
        {
            Transform transform = UIManager.GetUI((int)EUIID.UI_Activity_Topic).transform;
            var goList = transform.GetComponent<AssetDependencies>().mCustomDependencies;
            Transform tParent = transform.Find("Animator/Content");
            for (int i = 0; i < goList.Count; i++)
            {
                var prefabCell = goList[i] as GameObject;
                if (prefabCell.name == prefabNode)
                {
                    var goChild = FrameworkTool.CreateGameObject(prefabCell, tParent.gameObject);
                    return goChild;
                }
            }
            return null;
        }
        public bool CheckActivitySignRedPoint(uint id)
        {//id-0-普通1-合服
            if (!CheckLimitedActivityOpen())
            {
                return false;
            }
            if (ActivitySignDictionary.ContainsKey(id))
            {
                return ActivitySignDictionary[id].CheckSignRedPoint();
            }
            return false;
        }
        public bool CheckLimitedActivityRedPoint()
        {
            if (!CheckLimitedActivityOpen())return false;
           
            if (TopicId==menuId)
            {
                foreach (var item in CommonActivityTimeDictionary)
                {
                    if (RedPointShow((EActivityTopic)item.Key))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return CheckActivitySignRedPoint(0);
            }
            return false;
        }
        /// <summary>
        /// 检测任务引导红点
        /// </summary>
        /// <returns></returns>
        private bool CheckTaskGuideRedPoint()
        {
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            if (Sys_Role.Instance.lastLoginTime <= zeroTime)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 活动紧急开关
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public bool CheckLimitedActivitySwitch(EActivityTopic _index)
        {
            switch (_index)
            {
                case EActivityTopic.PetExpediton:
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(208))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021134));
                        return false;
                    }
                    break;
                case EActivityTopic.ItemExChange:
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(211))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028103));
                        return false;
                    }
                    break;
                case EActivityTopic.ActivityMall:
                case EActivityTopic.CoinMall:
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(210))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028105));
                        return false;
                    }
                    break;
                case EActivityTopic.ActivitySign:
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(209))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028101));
                        return false;
                    }
                    break;
                case EActivityTopic.RedEnvelopeRain:
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(204))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021134));
                        return false;
                    }
                    break;
                case EActivityTopic.ActivitySavingsBank:
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(213))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021134));
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
        public bool RedPointShow(EActivityTopic _id)
        {
            bool isRed = false;
            switch (_id)
            {
                case EActivityTopic.PetExpediton:
                    isRed = Sys_PetExpediton.Instance.CheckAllRedPoint();
                    break;
                case EActivityTopic.ItemExChange:
                    isRed = Sys_ItemExChange.Instance.hasRed() || Sys_ActivityQuest.Instance.hasRed();
                    break;
                case EActivityTopic.ActivitySign:
                    isRed = CheckActivitySignRedPoint(0);
                    break;
                case EActivityTopic.Fashion:
                    isRed = Sys_Fashion.Instance.freeDraw;
                    break;
                case EActivityTopic.ActivitySavingsBank:
                    isRed = Sys_ActivitySavingBank.Instance.CheckRedPoint();
                    break;
                case EActivityTopic.TaskGuide:
                    isRed = CheckTaskGuideRedPoint()&&isTaskGuide && CheckPanelActivityOpen((uint)EActivityTopic.TaskGuide);
                    break;
                default:
                    break;
            }
            return isRed;
        }
        #endregion
    }
    public partial class Sys_ActivityTopic:SystemModuleBase<Sys_ActivityTopic>
    {
        /// <summary>
        /// 主题活动id
        /// </summary>
        public uint TopicId
        {
            get;
            private set;
        } = 0;
        /// <summary>
        /// 存下主界面使用的活动id
        /// </summary>
        public uint menuId
        {
            get;
            private set;
        } = 0;
        public bool isTaskGuide
        {
            get;
            set;
        }
        public uint signRefreshTime
        {
            get;
            set;
        }

        public bool isDataReq
        {
            get;
            set;
        }
        public List<uint> activityTimeList;//主题活动时间表[0]-开始时间，[1]-结束时间
        public string prefabName;//主题活动面板预制体名
        public Dictionary<uint, List<uint>> CommonActivityTimeDictionary = new Dictionary<uint, List<uint>>();//运营活动ui表preformId-0开启时间/1结束时间/2-该行uid

        public Dictionary<uint, ActivitySignCeil> ActivitySignDictionary = new Dictionary<uint, ActivitySignCeil>();//签到数据 0-普通 1-合服
        public List<uint> nowMenuTime;//menu使用时间表
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            OnCommonActivityUpdate = 0,
        }
    }
    public class ActivitySignCeil
    {
        public uint IdOrType;//活动id
        public List<uint> activityTimeList;//[0]-开始时间，[1]-结束时间
        public uint signTextureId;//签到周期第一天后面如果配了背景图，则存下该行id
        public List<uint> MergeServerDateList = new List<uint>();//运营活动签到表
        public List<uint> MergeServerSignList = new List<uint>();//服务器签到数据-是否可领取
        public void InitSignCSVData()
        {
            MergeServerDateList.Clear();
            var sData = CSVSignActivity.Instance.GetAll();
            for (int i = 0; i < sData.Count; i++)
            {
                if (sData[i].Activiyid == IdOrType)
                {
                    MergeServerDateList.Add(sData[i].itemId);
                    if (sData[i].Image1 != string.Empty)
                    {
                        signTextureId = sData[i].id;
                    }

                }
            }
        }
        public void IniteSignServerData(RepeatedField<uint> _dayState)
        {
            MergeServerSignList.Clear();
            for (int i = 0; i < _dayState.Count; i++)
            {
                MergeServerSignList.Add(_dayState[i]);
            }
        }
        public void UpdateServerList(uint id)
        {
            if (IdOrType == 0 || id > MergeServerSignList.Count)
                return;

            MergeServerSignList[(int)id - 1] = 2;
        }
        public bool CheckSignRedPoint()
        {
            for (int i = 0; i < MergeServerSignList.Count; i++)
            {
                if (MergeServerSignList[i] == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

