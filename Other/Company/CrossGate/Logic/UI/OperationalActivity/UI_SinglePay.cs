using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace Logic
{
    public class UI_SinglePay : UI_OperationalActivityBase
    {
        private List<uint> listIds;
        private InfinityGrid infinity;
        private Text txtTime;

        private Timer timer;
        private float countDownTime = 0;

        #region 系统函数
        protected override void Loaded()
        {
            if (Sys_SinglePay.Instance.CheckSinglePayIsOpen())
            {
                Sys_SinglePay.Instance.ReqSinglePayData();
            }
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            timer?.Cancel();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            timer?.Cancel();
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_SinglePay.Instance.eventEmitter.Handle(Sys_SinglePay.EEvents.OnSinglePayDataUpdate, OnUpdateSinglePayData, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        #endregion
        #region func
        private void Parse()
        {
            txtTime = transform.Find("bg/Text_Time/Text_Value").GetComponent<Text>();

            infinity = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }
        private void UpdateView()
        {
            StartTimer();

            listIds = Sys_SinglePay.Instance.listTaskId;
            infinity.CellCount = listIds.Count;
            infinity.ForceRefreshActiveCell();
        }
        private void StartTimer()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var targetTime = Sys_SinglePay.Instance.curEndTime;
            countDownTime = targetTime - nowtime;
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            //关闭页签
            Sys_SinglePay.Instance.eventEmitter.Trigger(Sys_SinglePay.EEvents.OnSinglePayEnd);
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time && txtTime != null)
            {
                txtTime.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_4);
            }
        }
        private void OnUpdateSinglePayData()
        {
            UpdateView();
        }
        #endregion
        #region event
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_SinglePayCell mCell = new UI_SinglePayCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_SinglePayCell mCell = cell.mUserData as UI_SinglePayCell;
            mCell.UpdateCellView(listIds[index]);
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        #endregion

        #region class
        public class UI_SinglePayCell
        {
            private uint curId;
            private Transform transform;
            private Text txtCount;//剩余次数
            private Text txtCost;//充值消耗
            private GameObject goIsGet;
            private Button btnGet;

            private InfinityGrid infinity;
            private List<ItemIdCount> listItem;

            public void Init(Transform _transform)
            {
                transform = _transform;
                txtCount = transform.Find("Text_Time").GetComponent<Text>();
                txtCost = transform.Find("Text_Cost/Text_Value").GetComponent<Text>();
                goIsGet = transform.Find("State/Image").gameObject;
                btnGet = transform.Find("State/Btn_01").GetComponent<Button>();
                btnGet.onClick.AddListener(OnBtnGetClick);

                infinity = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
                infinity.onCreateCell += OnCreateCell;
                infinity.onCellChange += OnCellChange;
            }

            public void UpdateCellView(uint id)
            {
                curId = id;
                var data = CSVSinglePay.Instance.GetConfData(id);
                if (data != null)
                {
                    uint maxCost = data.Num;
                    uint curCost = Sys_SinglePay.Instance.GetResidueCount(id);
                    if (maxCost > 0)
                    {
                        txtCount.text = LanguageHelper.GetTextContent(2014305, curCost.ToString(), maxCost.ToString());//剩余次数（{0}/{1}）
                    }
                    else
                    {
                        txtCount.text = "";
                    }
                    txtCost.text = data.Recharge.ToString();// LanguageHelper.GetTextContent(11647, data.Recharge.ToString());
                    bool isGet = curCost <= 0;//已领取完毕
                    goIsGet.SetActive(isGet);
                    btnGet.gameObject.SetActive(!isGet);
                    if (!isGet)
                    {
                        bool canGet = Sys_SinglePay.Instance.CheckCanGet(id);
                        ImageHelper.SetImageGray(btnGet.transform.GetComponent<Image>(), !canGet, true);
                    }
                    //奖励列表
                    listItem = CSVDrop.Instance.GetDropItem(data.reward);
                    infinity.CellCount = listItem.Count;
                    infinity.ForceRefreshActiveCell();
                }
            }

            private void OnBtnGetClick()
            {
                bool canGet = Sys_SinglePay.Instance.CheckCanGet(curId);
                if (canGet)
                {
                    Sys_SinglePay.Instance.ReqSinglePayReward(curId);
                }
            }

            private void OnCreateCell(InfinityGridCell cell)
            {
                PropItem mCell = new PropItem();
                mCell.BindGameObject(cell.mRootTransform.gameObject);
                cell.BindUserData(mCell);
            }
            private void OnCellChange(InfinityGridCell cell, int index)
            {
                PropItem mCell = cell.mUserData as PropItem;
                var dropItem = listItem[index];
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItem.id, dropItem.count, true, false, false, false, false, true);
                mCell.SetData(itemData, EUIID.UI_OperationalActivity);
            }
        }
        #endregion
    }
}
