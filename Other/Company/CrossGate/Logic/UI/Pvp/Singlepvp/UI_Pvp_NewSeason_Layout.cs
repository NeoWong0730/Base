using Framework;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///pvp 新赛季
    /// </summary>
    public partial class UI_Pvp_NewSeason_Layout
    {
        ClickItemGroup<RectPropItem> m_Group = new ClickItemGroup<RectPropItem>() { AutoClone = false };

        private PlayableDirector m_NormalAnim;
        private PlayableDirector m_OpenAnim;
        private PlayableDirector m_RewardAnim;

        private Transform m_TransSeasonRank;
        private Transform m_TransSeasonReward;
        private Transform m_TransStart;

        private TouchInput m_TouchInput;

        private Button m_RewardClose;
        private Button m_RankClose;


        private Text m_TexRankTitle;
        private Text m_TexRankText;
        private Transform m_TransRankIcon;
        private Transform[] m_ATransRankStar = new Transform[5];


        private AnimationEndTrigger m_RankAnmEndTrigger;
        private Animator m_RankAnimator;

        private IListener m_Listener;

        UIPvpLevelIcon m_LevelIcon = new UIPvpLevelIcon();

        private Text m_TexNewSeasonTips;

        private Text m_TexRewardTitle;
        public void Load(Transform root)
        {
            Transform groupTrans = root.Find("Animator/SeasonReward/Grid");

            int itemcount = groupTrans.childCount;

            for (int i = 0; i < itemcount; i++)
            {
                string strname = string.Format("PropItem ({0})", i + 1);

                var transItem = groupTrans.Find(strname);
                if (transItem != null)
                    m_Group.AddChild(transItem);
            }


            m_NormalAnim = root.Find("Animator/Timeline/Nomal").GetComponent<PlayableDirector>();
            m_OpenAnim = root.Find("Animator/Timeline/Open").GetComponent<PlayableDirector>();
            m_RewardAnim = root.Find("Animator/Timeline/Reward").GetComponent<PlayableDirector>();


            m_TransStart = root.Find("Animator/Animator");
            m_TransSeasonRank = root.Find("Animator/SeasonRank");
            m_TransSeasonReward = root.Find("Animator/SeasonReward");

            m_TexRewardTitle = m_TransSeasonReward.Find("Image_Title/Text_Title").GetComponent<Text>();

            m_TouchInput = m_TransStart.Find("Image_bg").GetComponent<TouchInput>();

            m_RewardClose = m_TransSeasonReward.Find("Image_BG").GetComponent<Button>();
            m_RankClose = m_TransSeasonRank.Find("bg").GetComponent<Button>();


            m_TexRankTitle = m_TransSeasonRank.Find("Text_Title").GetComponent<Text>();
            m_TexRankText = m_TransSeasonRank.Find("Image_Rank/Text_level").GetComponent<Text>();
            m_TransRankIcon = m_TransSeasonRank.Find("Image");

            m_LevelIcon.Parent = m_TransRankIcon;

            Transform rankStarGrid = m_TransSeasonRank.Find("Grid_Star");
            int starcount = rankStarGrid.childCount;
            for (int i = 0; i < starcount; i++)
            {
                string strname = i == 0 ? "Image" : string.Format("Image ({0})", i);
                var item = rankStarGrid.Find(strname);
                if (item != null)
                    m_ATransRankStar[i] = item;
            }

            m_RankAnmEndTrigger = m_TransSeasonRank.GetComponent<AnimationEndTrigger>();

            m_RankAnimator = m_TransSeasonRank.GetComponent<Animator>();


            m_TexNewSeasonTips = m_TransSeasonRank.Find("Text_levelTips").GetComponent<Text>();
        }


        public void SetListener(IListener listener)
        {
            m_OpenAnim.stopped += listener.OnOpenEnd;
            m_RewardAnim.stopped += listener.OnRewardEnd;

            m_TouchInput.SendTouchUp = listener.OnStartStateTouch;

            m_TouchInput.SendTouchLongPress = listener.OnStartStateLongTouch;

            m_RewardClose.onClick.AddListener(listener.OnClickRewardClose);
            m_RankClose.onClick.AddListener(listener.OnClickRankClose);

            m_RankAnmEndTrigger.onAnimationEnd += listener.RankAnimationEnd;


        }

        public void OnDestory()
        {
            m_LevelIcon.Destory();
        }
        public void PlayNormal()
        {
            m_NormalAnim.Play();
        }

        public void CloseNormal()
        {
            m_NormalAnim.Stop();
        }
        public void PlayOpen()
        {
            m_OpenAnim.Play();
        }

        public void PlayReward()
        {
            m_RewardAnim.Play();
        }

        
        public void PlayRankAnimator(string name)
        {
     

            m_RankAnimator.Play(name);
        }
        public void SetStarActive(bool b)
        {
            if (m_TransStart.gameObject.activeSelf != b)
                m_TransStart.gameObject.SetActive(b);
        }
        public void SetRewardActive(bool b)
        {
            if (m_TransSeasonReward.gameObject.activeSelf != b)
                m_TransSeasonReward.gameObject.SetActive(b);
        }

        public void SetRankActive(bool b)
        {
            if (m_TransSeasonRank.gameObject.activeSelf != b)
                m_TransSeasonRank.gameObject.SetActive(b);
        }

        public void SetReward(IList<uint> ids, IList<uint> count)
        {
            int idscount = ids.Count;

            m_Group.SetChildSize(idscount);

            for (int i = 0; i < idscount; i++)
            {
                var item = m_Group.getAt(i);

                if (item != null)
                {
                    item.SetReward(ids[i], count[i]);
                }
            }
        }

        public void SetRankTitle(string tex)
        {
            m_TexRankTitle.text = tex;
        }

        public void SetRankIcon(string icon)
        {
            m_LevelIcon.ShowLevelIcon(icon);
        }

        public void SetRankDanLv(string tex)
        {
            m_TexRankText.text = tex;
        }

        public void SetRankStar(int num)
        {
            for (int i = 0; i < 5; i++)
            {
                var item = m_ATransRankStar[i];

                if (item != null)
                {
                    item.gameObject.SetActive(i < num);
                }
            }
        }

        public void SetRankNewSeasonTips(bool state,string tex)
        {
            m_TexNewSeasonTips.gameObject.SetActive(state);
            m_TexNewSeasonTips.text = tex;
        }

        public void SetRewardTitle(string tex)
        {
            m_TexRewardTitle.text = tex;
        }
    }

    public partial class UI_Pvp_NewSeason_Layout
    {
        class RectPropItem : IntClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }


            public void SetReward(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_SingleNewSeason, m_ItemData));
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_SingleNewSeason, m_ItemData));
            }
        }
    }
    public partial class UI_Pvp_NewSeason_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnStartStateTouch(Vector3 pos);

            void OnStartStateLongTouch(Vector3 pos);
            void OnOpenEnd(PlayableDirector playable);
            void OnRewardEnd(PlayableDirector playable);
            void OnClickRewardClose();

            void OnClickRankClose();

            void RankAnimationEnd(string name);
        }
    }


}
