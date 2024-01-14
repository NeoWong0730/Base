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
    public class UI_FamilyCreatures_Reward_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        //private Text nameText;
        private RawImage headImage;
        private Text levelText;
        private Text typeText;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            //nameText = transform.Find("Animator/View_Info/Title/Text").GetComponent<Text>();
            headImage = transform.Find("Animator/View_Info/Texture").GetComponent<RawImage>();
            levelText = transform.Find("Animator/View_Info/Image_Form/Text_Value").GetComponent<Text>();
            typeText = transform.Find("Animator/View_Info/Text_Name").GetComponent<Text>();
        }

        public void SetCurrentCreaturesInfo(uint creatureId)
        {
            CSVFamilyPet.Data cSVFamilyPetData = CSVFamilyPet.Instance.GetConfData(creatureId);
            ImageHelper.SetTexture(headImage, cSVFamilyPetData.icon2_id);
            levelText.text = LanguageHelper.GetTextContent(2023502, cSVFamilyPetData.stage.ToString());
            typeText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(cSVFamilyPetData.stage));
        }


        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_FamilyCreatures_RewardCeil
    {
        private GameObject notGo;
        private GameObject currentStarGo;
        private Text starText;
        private Text pointText;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private CSVFamilyPetTrainingStarReward.Data data;
        private List<ItemIdCount> items = new List<ItemIdCount>();
        public void Init(Transform transform)
        {
            notGo = transform.Find("StarLevel/Text_Not").gameObject;
            starText = transform.Find("StarLevel/Image_Star/Text_Value").GetComponent<Text>();
            pointText = transform.Find("Text_Point").GetComponent<Text>();
            currentStarGo = transform.Find("CurrentStar").gameObject;
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell = OnCreateCell;
            infinityGrid.onCellChange = OnCellChange;
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
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemdata.id, itemdata.count, true, false, false, false, false, true, false, true);
            entry.SetData(itemData, EUIID.UI_FamilyCreatures_Reward);
        }

        public void SetData(CSVFamilyPetTrainingStarReward.Data data, bool current)
        {
            this.data = data;
            currentStarGo.gameObject.SetActive(current);
            if(null != data)
            {
                bool hasStar = data.trainingStar != 0;
                starText.transform.parent.gameObject.SetActive(hasStar);
                notGo.SetActive(!hasStar);
                starText.text = data.trainingStar.ToString();
                pointText.text = data.trainingIntegralCondition.ToString();
                items.Clear();
                items = CSVDrop.Instance.GetDropItem(data.drop_id);
                infinityGrid.CellCount = items.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }
    }

    public class UI_FamilyCreatures_Reward : UIBase, UI_FamilyCreatures_Reward_Layout.IListener
    {
        private UI_FamilyCreatures_Reward_Layout layout = new UI_FamilyCreatures_Reward_Layout();
        private uint currentCreatureId;
        private List<CSVFamilyPetTrainingStarReward.Data>  datas = new List<CSVFamilyPetTrainingStarReward.Data>(5);

        private int starIndex = 0;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);            
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            currentCreatureId = guildPetTraining.TrainingStage;
            datas.Clear();
            for (int i = 1; i < 99; i++)
            {
                CSVFamilyPetTrainingStarReward.Data cSVFamilyPetTrainingStarReward = CSVFamilyPetTrainingStarReward.Instance.GetConfData((uint)(currentCreatureId * 10 + i));
                if(null != cSVFamilyPetTrainingStarReward)
                {
                    datas.Add(cSVFamilyPetTrainingStarReward);
                }
                else
                {
                    break;
                }
            }
        }

        protected override void OnShow()
        {
            starIndex = IsCurrentStar();
            layout.SetCurrentCreaturesInfo(currentCreatureId);
            layout.SetInfinityGridCell(datas.Count);
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilyCreatures_RewardCeil entry = new UI_FamilyCreatures_RewardCeil();
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
            if (index < 0 || index >= datas.Count)
                return;
            UI_FamilyCreatures_RewardCeil entry = cell.mUserData as UI_FamilyCreatures_RewardCeil;
            
            entry.SetData(datas[index], starIndex == index);
        }

        private int IsCurrentStar()
        {
            if(null != datas)
            {
                int currentIndex = -1;
                for (int i = 0; i < datas.Count; i++)
                {
                    if(Sys_Family.Instance.totalScore >= datas[i].trainingIntegralCondition)
                    {
                        currentIndex = i;
                    }
                }
                return currentIndex;
            }
            else
            {
                return 0;
            }
        }
        
        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Reward, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures_Reward);
        }

    }
}