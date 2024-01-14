using Framework;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Table;
using Lib.Core;

namespace Logic
{
    public partial class UI_Lotto : UIBase, UI_Lotto_Layout.IListener
    {
        UI_Lotto_Layout m_Layout = new UI_Lotto_Layout();


        private int m_Mode = 0;
        private uint m_ResultFlag = 0;

        private CmdLuckyDrawDrawRes m_ResultInfo;

        private int m_OnceCost = 0;
        private int m_ManyCost = 0;

        private int m_BigOnceCost = 0;
        private int m_BigManyCost = 0;

        private uint m_CostItemID = 0;

        static private int m_LottoType = 1; // 0 大额 ，1 普通

        private float m_LastClickTime = 0;

        private uint m_ExtraId = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

            m_Layout.LoadLottoResult(gameObject.transform);

            m_Layout.SetResultListener();

            m_OnceCost = GetCost(804);

            m_ManyCost = GetCost(805);

            m_BigOnceCost = GetCost(1026);

            m_BigManyCost = GetCost(1027);

            m_CostItemID = Sys_pub.Instance.CostItemID;

            m_ExtraId = Sys_pub.Instance.ExtraID;

            LoadShowScene();
           

            m_Layout.SetType(m_LottoType);

            m_Layout.SetAutoCompletement(Sys_pub.Instance.isAutoCompletementToggleCheck);

            m_Layout.SetCoinImage();

            m_Layout.SetLottoRewardResult(UI_Lotto_Layout.LottoState.LottoMain);
        }

        private int GetCost(uint parmeid)
        {

            var costParamData = CSVParam.Instance.GetConfData(parmeid);

            int value = costParamData == null ? 0 : int.Parse(costParamData.str_value);

            return value;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_pub.Instance.eventEmitter.Handle<CmdLuckyDrawDrawRes>(Sys_pub.EEvents.Result, OnLotoResult, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnItemCountChanged, toRegister);
        }
        protected override void OnOpen(object arg)
        {            
            m_ResultInfo = null;
        }

        protected override void OnShow()
        {            
            Refresh();

            m_Layout.SetShowSceneCameraActive(true);

            m_Layout.SetCoinImage();

            Sys_pub.Instance.SetLottoLoginRedPointState();
        }

        protected override void OnHide()
        {            
            m_Layout.SetShowSceneCameraActive(false);

            m_LastClickTime = 0;
        }

        protected override void OnClose()
        {            
            m_Layout.OnCloseLayout();

            m_Layout.UnLoadScene();
        }        

        private void OnItemCountChanged(int changeType, int curBoxId)
        {
            RefreshCount();
            RefreshLottoType();
        }
        private void Refresh()
        {
            RefreshCount();

            RefreshLottoType();
        }


        private void RefreshCount()
        {
            var count = Sys_pub.Instance.GetItemCount();

            m_Layout.SetItemCount(count);

            var coinCount = Sys_Bag.Instance.GetValueFormat(Sys_Bag.Instance.GetItemCount(1));
            m_Layout.SetCoinCount(coinCount);
        }
        private void LoadShowScene()
        {
            m_Layout.LoadShowScene();
        }

        /// <summary>
        /// 播放抽奖特效
        /// </summary>
        /// <param name="flag">//0: 1抽 1: 5抽 2:高级1抽 3:高级5抽</param>
        private void ShowEffect(uint flag)
        {
            m_Layout.SetMessageActive(false);

            m_ResultFlag = flag;

            m_Layout.SetLottoRewardResult(UI_Lotto_Layout.LottoState.PlayTimeLine);
            RefreshTimeLineShow();
        }

        List<ulong> petUIds = new List<ulong>();
        private void RefreshTimeLineShow()
        {
            bool goldLight = m_ResultInfo.PetUID.Count > 0;
            bool normal = m_ResultFlag < 2;
            m_Layout.SetCardFx( (UI_Lotto_Layout.CardFxState)(normal ?  (goldLight? 2: 0):(goldLight ? 3: 1)));
            switch (m_ResultFlag)
            {
                case 0:
                    m_Layout.SetCardAndModelMode(true);
                    m_Layout.SetOneCardFx(m_ResultInfo.AwardId[0]);
                    m_Layout.PlayOpen1();
                    break;
                case 1:
                    m_Layout.SetCardAndModelMode(true);
                    m_Layout.SetFiveCardsFx(m_ResultInfo.AwardId);
                    m_Layout.PlayOpen5();
                    break;
                case 2:
                    m_Layout.SetCardAndModelMode(false);
                    m_Layout.SetOneCardFx(m_ResultInfo.AwardId[0]);
                    m_Layout.PlayOpen1();
                    break;
                case 3:
                    m_Layout.SetCardAndModelMode(false);
                    m_Layout.SetFiveCardsFx(m_ResultInfo.AwardId);
                    m_Layout.PlayOpen5();
                    break;
            }
        }

        private int NeedItemCount(int mode)
        {
            int value = 0;

            if (mode == 0)
                value = m_LottoType == 0 ? m_BigOnceCost : m_OnceCost;

            else if (mode == 1)
                value = m_LottoType == 0 ? m_BigManyCost : m_ManyCost;

            return value;
        }
        private void DoLotto(int mode)
        {
            if (Time.time - m_LastClickTime < 1)
                return;

            m_Mode = mode;

            m_LastClickTime = Time.time;

            int needcount = NeedItemCount(mode);

            if (Sys_pub.Instance.isItemEnuge(needcount) == false)
            {
                if(m_Layout.IsAutoCompletementToggleCheck())
                {
                    var count = Sys_pub.Instance.GetItemCount();
                    var lostCount = needcount - count;
                    needMoCoin = lostCount * Sys_pub.Instance.SinglePrice;
                    if (Sys_pub.Instance.showBuyPromptBox)
                    {
                        string costStr = needMoCoin.ToString();
                        PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(2021132,lostCount.ToString(), costStr), 0, false, OnUseCashToBuy, null, OnValueChanged);
                    }
                    else
                    {
                        if (needMoCoin > Sys_Bag.Instance.GetItemCount(1))
                        {
                            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(8203), 0, OnBuyMoCoin);
                        }
                        else
                        {
                            int needfreeGridcount = (int)(Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdNormal) + Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdTemporary));

                            if (needfreeGridcount < needcount)
                                return;

                            Sys_pub.Instance.Apply_Lotto(m_LottoType == 0 ? (m_Mode + 2) : m_Mode, true);

                            return;
                        }
                    }
                }
                else
                    PromptBoxParameter.Instance.OpenPromptBox(2021113, 0, OnSkipToCompare);
                return;
            }

            int freeGridcount = (int)(Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdNormal) + Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdTemporary));

            if (freeGridcount < needcount)
                return;

            Sys_pub.Instance.Apply_Lotto(m_LottoType == 0 ? (m_Mode + 2) : m_Mode, false);
        }

        public void RefreshLottoType()
        {
            var needCountOnce = NeedItemCount(0);
            m_Layout.SetCostOnceTex(needCountOnce,Sys_pub.Instance.isItemEnuge(needCountOnce));

            var needCountMany = NeedItemCount(1);
            m_Layout.SetCostManyTex(needCountMany,Sys_pub.Instance.isItemEnuge(needCountMany));

            if (m_LottoType == 0)
                m_Layout.RefreshLottoTicketShow();
        }

        void RefreshResultTicketNum()
        {
            var needCountOne = NeedItemCount(0);
            m_Layout.SetResultOneTex(needCountOne, Sys_pub.Instance.isItemEnuge(needCountOne));

            var needCountMany = NeedItemCount(1);
            m_Layout.SetResultFiveTex(needCountMany, Sys_pub.Instance.isItemEnuge(needCountMany));
        }
    }

    public partial class UI_Lotto : UIBase, UI_Lotto_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickOnce()
        {
            if (Sys_OperationalActivity.Instance.CheckLotteryActivityIsOpen())
            {
                if (!Sys_pub.Instance.canDoLotto)
                {
                    if (Time.time - Sys_pub.Instance.lastLottoTime > Sys_pub.lottoInternalTime)
                    {
                        Sys_pub.Instance.canDoLotto = true;
                    }
                }
                if (Sys_pub.Instance.canDoLotto)
                {
                    if (m_LottoType == 0)
                    {
                        UIManager.HitButton(EUIID.UI_Lotto, "One_King", "");
                    }
                    else
                    {
                        UIManager.HitButton(EUIID.UI_Lotto, "One_Normal", "");
                    }
                    DoLotto(0);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021136));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021135));
            }
        }

        public void OnClickMany()
        {
            if (Sys_OperationalActivity.Instance.CheckLotteryActivityIsOpen())
            {
                if (!Sys_pub.Instance.canDoLotto)
                {
                    if (Time.time - Sys_pub.Instance.lastLottoTime > Sys_pub.lottoInternalTime)
                    {
                        Sys_pub.Instance.canDoLotto = true;
                    }
                }
                if (Sys_pub.Instance.canDoLotto)
                {
                    if (m_LottoType == 0)
                    {
                        UIManager.HitButton(EUIID.UI_Lotto, "Five_King", "");
                    }
                    else
                    {
                        UIManager.HitButton(EUIID.UI_Lotto, "Five_Normal", "");
                    }
                    DoLotto(1);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021136));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021135));
            }
        }

        public void OnOneAnimationEnd(PlayableDirector playableDirector)
        {
            OnShowLotoResult();
        }

        public void OnFiveAnimationEnd(PlayableDirector playableDirector)
        {
            OnShowLotoResult();
        }

        public void OnClickAwardResultClose()
        {
            Refresh();
            m_Layout.SetLottoRewardResult(UI_Lotto_Layout.LottoState.LottoMain);
        }

      
        public void OnClickReawrdDetail()
        {
            UIManager.OpenUI(EUIID.UI_Lotto_Preview, false, m_LottoType);
        }

        public void OnClickDetail()
        {

        }

        public void OnClickAddCoin()
        {
            Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.Diamonds, needMoCoin);
        }

        public void OnClickRule()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2021116) });
        }

        public void OnClickSkip()
        {
            switch (m_ResultFlag)
            {
                case 0:
                case 2:
                    m_Layout.PlayOpen1ToEnd();
                    break;
                case 1:
                case 3:
                    m_Layout.PlayOpen5ToEnd();
                    break;
            }
        }
        public void OnClickLottoIcon()
        {
            m_Layout.ShowItemInfo(m_CostItemID);
        }
        private void OnLotoResult(CmdLuckyDrawDrawRes info)
        {
            if (info.Ret != 0)
            {
                DebugUtil.LogError("lotto error code: " + info.Ret);

                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(info.Ret + 100000));

                return;
            }

            m_ResultInfo = info;
            if (info.Flag > 1)
            {
                Sys_pub.Instance.CurrentLottoTimes = info.Timesrst;
            }

            ShowEffect(info.Flag);
        }


        private void OnSkipToCompare()
        {
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

            iItemData.id = m_CostItemID;

            var boxEvt = new MessageBoxEvt(EUIID.UI_Lotto, iItemData);

            boxEvt.b_ForceShowScource = true;

            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }

        long needMoCoin = 0;

        private void OnUseCashToBuy()
        {
            if (needMoCoin > Sys_Bag.Instance.GetItemCount(1))
            {
                Lib.Core.Timer.Register(0.05f, () =>
                 {
                     PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(8203), 0, OnBuyMoCoin);
                 });
            }
            else
            {
                int needcount = NeedItemCount(m_Mode);
                int freeGridcount = (int)(Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdNormal) + Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdTemporary));

                if (freeGridcount < needcount)
                    return;

                Sys_pub.Instance.Apply_Lotto(m_LottoType == 0 ? (m_Mode + 2) : m_Mode, true);
            }
        }

        void GetItemTips()
        {
            for(int i = 0; i < m_ResultInfo.Itemid.Count; ++i)
            {
                var data = CSVAward.Instance.GetConfData(m_ResultInfo.AwardId[i]);
                m_Layout.PushTips(m_ResultInfo.Itemid[i], m_ResultInfo.Itemcount[i], data.isBroadCast !=0);
            }

            if (m_ResultInfo.ExtraGiftNum > 0)
                m_Layout.PushTips(m_ExtraId, m_ResultInfo.ExtraGiftNum, false);
        }

        private void OnBuyMoCoin()
        {
            Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.Diamonds, needMoCoin);
        }

        private void OnValueChanged(bool value)
        {
            Sys_pub.Instance.showBuyPromptBox = !value;
        }

        private void OnShowLotoResult()
        {
            switch(m_ResultFlag)
            {
                case 0:
                case 2:
                    m_Layout.SetOneReward(m_ResultInfo.AwardId);
                    break;
                case 1:
                case 3:
                    m_Layout.SetFiveRewards(m_ResultInfo.AwardId);
                    break;
            }
            RefreshResultTicketNum();
            GetItemTips();
            ShowPetEffect();
            m_Layout.SetRewardExtra(m_ExtraId, m_ResultInfo.ExtraGiftNum);
            m_Layout.SetLottoRewardResult(UI_Lotto_Layout.LottoState.LottoResult);
        }

        void ShowPetEffect()
        {
            if (m_ResultInfo.PetUID.Count >0 )
            {
                for (int i = 0;i < m_ResultInfo.PetUID.Count; ++i)
                {
                    ulong petUID = m_ResultInfo.PetUID[i];
                    var petInfo = Sys_Pet.Instance.GetPetByUId((uint)petUID);
                    if(petInfo != null)
                    {
                        UIManager.OpenUI(EUIID.UI_Pet_Get, false, petInfo);
                    }
                }
            }
        }


        public void OnBigLottoState(bool b)
        {
            if (b)
            {
                m_LottoType = 0;
                RefreshLottoType();
            }
        }

        public void OnNormalLottoState(bool b)
        {
            if (b)
            {
                m_LottoType = 1;
                RefreshLottoType();
            }
        }

        public void OnAutoCompletementState(bool b)
        {
            Sys_pub.Instance.isAutoCompletementToggleCheck = b;
        }
    }
}
