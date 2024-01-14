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
    public class Sys_ActivitySummer : SystemModuleBase<Sys_ActivitySummer>
    {

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public Dictionary<uint, List<uint>> LimitActivityDictinary = new Dictionary<uint, List<uint>>();//类型0-活动，1-签到/[0]-开始时间，[1]-结束时间

        public List<uint> LimitedSevenDaySignList = new List<uint>();//服务器签到数据-是否可领取

        public Dictionary<uint, List<uint>> LimitAllActivityTimeDic = new Dictionary<uint, List<uint>>();//运营活动ui表id-0开启时间/1结束时间

        public List<uint> dateList = new List<uint>();//运营活动签到表

        public uint signRefreshTime;


        public enum EEvents
        {
            OnSummerDateRefresh = 0,
        }
        #region 系统函数

        private void ProcessEvents(bool toRegister)
        {
            //if (toRegister)
            //{
            //    EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivitySignTakeReq, (ushort)CmdActivityRuler.CmdActivitySignTakeRes, OnLimitedActivitySignTakeRes, CmdActivitySignTakeRes.Parser);
            //}
            //else
            //{
            //    EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySignTakeRes, OnLimitedActivitySignTakeRes);
            //}
            //Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, OnUpdateActivityOperationRuler, toRegister);
            //Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }

        public override void Init()
        {
            ProcessEvents(true);
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public override void OnLogin()
        {

        }
        public override void OnLogout()
        {
            base.OnLogout();
            LimitActivityDictinary.Clear();
            LimitedSevenDaySignList.Clear();
            LimitAllActivityTimeDic.Clear();
            dateList.Clear();
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
            LimitedSevenDaySignList[(int)res.SignDay - 1] = 2;
            eventEmitter.Trigger(EEvents.OnSummerDateRefresh);
        }
        #endregion
        #region Function
        public void InitLimitedSevenDaySignList(RepeatedField<uint> _dayState)
        {
            LimitedSevenDaySignList.Clear();
            for (int i = 0; i < _dayState.Count; i++)
            {
                LimitedSevenDaySignList.Add(_dayState[i]);
            }
        }
        public List<uint> GetActivityTime(uint _type)
        {
            if (LimitActivityDictinary.TryGetValue(_type, out List<uint> _list))
            {
                return _list;
            }
            return null;
        }
        private void InitSummerActivityData()
        {
            LimitAllActivityTimeDic.Clear();
            var asDatas = CSVActivityUiJump.Instance.GetAll();
            for (int i = 0; i < asDatas.Count; i++)
            {
                var _dataSingle = asDatas[i];
                if (_dataSingle.Begining_Date == 0)
                {
                    LimitAllActivityTimeDic[_dataSingle.id] = LimitActivityDictinary[0];
                }
                else
                {
                    LimitAllActivityTimeDic[_dataSingle.id] = ReturnDateList(_dataSingle.Begining_Date, _dataSingle.Duration_Day);

                }

            }
        }

        private List<uint> ReturnDateList(uint _begin, uint _dura)
        {
            List<uint> _dlist = new List<uint>();
            //DateTime startT = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_begin));
            //DateTime endT = startT.AddDays(_dura);
            _dlist.Add(_begin);
            _dlist.Add(_begin + _dura * 3600 * 24);
            return _dlist;
        }
        public long ConvertDateTimeToUtc_10(DateTime _dt)
        {
            TimeSpan _SP = _dt - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(_SP.TotalSeconds);
        }
        private void InitSignCSVData()
        {
            dateList.Clear();
            var sData = CSVSignActivity.Instance.GetAll();
            for (int i = 0; i < sData.Count; i++)
            {
                dateList.Add(sData[i].itemId);
            }
        }
        public bool CheckLimitedActivityOpen(uint _id)
        {//检查活动是否开启 0活动界面 1签到界面
            if (!Sys_FunctionOpen.Instance.IsOpen(52401))
            {
                return false;
            }
            if (LimitActivityDictinary.ContainsKey(_id))
            {
                DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
                if (nowtime < TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(LimitActivityDictinary[_id][1])))
                {
                    return true;
                }

            }
            return false;
        }
        public bool CheckPanelActivityOpen(uint _id)
        {//检查面板活动

            if (LimitAllActivityTimeDic.ContainsKey(_id))
            {
                DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
                if (nowtime >= TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(LimitAllActivityTimeDic[_id][0])))
                {
                    return true;
                }

            }
            return false;
        }
        public bool CheckActivitySignRedPoint()
        {
            if (!CheckLimitedActivityOpen(1))
            {
                return false;
            }
            for (int i = 0; i < LimitedSevenDaySignList.Count; i++)
            {
                if (LimitedSevenDaySignList[i] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckLimitedActivityRedPoint()
        {
            if (!CheckLimitedActivityOpen(0))
            {
                return false;
            }
            bool itemBool = Sys_ItemExChange.Instance.hasRed() || Sys_ActivityQuest.Instance.hasRed();
            return CheckActivitySignRedPoint() || Sys_PetExpediton.Instance.CheckAllRedPoint() || Sys_Fashion.Instance.freeDraw || itemBool || Sys_ActivitySavingBank.Instance.CheckRedPoint();
        }
        public bool CheckLimitedActivityShow()
        {//检查活动界面是否显示
            if (!CheckLimitedActivityOpen(0))
            {
                return false;
            }
            DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            if (nowtime >= TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(LimitActivityDictinary[0][0])) && nowtime <= TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(LimitActivityDictinary[0][1])))
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
        public bool CheckLimitedActivitySwitch(int _index)
        {

            switch (_index)
            {
                case 1://宠物探险
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(208))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021134));
                        return false;
                    }
                    break;
                case 2://道具兑换
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(211))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028103));
                        return false;
                    }
                    break;
                case 3://折扣商店
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(210))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028105));
                        return false;
                    }
                    break;
                case 4://签到
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(209))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028101));
                        return false;
                    }
                    break;
                case 6://红包雨
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(204))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028101));
                        return false;
                    }
                    break;
                case 7://鼠王抽奖
                    if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(213))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028101));
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
        private void OnUpdateActivityOperationRuler()
        {
            for (uint i = 10; i <= 11; i++)
            {
                var _type = (EActivityRulerType)i;
                ActivityInfo aInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(_type);
                if (aInfo != null)
                {
                    var _index = i % 10;
                    LimitActivityDictinary[_index] = ReturnDateList(aInfo.csvData.Begining_Date, aInfo.csvData.Duration_Day);
                }
            }
            InitSummerActivityData();
            InitSignCSVData();
            OnLimitedActivityDataReq();
            eventEmitter.Trigger(EEvents.OnSummerDateRefresh);
        }

        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            eventEmitter.Trigger(EEvents.OnSummerDateRefresh);

        }
        /// <summary>
        /// 检测 2022夏日活动 第一次弹出
        /// </summary>
        /// <returns></returns>
        public bool CheckSummerActivityFaceTips()
        {

            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "SummerActivity2022FaceTip";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 2022夏日活动 第一次弹出
        /// </summary>
        public void SetSummerActivityFaceTip()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "SummerActivity2022FaceTip";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
            }
        }

        #endregion

    }

}

