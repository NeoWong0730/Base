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
    public class BackAwardGetCeil
    {
        private Transform trans;
        private GameObject go_freeItem;
        private GameObject go_payItem;
        private Button btn_free;
        private Button btn_pay;
        private Text txt_Name;
        private GameObject go_Cost;
        private Image img_pay;
        private Text txt_pay;
        private GameObject go_freeGetAll;
        private GameObject go_freeTodayGet;
        private GameObject go_payGetAll;
        private GameObject go_payTodayGet;

        private Sys_BackAwardGet.BackAwardCeil ceil;
        CSVReturnRecover.Data reData;
        private uint activityType;
        private uint ceilIdex;
        private bool isEnd;
        public void BindGameObject(GameObject go)
        {
            trans = go.transform;
            go_freeItem = trans.Find("Free/ItemGrid/PropItem").gameObject;
            go_payItem = trans.Find("Rechargeable/ItemGrid/PropItem").gameObject;
            btn_free = trans.Find("Free/State/Btn_01").GetComponent<Button>();
            btn_free.onClick.RemoveAllListeners();
            btn_free.onClick.AddListener(OnFreeButtonClicked);
            btn_pay = trans.Find("Rechargeable/State/Btn_01").GetComponent<Button>();
            btn_pay.onClick.RemoveAllListeners();
            btn_pay.onClick.AddListener(OnPayButtonClicked);
            txt_Name = trans.Find("Text_Title").GetComponent<Text>();
            go_Cost= trans.Find("Rechargeable/Cost").gameObject;
            img_pay = trans.Find("Rechargeable/Cost/Image_Icon").GetComponent<Image>();
            txt_pay = trans.Find("Rechargeable/Cost/Image_Icon/Text_Value").GetComponent<Text>();
            go_freeGetAll = trans.Find("Free/State/yilingqu").gameObject;
            go_freeTodayGet = trans.Find("Free/State/jinriyilingqu").gameObject;
            go_payGetAll = trans.Find("Rechargeable/State/yilingqu").gameObject;
            go_payTodayGet = trans.Find("Rechargeable/State/jinriyilingqu").gameObject;
        }
        public void SetCeilData(uint _activityType, int _index,uint _lastTime)
        {
            activityType = _activityType;
            ceilIdex = (uint)_index;
            ceil = Sys_BackAwardGet.Instance.BackAwardDictionary[_activityType].BackAwardList[_index];
            reData = CSVReturnRecover.Instance.GetConfData(ceil.TableId);
            isEnd = _lastTime > 0 ? false : true;
            SetData();
            SetButtonState();
            SetPropItem();
        }
        private void SetData()
        {
            //活动名称
            txt_Name.text = LanguageHelper.GetTextContent(reData.pack_name);
            //设置付费图片、金额
            var coinData = CSVItem.Instance.GetConfData(reData.price[0]);
            ImageHelper.SetIcon(img_pay, coinData.small_icon_id, true);
            txt_pay.text = reData.price[1].ToString();

        }
        private void SetButtonState()
        {
            switch ((Sys_BackAwardGet.BackAwardType)activityType)
            {
                case Sys_BackAwardGet.BackAwardType.Normal:
                    go_freeGetAll.SetActive(false);
                    go_payGetAll.SetActive(false);
                    go_freeTodayGet.SetActive(ceil.FreeGetState == 1);
                    go_payTodayGet.SetActive(ceil.PayGetState == 1);
                    break;
                case Sys_BackAwardGet.BackAwardType.LimitedTime:
                    go_freeTodayGet.SetActive(false);
                    go_payTodayGet.SetActive(false);
                    go_freeGetAll.SetActive(ceil.FreeGetState == 1);
                    go_payGetAll.SetActive(ceil.PayGetState == 1);
                    break;
            }
            //免费
            btn_free.gameObject.SetActive(ceil.FreeGetState == 0);
            ImageHelper.SetImageGray(btn_free.GetComponent<Image>(), isEnd);
            //付费
            btn_pay.gameObject.SetActive(ceil.PayGetState == 0);
            go_Cost.SetActive(ceil.PayGetState == 0);
            ImageHelper.SetImageGray(btn_pay.GetComponent<Image>(), isEnd);
        }
        private void SetPropItem()
        {
            PropItemInit(reData.Reward_Group[0], go_freeItem);
            PropItemInit(reData.Reward_Group[1], go_payItem);
        }

        private void PropItemInit(uint dropId,GameObject go)
        {
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(dropId);
            if (list_drop.Count==0)
            {
                DebugUtil.Log(ELogType.eOperationActivity, "DropId Is Empty");
                return;
            }
            FrameworkTool.CreateChildList(go.transform.parent, list_drop.Count);
            for (int i=0;i< list_drop.Count;i++)
            {
                GameObject _propGo = go.transform.parent.GetChild(i).gameObject;
                PropItem _propItem = new PropItem();
                _propItem.BindGameObject(_propGo);
                ItemIdCount itemIdCount = list_drop[i];
                _propItem.SetData(new MessageBoxEvt(EUIID.UI_BackActivity, new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
            }

        }
        public void OnDestory()
        {
            FrameworkTool.DestroyChildren(go_freeItem.transform.parent.gameObject, go_freeItem.transform.name);
            FrameworkTool.DestroyChildren(go_payItem.transform.parent.gameObject, go_payItem.transform.name);
        }

        private void OnFreeButtonClicked()
        {
            if (!Sys_BackAwardGet.Instance.BackAwardGetFunctionOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(107701).words);
                return;
            }
            if (isEnd)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014914));
                return;
            }
            if (ceil.FreeGetState==0)
            {
                Sys_BackAwardGet.Instance.OnBackAwardGetReq(activityType, 1, ceilIdex);
            }
        }

        private void OnPayButtonClicked()
        {
            if (!Sys_BackAwardGet.Instance.BackAwardGetFunctionOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(107701).words);
                return;
            }
            if (isEnd)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014914));
                return;
            }
            if (reData.price[1] >Sys_Bag.Instance.GetItemCount(reData.price[0]))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003016));//魔币不足
                return;
            }
            if (ceil.PayGetState == 0)
            {
                Sys_BackAwardGet.Instance.OnBackAwardGetReq(activityType, 2, ceilIdex);
            }
        }
    }
    public class UI_BackAwardGet : UI_BackActivityBase
    {
        #region 界面
        private InfinityGrid m_infinityGrid;
        private CP_ToggleRegistry tg_Registry;
        private GameObject tg_Limit;
        private GameObject go_dailyRedPoint;
        private GameObject go_limitRedPoint;
        private Button btn_Rule;
        private Text txt_Time;
        private GameObject go_Time;
        #endregion
        private uint nowActivityType = 1;
        private uint lastTime;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();

        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            //RefreshGroup();
            Sys_BackAwardGet.Instance.OnBackAwardDataReq(nowActivityType);
        }
        public override void Hide()
        {
            base.Hide();
            UIManager.CloseUI(EUIID.UI_Rule);
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BackAwardGet.Instance.eventEmitter.Handle(Sys_BackAwardGet.EEvents.OnBackAwardDataUpdate, OnBackAwardUpdate, toRegister);
        }
        public override bool CheckFunctionIsOpen()
        {
            return Sys_BackAwardGet.Instance.BackAwardGetFunctionOpen();
        }
        public override bool CheckTabRedPoint()
        {
            return Sys_BackAwardGet.Instance.BackAwardRedPoint();
        }
        #endregion
        #region Fuction
         private void Parse()
        {
            m_infinityGrid =transform.Find("Scroll View").GetComponent<InfinityGrid>();
            tg_Registry = transform.Find("TabList").GetComponent<CP_ToggleRegistry>();
            tg_Limit = transform.Find("TabList/ListItem (1)").gameObject;
            go_dailyRedPoint = transform.Find("TabList/ListItem/Image_Dot").gameObject;
            go_limitRedPoint = transform.Find("TabList/ListItem (1)/Image_Dot").gameObject;
            btn_Rule = transform.Find("Rule/Btn_Rule/").GetComponent<Button>();
            btn_Rule.onClick.AddListener(OnButtonRuleClicked);
            go_Time= transform.Find("Text_Time").gameObject;
            txt_Time = transform.Find("Text_Time/Text_Value").GetComponent<Text>();
            tg_Registry.onToggleChange += OnToggleChange;
            m_infinityGrid.CellCount = Sys_BackAwardGet.Instance.BackAwardDictionary[nowActivityType].BackAwardList.Count;
            m_infinityGrid.onCreateCell += OnCreateCell;
            m_infinityGrid.onCellChange += OnCellChange;
            SetTime();
            LimitTimeToggleShow();
        }

        private void RefreshGroup()
        {
            SetTime();
            LimitTimeToggleShow();
            RefreshToggleRedPoint();
            m_infinityGrid.CellCount = Sys_BackAwardGet.Instance.BackAwardDictionary[nowActivityType].BackAwardList.Count;
            m_infinityGrid.ForceRefreshActiveCell();
        }
        private void SetTime()
        {
            lastTime = Sys_BackAwardGet.Instance.BackAwardDictionary[nowActivityType].lastTime;
            txt_Time.text =LanguageHelper.GetTextContent(10154, lastTime.ToString());
        }
        private void RefreshToggleRedPoint()
        {
            go_dailyRedPoint.SetActive(Sys_BackAwardGet.Instance.BackAwardDictionary[1].ThisTypeRedPoint());
            go_limitRedPoint.SetActive(Sys_BackAwardGet.Instance.BackAwardDictionary[2].ThisTypeRedPoint());
        }
        private void LimitTimeToggleShow()
        {//限时找回按钮是否显示
            tg_Limit.SetActive(Sys_BackAwardGet.Instance.CheckLimitedTimeOpen());
        }
        private void OnToggleChange(int current, int old)
        {
            if ((int)nowActivityType == current)
                return;

            nowActivityType = (uint)current;
            Sys_BackAwardGet.Instance.NowBackAwardType = nowActivityType;
            Sys_BackAwardGet.Instance.OnBackAwardDataReq(nowActivityType);
            m_infinityGrid.MoveToIndex(0);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            BackAwardGetCeil entry = new BackAwardGetCeil();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            BackAwardGetCeil entry = cell.mUserData as BackAwardGetCeil;
            entry.OnDestory();
            entry.SetCeilData(nowActivityType,index,lastTime);
        }
        #endregion

        #region Event
        private void OnBackAwardUpdate()
        {
            RefreshGroup();
        }
        private void OnButtonRuleClicked()
        {
            UIRuleParam rParam = new UIRuleParam();
            rParam.StrContent = LanguageHelper.GetTextContent(2014911);
            rParam.Pos = CameraManager.mUICamera.WorldToScreenPoint((btn_Rule.GetComponent<RectTransform>().position + go_Time.GetComponent<RectTransform>().position)/2);
            UIManager.OpenUI(EUIID.UI_Rule, false, rParam);
        }
        #endregion
    }

}