using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Logic.Core;
using System.Text;
using Framework;
using System.Linq;

namespace Logic
{

    public class UI_FightTreasure_Ceil : UIComponent
    {
        private Text txt_Rank;
        private GameObject go_Gift;
        private GameObject go_MultGift;
        private Text txt_GiftCount;
        private GameObject go_Time;
        private Text txt_Time;
        private GameObject go_SignUp;
        private Button btn_SignUp;
        private Image img_SignCoin;
        private Text txt_SignPrice;
        private Text txt_No;
        private GameObject img_SignFin;
        private GameObject go_List;
        private Text txt_ListName;
        private Text txt_SingleName;
        private Button btn_List;
        private Text txt_SignUpCount;
        private Text txt_End;
        private GameObject go_end;
        private bool isTick = false;
        CSVOneCoinLottey.Data lData;
        Sys_OperationalActivity.SingleRoundData singleRound;
        private Action<int> onOver;
        private int nowRound;
        private uint ApplyCount;
        private bool isSwitch = true;
        EActivityRulerType _type;

        public override void Show()
        {
            base.Show();

        }
        protected override void Refresh()
        {
            base.Refresh();
            if (isTick)
            {//进行中剩余时间显示
                txt_Time.text = TickNumberShow(LastTimeReturn(singleRound.endDate));
            }
            if (singleRound.singleState == 1)
            {
                CheckEnd();
            }
            if (singleRound.singleState == 2)
            {
                CheckStart();
            }
        }
        public void BindGameObject(Transform transform)
        {
            txt_Rank = transform.Find("Text_Rank").GetComponent<Text>();
            go_Gift = transform.Find("PropItem").gameObject;
            go_MultGift = transform.Find("Button").gameObject;
            txt_GiftCount = transform.Find("Text_Amount").GetComponent<Text>();

            go_Time = transform.Find("Text_Time").gameObject;
            txt_Time = transform.Find("Text_Time/Text").GetComponent<Text>();
            go_end = transform.Find("Text_End").gameObject;
            go_SignUp = transform.Find("SignUp").gameObject;
            btn_SignUp = transform.Find("SignUp/Btn_01").GetComponent<Button>();
            img_SignCoin = transform.Find("SignUp/Text_Cost/Image_Coin").GetComponent<Image>();
            txt_SignPrice = transform.Find("SignUp/Text_Cost").GetComponent<Text>();
            txt_No = transform.Find("SignUp/Text_No").GetComponent<Text>();
            img_SignFin = transform.Find("SignUp/Image_Finish").gameObject;

            go_List = transform.Find("List").gameObject;
            txt_ListName = transform.Find("List/Text").GetComponent<Text>();
            txt_SingleName = transform.Find("List/Name/Text_Name").GetComponent<Text>();
            btn_List = transform.Find("List/Btn_01").GetComponent<Button>();

            txt_SignUpCount = transform.Find("Text").GetComponent<Text>();

            btn_SignUp.onClick.AddListener(OnSignUpButtonClicked);
            btn_List.onClick.AddListener(OnListButtonClicked);
            go_MultGift.GetComponent<Button>().onClick.AddListener(OnMultiplyGift);

        }
        public void SetCeilData(int _index, Sys_OperationalActivity.SingleRoundData _data, EActivityRulerType type)
        {
            nowRound = _index;
            singleRound = _data;
            _type = type;
            ApplyCount = singleRound.applyNum;
            isSwitch = Sys_OperationalActivity.Instance.CheckFightTreasureSwitch(type);
            lData = CSVOneCoinLottey.Instance.GetConfData(singleRound._uid);
            PanelStateShow();
            SetGiftItem();
            Refresh();
        }

        private void PanelStateShow()
        {
            isTick = false;
            go_Time.SetActive(singleRound.singleState != 0);
            go_end.SetActive(singleRound.singleState == 0);
            txt_Time.gameObject.SetActive(singleRound.singleState != 0);
            go_SignUp.SetActive(singleRound.singleState != 0);//报名
            go_List.SetActive(singleRound.singleState == 0);//获奖列表
            txt_No.gameObject.SetActive(singleRound.singleState == 2);//尚未开启
            txt_Rank.text = LanguageHelper.GetTextContent(1001937, (nowRound + 1).ToString());//当前第几轮
            txt_GiftCount.text = LanguageHelper.GetTextContent(1001935, lData.Prize_Number[singleRound.thisdayIndex].ToString());//夺宝总数
            ImageHelper.SetIcon(img_SignCoin, 992501, true);//设置货币图标
            txt_SignPrice.text = lData.Price.ToString();//设置售价
            txt_SignUpCount.gameObject.SetActive(false);//报名人数
            if (singleRound.singleState == 0)
            {//已结束
                EndState();
            }
            else if (singleRound.singleState == 1)
            {//进行中
                InProcess();
            }
            else
            {//未开始
                StandByState();
            }

        }

        private void EndState()
        {
            go_Time.GetComponent<Text>().text = LanguageHelper.GetTextContent(1001930);//本轮夺宝活动已结束
            btn_List.gameObject.SetActive(singleRound.roleNamesList.Count > 1);
            if (singleRound.roleNamesList.Count == 0)
            {
                go_List.SetActive(false);
                return;
            }
            if (singleRound.roleNamesList.Count > 1)
            {
                txt_SingleName.gameObject.SetActive(false);
                txt_ListName.text = LanguageHelper.GetTextContent(1001934);
            }
            else
            {
                txt_SingleName.text = singleRound.roleNamesList[0];
                txt_ListName.text = LanguageHelper.GetTextContent(1001933);
            }

        }
        private void InProcess()
        {
            go_Time.GetComponent<Text>().text = LanguageHelper.GetTextContent(1001931);//距离报名截止时间还有
            isTick = true;
            ButtonShow();
            ApplyCountShow(ApplyCount);
        }

        private void StandByState()
        {
            go_Time.GetComponent<Text>().text = LanguageHelper.GetTextContent(1001932);//报名开始时间
            StringBuilder _str = new StringBuilder();
            _str.Append(DateShow(singleRound.startDate.Year, singleRound.startDate.Month, singleRound.startDate.Day, "-")).Append(" ").Append(DateShow(singleRound.startDate.Hour, singleRound.startDate.Minute, singleRound.startDate.Second, ":"));
            txt_Time.text = _str.ToString();//年-月-日 时:分:秒
            btn_SignUp.gameObject.SetActive(false);
            img_SignFin.gameObject.SetActive(false);

        }
        public string DateShow(int first, int second, int third, string breakStr)
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0:00}", first).Append(breakStr).AppendFormat("{0:00}", second).Append(breakStr).AppendFormat("{0:00}", third);
            return str.ToString();

        }

        private TimeSpan LastTimeReturn(DateTime _dt)
        {
            DateTime nowtime = Sys_OperationalActivity.Instance.ServerDateTime();
            TimeSpan sp = _dt.Subtract(nowtime);

            return sp;

        }
        private string TickNumberShow(TimeSpan _sp)
        {
            uint lastSecond = (uint)_sp.TotalSeconds;
            string strTime = LanguageHelper.TimeToString(lastSecond, LanguageHelper.TimeFormat.Type_1);
            return strTime;
        }
        private void CheckStart()
        {//检查未开启是否开启
            var tick = LastTimeReturn(singleRound.startDate).TotalSeconds;
            if (tick < 0)
            {
                onOver?.Invoke(nowRound);
            }
        }

        private void CheckEnd()
        {//检查进行中是否结束
            var tick = LastTimeReturn(singleRound.endDate).TotalSeconds;
            if (tick < 0)
            {
                isTick = false;
                txt_Time.text = TickNumberShow(TimeSpan.Zero);
                onOver?.Invoke(nowRound);


            }
        }

        private void ApplyCountShow(uint _num)
        {
            string _str = "<10";
            if (_num < 10)
            {

            }
            else if ( _num < 100)
            {
                _str = "<100";
            }
            else if (_num < 500)
            {
                _str = "<500";
            }
            else if (_num < 999)
            {
                _str = "<999";
            }
            else
            {
                _str = "999+";
            }
            txt_SignUpCount.gameObject.SetActive(true);
            txt_SignUpCount.text = LanguageHelper.GetTextContent(1001936, _str);

        }

        private void SetGiftItem()
        {
            CSVDrop.Data dropDate = CSVDrop.Instance.GetDropItemData(lData.Drop_Id[singleRound.thisdayIndex]);
            go_Gift.SetActive(dropDate.reward_show.Count <= 1);
            go_MultGift.SetActive(dropDate.reward_show.Count > 1);
            if (dropDate.reward_show.Count <= 1)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(go_Gift);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, new PropIconLoader.ShowItemData(dropDate.reward_show[0][0], dropDate.reward_show[0][1], true, false, false, false, false,
                        _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
            }
        }

        private void ButtonShow()
        {
            if (Sys_OperationalActivity.Instance.applyList.ContainsKey(singleRound._aid))
            {
                var _isApply = Sys_OperationalActivity.Instance.applyList[singleRound._aid][nowRound];
                btn_SignUp.gameObject.SetActive(!_isApply);
                img_SignFin.gameObject.SetActive(_isApply);

            }
            else
            {
                DebugUtil.LogError("夺宝活动：" + singleRound._aid + "无报名信息");
                btn_SignUp.gameObject.SetActive(false);
                img_SignFin.gameObject.SetActive(false);
            }


        }

        private void OnSignUpButtonClicked()
        {
            var playerIcon = Sys_Bag.Instance.GetItemCount(1);
            if (playerIcon < lData.Price)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(8203);
                PromptBoxParameter.Instance.SetConfirm(true, () => {
                    MallPrama mPrama = new MallPrama();
                    mPrama.mallId = 101u;
                    mPrama.isCharge = true;
                    UIManager.OpenUI(EUIID.UI_Mall, false, mPrama);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

            }
            else
            {
                Sys_OperationalActivity.Instance.OnFightTreasureApplyActivityReq(singleRound._aid, (uint)nowRound);
            }

        }

        private void OnListButtonClicked()
        {
            UIListNumberParam _param = new UIListNumberParam();
            _param.nameList = singleRound.roleNamesList;
            Vector3 _vec = btn_List.gameObject.GetComponent<RectTransform>().position;
            Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(_vec);
            RectTransform _rec = btn_List.gameObject.GetComponent<RectTransform>();
            _param.Pos = _screenVec;
            _param.Rec = _rec;
            UIManager.OpenUI(EUIID.UI_ListNumber, false, _param);
        }

        public void OnFightTreasureUpdateButton(uint _Count)
        {
            if (singleRound.singleState==1)
            {
                ButtonShow();
                ApplyCount = _Count;
                ApplyCountShow(ApplyCount);
            }

        }

        private void OnMultiplyGift()
        {
            RewardPanelParam _param = new RewardPanelParam();
            Vector3 _vec = go_MultGift.gameObject.GetComponent<RectTransform>().position;
            Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(_vec);
            _param.propList = CSVDrop.Instance.GetDropItem(lData.Drop_Id[singleRound.thisdayIndex]);
            _param.Pos = _screenVec;
            UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
        }
        public void AddRefreshListener(Action<int> onOvered = null)
        {
            onOver = onOvered;
        }

    }
    public class UI_FightTreasure_OneDollar : UI_OperationalActivityBase
    {
        #region 界面显示
        private InfinityGrid PanelScrollGrid;
        private Timer m_timer;
        private uint _id;
        Sys_OperationalActivity.SingleFightType sft;
        private Dictionary<int, UI_FightTreasure_Ceil> entrydic = new Dictionary<int, UI_FightTreasure_Ceil>();
        #endregion
        #region 系统函数
        protected override void Update()
        {
            base.Update();
            if (entrydic.Count == 0)
            {
                return;
            }
            foreach (var item in entrydic)
            {
                item.Value.OnRefresh();
            }
            //for (int i=0;i<entrydic.Count;i++)
            //{
            //    KeyValuePair<int, UI_FightTreasure_Ceil> kv = entrydic.ElementAt(i);
            //    kv.Value.OnRefresh();
            //}
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void Show()
        {
            base.Show();
            OnDollar();

        }
        public override void Hide()
        {
            base.Hide();
            m_timer?.Cancel();
            entrydic.Clear();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle<uint>(Sys_OperationalActivity.EEvents.UpdateFightTreasureData, OnFightTreasure, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnOperatinalActivityShowOrHide, toRegister);
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, JumpDayRefresh, toRegister);
        }
        #endregion
        #region Function

        private void Parse()
        {
            PanelScrollGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            Sys_OperationalActivity.Instance.ScrollViewVect = CameraManager.mUICamera.WorldToScreenPoint(PanelScrollGrid.gameObject.GetComponent<RectTransform>().position);
            PanelScrollGrid.onCreateCell += OnCreateCell;
            PanelScrollGrid.onCellChange += OnCellChange;
        }

        private void OnDollar()
        {
            Sys_OperationalActivity.Instance.CheckFightActivityDictionary();
            if (Sys_OperationalActivity.Instance.FightActivityDic.ContainsKey(EActivityRulerType.OneDollarTreasure))
            {
                _id = Sys_OperationalActivity.Instance.FightActivityDic[EActivityRulerType.OneDollarTreasure].aId;
                Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = false;
                if (Sys_OperationalActivity.Instance.fightTreasureDic.ContainsKey(_id))
                {
                    
                    Sys_OperationalActivity.Instance.OnFightTreasureDataReq(_id);//请求数据

                }

            }
            else
            {
                ClearTemporaryHandle();
            }



        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_FightTreasure_Ceil entry = new UI_FightTreasure_Ceil();
            entry.BindGameObject(cell.mRootTransform);
            entry.AddRefreshListener(OnRefreshSingleGrid);
            cell.BindUserData(entry);
        }
        private void OnRefreshSingleGrid(int _round)
        {
            OnDollar();
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FightTreasure_Ceil entry = cell.mUserData as UI_FightTreasure_Ceil;
            entry.SetCeilData(index, sft.roundsList[index], sft.eType);
            entrydic[sft.roundsList[index].thisRoundIndex] = entry;
        }
        private void JumpDayRefresh()
        {
            //OnRefreshTotalGrid();
            OnDollar();
        }

        private void OnOperatinalActivityShowOrHide()
        {
            OnDollar();
        }

        private void ClearTemporaryHandle()
        {
            uint _id = 0;
            foreach (var item in Sys_OperationalActivity.Instance.fightTreasureDic)
            {
                if (item.Value.eType== EActivityRulerType.OneDollarTreasure)
                {
                    if (Sys_OperationalActivity.Instance.CheckFightActivityEnd(item.Key))
                    {
                        _id = item.Key;

                    }
                }
            }

            if (Sys_OperationalActivity.Instance.fightTreasureDic.TryGetValue(_id, out Sys_OperationalActivity.SingleFightType _sft))
            {
                entrydic.Clear();
                sft = _sft;
                sft.CheckNowRound();
                if (sft.InRound < 0)
                {
                    Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = true;
                }

                PanelScrollGrid.CellCount = sft.roundsList.Count;
                PanelScrollGrid.Apply();
                if (sft.recordRound >= 0)
                {
                    var _index = sft.recordRound - 1 < 0 ? 0 : sft.recordRound - 1;
                    PanelScrollGrid.MoveIndexToTop(_index);
                }
                PanelScrollGrid.ForceRefreshActiveCell();
            }
        }
        private void OnRefreshTotalGrid()
        {
            if (Sys_OperationalActivity.Instance.fightTreasureDic.TryGetValue(_id, out Sys_OperationalActivity.SingleFightType _sft))
            {
                entrydic.Clear();
                sft = _sft;
                sft.CheckNowRound();
                if (sft.InRound<0)
                {
                    Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = true;
                }
            }
            else
            {
                DebugUtil.LogError("夺宝活动id不匹配");
                return;
            }

            PanelScrollGrid.CellCount = sft.roundsList.Count;
            PanelScrollGrid.Apply();
            if (sft.recordRound >= 0)
            {
                var _index = sft.recordRound - 1 < 0 ? 0 : sft.recordRound - 1;
                PanelScrollGrid.MoveIndexToTop(_index);
            }
            PanelScrollGrid.ForceRefreshActiveCell();

        }

        private void OnFightTreasure(uint _type)
        {
            if (_type == 0)
            {
                OnRefreshTotalGrid();
            }
            else
            {
                entrydic[(int)_type].OnFightTreasureUpdateButton(sft.roundsList[(int)_type].applyNum);
            }

        }
        #endregion
    }

}
