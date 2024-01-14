using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework;
using System.Collections.Generic;


namespace Logic
{
    public class UI_Activity_Summer : UIBase
    {
        protected class ActivityButtonCeil
        {
            public Button i_button;
            public Timer i_Timer;
            public ActivityButtonCeil(Button _button)
            {
                i_button = _button;
                i_Timer?.Cancel();
            }
        }
        #region 界面组件
        private Button btn_Close;
        private Text txt_Time;
        private Timer m_Timer;

        private Dictionary<uint, ActivityButtonCeil> m_BtnDictionary = new Dictionary<uint, ActivityButtonCeil>();
        #endregion
        #region 系统函数
        public void Init(Transform transform)
        {
            m_BtnDictionary.Clear();
            btn_Close = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            txt_Time = transform.Find("Animator/Text_Time").GetComponent<Text>();
            Transform tr = transform.Find("Animator");
            int j = 1;
            for (int i = 0; i < tr.childCount; i++)
            {
                string _btnNmae = string.Format("Btn{0}", j);
                GameObject go = tr.GetChild(i).gameObject;
                if (go.name == _btnNmae)
                {
                    var _indexId = j;
                    Button button_Single = go.GetComponent<Button>();
                    ActivityButtonCeil _abtn = new ActivityButtonCeil(button_Single);
                    m_BtnDictionary.Add((uint)_indexId, _abtn);
                    button_Single.onClick.AddListener(() => { OnClick_Button(_indexId); });
                    j++;
                }
            }

        }
        protected override void OnLoaded()
        {
            Init(transform);
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivitySummer.Instance.eventEmitter.Handle(Sys_ActivitySummer.EEvents.OnSummerDateRefresh, RefreshPanelShow, toRegister);
            Sys_PetExpediton.Instance.eventEmitter.Handle(Sys_PetExpediton.EEvents.OnPetExpeditonDataUpdate, InitButtonShow, toRegister);
            Sys_ItemExChange.Instance.eventEmitter.Handle(Sys_ItemExChange.EEvents.e_UpdateRedState, RefreshRedPointShow, toRegister);
            Sys_ActivityQuest.Instance.eventEmitter.Handle(Sys_ActivityQuest.EEvents.e_UpdateRedState, RefreshRedPointShow, toRegister);
            Sys_ActivitySavingBank.Instance.eventEmitter.Handle(Sys_ActivitySavingBank.EEvents.OnRefreshRedPoint, RefreshRedPointShow, toRegister);
        }
        protected override void OnShow()
        {
            RefreshPanelShow();
        }
        protected override void OnHide()
        {
            m_Timer?.Cancel();
            foreach (var item in m_BtnDictionary)
            {
                item.Value.i_Timer?.Cancel();
            }
        }
        private void RefreshPanelShow()
        {
            DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            DateTime _endTime = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_ActivitySummer.Instance.LimitActivityDictinary[0][1]));
            if (nowtime > _endTime)
            {
                UIManager.CloseUI(EUIID.UI_Activity_Summer);
                return;
            }
            TimeSpan _sp = _endTime.Subtract(nowtime);
            m_Timer?.Cancel();
            m_Timer = Timer.Register((float)_sp.TotalSeconds, () =>
            {
                Sys_ActivitySummer.Instance.eventEmitter.Trigger(0);
                UIManager.CloseUI(EUIID.UI_Activity_Summer);
            }, null, true);
            InitButtonShow();
            txt_Time.text = LastTimeShow();
        }
        private void InitButtonShow()
        {
            foreach (var item in m_BtnDictionary)
            {
                ButtonDataInit(item.Value.i_button);
                RedPointShow(item.Key, item.Value.i_button);
                List<uint> _timelist = Sys_ActivitySummer.Instance.LimitAllActivityTimeDic[item.Key];
                if (_timelist != null)
                {
                    DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
                    if (nowtime <TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_timelist[0])))
                    {
                        item.Value.i_button.gameObject.transform.Find("Image_Lock").gameObject.SetActive(true);
                        
                    }
                    else if (nowtime > TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_timelist[1])))
                    {
                        item.Value.i_button.transform.Find("Image_End").gameObject.SetActive(true);
                        item.Value.i_button.enabled = false;
                    }
                    else
                    {
                        TimeSpan _sp = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_timelist[1])).Subtract(nowtime);
                        item.Value.i_Timer?.Cancel();
                        item.Value.i_Timer = Timer.Register((float)_sp.TotalSeconds, () =>
                        {
                            InitButtonShow();
                        }, null, true);
                    }

                }
                else
                {
                    item.Value.i_button.transform.Find("Image_End").gameObject.SetActive(true);
                    item.Value.i_button.enabled = false;
                }
            }
            SpecialFashion();
        }

        private void RefreshRedPointShow()
        {
            foreach (var item in m_BtnDictionary)
            {
                RedPointShow(item.Key, item.Value.i_button);
            }
        }

        private void SpecialFashion()
        {
            if (Sys_Fashion.Instance.activeId == 0)
            {
                m_BtnDictionary[5].i_button.gameObject.transform.Find("Image_Lock").gameObject.SetActive(true);
                m_BtnDictionary[5].i_button.gameObject.transform.Find("Dot").gameObject.SetActive(false);
            }
        }
        private void ButtonDataInit(Button _btn)
        {
            _btn.transform.Find("Image_End").gameObject.SetActive(false);
            _btn.transform.Find("Image_Lock").gameObject.SetActive(false);
            _btn.transform.Find("Dot").gameObject.SetActive(false);
        }
        private void RedPointShow(uint _id, Button _btn)
        {
            bool isRed = false;
            switch (_id)
            {
                case 1:
                    isRed = Sys_PetExpediton.Instance.CheckAllRedPoint();
                    break;
                case 2:
                    isRed = Sys_ItemExChange.Instance.hasRed() || Sys_ActivityQuest.Instance.hasRed();
                    break;
                case 3:
                    break;
                case 4:
                    isRed = Sys_ActivitySummer.Instance.CheckActivitySignRedPoint();
                    break;
                case 5:
                    isRed = Sys_Fashion.Instance.freeDraw;
                    break;
                case 6://红包雨
                    break;
                case 7://鼠王存钱
                    isRed = Sys_ActivitySavingBank.Instance.CheckRedPoint();
                    break;
            }
            _btn.transform.Find("Dot").gameObject.SetActive(isRed);
        }
        private string LastTimeShow()
        {
            var _timelist = Sys_ActivitySummer.Instance.GetActivityTime(0);
            DateTime _start = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_timelist[0]));
            DateTime _end = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_timelist[1]));
            string _str = LanguageHelper.GetTextContent(591000705, _start.Year.ToString(), _start.Month.ToString(), _start.Day.ToString(), FormatString(_start.Hour), FormatString(_start.Minute), _end.Year.ToString(), _end.Month.ToString(), _end.Day.ToString(), FormatString(_end.Hour), FormatString(_end.Minute));
            return _str;

        }

        private string FormatString(int _time)
        {
            return string.Format("{0:00}", _time);
        }

        #endregion
        #region 按钮
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Activity_Summer);
        }
        private void OnClick_Button(int _id)
        {
            if (!Sys_ActivitySummer.Instance.CheckLimitedActivitySwitch(_id))
            {
                return;
            }
            if (!Sys_ActivitySummer.Instance.CheckPanelActivityOpen((uint)_id))
            {
                var _Date = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_ActivitySummer.Instance.LimitAllActivityTimeDic[(uint)_id][0]));
                string strTime = LanguageHelper.TimeToString((uint)Sys_ActivitySummer.Instance.ConvertDateTimeToUtc_10(_Date), LanguageHelper.TimeFormat.Type_7);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000706,strTime));
                return;
            }
            CSVActivityUiJump.Data _idata = CSVActivityUiJump.Instance.GetConfData((uint)_id);
            if (_idata != null)
            {
                if (_idata.UiParam != 0)
                    UIManager.OpenUI((EUIID)_idata.UiId, false, _idata.UiParam);
                else
                    UIManager.OpenUI((EUIID)_idata.UiId);
            }

            UIManager.HitButton(EUIID.UI_Activity_Summer, _id.ToString());
        }
        #endregion
    }
    public class SummerSignCeil
    {
        private GameObject go_Prop;
        private Text txt_Name;
        private GameObject go_Select;
        private GameObject go_Get;
        private Button btn_Click;
        private int m_Id;
        private uint SignState = 0;
        private bool isEmergency;
        public SummerSignCeil(int _id)
        {
            m_Id = _id;//从0计数
            SignState = Sys_ActivitySummer.Instance.LimitedSevenDaySignList[m_Id];
        }

        public void Init(Transform transform)
        {
            btn_Click = transform.GetComponent<Button>();
            btn_Click.onClick.RemoveAllListeners();
            btn_Click.onClick.AddListener(On_ClickSignCeil);
            go_Prop = transform.Find("PropItem").gameObject;
            txt_Name = transform.Find("Text_Name").GetComponent<Text>();
            go_Select = transform.Find("Image_Select").gameObject;
            go_Get = transform.Find("Image_Get").gameObject;
        }
        public void SetData(uint _dropId)
        {
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(_dropId);
            if (list_drop.Count > 0)
            {
                PropItem _propItem = new PropItem();
                _propItem.BindGameObject(go_Prop);
                ItemIdCount itemIdCount = list_drop[0];
                _propItem.SetData(new MessageBoxEvt(EUIID.UI_Activity_SummerSign, new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));

                CSVItem.Data iData = CSVItem.Instance.GetConfData(itemIdCount.id);
                txt_Name.text = LanguageHelper.GetTextContent(iData.name_id);
            }
            SetState(SignState);
            //btn_Click.enabled =(SignState==1);
        }
        private void On_ClickSignCeil()
        {
            if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(209))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028102));
                return;
            }
            if (SignState == 1)
            {
                var _day = m_Id + 1;
                Sys_ActivitySummer.Instance.OnLimitedActivitySignTakeReq((uint)_day);
            }
        }

        private void SetState(uint _sa)
        {
            go_Get.SetActive(false);
            go_Select.SetActive(false);
            if (_sa == 1)
            {
                go_Select.SetActive(true);
            }
            else if (_sa == 2)
            {
                go_Get.SetActive(true);
            }

        }

        public void RefreshCeilInfo(uint _state)
        {
            SignState = _state;
            SetState(SignState);
        }

    }
    public class UI_Activity_SummerSign : UIBase
    {
        #region 界面组件
        private Button btn_Close;
        private Text txt_LastTime;

        private List<GameObject> list_SignItem = new List<GameObject>();
        private List<SummerSignCeil> list_SignCeil = new List<SummerSignCeil>();
        private uint m_signID;
        private DateTime signStartTime;
        private DateTime signEndTime;
        private DateTime signRefreshDate;
        private bool isDataReq = false;
        private string lastTimeText;
        #endregion
        #region 系统函数
        public void Init(Transform transform)
        {
            btn_Close = transform.Find("Animator/Image_bg01/View_Title07/Btn_Close").GetComponent<Button>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            txt_LastTime = transform.Find("Animator/Image_bg01/Image_Time/Num").GetComponent<Text>();
            Transform tr = transform.Find("Animator/Image_bg01");
            int j = 1;
            list_SignItem.Clear();
            for (int i = 0; i < tr.childCount; i++)
            {
                GameObject go = tr.GetChild(i).gameObject;
                string go_name = string.Format("Btn_0{0}", j);
                if (go.name == go_name)
                {
                    list_SignItem.Add(go);
                    j++;
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivitySummer.Instance.eventEmitter.Handle(Sys_ActivitySummer.EEvents.OnSummerDateRefresh, OnFreshAllCeil, toRegister);
            //Sys_ActivitySummer.Instance.eventEmitter.Handle<uint>(Sys_ActivitySummer.EEvents.OnSummerActivitySignTake, OnSignCeilRes, toRegister);

        }
        protected override void OnLoaded()
        {
            Init(transform);
            if (Sys_ActivitySummer.Instance.LimitedSevenDaySignList.Count == 0)
            {
                DebugUtil.LogError("无活动签到数据");
                UIManager.CloseUI(EUIID.UI_Activity_SummerSign);
                return;
            }

        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            LastTimeTextShow();

        }

        protected override void OnShow()
        {
            SignPanelShow();
        }

        protected override void OnHide()
        {
        }
        #endregion
        #region Function

        private void SignPanelShow()
        {
            InitData();
            SetSignItemShow();
            LastTimeTextShow();
        }
        private void InitData()
        {
            isDataReq = false;
            signStartTime = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_ActivitySummer.Instance.LimitActivityDictinary[1][0]));
            signEndTime = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_ActivitySummer.Instance.LimitActivityDictinary[1][1]));
            signRefreshDate = TimeManager.GetDateTime(Sys_ActivitySummer.Instance.signRefreshTime);
            lastTimeText = StringReturn();
        }

        private void SetSignItemShow()
        {
            list_SignCeil.Clear();
            for (int i = 0; i < list_SignItem.Count; i++)
            {
                SummerSignCeil s_ceil = new SummerSignCeil(i);
                s_ceil.Init(list_SignItem[i].transform);
                s_ceil.SetData(Sys_ActivitySummer.Instance.dateList[i]);
                list_SignCeil.Add(s_ceil);
            }

        }
        private void OnFreshAllCeil()
        {
            SignPanelShow();
        }

        private void LastTimeTextShow()
        {
            DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            if (nowtime > signEndTime)
            {
                UIManager.CloseUI(EUIID.UI_Activity_SummerSign);
                return;
            }
            if (nowtime > signRefreshDate && !isDataReq)
            {
                Sys_ActivitySummer.Instance.OnLimitedActivityDataReq();
                isDataReq = true;
            }
            TimeSpan sp = signEndTime.Subtract(nowtime);
            txt_LastTime.text = string.Format(lastTimeText, sp.Days.ToString(), sp.Hours.ToString(), sp.Minutes.ToString());
        }

        private string StringReturn()
        {
            StringBuilder strb = new StringBuilder();
            strb.Append("{0}").Append(LanguageHelper.GetTextContent(591000702)).Append("{1}").Append(LanguageHelper.GetTextContent(591000703)).Append("{2}").Append(LanguageHelper.GetTextContent(591000704));
            return strb.ToString();

        }
        #endregion
        #region Button

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Activity_SummerSign);
        }
        #endregion


    }
}



