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
    public class UI_Activity_Topic : UIBase
    {
        private UI_ActivityCommon uiComponent;
        private Button btn_Close;
        private Canvas BtnCloseCanvas;
        private int sortLayer;
        #region 系统函数
        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            BtnCloseCanvas=transform.Find("Animator/View_Title07").GetComponent<Canvas>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            SetPanelPrefabe();
        }
        protected override void OnDestroy()
        {
            if (uiComponent != null)
            {
                uiComponent.Hide();
            } 
        }

        protected override void OnShow()
        {
            if (uiComponent!=null)
            {
                uiComponent.RefreshRedPointShow();
            }
        }
        protected override void OnUpdate()
        {
            sortLayer = this.transform.GetComponent<Canvas>().sortingOrder;
            if (BtnCloseCanvas!=null)
            {
                BtnCloseCanvas.sortingOrder = sortLayer + 1;
            }
            if (uiComponent!=null)
            {
                uiComponent.transform.GetComponent<Canvas>().sortingOrder = sortLayer-50;
            }
            
        }
        protected override void OnHide()
        {

        }
        private void SetPanelPrefabe()
        {
            var m_PrefabName = Sys_ActivityTopic.Instance.prefabName;
            if (m_PrefabName == string.Empty)
            {
                UIManager.CloseUI(EUIID.UI_Activity_Topic);
                Debug.LogError("Table PrefabName Is NULL!");
                return;
            }
            if (uiComponent == null)
            {
                uiComponent = new UI_ActivityCommon();
                Transform uiTran = Sys_ActivityTopic.Instance.CreateActivityCellGameobject(m_PrefabName).transform;
                uiComponent.Init(uiTran.transform);
                uiComponent.Show();
            }
        }

        #endregion

        #region 按钮
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Activity_Topic);
        }
        #endregion
    }
    public class UI_ActivityCommon : UIComponent
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
        private Text txt_Time;
        private Timer m_Timer;

        private Dictionary<EActivityTopic, ActivityButtonCeil> m_BtnDictionary = new Dictionary<EActivityTopic, ActivityButtonCeil>();
        #endregion
        #region 系统函数
        protected override void Loaded()
        {
            m_BtnDictionary.Clear();
            txt_Time = transform.Find("Text_Time").GetComponent<Text>();
            Transform tr = this.transform;
            foreach (var item in Sys_ActivityTopic.Instance.CommonActivityTimeDictionary)
            {
                for (int i = 0; i < tr.childCount; i++)
                {
                    string _btnNmae = string.Format("Btn{0}", item.Key);
                    GameObject go = tr.GetChild(i).gameObject;
                    if (go.name == _btnNmae)
                    {
                        Button button_Single = go.GetComponent<Button>();
                        ActivityButtonCeil _abtn = new ActivityButtonCeil(button_Single);
                        m_BtnDictionary.Add((EActivityTopic)item.Key, _abtn);
                        button_Single.onClick.AddListener(() => { OnClick_Button((int)item.Key); });
                    }
                }
            }

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityTopic.Instance.eventEmitter.Handle(Sys_ActivityTopic.EEvents.OnCommonActivityUpdate, RefreshPanelShow, toRegister);
            Sys_PetExpediton.Instance.eventEmitter.Handle(Sys_PetExpediton.EEvents.OnPetExpeditonDataUpdate, RefreshRedPointShow, toRegister);
            Sys_ItemExChange.Instance.eventEmitter.Handle(Sys_ItemExChange.EEvents.e_UpdateRedState, RefreshRedPointShow, toRegister);
            Sys_ActivityQuest.Instance.eventEmitter.Handle(Sys_ActivityQuest.EEvents.e_UpdateRedState, RefreshRedPointShow, toRegister);
            Sys_ActivitySavingBank.Instance.eventEmitter.Handle(Sys_ActivitySavingBank.EEvents.OnRefreshRedPoint, RefreshRedPointShow, toRegister);
        }
        public override void Show()
        {
            base.Show();
            RefreshPanelShow();
        }
        public override void Hide()
        {
            base.Hide();
            m_Timer?.Cancel();
            foreach (var item in m_BtnDictionary)
            {
                item.Value.i_Timer?.Cancel();
            }
            m_BtnDictionary.Clear();
        }
        private void RefreshPanelShow()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            uint _endTime = TimeManager.ConvertFromZeroTimeZone(Sys_ActivityTopic.Instance.activityTimeList[1]);
            if (nowtime > _endTime)
            {
                UIManager.CloseUI(EUIID.UI_Activity_Topic);
                return;
            }
            float _dura = _endTime - nowtime;
            m_Timer?.Cancel();
            m_Timer = Timer.Register(_dura, () =>
            {
                Sys_ActivityTopic.Instance.eventEmitter.Trigger(0);
                UIManager.CloseUI(EUIID.UI_Activity_Topic);
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
                if (Sys_ActivityTopic.Instance.CommonActivityTimeDictionary.ContainsKey((uint)item.Key))
                {
                    var _List = new List<uint>();
                    _List = Sys_ActivityTopic.Instance.CommonActivityTimeDictionary[(uint)item.Key];
                    var _uId = _List[2];
                    CSVActivityUiJump.Data _idata = CSVActivityUiJump.Instance.GetConfData(_uId);
                    item.Value.i_button.gameObject.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(_idata.Tittle);
                    item.Value.i_button.enabled = true;
                    if (_List != null)
                    {
                        uint nowtime = Sys_Time.Instance.GetServerTime();
                        uint _startTime = TimeManager.ConvertFromZeroTimeZone(_List[0]);
                        uint _endTime = TimeManager.ConvertFromZeroTimeZone(_List[1]);

                        if (nowtime < _startTime)
                        {
                            item.Value.i_button.gameObject.transform.Find("Image_Lock").gameObject.SetActive(true);

                        }
                        else if (nowtime > _endTime)
                        {
                            item.Value.i_button.transform.Find("Image_End").gameObject.SetActive(true);
                            item.Value.i_button.enabled = false;
                        }
                        else
                        {
                            float _dr = _endTime - nowtime;
                            item.Value.i_Timer?.Cancel();
                            item.Value.i_Timer = Timer.Register(_dr, () =>
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
                else
                {
                    item.Value.i_button.enabled = false;
                    DebugUtil.LogError("UI_Activity_Topic:UI界面按钮与当期运营活动UI表不一致,id:"+ item.Key);
                }
            }
            if (m_BtnDictionary.ContainsKey(EActivityTopic.Fashion))
            {
                SpecialFashion();
            }
            
        }

        public void RefreshRedPointShow()
        {
            foreach (var item in m_BtnDictionary)
            {
                RedPointShow(item.Key, item.Value.i_button);
            }
        }

        private void SpecialFashion()
        {//5—时装
            if (Sys_Fashion.Instance.activeId == 0)
            {
                m_BtnDictionary[EActivityTopic.Fashion].i_button.gameObject.transform.Find("Image_Lock").gameObject.SetActive(true);
                m_BtnDictionary[EActivityTopic.Fashion].i_button.gameObject.transform.Find("Dot").gameObject.SetActive(false);
            }
        }
        private void ButtonDataInit(Button _btn)
        {
            if (_btn != null)
            {
                _btn.enabled = true;
                _btn.transform.Find("Image_End").gameObject.SetActive(false);
                _btn.transform.Find("Image_Lock").gameObject.SetActive(false);
                _btn.transform.Find("Dot").gameObject.SetActive(false);
            }
                
        }
        private void RedPointShow(EActivityTopic _id, Button _btn)
        {
            bool isRed = Sys_ActivityTopic.Instance.RedPointShow(_id);
            if (_btn != null)
                _btn.transform.Find("Dot").gameObject.SetActive(isRed);
        }
        private string LastTimeShow()
        {
            DateTime _start = DateTime.Now;
            DateTime _end =  DateTime.Now;
            Sys_ActivityTopic.Instance.TopicReturnTime(Sys_ActivityTopic.Instance.activityTimeList, ref _start, ref _end);
            string _str = LanguageHelper.GetTextContent(591000705, _start.Year.ToString(), _start.Month.ToString(), _start.Day.ToString(), FormatString(_start.Hour), FormatString(_start.Minute), _end.Year.ToString(), _end.Month.ToString(), _end.Day.ToString(), FormatString(_end.Hour), FormatString(_end.Minute));
            return _str;
        }

        private string FormatString(int _time)
        {
            return string.Format("{0:00}", _time);
        }
        private void FashionHint()
        {
            if (Sys_Fashion.Instance.activeId == 0)
            {
                DebugUtil.Log(ELogType.eOperationActivity,"Fashion ActiveID is Zero!");
                return;
            }
        }
        #endregion
        #region 按钮
        private void OnClick_Button(int _id)
        {
            if (!Sys_ActivityTopic.Instance.CheckLimitedActivitySwitch((EActivityTopic)_id))
            {
                return;
            }
            if (!Sys_ActivityTopic.Instance.CheckPanelActivityOpen((uint)_id))
            {
                var _time = Sys_ActivityTopic.Instance.CommonActivityTimeDictionary[(uint)_id][0];
                string strTime = LanguageHelper.TimeToString(TimeManager.ConvertFromZeroTimeZone(_time), LanguageHelper.TimeFormat.Type_7);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000706, strTime));
                return;
            }
            if ((EActivityTopic)_id == EActivityTopic.Fashion)
            {
                FashionHint();
            }
            var _uId = Sys_ActivityTopic.Instance.CommonActivityTimeDictionary[(uint)_id][2];
            CSVActivityUiJump.Data _idata = CSVActivityUiJump.Instance.GetConfData(_uId);
            if (_idata != null)
            {
                if (_idata.UiParam != 0)
                    UIManager.OpenUI((EUIID)_idata.UiId, false, _idata.UiParam);
                else
                    UIManager.OpenUI((EUIID)_idata.UiId);
            }

            UIManager.HitButton(EUIID.UI_Activity_Topic, _id.ToString());
        }
        #endregion
    }
}



