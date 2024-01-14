using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;

namespace Logic
{
    public partial class UI_Goddness_Rank_Layout
    {
        class SelectElement : ClickItem
        {
            private Text m_TNormal;
            private Text m_TSelect;

            private CP_ToggleEx m_TgToggle;
            public uint ID { get; set; }

            private IListener mListtener;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TgToggle = root.GetComponent<CP_ToggleEx>();

                m_TNormal = root.Find("Text_Dark").GetComponent<Text>();
                m_TSelect = root.Find("Image_Select/Text_Light").GetComponent<Text>();

                m_TgToggle.onValueChanged.AddListener(OnClickToggle);
            }

            public override ClickItem Clone()
            {
                return Clone<SelectElement>(this);
            }
            private void OnClickToggle(bool stage)
            {
                if(stage)
                mListtener?.OnClickDiffic(ID);
            }
            public void SetTextName(string name)
            {
                TextHelper.SetText(m_TNormal, name);
                TextHelper.SetText(m_TSelect, name);
            }

            public void SetListener(IListener listener)
            {
                mListtener = listener;
            }

            public void SetFocus(bool focus, bool sendmsg)
            {
                m_TgToggle.SetSelected(focus, sendmsg);
            }
        }
    }
    public partial class UI_Goddness_Rank_Layout
    {
        class SelectContent : ClickItem
        {
            ClickItemGroup<SelectElement> m_ElemengGroup = new ClickItemGroup<SelectElement>();

            private Transform m_TransSmallContant;

            private Transform m_TransSmallTemplete;

            private Text m_TNormal;
            private Text m_TSelect;

            IListener mListener;

            CP_Toggle m_TogToggle;

            public uint ID { get; set; }
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TogToggle = root.GetComponent<CP_Toggle>();

                m_TNormal = root.Find("GameObject/Text_Dark").GetComponent<Text>();
                m_TSelect = root.Find("GameObject/Image_Select/Text_Light").GetComponent<Text>();

                m_TransSmallContant = root.Find("Content_Small"); ;

                m_TransSmallTemplete = root.Find("Content_Small/Toggle_Select01");
                m_ElemengGroup.SetOriginItem(m_TransSmallTemplete);

                m_TogToggle.onValueChanged.AddListener(OnClickToggle);
            }

            private void OnAddElement(SelectElement item)
            {
               // item.mTransform.SetParent(m_TransSmallContant, false);

                
                item.SetListener(mListener);
            }

            public override ClickItem Clone()
            {
                return Clone<SelectContent>(this);
            }
            private void OnClickToggle(bool state)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(mTransform as RectTransform);

                if (state)
                    mListener?.OnClickInstance(ID);

               
            }
            public void SeListener(IListener listener)
            {
                mListener = listener;

                m_ElemengGroup.SetAddChildListenter(OnAddElement);
            }

            public void SetName(string name)
            {
                m_TNormal.text = name;
                m_TSelect.text = name;
            }

            public void SetChildCount(int count)
            {
                m_ElemengGroup.SetChildSize(count);
            }

            public void SetChildName(int index, string name)
            {
                var item = m_ElemengGroup.getAt(index);
                if (item == null)
                    return;

                item.SetTextName(name);
            }

            public void SetChildID(int index, uint id)
            {
                var item = m_ElemengGroup.getAt(index);
                if (item == null)
                    return;

                item.ID = id;
            }

            public void SetFocus(bool focus,bool sendmsg)
            {
                m_TogToggle.SetSelected(focus, sendmsg);
            }

            public void FocusChild(int index, bool sendmsg)
            {
                var item = m_ElemengGroup.getAt(index);
                if (item == null)
                    return;

                item.SetFocus(true, sendmsg);
            }
        }
    }


    public partial class UI_Goddness_Rank_Layout
    {

        private ClickItemGroup<SelectContent> m_SelectGroup = new ClickItemGroup<SelectContent>();

        private Transform m_TransContent;

        private VerticalLayoutGroup m_LeftContentLayout;
        private void LoadLeftContent(Transform root)
        {
            m_TransContent = root;

            var selectitem = root.Find("Toggle_Select01");

            m_SelectGroup.SetOriginItem(selectitem);

            m_LeftContentLayout = root.GetComponent<VerticalLayoutGroup>();

            m_SelectGroup.SetAddChildListenter(OnAddSelectContent);
        }

        private void OnAddSelectContent(SelectContent item)
        {
            item.SeListener(mListener);
        }
        public void SetTopicCount(int count)
        {
            m_SelectGroup.SetChildSize(count);

            //m_LeftContentLayout.SetLayoutVertical();

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_TransContent as RectTransform);
        }


        public void RebuildContent()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_TransContent as RectTransform);
        }

        public void FocusContent(int index,bool sendmsg)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            item.SetFocus(true, sendmsg);
        }
        public void SetTopicName(int index, uint name)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            string value = name == 0 ? string.Empty : LanguageHelper.GetTextContent(name);

            item.SetName(value);
        }
        public void SetTopicID(int index, uint id)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            item.ID = id;
        }
        public void SetTopicChildCount(int index,int count)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            item.SetChildCount(count);
        }

        public void SetTopicChildName(int index, int childIndex, string name)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            item.SetChildName(childIndex, name);
        }

        public void SetTopicChildID(int index, int childIndex, uint id)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            item.SetChildID(childIndex, id);
        }

        public void FocusContentChild(int index, int childIndex, bool sendmsg)
        {
            var item = m_SelectGroup.getAt(index);
            if (item == null)
                return;

            item.FocusChild(childIndex, sendmsg);
        }
    }

    public partial class UI_Goddness_Rank_Layout
    {
        class RankItem : ClickItem
        {
            private Text m_TNumber;
            private Text m_TName;
            private Text m_TScore;
            private Text m_TData;

            private Button m_BtnDetail;

            private IListener mListener;

            public int Index;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TNumber = root.Find("Text_Number").GetComponent<Text>();
                m_TName = root.Find("Text01").GetComponent<Text>();
                m_TScore = root.Find("Text02").GetComponent<Text>();
                m_TData = root.Find("Text03").GetComponent<Text>();

                m_BtnDetail = root.Find("Btn_Detail").GetComponent<Button>();

                m_BtnDetail.gameObject.SetActive(true);
            }

            public void Set(uint number, string name, uint score, uint date,bool showDetailBtn)
            {
                m_TNumber.text = number.ToString();
                m_TName.text = name;
                m_TScore.text = score.ToString();

                DateTime time = TimeManager.GetDateTime(date);

                string hourstr = time.Hour.ToString();
                if (time.Hour < 10)
                    hourstr = "0" + hourstr;

                string minutestr = time.Minute.ToString();
                if (time.Minute < 10)
                    minutestr = "0" + minutestr;

                m_TData.text = string.Format("{0}/{1}/{2}  {3}:{4}", time.Year.ToString(), time.Month.ToString(), time.Day.ToString(), hourstr,minutestr);

                m_BtnDetail.gameObject.SetActive(showDetailBtn);
            }

            public override ClickItem Clone()
            {
                return Clone<RankItem>(this);
            }

            public void SetListener(IListener listener)
            {
                mListener = listener;

                m_BtnDetail.onClick.AddListener(OnClickBtnDetail);
            }

            private void OnClickBtnDetail()
            {
                mListener.OnClickRankDetail(Index);
            }
        }

        private ClickItemGroup<RankItem> m_GroupRankItem = new ClickItemGroup<RankItem>();

        public void SetRankSize(int count)
        {
            m_GroupRankItem.SetChildSize(count);
        }

        public void SetRankItem(int index, uint number, string name, uint score, uint date,bool showDetailBtn)
        {
            var item = m_GroupRankItem.getAt(index);

            if (item == null)
                return;
            item.Index = index;
            item.Set(number, name, score, date, showDetailBtn);
        }
    }
    public partial class UI_Goddness_Rank_Layout
    {
        private IListener mListener;

        private Button m_BtnClose;

       
        public void Load(Transform root)
        {
           

            LoadLeftContent(root.Find("Animator/View_Left/Scroll01/Content"));

            var rankitem = root.Find("Animator/View_Right/Scroll_List/Grid/Item01");

            m_GroupRankItem.SetOriginItem(rankitem);

            m_BtnClose = root.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
        }

        public void SetListener(IListener listener)
        {
            mListener = listener;

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_GroupRankItem.SetAddChildListenter(OnAddRankItem);
        }

        private void OnAddRankItem(RankItem item)
        {
            item.SetListener(mListener);
        }
    }
    public partial class UI_Goddness_Rank_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnClickDiffic(uint id);

            void OnClickInstance(uint id);

            void OnClickRankDetail(int index);

        }
    }
}
