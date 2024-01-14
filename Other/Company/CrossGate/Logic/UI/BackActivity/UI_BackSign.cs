using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    public class UI_BackSign : UI_BackActivityBase
    {
        #region 界面显示
        private Transform CardPanel;
        #endregion
        private List<BackSignCeil> CeilList = new List<BackSignCeil>();
        private Timer m_Timer;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }
        public override void Show()
        {
            base.Show();
            Sys_BackSign.Instance.OnGetBackSignInDataReq();
        }
        public override void Hide()
        {
            base.Hide();
            ListDestory();
            m_Timer?.Cancel();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override bool CheckFunctionIsOpen()
        {
            return Sys_BackSign.Instance.BackSignFunctionOpen(); 
        }

        public override bool CheckTabRedPoint()
        {
            return Sys_BackSign.Instance.BackSignRedPoint();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BackSign.Instance.eventEmitter.Handle(Sys_BackSign.EEvents.OnBackSignDataUpdate, OnBackSignUpdate, toRegister);
            Sys_BackSign.Instance.eventEmitter.Handle<uint>(Sys_BackSign.EEvents.OnBackSignRes, OnBackSignRes, toRegister);
        }
        #endregion
        #region Function
        private void Parse()
        {
            CardPanel = transform.Find("Card");
        }
        private void InitCeilList()
        {
            if (CardPanel==null)
            {
                return;
            }
            ListDestory();
            for (int i=0;i<CardPanel.childCount;i++)
            {
                BackSignCeil _ceil = new BackSignCeil(i);
                GameObject _go = CardPanel.GetChild(i).gameObject;
                _ceil.Init(_go.transform);
                _ceil.SetThisCeilData();
                CeilList.Add(_ceil);
            }
        }
        private void ListDestory()
        {
            for (int i=0;i<CeilList.Count;i++)
            {
                CeilList[i].Destory();
            }
            CeilList.Clear();
        }
        private void OnBackSignUpdate()
        {
            InitCeilList();
        }
        private void OnBackSignRes(uint _index)
        {
            if (_index<CeilList.Count)
            {
                CeilList[(int)_index].SetThisCeilData();
                Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
            }
        }
        #endregion
        public class BackSignCeil
        {
            //不可领取节点
            private GameObject un_received;
            //正领取节点
            private GameObject on_received;
            //已领取节点
            private GameObject off_received;
            private GameObject received;
            //签到奖励
            private GameObject awardItem;
            //道具名称
            private Text txt_unreceived;
            private Text txt_onreceived;
            private Text txt_offreceived;

            private Button btn_Sign;
            private int m_index;
            private uint SignState = 0;
            private bool isOpen;
            private bool isReceive;
            private Timer m_timer;

            public BackSignCeil(int m_index)
            {
                this.m_index = m_index;
            }

            public void Init(Transform trans)
            {
                un_received = trans.Find("un_received").gameObject;
                on_received = trans.Find("on_receive").gameObject;
                off_received = trans.Find("off_reveive").gameObject;
                received = trans.Find("received").gameObject;
                awardItem = trans.Find("Item").gameObject;
                txt_unreceived = trans.Find("un_received/Text_Name").GetComponent<Text>();
                txt_onreceived = trans.Find("on_receive/Group/Image2/Text_Name").GetComponent<Text>();
                txt_offreceived = trans.Find("off_reveive/Top/Image2/Text_Name").GetComponent<Text>();
                btn_Sign = trans.Find("on_receive/Group/Button").GetComponent<Button>();
                btn_Sign.onClick.RemoveAllListeners();
                btn_Sign.onClick.AddListener(On_ClickSignCeil);
                txt_unreceived.gameObject.SetActive(false);
                txt_onreceived.gameObject.SetActive(false);
                txt_offreceived.gameObject.SetActive(false);

            }
            public void SetThisCeilData()
            {
                SetState();
                uint _dropId = 1;
                if (m_index < Sys_BackSign.Instance.BackSignGetList.Count)
                {
                    _dropId = Sys_BackSign.Instance.BackSignGetList[m_index];
                }
                else
                {
                    DebugUtil.Log(ELogType.eOperationActivity, "BackSignGetList ERROR");
                    return;
                }
                List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(_dropId);
                if (list_drop.Count > 0)
                {
                    PropItem _propItem = new PropItem();
                    _propItem.BindGameObject(awardItem);
                    ItemIdCount itemIdCount = list_drop[0];
                    _propItem.SetData(new MessageBoxEvt(EUIID.UI_BackActivity, new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false,
                                _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
                    ImageHelper.SetImageGray(_propItem.Layout.imgIcon, isReceive, true);
                    //CSVItem.Data iData = CSVItem.Instance.GetConfData(itemIdCount.id);
                    //txt_unreceived.text = LanguageHelper.GetTextContent(iData.name_id);
                    //txt_onreceived.text = txt_unreceived.text;
                    //txt_offreceived.text = txt_unreceived.text;
                }

            }
            private void SetState()
            {
                if (m_index < Sys_BackSign.Instance.BackSignList.Count)
                {
                    SignState = Sys_BackSign.Instance.BackSignList[m_index];
                }
                else
                {
                    DebugUtil.Log(ELogType.eOperationActivity, "BackSignList ERROR");
                }
                switch (SignState)
                {
                    case 0:
                        isOpen = true;
                        isReceive = false;
                        break;
                    case 1:
                        isOpen = true;
                        isReceive = true;
                        break;
                    case 2:
                        isOpen = false;
                        break;
                    default:
                        break;
                }

                un_received.SetActive(!isOpen);
                on_received.SetActive(isOpen && !isReceive);
                off_received.SetActive(isOpen & isReceive);
                received.SetActive(isOpen & isReceive);
            }
            private void On_ClickSignCeil()
            {
                btn_Sign.enabled = false;
                m_timer?.Cancel();
                m_timer = Timer.Register(1f, () =>
                {
                    btn_Sign.enabled = true;
                });
                if (SignState == 0)
                {
                    if (!Sys_BackSign.Instance.BackSignFunctionOpen())
                    {
                        Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(107701).words);
                        return;
                    }
                    Sys_BackSign.Instance.OnBackSignSendReq((uint)m_index);
                }
            }
            public void Destory()
            {
                m_timer?.Cancel();
            }

        }

    }

}
