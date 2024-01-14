using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public partial class UI_SurvivalPvp_Layout
    {
        private Button m_BtnClose;
        private Button m_BtnRank;
        private Button m_BtnMatch;

        private Text m_TexMatch;
        private Text m_TexMatchTime;

        private Text m_TexCountDown;
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();

            Transform rightView = root.Find("Animator/View_Right");

            m_BtnRank = rightView.Find("Btn_Rank").GetComponent<Button>();
            m_BtnMatch = rightView.Find("Btn_Matching").GetComponent<Button>();

            m_TexMatch = m_BtnMatch.transform.Find("Text").GetComponent<Text>();
            m_TexMatchTime = m_BtnMatch.transform.Find("matchTime").GetComponent<Text>();


            m_TexCountDown = rightView.Find("Conuntdown/Text").GetComponent<Text>();

            //队伍
            Transform memberParent = root.Find("Animator/View_Left/Scroll View/Viewport/Content");
            int memberItemcount = memberParent.childCount;
            for (int i = 0; i < memberItemcount; i++)
            {
                var item = memberParent.Find("Item" + i);
                if (item != null)
                    m_TeamMemberGroup.AddChild(item);
            }
            m_BtnAddMember = memberParent.Find("Item_Null").GetComponent<Button>();
            //
            //战斗信息

            m_TexRank = rightView.Find("Image_Title/Rank/Value").GetComponent<Text>();
            m_TexPvpTimes = rightView.Find("Image_Title/Fight/Value").GetComponent<Text>();
            m_TexWinTimes = rightView.Find("Image_Title/Victory/Value").GetComponent<Text>();
            m_TexScore = rightView.Find("Image_Title/Point/Value").GetComponent<Text>();

            m_ActiveTime = rightView.Find("Conuntdown/Text").GetComponent<Text>();

            //活动信息

            mActivityInfoGrid = rightView.Find("Info/Scroll View").GetComponent<InfinityGrid>();
            m_TogOnlySelf = rightView.Find("Info/Toggle").GetComponent<Toggle>();

            //排行奖励

            m_BtnReward = rightView.Find("Btn_Award").GetComponent<Button>();
            m_RankReward = root.Find("Animator/RankReward_Popup");

            m_BtnRewardClose = m_RankReward.Find("close").GetComponent<Button>();

            Transform rewardItem = m_RankReward.Find("Scroll View/Viewport/Content/Item");
            m_RewardGroup.SetOriginItem(rewardItem);

        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnRank.onClick.AddListener(listener.OnClickRank);
            m_BtnMatch.onClick.AddListener(listener.OnClickMatch);
            m_BtnReward.onClick.AddListener(listener.OnClickReward);
            m_BtnAddMember.onClick.AddListener(listener.OnClickAddMember);
            m_TogOnlySelf.onValueChanged.AddListener(listener.OnClickTogSelf);

            m_BtnRewardClose.onClick.AddListener(OnClickRewardClose);

            mActivityInfoGrid.onCreateCell = OnInfinityGridCreate;
            mActivityInfoGrid.onCellChange = listener.OnActivityInfoInfinityGridChange;
        }


        public void SetMatching(bool b)
        {
            m_TexMatch.text = b ? LanguageHelper.GetTextContent(10175) : LanguageHelper.GetTextContent(10153);

            if (m_TexMatchTime.gameObject.activeSelf != b)
                m_TexMatchTime.gameObject.SetActive(b);
        }

        public void SetMatchTime(float time)
        {
            int min = (int)(time / 60);
            int sec = (int)(time - min * 60);

            m_TexMatchTime.text = GetTimeMinAndSecString(min) + ":" + GetTimeMinAndSecString(sec);
        }

        private string GetTimeMinAndSecString(int time)
        {
            string minstr = time == 0 ? ("00") : (time < 10 ? ("0" + time.ToString()) : time.ToString());

            return minstr;
        }

        public void SetOnlySelf(bool b)
        {
            m_TogOnlySelf.isOn = b;
        }

        public void SetTimeCountDown(string time)
        {
            m_TexCountDown.text = time;
        }
    }

    /// <summary>
    /// 操作接口IListener
    /// </summary>
    public partial class UI_SurvivalPvp_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnClickRank();
            void OnClickMatch();
            void OnClickReward();
            void OnClickAddMember();

            void OnClickTogSelf(bool state);

            void OnActivityInfoInfinityGridChange(InfinityGridCell infinityGridCell, int index);
        }
    }

    /// <summary>
    /// 队伍信息
    /// </summary>
    public partial class UI_SurvivalPvp_Layout
    {
        private ClickItemGroup<TeamMemberItem> m_TeamMemberGroup = new ClickItemGroup<TeamMemberItem>() { AutoClone = false };
        private Button m_BtnAddMember;
        class TeamMemberItem : ClickItem
        {
            public Image m_ImaHead;
            public Text m_TexName;

            public Image m_ImaProfession;
            public Text m_TexProfession;

            public Text m_TexScore;

            public ulong RoleID;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_ImaHead = root.Find("Image_Head/Image_Frame").GetComponent<Image>();
                m_TexName = root.Find("Text_Name").GetComponent<Text>();

                m_ImaProfession = root.Find("Profession_bg/Image_Profession").GetComponent<Image>();
                m_TexProfession = root.Find("Profession_bg/Text_Profession").GetComponent<Text>();

                m_TexScore = root.Find("Point/Text_Point/Text_Num").GetComponent<Text>();
            }

            public override ClickItem Clone()
            {
                return Clone<TeamMemberItem>(this);
            }
        }

        public void SetMemberCount(int count)
        {
            m_TeamMemberGroup.SetChildSize(count);
        }
        public void SetMemberHead(int index, uint headID)
        {
            var item = m_TeamMemberGroup.getAt(index);

            if (item == null)
                return;

            ImageHelper.SetIcon(item.m_ImaHead, headID);
        }


        public void SetMemberName(int index, string name)
        {
            var item = m_TeamMemberGroup.getAt(index);

            if (item == null)
                return;

            item.m_TexName.text = name;
        }

        public void SetMemberProfession(int index, uint professionID)
        {
            var item = m_TeamMemberGroup.getAt(index);

            if (item == null)
                return;

            ImageHelper.SetIcon(item.m_ImaProfession, OccupationHelper.GetIconID(professionID));

            item.m_TexProfession.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(professionID));
        }

        public void SetMemberRoleID(int index, ulong roleid)
        {
            var item = m_TeamMemberGroup.getAt(index);

            if (item == null)
                return;

            item.RoleID = roleid;
        }
        public void SetMemberScore(int index, uint score)
        {
            var item = m_TeamMemberGroup.getAt(index);

            if (item == null)
                return;

            item.m_TexScore.text = score.ToString();
        }

        public void SetMemberScore(ulong roleid, uint score)
        {
            int count = m_TeamMemberGroup.Count;

            for (int i = 0; i < count; i++)
            {
                var item = m_TeamMemberGroup.getAt(i);

                if (item != null && item.RoleID == roleid)
                {
                    item.m_TexScore.text = score.ToString();
                    continue;
                }
             
            }

        }

        public void SetMemberAddActive(bool bActive)
        {
            if (m_BtnAddMember.gameObject.activeSelf != bActive)
                m_BtnAddMember.gameObject.SetActive(bActive);
        }
    }

    /// <summary>
    /// 活动信息
    /// </summary>
    public partial class UI_SurvivalPvp_Layout
    {
        private InfinityGrid mActivityInfoGrid;
        private Toggle m_TogOnlySelf;

        public class ActivityInfoItem : ClickItem
        {
            private Text m_Text;
            public override void Load(Transform root)
            {
                base.Load(root);
                m_Text = root.GetComponent<Text>();
            }

            public void SetText(string tex)
            {
                m_Text.text = tex;
            }
        }
        public void SetActivityInfo(int count)
        {
            SetActivityInfoCount(count);

            mActivityInfoGrid.ForceRefreshActiveCell();

        }

        public void SetActivityInfoCount(int count)
        {
            mActivityInfoGrid.CellCount = count;
        }


        private void OnInfinityGridCreate(InfinityGridCell infinityGridCell)
        {
            ActivityInfoItem item = new ActivityInfoItem();

            item.Load(infinityGridCell.mRootTransform);

            infinityGridCell.BindUserData(item);
        }
    }

    /// <summary>
    /// 战斗相关信息
    /// </summary>
    public partial class UI_SurvivalPvp_Layout
    {
        private Text m_TexRank;
        private Text m_TexPvpTimes;
        private Text m_TexWinTimes;
        private Text m_TexScore;

        private Text m_ActiveTime;
        public void SetRankTimes(int times)
        {
            m_TexRank.text = times <= 0 ? LanguageHelper.GetTextContent(2022103) : times.ToString();
        }

        public void SetPvpTimes(int times)
        {
            m_TexPvpTimes.text = times.ToString();
        }

        public void SetWinTimes(int times)
        {
            m_TexWinTimes.text = times.ToString();
        }

        public void SetScore(int score)
        {
            m_TexScore.text = score.ToString();
        }
    }

    /// <summary>
    /// 排行奖励
    /// </summary>
    public partial class UI_SurvivalPvp_Layout
    {
        private Transform m_RankReward;
        private Button m_BtnReward;

        private Button m_BtnRewardClose;

        ClickItemGroup<RewardItem> m_RewardGroup = new ClickItemGroup<RewardItem>();
        public void SetRewardActive(bool bActive)
        {
            if (m_RankReward.gameObject.activeSelf != bActive)
                m_RankReward.gameObject.SetActive(bActive);
        }

        private void OnClickRewardClose()
        {
            SetRewardActive(false);
        }

        public void SetRewardCount(int count)
        {
            m_RewardGroup.SetChildSize(count);
        }

        public void SetRewardList(int index, List<ItemIdCount> rewardlist)
        {
            var item = m_RewardGroup.getAt(index);

            if (item == null)
                return;

            item.SetReward(rewardlist);
        }

        public void SetRewardTitle(int index, string title)
        {
            var item = m_RewardGroup.getAt(index);

            if (item == null)
                return;

            item.SetTitle(title);
        }
    }

    public partial class UI_SurvivalPvp_Layout
    {
        class RewardItem : ClickItem
        {
            public Text m_TexTitle;

            public ClickItemGroup<RewardChildItem> m_ChildGroup = new ClickItemGroup<RewardChildItem>();

            public VerticalLayoutGroup m_LayoutGroup;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexTitle = root.Find("Title/Text").GetComponent<Text>();

                m_ChildGroup.SetOriginItem(root.Find("Grid/PropItem"));

                m_LayoutGroup = root.GetComponent<VerticalLayoutGroup>();
            }

            public override ClickItem Clone()
            {
                return Clone<RewardItem>(this);
            }

            public void SetReward(List<ItemIdCount> rewardlist)
            {
                int count = rewardlist.Count;

                m_ChildGroup.SetChildSize(count);

                for (int i = 0; i < count; i++)
                {
                    var item = m_ChildGroup.getAt(i);

                    if (item != null)
                    {
                        item.SetReward(rewardlist[i].id, (uint)rewardlist[i].count);
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(mTransform as RectTransform);
            }

            public void SetTitle(string title)
            {
                m_TexTitle.text = title;
            }
        }


        class RewardChildItem : ClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public override ClickItem Clone()
            {
                return Clone<RewardChildItem>(this);
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_SurvivalPvp, m_ItemData));

                var data = CSVItem.Instance.GetConfData(id);
                if (data != null)
                    m_Item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
            }

        }
    }
}
