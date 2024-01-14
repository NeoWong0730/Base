using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public class UI_Knowledge_Cooking_Award : UIBase
    {
        private Transform m_RewardCeilParent;
        private List<CookReward> m_RewardCeils = new List<CookReward>();
        private Image m_ClickCloseBG;

        protected override void OnLoaded()
        {
            m_RewardCeilParent = transform.Find("Animator/Image_Bg/Scroll_View01/Viewport");
            m_ClickCloseBG = transform.Find("Animator/eventBg").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_ClickCloseBG);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnClickClose);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle<uint>(Sys_Cooking.EEvents.OnGetReward, OnGetReward, toRegister);
        }

        protected override void OnShow()
        {
            BuildRewardList();
        }

        private void BuildRewardList()
        {
            m_RewardCeils.Clear();
            int needCount = Sys_Cooking.Instance.showRewards.Count;
            FrameworkTool.CreateChildList(m_RewardCeilParent, needCount);
            FrameworkTool.ForceRebuildLayout(m_RewardCeilParent.gameObject);
            for (int i = 0; i < needCount; i++)
            {
                GameObject go = m_RewardCeilParent.GetChild(i).gameObject;
                CookReward rewardCeil = new CookReward();
                rewardCeil.BindGameObject(go);
                rewardCeil.SetData(Sys_Cooking.Instance.showRewards[i], i);
                m_RewardCeils.Add(rewardCeil);
            }
        }

        private void OnGetReward(uint rewarId)
        {
            for (int i = 0; i < m_RewardCeils.Count; i++)
            {
                if (m_RewardCeils[i].rewardId == rewarId)
                {
                    m_RewardCeils[i].OnGetReward();
                }
            }
        }

        private void OnClickClose(BaseEventData baseEventData)
        {
            UIManager.CloseUI(EUIID.UI_Knowledge_Cooking_Award);
        }

        public class CookReward
        {
            public uint rewardId;
            private int m_RewardIndex;
            public GameObject gameObject;
            private GameObject m_RedPoint;
            private Button m_GetButton;
            private GameObject m_GetGo;
            private Transform m_ItemParent;
            private Text m_Score;
            private CSVCookBookReward.Data m_CSVCookBookRewardData;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                m_Score = gameObject.transform.Find("Image_Title/Text_Num").GetComponent<Text>();
                m_GetButton = gameObject.transform.Find("Scroll_View/Btn_04").GetComponent<Button>();
                m_GetGo = gameObject.transform.Find("Scroll_View/Image").gameObject;
                m_RedPoint = gameObject.transform.Find("Scroll_View/Image_Red").gameObject;
                m_ItemParent = gameObject.transform.Find("Scroll_View/Viewport");
                m_GetButton.onClick.AddListener(OnGetButtonClicked);
            }

            public void SetData(uint rewardId, int rewardIndex)
            {
                this.rewardId = rewardId;
                this.m_RewardIndex = rewardIndex;
                this.m_CSVCookBookRewardData = CSVCookBookReward.Instance.GetConfData(rewardId);
                Refresh();
                RefreshScore();
            }

            private void RefreshScore()
            {
                uint targetScore = CSVCookBookReward.Instance.GetConfData(rewardId).score;
                TextHelper.SetText(m_Score, string.Format($"{Sys_Cooking.Instance.curScore}/{targetScore}"));
            }

            public void Refresh()
            {
                Sys_Cooking.Instance.GetRewardState(out int state, rewardId);
                if (state == -1)
                {
                    m_RedPoint.SetActive(false);
                    m_GetButton.gameObject.SetActive(true);
                    ButtonHelper.Enable(m_GetButton, false);
                    m_GetGo.SetActive(false);
                }
                else if (state == 0)
                {
                    m_RedPoint.SetActive(true);
                    m_GetButton.gameObject.SetActive(true);
                    ButtonHelper.Enable(m_GetButton, true);
                    m_GetGo.SetActive(false);
                }
                else
                {
                    m_RedPoint.SetActive(false);
                    m_GetButton.gameObject.SetActive(false);
                    m_GetGo.SetActive(true);
                }
                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(m_CSVCookBookRewardData.reward);
                FrameworkTool.CreateChildList(m_ItemParent, itemIdCounts.Count);
                FrameworkTool.ForceRebuildLayout(m_ItemParent.gameObject);
                for (int i = 0; i < itemIdCounts.Count; i++)
                {
                    GameObject go = m_ItemParent.GetChild(i).gameObject;
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                           (_id: itemIdCounts[i].id,
                           _count: itemIdCounts[i].count,
                           _bUseQuailty: true,
                           _bBind: false,
                           _bNew: false,
                           _bUnLock: false,
                           _bSelected: false,
                           _bShowCount: true,
                           _bShowBagCount: false,
                           _bUseClick: true);
                    showItem.SetQuality(itemIdCounts[i].CSV.quality);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Knowledge_Cooking_Award, showItem));
                }
            }

            public void OnGetReward()
            {
                m_RedPoint.SetActive(false);
                m_GetButton.gameObject.SetActive(false);
                m_GetGo.SetActive(true);
            }

            private void OnGetButtonClicked()
            {
                Sys_Cooking.Instance.CookGetRewardReq(rewardId);
            }
        }
    }
}


