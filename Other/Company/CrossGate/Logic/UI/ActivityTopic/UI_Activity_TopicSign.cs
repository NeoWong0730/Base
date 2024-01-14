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
    public class TopicSignCeil
    {
        private GameObject go_Prop;
        private Text txt_Name;
        private GameObject go_Select;
        private GameObject go_Get;
        private Button btn_Click;
        private int m_Id;
        private uint SignState = 0;
        private int signType=-1;
        private bool isEmergency;
        public TopicSignCeil(int _id,uint _state,int type)
        {
            m_Id = _id;//从0计数
            SignState = _state;
            signType = type;
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
                switch (signType)
                {
                    case 0:
                        Sys_ActivityTopic.Instance.OnLimitedActivitySignTakeReq((uint)_day);
                        break;
                    case 1:
                        Sys_ActivityTopic.Instance.OnMergeServerSignTakeReq((uint)_day);
                        break;
                    default:
                        break;
                }
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
    public class UI_Activity_TopicSign : UIBase
    {
        #region 界面组件
        private Button btn_Close;
        private Text txt_LastTime;
        private RawImage img_Character;
        private RawImage img_BG;
        private RawImage img_Close;
        private RawImage img_Gift;
        private List<GameObject> list_SignItem = new List<GameObject>();
        private List<TopicSignCeil> list_SignCeil = new List<TopicSignCeil>();
        private uint m_signID;
        private DateTime signStartTime;
        private DateTime signEndTime;
        private DateTime signRefreshDate;
        private bool isDataReq = false;
        private string lastTimeText;
        private uint signActivityID=0;
        private ActivitySignCeil ceil;
        private uint ceilIndex;
        #endregion
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg!=null)
            {
                signActivityID= (uint)arg;
            }
        }
        public void Init(Transform transform)
        {
            btn_Close = transform.Find("Animator/Image_bg01/View_Title07/Btn_Close").GetComponent<Button>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            txt_LastTime = transform.Find("Animator/Image_bg01/Image_Time/Num").GetComponent<Text>();
            img_Character = transform.Find("Animator/Image_bg01 (1)").GetComponent<RawImage>();
            img_BG = transform.Find("Animator/Image_bg01").GetComponent<RawImage>();
            img_Close = transform.Find("Animator/Image_bg01/Image_bg01 (2)").GetComponent<RawImage>();
            img_Gift = transform.Find("Animator/Image_bg01/Image_bg01 (3)").GetComponent<RawImage>();
            var textureData = CSVSignActivity.Instance.GetConfData(ceil.signTextureId);
            ImageHelper.SetTexture(img_BG, textureData.Image1, true);
            ImageHelper.SetTexture(img_Close, textureData.Image2, true);
            ImageHelper.SetTexture(img_Character, textureData.Image3,true);
            ImageHelper.SetTexture(img_Gift, textureData.Image4, true);
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
            Sys_ActivityTopic.Instance.eventEmitter.Handle(Sys_ActivityTopic.EEvents.OnCommonActivityUpdate, OnFreshAllCeil, toRegister);
        }
        protected override void OnLoaded()
        {
            if (signActivityID == 0)
            {
                DebugUtil.LogError("无活动签到数据");
                UIManager.CloseUI(EUIID.UI_Activity_SummerSign);
                return;
            }
            foreach (var item in Sys_ActivityTopic.Instance.ActivitySignDictionary)
            {
                if (item.Value.IdOrType == signActivityID)
                {
                    ceilIndex = item.Key;
                    ceil = item.Value;
                }
            }
            if (ceil.IdOrType == 0)
            {
                DebugUtil.LogError("签到数据缺失");
                return;
            }
            Init(transform);
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
            ceil = Sys_ActivityTopic.Instance.ActivitySignDictionary[ceilIndex];
            Sys_ActivityTopic.Instance.TopicReturnTime(ceil.activityTimeList, ref signStartTime,ref signEndTime);
            signRefreshDate = TimeManager.GetDateTime(Sys_ActivityTopic.Instance.signRefreshTime);
            lastTimeText = StringReturn();
        }

        private void SetSignItemShow()
        {
            list_SignCeil.Clear();
            for (int i = 0; i < list_SignItem.Count; i++)
            {
                TopicSignCeil s_ceil = new TopicSignCeil(i, ceil.MergeServerSignList[i],(int)ceilIndex);
                s_ceil.Init(list_SignItem[i].transform);
                s_ceil.SetData(ceil.MergeServerDateList[i]);
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



