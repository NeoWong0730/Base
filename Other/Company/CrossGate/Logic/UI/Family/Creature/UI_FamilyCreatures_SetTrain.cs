using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_FamilyCreatures_SetTrain_Layout
    {
        private Button closeBtn;
        private Button setBtn;
        private Button confirmBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private Transform iconsTran;
        public Transform IconsTran { get => iconsTran; }

        private Text setTimeText;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Right/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            setBtn = transform.Find("Animator/View_Right/Btn_Set").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/View_Right/Btn_Confirm").GetComponent<Button>();
            iconsTran = transform.Find("Animator/View_LeftList/Toggle_Group");
            setTimeText =transform.Find("Animator/View_Right/Image_Time/Text_Time").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            setBtn.onClick.AddListener(listener.SetBtnClicked);
            confirmBtn.onClick.AddListener(listener.ConfirmBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetOperateBtnState(bool state)
        {
            setBtn.gameObject.SetActive(state);
            confirmBtn.gameObject.SetActive(state);
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void SetTime(string timeStr)
        {
            setTimeText.text = timeStr;
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void SetBtnClicked();
            void ConfirmBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_FamilyCreatures_SetTrainCeil : UIComponent
    { 
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private List<ItemIdCount> items = new List<ItemIdCount>();
        public Image iconImage;
        public Text stageText;
        public Text levelText;
        public Text difficultyText;
        public GameObject currentTrainObj;
        public GameObject currentObj;
        public GameObject selectObj;
        public CSVFamilyPet.Data data;
        public CP_Toggle toggle;

        public interface IListener
        {
            void OnTrainClicked(CSVFamilyPet.Data data);
        }

        private IListener listener;

        public UI_FamilyCreatures_SetTrainCeil(IListener listener)
        {
            this.listener = listener;
        }

        protected override void Loaded()
        {
            toggle = transform.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener((b) =>
            {
                if (b)
                {
                    if (null != data)
                    {

                        listener?.OnTrainClicked(data);
                    }
                }
            });
            iconImage = transform.Find("Icon").GetComponent<Image>();
            stageText = transform.Find("Image_Stage/Text").GetComponent<Text>();
            levelText = transform.Find("Text_Recommend/Text_Value").GetComponent<Text>();
            difficultyText = transform.Find("Text_Difficulty/Text_Describe").GetComponent<Text>();
            currentObj = transform.Find("Image_Currene").gameObject;
            currentTrainObj = transform.Find("Image_Check").gameObject;
            selectObj = transform.Find("Image_Select").gameObject;

            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell = OnCreateCell;
            infinityGrid.onCellChange = OnCellChange;
        }

        public void SetInfo(CSVFamilyPet.Data data, bool isSelect)
        {
            this.data = data;
            if (data != null)
            {
                ImageHelper.SetIcon(iconImage, data.icon_id);
                stageText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(data.stage));
                currentObj.SetActive(Sys_Family.Instance.IsHasCreatureState(data.id));
                selectObj.SetActive(isSelect);
                GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
                currentTrainObj.SetActive(data.id == guildPetTraining.TrainingStage);
                if(null != data.train_id && data.train_id.Count >= 2)
                {
                    levelText.text = LanguageHelper.GetTextContent(2023730, data.train_id[0].ToString(), data.train_id[1].ToString());
                }
                TextHelper.SetText(difficultyText, data.simpleText);
                items.Clear();
                items = CSVDrop.Instance.GetDropItem(data.rewardPreview);
                infinityGrid.CellCount = items.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            PropItem entry = new PropItem();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BindGameObject(go);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= items.Count)
                return;
            PropItem entry = cell.mUserData as PropItem;
            ItemIdCount itemdata = items[index];
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemdata.id, itemdata.count, false, false, false, false, false, false, false, true);
            entry.SetData(itemData, EUIID.UI_FamilyCreatures_SetTrain);
        }
    }

    public class UI_FamilyCreatures_SetTrain : UIBase, UI_FamilyCreatures_SetTrain_Layout.IListener, UI_FamilyCreatureIconController.IListener, UI_FamilyCreatures_SetTrainCeil.IListener
    {
        private UI_FamilyCreatures_SetTrain_Layout layout = new UI_FamilyCreatures_SetTrain_Layout();
        private UI_FamilyCreatureIconController iconController;
        List<CSVFamilyPet.Data>  cSVFamilyPetDatas = new List<CSVFamilyPet.Data>();
        private uint time;
        private int index = -1;
        private uint selectId = 0;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            iconController = new UI_FamilyCreatureIconController(this);
            iconController.Init(layout.IconsTran);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnSetTrainInfoEnd, RefreshByIndex, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
        }

        protected override void OnShow()
        {
            if (!Sys_Family.Instance.familyData.isInFamily)
            {
                CloseSelf();
                return;
            }
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            layout.SetOperateBtnState(Sys_Family.Instance.IsFamilyCreaturesOpenDate() && Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetTraining) && !guildPetTraining.BSet);
                
            if (index == -1)
                index = 0;
            iconController.SetListInfo(index, true);
            
            time = guildPetTraining.StartTime;
            ChangeTimeType();
            selectId = guildPetTraining.TrainingStage;
            RefreshByIndex();
        }

        private void Refresh()
        {

        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnClose()
        {
            index = -1;
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilyCreatures_SetTrainCeil entry = new UI_FamilyCreatures_SetTrainCeil(this);
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= cSVFamilyPetDatas.Count)
                return;
            UI_FamilyCreatures_SetTrainCeil entry = cell.mUserData as UI_FamilyCreatures_SetTrainCeil;
            
            entry.SetInfo(cSVFamilyPetDatas[index], selectId == cSVFamilyPetDatas[index].id);
        }

        private uint GetCurrentSecond()
        {
            DateTime dateTime = Sys_Time.ConvertToLocalTime(Sys_Time.Instance.GetServerTime());
            int hour = dateTime.Hour;
            int min = dateTime.Minute;
            int second = dateTime.Second;
            //当前时间的当日秒时
            return (uint)(hour * 3600 + min * 60 + second);
        }

        private bool CheckActiveIsOpen()
        {
            return false;
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTrain, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures_SetTrain);
        }

        public void OnSelectListIndex(int index)
        {
            this.index = index;
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            selectId = guildPetTraining.TrainingStage;
            RefreshByIndex();
        }

        private void RefreshByIndex()
        {
            FamilyCreatureEntry familyCreatureEntry = Sys_Family.Instance.GetFamilyCreatureByIndex(index);

            cSVFamilyPetDatas.Clear();

            var dataList = CSVFamilyPet.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                var tempData = dataList[i];
                if (familyCreatureEntry.cSV.food_Type == tempData.food_Type && familyCreatureEntry.ConfigId >= tempData.id)
                {
                    cSVFamilyPetDatas.Add(tempData);
                }
            }
            if (cSVFamilyPetDatas.Count > 1)
            {
                cSVFamilyPetDatas.Sort((a, b) => {
                    return (int)a.stage - (int)b.stage;
                });
            }
            layout.SetInfinityGridCell(cSVFamilyPetDatas.Count);
        }

        public void OnTrainClicked(CSVFamilyPet.Data data)
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTrain, "OnTrainClicked:" + data.id.ToString());
            selectId = data.id;
        }

        public void SetTimeHandle(uint time)
        {
            this.time = time;
            ChangeTimeType();
        }

        private void ChangeTimeType()
        {
            uint Hour = time / 3600;
            uint Minus = time % 3600 / 60;
            layout.SetTime(string.Format("{0}:{1}:00", Hour.ToString("D2"), Minus.ToString("D2")));
        }

        public void SetBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTrain, "SetBtnClicked");
            UI_FamilyCreatures_SetTime.UI_FamilyCreatures_SetTimeParam param = new UI_FamilyCreatures_SetTime.UI_FamilyCreatures_SetTimeParam();
            uint Hour = time / 3600;
            uint Minus = time % 3600 / 60;
            param.currentHour = Hour;
            param.currentMinus = Minus;
            param.action = SetTimeHandle;
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_SetTime, false, param);
        }

        public void ConfirmBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTrain, "ConfirmBtnClicked");
            if (!Sys_Family.Instance.IsFamilyCreaturesOpenDate())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023415));
                return;
            }
            else if (CheckActiveIsOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023418));
                return;
            }
            GuildPetTraining Server = Sys_Family.Instance.GetTrainInfo();
            if (Server.BSet)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023417));
                return;
            }

            //活动将会持续的时间
            uint durationTime = Convert.ToUInt32(CSVParam.Instance.GetConfData(1264).str_value);
            //活动最低配置时间秒时
            uint minTime = Sys_Family.Instance.MinTime;
            //活动可配置准备时间
            uint readyTime = Convert.ToUInt32(CSVParam.Instance.GetConfData(1266).str_value);

            //当日总秒时
            uint ADaySceond = 86400;
            //最后可配置时间
            uint lastSetTime = ADaySceond - durationTime - readyTime;
            // 当前时间的当日秒时
            uint currentTimer = GetCurrentSecond();

            //当日的0点时间戳
            //ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();

            //设置的秒时
            uint setTime = time;

            uint lastHour = lastSetTime / 3600;
            uint lastMinus = lastSetTime % 3600 / 60;
            // （最后可设置时间 = 86399 - 准备时间+ 持续时间 ）如果小于 当前的秒数（从当日0点开始算秒）
            if (lastSetTime <= currentTimer)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023419, lastHour.ToString(), lastMinus.ToString()));
                return;
            }

            //设置的时间 小于 当前时间+准备时间
            if (setTime <= currentTimer + readyTime || setTime >= lastSetTime)
            {
                uint tempTime = currentTimer + readyTime;
                uint eHour = tempTime / 3600;
                uint eminus = tempTime % 3600 / 60;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023420, eHour.ToString(), eminus.ToString(), lastHour.ToString(), lastMinus.ToString()));
                return;
            }
           
            if (Server.StartTime == time && Server.TrainingStage == selectId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023416));
                return;
            }
            PromptBoxParameter.Instance.OpenPromptBox(2023402, 0,()=> {
                GuildPetTraining guildPetTraining = new GuildPetTraining();
                guildPetTraining.StartTime = time;
                guildPetTraining.TrainingStage = selectId;
                Sys_Family.Instance.GuildPetSetTrainingReq(guildPetTraining);
            });
        }
    }
}