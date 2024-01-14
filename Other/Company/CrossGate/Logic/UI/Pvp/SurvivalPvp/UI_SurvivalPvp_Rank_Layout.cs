using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;


namespace Logic
{
    public partial class UI_SurvivalPvp_Rank_Layout
    {
        private Button m_BtnClose;

        private IListener m_Listener;

        private Transform m_TransNoInfo;
        public void Load(Transform root)
        {
            m_RankItemGroup = root.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();

            m_MyRank.Load(root.Find("Animator/MyRank"));

            m_BtnClose = root.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();

            m_TransNoInfo = root.Find("Animator/View_NoInfo");

        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_RankItemGroup.onCreateCell = OnInfinityGridCreate;
            m_RankItemGroup.onCellChange = listener.OnInfinityGridChange;

            m_MyRank.SetListener(listener);

            m_Listener = listener;
        }


        public void SetRankSize(int count)
        {
            bool show = count > 0;

            SetRankNoInfo(!show);

            if (m_RankItemGroup.Content.gameObject.activeSelf != show)
                m_RankItemGroup.Content.gameObject.SetActive(show);

            m_RankItemGroup.CellCount = count;

            m_RankItemGroup.ForceRefreshActiveCell();
            m_RankItemGroup.MoveToIndex(0);
        }

        public void SetRankNoInfo(bool active)
        {
            if (m_TransNoInfo.gameObject.activeSelf != active)
                m_TransNoInfo.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// 操作接口IListener
    /// </summary>
    public partial class UI_SurvivalPvp_Rank_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnInfinityGridChange(InfinityGridCell infinityGridCell,int index);

            void OnClickReward(int num, Vector3[] points);

        }
    }

    /// <summary>
    /// 排行榜条目
    /// </summary>
    public partial class UI_SurvivalPvp_Rank_Layout
    {
        private InfinityGrid m_RankItemGroup;

        public MyRankItem m_MyRank = new MyRankItem();
        public class RankItem : ClickItem
        {
            public Text m_TexName;
            public Text m_TexProfession;
            public Text m_TexPoint;

            public Button m_BtnReward;

            public Text m_TexRankNum;

            public Transform[] m_TransRankState = new Transform[3];

           

            private int Num = 0;

            private int RankNum = 0;

            private Vector3[] m_RewardPoint = new Vector3[4];

            private IListener m_Listener;

            public Text m_TexServerName;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexName = root.Find("Text_Name").GetComponent<Text>();
                m_TexProfession = root.Find("Text_Profession").GetComponent<Text>();
                m_TexPoint = root.Find("Text_Point").GetComponent<Text>();
                m_TexRankNum = root.Find("Rank/Text_Rank").GetComponent<Text>();

                m_BtnReward = root.Find("Button_Reward").GetComponent<Button>();

                m_TransRankState[0] = root.Find("Rank/Image_Icon");
                m_TransRankState[1] = root.Find("Rank/Image_Icon1");
                m_TransRankState[2] = root.Find("Rank/Image_Icon2");

                m_TexServerName = root.Find("Text_Server").GetComponent<Text>();
            }

            public void SetListener(IListener listener)
            {
                m_BtnReward.onClick.AddListener(OnClickReward);
                m_Listener = listener;
            }

            private void OnClickReward()
            {
                RectTransform rect = m_BtnReward.transform as RectTransform;

                rect.GetWorldCorners(m_RewardPoint);

                m_Listener.OnClickReward(RankNum, m_RewardPoint);
            }
            public void SetRankNum(int id,uint num)
            {
                Num = id;
                RankNum = (int)num;

                m_TexRankNum.text = num == 0 ? LanguageHelper.GetTextContent(10168) : num.ToString();

                m_TransRankState[0].gameObject.SetActive(num == 1);
                m_TransRankState[1].gameObject.SetActive(num == 2);
                m_TransRankState[2].gameObject.SetActive(num == 3);
            }

            public void SetName(string name)
            {
                m_TexName.text = name;
            }

            public void SetProfession(uint profession)
            {
               m_TexProfession.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(profession));
            }


            public void SetScore(uint score)
            {
                m_TexPoint.text = score.ToString();
            }

            public void SetServerName(string name)
            {
                m_TexServerName.text = name;
            }
        }



        public class MyRankItem : RankItem
        {
            private Transform m_TransNoInfo;

            private Transform m_TransInfo;
            public override void Load(Transform root)
            {
                m_TransInfo = root.Find("Info");
                base.Load(m_TransInfo);

                mTransform = root;

                m_TransNoInfo = root.Find("Text_NoInfo");
            }

            public void SetNoInfoActive(bool active)
            {
                if (m_TransNoInfo.gameObject.activeSelf != active)
                    m_TransNoInfo.gameObject.SetActive(active);

                bool isInfo = !active;

                if (m_TransInfo.gameObject.activeSelf != isInfo)
                    m_TransInfo.gameObject.SetActive(isInfo);
            }
        }
        private void OnInfinityGridCreate(InfinityGridCell infinityGridCell)
        {
            RankItem item = new RankItem();

            item.Load(infinityGridCell.mRootTransform);

            item.SetListener(m_Listener);

            infinityGridCell.BindUserData(item);
        }


    }
}
