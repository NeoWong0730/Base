using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    /// 首通
    /// </summary>
    public partial class UI_Multi_InfoNew_Layout
    {
        class FristCrossPlayerItem : ClickItem
        {
            private Text m_TexName;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexName = root.Find("Text").GetComponent<Text>();
            }

            public void SetName(string name)
            {
                m_TexName.text = name;
            }
        }

        ClickItemGroup<FristCrossPlayerItem> m_FristCrossGroup = new ClickItemGroup<FristCrossPlayerItem>() { AutoClone = false };

        public void SetFristPlayerCount(int count)
        {
            m_FristCrossGroup.SetChildSize(count);
        }
        public void SetFristPlayerName(int index, string name)
        {
            var item = m_FristCrossGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(name);
        }
    }

    /// <summary>
    /// 奖励common
    /// </summary>
    public partial class UI_Multi_InfoNew_Layout
    {
        class RewardItem : ClickItem
        {
            PropItem m_Item;
            public PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Multi_Info, m_ItemData));

                var data = CSVItem.Instance.GetConfData(id);
                if (data != null)
                    m_Item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
            }
        }


        class RewardGroup
        {
            public InfinityGrid m_RewardGroup { get; set; }

            public void OnInfinityGridCreate(InfinityGridCell infinityGridCell)
            {
                RewardItem item = new RewardItem();

                item.Load(infinityGridCell.mRootTransform);

                infinityGridCell.BindUserData(item);
            }
            public void SetRewardCount(int count)
            {
                bool show = count > 0;

                if (m_RewardGroup.Content.gameObject.activeSelf != show)
                    m_RewardGroup.Content.gameObject.SetActive(show);

                m_RewardGroup.CellCount = count;

                m_RewardGroup.ForceRefreshActiveCell();
                m_RewardGroup.MoveToIndex(0);
            }
        }

    }

    /// <summary>
    /// 首通奖励
    /// </summary>
    public partial class UI_Multi_InfoNew_Layout
    {
        public void SetFristReward(List<ItemIdCount> reward)
        {
            int count = reward.Count;

            m_FristRewardGroup.SetRewardCount(count);
        }

        public void SetFristReward(InfinityGridCell cell, ItemIdCount item)
        {
            var uiItem = cell.mUserData as RewardItem;

            if (uiItem == null)
                return;

            uiItem.SetReward(item.id, (uint)item.count);
        }
    }

    /// <summary>
    /// 可获得奖励 
    /// </summary>
    public partial class UI_Multi_InfoNew_Layout
    {
        public void SetCanGetReward(List<ItemIdCount> reward)
        {
            int count = reward.Count;
            m_CanGetRewardGroup.SetRewardCount(count);
        }

        public void SetCanGetReward(InfinityGridCell cell, ItemIdCount item)
        {
            var uiItem = cell.mUserData as RewardItem;

            if (uiItem == null)
                return;

            uiItem.m_ItemData.bShowCount = false;

            uiItem.SetReward(item.id, (uint)item.count);
        }
    }

    public partial class UI_Multi_InfoNew_Layout
    {
        public void SetInstanceSeriesName(uint langueid)
        {
            TextHelper.SetText(m_TexInstanceName, langueid);
        }

        public void SetInstanceName(uint langueid)
        {
            TextHelper.SetText(m_TexCharpterName, langueid);
        }

        public void SetInstanceDescribe(uint langueid)
        {
            TextHelper.SetText(m_TexCharpterDescribe, langueid);
        }

        public void SetInstanceImage(string path)
        {
            ImageHelper.SetTexture(m_ImgCharpter, path);
        }
        public void SetCrossTimes(uint langueid, uint curTimes, uint maxTimes)
        {
            TextHelper.SetText(m_TexCrossNum, langueid, curTimes.ToString(), maxTimes.ToString());
        }

        public void SetFristCrossDate(string date)
        {
            TextHelper.SetText(m_TexFristCrossDate, date);
        }
    }
    public partial class UI_Multi_InfoNew_Layout
    {
        private Button mBtnGoOn;
        private Button mBtnClose;
        private Button m_BtnFastTeam;

        private IListener m_Listener;

        private Button m_BtnRewardView;
        private Button m_BtnCrossDetail;

        RewardGroup m_FristRewardGroup = new RewardGroup();
        RewardGroup m_CanGetRewardGroup = new RewardGroup();

        private Text m_TexCrossNum;

        private Text m_TexFristCrossDate;

        private Text m_TexInstanceName;
        private Text m_TexCharpterName;

        private RawImage m_ImgCharpter;
        private Text m_TexCharpterDescribe;

        private Transform m_TransRule;
        private Button m_BtnRuleClose;

        private Toggle m_TogSkipTalk;
        public void Load(Transform root)
        {
            m_FristRewardGroup.m_RewardGroup = root.Find("Animator/View_Content/Content/Scroll View0").GetComponent<InfinityGrid>();

            m_CanGetRewardGroup.m_RewardGroup = root.Find("Animator/View_Content/Content/Scroll View1").GetComponent<InfinityGrid>();

            m_BtnFastTeam = root.Find("Animator/View_Content/Btn_01").GetComponent<Button>();
            mBtnGoOn = root.Find("Animator/View_Content/Btn_02").GetComponent<Button>();
            m_BtnRewardView = root.Find("Animator/View_Content/Button_View").GetComponent<Button>();
            m_BtnCrossDetail = root.Find("Animator/View_Content/Button_Detil").GetComponent<Button>();

            mBtnClose = root.Find("Animator/Image_Bg/Close").GetComponent<Button>();

            m_TexCrossNum = root.Find("Animator/View_Content/Text_Num").GetComponent<Text>();

            m_TexFristCrossDate = root.Find("Animator/View_Content/Content/Title0/Date").GetComponent<Text>();

            m_TexInstanceName = root.Find("Animator/Left/Text_Title").GetComponent<Text>();
            m_TexCharpterName = root.Find("Animator/Left/Text_Name").GetComponent<Text>();
            m_ImgCharpter = root.Find("Animator/Left/bg_Icon/Icon").GetComponent<RawImage>();
            m_TexCharpterDescribe = root.Find("Animator/Left/Text_Describe").GetComponent<Text>();


            Transform players = root.Find("Animator/View_Content/Content/Players");

            int playersCount = players.childCount;

            for (int i = 0; i < playersCount; i++)
            {
                var child = players.Find("Image" + i.ToString());
                if (child != null)
                    m_FristCrossGroup.AddChild(child);
            }


            m_TransRule = root.Find("Animator/UI_Rule1");
            m_BtnRuleClose = root.Find("Animator/UI_Rule1/Black").GetComponent<Button>();

            m_TogSkipTalk = root.Find("Animator/Left/Toggle").GetComponent<Toggle>();
        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnFastTeam.onClick.AddListener(listener.OnClickFastTeam);
            mBtnGoOn.onClick.AddListener(listener.OnClickGoOn);
            mBtnClose.onClick.AddListener(listener.OnClickClose);

            m_FristRewardGroup.m_RewardGroup.onCellChange = listener.OnFristRewardInfinityGridChange;
            m_CanGetRewardGroup.m_RewardGroup.onCellChange = listener.OnRewardInfinityGridChange;

            m_FristRewardGroup.m_RewardGroup.onCreateCell = m_FristRewardGroup.OnInfinityGridCreate;
            m_CanGetRewardGroup.m_RewardGroup.onCreateCell = m_CanGetRewardGroup.OnInfinityGridCreate;

            m_BtnCrossDetail.onClick.AddListener(listener.OnClickCrossDetail);
            m_BtnRewardView.onClick.AddListener(listener.OnClickRewardView);
            m_BtnRuleClose.onClick.AddListener(listener.OnClickRuleClose);

            m_TogSkipTalk.onValueChanged.AddListener(listener.OnClickTogSkipTalk);
        }

        public void SetRuleActive(bool active)
        {
            m_TransRule.gameObject.SetActive(active);
        }

        public void SetSkipTalk(bool active, bool istoggle)
        {
            m_TogSkipTalk.gameObject.SetActive(active);
            m_TogSkipTalk.isOn = istoggle;
        }
        public interface IListener
        {

            void OnClickGoOn();

            void OnClickClose();

            void OnClickFastTeam();

            void OnFristRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index);
            void OnRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index);

            void OnClickRewardView();
            void OnClickCrossDetail();

            void OnClickRuleClose();

            void OnClickTogSkipTalk(bool state);

        }
    }
}
