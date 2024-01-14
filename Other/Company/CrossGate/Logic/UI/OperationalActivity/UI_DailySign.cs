 using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Packet;
using DG.Tweening;

namespace Logic
{
    public class UI_AwardItem
    {
        public GameObject itemGo;
        private GameObject select;
        private GameObject forbidGo;
        private Text count;

        public uint ItemId;
        public uint signId;
        public int itemPos;
     

        public void ParseComponent(Transform transform)
        {
            itemGo = transform.gameObject;
            select = transform.Find("Image_Select").gameObject;
            forbidGo = transform.Find("Image_Forbid").gameObject;
            count = transform.Find("Text_Number").GetComponent<Text>();
        }

        public void SetData(uint itemId,uint id,int pos)
        {
            ItemId = itemId;
            signId = id;
            itemPos = pos;
            bool showquailty = itemPos != 11;
            PropItem propItem = new PropItem();
            propItem.BindGameObject(itemGo);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, new PropIconLoader.ShowItemData(itemId, 1, showquailty, false, false, false, false, true, false, true,null,false,true)));
            forbidGo.SetActive(Sys_Sign.Instance.awardTakeList.Contains(signId));
            count.text = CSVSign.Instance.GetConfData(signId).Count.ToString();
        }

        public void SetTakeData()
        {
            forbidGo.SetActive(true);
        }
    }

    public class UI_Award_Record_Tips
    {
        private Text text;
        private CSVItem.Data itemData;
        public void ParseComponent(Transform transform)
        {
            text = transform.gameObject.GetComponent<Text>();
        }

        public void SetData(DailySignRecord data)
        {
            uint itemId = CSVSign.Instance.GetConfData(data.InfoId).Reward;
            uint count = CSVSign.Instance.GetConfData(data.InfoId).Count;
            itemData = CSVItem.Instance.GetConfData(itemId);
            string itemName = LanguageHelper.GetLanguageColorWordsFormat(CSVLanguage.Instance.GetConfData(itemData.name_id).words, TextHelper.GetQuailtyLangId(itemData.quality));
            text.text = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(5224), data.RoleName, itemName, count.ToString());
        }
    }

    public class UI_DailySign : UI_OperationalActivityBase
    {
        private Text refreshTime;
        private Text leftTime;
        private Button awardBtn;
        private Button addBtn;
        private Toggle playToggle;
        private GameObject itemParentGo;
        private GameObject getTipsParentGo;
        private GameObject getTipsItem;
        private GameObject getTipsBg;
        private RectTransform contentRect;

        private Timer fxTimer;
        private uint limitBuyCount;
        private List<UI_AwardItem> items = new List<UI_AwardItem>();
        private List<UI_AwardItem> circleItems = new List<UI_AwardItem>();
        private List<UI_Award_Record_Tips> recordItems = new List<UI_Award_Record_Tips>();
        private RewardCircle circle;
        private bool isPlayCircleAni=false;

        private float startPos;
        private float endPos;
        private float speed;
        private bool startRolling;

        protected override void Loaded()
        {
        }

        protected override void InitBeforOnShow()
        {
            refreshTime = transform.Find("Tips1").GetComponent<Text>();
            leftTime = transform.Find("Time/Value").GetComponent<Text>();
            awardBtn = transform.Find("Btn_Play").GetComponent<Button>();
            awardBtn.onClick.AddListener(OnAwardBtn);
            addBtn = transform.Find("Time/Btn_add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnAddBtn);
            playToggle = transform.Find("Toggle").GetComponent<Toggle>();
            playToggle.onValueChanged.AddListener(onPlayToggleValueChanged);
            itemParentGo = transform.Find("Award").gameObject;
            getTipsParentGo = transform.Find("Scroll View/Viewport/Content").gameObject;
            contentRect = getTipsParentGo.GetComponent<RectTransform>();
            getTipsItem = transform.Find("Scroll View/Viewport/Content/Item").gameObject;
            getTipsBg = transform.Find("Scroll View").gameObject;
            getTipsItem.SetActive(false);
            getTipsBg.SetActive(false);

            uint.TryParse(CSVParam.Instance.GetConfData(1007).str_value, out limitBuyCount);
            circle = AddComponent<RewardCircle>(itemParentGo.transform);
            circle.lightTrans = transform.Find("Award/Image_Light");
        }
        public override void Show()
        {
            base.Show();
            SetData();
            InitAwardPoolData();
            InitRecordItem();
            playToggle.gameObject.SetActive(true);
            playToggle.isOn = false;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Sign.Instance.eventEmitter.Handle(Sys_Sign.EEvents.UpdateDailySignAwardCount, OnUpdateDailySignAwardCount, toRegister);
            Sys_Sign.Instance.eventEmitter.Handle(Sys_Sign.EEvents.UpdateDailySignAwardPool, OnUpdateDailySignAwardPool, toRegister);
            Sys_Sign.Instance.eventEmitter.Handle<uint>(Sys_Sign.EEvents.UpdateTakeDailySignAward, OnUpdateTakeDailySignAward, toRegister);
            Sys_Sign.Instance.eventEmitter.Handle(Sys_Sign.EEvents.UpdateDailySignRecord, OnUpdateDailySignRecord, toRegister);
            Sys_Sign.Instance.eventEmitter.Handle(Sys_Sign.EEvents.CompletePlayingCircle, OnCompletePlayingCircle, toRegister);
        }

        public override void Hide()
        {
            base.Hide();

            fxTimer?.Cancel();
            items.Clear();
            circle?.OnHide();
            recordItems.Clear();
            DefaultRecordItems();
            DefaultRollingData();
        }

        #region CallBack
        private void OnUpdateDailySignAwardCount()
        {
            SetData(); 
        }

        private void OnUpdateDailySignAwardPool()
        {
            InitAwardPoolData();
        }

        private void OnUpdateDailySignRecord()
        {
            DefaultRecordItems();
            InitRecordItem();
        }

        private void OnCompletePlayingCircle()
        {
            circleItems.Clear();
            for (int i = 0; i < items.Count; ++i)
            {
                if (!Sys_Sign.Instance.awardTakeList.Contains(items[i].signId))
                {
                    circleItems.Add(items[i]);
                }
            }
            circle.items = circleItems;
            circle.rewardTransArray = new Transform[circleItems.Count];
            for (int i = 0; i < circleItems.Count; ++i)
            {
                circle.rewardTransArray[i] = circleItems[i].itemGo.transform;
            }
        }

        private void OnUpdateTakeDailySignAward(uint rewardSignId)
        {
            if (isPlayCircleAni)
            { 
                circle.StartCircle(rewardSignId);
            }
            else
            {
                int index = 0; 
                for (int i = 0; i < circleItems.Count; ++i)
                {
                    if (Sys_Sign.Instance.awardTakeList.Contains(circleItems[i].signId))
                    {
                        circleItems[i].SetTakeData();
                        index = i;
                    }
                }
                uint rewardItemId = CSVSign.Instance.GetConfData(rewardSignId).Reward;
                uint count = CSVSign.Instance.GetConfData(rewardSignId).Count;
                string str = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(5223), LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(rewardItemId).name_id), count.ToString());
                circle.SetAwardLightTrans(index);
                Sys_Hint.Instance.PushContent_Normal(str);
                Sys_Sign.Instance.eventEmitter.Trigger(Sys_Sign.EEvents.CompletePlayingCircle);
            }
        }
        #endregion

        #region Function
        private void SetData()
        {       
            leftTime.text = Sys_Sign.Instance.awardCount.ToString();
            ImageHelper.SetImageGray(awardBtn.GetComponent<Image>(), Sys_Sign.Instance.awardCount <= 0,true);
        }

        private void InitAwardPoolData()
        {
            items.Clear();
            for (int i = 0; i < Sys_Sign.Instance.awardPoolList.Count; ++i)
            {
                UI_AwardItem item = new UI_AwardItem();
                item.ParseComponent(itemParentGo.transform.GetChild(i));
                item.SetData(CSVSign.Instance.GetConfData(Sys_Sign.Instance.awardPoolList[i]).Reward, Sys_Sign.Instance.awardPoolList[i],i);
                items.Add(item);
            }
            circleItems.Clear();
            for (int i = 0; i < items.Count; ++i)
            {
                if (!Sys_Sign.Instance.awardTakeList.Contains(items[i].signId))
                {
                    circleItems.Add(items[i]);
                }
            }
            circle.items = circleItems;
            if (circleItems.Count == 0)
            {
                circle.lightTrans.gameObject.SetActive(false);
            }
            else
            {
                circle.lightTrans.gameObject.SetActive(true);
                circle.rewardTransArray = new Transform[circleItems.Count];
                for (int i = 0; i < circleItems.Count; ++i)
                {
                    circle.rewardTransArray[i] = circleItems[i].itemGo.transform;
                }
                circle.lightTrans.position = circleItems[0].itemGo.transform.position;
            }
            DateTime dt = TimeManager.GetDateTime(Sys_Sign.Instance.refreshTime);         
            refreshTime.text =LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(5203), dt.Month.ToString(), dt.Day.ToString(),dt.Hour.ToString());
        }

        private void InitRecordItem()
        {
            if (Sys_Sign.Instance.dailySignRecordList.Count == 0)
            {
                return;
            }
            int count = Sys_Sign.Instance.dailySignRecordList.Count;
            getTipsBg.SetActive(true);
            FrameworkTool.CreateChildList(getTipsParentGo.transform, count);
            for (int i = 0; i < count; i++)
            {
                GameObject go = getTipsParentGo.transform.GetChild(i).gameObject;
                go.SetActive(true);
                UI_Award_Record_Tips item = new UI_Award_Record_Tips();
                item.ParseComponent(go.transform);
                item.SetData(Sys_Sign.Instance.dailySignRecordList[i]);
                recordItems.Add(item);
            }
            ForceRebuildLayout(getTipsParentGo);
            DefaultRollingData();
            endPos =(count * 200 + (count - 1) * 20)*-1;
            startRolling = true;
            speed = count * 5f;
            SetRollingContent();
        }

        private void SetRollingContent()
        {
            if (startRolling)
            {
                contentRect.transform.DOLocalMoveX(endPos, speed).SetEase(Ease.Linear).onComplete += () => 
                {
                    contentRect.anchoredPosition = new Vector2(startPos, contentRect.anchoredPosition.y);
                    SetRollingContent();
                };
            }
        }

        private void DefaultRollingData()
        {
            startPos = 780;
            endPos = 0;
            startRolling = false;
            if (contentRect != null)
            {
                contentRect.anchoredPosition = new Vector2(startPos, contentRect.anchoredPosition.y);
                DOTween.Kill(contentRect);
            }
        }

        private void DefaultRecordItems()
        {
            if (getTipsParentGo != null && getTipsItem != null)
            {
                FrameworkTool.DestroyChildren(getTipsParentGo, getTipsItem.name);
            }
        }


        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }


        #endregion

        #region Button

        private void OnAwardBtn()
        {
            if (!Sys_Sign.Instance.CheckDailySignIsOpen())
            {
                Sys_Hint.Instance.PushContent_Normal( LanguageHelper.GetTextContent(5227));
                return; 
            }
            if (circle.isPlaying)
            {
                return;
            }
            if (Sys_Sign.Instance.awardTakeList.Count == 30)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5225));
                return;
            }
            if (Sys_Sign.Instance.awardCount > 0)
            {
                Sys_Sign.Instance.SignDailySignDrawReq();
                UIManager.HitButton(EUIID.UI_OperationalActivity, "ChouJiang", EOperationalActivity.DailySign.ToString());
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5221));
            }
        }

        private void OnAddBtn()
        {
            if (!Sys_Sign.Instance.CheckDailySignIsOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5228));
                return;
            }
            if (Sys_Time.IsServerSameDay(Sys_Sign.Instance.buyTime, Sys_Time.Instance.GetServerTime()))
            {
                if (limitBuyCount <= Sys_Sign.Instance.buyCount)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5220));
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_SignBuy);
                }
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_SignBuy);
            }
        }

        private void onPlayToggleValueChanged(bool isClose)
        {
            isPlayCircleAni =! isClose;
        }
        #endregion
    }
}
